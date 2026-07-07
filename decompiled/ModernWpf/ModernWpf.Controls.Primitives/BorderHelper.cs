using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class BorderHelper
{
	public static readonly DependencyProperty ChildProperty = DependencyProperty.RegisterAttached("Child", typeof(UIElement), typeof(BorderHelper), new PropertyMetadata((object)null, new PropertyChangedCallback(OnChildChanged)));

	public static UIElement GetChild(Border border)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (UIElement)((DependencyObject)border).GetValue(ChildProperty);
	}

	public static void SetChild(Border border, UIElement value)
	{
		((DependencyObject)border).SetValue(ChildProperty, (object)value);
	}

	private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((Decorator)(Border)d).Child = (UIElement)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
	}
}
