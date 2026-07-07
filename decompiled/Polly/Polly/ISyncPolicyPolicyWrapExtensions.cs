using System;
using Polly.Wrap;

namespace Polly;

public static class ISyncPolicyPolicyWrapExtensions
{
	public static PolicyWrap Wrap(this ISyncPolicy outerPolicy, ISyncPolicy innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((Policy)outerPolicy).Wrap(innerPolicy);
	}

	public static PolicyWrap<TResult> Wrap<TResult>(this ISyncPolicy outerPolicy, ISyncPolicy<TResult> innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((Policy)outerPolicy).Wrap(innerPolicy);
	}

	public static PolicyWrap<TResult> Wrap<TResult>(this ISyncPolicy<TResult> outerPolicy, ISyncPolicy innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((Policy<TResult>)outerPolicy).Wrap(innerPolicy);
	}

	public static PolicyWrap<TResult> Wrap<TResult>(this ISyncPolicy<TResult> outerPolicy, ISyncPolicy<TResult> innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((Policy<TResult>)outerPolicy).Wrap(innerPolicy);
	}
}
