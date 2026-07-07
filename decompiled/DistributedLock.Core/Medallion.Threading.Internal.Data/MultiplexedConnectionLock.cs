using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class MultiplexedConnectionLock : IAsyncDisposable
{
	public readonly struct Result
	{
		public IDistributedSynchronizationHandle? Handle { get; }

		public MultiplexedConnectionLockRetry Retry { get; }

		public bool CanSafelyDispose { get; }

		public Result(IDistributedSynchronizationHandle handle)
		{
			Handle = handle;
			Retry = MultiplexedConnectionLockRetry.NoRetry;
			CanSafelyDispose = false;
		}

		public Result(MultiplexedConnectionLockRetry retry, bool canSafelyDispose)
		{
			Handle = null;
			Retry = retry;
			CanSafelyDispose = canSafelyDispose;
		}
	}

	private sealed class Handle<TLockCookie> : IDistributedSynchronizationHandle, IDisposable, IAsyncDisposable where TLockCookie : class
	{
		private readonly string _name;

		private RefBox<(MultiplexedConnectionLock @lock, IDbSynchronizationStrategy<TLockCookie> strategy, TLockCookie lockCookie, IDatabaseConnectionMonitoringHandle? monitoringHandle)>? _box;

		public CancellationToken HandleLostToken
		{
			get
			{
				RefBox<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)> refBox = Volatile.Read(in _box);
				if (refBox != null && refBox.Value.Item4 == null)
				{
					IDatabaseConnectionMonitoringHandle monitoringHandle = refBox.Value.Item1._connection.ConnectionMonitor.GetMonitoringHandle();
					(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle) value = refBox.Value;
					value.Item4 = monitoringHandle;
					RefBox<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)> value2 = RefBox.Create<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)>(value);
					RefBox<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)> refBox2 = Interlocked.CompareExchange(ref _box, value2, refBox);
					if (refBox2 == refBox)
					{
						return monitoringHandle.ConnectionLostToken;
					}
					refBox = refBox2;
				}
				if (refBox == null)
				{
					throw this.ObjectDisposed();
				}
				return refBox.Value.Item4.ConnectionLostToken;
			}
		}

		public Handle(MultiplexedConnectionLock @lock, IDbSynchronizationStrategy<TLockCookie> strategy, string name, TLockCookie lockCookie)
		{
			_name = name;
			_box = RefBox.Create<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)>((@lock, strategy, lockCookie, (IDatabaseConnectionMonitoringHandle)null));
		}

		public ValueTask DisposeAsync()
		{
			if (RefBox.TryConsume<(MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle)>(ref _box, out (MultiplexedConnectionLock, IDbSynchronizationStrategy<TLockCookie>, TLockCookie, IDatabaseConnectionMonitoringHandle) value))
			{
				value.Item4?.Dispose();
				return value.Item1.ReleaseAsync(value.Item2, _name, value.Item3);
			}
			return default(ValueTask);
		}

		void IDisposable.Dispose()
		{
			this.DisposeSyncViaAsync();
		}
	}

	private sealed class ManagedFinalizationDistributedLockHandle : IDistributedSynchronizationHandle, IDisposable, IAsyncDisposable
	{
		private readonly IDistributedSynchronizationHandle _innerHandle;

		private readonly IDisposable _finalizerRegistration;

		public CancellationToken HandleLostToken => _innerHandle.HandleLostToken;

		public ManagedFinalizationDistributedLockHandle(IDistributedSynchronizationHandle innerHandle)
		{
			_innerHandle = innerHandle;
			_finalizerRegistration = ManagedFinalizerQueue.Instance.Register(this, innerHandle);
		}

		public void Dispose()
		{
			this.DisposeSyncViaAsync();
		}

		public ValueTask DisposeAsync()
		{
			_finalizerRegistration.Dispose();
			return _innerHandle.DisposeAsync();
		}
	}

	private readonly AsyncLock _mutex = AsyncLock.Create();

	private readonly Dictionary<string, TimeoutValue> _heldLocksToKeepaliveCadences = new Dictionary<string, TimeoutValue>();

	private readonly DatabaseConnection _connection;

	private bool _connectionOpened;

	private bool IsConnectionBrokenNoLock
	{
		get
		{
			if (_connectionOpened)
			{
				return !_connection.CanExecuteQueries;
			}
			return false;
		}
	}

	public MultiplexedConnectionLock(DatabaseConnection connection)
	{
		_connection = connection;
	}

	public async ValueTask<Result> TryAcquireAsync<TLockCookie>(string name, TimeoutValue timeout, IDbSynchronizationStrategy<TLockCookie> strategy, TimeoutValue keepaliveCadence, CancellationToken cancellationToken, bool opportunistic) where TLockCookie : class
	{
		using IDisposable mutexHandle = await _mutex.TryAcquireAsync(opportunistic ? TimeSpan.Zero : Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (mutexHandle == null)
		{
			return new Result(MultiplexedConnectionLockRetry.Retry, canSafelyDispose: false);
		}
		if (opportunistic && IsConnectionBrokenNoLock)
		{
			return GetAlreadyBrokenResultNoLock();
		}
		Result result;
		try
		{
			int num;
			_ = num - 1;
			_ = 1;
			try
			{
				if (_heldLocksToKeepaliveCadences.ContainsKey(name))
				{
					result = GetFailureResultNoLock(isAlreadyHeld: true, opportunistic, timeout);
				}
				else
				{
					if (!_connectionOpened)
					{
						await _connection.OpenAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						_connectionOpened = true;
					}
					TLockCookie val = await strategy.TryAcquireAsync(_connection, name, opportunistic ? ((TimeoutValue)TimeSpan.Zero) : timeout, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (val != null)
					{
						ManagedFinalizationDistributedLockHandle handle = new ManagedFinalizationDistributedLockHandle(new Handle<TLockCookie>(this, strategy, name, val));
						_heldLocksToKeepaliveCadences.Add(name, keepaliveCadence);
						if (!keepaliveCadence.IsInfinite)
						{
							SetKeepaliveCadenceNoLock();
						}
						result = new Result(handle);
					}
					else
					{
						result = GetFailureResultNoLock(isAlreadyHeld: false, opportunistic, timeout);
					}
				}
			}
			catch when (opportunistic && IsConnectionBrokenNoLock)
			{
				result = GetAlreadyBrokenResultNoLock();
			}
		}
		finally
		{
			await CloseConnectionIfNeededNoLockAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	public ValueTask DisposeAsync()
	{
		return _connection.DisposeAsync();
	}

	public async ValueTask<bool> GetIsInUseAsync()
	{
		using IDisposable disposable = await _mutex.TryAcquireAsync(TimeSpan.Zero, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
		return disposable == null || _heldLocksToKeepaliveCadences.Count != 0;
	}

	private Result GetAlreadyBrokenResultNoLock()
	{
		return new Result(MultiplexedConnectionLockRetry.Retry, _heldLocksToKeepaliveCadences.Count == 0);
	}

	private Result GetFailureResultNoLock(bool isAlreadyHeld, bool opportunistic, TimeoutValue timeout)
	{
		if (!opportunistic)
		{
			return new Result(MultiplexedConnectionLockRetry.NoRetry, _heldLocksToKeepaliveCadences.Count == 0);
		}
		if (isAlreadyHeld)
		{
			return new Result(MultiplexedConnectionLockRetry.Retry, canSafelyDispose: false);
		}
		bool flag = _heldLocksToKeepaliveCadences.Count != 0;
		if (timeout.IsZero)
		{
			return new Result(MultiplexedConnectionLockRetry.NoRetry, !flag);
		}
		if (flag)
		{
			return new Result(MultiplexedConnectionLockRetry.Retry, canSafelyDispose: false);
		}
		return new Result(MultiplexedConnectionLockRetry.RetryOnThisLock, canSafelyDispose: true);
	}

	private async ValueTask ReleaseAsync<TLockCookie>(IDbSynchronizationStrategy<TLockCookie> strategy, string name, TLockCookie lockCookie) where TLockCookie : class
	{
		using (await _mutex.AcquireAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			try
			{
				await strategy.ReleaseAsync(_connection, name, lockCookie).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				if (_heldLocksToKeepaliveCadences.TryGetValue(name, out var value))
				{
					_heldLocksToKeepaliveCadences.Remove(name);
					if (!value.IsInfinite)
					{
						SetKeepaliveCadenceNoLock();
					}
				}
				await CloseConnectionIfNeededNoLockAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	private async ValueTask CloseConnectionIfNeededNoLockAsync()
	{
		if (_connectionOpened && _heldLocksToKeepaliveCadences.Count == 0)
		{
			await _connection.CloseAsync().ConfigureAwait(continueOnCapturedContext: false);
			_connectionOpened = false;
		}
	}

	private void SetKeepaliveCadenceNoLock()
	{
		TimeoutValue timeoutValue = Timeout.InfiniteTimeSpan;
		foreach (KeyValuePair<string, TimeoutValue> heldLocksToKeepaliveCadence in _heldLocksToKeepaliveCadences)
		{
			if (heldLocksToKeepaliveCadence.Value.CompareTo(timeoutValue) < 0)
			{
				timeoutValue = heldLocksToKeepaliveCadence.Value;
			}
		}
		_connection.SetKeepaliveCadence(timeoutValue);
	}
}
