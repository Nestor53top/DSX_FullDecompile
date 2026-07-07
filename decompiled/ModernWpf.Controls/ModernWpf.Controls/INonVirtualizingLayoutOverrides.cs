using System.Windows;

namespace ModernWpf.Controls;

internal interface INonVirtualizingLayoutOverrides
{
	void InitializeForContextCore(NonVirtualizingLayoutContext context);

	void UninitializeForContextCore(NonVirtualizingLayoutContext context);

	Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize);

	Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize);
}
