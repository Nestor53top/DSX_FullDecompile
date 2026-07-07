using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal static class EnumerableExtensions
{
	internal static IEnumerable<TElement> DistinctLast<TElement>(this IEnumerable<TElement> source, IEqualityComparer<TElement> equalityComparer, IComparer<TElement> comparer)
	{
		bool flag = true;
		bool flag2 = false;
		TElement y = default(TElement);
		TElement maxElement = default(TElement);
		foreach (TElement element in source)
		{
			if (!flag && !equalityComparer.Equals(element, y))
			{
				yield return maxElement;
				flag2 = false;
			}
			if (!flag2 || (flag2 && comparer.Compare(maxElement, element) < 0))
			{
				maxElement = element;
				flag2 = true;
			}
			y = element;
			flag = false;
		}
		if (!flag)
		{
			yield return maxElement;
		}
	}

	internal static IEnumerable<TElement> SafeIterate<TElement>(this IEnumerable<TElement> source)
	{
		List<TElement> list = new List<TElement>();
		using (IEnumerator<TElement> enumerator = source.GetEnumerator())
		{
			bool flag = true;
			while (flag)
			{
				try
				{
					flag = enumerator.MoveNext();
					if (!flag)
					{
						break;
					}
					list.Add(enumerator.Current);
					continue;
				}
				catch
				{
				}
				break;
			}
		}
		return list;
	}

	public static bool IsEmpty<T>(this IEnumerable<T> sequence)
	{
		if (sequence != null)
		{
			return !sequence.Any();
		}
		return true;
	}
}
