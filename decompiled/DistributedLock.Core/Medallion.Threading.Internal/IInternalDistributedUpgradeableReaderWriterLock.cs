using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal interface IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> : IDistributedUpgradeableReaderWriterLock, IDistributedReaderWriterLock, IInternalDistributedReaderWriterLock<THandle> where THandle : class, IDistributedSynchronizationHandle where TUpgradeableHandle : class, IDistributedLockUpgradeableHandle
{
	new TUpgradeableHandle? TryAcquireUpgradeableReadLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new TUpgradeableHandle AcquireUpgradeableReadLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<TUpgradeableHandle?> TryAcquireUpgradeableReadLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<TUpgradeableHandle> AcquireUpgradeableReadLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<TUpgradeableHandle?> InternalTryAcquireUpgradeableReadLockAsync(TimeoutValue timeout, CancellationToken cancellationToken);
}
