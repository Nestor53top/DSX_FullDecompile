using System;
using System.Windows;

namespace ModernWpf.Controls;

public class VirtualizingLayoutContext : LayoutContext, IVirtualizingLayoutContextOverrides
{
	private NonVirtualizingLayoutContext m_contextAdapter;

	public Point LayoutOrigin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return LayoutOriginCore;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			LayoutOriginCore = value;
		}
	}

	public int ItemCount => ItemCountCore();

	public Rect RealizationRect => RealizationRectCore();

	public int RecommendedAnchorIndex => RecommendedAnchorIndexCore;

	protected virtual Point LayoutOriginCore
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	protected virtual int RecommendedAnchorIndexCore
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	int IVirtualizingLayoutContextOverrides.RecommendedAnchorIndexCore => RecommendedAnchorIndexCore;

	Point IVirtualizingLayoutContextOverrides.LayoutOriginCore
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return LayoutOriginCore;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			LayoutOriginCore = value;
		}
	}

	public object GetItemAt(int index)
	{
		return GetItemAtCore(index);
	}

	public UIElement GetOrCreateElementAt(int index)
	{
		return GetOrCreateElementAtCore(index, ElementRealizationOptions.None);
	}

	public UIElement GetOrCreateElementAt(int index, ElementRealizationOptions options)
	{
		return GetOrCreateElementAtCore(index, options);
	}

	public void RecycleElement(UIElement element)
	{
		RecycleElementCore(element);
	}

	protected virtual int ItemCountCore()
	{
		throw new NotImplementedException();
	}

	protected virtual object GetItemAtCore(int index)
	{
		throw new NotImplementedException();
	}

	protected virtual Rect RealizationRectCore()
	{
		throw new NotImplementedException();
	}

	protected virtual UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
	{
		throw new NotImplementedException();
	}

	protected virtual void RecycleElementCore(UIElement element)
	{
		throw new NotImplementedException();
	}

	internal NonVirtualizingLayoutContext GetNonVirtualizingContextAdapter()
	{
		if (m_contextAdapter == null)
		{
			m_contextAdapter = new VirtualLayoutContextAdapter(this);
		}
		return m_contextAdapter;
	}

	int IVirtualizingLayoutContextOverrides.ItemCountCore()
	{
		return ItemCountCore();
	}

	object IVirtualizingLayoutContextOverrides.GetItemAtCore(int index)
	{
		return GetItemAtCore(index);
	}

	UIElement IVirtualizingLayoutContextOverrides.GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
	{
		return GetOrCreateElementAtCore(index, options);
	}

	void IVirtualizingLayoutContextOverrides.RecycleElementCore(UIElement element)
	{
		RecycleElementCore(element);
	}

	Rect IVirtualizingLayoutContextOverrides.RealizationRectCore()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RealizationRectCore();
	}
}
