using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Storage;

namespace Microsoft.AppCenter.Channel;

public sealed class ChannelGroup : IChannelGroup, IChannel, IDisposable, IAppSecretHolder
{
	private static readonly TimeSpan WaitStorageTimeout = TimeSpan.FromSeconds(5.0);

	private readonly HashSet<IChannelUnit> _channels = new HashSet<IChannelUnit>();

	private readonly IIngestion _ingestion;

	private readonly IStorage _storage;

	private readonly object _channelGroupLock = new object();

	private bool _isDisposed;

	private bool _isShutdown;

	public string AppSecret { get; internal set; }

	public event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;

	public event EventHandler<FilteringLogEventArgs> FilteringLog;

	public event EventHandler<SendingLogEventArgs> SendingLog;

	public event EventHandler<SentLogEventArgs> SentLog;

	public event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

	public ChannelGroup(string appSecret)
		: this(appSecret, null, null)
	{
	}

	public ChannelGroup(string appSecret, IHttpNetworkAdapter httpNetwork, INetworkStateAdapter networkState)
		: this(DefaultIngestion(httpNetwork, networkState), DefaultStorage(), appSecret)
	{
	}

	internal ChannelGroup(IIngestion ingestion, IStorage storage, string appSecret)
	{
		_ingestion = ingestion;
		_storage = storage;
		AppSecret = appSecret;
	}

	public void SetLogUrl(string logUrl)
	{
		ThrowIfDisposed();
		lock (_channelGroupLock)
		{
			_ingestion.SetLogUrl(logUrl);
		}
	}

	public IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
	{
		ThrowIfDisposed();
		lock (_channelGroupLock)
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "AddChannel(" + name + ")");
			Channel channel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, AppSecret, _ingestion, _storage);
			AddChannel(channel);
			return channel;
		}
	}

	public void AddChannel(IChannelUnit channel)
	{
		ThrowIfDisposed();
		lock (_channelGroupLock)
		{
			if (channel == null)
			{
				throw new AppCenterException("Attempted to add null channel to group");
			}
			if (!_channels.Add(channel))
			{
				throw new AppCenterException("Attempted to add duplicate channel to group");
			}
			channel.EnqueuingLog += AnyChannelEnqueuingLog;
			channel.FilteringLog += AnyChannelFilteringLog;
			channel.SendingLog += AnyChannelSendingLog;
			channel.SentLog += AnyChannelSentLog;
			channel.FailedToSendLog += AnyChannelFailedToSendLog;
		}
	}

	public void SetEnabled(bool enabled)
	{
		ThrowIfDisposed();
		lock (_channelGroupLock)
		{
			foreach (IChannelUnit channel in _channels)
			{
				channel.SetEnabled(enabled);
			}
		}
	}

	public void SetNetworkRequestAllowed(bool isAllowed)
	{
		foreach (IChannelUnit channel in _channels)
		{
			channel.SetNetworkRequestAllowed(isAllowed);
		}
	}

	public Task<bool> SetMaxStorageSizeAsync(long sizeInBytes)
	{
		ThrowIfDisposed();
		return _storage.SetMaxStorageSizeAsync(sizeInBytes);
	}

	public Task WaitStorageOperationsAsync()
	{
		ThrowIfDisposed();
		AppCenterLog.Debug(AppCenterLog.LogTag, "Waiting for storage to finish operations.");
		return _storage.WaitOperationsAsync(WaitStorageTimeout);
	}

	public async Task ShutdownAsync()
	{
		ThrowIfDisposed();
		List<Task> list = new List<Task>();
		lock (_channelGroupLock)
		{
			if (_isShutdown)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Attempted to shutdown channel multiple times.");
				return;
			}
			_isShutdown = true;
			_ingestion.Close();
			foreach (IChannelUnit channel in _channels)
			{
				list.Add(channel.ShutdownAsync());
			}
		}
		await Task.WhenAll(list).ConfigureAwait(continueOnCapturedContext: false);
		AppCenterLog.Debug(AppCenterLog.LogTag, "Waiting for storage to finish operations.");
		if (!(await _storage.ShutdownAsync(WaitStorageTimeout).ConfigureAwait(continueOnCapturedContext: false)))
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Storage taking too long to finish operations; shutting down channel without waiting any longer.");
		}
	}

	private static IIngestion DefaultIngestion(IHttpNetworkAdapter httpNetwork = null, INetworkStateAdapter networkState = null)
	{
		if (httpNetwork == null)
		{
			httpNetwork = new HttpNetworkAdapter();
		}
		if (networkState == null)
		{
			networkState = new NetworkStateAdapter();
		}
		return new NetworkStateIngestion(new RetryableIngestion(new IngestionHttp(httpNetwork)), networkState);
	}

	private static IStorage DefaultStorage()
	{
		return new Microsoft.AppCenter.Storage.Storage();
	}

	private void AnyChannelEnqueuingLog(object sender, EnqueuingLogEventArgs e)
	{
		this.EnqueuingLog?.Invoke(sender, e);
	}

	private void AnyChannelFilteringLog(object sender, FilteringLogEventArgs e)
	{
		this.FilteringLog?.Invoke(sender, e);
	}

	private void AnyChannelSendingLog(object sender, SendingLogEventArgs e)
	{
		this.SendingLog?.Invoke(sender, e);
	}

	private void AnyChannelSentLog(object sender, SentLogEventArgs e)
	{
		this.SentLog?.Invoke(sender, e);
	}

	private void AnyChannelFailedToSendLog(object sender, FailedToSendLogEventArgs e)
	{
		this.FailedToSendLog?.Invoke(sender, e);
	}

	public void Dispose()
	{
		foreach (IChannelUnit channel in _channels)
		{
			channel.Dispose();
		}
		_ingestion.Dispose();
		_storage.Dispose();
		_isDisposed = true;
	}

	private void ThrowIfDisposed()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException("ChannelGroup");
		}
	}
}
