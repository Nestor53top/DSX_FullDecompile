using System;
using System.Collections.Specialized;
using System.Windows;

namespace ModernWpf.Controls;

public class VirtualizingLayout : Layout, IVirtualizingLayoutOverrides
{
	protected virtual void InitializeForContextCore(VirtualizingLayoutContext context)
	{
	}

	protected virtual void UninitializeForContextCore(VirtualizingLayoutContext context)
	{
	}

	protected virtual Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
	{
		throw new NotImplementedException();
	}

	protected virtual Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return finalSize;
	}

	protected virtual void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
	{
		InvalidateMeasure();
	}

	void IVirtualizingLayoutOverrides.InitializeForContextCore(VirtualizingLayoutContext context)
	{
		InitializeForContextCore(context);
	}

	void IVirtualizingLayoutOverrides.UninitializeForContextCore(VirtualizingLayoutContext context)
	{
		UninitializeForContextCore(context);
	}

	Size IVirtualizingLayoutOverrides.MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return MeasureOverride(context, availableSize);
	}

	Size IVirtualizingLayoutOverrides.ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ArrangeOverride(context, finalSize);
	}

	void IVirtualizingLayoutOverrides.OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
	{
		OnItemsChangedCore(context, source, args);
	}
}
