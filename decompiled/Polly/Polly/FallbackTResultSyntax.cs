using System;
using System.Threading;
using Polly.Fallback;

namespace Polly;

public static class FallbackTResultSyntax
{
	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue)
	{
		Action<DelegateResult<TResult>> onFallback = delegate
		{
		};
		return policyBuilder.Fallback(() => fallbackValue, onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<TResult> fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Action<DelegateResult<TResult>> onFallback = delegate
		{
		};
		return policyBuilder.Fallback(fallbackAction, onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<CancellationToken, TResult> fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Action<DelegateResult<TResult>> onFallback = delegate
		{
		};
		return policyBuilder.Fallback(fallbackAction, onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue, Action<DelegateResult<TResult>> onFallback)
	{
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackValue, delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onFallback(outcome);
		});
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<TResult> fallbackAction, Action<DelegateResult<TResult>> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(), delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onFallback(outcome);
		});
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<CancellationToken, TResult> fallbackAction, Action<DelegateResult<TResult>> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(ct), delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onFallback(outcome);
		});
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, TResult fallbackValue, Action<DelegateResult<TResult>, Context> onFallback)
	{
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackValue, onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<Context, TResult> fallbackAction, Action<DelegateResult<TResult>, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(ctx), onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<Context, CancellationToken, TResult> fallbackAction, Action<DelegateResult<TResult>, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((DelegateResult<TResult> outcome, Context ctx, CancellationToken ct) => fallbackAction(ctx, ct), onFallback);
	}

	public static FallbackPolicy<TResult> Fallback<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, Context, CancellationToken, TResult> fallbackAction, Action<DelegateResult<TResult>, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return new FallbackPolicy<TResult>(policyBuilder, onFallback, fallbackAction);
	}
}
