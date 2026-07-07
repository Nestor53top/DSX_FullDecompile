using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ModernWpf.Controls;

internal class CachedVisualTreeHelpers
{
	public static Rect GetLayoutSlot(FrameworkElement element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LayoutInformation.GetLayoutSlot(element);
	}

	public static DependencyObject GetParent(DependencyObject element)
	{
		if (element is Visual || element is Visual3D)
		{
			return VisualTreeHelper.GetParent(element);
		}
		FrameworkContentElement val = (FrameworkContentElement)(object)((element is FrameworkContentElement) ? element : null);
		if (val != null)
		{
			return val.Parent;
		}
		return null;
	}
}
