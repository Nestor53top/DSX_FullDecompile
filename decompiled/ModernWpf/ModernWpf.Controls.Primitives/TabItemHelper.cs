using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class TabItemHelper
{
	public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(TabItemHelper));

	public static object GetIcon(TabItem tabItem)
	{
		return ((DependencyObject)tabItem).GetValue(IconProperty);
	}

	public static void SetIcon(TabItem tabItem, object value)
	{
		((DependencyObject)tabItem).SetValue(IconProperty, value);
	}
}
