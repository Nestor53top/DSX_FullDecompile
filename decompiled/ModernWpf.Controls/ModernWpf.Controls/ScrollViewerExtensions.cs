using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

internal static class ScrollViewerExtensions
{
	public static UIElement GetContentTemplateRoot(this ScrollViewer scrollViewer)
	{
		object content = ((ContentControl)scrollViewer).Content;
		return (UIElement)((content is UIElement) ? content : null);
	}

	public static bool ChangeView(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, float? zoomFactor)
	{
		return scrollViewer.ChangeView(horizontalOffset, verticalOffset, zoomFactor, disableAnimation: false);
	}

	public static bool ChangeView(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAnimation)
	{
		if (horizontalOffset.HasValue)
		{
			scrollViewer.ScrollToHorizontalOffset(horizontalOffset.Value);
		}
		if (verticalOffset.HasValue)
		{
			scrollViewer.ScrollToVerticalOffset(verticalOffset.Value);
		}
		return true;
	}
}
