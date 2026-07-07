using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class TextControlContentHostHelper
{
	public static readonly DependencyProperty ContentPresenterMarginProperty = DependencyProperty.RegisterAttached("ContentPresenterMargin", typeof(Thickness), typeof(TextControlContentHostHelper));

	public static Thickness GetContentPresenterMargin(ScrollViewer contentHost)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Thickness)((DependencyObject)contentHost).GetValue(ContentPresenterMarginProperty);
	}

	public static void SetContentPresenterMargin(ScrollViewer contentHost, Thickness value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)contentHost).SetValue(ContentPresenterMarginProperty, (object)value);
	}
}
