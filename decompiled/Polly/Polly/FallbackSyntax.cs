using System;
using System.Threading;
using Polly.Fallback;

namespace Polly;

public static class FallbackSyntax
{
	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Action<Exception> onFallback = delegate
		{
		};
		return policyBuilder.Fallback(fallbackAction, onFallback);
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action<CancellationToken> fallbackAction)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		Action<Exception> onFallback = delegate
		{
		};
		return policyBuilder.Fallback(fallbackAction, onFallback);
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action fallbackAction, Action<Exception> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback((Action<Exception, Context, CancellationToken>)delegate
		{
			fallbackAction();
		}, (Action<Exception, Context>)delegate(Exception exception, Context ctx)
		{
			onFallback(exception);
		});
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action<CancellationToken> fallbackAction, Action<Exception> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback(delegate(Exception outcome, Context ctx, CancellationToken ct)
		{
			fallbackAction(ct);
		}, delegate(Exception exception, Context ctx)
		{
			onFallback(exception);
		});
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action<Context> fallbackAction, Action<Exception, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback(delegate(Exception outcome, Context ctx, CancellationToken ct)
		{
			fallbackAction(ctx);
		}, onFallback);
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action<Context, CancellationToken> fallbackAction, Action<Exception, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return policyBuilder.Fallback(delegate(Exception outcome, Context ctx, CancellationToken ct)
		{
			fallbackAction(ctx, ct);
		}, onFallback);
	}

	public static FallbackPolicy Fallback(this PolicyBuilder policyBuilder, Action<Exception, Context, CancellationToken> fallbackAction, Action<Exception, Context> onFallback)
	{
		if (fallbackAction == null)
		{
			throw new ArgumentNullException("fallbackAction");
		}
		if (onFallback == null)
		{
			throw new ArgumentNullException("onFallback");
		}
		return new FallbackPolicy(policyBuilder, onFallback, fallbackAction);
	}
}
