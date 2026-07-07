using System;
using Polly.CircuitBreaker;

namespace Polly;

public static class CircuitBreakerTResultSyntax
{
	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
	{
		Action<DelegateResult<TResult>, TimeSpan> onBreak = delegate
		{
		};
		Action onReset = delegate
		{
		};
		return policyBuilder.CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak, onBreak, onReset);
	}

	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, TimeSpan> onBreak, Action onReset)
	{
		return policyBuilder.CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, Context context)
		{
			onBreak(outcome, timespan);
		}, delegate
		{
			onReset();
		});
	}

	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, TimeSpan, Context> onBreak, Action<Context> onReset)
	{
		Action onHalfOpen = delegate
		{
		};
		return policyBuilder.CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak, onBreak, onReset, onHalfOpen);
	}

	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, TimeSpan> onBreak, Action onReset, Action onHalfOpen)
	{
		return policyBuilder.CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, Context context)
		{
			onBreak(outcome, timespan);
		}, delegate
		{
			onReset();
		}, onHalfOpen);
	}

	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
	{
		return policyBuilder.CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak, delegate(DelegateResult<TResult> outcome, CircuitState state, TimeSpan timespan, Context context)
		{
			onBreak(outcome, timespan, context);
		}, onReset, onHalfOpen);
	}

	public static CircuitBreakerPolicy<TResult> CircuitBreaker<TResult>(this PolicyBuilder<TResult> policyBuilder, int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, CircuitState, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
	{
		if (handledEventsAllowedBeforeBreaking <= 0)
		{
			throw new ArgumentOutOfRangeException("handledEventsAllowedBeforeBreaking", "Value must be greater than zero.");
		}
		if (durationOfBreak < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("durationOfBreak", "Value must be greater than zero.");
		}
		if (onBreak == null)
		{
			throw new ArgumentNullException("onBreak");
		}
		if (onReset == null)
		{
			throw new ArgumentNullException("onReset");
		}
		if (onHalfOpen == null)
		{
			throw new ArgumentNullException("onHalfOpen");
		}
		ICircuitController<TResult> breakerController = new ConsecutiveCountCircuitController<TResult>(handledEventsAllowedBeforeBreaking, durationOfBreak, onBreak, onReset, onHalfOpen);
		return new CircuitBreakerPolicy<TResult>(policyBuilder, breakerController);
	}
}
