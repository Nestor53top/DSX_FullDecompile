using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class StackLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
{
	public static readonly DependencyProperty DisableVirtualizationProperty = DependencyProperty.Register("DisableVirtualization", typeof(bool), typeof(StackLayout), new PropertyMetadata((object)false, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackLayout), new PropertyMetadata((object)(Orientation)1, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register("Spacing", typeof(double), typeof(StackLayout), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OnPropertyChanged)));

	private double m_itemSpacing;

	public bool DisableVirtualization
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(DisableVirtualizationProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisableVirtualizationProperty, (object)value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Orientation)((DependencyObject)this).GetValue(OrientationProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(OrientationProperty, (object)value);
		}
	}

	public double Spacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(SpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SpacingProperty, (object)value);
		}
	}

	private OrientationBasedMeasures OM { get; } = new OrientationBasedMeasures();

	public StackLayout()
	{
		base.LayoutId = "StackLayout";
	}

	private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((StackLayout)(object)sender).PrivateOnPropertyChanged(args);
	}

	protected override void InitializeForContextCore(VirtualizingLayoutContext context)
	{
		object layoutState = context.LayoutState;
		StackLayoutState stackLayoutState = null;
		if (layoutState != null)
		{
			stackLayoutState = GetAsStackState(layoutState);
		}
		if (stackLayoutState == null)
		{
			if (layoutState != null)
			{
				throw new Exception("LayoutState must derive from StackLayoutState.");
			}
			stackLayoutState = new StackLayoutState();
		}
		stackLayoutState.InitializeForContext(context, this);
	}

	protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
	{
		GetAsStackState(context.LayoutState).UninitializeForContext(context);
	}

	protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		GetAsStackState(context.LayoutState).OnMeasureStart();
		return GetFlowAlgorithm(context).Measure(availableSize, context, isWrapping: false, 0.0, m_itemSpacing, uint.MaxValue, OM.ScrollOrientation, DisableVirtualization, base.LayoutId);
	}

	protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return GetFlowAlgorithm(context).Arrange(finalSize, context, isWrapping: false, FlowLayoutAlgorithm.LineAlignment.Start, base.LayoutId);
	}

	protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
	{
		GetFlowAlgorithm(context).OnItemsSourceChanged(source, args, context);
		InvalidateLayout();
	}

	private FlowLayoutAnchorInfo GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		int index = -1;
		double offset = double.NaN;
		int itemCount = context.ItemCount;
		if (itemCount > 0)
		{
			Rect realizationRect = context.RealizationRect;
			StackLayoutState asStackState = GetAsStackState(context.LayoutState);
			Rect lastExtent = asStackState.FlowAlgorithm.LastExtent;
			double num = GetAverageElementSize(availableSize, context, asStackState) + m_itemSpacing;
			double num2 = OM.MajorStart(realizationRect) - OM.MajorStart(lastExtent);
			double num3 = ((OM.MajorSize(lastExtent) == 0.0) ? Math.Max(0.0, num * (double)itemCount - m_itemSpacing) : OM.MajorSize(lastExtent));
			if (itemCount > 0 && OM.MajorSize(realizationRect) >= 0.0 && num2 + OM.MajorSize(realizationRect) >= 0.0 && num2 <= num3)
			{
				index = (int)(num2 / num);
				offset = (double)index * num + OM.MajorStart(lastExtent);
				index = Math.Max(0, Math.Min(itemCount - 1, index));
			}
		}
		return new FlowLayoutAnchorInfo
		{
			Index = index,
			Offset = offset
		};
	}

	private Rect GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		int itemCount = context.ItemCount;
		StackLayoutState asStackState = GetAsStackState(context.LayoutState);
		double num = GetAverageElementSize(availableSize, context, asStackState) + m_itemSpacing;
		OM.SetMinorSize(ref rect, asStackState.MaxArrangeBounds);
		OM.SetMajorSize(ref rect, Math.Max(0.0, (double)itemCount * num - m_itemSpacing));
		if (itemCount > 0 && firstRealized != null)
		{
			OM.SetMajorStart(ref rect, OM.MajorStart(firstRealizedLayoutBounds) - (double)firstRealizedItemIndex * num);
			int num2 = itemCount - lastRealizedItemIndex - 1;
			OM.SetMajorSize(ref rect, OM.MajorEnd(lastRealizedLayoutBounds) - OM.MajorStart(rect) + (double)num2 * num);
		}
		return rect;
	}

	private void OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (context != null)
		{
			GetAsStackState(context.LayoutState).OnElementMeasured(index, OM.Major(provisionalArrangeSize), OM.Minor(provisionalArrangeSize));
		}
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetMeasureSize(int index, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return availableSize;
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		double num = OM.Minor(measureSize);
		return OM.MinorMajorSize((!double.IsInfinity(num)) ? Math.Max(num, OM.Minor(desiredSize)) : OM.Minor(desiredSize), OM.Major(desiredSize));
	}

	bool IFlowLayoutAlgorithmDelegates.Algorithm_ShouldBreakLine(int index, double remainingSpace)
	{
		return true;
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchorForRealizationRect(availableSize, context);
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		double offset = double.NaN;
		int num = -1;
		int itemCount = context.ItemCount;
		if (targetIndex >= 0 && targetIndex < itemCount)
		{
			num = targetIndex;
			StackLayoutState asStackState = GetAsStackState(context.LayoutState);
			double num2 = GetAverageElementSize(availableSize, context, asStackState) + m_itemSpacing;
			offset = (double)num * num2 + OM.MajorStart(asStackState.FlowAlgorithm.LastExtent);
		}
		return new FlowLayoutAnchorInfo
		{
			Index = num,
			Offset = offset
		};
	}

	Rect IFlowLayoutAlgorithmDelegates.Algorithm_GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return GetExtent(availableSize, context, firstRealized, firstRealizedItemIndex, firstRealizedLayoutBounds, lastRealized, lastRealizedItemIndex, lastRealizedLayoutBounds);
	}

	void IFlowLayoutAlgorithmDelegates.Algorithm_OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		OnElementMeasured(element, index, availableSize, measureSize, desiredSize, provisionalArrangeSize, context);
	}

	void IFlowLayoutAlgorithmDelegates.Algorithm_OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
	{
	}

	private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == OrientationProperty)
		{
			ScrollOrientation scrollOrientation = (((int)(Orientation)((DependencyPropertyChangedEventArgs)(ref args)).NewValue == 0) ? ScrollOrientation.Horizontal : ScrollOrientation.Vertical);
			OM.ScrollOrientation = scrollOrientation;
		}
		else if (property == SpacingProperty)
		{
			m_itemSpacing = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		InvalidateLayout();
	}

	private double GetAverageElementSize(Size availableSize, VirtualizingLayoutContext context, StackLayoutState stackLayoutState)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		double result = 0.0;
		if (context.ItemCount > 0)
		{
			if (stackLayoutState.TotalElementsMeasured == 0)
			{
				UIElement orCreateElementAt = context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
				stackLayoutState.FlowAlgorithm.MeasureElement(orCreateElementAt, 0, availableSize, context);
				context.RecycleElement(orCreateElementAt);
			}
			result = Math.Round(stackLayoutState.TotalElementSize / (double)stackLayoutState.TotalElementsMeasured, MidpointRounding.AwayFromZero);
		}
		return result;
	}

	private StackLayoutState GetAsStackState(object state)
	{
		return state as StackLayoutState;
	}

	private void InvalidateLayout()
	{
		InvalidateMeasure();
	}

	private FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
	{
		return GetAsStackState(context.LayoutState).FlowAlgorithm;
	}
}
