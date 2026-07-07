using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly.Utilities;
using Polly.Wrap;

namespace Polly;

public abstract class AsyncPolicy : PolicyBase, IAsyncPolicy, IsPolicy
{
	public AsyncPolicy WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	IAsyncPolicy IAsyncPolicy.WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	internal AsyncPolicy(ExceptionPredicates exceptionPredicates)
		: base(exceptionPredicates)
	{
	}

	protected AsyncPolicy(PolicyBuilder policyBuilder = null)
		: base(policyBuilder)
	{
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Task> action)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Context, Task> action, IDictionary<string, object> contextData)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Context, Task> action, Context context)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task ExecuteAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			await ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Context, Task<TResult>> action, IDictionary<string, object> contextData)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Context, Task<TResult>> action, Context context)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			return await ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Task> action)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, Task> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, Task> action, Context context)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			await ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
			return PolicyResult.Successful(context);
		}
		catch (Exception exception)
		{
			return PolicyResult.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Task<TResult>> action)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, Task<TResult>> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, Task<TResult>> action, Context context)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			return PolicyResult<TResult>.Successful(await ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext), context);
		}
		catch (Exception exception)
		{
			return PolicyResult<TResult>.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	protected virtual Task ImplementationAsync(Func<Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ImplementationAsync(async delegate(Context ctx, CancellationToken token)
		{
			await action(ctx, token).ConfigureAwait(continueOnCapturedContext);
			return EmptyStruct.Instance;
		}, context, cancellationToken, continueOnCapturedContext);
	}

	protected abstract Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext);

	public AsyncPolicyWrap WrapAsync(IAsyncPolicy innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new AsyncPolicyWrap(this, innerPolicy);
	}

	public AsyncPolicyWrap<TResult> WrapAsync<TResult>(IAsyncPolicy<TResult> innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new AsyncPolicyWrap<TResult>(this, innerPolicy);
	}
}
public abstract class AsyncPolicy<TResult> : PolicyBase<TResult>, IAsyncPolicy<TResult>, IsPolicy
{
	public AsyncPolicy<TResult> WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	IAsyncPolicy<TResult> IAsyncPolicy<TResult>.WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	protected abstract Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext);

	internal AsyncPolicy(ExceptionPredicates exceptionPredicates, ResultPredicates<TResult> resultPredicates)
		: base(exceptionPredicates, resultPredicates)
	{
	}

	protected AsyncPolicy(PolicyBuilder<TResult> policyBuilder = null)
		: base(policyBuilder)
	{
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Task<TResult>> action)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(), new Context(), CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Context, Task<TResult>> action, IDictionary<string, object> contextData)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Context, Task<TResult>> action, Context context)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ctx), context, CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<TResult> ExecuteAsync(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task<TResult> ExecuteAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			return await ImplementationAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Task<TResult>> action)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(), new Context(), CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, Task<TResult>> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, Task<TResult>> action, Context context)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ctx), context, CancellationToken.None, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken, continueOnCapturedContext);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken)
	{
		return ExecuteAndCaptureAsync(action, context, cancellationToken, continueOnCapturedContext: false);
	}

	[DebuggerStepThrough]
	public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return ExecuteAndCaptureAsync(action, new Context(contextData), cancellationToken, continueOnCapturedContext);
	}

	public async Task<PolicyResult<TResult>> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			TResult val = await ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
			if (base.ResultPredicates.AnyMatch(val))
			{
				return PolicyResult<TResult>.Failure(val, context);
			}
			return PolicyResult<TResult>.Successful(val, context);
		}
		catch (Exception exception)
		{
			return PolicyResult<TResult>.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	public AsyncPolicyWrap<TResult> WrapAsync(IAsyncPolicy innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new AsyncPolicyWrap<TResult>(this, innerPolicy);
	}

	public AsyncPolicyWrap<TResult> WrapAsync(IAsyncPolicy<TResult> innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new AsyncPolicyWrap<TResult>(this, innerPolicy);
	}
}
