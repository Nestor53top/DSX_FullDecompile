using System;
using System.Windows;

namespace ModernWpf.Controls;

public class Layout : DependencyObject
{
	public string LayoutId { get; set; }

	public event TypedEventHandler<Layout, object> MeasureInvalidated;

	public event TypedEventHandler<Layout, object> ArrangeInvalidated;

	private VirtualizingLayoutContext GetVirtualizingLayoutContext(LayoutContext context)
	{
		if (context is VirtualizingLayoutContext result)
		{
			return result;
		}
		if (context is NonVirtualizingLayoutContext nonVirtualizingLayoutContext)
		{
			return nonVirtualizingLayoutContext.GetVirtualizingContextAdapter();
		}
		throw new NotImplementedException();
	}

	private NonVirtualizingLayoutContext GetNonVirtualizingLayoutContext(LayoutContext context)
	{
		if (context is NonVirtualizingLayoutContext result)
		{
			return result;
		}
		if (context is VirtualizingLayoutContext virtualizingLayoutContext)
		{
			return virtualizingLayoutContext.GetNonVirtualizingContextAdapter();
		}
		throw new NotImplementedException();
	}

	public void InitializeForContext(LayoutContext context)
	{
		if (this is IVirtualizingLayoutOverrides virtualizingLayoutOverrides)
		{
			VirtualizingLayoutContext virtualizingLayoutContext = GetVirtualizingLayoutContext(context);
			virtualizingLayoutOverrides.InitializeForContextCore(virtualizingLayoutContext);
			return;
		}
		if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayoutOverrides)
		{
			NonVirtualizingLayoutContext nonVirtualizingLayoutContext = GetNonVirtualizingLayoutContext(context);
			nonVirtualizingLayoutOverrides.InitializeForContextCore(nonVirtualizingLayoutContext);
			return;
		}
		throw new NotImplementedException();
	}

	public void UninitializeForContext(LayoutContext context)
	{
		if (this is IVirtualizingLayoutOverrides virtualizingLayoutOverrides)
		{
			VirtualizingLayoutContext virtualizingLayoutContext = GetVirtualizingLayoutContext(context);
			virtualizingLayoutOverrides.UninitializeForContextCore(virtualizingLayoutContext);
			return;
		}
		if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayoutOverrides)
		{
			NonVirtualizingLayoutContext nonVirtualizingLayoutContext = GetNonVirtualizingLayoutContext(context);
			nonVirtualizingLayoutOverrides.UninitializeForContextCore(nonVirtualizingLayoutContext);
			return;
		}
		throw new NotImplementedException();
	}

	public Size Measure(LayoutContext context, Size availableSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (this is IVirtualizingLayoutOverrides virtualizingLayoutOverrides)
		{
			VirtualizingLayoutContext virtualizingLayoutContext = GetVirtualizingLayoutContext(context);
			return virtualizingLayoutOverrides.MeasureOverride(virtualizingLayoutContext, availableSize);
		}
		if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayoutOverrides)
		{
			NonVirtualizingLayoutContext nonVirtualizingLayoutContext = GetNonVirtualizingLayoutContext(context);
			return nonVirtualizingLayoutOverrides.MeasureOverride(nonVirtualizingLayoutContext, availableSize);
		}
		throw new NotImplementedException();
	}

	public Size Arrange(LayoutContext context, Size finalSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (this is IVirtualizingLayoutOverrides virtualizingLayoutOverrides)
		{
			VirtualizingLayoutContext virtualizingLayoutContext = GetVirtualizingLayoutContext(context);
			return virtualizingLayoutOverrides.ArrangeOverride(virtualizingLayoutContext, finalSize);
		}
		if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayoutOverrides)
		{
			NonVirtualizingLayoutContext nonVirtualizingLayoutContext = GetNonVirtualizingLayoutContext(context);
			return nonVirtualizingLayoutOverrides.ArrangeOverride(nonVirtualizingLayoutContext, finalSize);
		}
		throw new NotImplementedException();
	}

	protected void InvalidateMeasure()
	{
		this.MeasureInvalidated?.Invoke(this, null);
	}

	protected void InvalidateArrange()
	{
		this.ArrangeInvalidated?.Invoke(this, null);
	}
}
