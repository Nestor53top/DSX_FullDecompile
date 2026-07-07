using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal interface IDbDistributedLock
{
	ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync<TLockCookie>(TimeoutValue timeout, IDbSynchronizationStrategy<TLockCookie> strategy, CancellationToken cancellationToken, IDistributedSynchronizationHandle? contextHandle) where TLockCookie : class;
}
