using System;

namespace Polly.Timeout;

public class TimeoutRejectedException : ExecutionRejectedException
{
	public TimeoutRejectedException()
	{
	}

	public TimeoutRejectedException(string message)
		: base(message)
	{
	}

	public TimeoutRejectedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
