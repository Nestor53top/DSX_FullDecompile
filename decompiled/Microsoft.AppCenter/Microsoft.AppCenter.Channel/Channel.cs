using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Synchronization;

namespace Microsoft.AppCenter.Channel;

public sealed class Channel : IChannelUnit, IChannel, IDisposable
{
	private const int ClearBatchSize = 100;

	private Microsoft.AppCenter.Ingestion.Models.Device _device;

	private readonly string _appSecret;

	private readonly IStorage _storage;

	private readonly IIngestion _ingestion;

	private readonly IDeviceInformationHelper _deviceInfoHelper = new DeviceInformationHelper();

	private readonly IDictionary<string, List<Log>> _sendingBatches = new Dictionary<string, List<Log>>();

	private readonly ISet<IServiceCall> _calls = new HashSet<IServiceCall>();

	private readonly int _maxParallelBatches;

	private readonly int _maxLogsPerBatch;

	private long _pendingLogCount;

	private bool _enabled;

	private bool _discardLogs;

	private bool _batchScheduled;

	private TimeSpan _batchTimeInterval;

	private readonly StatefulMutex _mutex = new StatefulMutex();

	public bool IsEnabled
	{
		get
		{
			using (_mutex.GetLock())
			{
				return _enabled;
			}
		}
	}

	public string Name { get; }

	public event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;

	public event EventHandler<FilteringLogEventArgs> FilteringLog;

	public event EventHandler<SendingLogEventArgs> SendingLog;

	public event EventHandler<SentLogEventArgs> SentLog;

	public event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

