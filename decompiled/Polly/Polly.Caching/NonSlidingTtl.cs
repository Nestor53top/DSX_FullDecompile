using System;
using Polly.Utilities;

namespace Polly.Caching;

public abstract class NonSlidingTtl : ITtlStrategy, ITtlStrategy<object>
{
	protected readonly DateTimeOffset absoluteExpirationTime;

	protected NonSlidingTtl(DateTimeOffset absoluteExpirationTime)
	{
		this.absoluteExpirationTime = absoluteExpirationTime;
	}

	public Ttl GetTtl(Context context, object result)
	{
		DateTimeOffset dateTimeOffset = absoluteExpirationTime;
		TimeSpan timeSpan = dateTimeOffset.Subtract(SystemClock.DateTimeOffsetUtcNow());
		return new Ttl((timeSpan > TimeSpan.Zero) ? timeSpan : TimeSpan.Zero, slidingExpiration: false);
	}
}
