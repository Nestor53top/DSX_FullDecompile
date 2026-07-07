using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal interface IDbSynchronizationStrategy<TLockCookie> where TLockCookie : class
{
	bool IsUpgradeable { get; }

	ValueTask<TLockCookie?> TryAcquireAsync(DatabaseConnection connection, string resourceName, TimeoutValue timeout, CancellationToken cancellationToken);

	ValueTask ReleaseAsync(DatabaseConnection connection, string resourceName, TLockCookie lockCookie);
}
