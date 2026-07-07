using System.Collections.Specialized;
using System.Windows;

namespace ModernWpf.Controls;

internal interface IVirtualizingLayoutOverrides
{
	void InitializeForContextCore(VirtualizingLayoutContext context);

	void UninitializeForContextCore(VirtualizingLayoutContext context);

	Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize);

	Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize);

	void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args);
}
