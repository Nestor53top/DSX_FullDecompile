using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public static class FlyoutService
{
	public static readonly DependencyProperty FlyoutProperty = DependencyProperty.RegisterAttached("Flyout", typeof(FlyoutBase), typeof(FlyoutService), new PropertyMetadata(new PropertyChangedCallback(OnFlyoutChanged)));

	public static FlyoutBase GetFlyout(Button button)
	{
		return (FlyoutBase)((DependencyObject)button).GetValue(FlyoutProperty);
	}

	public static void SetFlyout(Button button, FlyoutBase value)
	{
		((DependencyObject)button).SetValue(FlyoutProperty, (object)value);
	}

	private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		Button val = (Button)d;
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is FlyoutBase)
		{
			((ButtonBase)val).Click -= new RoutedEventHandler(OnButtonClick);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is FlyoutBase)
		{
			((ButtonBase)val).Click += new RoutedEventHandler(OnButtonClick);
		}
	}

	private static void OnButtonClick(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		GetFlyout(val)?.ShowAt((FrameworkElement)(object)val);
	}
}
