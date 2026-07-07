using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal static class CollectionExtensions
{
	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			collection.Add(item);
		}
	}

	public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> match)
	{
		IList<T> list = collection.Where(match).ToList();
		foreach (T item in list)
		{
			collection.Remove(item);
		}
		return list.Count;
	}
}
