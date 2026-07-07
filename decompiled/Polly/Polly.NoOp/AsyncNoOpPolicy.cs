using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.NoOp;

public class AsyncNoOpPolicy : AsyncPolicy, INoOpPolicy, IsPolicy
{
	internal AsyncNoOpPolicy()
	{
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return NoOpEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext);
	}
}
public class AsyncNoOpPolicy<TResult> : AsyncPolicy<TResult>, INoOpPolicy<TResult>, INoOpPolicy, IsPolicy
{
	internal AsyncNoOpPolicy()
		: base((PolicyBuilder<TResult>)null)
	{
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return NoOpEngine.ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext);
	}
}
