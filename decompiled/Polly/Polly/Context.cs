using System;
using System.Collections;
using System.Collections.Generic;

namespace Polly;

public class Context : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<string, object>, IReadOnlyCollection<KeyValuePair<string, object>>
{
	private Guid? _correlationId;

	private Dictionary<string, object> wrappedDictionary;

	public string PolicyWrapKey { get; internal set; }

	public string PolicyKey { get; internal set; }

	public string OperationKey { get; }

	public Guid CorrelationId
	{
		get
		{
			if (!_correlationId.HasValue)
			{
				_correlationId = Guid.NewGuid();
			}
			return _correlationId.Value;
		}
	}

	private Dictionary<string, object> WrappedDictionary => wrappedDictionary ?? (wrappedDictionary = new Dictionary<string, object>());

	public ICollection<string> Keys => WrappedDictionary.Keys;

	public ICollection<object> Values => WrappedDictionary.Values;

	public int Count => WrappedDictionary.Count;

	bool ICollection<KeyValuePair<string, object>>.IsReadOnly => ((ICollection<KeyValuePair<string, object>>)WrappedDictionary).IsReadOnly;

	public object this[string key]
	{
		get
		{
			return WrappedDictionary[key];
		}
		set
		{
			WrappedDictionary[key] = value;
		}
	}

	IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => ((IReadOnlyDictionary<string, object>)WrappedDictionary).Keys;

	IEnumerable<object> IReadOnlyDictionary<string, object>.Values => ((IReadOnlyDictionary<string, object>)WrappedDictionary).Values;

	bool IDictionary.IsFixedSize => ((IDictionary)WrappedDictionary).IsFixedSize;

	bool IDictionary.IsReadOnly => ((IDictionary)WrappedDictionary).IsReadOnly;

	ICollection IDictionary.Keys => ((IDictionary)WrappedDictionary).Keys;

	ICollection IDictionary.Values => ((IDictionary)WrappedDictionary).Values;

	bool ICollection.IsSynchronized => ((ICollection)WrappedDictionary).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)WrappedDictionary).SyncRoot;

	object IDictionary.this[object key]
	{
		get
		{
			return ((IDictionary)WrappedDictionary)[key];
		}
		set
		{
			((IDictionary)WrappedDictionary)[key] = value;
		}
	}

	internal static Context None()
	{
		return new Context();
	}

	public Context(string operationKey)
	{
		OperationKey = operationKey;
	}

	public Context()
	{
	}

	public Context(string operationKey, IDictionary<string, object> contextData)
		: this(contextData)
	{
		OperationKey = operationKey;
	}

	internal Context(IDictionary<string, object> contextData)
		: this()
	{
		if (contextData == null)
		{
			throw new ArgumentNullException("contextData");
		}
		wrappedDictionary = new Dictionary<string, object>(contextData);
	}

	public void Add(string key, object value)
	{
		WrappedDictionary.Add(key, value);
	}

	public bool ContainsKey(string key)
	{
		return WrappedDictionary.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		return WrappedDictionary.Remove(key);
	}

	public bool TryGetValue(string key, out object value)
	{
		return WrappedDictionary.TryGetValue(key, out value);
	}

	void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
	{
		((ICollection<KeyValuePair<string, object>>)WrappedDictionary).Add(item);
	}

	public void Clear()
	{
		WrappedDictionary.Clear();
	}

	bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
	{
		return ((ICollection<KeyValuePair<string, object>>)WrappedDictionary).Contains(item);
	}

	void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<string, object>>)WrappedDictionary).CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
	{
		return ((ICollection<KeyValuePair<string, object>>)WrappedDictionary).Remove(item);
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
	{
		return WrappedDictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return WrappedDictionary.GetEnumerator();
	}

	public void Add(object key, object value)
	{
		((IDictionary)WrappedDictionary).Add(key, value);
	}

	public bool Contains(object key)
	{
		return ((IDictionary)WrappedDictionary).Contains(key);
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IDictionary)WrappedDictionary).GetEnumerator();
	}

	public void Remove(object key)
	{
		((IDictionary)WrappedDictionary).Remove(key);
	}

	public void CopyTo(Array array, int index)
	{
		((ICollection)WrappedDictionary).CopyTo(array, index);
	}
}
