using System;
using System.Threading;

namespace Polly.Fallback;

internal static class FallbackEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken, ExceptionPredicates shouldHandleExceptionPredicates, ResultPredicates<TResult> shouldHandleResultPredicates, Action<DelegateResult<TResult>, Context> onFallback, Func<DelegateResult<TResult>, Context, CancellationToken, TResult> fallbackAction)
	{
		DelegateResult<TResult> arg;
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			TResult result = action(context, cancellationToken);
			if (!shouldHandleResultPredicates.AnyMatch(result))
			{
				return result;
			}
			arg = new DelegateResult<TResult>(result);
		}
		catch (Exception ex)
		{
			Exception ex2 = shouldHandleExceptionPredicates.FirstMatchOrDefault(ex);
			if (ex2 == null)
			{
				throw;
			}
			arg = new DelegateResult<TResult>(ex2);
		}
		onFallback(arg, context);
		return fallbackAction(arg, context, cancellationToken);
	}
}
