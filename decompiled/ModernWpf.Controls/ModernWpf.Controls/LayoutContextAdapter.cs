using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls;

internal class LayoutContextAdapter : VirtualizingLayoutContext
{
	private readonly WeakReference<NonVirtualizingLayoutContext> m_nonVirtualizingContext;

	protected override object LayoutStateCore
	{
		get
		{
			if (m_nonVirtualizingContext.TryGetTarget(out var target))
			{
				return target.LayoutState;
			}
			return null;
		}
		set
		{
			if (m_nonVirtualizingContext.TryGetTarget(out var target))
			{
				((ILayoutContextOverrides)target).LayoutStateCore = value;
			}
		}
	}

	protected override int RecommendedAnchorIndexCore => -1;

	protected override Point LayoutOriginCore
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return new Point(0.0, 0.0);
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (value != new Point(0.0, 0.0))
			{
				throw new ArgumentOutOfRangeException("value", "LayoutOrigin must be at (0,0) when RealizationRect is infinite sized.");
			}
		}
	}

	public LayoutContextAdapter(NonVirtualizingLayoutContext nonVirtualizingContext)
	{
		m_nonVirtualizingContext = new WeakReference<NonVirtualizingLayoutContext>(nonVirtualizingContext);
	}

	protected override int ItemCountCore()
	{
		if (m_nonVirtualizingContext.TryGetTarget(out var target))
		{
			return target.Children.Count;
		}
		return 0;
	}

	protected override object GetItemAtCore(int index)
	{
		if (m_nonVirtualizingContext.TryGetTarget(out var target))
		{
			return target.Children[index];
		}
		return null;
	}

	protected override UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
	{
		if (m_nonVirtualizingContext.TryGetTarget(out var target))
		{
			return target.Children[index];
		}
		return null;
	}

	protected override void RecycleElementCore(UIElement element)
	{
	}

	protected int GetElementIndexCore(UIElement element)
	{
		if (m_nonVirtualizingContext.TryGetTarget(out var target))
		{
			IReadOnlyList<UIElement> children = target.Children;
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] == element)
				{
					return i;
				}
			}
		}
		return -1;
	}

	protected override Rect RealizationRectCore()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(0.0, 0.0, double.PositiveInfinity, double.PositiveInfinity);
	}
}
