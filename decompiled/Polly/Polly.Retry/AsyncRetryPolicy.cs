using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Retry;

public class AsyncRetryPolicy : AsyncPolicy, IRetryPolicy, IsPolicy
{
	private readonly Func<Exception, TimeSpan, int, Context, Task> _onRetryAsync;

	private readonly int _permittedRetryCount;

	private readonly IEnumerable<TimeSpan> _sleepDurationsEnumerable;

	private readonly Func<int, Exception, Context, TimeSpan> _sleepDurationProvider;

	internal AsyncRetryPolicy(PolicyBuilder policyBuilder, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, Exception, Context, TimeSpan> sleepDurationProvider = null)
		: base(policyBuilder)
	{
		_permittedRetryCount = permittedRetryCount;
		_sleepDurationsEnumerable = sleepDurationsEnumerable;
		_sleepDurationProvider = sleepDurationProvider;
		_onRetryAsync = onRetryAsync ?? throw new ArgumentNullException("onRetryAsync");
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncRetryEngine.ImplementationAsync(action, context, cancellationToken, base.ExceptionPredicates, ResultPredicates<TResult>.None, (DelegateResult<TResult> outcome, TimeSpan timespan, int retryCount, Context ctx) => _onRetryAsync(outcome.Exception, timespan, retryCount, ctx), _permittedRetryCount, _sleepDurationsEnumerable, (_sleepDurationProvider != null) ? ((Func<int, DelegateResult<TResult>, Context, TimeSpan>)((int retryCount, DelegateResult<TResult> outcome, Context ctx) => _sleepDurationProvider(retryCount, outcome.Exception, ctx))) : null, continueOnCapturedContext);
	}
}
public class AsyncRetryPolicy<TResult> : AsyncPolicy<TResult>, IRetryPolicy<TResult>, IRetryPolicy, IsPolicy
{
	private readonly Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> _onRetryAsync;

	private readonly int _permittedRetryCount;

	private readonly IEnumerable<TimeSpan> _sleepDurationsEnumerable;

	private readonly Func<int, DelegateResult<TResult>, Context, TimeSpan> _sleepDurationProvider;

	internal AsyncRetryPolicy(PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync, int permittedRetryCount = int.MaxValue, IEnumerable<TimeSpan> sleepDurationsEnumerable = null, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider = null)
		: base(policyBuilder)
	{
		_permittedRetryCount = permittedRetryCount;
		_sleepDurationsEnumerable = sleepDurationsEnumerable;
		_sleepDurationProvider = sleepDurationProvider;
		_onRetryAsync = onRetryAsync ?? throw new ArgumentNullException("onRetryAsync");
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncRetryEngine.ImplementationAsync(action, context, cancellationToken, base.ExceptionPredicates, base.ResultPredicates, _onRetryAsync, _permittedRetryCount, _sleepDurationsEnumerable, _sleepDurationProvider, continueOnCapturedContext);
	}
}
