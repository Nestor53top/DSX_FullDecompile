using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Fallback;
using Polly.Utilities;

namespace Polly;

public static class AsyncFallbackSyntax
{
	public static AsyncFallbackPolicy FallbackAsync(this PolicyBuilder policyBuilder, Func<CancellationToken, Task> fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Func<Exception, Task> onFallbackAsync = (Exception _) => TaskHelper.EmptyTask;
		return policyBuilder.FallbackAsync(fallbackAction, onFallbackAsync);
	}

	public static AsyncFallbackPolicy FallbackAsync(this PolicyBuilder policyBuilder, Func<CancellationToken, Task> fallbackAction, Func<Exception, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((Exception outcome, Context ctx, CancellationToken ct) => fallbackAction(ct), (Exception outcome, Context context) => onFallbackAsync(outcome));
	}

	public static AsyncFallbackPolicy FallbackAsync(this PolicyBuilder policyBuilder, Func<Context, CancellationToken, Task> fallbackAction, Func<Exception, Context, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((Exception outcome, Context ctx, CancellationToken ct) => fallbackAction(ctx, ct), onFallbackAsync);
	}

	public static AsyncFallbackPolicy FallbackAsync(this PolicyBuilder policyBuilder, Func<Exception, Context, CancellationToken, Task> fallbackAction, Func<Exception, Context, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return new AsyncFallbackPolicy(policyBuilder, onFallbackAsync, fallbackAction);
	}
}
