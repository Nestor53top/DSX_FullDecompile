using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class FlowLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
{
	public static readonly DependencyProperty LineAlignmentProperty = DependencyProperty.Register("LineAlignment", typeof(FlowLayoutLineAlignment), typeof(FlowLayout), new PropertyMetadata((object)FlowLayoutLineAlignment.Start, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinColumnSpacingProperty = DependencyProperty.Register("MinColumnSpacing", typeof(double), typeof(FlowLayout), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinRowSpacingProperty = DependencyProperty.Register("MinRowSpacing", typeof(double), typeof(FlowLayout), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowLayout), new PropertyMetadata((object)(Orientation)0, new PropertyChangedCallback(OnPropertyChanged)));

	private double m_minRowSpacing;

	private double m_minColumnSpacing;

	private FlowLayoutLineAlignment m_lineAlignment;

	public FlowLayoutLineAlignment LineAlignment
	{
		get
		{
			return (FlowLayoutLineAlignment)((DependencyObject)this).GetValue(LineAlignmentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LineAlignmentProperty, (object)value);
		}
	}

	public double MinColumnSpacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinColumnSpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinColumnSpacingProperty, (object)value);
		}
	}

	public double MinRowSpacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinRowSpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinRowSpacingProperty, (object)value);
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

	private double LineSpacing
	{
		get
		{
			if (OM.ScrollOrientation != ScrollOrientation.Vertical)
			{
				return m_minColumnSpacing;
			}
			return m_minRowSpacing;
		}
	}

	private double MinItemSpacing
	{
		get
		{
			if (OM.ScrollOrientation != ScrollOrientation.Vertical)
			{
				return m_minRowSpacing;
			}
			return m_minColumnSpacing;
		}
	}

	private OrientationBasedMeasures OM { get; } = new OrientationBasedMeasures();

	public FlowLayout()
	{
		base.LayoutId = "FlowLayout";
	}

	private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((FlowLayout)(object)sender).PrivateOnPropertyChanged(args);
	}

	protected override void InitializeForContextCore(VirtualizingLayoutContext context)
	{
		object layoutState = context.LayoutState;
		FlowLayoutState flowLayoutState = null;
		if (layoutState != null)
		{
			flowLayoutState = GetAsFlowState(layoutState);
		}
		if (flowLayoutState == null)
		{
			if (layoutState != null)
			{
				throw new Exception("LayoutState must derive from FlowLayoutState.");
			}
			flowLayoutState = new FlowLayoutState();
		}
		flowLayoutState.InitializeForContext(context, this);
	}

	protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
	{
		GetAsFlowState(context.LayoutState).UninitializeForContext(context);
	}

	protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return GetFlowAlgorithm(context).Measure(availableSize, context, isWrapping: true, MinItemSpacing, LineSpacing, uint.MaxValue, OM.ScrollOrientation, disableVirtualization: false, base.LayoutId);
	}

	protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return GetFlowAlgorithm(context).Arrange(finalSize, context, isWrapping: true, (FlowLayoutAlgorithm.LineAlignment)m_lineAlignment, base.LayoutId);
	}

	protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
	{
		GetFlowAlgorithm(context).OnItemsSourceChanged(source, args, context);
		InvalidateLayout();
	}

	protected virtual Size GetMeasureSize(int index, Size availableSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return availableSize;
	}

	protected virtual Size GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return desiredSize;
	}

	protected virtual bool ShouldBreakLine(int index, double remainingSpace)
	{
		return remainingSpace < 0.0;
	}

	protected virtual FlowLayoutAnchorInfo GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		int index = -1;
		double offset = double.NaN;
		int itemCount = context.ItemCount;
		if (itemCount > 0)
		{
			Rect realizationRect = context.RealizationRect;
			object layoutState = context.LayoutState;
			FlowLayoutState asFlowState = GetAsFlowState(layoutState);
			Rect lastExtent = asFlowState.FlowAlgorithm.LastExtent;
			double avgCountInLine = 0.0;
			double num = GetAverageLineInfo(availableSize, context, asFlowState, ref avgCountInLine) + LineSpacing;
			double majorSize = ((OM.MajorSize(lastExtent) == 0.0) ? ((double)itemCount / avgCountInLine * num) : OM.MajorSize(lastExtent));
			if (itemCount > 0 && OM.MajorSize(realizationRect) > 0.0 && DoesRealizationWindowOverlapExtent(realizationRect, OM.MinorMajorRect(OM.MinorStart(lastExtent), OM.MajorStart(lastExtent), OM.Minor(availableSize), majorSize)))
			{
				double num2 = OM.MajorStart(realizationRect) - OM.MajorStart(lastExtent);
				int num3 = Math.Max(0, (int)(num2 / num));
				index = (int)((double)num3 * avgCountInLine);
				index = Math.Max(0, Math.Min(itemCount - 1, index));
				offset = (double)num3 * num + OM.MajorStart(lastExtent);
			}
		}
		return new FlowLayoutAnchorInfo
		{
			Index = index,
			Offset = offset
		};
	}

	protected virtual FlowLayoutAnchorInfo GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		double offset = double.NaN;
		int index = -1;
		int itemCount = context.ItemCount;
		if (targetIndex >= 0 && targetIndex < itemCount)
		{
			index = targetIndex;
			object layoutState = context.LayoutState;
			FlowLayoutState asFlowState = GetAsFlowState(layoutState);
			double avgCountInLine = 0.0;
			double num = GetAverageLineInfo(availableSize, context, asFlowState, ref avgCountInLine) + LineSpacing;
			offset = (double)(int)((double)targetIndex / avgCountInLine) * num + OM.MajorStart(asFlowState.FlowAlgorithm.LastExtent);
		}
		return new FlowLayoutAnchorInfo
		{
			Index = index,
			Offset = offset
		};
	}

	protected virtual Rect GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		int itemCount = context.ItemCount;
		if (itemCount > 0)
		{
			double num = OM.Minor(availableSize);
			object layoutState = context.LayoutState;
			FlowLayoutState asFlowState = GetAsFlowState(layoutState);
			double avgCountInLine = 0.0;
			double num2 = GetAverageLineInfo(availableSize, context, asFlowState, ref avgCountInLine) + LineSpacing;
			if (firstRealized == null)
			{
				double lineSpacing = LineSpacing;
				double minItemSpacing = MinItemSpacing;
				int num3 = (int)Math.Ceiling((double)itemCount / avgCountInLine);
				return (!double.IsInfinity(num)) ? OM.MinorMajorRect(0.0, 0.0, num, Math.Max(0.0, (double)num3 * num2 - lineSpacing)) : OM.MinorMajorRect(0.0, 0.0, Math.Max(0.0, (OM.Minor(asFlowState.SpecialElementDesiredSize) + minItemSpacing) * (double)itemCount - minItemSpacing), Math.Max(0.0, num2 - lineSpacing));
			}
			int num4 = (int)((double)firstRealizedItemIndex / avgCountInLine);
			double value = OM.MajorStart(firstRealizedLayoutBounds) - (double)num4 * num2;
			OM.SetMajorStart(ref rect, value);
			int num5 = (int)((double)(itemCount - lastRealizedItemIndex - 1) / avgCountInLine);
			double value2 = OM.MajorEnd(lastRealizedLayoutBounds) - OM.MajorStart(rect) + (double)num5 * num2;
			OM.SetMajorSize(ref rect, value2);
			OM.SetMinorSize(ref rect, (!double.IsInfinity(num)) ? num : Math.Max(0.0, OM.MinorEnd(lastRealizedLayoutBounds)));
		}
		return rect;
	}

	protected virtual void OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
	{
	}

	protected virtual void OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
	{
		GetAsFlowState(context.LayoutState).OnLineArranged(startIndex, countInLine, lineSize, context);
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetMeasureSize(int index, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetMeasureSize(index, availableSize);
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetProvisionalArrangeSize(index, measureSize, desiredSize);
	}

	bool IFlowLayoutAlgorithmDelegates.Algorithm_ShouldBreakLine(int index, double remainingSpace)
	{
		return ShouldBreakLine(index, remainingSpace);
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchorForRealizationRect(availableSize, context);
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchorForTargetElement(targetIndex, availableSize, context);
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
		OnLineArranged(startIndex, countInLine, lineSize, context);
	}

	private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == OrientationProperty)
		{
			ScrollOrientation scrollOrientation = (((int)(Orientation)((DependencyPropertyChangedEventArgs)(ref args)).NewValue != 0) ? ScrollOrientation.Horizontal : ScrollOrientation.Vertical);
			OM.ScrollOrientation = scrollOrientation;
		}
		else if (property == MinColumnSpacingProperty)
		{
			m_minColumnSpacing = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == MinRowSpacingProperty)
		{
			m_minRowSpacing = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == LineAlignmentProperty)
		{
			m_lineAlignment = (FlowLayoutLineAlignment)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		InvalidateLayout();
	}

	private double GetAverageLineInfo(Size availableSize, VirtualizingLayoutContext context, FlowLayoutState flowState, ref double avgCountInLine)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		avgCountInLine = 1.0;
		if (flowState.TotalLinesMeasured == 0)
		{
			UIElement orCreateElementAt = context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
			Size val = flowState.FlowAlgorithm.MeasureElement(orCreateElementAt, 0, availableSize, context);
			context.RecycleElement(orCreateElementAt);
			int countInLine = Math.Max(1, (int)(OM.Minor(availableSize) / OM.Minor(val)));
			flowState.OnLineArranged(0, countInLine, OM.Major(val), context);
			flowState.SpecialElementDesiredSize = val;
		}
		avgCountInLine = Math.Max(1.0, flowState.TotalItemsPerLine / (double)flowState.TotalLinesMeasured);
		return Math.Round(flowState.TotalLineSize / (double)flowState.TotalLinesMeasured);
	}

	private FlowLayoutState GetAsFlowState(object state)
	{
		return (FlowLayoutState)state;
	}

	private void InvalidateLayout()
	{
		InvalidateMeasure();
	}

	private FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
	{
		return GetAsFlowState(context.LayoutState).FlowAlgorithm;
	}

	private bool DoesRealizationWindowOverlapExtent(Rect realizationWindow, Rect extent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (OM.MajorEnd(realizationWindow) >= OM.MajorStart(extent))
		{
			return OM.MajorStart(realizationWindow) <= OM.MajorEnd(extent);
		}
		return false;
	}
}
