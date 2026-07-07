using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public interface IDistributedReaderWriterLock
{
	string Name { get; }

	IDistributedSynchronizationHandle? TryAcquireReadLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	IDistributedSynchronizationHandle AcquireReadLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle?> TryAcquireReadLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle> AcquireReadLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	IDistributedSynchronizationHandle? TryAcquireWriteLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	IDistributedSynchronizationHandle AcquireWriteLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle?> TryAcquireWriteLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle> AcquireWriteLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));
}
