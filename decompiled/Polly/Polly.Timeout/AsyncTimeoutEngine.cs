using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.Timeout;

internal static class AsyncTimeoutEngine
{
	internal static async Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync, bool continueOnCapturedContext)
	{
		cancellationToken.ThrowIfCancellationRequested();
		TimeSpan timeout = timeoutProvider(context);
		TResult result = default(TResult);
		using (CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource())
		{
			using CancellationTokenSource combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
			Task<TResult> actionTask = null;
			CancellationToken token = combinedTokenSource.Token;
			try
			{
				if (timeoutStrategy == TimeoutStrategy.Optimistic)
				{
					SystemClock.CancelTokenAfter(timeoutCancellationTokenSource, timeout);
					result = await action(context, token).ConfigureAwait(continueOnCapturedContext);
					return result;
				}
				Task<TResult> task = timeoutCancellationTokenSource.Token.AsTask<TResult>();
				SystemClock.CancelTokenAfter(timeoutCancellationTokenSource, timeout);
				actionTask = action(context, token);
				result = await (await Task.WhenAny(new Task<TResult>[2] { actionTask, task }).ConfigureAwait(continueOnCapturedContext)).ConfigureAwait(continueOnCapturedContext);
				return result;
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (ex2 is OperationCanceledException && timeoutCancellationTokenSource.IsCancellationRequested)
				{
					await onTimeoutAsync(context, timeout, actionTask, ex2).ConfigureAwait(continueOnCapturedContext);
					throw new TimeoutRejectedException("The delegate executed asynchronously through TimeoutPolicy did not complete within the timeout.", ex2);
				}
				ExceptionDispatchInfo.Capture((ex as Exception) ?? throw ex).Throw();
			}
		}
		return result;
	}

	private static Task<TResult> AsTask<TResult>(this CancellationToken cancellationToken)
	{
		TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
		IDisposable registration = null;
		registration = cancellationToken.Register(delegate
		{
			tcs.TrySetCanceled();
			registration?.Dispose();
		}, useSynchronizationContext: false);
		return tcs.Task;
	}
}
