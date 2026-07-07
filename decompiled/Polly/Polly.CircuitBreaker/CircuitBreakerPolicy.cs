using System;
using System.Diagnostics;
using System.Threading;
using Polly.Utilities;

namespace Polly.CircuitBreaker;

public class CircuitBreakerPolicy : Policy, ICircuitBreakerPolicy, IsPolicy
{
	internal readonly ICircuitController<EmptyStruct> _breakerController;

	public CircuitState CircuitState => _breakerController.CircuitState;

	public Exception LastException => _breakerController.LastException;

	internal CircuitBreakerPolicy(PolicyBuilder policyBuilder, ICircuitController<EmptyStruct> breakerController)
		: base(policyBuilder)
	{
		_breakerController = breakerController;
	}

	public void Isolate()
	{
		_breakerController.Isolate();
	}

	public void Reset()
	{
		_breakerController.Reset();
	}

	[DebuggerStepThrough]
	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		TResult result = default(TResult);
		CircuitBreakerEngine.Implementation(delegate(Context ctx, CancellationToken ct)
		{
			result = action(ctx, ct);
			return EmptyStruct.Instance;
		}, context, cancellationToken, base.ExceptionPredicates, ResultPredicates<EmptyStruct>.None, _breakerController);
		return result;
	}
}
public class CircuitBreakerPolicy<TResult> : Policy<TResult>, ICircuitBreakerPolicy<TResult>, ICircuitBreakerPolicy, IsPolicy
{
	internal readonly ICircuitController<TResult> _breakerController;

	public CircuitState CircuitState => _breakerController.CircuitState;

	public Exception LastException => _breakerController.LastException;

	public TResult LastHandledResult => _breakerController.LastHandledResult;

	internal CircuitBreakerPolicy(PolicyBuilder<TResult> policyBuilder, ICircuitController<TResult> breakerController)
		: base(policyBuilder)
	{
		_breakerController = breakerController;
	}

	public void Isolate()
	{
		_breakerController.Isolate();
	}

	public void Reset()
	{
		_breakerController.Reset();
	}

	[DebuggerStepThrough]
	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return CircuitBreakerEngine.Implementation(action, context, cancellationToken, base.ExceptionPredicates, base.ResultPredicates, _breakerController);
	}
}
