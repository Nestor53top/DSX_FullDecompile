using System;
using System.Collections.Generic;
using System.Threading;
using Polly.Utilities;

namespace Polly.Retry;

internal static class RetryEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken, ExceptionPredicates shouldRetryExceptionPredicates, ResultPredicates<TResult> shouldRetryResultPredicates, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider = null)
	{
		int num = 0;
		using IEnumerator<TimeSpan> enumerator = sleepDurationsEnumerable?.GetEnumerator();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			DelegateResult<TResult> delegateResult;
			try
			{
				TResult result = action(context, cancellationToken);
				if (!shouldRetryResultPredicates.AnyMatch(result))
				{
					return result;
				}
				if (num >= permittedRetryCount || (sleepDurationsEnumerable != null && !enumerator.MoveNext()))
				{
					return result;
				}
				delegateResult = new DelegateResult<TResult>(result);
			}
			catch (Exception ex)
			{
				Exception ex2 = shouldRetryExceptionPredicates.FirstMatchOrDefault(ex);
				if (ex2 == null)
				{
					throw;
				}
				if (num >= permittedRetryCount || (sleepDurationsEnumerable != null && !enumerator.MoveNext()))
				{
					ex2.RethrowWithOriginalStackTraceIfDiffersFrom(ex);
					throw;
				}
				delegateResult = new DelegateResult<TResult>(ex2);
			}
			if (num < int.MaxValue)
			{
				num++;
			}
			TimeSpan timeSpan = enumerator?.Current ?? sleepDurationProvider?.Invoke(num, delegateResult, context) ?? TimeSpan.Zero;
			onRetry(delegateResult, timeSpan, num, context);
			if (timeSpan > TimeSpan.Zero)
			{
				SystemClock.Sleep(timeSpan, cancellationToken);
			}
		}
	}
}
