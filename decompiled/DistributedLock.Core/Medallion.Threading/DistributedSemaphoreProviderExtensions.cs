using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public static class DistributedSemaphoreProviderExtensions
{
	public static IDistributedSynchronizationHandle? TryAcquireSemaphore(this IDistributedSemaphoreProvider provider, string name, int maxCount, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateSemaphore(name, maxCount).TryAcquire(timeout, cancellationToken);
	}

	public static IDistributedSynchronizationHandle AcquireSemaphore(this IDistributedSemaphoreProvider provider, string name, int maxCount, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateSemaphore(name, maxCount).Acquire(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle?> TryAcquireSemaphoreAsync(this IDistributedSemaphoreProvider provider, string name, int maxCount, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateSemaphore(name, maxCount).TryAcquireAsync(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedSynchronizationHandle> AcquireSemaphoreAsync(this IDistributedSemaphoreProvider provider, string name, int maxCount, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateSemaphore(name, maxCount).AcquireAsync(timeout, cancellationToken);
	}
}
