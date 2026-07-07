using System;
using Polly.CircuitBreaker;
using Polly.Utilities;

namespace Polly;

public static class AdvancedCircuitBreakerSyntax
{
	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak)
	{
		Action<Exception, TimeSpan> onBreak = delegate
		{
		};
		Action onReset = delegate
		{
		};
		return policyBuilder.AdvancedCircuitBreaker(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, onBreak, onReset);
	}

	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak, Action<Exception, TimeSpan> onBreak, Action onReset)
	{
		return policyBuilder.AdvancedCircuitBreaker(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, delegate(Exception exception, TimeSpan timespan, Context context)
		{
			onBreak(exception, timespan);
		}, delegate
		{
			onReset();
		});
	}

	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak, Action<Exception, TimeSpan, Context> onBreak, Action<Context> onReset)
	{
		Action onHalfOpen = delegate
		{
		};
		return policyBuilder.AdvancedCircuitBreaker(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, onBreak, onReset, onHalfOpen);
	}

	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak, Action<Exception, TimeSpan> onBreak, Action onReset, Action onHalfOpen)
	{
		return policyBuilder.AdvancedCircuitBreaker(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, delegate(Exception exception, TimeSpan timespan, Context context)
		{
			onBreak(exception, timespan);
		}, delegate
		{
			onReset();
		}, onHalfOpen);
	}

	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak, Action<Exception, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
	{
		return policyBuilder.AdvancedCircuitBreaker(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, delegate(Exception exception, CircuitState state, TimeSpan timespan, Context context)
		{
			onBreak(exception, timespan, context);
		}, onReset, onHalfOpen);
	}

	public static CircuitBreakerPolicy AdvancedCircuitBreaker(this PolicyBuilder policyBuilder, double failureThreshold, TimeSpan samplingDuration, int minimumThroughput, TimeSpan durationOfBreak, Action<Exception, CircuitState, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
	{
		TimeSpan timeSpan = TimeSpan.FromTicks(AdvancedCircuitController<EmptyStruct>.ResolutionOfCircuitTimer);
		if (failureThreshold <= 0.0)
		{
			throw new ArgumentOutOfRangeException("failureThreshold", "Value must be greater than zero.");
		}
		if (failureThreshold > 1.0)
		{
			throw new ArgumentOutOfRangeException("failureThreshold", "Value must be less than or equal to one.");
		}
		if (samplingDuration < timeSpan)
		{
			throw new ArgumentOutOfRangeException("samplingDuration", $"Value must be equal to or greater than {timeSpan.TotalMilliseconds} milliseconds. This is the minimum resolution of the CircuitBreaker timer.");
		}
		if (minimumThroughput <= 1)
		{
			throw new ArgumentOutOfRangeException("minimumThroughput", "Value must be greater than one.");
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
		AdvancedCircuitController<EmptyStruct> breakerController = new AdvancedCircuitController<EmptyStruct>(failureThreshold, samplingDuration, minimumThroughput, durationOfBreak, delegate(DelegateResult<EmptyStruct> outcome, CircuitState state, TimeSpan timespan, Context context)
		{
			onBreak(outcome.Exception, state, timespan, context);
		}, onReset, onHalfOpen);
		return new CircuitBreakerPolicy(policyBuilder, breakerController);
	}
}
