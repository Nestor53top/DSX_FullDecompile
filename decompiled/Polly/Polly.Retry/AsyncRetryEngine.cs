using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.Retry;

internal static class AsyncRetryEngine
{
	internal static async Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, ExceptionPredicates shouldRetryExceptionPredicates, ResultPredicates<TResult> shouldRetryResultPredicates, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider = null, bool continueOnCapturedContext = false)
	{
		int tryCount = 0;
		using IEnumerator<TimeSpan> sleepDurationsEnumerator = sleepDurationsEnumerable?.GetEnumerator();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			DelegateResult<TResult> delegateResult;
			try
			{
				TResult result = await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
				if (!shouldRetryResultPredicates.AnyMatch(result))
				{
					return result;
				}
				if (tryCount >= permittedRetryCount || (sleepDurationsEnumerable != null && !sleepDurationsEnumerator.MoveNext()))
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
				if (tryCount >= permittedRetryCount || (sleepDurationsEnumerable != null && !sleepDurationsEnumerator.MoveNext()))
				{
					ex2.RethrowWithOriginalStackTraceIfDiffersFrom(ex);
					throw;
				}
				delegateResult = new DelegateResult<TResult>(ex2);
			}
			if (tryCount < int.MaxValue)
			{
				tryCount++;
			}
			TimeSpan waitDuration = sleepDurationsEnumerator?.Current ?? sleepDurationProvider?.Invoke(tryCount, delegateResult, context) ?? TimeSpan.Zero;
			await onRetryAsync(delegateResult, waitDuration, tryCount, context).ConfigureAwait(continueOnCapturedContext);
			if (waitDuration > TimeSpan.Zero)
			{
				await SystemClock.SleepAsync(waitDuration, cancellationToken).ConfigureAwait(continueOnCapturedContext);
			}
		}
	}
}
