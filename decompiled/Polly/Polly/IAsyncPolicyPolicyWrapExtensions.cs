using System;
using Polly.Wrap;

namespace Polly;

public static class IAsyncPolicyPolicyWrapExtensions
{
	public static AsyncPolicyWrap WrapAsync(this IAsyncPolicy outerPolicy, IAsyncPolicy innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((AsyncPolicy)outerPolicy).WrapAsync(innerPolicy);
	}

	public static AsyncPolicyWrap<TResult> WrapAsync<TResult>(this IAsyncPolicy outerPolicy, IAsyncPolicy<TResult> innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((AsyncPolicy)outerPolicy).WrapAsync(innerPolicy);
	}

	public static AsyncPolicyWrap<TResult> WrapAsync<TResult>(this IAsyncPolicy<TResult> outerPolicy, IAsyncPolicy innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((AsyncPolicy<TResult>)outerPolicy).WrapAsync(innerPolicy);
	}

	public static AsyncPolicyWrap<TResult> WrapAsync<TResult>(this IAsyncPolicy<TResult> outerPolicy, IAsyncPolicy<TResult> innerPolicy)
	{
		if (outerPolicy == null)
		{
			throw new ArgumentNullException("outerPolicy");
		}
		return ((AsyncPolicy<TResult>)outerPolicy).WrapAsync(innerPolicy);
	}
}
