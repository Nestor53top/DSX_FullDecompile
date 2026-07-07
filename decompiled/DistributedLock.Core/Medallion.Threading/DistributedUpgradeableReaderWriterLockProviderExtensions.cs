using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading;

public static class DistributedUpgradeableReaderWriterLockProviderExtensions
{
	public static IDistributedLockUpgradeableHandle? TryAcquireUpgradeableReadLock(this IDistributedUpgradeableReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateUpgradeableReaderWriterLock(name).TryAcquireUpgradeableReadLock(timeout, cancellationToken);
	}

	public static IDistributedLockUpgradeableHandle AcquireUpgradeableReadLock(this IDistributedUpgradeableReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateUpgradeableReaderWriterLock(name).AcquireUpgradeableReadLock(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedLockUpgradeableHandle?> TryAcquireUpgradeableReadLockAsync(this IDistributedUpgradeableReaderWriterLockProvider provider, string name, TimeSpan timeout = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateUpgradeableReaderWriterLock(name).TryAcquireUpgradeableReadLockAsync(timeout, cancellationToken);
	}

	public static ValueTask<IDistributedLockUpgradeableHandle> AcquireUpgradeableReadLockAsync(this IDistributedUpgradeableReaderWriterLockProvider provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (provider ?? throw new ArgumentNullException("provider")).CreateUpgradeableReaderWriterLock(name).AcquireUpgradeableReadLockAsync(timeout, cancellationToken);
	}
}
