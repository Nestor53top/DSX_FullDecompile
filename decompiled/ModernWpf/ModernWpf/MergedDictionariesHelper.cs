using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernWpf;

internal static class MergedDictionariesHelper
{
	public static void AddIfNotNull(this Collection<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
	{
		if (item != null)
		{
			mergedDictionaries.Add(item);
		}
	}

	public static void RemoveIfNotNull(this Collection<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
	{
		if (item != null)
		{
			mergedDictionaries.Remove(item);
		}
	}

	public static void InsertOrReplace(this Collection<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
	{
		if (mergedDictionaries.Count > index)
		{
			mergedDictionaries[index] = item;
		}
		else
		{
			mergedDictionaries.Insert(index, item);
		}
	}

	public static void RemoveAll<T>(this Collection<ResourceDictionary> mergedDictionaries) where T : ResourceDictionary
	{
		for (int num = mergedDictionaries.Count - 1; num >= 0; num--)
		{
			if (mergedDictionaries[num] is T)
			{
				mergedDictionaries.RemoveAt(num);
			}
		}
	}

	public static void InsertIfNotExists(this Collection<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
	{
		if (!mergedDictionaries.Contains(item))
		{
			mergedDictionaries.Insert(index, item);
		}
	}

	public static void Swap(this Collection<ResourceDictionary> mergedDictionaries, int index1, int index2)
	{
		if (index1 != index2)
		{
			int index3 = Math.Min(index1, index2);
			int index4 = Math.Max(index1, index2);
			ResourceDictionary item = mergedDictionaries[index3];
			mergedDictionaries.RemoveAt(index3);
			mergedDictionaries.Insert(index4, item);
		}
	}
}
