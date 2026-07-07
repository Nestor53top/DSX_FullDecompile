using System;
using System.Collections.Generic;
using System.Linq;

namespace Polly.Wrap;

public static class IPolicyWrapExtension
{
	public static IEnumerable<IsPolicy> GetPolicies(this IPolicyWrap policyWrap)
	{
		IsPolicy[] array = new IsPolicy[2] { policyWrap.Outer, policyWrap.Inner };
		IsPolicy[] array2 = array;
		foreach (IsPolicy isPolicy in array2)
		{
			if (isPolicy is IPolicyWrap policyWrap2)
			{
				foreach (IsPolicy policy in policyWrap2.GetPolicies())
				{
					yield return policy;
				}
			}
			else if (isPolicy != null)
			{
				yield return isPolicy;
			}
		}
	}

	public static IEnumerable<TPolicy> GetPolicies<TPolicy>(this IPolicyWrap policyWrap)
	{
		return policyWrap.GetPolicies().OfType<TPolicy>();
	}

	public static IEnumerable<TPolicy> GetPolicies<TPolicy>(this IPolicyWrap policyWrap, Func<TPolicy, bool> filter)
	{
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		return policyWrap.GetPolicies().OfType<TPolicy>().Where(filter);
	}

	public static TPolicy GetPolicy<TPolicy>(this IPolicyWrap policyWrap)
	{
		return policyWrap.GetPolicies().OfType<TPolicy>().SingleOrDefault();
	}

	public static TPolicy GetPolicy<TPolicy>(this IPolicyWrap policyWrap, Func<TPolicy, bool> filter)
	{
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		return policyWrap.GetPolicies().OfType<TPolicy>().SingleOrDefault(filter);
	}
}
