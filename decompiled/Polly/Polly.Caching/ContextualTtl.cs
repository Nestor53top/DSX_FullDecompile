using System;

namespace Polly.Caching;

public class ContextualTtl : ITtlStrategy, ITtlStrategy<object>
{
	public static readonly string TimeSpanKey = "ContextualTtlTimeSpan";

	public static readonly string SlidingExpirationKey = "ContextualTtlSliding";

	private static readonly Ttl _noTtl = new Ttl(TimeSpan.Zero, slidingExpiration: false);

	public Ttl GetTtl(Context context, object result)
	{
		if (!context.ContainsKey(TimeSpanKey))
		{
			return _noTtl;
		}
		bool slidingExpiration = context.ContainsKey(SlidingExpirationKey) && context[SlidingExpirationKey] as bool? == true;
		return new Ttl((context[TimeSpanKey] as TimeSpan?) ?? TimeSpan.Zero, slidingExpiration);
	}
}
