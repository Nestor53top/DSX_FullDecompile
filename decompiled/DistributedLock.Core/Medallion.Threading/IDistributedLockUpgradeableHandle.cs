using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public interface IDistributedLockUpgradeableHandle : IDistributedSynchronizationHandle, IDisposable, IAsyncDisposable
{
	bool TryUpgradeToWriteLock(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	void UpgradeToWriteLock(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));

	ValueTask<bool> TryUpgradeToWriteLockAsync(TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken));

	ValueTask UpgradeToWriteLockAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken));
}
