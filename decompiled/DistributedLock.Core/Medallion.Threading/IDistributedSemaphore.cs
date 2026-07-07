using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public interface IDistributedSemaphore
{
	string Name { get; }

	int MaxCount { get; }

	IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));
}
