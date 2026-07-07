using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Squirrel.SimpleSplat;

internal class FuncDependencyResolver : IMutableDependencyResolver, IDependencyResolver, IDisposable
{
	private readonly Func<Type, string, IEnumerable<object>> innerGetServices;

	private readonly Action<Func<object>, Type, string> innerRegister;

	private readonly Dictionary<Tuple<Type, string>, List<Action<IDisposable>>> _callbackRegistry = new Dictionary<Tuple<Type, string>, List<Action<IDisposable>>>();

	private IDisposable inner;

	public FuncDependencyResolver(Func<Type, string, IEnumerable<object>> getAllServices, Action<Func<object>, Type, string> register = null, IDisposable toDispose = null)
	{
		innerGetServices = getAllServices;
		innerRegister = register;
		inner = toDispose ?? ActionDisposable.Empty;
	}

	public object GetService(Type serviceType, string contract = null)
	{
		return (GetServices(serviceType, contract) ?? Enumerable.Empty<object>()).LastOrDefault();
	}

	public IEnumerable<object> GetServices(Type serviceType, string contract = null)
	{
		return innerGetServices(serviceType, contract);
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref inner, ActionDisposable.Empty).Dispose();
	}

	public void Register(Func<object> factory, Type serviceType, string contract = null)
	{
		if (innerRegister == null)
		{
			throw new NotImplementedException();
		}
		innerRegister(factory, serviceType, contract);
		Tuple<Type, string> key = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_callbackRegistry.ContainsKey(key))
		{
			return;
		}
		List<Action<IDisposable>> list = null;
		foreach (Action<IDisposable> item in _callbackRegistry[key])
		{
			bool remove = false;
			ActionDisposable obj = new ActionDisposable(delegate
			{
				remove = true;
			});
			item(obj);
			if (remove)
			{
				if (list == null)
				{
					list = new List<Action<IDisposable>>();
				}
				list.Add(item);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (Action<IDisposable> item2 in list)
		{
			_callbackRegistry[key].Remove(item2);
		}
	}

	public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
	{
		Tuple<Type, string> pair = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_callbackRegistry.ContainsKey(pair))
		{
			_callbackRegistry[pair] = new List<Action<IDisposable>>();
		}
		_callbackRegistry[pair].Add(callback);
		return new ActionDisposable(delegate
		{
			_callbackRegistry[pair].Remove(callback);
		});
	}
}
