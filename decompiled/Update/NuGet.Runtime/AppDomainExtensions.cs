using System;

namespace NuGet.Runtime;

internal static class AppDomainExtensions
{
	public static T CreateInstance<T>(this AppDomain domain)
	{
		return (T)domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
	}
}
