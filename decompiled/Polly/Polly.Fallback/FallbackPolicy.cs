using System;
using System.Diagnostics;
using System.Threading;
using Polly.Utilities;

namespace Polly.Fallback;

public class FallbackPolicy : Policy, IFallbackPolicy, IsPolicy
{
	private Action<Exception, Context> _onFallback;

	private Action<Exception, Context, CancellationToken> _fallbackAction;

	internal FallbackPolicy(PolicyBuilder policyBuilder, Action<Exception, Context> onFallback, Action<Exception, Context, CancellationToken> fallbackAction)
		: base(policyBuilder)
	{
		_onFallback = onFallback ?? throw new ArgumentNullException("onFallback");
		_fallbackAction = fallbackAction ?? throw new ArgumentNullException("fallbackAction");
	}

	[DebuggerStepThrough]
	protected override void Implementation(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		FallbackEngine.Implementation(delegate(Context ctx, CancellationToken token)
		{
			action(ctx, token);
			return EmptyStruct.Instance;
		}, context, cancellationToken, base.ExceptionPredicates, ResultPredicates<EmptyStruct>.None, delegate(DelegateResult<EmptyStruct> outcome, Context ctx)
		{
			_onFallback(outcome.Exception, ctx);
		}, delegate(DelegateResult<EmptyStruct> outcome, Context ctx, CancellationToken ct)
		{
			_fallbackAction(outcome.Exception, ctx, ct);
			return EmptyStruct.Instance;
		});
	}

	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		throw new InvalidOperationException("You have executed the generic .Execute<TResult> method on a non-generic FallbackPolicy.  A non-generic FallbackPolicy only defines a fallback action which returns void; it can never return a substitute TResult value.  To use FallbackPolicy to provide fallback TResult values you must define a generic fallback policy FallbackPolicy<TResult>.  For example, define the policy as Policy<TResult>.Handle<Whatever>.Fallback<TResult>(/* some TResult value or Func<..., TResult> */);");
	}
}
public class FallbackPolicy<TResult> : Policy<TResult>, IFallbackPolicy<TResult>, IFallbackPolicy, IsPolicy
{
	private Action<DelegateResult<TResult>, Context> _onFallback;

	private Func<DelegateResult<TResult>, Context, CancellationToken, TResult> _fallbackAction;

	internal FallbackPolicy(PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, Context> onFallback, Func<DelegateResult<TResult>, Context, CancellationToken, TResult> fallbackAction)
		: base(policyBuilder)
	{
		_onFallback = onFallback ?? throw new ArgumentNullException("onFallback");
		_fallbackAction = fallbackAction ?? throw new ArgumentNullException("fallbackAction");
	}

	[DebuggerStepThrough]
	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return FallbackEngine.Implementation(action, context, cancellationToken, base.ExceptionPredicates, base.ResultPredicates, _onFallback, _fallbackAction);
	}
}
