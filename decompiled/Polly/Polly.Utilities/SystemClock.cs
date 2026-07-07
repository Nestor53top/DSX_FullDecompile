using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Utilities;

public static class SystemClock
{
	public static Action<TimeSpan, CancellationToken> Sleep = delegate(TimeSpan timeSpan, CancellationToken cancellationToken)
	{
		if (cancellationToken.WaitHandle.WaitOne(timeSpan))
		{
			cancellationToken.ThrowIfCancellationRequested();
		}
	};

	public static Func<TimeSpan, CancellationToken, Task> SleepAsync = Task.Delay;

	public static Func<DateTime> UtcNow = () => DateTime.UtcNow;

	public static Func<DateTimeOffset> DateTimeOffsetUtcNow = () => DateTimeOffset.UtcNow;

	public static Action<CancellationTokenSource, TimeSpan> CancelTokenAfter = delegate(CancellationTokenSource tokenSource, TimeSpan timespan)
	{
		tokenSource.CancelAfter(timespan);
	};

	public static void Reset()
	{
		Sleep = delegate(TimeSpan timeSpan, CancellationToken cancellationToken)
		{
			if (cancellationToken.WaitHandle.WaitOne(timeSpan))
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
		};
		SleepAsync = Task.Delay;
		UtcNow = () => DateTime.UtcNow;
		DateTimeOffsetUtcNow = () => DateTimeOffset.UtcNow;
		CancelTokenAfter = delegate(CancellationTokenSource tokenSource, TimeSpan timespan)
		{
			tokenSource.CancelAfter(timespan);
		};
	}
}
