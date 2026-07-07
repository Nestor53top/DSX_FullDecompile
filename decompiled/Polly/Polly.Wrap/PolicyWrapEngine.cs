using System;
using System.Threading;

namespace Polly.Wrap;

internal static class PolicyWrapEngine
{
	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> func, Context context, CancellationToken cancellationToken, ISyncPolicy<TResult> outerPolicy, ISyncPolicy<TResult> innerPolicy)
	{
		return outerPolicy.Execute((Context ctx, CancellationToken ct) => innerPolicy.Execute(func, ctx, ct), context, cancellationToken);
	}

	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> func, Context context, CancellationToken cancellationToken, ISyncPolicy<TResult> outerPolicy, ISyncPolicy innerPolicy)
	{
		return outerPolicy.Execute((Context ctx, CancellationToken ct) => innerPolicy.Execute(func, ctx, ct), context, cancellationToken);
	}

	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> func, Context context, CancellationToken cancellationToken, ISyncPolicy outerPolicy, ISyncPolicy<TResult> innerPolicy)
	{
		return outerPolicy.Execute((Context ctx, CancellationToken ct) => innerPolicy.Execute(func, ctx, ct), context, cancellationToken);
	}

	internal static TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> func, Context context, CancellationToken cancellationToken, ISyncPolicy outerPolicy, ISyncPolicy innerPolicy)
	{
		return outerPolicy.Execute((Context ctx, CancellationToken ct) => innerPolicy.Execute(func, ctx, ct), context, cancellationToken);
	}

	internal static void Implementation(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken, ISyncPolicy outerPolicy, ISyncPolicy innerPolicy)
	{
		outerPolicy.Execute(delegate(Context ctx, CancellationToken ct)
		{
			innerPolicy.Execute(action, ctx, ct);
		}, context, cancellationToken);
	}
}