	internal Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches, string appSecret, IIngestion ingestion, IStorage storage)
	{
		Name = name;
		_maxParallelBatches = maxParallelBatches;
		_maxLogsPerBatch = maxLogsPerBatch;
		_appSecret = appSecret;
		_ingestion = ingestion;
		_storage = storage;
		_batchTimeInterval = batchTimeInterval;
		_batchScheduled = false;
		_enabled = true;
		AbstractDeviceInformationHelper.InformationInvalidated += delegate
		{
			InvalidateDeviceCache();
		};
		StatefulMutex.LockHolder lockHolder = _mutex.GetLock();
		Task.Run(() => _storage.CountLogsAsync(Name)).ContinueWith(delegate(Task<int> task)
		{
			if (!task.IsFaulted && !task.IsCanceled)
			{
				_pendingLogCount = task.Result;
			}
			lockHolder.Dispose();
			CheckPendingLogs(_mutex.State);
		});
	}

	public void SetEnabled(bool enabled)
	{
		State state;
		using (_mutex.GetLock())
		{
			if (_enabled == enabled)
			{
				return;
			}
			state = _mutex.State;
		}
		if (enabled)
		{
			Resume(state);
		}
		else
		{
			Suspend(state, deleteLogs: true, new CancellationException());
		}
	}

	public async Task EnqueueAsync(Log log)
	{
		_ = 2;
		try
		{
			State state;
			bool discardLogs;
			using (await _mutex.GetLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				state = _mutex.State;
				discardLogs = _discardLogs;
			}
			if (discardLogs)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Channel is disabled; logs are discarded");
				AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke SendingLog event for channel '" + Name + "'");
				this.SendingLog?.Invoke(this, new SendingLogEventArgs(log));
				AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke FailedToSendLog event for channel '" + Name + "'");
				this.FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
				return;
			}
			AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke EnqueuingLog event for channel '" + Name + "'");
			this.EnqueuingLog?.Invoke(this, new EnqueuingLogEventArgs(log));
			await PrepareLogAsync(log, state).ConfigureAwait(continueOnCapturedContext: false);
			AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke FilteringLog event for channel '" + Name + "'");
			FilteringLogEventArgs e = new FilteringLogEventArgs(log);
			this.FilteringLog?.Invoke(this, e);
			if (e.FilterRequested)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, $"Filtering out a log of type '{log.GetType()}' at the request of an event handler.");
			}
			else
			{
				await PersistLogAsync(log, state).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "The Enqueue operation has been canceled");
		}
	}

	private async Task PrepareLogAsync(Log log, State state)
	{
		if (log.Device == null && _device == null)
		{
			Microsoft.AppCenter.Ingestion.Models.Device device = await _deviceInfoHelper.GetDeviceInformationAsync().ConfigureAwait(continueOnCapturedContext: false);
			using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
			{
				_device = device;
			}
		}
		log.Device = log.Device ?? _device;
		log.Timestamp = log.Timestamp ?? DateTime.Now;
	}

	private async Task PersistLogAsync(Log log, State state)
	{
		try
		{
			await _storage.PutLog(Name, log).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (StorageException exception)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Error persisting log", exception);
			return;
		}
		try
		{
			bool enabled;
			using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
			{
				_pendingLogCount++;
				enabled = _enabled;
			}
			if (enabled)
			{
				CheckPendingLogs(state);
			}
			else
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Channel is temporarily disabled; log was saved to disk");
			}
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "The PersistLog operation has been canceled");
		}
	}

	public void InvalidateDeviceCache()
	{
		using (_mutex.GetLock())
		{
			_device = null;
		}
	}

	public async Task ClearAsync()
	{
		State state = _mutex.State;
		try
		{
			await _storage.DeleteLogs(Name).ConfigureAwait(continueOnCapturedContext: false);
			using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
			{
				_pendingLogCount = 0L;
			}
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "The Clear operation has been canceled");
		}
		catch (StorageException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not delete logs with error: ", exception);
		}
	}

	private void Resume(State state, bool needEnableChannel = true)
	{
		AppCenterLog.Debug(AppCenterLog.LogTag, "Resume channel: '" + Name + "'");
		try
		{
			using (_mutex.GetLock(state))
			{
				if (needEnableChannel)
				{
					_enabled = true;
				}
				_discardLogs = false;
				state = _mutex.InvalidateState();
			}
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "The resume operation has been canceled");
		}
		CheckPendingLogs(state);
	}

	private void Suspend(State state, bool deleteLogs, Exception exception, bool needDisableChannel = true)
	{
		AppCenterLog.Debug(AppCenterLog.LogTag, "Suspend channel: '" + Name + "'");
		try
		{
			IList<string> sendingBatches = null;
			IList<Log> list = null;
			using (_mutex.GetLock(state))
			{
				if (needDisableChannel)
				{
					_enabled = false;
				}
				_batchScheduled = false;
				_discardLogs = deleteLogs;
				if (deleteLogs)
				{
					sendingBatches = _sendingBatches.Keys.ToList();
					list = _sendingBatches.Values.SelectMany((List<Log> batch) => batch).ToList();
					_sendingBatches.Clear();
				}
				state = _mutex.InvalidateState();
			}
			if (list != null && this.FailedToSendLog != null)
			{
				foreach (Log item in list)
				{
					AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke FailedToSendLog event for channel '" + Name + "'");
					this.FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(item, exception));
				}
			}
			if (deleteLogs)
			{
				IList<IServiceCall> list2;
				using (_mutex.GetLock(state))
				{
					list2 = _calls.ToList();
					_calls.Clear();
					_pendingLogCount = 0L;
					TriggerDeleteLogsOnSuspending(sendingBatches);
				}
				foreach (IServiceCall item2 in list2)
				{
					item2.Cancel();
				}
			}
			_storage.ClearPendingLogState(Name);
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "The suspend operation has been canceled");
		}
	}

	private void TriggerDeleteLogsOnSuspending(IList<string> sendingBatches)
	{
		try
		{
			if (this.SendingLog == null && this.FailedToSendLog == null)
			{
				_storage.DeleteLogs(Name);
				return;
			}
			SignalDeletingLogs(sendingBatches).ContinueWith(delegate
			{
				try
				{
					_storage.DeleteLogs(Name);
				}
				catch (StorageException exception2)
				{
					AppCenterLog.Warn(AppCenterLog.LogTag, "Could not delete logs with error: ", exception2);
				}
			});
		}
		catch (StorageException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not delete logs with error: ", exception);
		}
	}

	private async Task SignalDeletingLogs(IList<string> sendingBatches)
	{
		List<Log> logs = new List<Log>();
		try
		{
			do
			{
				if (sendingBatches.Contains(await _storage.GetLogsAsync(Name, 100, logs).ConfigureAwait(continueOnCapturedContext: false)))
				{
					continue;
				}
				foreach (Log item in logs)
				{
					AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke SendingLog for channel '" + Name + "'");
					this.SendingLog?.Invoke(this, new SendingLogEventArgs(item));
					AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke FailedToSendLog event for channel '" + Name + "'");
					this.FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(item, new CancellationException()));
				}
			}
			while (logs.Count >= 100);
		}
		catch
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to invoke events for logs being deleted.");
		}
	}

	private async Task TriggerIngestionAsync(State state)
	{
		using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
		{
			if (!_enabled || !_batchScheduled)
			{
				return;
			}
			AppCenterLog.Debug(AppCenterLog.LogTag, $"TriggerIngestion({Name}) pending log count: {_pendingLogCount}");
			_batchScheduled = false;
			if (_sendingBatches.Count >= _maxParallelBatches)
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, $"Already sending {_maxParallelBatches} batches of analytics data to the server");
				return;
			}
		}
		List<Log> logs = new List<Log>();
		string batchId = await _storage.GetLogsAsync(Name, _maxLogsPerBatch, logs).ConfigureAwait(continueOnCapturedContext: false);
		if (batchId == null)
		{
			return;
		}
		using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
		{
			_sendingBatches.Add(batchId, logs);
			_pendingLogCount -= logs.Count;
		}
		try
		{
			if (this.SendingLog != null)
			{
				foreach (Log item in logs)
				{
					AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke SendingLog event for channel '" + Name + "'");
					this.SendingLog?.Invoke(this, new SendingLogEventArgs(item));
				}
			}
			Guid installId = (await AppCenter.GetInstallIdAsync().ConfigureAwait(continueOnCapturedContext: false)) ?? Guid.Empty;
			IServiceCall ingestionCall = _ingestion.Call(_appSecret, installId, logs);
			using (await _mutex.GetLockAsync(state).ConfigureAwait(continueOnCapturedContext: false))
			{
				_calls.Add(ingestionCall);
			}
			ingestionCall.ContinueWith(delegate(IServiceCall call)
			{
				HandleSendingResult(state, batchId, call);
			});
			CheckPendingLogs(state);
		}
		catch (StorageException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Something went wrong sending logs to ingestion");
		}
	}

	private void HandleSendingResult(State state, string batchId, IServiceCall call)
	{
		using (_mutex.GetLock())
		{
			_calls.Remove(call);
		}
		try
		{
			if (call.IsCanceled)
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Sending logs for channel '" + Name + "', batch '" + batchId + "' canceled");
				HandleSendingFailure(state, batchId, new CancellationException());
			}
			else if (call.IsFaulted)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Sending logs for channel '" + Name + "', batch '" + batchId + "' failed: " + call.Exception);
				bool flag = call.Exception is IngestionException ex && ex.IsRecoverable;
				if (flag)
				{
					using (_mutex.GetLock(state))
					{
						List<Log> list = _sendingBatches[batchId];
						_sendingBatches.Remove(batchId);
						_pendingLogCount += list.Count;
					}
				}
				else
				{
					HandleSendingFailure(state, batchId, call.Exception);
				}
				Suspend(state, !flag, call.Exception);
			}
			else
			{
				HandleSendingSuccess(state, batchId);
			}
		}
		catch (StatefulMutexException)
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "Handle sending operation has been canceled. Callbacks were invoked when channel suspended.");
		}
	}

	private void HandleSendingSuccess(State state, string batchId)
	{
		IList<Log> list;
		using (_mutex.GetLock(state))
		{
			list = _sendingBatches[batchId];
			_sendingBatches.Remove(batchId);
		}
		if (this.SentLog != null)
		{
			foreach (Log item in list)
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke SentLog event for channel '" + Name + "'");
				this.SentLog?.Invoke(this, new SentLogEventArgs(item));
			}
		}
		try
		{
			_storage.DeleteLogs(Name, batchId);
		}
		catch (StorageException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not delete logs for batch " + batchId, exception);
		}
		CheckPendingLogs(state);
	}

	private void HandleSendingFailure(State state, string batchId, Exception exception)
	{
		IList<Log> list;
		using (_mutex.GetLock(state))
		{
			list = _sendingBatches[batchId];
			_sendingBatches.Remove(batchId);
		}
		if (this.FailedToSendLog != null)
		{
			foreach (Log item in list)
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Invoke FailedToSendLog event for channel '" + Name + "'");
				this.FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(item, exception));
			}
		}
		try
		{
			_storage.DeleteLogs(Name, batchId);
		}
		catch (StorageException exception2)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not delete logs for batch " + batchId, exception2);
		}
	}

	public void SetNetworkRequestAllowed(bool isAllowed)
	{
		if (isAllowed)
		{
			Resume(_mutex.State, needEnableChannel: false);
		}
		else
		{
			Suspend(_mutex.State, deleteLogs: false, new CancellationException(), needDisableChannel: false);
		}
	}

	private void CheckPendingLogs(State state)
	{
		if (!_enabled)
		{
			AppCenterLog.Info(AppCenterLog.LogTag, "The service has been disabled. Stop processing logs.");
			return;
		}
		if (!_ingestion.IsEnabled)
		{
			AppCenterLog.Info(AppCenterLog.LogTag, "App Center is in offline mode.");
			return;
		}
		AppCenterLog.Debug(AppCenterLog.LogTag, $"CheckPendingLogs({Name}) pending log count: {_pendingLogCount}");
		using (_mutex.GetLock())
		{
			if (_pendingLogCount >= _maxLogsPerBatch)
			{
				_batchScheduled = true;
				Task.Run(async delegate
				{
					try
					{
						await TriggerIngestionAsync(state).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (StatefulMutexException)
					{
						AppCenterLog.Warn(AppCenterLog.LogTag, "Sending logs operation has been canceled.");
					}
				});
			}
			else
			{
				if (_pendingLogCount <= 0 || _batchScheduled)
				{
					return;
				}
				_batchScheduled = true;
				Task.Run(async delegate
				{
					await Task.Delay((int)_batchTimeInterval.TotalMilliseconds).ConfigureAwait(continueOnCapturedContext: false);
					if (_batchScheduled)
					{
						try
						{
							await TriggerIngestionAsync(_mutex.State).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (StatefulMutexException)
						{
							AppCenterLog.Warn(AppCenterLog.LogTag, "Sending logs operation has been canceled.");
						}
					}
				});
			}
		}
	}

	public Task ShutdownAsync()
	{
		Suspend(_mutex.State, deleteLogs: false, new CancellationException());
		return Task.FromResult<object>(null);
	}

	public void Dispose()
	{
		_mutex.Dispose();
	}
}
