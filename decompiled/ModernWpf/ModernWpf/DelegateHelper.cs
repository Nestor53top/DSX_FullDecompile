using System;
using System.Reflection;

namespace ModernWpf;

internal static class DelegateHelper
{
	private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

	public static T CreateDelegate<T>(MethodInfo method) where T : Delegate
	{
		return (T)Delegate.CreateDelegate(typeof(T), method);
	}

	public static T CreateDelegate<T>(object firstArgument, MethodInfo method) where T : Delegate
	{
		return (T)Delegate.CreateDelegate(typeof(T), firstArgument, method);
	}

	public static T CreateDelegate<T>(Type target, string method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) where T : Delegate
	{
		if (bindingAttr != (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
		{
			MethodInfo method2 = target.GetMethod(method, bindingAttr);
			if (method2 != null)
			{
				return CreateDelegate<T>(method2);
			}
			return null;
		}
		return (T)Delegate.CreateDelegate(typeof(T), target, method);
	}

	public static T CreateDelegate<T>(object target, string method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) where T : Delegate
	{
		if (bindingAttr != (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
		{
			MethodInfo method2 = target.GetType().GetMethod(method, bindingAttr);
			if (method2 != null)
			{
				return CreateDelegate<T>(target, method2);
			}
			return null;
		}
		return (T)Delegate.CreateDelegate(typeof(T), target, method);
	}

	public static Func<TType, TProperty> CreatePropertyGetter<TType, TProperty>(string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, bool nonPublic = false)
	{
		PropertyInfo property = typeof(TType).GetProperty(name, bindingAttr);
		if (property != null)
		{
			MethodInfo getMethod = property.GetGetMethod(nonPublic);
			if (getMethod != null)
			{
				return CreateDelegate<Func<TType, TProperty>>(getMethod);
			}
		}
		return null;
	}

	public static Action<TType, TProperty> CreatePropertySetter<TType, TProperty>(string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, bool nonPublic = false)
	{
		PropertyInfo property = typeof(TType).GetProperty(name, bindingAttr);
		if (property != null)
		{
			MethodInfo setMethod = property.GetSetMethod(nonPublic);
			if (setMethod != null)
			{
				return CreateDelegate<Action<TType, TProperty>>(setMethod);
			}
		}
		return null;
	}
}
