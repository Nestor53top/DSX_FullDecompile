using System;

namespace Polly.Caching;

public class SlidingTtl : ITtlStrategy, ITtlStrategy<object>
{
	private readonly Ttl ttl;

	public SlidingTtl(TimeSpan slidingTtl)
	{
		if (slidingTtl < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("slidingTtl", "The ttl for items to cache must be greater than zero.");
		}
		ttl = new Ttl(slidingTtl, slidingExpiration: true);
	}

	public Ttl GetTtl(Context context, object result)
	{
		return ttl;
	}
}
