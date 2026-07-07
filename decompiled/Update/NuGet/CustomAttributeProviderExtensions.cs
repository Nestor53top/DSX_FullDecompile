using System;
using System.Linq;
using System.Reflection;

namespace NuGet;

internal static class CustomAttributeProviderExtensions
{
	public static T GetCustomAttribute<T>(this ICustomAttributeProvider attributeProvider)
	{
		return (T)attributeProvider.GetCustomAttribute(typeof(T));
	}

	public static object GetCustomAttribute(this ICustomAttributeProvider attributeProvider, Type type)
	{
		return attributeProvider.GetCustomAttributes(type, inherit: false).FirstOrDefault();
	}
}
