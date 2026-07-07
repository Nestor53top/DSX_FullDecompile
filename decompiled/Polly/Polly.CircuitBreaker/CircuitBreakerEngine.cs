using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Polly.CircuitBreaker;

internal class CircuitBreakerEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken, ExceptionPredicates shouldHandleExceptionPredicates, ResultPredicates<TResult> shouldHandleResultPredicates, ICircuitController<TResult> breakerController)
	{
		cancellationToken.ThrowIfCancellationRequested();
		breakerController.OnActionPreExecute();
		try
		{
			TResult result = action(context, cancellationToken);
			if (shouldHandleResultPredicates.AnyMatch(result))
			{
				breakerController.OnActionFailure(new DelegateResult<TResult>(result), context);
			}
			else
			{
				breakerController.OnActionSuccess(context);
			}
			return result;
		}
		catch (Exception ex)
		{
			Exception ex2 = shouldHandleExceptionPredicates.FirstMatchOrDefault(ex);
			if (ex2 == null)
			{
				throw;
			}
			breakerController.OnActionFailure(new DelegateResult<TResult>(ex2), context);
			if (ex2 != ex)
			{
				ExceptionDispatchInfo.Capture(ex2).Throw();
			}
			throw;
		}
	}
}
