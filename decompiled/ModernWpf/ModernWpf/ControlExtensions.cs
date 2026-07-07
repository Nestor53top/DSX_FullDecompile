using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf;

internal static class ControlExtensions
{
	public static FrameworkElement GetTemplateRoot(this Control control)
	{
		if (VisualTreeHelper.GetChildrenCount((DependencyObject)(object)control) > 0)
		{
			DependencyObject child = VisualTreeHelper.GetChild((DependencyObject)(object)control, 0);
			return (FrameworkElement)(object)((child is FrameworkElement) ? child : null);
		}
		return null;
	}

	public static T GetTemplateChild<T>(this Control control, string childName) where T : DependencyObject
	{
		ControlTemplate template = control.Template;
		object obj = ((template != null) ? ((FrameworkTemplate)template).FindName(childName, (FrameworkElement)(object)control) : null);
		return (T)((obj is T) ? obj : null);
	}
}
