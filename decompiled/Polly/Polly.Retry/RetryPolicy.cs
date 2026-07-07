using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Polly.Retry;

public class RetryPolicy : Policy, IRetryPolicy, IsPolicy
{
	private readonly Action<Exception, TimeSpan, int, Context> _onRetry;

	private readonly int _permittedRetryCount;

	private readonly IEnumerable<TimeSpan> _sleepDurationsEnumerable;

	private readonly Func<int, Exception, Context, TimeSpan> _sleepDurationProvider;

	internal RetryPolicy(PolicyBuilder policyBuilder, Action<Exception, TimeSpan, int, Context> onRetry, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, Exception, Context, TimeSpan> sleepDurationProvider = null)
		: base(policyBuilder)
	{
		_permittedRetryCount = permittedRetryCount;
		_sleepDurationsEnumerable = sleepDurationsEnumerable;
		_sleepDurationProvider = sleepDurationProvider;
		_onRetry = onRetry ?? throw new ArgumentNullException("onRetry");
	}

	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return RetryEngine.Implementation(action, context, cancellationToken, base.ExceptionPredicates, ResultPredicates<TResult>.None, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int retryCount, Context ctx)
		{
			_onRetry(outcome.Exception, timespan, retryCount, ctx);
		}, _permittedRetryCount, _sleepDurationsEnumerable, (_sleepDurationProvider != null) ? ((Func<int, DelegateResult<TResult>, Context, TimeSpan>)((int retryCount, DelegateResult<TResult> outcome, Context ctx) => _sleepDurationProvider(retryCount, outcome.Exception, ctx))) : null);
	}
}
public class RetryPolicy<TResult> : Policy<TResult>, IRetryPolicy<TResult>, IRetryPolicy, IsPolicy
{
	private readonly Action<DelegateResult<TResult>, TimeSpan, int, Context> _onRetry;

	private readonly int _permittedRetryCount;

	private readonly IEnumerable<TimeSpan> _sleepDurationsEnumerable;

	private readonly Func<int, DelegateResult<TResult>, Context, TimeSpan> _sleepDurationProvider;

	internal RetryPolicy(PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider = null)
		: base(policyBuilder)
	{
		_permittedRetryCount = permittedRetryCount;
		_sleepDurationsEnumerable = sleepDurationsEnumerable;
		_sleepDurationProvider = sleepDurationProvider;
		_onRetry = onRetry ?? throw new ArgumentNullException("onRetry");
	}

	[DebuggerStepThrough]
	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return RetryEngine.Implementation(action, context, cancellationToken, base.ExceptionPredicates, base.ResultPredicates, _onRetry, _permittedRetryCount, _sleepDurationsEnumerable, _sleepDurationProvider);
	}
}
