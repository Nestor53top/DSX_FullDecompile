using System;
using System.Threading;

namespace Polly.Timeout;

internal static class TimeoutValidator
{
	internal static void ValidateSecondsTimeout(int seconds)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
	}

	internal static void ValidateTimeSpanTimeout(TimeSpan timeout)
	{
		if (timeout <= TimeSpan.Zero && timeout != System.Threading.Timeout.InfiniteTimeSpan)
		{
			throw new ArgumentOutOfRangeException("timeout", timeout, "timeout must be a positive TimeSpan (or Timeout.InfiniteTimeSpan to indicate no timeout)");
		}
	}
}
