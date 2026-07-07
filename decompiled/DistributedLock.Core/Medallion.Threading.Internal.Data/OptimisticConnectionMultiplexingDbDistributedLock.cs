using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class OptimisticConnectionMultiplexingDbDistributedLock : IDbDistributedLock
{
	private readonly string _name;

	private readonly string _connectionString;

	private readonly MultiplexedConnectionLockPool _multiplexedConnectionLockPool;

	private readonly TimeoutValue _keepaliveCadence;

	private readonly IDbDistributedLock _fallbackLock;

	public OptimisticConnectionMultiplexingDbDistributedLock(string name, string connectionString, MultiplexedConnectionLockPool multiplexedConnectionLockPool, TimeoutValue keepaliveCadence)
	{
		_name = name;
		_connectionString = connectionString;
		_multiplexedConnectionLockPool = multiplexedConnectionLockPool;
		_keepaliveCadence = keepaliveCadence;
		_fallbackLock = new DedicatedConnectionOrTransactionDbDistributedLock(name, () => _multiplexedConnectionLockPool.ConnectionFactory(_connectionString), useTransaction: false, keepaliveCadence);
	}

	public ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync<TLockCookie>(TimeoutValue timeout, IDbSynchronizationStrategy<TLockCookie> strategy, CancellationToken cancellationToken, IDistributedSynchronizationHandle? contextHandle) where TLockCookie : class
	{
		if (!strategy.IsUpgradeable && contextHandle == null)
		{
			return _multiplexedConnectionLockPool.TryAcquireAsync(_connectionString, _name, timeout, strategy, _keepaliveCadence, cancellationToken);
		}
		return _fallbackLock.TryAcquireAsync(timeout, strategy, cancellationToken, contextHandle);
	}
}
