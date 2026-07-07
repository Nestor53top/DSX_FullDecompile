using System.Windows;

namespace ModernWpf.Controls;

internal interface IVirtualizingLayoutContextOverrides
{
	int RecommendedAnchorIndexCore { get; }

	Point LayoutOriginCore { get; set; }

	int ItemCountCore();

	object GetItemAtCore(int index);

	UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options);

	void RecycleElementCore(UIElement element);

	Rect RealizationRectCore();
}
