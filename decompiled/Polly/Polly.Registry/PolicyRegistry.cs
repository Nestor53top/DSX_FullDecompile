using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Polly.Registry;

public class PolicyRegistry : IConcurrentPolicyRegistry<string>, IPolicyRegistry<string>, IReadOnlyPolicyRegistry<string>, IEnumerable<KeyValuePair<string, IsPolicy>>, IEnumerable
{
	private readonly IDictionary<string, IsPolicy> _registry = new ConcurrentDictionary<string, IsPolicy>();

	public int Count => _registry.Count;

	public IsPolicy this[string key]
	{
		get
		{
			return _registry[key];
		}
		set
		{
			_registry[key] = value;
		}
	}

	public PolicyRegistry()
	{
	}

	internal PolicyRegistry(IDictionary<string, IsPolicy> registry)
	{
		_registry = registry ?? throw new NullReferenceException("registry");
	}

	private ConcurrentDictionary<string, IsPolicy> ThrowIfNotConcurrentImplementation()
	{
		if (_registry is ConcurrentDictionary<string, IsPolicy> result)
		{
			return result;
		}
		throw new InvalidOperationException("This PolicyRegistry is not configured for concurrent operations. This exception should never be thrown in production code as the only public constructors create PolicyRegistry instances of the correct form.");
	}

	public void Add<TPolicy>(string key, TPolicy policy) where TPolicy : IsPolicy
	{
		_registry.Add(key, policy);
	}

	public bool TryAdd<TPolicy>(string key, TPolicy policy) where TPolicy : IsPolicy
	{
		return ThrowIfNotConcurrentImplementation().TryAdd(key, policy);
	}

	public TPolicy Get<TPolicy>(string key) where TPolicy : IsPolicy
	{
		return (TPolicy)_registry[key];
	}

	public bool TryGet<TPolicy>(string key, out TPolicy policy) where TPolicy : IsPolicy
	{
		IsPolicy value;
		bool flag = _registry.TryGetValue(key, out value);
		policy = (flag ? ((TPolicy)value) : default(TPolicy));
		return flag;
	}

	public void Clear()
	{
		_registry.Clear();
	}

	public bool ContainsKey(string key)
	{
		return _registry.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		return _registry.Remove(key);
	}

	public bool TryRemove<TPolicy>(string key, out TPolicy policy) where TPolicy : IsPolicy
	{
		IsPolicy value;
		bool flag = ThrowIfNotConcurrentImplementation().TryRemove(key, out value);
		policy = (flag ? ((TPolicy)value) : default(TPolicy));
		return flag;
	}

	public bool TryUpdate<TPolicy>(string key, TPolicy newPolicy, TPolicy comparisonPolicy) where TPolicy : IsPolicy
	{
		return ThrowIfNotConcurrentImplementation().TryUpdate(key, newPolicy, comparisonPolicy);
	}

	public TPolicy GetOrAdd<TPolicy>(string key, Func<string, TPolicy> policyFactory) where TPolicy : IsPolicy
	{
		return (TPolicy)ThrowIfNotConcurrentImplementation().GetOrAdd(key, (string k) => policyFactory(k));
	}

	public TPolicy GetOrAdd<TPolicy>(string key, TPolicy policy) where TPolicy : IsPolicy
	{
		return (TPolicy)ThrowIfNotConcurrentImplementation().GetOrAdd(key, policy);
	}

	public TPolicy AddOrUpdate<TPolicy>(string key, Func<string, TPolicy> addPolicyFactory, Func<string, TPolicy, TPolicy> updatePolicyFactory) where TPolicy : IsPolicy
	{
		return (TPolicy)ThrowIfNotConcurrentImplementation().AddOrUpdate(key, (string k) => addPolicyFactory(k), (string k, IsPolicy e) => updatePolicyFactory(k, (TPolicy)e));
	}

	public TPolicy AddOrUpdate<TPolicy>(string key, TPolicy addPolicy, Func<string, TPolicy, TPolicy> updatePolicyFactory) where TPolicy : IsPolicy
	{
		return (TPolicy)ThrowIfNotConcurrentImplementation().AddOrUpdate(key, addPolicy, (string k, IsPolicy e) => updatePolicyFactory(k, (TPolicy)e));
	}

	public IEnumerator<KeyValuePair<string, IsPolicy>> GetEnumerator()
	{
		return _registry.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
