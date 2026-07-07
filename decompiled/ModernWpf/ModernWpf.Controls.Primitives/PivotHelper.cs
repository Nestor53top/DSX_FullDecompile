using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class PivotHelper
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(object), typeof(PivotHelper));

	public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.RegisterAttached("TitleTemplate", typeof(DataTemplate), typeof(PivotHelper));

	public static readonly DependencyProperty LeftHeaderProperty = DependencyProperty.RegisterAttached("LeftHeader", typeof(object), typeof(PivotHelper));

	public static readonly DependencyProperty LeftHeaderTemplateProperty = DependencyProperty.RegisterAttached("LeftHeaderTemplate", typeof(DataTemplate), typeof(PivotHelper));

	public static readonly DependencyProperty RightHeaderProperty = DependencyProperty.RegisterAttached("RightHeader", typeof(object), typeof(PivotHelper));

	public static readonly DependencyProperty RightHeaderTemplateProperty = DependencyProperty.RegisterAttached("RightHeaderTemplate", typeof(DataTemplate), typeof(PivotHelper));

	public static object GetTitle(TabControl tabControl)
	{
		return ((DependencyObject)tabControl).GetValue(TitleProperty);
	}

	public static void SetTitle(TabControl tabControl, object value)
	{
		((DependencyObject)tabControl).SetValue(TitleProperty, value);
	}

	public static DataTemplate GetTitleTemplate(TabControl tabControl)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)tabControl).GetValue(TitleTemplateProperty);
	}

	public static void SetTitleTemplate(TabControl tabControl, DataTemplate value)
	{
		((DependencyObject)tabControl).SetValue(TitleTemplateProperty, (object)value);
	}

	public static object GetLeftHeader(TabControl tabControl)
	{
		return ((DependencyObject)tabControl).GetValue(LeftHeaderProperty);
	}

	public static void SetLeftHeader(TabControl tabControl, object value)
	{
		((DependencyObject)tabControl).SetValue(LeftHeaderProperty, value);
	}

	public static DataTemplate GetLeftHeaderTemplate(TabControl tabControl)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)tabControl).GetValue(LeftHeaderTemplateProperty);
	}

	public static void SetLeftHeaderTemplate(TabControl tabControl, DataTemplate value)
	{
		((DependencyObject)tabControl).SetValue(LeftHeaderTemplateProperty, (object)value);
	}

	public static object GetRightHeader(TabControl tabControl)
	{
		return ((DependencyObject)tabControl).GetValue(RightHeaderProperty);
	}

	public static void SetRightHeader(TabControl tabControl, object value)
	{
		((DependencyObject)tabControl).SetValue(RightHeaderProperty, value);
	}

	public static DataTemplate GetRightHeaderTemplate(TabControl tabControl)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)tabControl).GetValue(RightHeaderTemplateProperty);
	}

	public static void SetRightHeaderTemplate(TabControl tabControl, DataTemplate value)
	{
		((DependencyObject)tabControl).SetValue(RightHeaderTemplateProperty, (object)value);
	}
}
