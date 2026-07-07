namespace Polly.CircuitBreaker;

public enum CircuitState
{
	Closed,
	Open,
	HalfOpen,
	Isolated
}
