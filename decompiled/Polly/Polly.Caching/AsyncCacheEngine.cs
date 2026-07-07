using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching;

internal static class AsyncCacheEngine
{
	internal static async Task<TResult> ImplementationAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		cancellationToken.ThrowIfCancellationRequested();
		string cacheKey = cacheKeyStrategy(context);
		if (cacheKey == null)
		{
			return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
		}
		bool flag;
		TResult result;
		try
		{
			(flag, result) = await cacheProvider.TryGetAsync(cacheKey, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		}
		catch (Exception arg)
		{
			flag = false;
			result = default(TResult);
			onCacheGetError(context, cacheKey, arg);
		}
		if (flag)
		{
			onCacheGet(context, cacheKey);
			return result;
		}
		onCacheMiss(context, cacheKey);
		TResult result2 = await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
		Ttl ttl = ttlStrategy.GetTtl(context, result2);
		if (ttl.Timespan > TimeSpan.Zero)
		{
			try
			{
				await cacheProvider.PutAsync(cacheKey, result2, ttl, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
				onCachePut(context, cacheKey);
			}
			catch (Exception arg2)
			{
				onCachePutError(context, cacheKey, arg2);
			}
		}
		return result2;
	}
}
