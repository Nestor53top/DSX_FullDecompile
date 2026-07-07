using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public static class ContextFlyoutService
{
	public static readonly DependencyProperty ContextFlyoutProperty = DependencyProperty.RegisterAttached("ContextFlyout", typeof(FlyoutBase), typeof(ContextFlyoutService), new PropertyMetadata(new PropertyChangedCallback(OnContextFlyoutChanged)));

	public static FlyoutBase GetContextFlyout(FrameworkElement element)
	{
		return (FlyoutBase)((DependencyObject)element).GetValue(ContextFlyoutProperty);
	}

	public static void SetContextFlyout(FrameworkElement element, FlyoutBase value)
	{
		((DependencyObject)element).SetValue(ContextFlyoutProperty, (object)value);
	}

	private static void OnContextFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		FlyoutBase obj = (FlyoutBase)((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		FlyoutBase flyoutBase = (FlyoutBase)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (obj != null)
		{
			val.ContextMenuOpening -= new ContextMenuEventHandler(OnContextMenuOpening);
		}
		if (flyoutBase != null)
		{
			val.ContextMenuOpening += new ContextMenuEventHandler(OnContextMenuOpening);
		}
	}

	private static void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)sender;
		FlyoutBase contextFlyout = GetContextFlyout(val);
		if (contextFlyout != null)
		{
			((RoutedEventArgs)e).Handled = true;
			contextFlyout.ShowAsContextFlyout(val);
		}
	}
}
