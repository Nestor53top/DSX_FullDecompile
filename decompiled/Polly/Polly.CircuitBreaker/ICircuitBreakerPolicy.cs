using System;

namespace Polly.CircuitBreaker;

public interface ICircuitBreakerPolicy : IsPolicy
{
	CircuitState CircuitState { get; }

	Exception LastException { get; }

	void Isolate();

	void Reset();
}
public interface ICircuitBreakerPolicy<TResult> : ICircuitBreakerPolicy, IsPolicy
{
	TResult LastHandledResult { get; }
}
