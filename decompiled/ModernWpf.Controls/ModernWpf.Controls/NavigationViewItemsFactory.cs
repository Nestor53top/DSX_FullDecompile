using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

internal class NavigationViewItemsFactory : ElementFactory
{
	private IElementFactoryShim m_itemTemplateWrapper;

	private NavigationViewItemBase m_settingsItem;

	private List<NavigationViewItem> navigationViewItemPool = new List<NavigationViewItem>();

	public void UserElementFactory(object newValue)
	{
		m_itemTemplateWrapper = newValue as IElementFactoryShim;
		if (m_itemTemplateWrapper == null)
		{
			DataTemplate val = (DataTemplate)((newValue is DataTemplate) ? newValue : null);
			if (val != null)
			{
				m_itemTemplateWrapper = new ItemTemplateWrapper(val);
			}
			else
			{
				DataTemplateSelector val2 = (DataTemplateSelector)((newValue is DataTemplateSelector) ? newValue : null);
				if (val2 != null)
				{
					m_itemTemplateWrapper = new ItemTemplateWrapper(val2);
				}
			}
		}
		navigationViewItemPool = new List<NavigationViewItem>();
	}

	internal void SettingsItem(NavigationViewItemBase settingsItem)
	{
		m_settingsItem = settingsItem;
	}

	protected override UIElement GetElementCore(ElementFactoryGetArgs args)
	{
		object obj = init();
		if (obj is NavigationViewItemBase result)
		{
			return (UIElement)(object)result;
		}
		NavigationViewItem navigationViewItem = init2();
		navigationViewItem.CreatedByNavigationViewItemsFactory = true;
		if (m_itemTemplateWrapper != null && m_itemTemplateWrapper is ItemTemplateWrapper itemTemplateWrapper)
		{
			ElementFactoryRecycleArgs elementFactoryRecycleArgs = new ElementFactoryRecycleArgs();
			elementFactoryRecycleArgs.Element = (UIElement)((obj is UIElement) ? obj : null);
			m_itemTemplateWrapper.RecycleElement(elementFactoryRecycleArgs);
			((ContentControl)navigationViewItem).Content = args.Data;
			((ContentControl)navigationViewItem).ContentTemplate = itemTemplateWrapper.Template;
			((ContentControl)navigationViewItem).ContentTemplateSelector = itemTemplateWrapper.TemplateSelector;
			return (UIElement)(object)navigationViewItem;
		}
		((ContentControl)navigationViewItem).Content = obj;
		return (UIElement)(object)navigationViewItem;
		object init()
		{
			if (m_settingsItem != null && m_settingsItem == args.Data)
			{
				return args.Data;
			}
			if (m_itemTemplateWrapper != null)
			{
				return m_itemTemplateWrapper.GetElement(args);
			}
			return args.Data;
		}
		NavigationViewItem init2()
		{
			if (navigationViewItemPool.Count > 0)
			{
				NavigationViewItem result2 = navigationViewItemPool.Last();
				navigationViewItemPool.RemoveLast();
				return result2;
			}
			return new NavigationViewItem();
		}
	}

	protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
	{
		UIElement element = args.Element;
		if (element == null)
		{
			return;
		}
		if (element is NavigationViewItem navigationViewItem)
		{
			NavigationViewItem navigationViewItem2 = navigationViewItem;
			if (navigationViewItem2.CreatedByNavigationViewItemsFactory)
			{
				navigationViewItem2.CreatedByNavigationViewItemsFactory = false;
				UnlinkElementFromParent(args);
				args.Element = null;
				navigationViewItemPool.Add(navigationViewItem);
				_ = m_itemTemplateWrapper;
			}
		}
		bool flag = m_settingsItem != null && (object)m_settingsItem == args.Element;
		if (m_itemTemplateWrapper != null && !flag)
		{
			m_itemTemplateWrapper.RecycleElement(args);
		}
		else
		{
			UnlinkElementFromParent(args);
		}
	}

	private void UnlinkElementFromParent(ElementFactoryRecycleArgs args)
	{
		UIElement parent = args.Parent;
		Panel val = (Panel)(object)((parent is Panel) ? parent : null);
		if (val != null)
		{
			UIElementCollection children = val.Children;
			int index = 0;
			if (children.IndexOf(args.Element, out index))
			{
				children.RemoveAt(index);
			}
		}
	}
}
