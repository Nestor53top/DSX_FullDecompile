using System;
using System.Diagnostics;
using System.Threading;

namespace Polly.NoOp;

public class NoOpPolicy : Policy, INoOpPolicy, IsPolicy
{
	internal NoOpPolicy()
	{
	}

	[DebuggerStepThrough]
	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return NoOpEngine.Implementation(action, context, cancellationToken);
	}
}
public class NoOpPolicy<TResult> : Policy<TResult>, INoOpPolicy<TResult>, INoOpPolicy, IsPolicy
{
	internal NoOpPolicy()
		: base((PolicyBuilder<TResult>)null)
	{
	}

	[DebuggerStepThrough]
	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return NoOpEngine.Implementation(action, context, cancellationToken);
	}
}
