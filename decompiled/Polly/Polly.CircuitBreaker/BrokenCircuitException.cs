using System;

namespace Polly.CircuitBreaker;

public class BrokenCircuitException : ExecutionRejectedException
{
	public BrokenCircuitException()
	{
	}

	public BrokenCircuitException(string message)
		: base(message)
	{
	}

	public BrokenCircuitException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
public class BrokenCircuitException<TResult> : BrokenCircuitException
{
	private readonly TResult result;

	public TResult Result => result;

	public BrokenCircuitException(TResult result)
	{
		this.result = result;
	}

	public BrokenCircuitException(string message, TResult result)
		: base(message)
	{
		this.result = result;
	}
}
