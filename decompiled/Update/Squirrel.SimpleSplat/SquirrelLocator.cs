using System;
using System.Collections.Generic;

namespace Squirrel.SimpleSplat;

internal static class SquirrelLocator
{
	[ThreadStatic]
	private static IDependencyResolver unitTestDependencyResolver;

	private static IDependencyResolver dependencyResolver;

	private static readonly List<Action> resolverChanged;

	public static IDependencyResolver Current
	{
		get
		{
			return unitTestDependencyResolver ?? dependencyResolver;
		}
		set
		{
			if (ModeDetector.InUnitTestRunner())
			{
				unitTestDependencyResolver = value;
				dependencyResolver = dependencyResolver ?? value;
			}
			else
			{
				dependencyResolver = value;
			}
			Action[] array = null;
			lock (resolverChanged)
			{
				array = resolverChanged.ToArray();
			}
			Action[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]();
			}
		}
	}

	public static IMutableDependencyResolver CurrentMutable
	{
		get
		{
			return Current as IMutableDependencyResolver;
		}
		set
		{
			Current = value;
		}
	}

	static SquirrelLocator()
	{
		resolverChanged = new List<Action>();
		dependencyResolver = new ModernDependencyResolver();
		RegisterResolverCallbackChanged(delegate
		{
			if (CurrentMutable != null)
			{
				CurrentMutable.InitializeSplat();
			}
		});
	}

	public static IDisposable RegisterResolverCallbackChanged(Action callback)
	{
		lock (resolverChanged)
		{
			resolverChanged.Add(callback);
		}
		callback();
		return new ActionDisposable(delegate
		{
			lock (resolverChanged)
			{
				resolverChanged.Remove(callback);
			}
		});
	}
}
