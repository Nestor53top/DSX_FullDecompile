using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;

namespace Polly.CircuitBreaker;

public class AsyncCircuitBreakerPolicy : AsyncPolicy, ICircuitBreakerPolicy, IsPolicy
{
	internal readonly ICircuitController<EmptyStruct> _breakerController;

	public CircuitState CircuitState => _breakerController.CircuitState;

	public Exception LastException => _breakerController.LastException;

	internal AsyncCircuitBreakerPolicy(PolicyBuilder policyBuilder, ICircuitController<EmptyStruct> breakerController)
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

	protected override async Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		TResult result = default(TResult);
		await AsyncCircuitBreakerEngine.ImplementationAsync(async delegate(Context ctx, CancellationToken ct)
		{
			result = await action(ctx, ct).ConfigureAwait(continueOnCapturedContext);
			return EmptyStruct.Instance;
		}, context, cancellationToken, continueOnCapturedContext, base.ExceptionPredicates, ResultPredicates<EmptyStruct>.None, _breakerController).ConfigureAwait(continueOnCapturedContext);
		return result;
	}
}
public class AsyncCircuitBreakerPolicy<TResult> : AsyncPolicy<TResult>, ICircuitBreakerPolicy<TResult>, ICircuitBreakerPolicy, IsPolicy
{
	internal readonly ICircuitController<TResult> _breakerController;

	public CircuitState CircuitState => _breakerController.CircuitState;

	public Exception LastException => _breakerController.LastException;

	public TResult LastHandledResult => _breakerController.LastHandledResult;

	internal AsyncCircuitBreakerPolicy(PolicyBuilder<TResult> policyBuilder, ICircuitController<TResult> breakerController)
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
	protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncCircuitBreakerEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext, base.ExceptionPredicates, base.ResultPredicates, _breakerController);
	}
}
