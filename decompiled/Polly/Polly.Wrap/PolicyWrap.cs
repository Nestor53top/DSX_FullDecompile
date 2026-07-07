using System;
using System.Diagnostics;
using System.Threading;

namespace Polly.Wrap;

public class PolicyWrap : Policy, IPolicyWrap, IsPolicy
{
	private ISyncPolicy _outer;

	private ISyncPolicy _inner;

	public IsPolicy Outer => _outer;

	public IsPolicy Inner => _inner;

	internal override void SetPolicyContext(Context executionContext, out string priorPolicyWrapKey, out string priorPolicyKey)
	{
		priorPolicyWrapKey = executionContext.PolicyWrapKey;
		priorPolicyKey = executionContext.PolicyKey;
		if (executionContext.PolicyWrapKey == null)
		{
			executionContext.PolicyWrapKey = base.PolicyKey;
		}
		base.SetPolicyContext(executionContext, out var _, out var _);
	}

	internal PolicyWrap(Policy outer, ISyncPolicy inner)
		: base(outer.ExceptionPredicates)
	{
		_outer = outer;
		_inner = inner;
	}

	[DebuggerStepThrough]
	protected override void Implementation(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		PolicyWrapEngine.Implementation(action, context, cancellationToken, _outer, _inner);
	}

	[DebuggerStepThrough]
	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return PolicyWrapEngine.Implementation(action, context, cancellationToken, _outer, _inner);
	}
}
public class PolicyWrap<TResult> : Policy<TResult>, IPolicyWrap<TResult>, IPolicyWrap, IsPolicy
{
	private ISyncPolicy _outerNonGeneric;

	private ISyncPolicy _innerNonGeneric;

	private ISyncPolicy<TResult> _outerGeneric;

	private ISyncPolicy<TResult> _innerGeneric;

	public IsPolicy Outer
	{
		get
		{
			IsPolicy outerGeneric = _outerGeneric;
			return outerGeneric ?? _outerNonGeneric;
		}
	}

	public IsPolicy Inner
	{
		get
		{
			IsPolicy innerGeneric = _innerGeneric;
			return innerGeneric ?? _innerNonGeneric;
		}
	}

	internal override void SetPolicyContext(Context executionContext, out string priorPolicyWrapKey, out string priorPolicyKey)
	{
		priorPolicyWrapKey = executionContext.PolicyWrapKey;
		priorPolicyKey = executionContext.PolicyKey;
		if (executionContext.PolicyWrapKey == null)
		{
			executionContext.PolicyWrapKey = base.PolicyKey;
		}
		base.SetPolicyContext(executionContext, out var _, out var _);
	}

	internal PolicyWrap(Policy outer, ISyncPolicy<TResult> inner)
		: base(outer.ExceptionPredicates, ResultPredicates<TResult>.None)
	{
		_outerNonGeneric = outer;
		_innerGeneric = inner;
	}

	internal PolicyWrap(Policy<TResult> outer, ISyncPolicy inner)
		: base(outer.ExceptionPredicates, outer.ResultPredicates)
	{
		_outerGeneric = outer;
		_innerNonGeneric = inner;
	}

	internal PolicyWrap(Policy<TResult> outer, ISyncPolicy<TResult> inner)
		: base(outer.ExceptionPredicates, outer.ResultPredicates)
	{
		_outerGeneric = outer;
		_innerGeneric = inner;
	}

	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		if (_outerNonGeneric != null)
		{
			if (_innerNonGeneric != null)
			{
				return PolicyWrapEngine.Implementation(action, context, cancellationToken, _outerNonGeneric, _innerNonGeneric);
			}
			if (_innerGeneric != null)
			{
				return PolicyWrapEngine.Implementation(action, context, cancellationToken, _outerNonGeneric, _innerGeneric);
			}
			throw new InvalidOperationException("A PolicyWrap must define an inner policy.");
		}
		if (_outerGeneric != null)
		{
			if (_innerNonGeneric != null)
			{
				return PolicyWrapEngine.Implementation(action, context, cancellationToken, _outerGeneric, _innerNonGeneric);
			}
			if (_innerGeneric != null)
			{
				return PolicyWrapEngine.Implementation(action, context, cancellationToken, _outerGeneric, _innerGeneric);
			}
			throw new InvalidOperationException("A PolicyWrap must define an inner policy.");
		}
		throw new InvalidOperationException("A PolicyWrap must define an outer policy.");
	}
}
