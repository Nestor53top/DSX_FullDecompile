using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal interface IInternalDistributedLockUpgradeableHandle : IDistributedLockUpgradeableHandle, IDistributedSynchronizationHandle, IDisposable, IAsyncDisposable
{
	ValueTask<bool> InternalTryUpgradeToWriteLockAsync(TimeoutValue timeout, CancellationToken cancellationToken);
}
