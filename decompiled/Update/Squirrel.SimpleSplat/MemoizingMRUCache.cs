using System;
using System.Collections.Generic;
using System.Linq;

namespace Squirrel.SimpleSplat;

internal class MemoizingMRUCache<TParam, TVal>
{
	private readonly Func<TParam, object, TVal> calculationFunction;

	private readonly Action<TVal> releaseFunction;

	private readonly int maxCacheSize;

	private LinkedList<TParam> cacheMRUList;

	private Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>> cacheEntries;

	public MemoizingMRUCache(Func<TParam, object, TVal> calculationFunc, int maxSize, Action<TVal> onRelease = null)
	{
		calculationFunction = calculationFunc;
		releaseFunction = onRelease;
		maxCacheSize = maxSize;
		InvalidateAll();
	}

	public TVal Get(TParam key)
	{
		return Get(key, null);
	}

	public TVal Get(TParam key, object context = null)
	{
		if (cacheEntries.ContainsKey(key))
		{
			Tuple<LinkedListNode<TParam>, TVal> tuple = cacheEntries[key];
			cacheMRUList.Remove(tuple.Item1);
			cacheMRUList.AddFirst(tuple.Item1);
			return tuple.Item2;
		}
		TVal val = calculationFunction(key, context);
		LinkedListNode<TParam> linkedListNode = new LinkedListNode<TParam>(key);
		cacheMRUList.AddFirst(linkedListNode);
		cacheEntries[key] = new Tuple<LinkedListNode<TParam>, TVal>(linkedListNode, val);
		maintainCache();
		return val;
	}

	public bool TryGet(TParam key, out TVal result)
	{
		Tuple<LinkedListNode<TParam>, TVal> value;
		bool num = cacheEntries.TryGetValue(key, out value);
		if (num && value != null)
		{
			cacheMRUList.Remove(value.Item1);
			cacheMRUList.AddFirst(value.Item1);
			result = value.Item2;
			return num;
		}
		result = default(TVal);
		return num;
	}

	public void Invalidate(TParam key)
	{
		if (cacheEntries.ContainsKey(key))
		{
			Tuple<LinkedListNode<TParam>, TVal> tuple = cacheEntries[key];
			if (releaseFunction != null)
			{
				releaseFunction(tuple.Item2);
			}
			cacheMRUList.Remove(tuple.Item1);
			cacheEntries.Remove(key);
		}
	}

	public void InvalidateAll()
	{
		if (releaseFunction == null || cacheEntries == null)
		{
			cacheMRUList = new LinkedList<TParam>();
			cacheEntries = new Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>>();
		}
		else if (cacheEntries.Count != 0)
		{
			TParam[] array = cacheEntries.Keys.ToArray();
			foreach (TParam key in array)
			{
				Invalidate(key);
			}
		}
	}

	public IEnumerable<TVal> CachedValues()
	{
		return cacheEntries.Select((KeyValuePair<TParam, Tuple<LinkedListNode<TParam>, TVal>> x) => x.Value.Item2);
	}

	private void maintainCache()
	{
		while (cacheMRUList.Count > maxCacheSize)
		{
			TParam value = cacheMRUList.Last.Value;
			if (releaseFunction != null)
			{
				releaseFunction(cacheEntries[value].Item2);
			}
			cacheEntries.Remove(cacheMRUList.Last.Value);
			cacheMRUList.RemoveLast();
		}
	}

	private void Invariants()
	{
	}
}
