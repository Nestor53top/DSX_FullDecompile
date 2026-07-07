using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Timeout;

public class AsyncTimeoutPolicy : AsyncPolicy, ITimeoutPolicy, IsPolicy
{
	private readonly Func<Context, TimeSpan> _timeoutProvider;

	private readonly TimeoutStrategy _timeoutStrategy;

	private readonly Func<Context, TimeSpan, Task, Exception, Task> _onTimeoutAsync;

	internal AsyncTimeoutPolicy(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		_timeoutProvider = timeoutProvider ?? throw new ArgumentNullException("timeoutProvider");
		_timeoutStrategy = timeoutStrategy;
		_onTimeoutAsync = onTimeoutAsync ?? throw new ArgumentNullException("onTimeoutAsync");
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncTimeoutEngine.ImplementationAsync(action, context, cancellationToken, _timeoutProvider, _timeoutStrategy, _onTimeoutAsync, continueOnCapturedContext);
	}
}
public class AsyncTimeoutPolicy<TResult> : AsyncPolicy<TResult>, ITimeoutPolicy<TResult>, ITimeoutPolicy, IsPolicy
{
	private Func<Context, TimeSpan> _timeoutProvider;

	private TimeoutStrategy _timeoutStrategy;

	private Func<Context, TimeSpan, Task, Exception, Task> _onTimeoutAsync;

	internal AsyncTimeoutPolicy(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
		: base((PolicyBuilder<TResult>)null)
	{
		_timeoutProvider = timeoutProvider ?? throw new ArgumentNullException("timeoutProvider");
		_timeoutStrategy = timeoutStrategy;
		_onTimeoutAsync = onTimeoutAsync ?? throw new ArgumentNullException("onTimeoutAsync");
	}

	[DebuggerStepThrough]
	protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		return AsyncTimeoutEngine.ImplementationAsync(action, context, cancellationToken, _timeoutProvider, _timeoutStrategy, _onTimeoutAsync, continueOnCapturedContext);
	}
}
