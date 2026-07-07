using System;

namespace Polly;

public abstract class ExecutionRejectedException : Exception
{
	protected ExecutionRejectedException()
	{
	}

	protected ExecutionRejectedException(string message)
		: base(message)
	{
	}

	protected ExecutionRejectedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
