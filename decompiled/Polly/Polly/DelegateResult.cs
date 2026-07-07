using System;

namespace Polly;

public class DelegateResult<TResult>
{
	public TResult Result { get; }

	public Exception Exception { get; }

	public DelegateResult(TResult result)
	{
		Result = result;
	}

	public DelegateResult(Exception exception)
	{
		Exception = exception;
	}
}
