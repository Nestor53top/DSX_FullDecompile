using System;
using System.Collections.Generic;
using System.Linq;

namespace Squirrel.SimpleSplat;

internal class ModernDependencyResolver : IMutableDependencyResolver, IDependencyResolver, IDisposable
{
	private Dictionary<Tuple<Type, string>, List<Func<object>>> _registry;

	private Dictionary<Tuple<Type, string>, List<Action<IDisposable>>> _callbackRegistry;

	public ModernDependencyResolver()
		: this(null)
	{
	}

	protected ModernDependencyResolver(Dictionary<Tuple<Type, string>, List<Func<object>>> registry)
	{
		_registry = ((registry != null) ? registry.ToDictionary((KeyValuePair<Tuple<Type, string>, List<Func<object>>> k) => k.Key, (KeyValuePair<Tuple<Type, string>, List<Func<object>>> v) => v.Value.ToList()) : new Dictionary<Tuple<Type, string>, List<Func<object>>>());
		_callbackRegistry = new Dictionary<Tuple<Type, string>, List<Action<IDisposable>>>();
	}

	public void Register(Func<object> factory, Type serviceType, string contract = null)
	{
		Tuple<Type, string> key = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_registry.ContainsKey(key))
		{
			_registry[key] = new List<Func<object>>();
		}
		_registry[key].Add(factory);
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

	public object GetService(Type serviceType, string contract = null)
	{
		Tuple<Type, string> key = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_registry.ContainsKey(key))
		{
			return null;
		}
		return _registry[key].Last()();
	}

	public IEnumerable<object> GetServices(Type serviceType, string contract = null)
	{
		Tuple<Type, string> key = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_registry.ContainsKey(key))
		{
			return Enumerable.Empty<object>();
		}
		return _registry[key].Select((Func<object> x) => x()).ToList();
	}

	public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
	{
		Tuple<Type, string> pair = Tuple.Create(serviceType, contract ?? string.Empty);
		if (!_callbackRegistry.ContainsKey(pair))
		{
			_callbackRegistry[pair] = new List<Action<IDisposable>>();
		}
		_callbackRegistry[pair].Add(callback);
		ActionDisposable actionDisposable = new ActionDisposable(delegate
		{
			_callbackRegistry[pair].Remove(callback);
		});
		if (_registry.ContainsKey(pair))
		{
			foreach (Func<object> item in _registry[pair])
			{
				_ = item;
				callback(actionDisposable);
			}
		}
		return actionDisposable;
	}

	public ModernDependencyResolver Duplicate()
	{
		return new ModernDependencyResolver(_registry);
	}

	public void Dispose()
	{
		_registry = null;
	}
}
