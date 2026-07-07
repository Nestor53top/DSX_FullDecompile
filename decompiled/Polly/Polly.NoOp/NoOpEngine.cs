using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.NoOp;

internal static class NoOpEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return action(context, cancellationToken);
	}

	internal static async Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
	}
}
