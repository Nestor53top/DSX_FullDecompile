using System;

namespace Polly;

public class PolicyResult
{
	public OutcomeType Outcome { get; }

	public Exception FinalException { get; }

	public ExceptionType? ExceptionType { get; }

	public Context Context { get; }

	internal PolicyResult(OutcomeType outcome, Exception finalException, ExceptionType? exceptionType, Context context)
	{
		Outcome = outcome;
		FinalException = finalException;
		ExceptionType = exceptionType;
		Context = context;
	}

	public static PolicyResult Successful(Context context)
	{
		return new PolicyResult(OutcomeType.Successful, null, null, context);
	}

	public static PolicyResult Failure(Exception exception, ExceptionType exceptionType, Context context)
	{
		return new PolicyResult(OutcomeType.Failure, exception, exceptionType, context);
	}
}
public class PolicyResult<TResult>
{
	public OutcomeType Outcome { get; }

	public Exception FinalException { get; }

	public ExceptionType? ExceptionType { get; }

	public TResult Result { get; }

	public TResult FinalHandledResult { get; }

	public FaultType? FaultType { get; }

	public Context Context { get; }

	internal PolicyResult(TResult result, OutcomeType outcome, Exception finalException, ExceptionType? exceptionType, Context context)
		: this(result, outcome, finalException, exceptionType, default(TResult), (FaultType?)null, context)
	{
	}

	internal PolicyResult(TResult result, OutcomeType outcome, Exception finalException, ExceptionType? exceptionType, TResult finalHandledResult, FaultType? faultType, Context context)
	{
		Result = result;
		Outcome = outcome;
		FinalException = finalException;
		ExceptionType = exceptionType;
		FinalHandledResult = finalHandledResult;
		FaultType = faultType;
		Context = context;
	}

	public static PolicyResult<TResult> Successful(TResult result, Context context)
	{
		return new PolicyResult<TResult>(result, OutcomeType.Successful, null, null, context);
	}

	public static PolicyResult<TResult> Failure(Exception exception, ExceptionType exceptionType, Context context)
	{
		return new PolicyResult<TResult>(default(TResult), OutcomeType.Failure, exception, exceptionType, default(TResult), (exceptionType != Polly.ExceptionType.HandledByThisPolicy) ? Polly.FaultType.UnhandledException : Polly.FaultType.ExceptionHandledByThisPolicy, context);
	}

	public static PolicyResult<TResult> Failure(TResult handledResult, Context context)
	{
		return new PolicyResult<TResult>(default(TResult), OutcomeType.Failure, null, null, handledResult, Polly.FaultType.ResultHandledByThisPolicy, context);
	}
}
