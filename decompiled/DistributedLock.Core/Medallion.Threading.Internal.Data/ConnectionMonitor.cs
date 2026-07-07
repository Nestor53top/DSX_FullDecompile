using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class ConnectionMonitor : IAsyncDisposable
{
	private sealed class MonitoringHandle : IDatabaseConnectionMonitoringHandle, IDisposable
	{
		private ConnectionMonitor? _monitor;

		private readonly CancellationToken _connectionLostToken;

		public CancellationToken ConnectionLostToken
		{
			get
			{
				if (Volatile.Read(in _monitor) == null)
				{
					throw new ObjectDisposedException("handle");
				}
				return _connectionLostToken;
			}
		}

		public MonitoringHandle(ConnectionMonitor keepaliveHelper, CancellationToken cancellationToken)
		{
			_monitor = keepaliveHelper;
			_connectionLostToken = cancellationToken;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _monitor, null)?.ReleaseMonitoringHandle(this);
		}
	}

	private sealed class AlreadyCanceledHandle : IDatabaseConnectionMonitoringHandle, IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public CancellationToken ConnectionLostToken => _cancellationTokenSource.Token;

		public AlreadyCanceledHandle()
		{
			_cancellationTokenSource.Cancel();
		}

		public void Dispose()
		{
			_cancellationTokenSource.Dispose();
		}
	}

	private sealed class NullHandle : IDatabaseConnectionMonitoringHandle, IDisposable
	{
		public static readonly NullHandle Instance = new NullHandle();

		public CancellationToken ConnectionLostToken => CancellationToken.None;

		private NullHandle()
		{
		}

		public void Dispose()
		{
		}
	}

	private enum State : byte
	{
		Idle,
		Active,
		AutoStopped,
		Stopped,
		Disposed
	}

	private readonly WeakReference<DatabaseConnection> _weakConnection;

	private readonly StateChangeEventHandler? _stateChangedHandler;

	private readonly AsyncLock _connectionLock = AsyncLock.Create();

	private readonly bool _isExternallyOwnedConnection;

	private TimeoutValue _keepaliveCadence = Timeout.InfiniteTimeSpan;

	private State _state;

	private Dictionary<MonitoringHandle, CancellationTokenSource>? _monitoringHandleRegistrations;

	private CancellationTokenSource? _monitorStateChangedTokenSource;

	private Task _monitoringWorkerTask = Task.CompletedTask;

	private object Lock => _weakConnection;

	private bool HasRegisteredMonitoringHandlesNoLock => (_monitoringHandleRegistrations?.Count).GetValueOrDefault() != 0;

	public ConnectionMonitor(DatabaseConnection connection)
	{
		_weakConnection = new WeakReference<DatabaseConnection>(connection);
		_isExternallyOwnedConnection = connection.IsExernallyOwned;
		_state = ((!connection.CanExecuteQueries) ? State.Stopped : State.Idle);
		if (connection.InnerConnection is DbConnection dbConnection)
		{
			dbConnection.StateChange += (_stateChangedHandler = OnConnectionStateChanged);
		}
	}

	public async ValueTask<IDisposable> AcquireConnectionLockAsync(CancellationToken cancellationToken)
	{
		IDisposable disposable;
		do
		{
			ValueTask<IDisposable> valueTask;
			lock (Lock)
			{
				if (_state == State.Active && HasRegisteredMonitoringHandlesNoLock)
				{
					FireStateChangedNoLock();
				}
				valueTask = _connectionLock.TryAcquireAsync(TimeSpan.FromSeconds(2.0), cancellationToken);
			}
			disposable = await valueTask.ConfigureAwait(continueOnCapturedContext: false);
		}
		while (disposable == null);
		return disposable;
	}

	public void SetKeepaliveCadence(TimeoutValue keepaliveCadence)
	{
		lock (Lock)
		{
			TimeoutValue keepaliveCadence2 = _keepaliveCadence;
			_keepaliveCadence = keepaliveCadence;
			if (!StartMonitorWorkerIfNeededNoLock() && _state == State.Active && !HasRegisteredMonitoringHandlesNoLock && keepaliveCadence.CompareTo(keepaliveCadence2) < 0)
			{
				FireStateChangedNoLock();
			}
		}
	}

	public IDatabaseConnectionMonitoringHandle GetMonitoringHandle()
	{
		lock (Lock)
		{
			if (_state == State.Disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (_state == State.AutoStopped || _state == State.Stopped)
			{
				return new AlreadyCanceledHandle();
			}
			if (_stateChangedHandler == null)
			{
				return NullHandle.Instance;
			}
			bool hasRegisteredMonitoringHandlesNoLock = HasRegisteredMonitoringHandlesNoLock;
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			MonitoringHandle monitoringHandle = new MonitoringHandle(this, cancellationTokenSource.Token);
			(_monitoringHandleRegistrations ?? (_monitoringHandleRegistrations = new Dictionary<MonitoringHandle, CancellationTokenSource>())).Add(monitoringHandle, cancellationTokenSource);
			if (!StartMonitorWorkerIfNeededNoLock() && !hasRegisteredMonitoringHandlesNoLock && _state == State.Active)
			{
				FireStateChangedNoLock();
			}
			return monitoringHandle;
		}
	}

	private void ReleaseMonitoringHandle(MonitoringHandle handle)
	{
		lock (Lock)
		{
			if (_monitoringHandleRegistrations.TryGetValue(handle, out CancellationTokenSource value))
			{
				_monitoringHandleRegistrations.Remove(handle);
				value.Dispose();
				if (_monitoringHandleRegistrations.Count == 0 && _state == State.Active)
				{
					FireStateChangedNoLock();
				}
			}
		}
	}

	private void OnConnectionStateChanged(object sender, StateChangeEventArgs args)
	{
		if (args.OriginalState == ConnectionState.Open && args.CurrentState != ConnectionState.Open)
		{
			lock (Lock)
			{
				if (_state == State.Idle || _state == State.Active)
				{
					_state = State.AutoStopped;
					CloseOrCancelMonitoringHandleRegistrationsNoLock(isCancel: true);
				}
				return;
			}
		}
		if (args.OriginalState == ConnectionState.Open || args.CurrentState != ConnectionState.Open)
		{
			return;
		}
		lock (Lock)
		{
			if (_state == State.AutoStopped)
			{
				StartNoLock();
			}
		}
	}

	public void Start()
	{
		lock (Lock)
		{
			StartNoLock();
		}
	}

	private void StartNoLock()
	{
		_state = State.Idle;
		StartMonitorWorkerIfNeededNoLock();
	}

	public ValueTask StopAsync()
	{
		return StopOrDisposeAsync(isDispose: false);
	}

	public ValueTask DisposeAsync()
	{
		return StopOrDisposeAsync(isDispose: true);
	}

	private async ValueTask StopOrDisposeAsync(bool isDispose)
	{
		Task monitoringWorkerTask;
		lock (Lock)
		{
			if (isDispose)
			{
				_state = State.Disposed;
			}
			else
			{
				_state = State.Stopped;
			}
			CloseOrCancelMonitoringHandleRegistrationsNoLock(isCancel: false);
			monitoringWorkerTask = _monitoringWorkerTask;
			_monitorStateChangedTokenSource?.Cancel();
			if (_stateChangedHandler != null && _weakConnection.TryGetTarget(out DatabaseConnection target))
			{
				((DbConnection)target.InnerConnection).StateChange -= _stateChangedHandler;
			}
		}
		if (monitoringWorkerTask != null)
		{
			await monitoringWorkerTask.AwaitSyncOverAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void CloseOrCancelMonitoringHandleRegistrationsNoLock(bool isCancel)
	{
		if (_monitoringHandleRegistrations == null)
		{
			return;
		}
		foreach (KeyValuePair<MonitoringHandle, CancellationTokenSource> monitoringHandleRegistration in _monitoringHandleRegistrations)
		{
			CancellationTokenSource cancellationTokenSource = monitoringHandleRegistration.Value;
			if (isCancel)
			{
				Task.Run(delegate
				{
					try
					{
						cancellationTokenSource.Cancel();
					}
					finally
					{
						cancellationTokenSource.Dispose();
					}
				});
			}
			else
			{
				cancellationTokenSource.Dispose();
			}
		}
		_monitoringHandleRegistrations.Clear();
	}

	private bool StartMonitorWorkerIfNeededNoLock()
	{
		if (_isExternallyOwnedConnection)
		{
			return false;
		}
		if (_state != State.Idle)
		{
			return false;
		}
		if (_keepaliveCadence.IsInfinite && !HasRegisteredMonitoringHandlesNoLock)
		{
			return false;
		}
		_monitorStateChangedTokenSource = new CancellationTokenSource();
		_monitoringWorkerTask = _monitoringWorkerTask.ContinueWith((Task _, object state) => ((ConnectionMonitor)state).MonitorWorkerLoop(), this).Unwrap();
		_state = State.Active;
		return true;
	}

	private void FireStateChangedNoLock()
	{
		CancellationTokenSource monitorStateChangedTokenSource = _monitorStateChangedTokenSource;
		_monitorStateChangedTokenSource = new CancellationTokenSource();
		Task.Run(delegate
		{
			try
			{
				monitorStateChangedTokenSource.Cancel();
			}
			finally
			{
				monitorStateChangedTokenSource.Dispose();
			}
		});
	}

	private async Task MonitorWorkerLoop()
	{
		while (await TryKeepaliveOrMonitorAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
		}
	}

	private async Task<bool> TryKeepaliveOrMonitorAsync()
	{
		TimeoutValue keepaliveCadence;
		bool hasRegisteredMonitoringHandlesNoLock;
		CancellationToken token;
		lock (Lock)
		{
			if (_state != State.Active)
			{
				return false;
			}
			keepaliveCadence = _keepaliveCadence;
			hasRegisteredMonitoringHandlesNoLock = HasRegisteredMonitoringHandlesNoLock;
			token = _monitorStateChangedTokenSource.Token;
		}
		return await (hasRegisteredMonitoringHandlesNoLock ? DoMonitoringAsync(token) : DoKeepaliveAsync(keepaliveCadence, token)).ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<bool> DoMonitoringAsync(CancellationToken cancellationToken)
	{
		if (!_weakConnection.TryGetTarget(out DatabaseConnection connection))
		{
			return false;
		}
		using (await _connectionLock.AcquireAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			await connection.SleepAsync(TimeSpan.FromMinutes(1.0), cancellationToken, (DatabaseCommand command, CancellationToken token) => command.ExecuteNonQueryAsync(token, disallowAsyncCancellation: false, isConnectionMonitoringQuery: true)).TryAwait();
			return true;
		}
	}

	private async Task<bool> DoKeepaliveAsync(TimeoutValue keepaliveCadence, CancellationToken stateChangedToken)
	{
		await Task.Delay(keepaliveCadence.InMilliseconds, stateChangedToken).TryAwait();
		if (stateChangedToken.IsCancellationRequested)
		{
			return true;
		}
		if (!_weakConnection.TryGetTarget(out DatabaseConnection connection))
		{
			return false;
		}
		using IDisposable connectionLockHandle = await _connectionLock.TryAcquireAsync(TimeSpan.Zero, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
		if (connectionLockHandle != null)
		{
			using DatabaseCommand command = connection.CreateCommand();
			command.SetCommandText("SELECT 0 /* DistributedLock connection keepalive */");
			await command.ExecuteNonQueryAsync(CancellationToken.None, disallowAsyncCancellation: false, isConnectionMonitoringQuery: true).AsTask().TryAwait();
		}
		return true;
	}
}
