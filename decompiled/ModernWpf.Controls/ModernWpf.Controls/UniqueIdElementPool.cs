using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls;

internal class UniqueIdElementPool : IEnumerable<KeyValuePair<string, UIElement>>, IEnumerable
{
	private readonly ItemsRepeater m_owner;

	private readonly Dictionary<string, UIElement> m_elementMap = new Dictionary<string, UIElement>();

	public UniqueIdElementPool(ItemsRepeater owner)
	{
		m_owner = owner;
	}

	public void Add(UIElement element)
	{
		VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(element);
		string uniqueId = virtualizationInfo.UniqueId;
		if (m_elementMap.ContainsKey(uniqueId))
		{
			throw new Exception("The unique id provided (" + virtualizationInfo.UniqueId + ") is not unique.");
		}
		m_elementMap.Add(uniqueId, element);
	}

	public UIElement Remove(int index)
	{
		string key = m_owner.ItemsSourceView.KeyFromIndex(index);
		if (m_elementMap.TryGetValue(key, out var value))
		{
			m_elementMap.Remove(key);
		}
		return value;
	}

	public void Clear()
	{
		m_elementMap.Clear();
	}

	public IEnumerator<KeyValuePair<string, UIElement>> GetEnumerator()
	{
		return m_elementMap.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
