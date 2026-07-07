using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Squirrel.Json;

[GeneratedCode("simple-json", "1.0.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
internal class JsonObject : DynamicObject, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
{
	private readonly Dictionary<string, object> _members;

	public object this[int index] => GetAtIndex(_members, index);

	public ICollection<string> Keys => _members.Keys;

	public ICollection<object> Values => _members.Values;

	public object this[string key]
	{
		get
		{
			return _members[key];
		}
		set
		{
			_members[key] = value;
		}
	}

	public int Count => _members.Count;

	public bool IsReadOnly => false;

	public JsonObject()
	{
		_members = new Dictionary<string, object>();
	}

	public JsonObject(IEqualityComparer<string> comparer)
	{
		_members = new Dictionary<string, object>(comparer);
	}

	internal static object GetAtIndex(IDictionary<string, object> obj, int index)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (index >= obj.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		int num = 0;
		foreach (KeyValuePair<string, object> item in obj)
		{
			if (num++ == index)
			{
				return item.Value;
			}
		}
		return null;
	}

	public void Add(string key, object value)
	{
		_members.Add(key, value);
	}

	public bool ContainsKey(string key)
	{
		return _members.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		return _members.Remove(key);
	}

	public bool TryGetValue(string key, out object value)
	{
		return _members.TryGetValue(key, out value);
	}

	public void Add(KeyValuePair<string, object> item)
	{
		_members.Add(item.Key, item.Value);
	}

	public void Clear()
	{
		_members.Clear();
	}

	public bool Contains(KeyValuePair<string, object> item)
	{
		if (_members.ContainsKey(item.Key))
		{
			return _members[item.Key] == item.Value;
		}
		return false;
	}

	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		int num = Count;
		using IEnumerator<KeyValuePair<string, object>> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, object> current = enumerator.Current;
			array[arrayIndex++] = current;
			if (--num <= 0)
			{
				break;
			}
		}
	}

	public bool Remove(KeyValuePair<string, object> item)
	{
		return _members.Remove(item.Key);
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
	{
		return _members.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _members.GetEnumerator();
	}

	public override string ToString()
	{
		return SimpleJson.SerializeObject(this);
	}

	public override bool TryConvert(ConvertBinder binder, out object result)
	{
		if (binder == null)
		{
			throw new ArgumentNullException("binder");
		}
		Type type = binder.Type;
		if (type == typeof(IEnumerable) || type == typeof(IEnumerable<KeyValuePair<string, object>>) || type == typeof(IDictionary<string, object>) || type == typeof(IDictionary))
		{
			result = this;
			return true;
		}
		return base.TryConvert(binder, out result);
	}

	public override bool TryDeleteMember(DeleteMemberBinder binder)
	{
		if (binder == null)
		{
			throw new ArgumentNullException("binder");
		}
		return _members.Remove(binder.Name);
	}

	public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
	{
		if (indexes == null)
		{
			throw new ArgumentNullException("indexes");
		}
		if (indexes.Length == 1)
		{
			result = ((IDictionary<string, object>)this)[(string)indexes[0]];
			return true;
		}
		result = null;
		return true;
	}

	public override bool TryGetMember(GetMemberBinder binder, out object result)
	{
		if (_members.TryGetValue(binder.Name, out var value))
		{
			result = value;
			return true;
		}
		result = null;
		return true;
	}

	public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
	{
		if (indexes == null)
		{
			throw new ArgumentNullException("indexes");
		}
		if (indexes.Length == 1)
		{
			((IDictionary<string, object>)this)[(string)indexes[0]] = value;
			return true;
		}
		return base.TrySetIndex(binder, indexes, value);
	}

	public override bool TrySetMember(SetMemberBinder binder, object value)
	{
		if (binder == null)
		{
			throw new ArgumentNullException("binder");
		}
		_members[binder.Name] = value;
		return true;
	}

	public override IEnumerable<string> GetDynamicMemberNames()
	{
		foreach (string key in Keys)
		{
			yield return key;
		}
	}
}
