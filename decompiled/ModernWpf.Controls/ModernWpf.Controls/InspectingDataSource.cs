using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ModernWpf.Controls;

internal class InspectingDataSource : ItemsSourceView
{
	private readonly IList m_vector;

	private readonly IKeyIndexMapping m_uniqueIdMaping;

	public InspectingDataSource(object source)
		: base(source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (source is IList vector)
		{
			m_vector = vector;
			ListenToCollectionChanges();
		}
		else
		{
			if (!(source is IEnumerable iterable))
			{
				throw new ArgumentException("Argument 'source' is not a supported vector.");
			}
			m_vector = WrapIterable(iterable);
		}
		m_uniqueIdMaping = source as IKeyIndexMapping;
	}

	~InspectingDataSource()
	{
		UnListenToCollectionChanges();
	}

	internal override int GetSizeCore()
	{
		return m_vector.Count;
	}

	internal override object GetAtCore(int index)
	{
		return m_vector[index];
	}

	internal override bool HasKeyIndexMappingCore()
	{
		return m_uniqueIdMaping != null;
	}

	internal override string KeyFromIndexCore(int index)
	{
		if (m_uniqueIdMaping != null)
		{
			return m_uniqueIdMaping.KeyFromIndex(index);
		}
		throw new NotImplementedException();
	}

	internal override int IndexFromKeyCore(string id)
	{
		if (m_uniqueIdMaping != null)
		{
			return m_uniqueIdMaping.IndexFromKey(id);
		}
		throw new NotImplementedException();
	}

	internal override int IndexOfCore(object value)
	{
		int result = -1;
		if (m_vector != null)
		{
			int num = m_vector.IndexOf(value);
			if (num >= 0)
			{
				result = num;
			}
		}
		return result;
	}

	private IList WrapIterable(IEnumerable iterable)
	{
		List<object> list = new List<object>();
		IEnumerator enumerator = iterable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			list.Add(enumerator.Current);
		}
		return list;
	}

	private void UnListenToCollectionChanges()
	{
		if (m_vector is INotifyCollectionChanged notifyCollectionChanged)
		{
			CollectionChangedEventManager.RemoveHandler(notifyCollectionChanged, (EventHandler<NotifyCollectionChangedEventArgs>)OnCollectionChanged);
		}
	}

	private void ListenToCollectionChanges()
	{
		if (m_vector is INotifyCollectionChanged notifyCollectionChanged)
		{
			CollectionChangedEventManager.AddHandler(notifyCollectionChanged, (EventHandler<NotifyCollectionChangedEventArgs>)OnCollectionChanged);
		}
	}

	private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		OnItemsSourceChanged(e);
	}
}
