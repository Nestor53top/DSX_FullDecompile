using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.Timeout;

internal static class TimeoutEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken, Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		cancellationToken.ThrowIfCancellationRequested();
		TimeSpan arg = timeoutProvider(context);
		using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		using CancellationTokenSource cancellationTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
		CancellationToken combinedToken = cancellationTokenSource2.Token;
		Task<TResult> task = null;
		try
		{
			if (timeoutStrategy == TimeoutStrategy.Optimistic)
			{
				SystemClock.CancelTokenAfter(cancellationTokenSource, arg);
				return action(context, combinedToken);
			}
			SystemClock.CancelTokenAfter(cancellationTokenSource, arg);
			task = Task.Run(() => action(context, combinedToken), combinedToken);
			try
			{
				task.Wait(cancellationTokenSource.Token);
			}
			catch (AggregateException ex) when (ex.InnerExceptions.Count == 1)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
			}
			return task.Result;
		}
		catch (Exception ex2)
		{
			if (ex2 is OperationCanceledException && cancellationTokenSource.IsCancellationRequested)
			{
				onTimeout(context, arg, task, ex2);
				throw new TimeoutRejectedException("The delegate executed through TimeoutPolicy did not complete within the timeout.", ex2);
			}
			throw;
		}
	}
}
