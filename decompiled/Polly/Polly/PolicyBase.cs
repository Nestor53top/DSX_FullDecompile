using System;
using System.Threading;
using Polly.Utilities;

namespace Polly;

public abstract class PolicyBase
{
	protected string policyKeyInternal;

	internal readonly CancellationToken DefaultCancellationToken = CancellationToken.None;

	internal const bool DefaultContinueOnCapturedContext = false;

	public string PolicyKey => policyKeyInternal ?? (policyKeyInternal = GetType().Name + "-" + KeyHelper.GuidPart());

	internal static ArgumentException PolicyKeyMustBeImmutableException => new ArgumentException("PolicyKey cannot be changed once set; or (when using the default value after the PolicyKey property has been accessed.", "policyKey");

	protected internal ExceptionPredicates ExceptionPredicates { get; }

	internal virtual void SetPolicyContext(Context executionContext, out string priorPolicyWrapKey, out string priorPolicyKey)
	{
		priorPolicyWrapKey = executionContext.PolicyWrapKey;
		priorPolicyKey = executionContext.PolicyKey;
		executionContext.PolicyKey = PolicyKey;
	}

	internal void RestorePolicyContext(Context executionContext, string priorPolicyWrapKey, string priorPolicyKey)
	{
		executionContext.PolicyWrapKey = priorPolicyWrapKey;
		executionContext.PolicyKey = priorPolicyKey;
	}

	internal static ExceptionType GetExceptionType(ExceptionPredicates exceptionPredicates, Exception exception)
	{
		if (exceptionPredicates.FirstMatchOrDefault(exception) == null)
		{
			return ExceptionType.Unhandled;
		}
		return ExceptionType.HandledByThisPolicy;
	}

	internal PolicyBase(ExceptionPredicates exceptionPredicates)
	{
		ExceptionPredicates = exceptionPredicates ?? ExceptionPredicates.None;
	}

	protected PolicyBase(PolicyBuilder policyBuilder)
		: this(policyBuilder?.ExceptionPredicates)
	{
	}
}
public abstract class PolicyBase<TResult> : PolicyBase
{
	protected internal ResultPredicates<TResult> ResultPredicates { get; }

	internal PolicyBase(ExceptionPredicates exceptionPredicates, ResultPredicates<TResult> resultPredicates)
		: base(exceptionPredicates)
	{
		ResultPredicates = resultPredicates ?? ResultPredicates<TResult>.None;
	}

	protected PolicyBase(PolicyBuilder<TResult> policyBuilder)
		: this(policyBuilder?.ExceptionPredicates, policyBuilder?.ResultPredicates)
	{
	}
}
