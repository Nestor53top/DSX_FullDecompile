using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public static class DistributedLockProviderExtensions
{
	public static IDistributedSynchronizationHandle? TryAcquireLock(this IDistributedLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateLock(name).TryAcquire(timeout, cancellationToken);
	}

	public static IDistributedSynchronizationHandle AcquireLock(this IDistributedLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateLock(name).Acquire(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle?> TryAcquireLockAsync(this IDistributedLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateLock(name).TryAcquireAsync(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle> AcquireLockAsync(this IDistributedLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateLock(name).AcquireAsync(timeout, cancellationToken);
	}
}
