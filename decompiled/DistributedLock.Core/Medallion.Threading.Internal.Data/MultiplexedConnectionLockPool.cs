using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class MultiplexedConnectionLockPool
{
	private readonly AsyncLock _lock = AsyncLock.Create();

	private readonly Dictionary<string, Queue<MultiplexedConnectionLock>> _poolsByConnectionString = new Dictionary<string, Queue<MultiplexedConnectionLock>>();

	private uint _storeCountSinceLastPrune;

	private uint _pooledLockCount;

	internal Func<string, DatabaseConnection> ConnectionFactory { get; }

	public MultiplexedConnectionLockPool(Func<string, DatabaseConnection> connectionFactory)
	{
		ConnectionFactory = connectionFactory;
	}

	public async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync<TLockCookie>(string connectionString, string name, TimeoutValue timeout, IDbSynchronizationStrategy<TLockCookie> strategy, TimeoutValue keepaliveCadence, CancellationToken cancellationToken) where TLockCookie : class
	{
		MultiplexedConnectionLock existingLock = await GetExistingLockOrDefaultAsync(connectionString).ConfigureAwait(continueOnCapturedContext: false);
		IDistributedSynchronizationHandle result2;
		if (existingLock != null)
		{
			bool canSafelyDisposeExistingLock = false;
			try
			{
				MultiplexedConnectionLock.Result result = await TryAcquireAsync(existingLock, opportunistic: true).ConfigureAwait(continueOnCapturedContext: false);
				if (result.Handle != null)
				{
					result2 = result.Handle;
					goto IL_035c;
				}
				canSafelyDisposeExistingLock = result.CanSafelyDispose;
				switch (result.Retry)
				{
				case MultiplexedConnectionLockRetry.NoRetry:
					result2 = null;
					goto IL_035c;
				case MultiplexedConnectionLockRetry.RetryOnThisLock:
				{
					MultiplexedConnectionLock.Result result3 = await TryAcquireAsync(existingLock, opportunistic: false).ConfigureAwait(continueOnCapturedContext: false);
					canSafelyDisposeExistingLock = result3.CanSafelyDispose;
					result2 = result3.Handle;
					goto IL_035c;
				}
				default:
					throw new InvalidOperationException("unexpected retry");
				case MultiplexedConnectionLockRetry.Retry:
					break;
				}
			}
			finally
			{
				await StoreOrDisposeLockAsync(connectionString, existingLock, canSafelyDisposeExistingLock).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		MultiplexedConnectionLock @lock = new MultiplexedConnectionLock(ConnectionFactory(connectionString));
		MultiplexedConnectionLock.Result? result4 = null;
		try
		{
			result4 = await TryAcquireAsync(@lock, opportunistic: false).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			await StoreOrDisposeLockAsync(connectionString, @lock, result4?.CanSafelyDispose ?? true).ConfigureAwait(continueOnCapturedContext: false);
		}
		return result4.Value.Handle;
		IL_035c:
		return result2;
		ValueTask<MultiplexedConnectionLock.Result> TryAcquireAsync(MultiplexedConnectionLock multiplexedConnectionLock, bool opportunistic)
		{
			return multiplexedConnectionLock.TryAcquireAsync(name, timeout, strategy, keepaliveCadence, cancellationToken, opportunistic);
		}
	}

	private async ValueTask<MultiplexedConnectionLock?> GetExistingLockOrDefaultAsync(string connectionString)
	{
		using (await _lock.AcquireAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			if (_poolsByConnectionString.TryGetValue(connectionString, out Queue<MultiplexedConnectionLock> value) && value.Count != 0)
			{
				_pooledLockCount--;
				return value.Dequeue();
			}
			return null;
		}
	}

	private async ValueTask StoreOrDisposeLockAsync(string connectionString, MultiplexedConnectionLock @lock, bool shouldDispose)
	{
		if (shouldDispose)
		{
			try
			{
				await @lock.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
		}
		using (await _lock.AcquireAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			_storeCountSinceLastPrune++;
			if (shouldDispose)
			{
				if (_poolsByConnectionString.TryGetValue(connectionString, out Queue<MultiplexedConnectionLock> value) && value.Count == 0)
				{
					_poolsByConnectionString.Remove(connectionString);
				}
			}
			else
			{
				_pooledLockCount++;
				if (_poolsByConnectionString.TryGetValue(connectionString, out Queue<MultiplexedConnectionLock> value2))
				{
					value2.Enqueue(@lock);
				}
				else
				{
					Queue<MultiplexedConnectionLock> queue = new Queue<MultiplexedConnectionLock>();
					queue.Enqueue(@lock);
					_poolsByConnectionString.Add(connectionString, queue);
				}
			}
			if (IsDueForPruningNoLock())
			{
				await PrunePoolsNoLockAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	private bool IsDueForPruningNoLock()
	{
		long num = _pooledLockCount + _poolsByConnectionString.Count;
		if (num > 64)
		{
			return _storeCountSinceLastPrune >= num;
		}
		return false;
	}

	private async ValueTask PrunePoolsNoLockAsync()
	{
		_storeCountSinceLastPrune = 0u;
		List<string> connectionStringsToRemove = null;
		foreach (KeyValuePair<string, Queue<MultiplexedConnectionLock>> kvp in _poolsByConnectionString)
		{
			Queue<MultiplexedConnectionLock> pool = kvp.Value;
			MultiplexedConnectionLock firstRetainedLock = null;
			while (pool.Count != 0 && pool.Peek() != firstRetainedLock)
			{
				MultiplexedConnectionLock @lock = pool.Dequeue();
				if (await @lock.GetIsInUseAsync().ConfigureAwait(continueOnCapturedContext: false))
				{
					if (firstRetainedLock == null)
					{
						firstRetainedLock = @lock;
					}
					pool.Enqueue(@lock);
				}
				else
				{
					_pooledLockCount--;
					try
					{
						await @lock.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch
					{
					}
				}
			}
			if (pool.Count == 0)
			{
				List<string> list = connectionStringsToRemove;
				if (list == null)
				{
					List<string> list2;
					connectionStringsToRemove = (list2 = new List<string>());
					list = list2;
				}
				list.Add(kvp.Key);
			}
		}
		if (connectionStringsToRemove == null)
		{
			return;
		}
		foreach (string item in connectionStringsToRemove)
		{
			_poolsByConnectionString.Remove(item);
		}
	}
}
