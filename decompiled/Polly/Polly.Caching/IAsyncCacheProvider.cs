using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching;

public interface IAsyncCacheProvider
{
	Task<(bool, object)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext);

	Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext);
}
public interface IAsyncCacheProvider<TResult>
{
	Task<(bool, TResult)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext);

	Task PutAsync(string key, TResult value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext);
}
