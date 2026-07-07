using System;

namespace Polly.Caching;

public class AbsoluteTtl : NonSlidingTtl
{
	public AbsoluteTtl(DateTimeOffset absoluteExpirationTime)
		: base(absoluteExpirationTime)
	{
	}
}
