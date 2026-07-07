using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal interface IInternalDistributedReaderWriterLock<THandle> : IDistributedReaderWriterLock where THandle : class, IDistributedSynchronizationHandle
{
	new THandle? TryAcquireReadLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new THandle AcquireReadLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle?> TryAcquireReadLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle> AcquireReadLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	new THandle? TryAcquireWriteLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new THandle AcquireWriteLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle?> TryAcquireWriteLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle> AcquireWriteLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<THandle?> InternalTryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken, bool isWrite);
}
