using System;

namespace Polly.Caching;

internal class GenericTtlStrategy<TResult> : ITtlStrategy<TResult>
{
	private readonly ITtlStrategy _wrappedTtlStrategy;

	internal GenericTtlStrategy(ITtlStrategy ttlStrategy)
	{
		_wrappedTtlStrategy = ttlStrategy ?? throw new ArgumentNullException("ttlStrategy");
	}

	public Ttl GetTtl(Context context, TResult result)
	{
		return _wrappedTtlStrategy.GetTtl(context, result);
	}
}
