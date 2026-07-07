using System.Collections.Generic;

namespace ModernWpf.Controls;

internal static class CollectionHelper
{
	public static bool contains<T>(IList<T> c, T v)
	{
		return c.Contains(v);
	}

	public static void unique_push_back<T>(IList<T> c, T v)
	{
		if (!c.Contains(v))
		{
			c.Add(v);
		}
	}
}
