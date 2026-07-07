using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class NavigationViewItemCollapsedEventArgs : EventArgs
{
	private object m_collapsedItem;

	private NavigationView m_navigationView;

	public NavigationViewItemBase CollapsedItemContainer { get; internal set; }

	public object CollapsedItem
	{
		get
		{
			if (m_collapsedItem != null)
			{
				return m_collapsedItem;
			}
			NavigationView navigationView = m_navigationView;
			if (navigationView != null)
			{
				m_collapsedItem = navigationView.MenuItemFromContainer((DependencyObject)(object)CollapsedItemContainer);
				return m_collapsedItem;
			}
			return null;
		}
	}

	internal NavigationViewItemCollapsedEventArgs(NavigationView navigationView)
	{
		m_navigationView = navigationView;
	}
}
