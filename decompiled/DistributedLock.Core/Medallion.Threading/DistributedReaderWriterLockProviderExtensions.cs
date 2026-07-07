using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public static class DistributedReaderWriterLockProviderExtensions
{
	public static IDistributedSynchronizationHandle? TryAcquireReadLock(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).TryAcquireReadLock(timeout, cancellationToken);
	}

	public static IDistributedSynchronizationHandle AcquireReadLock(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).AcquireReadLock(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle?> TryAcquireReadLockAsync(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).TryAcquireReadLockAsync(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle> AcquireReadLockAsync(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).AcquireReadLockAsync(timeout, cancellationToken);
	}

	public static IDistributedSynchronizationHandle? TryAcquireWriteLock(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).TryAcquireWriteLock(timeout, cancellationToken);
	}

	public static IDistributedSynchronizationHandle AcquireWriteLock(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).AcquireWriteLock(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle?> TryAcquireWriteLockAsync(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).TryAcquireWriteLockAsync(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle> AcquireWriteLockAsync(this IDistributedReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateReaderWriterLock(name).AcquireWriteLockAsync(timeout, cancellationToken);
	}
}
