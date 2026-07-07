using System;

namespace Polly.Caching;

public class RelativeTtl : ITtlStrategy, ITtlStrategy<object>
{
	private readonly TimeSpan ttl;

	public RelativeTtl(TimeSpan ttl)
	{
		if (ttl < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("ttl", "The ttl for items to cache must be greater than zero.");
		}
		this.ttl = ttl;
	}

	public Ttl GetTtl(Context context, object result)
	{
		return new Ttl(ttl);
	}
}
