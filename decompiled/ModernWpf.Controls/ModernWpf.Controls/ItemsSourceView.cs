using System;
using System.Collections.Specialized;

namespace ModernWpf.Controls;

public class ItemsSourceView(object source) : INotifyCollectionChanged
{
	internal class CollectionChangedRevoker : EventRevoker<ItemsSourceView, NotifyCollectionChangedEventHandler>
	{
		public CollectionChangedRevoker(ItemsSourceView source, NotifyCollectionChangedEventHandler handler)
			: base(source, handler)
		{
		}

		protected override void AddHandler(ItemsSourceView source, NotifyCollectionChangedEventHandler handler)
		{
			source.CollectionChanged += handler;
		}

		protected override void RemoveHandler(ItemsSourceView source, NotifyCollectionChangedEventHandler handler)
		{
			source.CollectionChanged -= handler;
		}
	}

	private int m_cachedSize = -1;

	public int Count
	{
		get
		{
			if (m_cachedSize == -1)
			{
				m_cachedSize = GetSizeCore();
			}
			return m_cachedSize;
		}
	}

	public bool HasKeyIndexMapping => HasKeyIndexMappingCore();

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public object GetAt(int index)
	{
		return GetAtCore(index);
	}

	public string KeyFromIndex(int index)
	{
		return KeyFromIndexCore(index);
	}

	public int IndexFromKey(string key)
	{
		return IndexFromKeyCore(key);
	}

	public int IndexOf(object value)
	{
		return IndexOfCore(value);
	}

	internal void OnItemsSourceChanged(NotifyCollectionChangedEventArgs args)
	{
		m_cachedSize = GetSizeCore();
		this.CollectionChanged?.Invoke(this, args);
	}

	internal virtual int GetSizeCore()
	{
		throw new NotImplementedException();
	}

	internal virtual object GetAtCore(int index)
	{
		throw new NotImplementedException();
	}

	internal virtual bool HasKeyIndexMappingCore()
	{
		throw new NotImplementedException();
	}

	internal virtual string KeyFromIndexCore(int index)
	{
		throw new NotImplementedException();
	}

	internal virtual int IndexFromKeyCore(string id)
	{
		throw new NotImplementedException();
	}

	internal virtual int IndexOfCore(object value)
	{
		throw new NotImplementedException();
	}
}
