using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public interface IDistributedUpgradeableReaderWriterLock : IDistributedReaderWriterLock
{
	IDistributedLockUpgradeableHandle? TryAcquireUpgradeableReadLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	IDistributedLockUpgradeableHandle AcquireUpgradeableReadLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedLockUpgradeableHandle?> TryAcquireUpgradeableReadLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedLockUpgradeableHandle> AcquireUpgradeableReadLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));
}
