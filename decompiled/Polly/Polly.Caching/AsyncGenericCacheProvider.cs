using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching;

internal class AsyncGenericCacheProvider<TCacheFormat> : IAsyncCacheProvider<TCacheFormat>
{
	private readonly IAsyncCacheProvider _wrappedCacheProvider;

	internal AsyncGenericCacheProvider(IAsyncCacheProvider nonGenericCacheProvider)
	{
		_wrappedCacheProvider = nonGenericCacheProvider ?? throw new ArgumentNullException("nonGenericCacheProvider");
	}

	async Task<(bool, TCacheFormat)> IAsyncCacheProvider<TCacheFormat>.TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		var (item, obj) = await _wrappedCacheProvider.TryGetAsync(key, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		return (item, (TCacheFormat)(obj ?? ((object)default(TCacheFormat))));
	}

	Task IAsyncCacheProvider<TCacheFormat>.PutAsync(string key, TCacheFormat value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return _wrappedCacheProvider.PutAsync(key, value, ttl, cancellationToken, continueOnCapturedContext);
	}
}
