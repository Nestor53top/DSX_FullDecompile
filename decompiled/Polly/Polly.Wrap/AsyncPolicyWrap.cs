using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Wrap;

public class AsyncPolicyWrap : AsyncPolicy, IPolicyWrap, IsPolicy
{
	private IAsyncPolicy _outer;

	private IAsyncPolicy _inner;

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

	internal AsyncPolicyWrap(AsyncPolicy outer, IAsyncPolicy inner)
		: base(outer.ExceptionPredicates)
	{
		_outer = outer;
		_inner = inner;
	}

	[DebuggerStepThrough]
	protected override Task ImplementationAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outer, _inner);
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outer, _inner);
	}
}
public class AsyncPolicyWrap<TResult> : AsyncPolicy<TResult>, IPolicyWrap<TResult>, IPolicyWrap, IsPolicy
{
	private IAsyncPolicy _outerNonGeneric;

	private IAsyncPolicy _innerNonGeneric;

	private IAsyncPolicy<TResult> _outerGeneric;

	private IAsyncPolicy<TResult> _innerGeneric;

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

	internal AsyncPolicyWrap(AsyncPolicy outer, IAsyncPolicy<TResult> inner)
		: base(outer.ExceptionPredicates, ResultPredicates<TResult>.None)
	{
		_outerNonGeneric = outer;
		_innerGeneric = inner;
	}

	internal AsyncPolicyWrap(AsyncPolicy<TResult> outer, IAsyncPolicy inner)
		: base(outer.ExceptionPredicates, outer.ResultPredicates)
	{
		_outerGeneric = outer;
		_innerNonGeneric = inner;
	}

	internal AsyncPolicyWrap(AsyncPolicy<TResult> outer, IAsyncPolicy<TResult> inner)
		: base(outer.ExceptionPredicates, outer.ResultPredicates)
	{
		_outerGeneric = outer;
		_innerGeneric = inner;
	}

	protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (_outerNonGeneric != null)
		{
			if (_innerNonGeneric != null)
			{
				return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outerNonGeneric, _innerNonGeneric);
			}
			if (_innerGeneric != null)
			{
				return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outerNonGeneric, _innerGeneric);
			}
			throw new InvalidOperationException("A AsyncPolicyWrap must define an inner policy.");
		}
		if (_outerGeneric != null)
		{
			if (_innerNonGeneric != null)
			{
				return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outerGeneric, _innerNonGeneric);
			}
			if (_innerGeneric != null)
			{
				return AsyncPolicyWrapEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, _outerGeneric, _innerGeneric);
			}
			throw new InvalidOperationException("A AsyncPolicyWrap must define an inner policy.");
		}
		throw new InvalidOperationException("A AsyncPolicyWrap must define an outer policy.");
	}
}
