using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class TabControlHelper
{
	public static readonly DependencyProperty TabStripHeaderProperty = DependencyProperty.RegisterAttached("TabStripHeader", typeof(object), typeof(TabControlHelper));

	public static readonly DependencyProperty TabStripHeaderTemplateProperty = DependencyProperty.RegisterAttached("TabStripHeaderTemplate", typeof(DataTemplate), typeof(TabControlHelper));

	public static readonly DependencyProperty TabStripFooterProperty = DependencyProperty.RegisterAttached("TabStripFooter", typeof(object), typeof(TabControlHelper));

	public static readonly DependencyProperty TabStripFooterTemplateProperty = DependencyProperty.RegisterAttached("TabStripFooterTemplate", typeof(DataTemplate), typeof(TabControlHelper));

	public static object GetTabStripHeader(TabControl tabControl)
	{
		return ((DependencyObject)tabControl).GetValue(TabStripHeaderProperty);
	}

	public static void SetTabStripHeader(TabControl tabControl, object value)
	{
		((DependencyObject)tabControl).SetValue(TabStripHeaderProperty, value);
	}

	public static DataTemplate GetTabStripHeaderTemplate(TabControl tabControl)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)tabControl).GetValue(TabStripHeaderTemplateProperty);
	}

	public static void SetTabStripHeaderTemplate(TabControl tabControl, DataTemplate value)
	{
		((DependencyObject)tabControl).SetValue(TabStripHeaderTemplateProperty, (object)value);
	}

	public static object GetTabStripFooter(TabControl tabControl)
	{
		return ((DependencyObject)tabControl).GetValue(TabStripFooterProperty);
	}

	public static void SetTabStripFooter(TabControl tabControl, object value)
	{
		((DependencyObject)tabControl).SetValue(TabStripFooterProperty, value);
	}

	public static DataTemplate GetTabStripFooterTemplate(TabControl tabControl)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)tabControl).GetValue(TabStripFooterTemplateProperty);
	}

	public static void SetTabStripFooterTemplate(TabControl tabControl, DataTemplate value)
	{
		((DependencyObject)tabControl).SetValue(TabStripFooterTemplateProperty, (object)value);
	}
}
