using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWpf.Controls;

public class ScrollViewerEx : ScrollViewer
{
	protected override void OnInitialized(EventArgs e)
	{
		((FrameworkElement)this).OnInitialized(e);
		if (((FrameworkElement)this).Style == null && ((DependencyObject)this).ReadLocalValue(FrameworkElement.StyleProperty) == DependencyProperty.UnsetValue)
		{
			((FrameworkElement)this).SetResourceReference(FrameworkElement.StyleProperty, (object)typeof(ScrollViewer));
		}
	}

	protected override void OnMouseWheel(MouseWheelEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled && ((ScrollViewer)this).ScrollableHeight > 0.0)
		{
			((ScrollViewer)this).OnMouseWheel(e);
		}
	}
}
