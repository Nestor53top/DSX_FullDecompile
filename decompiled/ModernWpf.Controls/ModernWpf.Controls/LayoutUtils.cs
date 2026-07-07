using System.Windows;

namespace ModernWpf.Controls;

internal static class LayoutUtils
{
	public static double MeasureAndGetDesiredWidthFor(UIElement element, Size availableSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		double result = 0.0;
		if (element != null)
		{
			element.Measure(availableSize);
			Size desiredSize = element.DesiredSize;
			result = ((Size)(ref desiredSize)).Width;
		}
		return result;
	}

	public static double GetActualWidthFor(FrameworkElement element)
	{
		if (element == null)
		{
			return 0.0;
		}
		return element.ActualWidth;
	}
}
