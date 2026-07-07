using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Timeout;

public class TimeoutPolicy : Policy, ITimeoutPolicy, IsPolicy
{
	private Func<Context, TimeSpan> _timeoutProvider;

	private TimeoutStrategy _timeoutStrategy;

	private Action<Context, TimeSpan, Task, Exception> _onTimeout;

	internal TimeoutPolicy(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		_timeoutProvider = timeoutProvider ?? throw new ArgumentNullException("timeoutProvider");
		_timeoutStrategy = timeoutStrategy;
		_onTimeout = onTimeout ?? throw new ArgumentNullException("onTimeout");
	}

	[DebuggerStepThrough]
	protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return TimeoutEngine.Implementation(action, context, cancellationToken, _timeoutProvider, _timeoutStrategy, _onTimeout);
	}
}
public class TimeoutPolicy<TResult> : Policy<TResult>, ITimeoutPolicy<TResult>, ITimeoutPolicy, IsPolicy
{
	private Func<Context, TimeSpan> _timeoutProvider;

	private TimeoutStrategy _timeoutStrategy;

	private Action<Context, TimeSpan, Task, Exception> _onTimeout;

	internal TimeoutPolicy(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
		: base((PolicyBuilder<TResult>)null)
	{
		_timeoutProvider = timeoutProvider ?? throw new ArgumentNullException("timeoutProvider");
		_timeoutStrategy = timeoutStrategy;
		_onTimeout = onTimeout ?? throw new ArgumentNullException("onTimeout");
	}

	protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		return TimeoutEngine.Implementation(action, context, cancellationToken, _timeoutProvider, _timeoutStrategy, _onTimeout);
	}
}
