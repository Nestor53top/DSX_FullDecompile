using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal interface IInternalDistributedLock<THandle> : IDistributedLock where THandle : class, IDistributedSynchronizationHandle
{
	new THandle? TryAcquire(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new THandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle?> TryAcquireAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	new ValueTask<THandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<THandle?> InternalTryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken);
}
