using System;
using System.Diagnostics;
using System.Threading;

namespace Polly.Caching;

public class CachePolicy : Policy, ICachePolicy, IsPolicy
{
	private readonly ISyncCacheProvider _syncCacheProvider;

	private readonly ITtlStrategy _ttlStrategy;

	private readonly Func<Context, string> _cacheKeyStrategy;

	private readonly Action<Context, string> _onCacheGet;

	private readonly Action<Context, string> _onCacheMiss;

	private readonly Action<Context, string> _onCachePut;

	private readonly Action<Context, string, Exception> _onCacheGetError;

	private readonly Action<Context, string, Exception> _onCachePutError;

	internal CachePolicy(ISyncCacheProvider syncCacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		_syncCacheProvider = syncCacheProvider;
		_ttlStrategy = ttlStrategy;
		_cacheKeyStrategy = cacheKeyStrategy;
		_onCacheGet = onCacheGet;
		_onCachePut = onCachePut;
		_onCacheMiss = onCacheMiss;
		_onCacheGetError = onCacheGetError;
		_onCachePutError = onCachePutError;
	}

	protected override void Implementation(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		action(context, cancellationToken);
	}

	[DebuggerStepThrough]
	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return CacheEngine.Implementation(_syncCacheProvider.For<TResult>(), _ttlStrategy.For<TResult>(), _cacheKeyStrategy, action, context, cancellationToken, _onCacheGet, _onCacheMiss, _onCachePut, _onCacheGetError, _onCachePutError);
	}
}
public class CachePolicy<TResult> : Policy<TResult>, ICachePolicy<TResult>, ICachePolicy, IsPolicy
{
	private ISyncCacheProvider<TResult> _syncCacheProvider;

	private ITtlStrategy<TResult> _ttlStrategy;

	private Func<Context, string> _cacheKeyStrategy;

	private readonly Action<Context, string> _onCacheGet;

	private readonly Action<Context, string> _onCacheMiss;

	private readonly Action<Context, string> _onCachePut;

	private readonly Action<Context, string, Exception> _onCacheGetError;

	private readonly Action<Context, string, Exception> _onCachePutError;

	internal CachePolicy(ISyncCacheProvider<TResult> syncCacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
		: base((PolicyBuilder<TResult>)null)
	{
		_syncCacheProvider = syncCacheProvider;
		_ttlStrategy = ttlStrategy;
		_cacheKeyStrategy = cacheKeyStrategy;
		_onCacheGet = onCacheGet;
		_onCachePut = onCachePut;
		_onCacheMiss = onCacheMiss;
		_onCacheGetError = onCacheGetError;
		_onCachePutError = onCachePutError;
	}

	[DebuggerStepThrough]
	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return CacheEngine.Implementation(_syncCacheProvider, _ttlStrategy, _cacheKeyStrategy, action, context, cancellationToken, _onCacheGet, _onCacheMiss, _onCachePut, _onCacheGetError, _onCachePutError);
	}
}
