using System;

namespace Polly.Bulkhead;

public class BulkheadRejectedException : ExecutionRejectedException
{
	public BulkheadRejectedException()
		: this("The bulkhead semaphore and queue are full and execution was rejected.")
	{
	}

	public BulkheadRejectedException(string message)
		: base(message)
	{
	}

	public BulkheadRejectedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
