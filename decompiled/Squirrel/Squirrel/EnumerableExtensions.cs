using System;
using System.Collections.Generic;

namespace Squirrel;

internal static class EnumerableExtensions
{
	public static IEnumerable<T> Return<T>(T value)
	{
		yield return value;
	}

	public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onNext == null)
		{
			throw new ArgumentNullException("onNext");
		}
		foreach (TSource item in source)
		{
			onNext(item);
		}
	}

	public static IList<TSource> MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
		return source.MaxBy(keySelector, Comparer<TKey>.Default);
	}

	public static IList<TSource> MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
		if (comparer == null)
		{
			throw new ArgumentNullException("comparer");
		}
		return ExtremaBy(source, keySelector, (TKey key, TKey minValue) => comparer.Compare(key, minValue));
	}

	private static IList<TSource> ExtremaBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
	{
		List<TSource> list = new List<TSource>();
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			throw new InvalidOperationException("Source sequence doesn't contain any elements.");
		}
		TSource current = enumerator.Current;
		TKey arg = keySelector(current);
		list.Add(current);
		while (enumerator.MoveNext())
		{
			TSource current2 = enumerator.Current;
			TKey val = keySelector(current2);
			int num = compare(val, arg);
			if (num == 0)
			{
				list.Add(current2);
			}
			else if (num > 0)
			{
				list = new List<TSource> { current2 };
				arg = val;
			}
		}
		return list;
	}

	public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onNext == null)
		{
			throw new ArgumentNullException("onNext");
		}
		return source.DoHelper(onNext, delegate
		{
		}, delegate
		{
		});
	}

	public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action onCompleted)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onNext == null)
		{
			throw new ArgumentNullException("onNext");
		}
		if (onCompleted == null)
		{
			throw new ArgumentNullException("onCompleted");
		}
		return source.DoHelper(onNext, delegate
		{
		}, onCompleted);
	}

	public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onNext == null)
		{
			throw new ArgumentNullException("onNext");
		}
		if (onError == null)
		{
			throw new ArgumentNullException("onError");
		}
		return source.DoHelper(onNext, onError, delegate
		{
		});
	}

	public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onNext == null)
		{
			throw new ArgumentNullException("onNext");
		}
		if (onError == null)
		{
			throw new ArgumentNullException("onError");
		}
		if (onCompleted == null)
		{
			throw new ArgumentNullException("onCompleted");
		}
		return source.DoHelper(onNext, onError, onCompleted);
	}

	private static IEnumerable<TSource> DoHelper<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
	{
		using IEnumerator<TSource> e = source.GetEnumerator();
		while (true)
		{
			TSource current;
			try
			{
				if (!e.MoveNext())
				{
					break;
				}
				current = e.Current;
				goto IL_0069;
			}
			catch (Exception obj)
			{
				onError(obj);
				throw;
			}
			IL_0069:
			onNext(current);
			yield return current;
		}
		onCompleted();
	}

	public static IEnumerable<TSource> StartWith<TSource>(this IEnumerable<TSource> source, params TSource[] values)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return source.StartWith_(values);
	}

	private static IEnumerable<TSource> StartWith_<TSource>(this IEnumerable<TSource> source, params TSource[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			yield return values[i];
		}
		foreach (TSource item in source)
		{
			yield return item;
		}
	}

	public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
		return source.Distinct_(keySelector, EqualityComparer<TKey>.Default);
	}

	public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
		if (comparer == null)
		{
			throw new ArgumentNullException("comparer");
		}
		return source.Distinct_(keySelector, comparer);
	}

	private static IEnumerable<TSource> Distinct_<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		HashSet<TKey> set = new HashSet<TKey>(comparer);
		foreach (TSource item2 in source)
		{
			TKey item = keySelector(item2);
			if (set.Add(item))
			{
				yield return item2;
			}
		}
	}
}
