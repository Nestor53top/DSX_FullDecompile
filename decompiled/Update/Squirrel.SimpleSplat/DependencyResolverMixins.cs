using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Squirrel.SimpleSplat;

internal static class DependencyResolverMixins
{
	public static T GetService<T>(this IDependencyResolver This, string contract = null)
	{
		return (T)This.GetService(typeof(T), contract);
	}

	public static IEnumerable<T> GetServices<T>(this IDependencyResolver This, string contract = null)
	{
		return This.GetServices(typeof(T), contract).Cast<T>();
	}

	public static IDisposable ServiceRegistrationCallback(this IMutableDependencyResolver This, Type serviceType, Action<IDisposable> callback)
	{
		return This.ServiceRegistrationCallback(serviceType, null, callback);
	}

	public static IDisposable WithResolver(this IDependencyResolver resolver)
	{
		IDependencyResolver origResolver = SquirrelLocator.Current;
		SquirrelLocator.Current = resolver;
		return new ActionDisposable(delegate
		{
			SquirrelLocator.Current = origResolver;
		});
	}

	public static void RegisterConstant(this IMutableDependencyResolver This, object value, Type serviceType, string contract = null)
	{
		This.Register(() => value, serviceType, contract);
	}

	public static void RegisterLazySingleton(this IMutableDependencyResolver This, Func<object> valueFactory, Type serviceType, string contract = null)
	{
		Lazy<object> val = new Lazy<object>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
		This.Register(() => val.Value, serviceType, contract);
	}

	public static void InitializeSplat(this IMutableDependencyResolver This)
	{
		This.Register(() => new DefaultLogManager(), typeof(ILogManager));
		This.Register(() => new DebugLogger(), typeof(ILogger));
	}
}
