using System;
using System.Windows;

namespace ModernWpf.Controls;

public class NonVirtualizingLayout : Layout, INonVirtualizingLayoutOverrides
{
	protected virtual void InitializeForContextCore(NonVirtualizingLayoutContext context)
	{
	}

	protected virtual void UninitializeForContextCore(NonVirtualizingLayoutContext context)
	{
	}

	protected virtual Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
	{
		throw new NotImplementedException();
	}

	protected virtual Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
	{
		throw new NotImplementedException();
	}

	void INonVirtualizingLayoutOverrides.InitializeForContextCore(NonVirtualizingLayoutContext context)
	{
		InitializeForContextCore(context);
	}

	void INonVirtualizingLayoutOverrides.UninitializeForContextCore(NonVirtualizingLayoutContext context)
	{
		UninitializeForContextCore(context);
	}

	Size INonVirtualizingLayoutOverrides.MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return MeasureOverride(context, availableSize);
	}

	Size INonVirtualizingLayoutOverrides.ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ArrangeOverride(context, finalSize);
	}
}
