using System;

namespace Polly.Caching;

public struct Ttl
{
	public TimeSpan Timespan;

	public bool SlidingExpiration;

	public Ttl(TimeSpan timeSpan)
		: this(timeSpan, slidingExpiration: false)
	{
	}

	public Ttl(TimeSpan timeSpan, bool slidingExpiration)
	{
		Timespan = timeSpan;
		SlidingExpiration = slidingExpiration;
	}
}
