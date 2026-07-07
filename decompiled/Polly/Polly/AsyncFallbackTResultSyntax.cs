using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Fallback;
using Polly.Utilities;

namespace Polly;

public static class AsyncFallbackTResultSyntax
{
	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue)
	{
		Func<DelegateResult<TResult>, Task> onFallbackAsync = (DelegateResult<TResult> _) => TaskHelper.EmptyTask;
		return policyBuilder.FallbackAsync((CancellationToken ct) => Task.FromResult(fallbackValue), onFallbackAsync);
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<CancellationToken, Task<TResult>> fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Func<DelegateResult<TResult>, Task> onFallbackAsync = (DelegateResult<TResult> _) => TaskHelper.EmptyTask;
		return policyBuilder.FallbackAsync(fallbackAction, onFallbackAsync);
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue, Func<DelegateResult<TResult>, Task> onFallbackAsync)
	{
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => Task.FromResult(fallbackValue), (DelegateResult<TResult> outcome, Context context) => onFallbackAsync(outcome));
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<CancellationToken, Task<TResult>> fallbackAction, Func<DelegateResult<TResult>, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(ct), (DelegateResult<TResult> outcome, Context context) => onFallbackAsync(outcome));
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue, Func<DelegateResult<TResult>, Context, Task> onFallbackAsync)
	{
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => Task.FromResult(fallbackValue), onFallbackAsync);
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<Context, CancellationToken, Task<TResult>> fallbackAction, Func<DelegateResult<TResult>, Context, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return policyBuilder.FallbackAsync((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(ctx, ct), onFallbackAsync);
	}

	public static AsyncFallbackPolicy<TResult> FallbackAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, Context, CancellationToken, Task<TResult>> fallbackAction, Func<DelegateResult<TResult>, Context, Task> onFallbackAsync)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallbackAsync == null)
		{
			throw new ArgumentNullException("onFallbackAsync");
		}
		return new AsyncFallbackPolicy<TResult>(policyBuilder, onFallbackAsync, fallbackAction);
	}
}
