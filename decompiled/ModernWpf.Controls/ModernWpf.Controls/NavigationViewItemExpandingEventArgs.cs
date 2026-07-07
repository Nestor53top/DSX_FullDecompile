using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class NavigationViewItemExpandingEventArgs : EventArgs
{
	private object m_expandingItem;

	private NavigationView m_navigationView;

	public NavigationViewItemBase ExpandingItemContainer { get; internal set; }

	public object ExpandingItem
	{
		get
		{
			if (m_expandingItem != null)
			{
				return m_expandingItem;
			}
			NavigationView navigationView = m_navigationView;
			if (navigationView != null)
			{
				m_expandingItem = navigationView.MenuItemFromContainer((DependencyObject)(object)ExpandingItemContainer);
				return m_expandingItem;
			}
			return null;
		}
	}

	internal NavigationViewItemExpandingEventArgs(NavigationView navigationView)
	{
		m_navigationView = navigationView;
	}
}
