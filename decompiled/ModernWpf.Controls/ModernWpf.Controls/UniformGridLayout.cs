using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class UniformGridLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
{
	public static readonly DependencyProperty ItemsJustificationProperty = DependencyProperty.Register("ItemsJustification", typeof(UniformGridLayoutItemsJustification), typeof(UniformGridLayout), new PropertyMetadata((object)UniformGridLayoutItemsJustification.Start, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty ItemsStretchProperty = DependencyProperty.Register("ItemsStretch", typeof(UniformGridLayoutItemsStretch), typeof(UniformGridLayout), new PropertyMetadata((object)UniformGridLayoutItemsStretch.None, new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MaximumRowsOrColumnsProperty = DependencyProperty.Register("MaximumRowsOrColumns", typeof(int), typeof(UniformGridLayout), new PropertyMetadata((object)(-1), new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinColumnSpacingProperty = DependencyProperty.Register("MinColumnSpacing", typeof(double), typeof(UniformGridLayout), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinItemHeightProperty = DependencyProperty.Register("MinItemHeight", typeof(double), typeof(UniformGridLayout), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinItemWidthProperty = DependencyProperty.Register("MinItemWidth", typeof(double), typeof(UniformGridLayout), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty MinRowSpacingProperty = DependencyProperty.Register("MinRowSpacing", typeof(double), typeof(UniformGridLayout), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

	public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformGridLayout), new PropertyMetadata((object)(Orientation)0, new PropertyChangedCallback(OnPropertyChanged)));

	private double m_minItemWidth = double.NaN;

	private double m_minItemHeight = double.NaN;

	private double m_minRowSpacing;

	private double m_minColumnSpacing;

	private UniformGridLayoutItemsJustification m_itemsJustification;

	private UniformGridLayoutItemsStretch m_itemsStretch;

	private uint m_maximumRowsOrColumns = uint.MaxValue;

	public UniformGridLayoutItemsJustification ItemsJustification
	{
		get
		{
			return (UniformGridLayoutItemsJustification)((DependencyObject)this).GetValue(ItemsJustificationProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsJustificationProperty, (object)value);
		}
	}

	public UniformGridLayoutItemsStretch ItemsStretch
	{
		get
		{
			return (UniformGridLayoutItemsStretch)((DependencyObject)this).GetValue(ItemsStretchProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsStretchProperty, (object)value);
		}
	}

	public int MaximumRowsOrColumns
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(MaximumRowsOrColumnsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaximumRowsOrColumnsProperty, (object)value);
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

	public double MinItemHeight
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinItemHeightProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinItemHeightProperty, (object)value);
		}
	}

	public double MinItemWidth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinItemWidthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinItemWidthProperty, (object)value);
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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Orientation != 0)
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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Orientation != 0)
			{
				return m_minRowSpacing;
			}
			return m_minColumnSpacing;
		}
	}

	private OrientationBasedMeasures OM { get; } = new OrientationBasedMeasures();

	public UniformGridLayout()
	{
		base.LayoutId = "UniformGridLayout";
	}

	private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((UniformGridLayout)(object)sender).PrivateOnPropertyChanged(args);
	}

	protected override void InitializeForContextCore(VirtualizingLayoutContext context)
	{
		object layoutState = context.LayoutState;
		UniformGridLayoutState uniformGridLayoutState = null;
		if (layoutState != null)
		{
			uniformGridLayoutState = GetAsGridState(layoutState);
		}
		if (uniformGridLayoutState == null)
		{
			if (layoutState != null)
			{
				throw new Exception("LayoutState must derive from UniformGridLayoutState.");
			}
			uniformGridLayoutState = new UniformGridLayoutState();
		}
		uniformGridLayoutState.InitializeForContext(context, this);
	}

	protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
	{
		GetAsGridState(context.LayoutState).UninitializeForContext(context);
	}

	protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		GetAsGridState(context.LayoutState).EnsureElementSize(availableSize, context, m_minItemWidth, m_minItemHeight, m_itemsStretch, Orientation, MinRowSpacing, MinColumnSpacing, m_maximumRowsOrColumns);
		return GetFlowAlgorithm(context).Measure(availableSize, context, isWrapping: true, MinItemSpacing, LineSpacing, m_maximumRowsOrColumns, OM.ScrollOrientation, disableVirtualization: false, base.LayoutId);
	}

	protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return GetFlowAlgorithm(context).Arrange(finalSize, context, isWrapping: true, (FlowLayoutAlgorithm.LineAlignment)m_itemsJustification, base.LayoutId);
	}

	protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
	{
		GetFlowAlgorithm(context).OnItemsSourceChanged(source, args, context);
		InvalidateLayout();
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetMeasureSize(int index, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
		return new Size(asGridState.EffectiveItemWidth, asGridState.EffectiveItemHeight);
	}

	Size IFlowLayoutAlgorithmDelegates.Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
		return new Size(asGridState.EffectiveItemWidth, asGridState.EffectiveItemHeight);
	}

	bool IFlowLayoutAlgorithmDelegates.Algorithm_ShouldBreakLine(int index, double remainingSpace)
	{
		return remainingSpace < 0.0;
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		Rect layoutRectForDataIndex = default(Rect);
		((Rect)(ref layoutRectForDataIndex))._002Ector(double.NaN, double.NaN, double.NaN, double.NaN);
		int index = -1;
		int itemCount = context.ItemCount;
		Rect realizationRect = context.RealizationRect;
		if (itemCount > 0 && OM.MajorSize(realizationRect) > 0.0)
		{
			Rect lastExtent = GetAsGridState(context.LayoutState).FlowAlgorithm.LastExtent;
			int num = (int)Math.Min(Math.Max(1u, (uint)(OM.Minor(availableSize) / GetMinorSizeWithSpacing(context))), Math.Max(1u, m_maximumRowsOrColumns));
			double num2 = (double)(itemCount / num) * GetMajorSizeWithSpacing(context);
			double num3 = OM.MajorStart(realizationRect) - OM.MajorStart(lastExtent);
			if (num3 + OM.MajorSize(realizationRect) >= 0.0 && num3 <= num2)
			{
				int num4 = (int)(Math.Max(0.0, OM.MajorStart(realizationRect) - OM.MajorStart(lastExtent)) / GetMajorSizeWithSpacing(context));
				index = Math.Max(0, Math.Min(itemCount - 1, num4 * num));
				layoutRectForDataIndex = GetLayoutRectForDataIndex(availableSize, index, lastExtent, context);
			}
		}
		return new FlowLayoutAnchorInfo
		{
			Index = index,
			Offset = OM.MajorStart(layoutRectForDataIndex)
		};
	}

	FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		int index = -1;
		double offset = double.NaN;
		int itemCount = context.ItemCount;
		if (targetIndex >= 0 && targetIndex < itemCount)
		{
			int num = (int)Math.Min(Math.Max(1.0, (double)(uint)OM.Minor(availableSize) / GetMinorSizeWithSpacing(context)), Math.Max(1u, m_maximumRowsOrColumns));
			int num2 = targetIndex / num * num;
			index = num2;
			UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
			offset = OM.MajorStart(GetLayoutRectForDataIndex(availableSize, num2, asGridState.FlowAlgorithm.LastExtent, context));
		}
		return new FlowLayoutAnchorInfo
		{
			Index = index,
			Offset = offset
		};
	}

	Rect IFlowLayoutAlgorithmDelegates.Algorithm_GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		int itemCount = context.ItemCount;
		double num = OM.Minor(availableSize);
		int num2 = (int)Math.Min(Math.Max(1u, (!double.IsInfinity(num)) ? ((uint)(num / GetMinorSizeWithSpacing(context))) : ((uint)itemCount)), Math.Max(1u, m_maximumRowsOrColumns));
		double majorSizeWithSpacing = GetMajorSizeWithSpacing(context);
		if (itemCount > 0)
		{
			OM.SetMinorSize(ref rect, (!double.IsInfinity(num) && m_itemsStretch == UniformGridLayoutItemsStretch.Fill) ? num : Math.Max(0.0, (double)num2 * GetMinorSizeWithSpacing(context) - MinItemSpacing));
			OM.SetMajorSize(ref rect, Math.Max(0.0, (double)(itemCount / num2) * majorSizeWithSpacing - LineSpacing));
			if (firstRealized != null)
			{
				OM.SetMajorStart(ref rect, OM.MajorStart(firstRealizedLayoutBounds) - (double)(firstRealizedItemIndex / num2) * majorSizeWithSpacing);
				int num3 = itemCount - lastRealizedItemIndex - 1;
				OM.SetMajorSize(ref rect, OM.MajorEnd(lastRealizedLayoutBounds) - OM.MajorStart(rect) + (double)(num3 / num2) * majorSizeWithSpacing);
			}
		}
		return rect;
	}

	void IFlowLayoutAlgorithmDelegates.Algorithm_OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
	{
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
		else if (property == ItemsJustificationProperty)
		{
			m_itemsJustification = (UniformGridLayoutItemsJustification)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == ItemsStretchProperty)
		{
			m_itemsStretch = (UniformGridLayoutItemsStretch)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == MinItemWidthProperty)
		{
			m_minItemWidth = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == MinItemHeightProperty)
		{
			m_minItemHeight = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == MaximumRowsOrColumnsProperty)
		{
			m_maximumRowsOrColumns = (uint)(int)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		InvalidateLayout();
	}

	private double GetMinorSizeWithSpacing(VirtualizingLayoutContext context)
	{
		double minItemSpacing = MinItemSpacing;
		UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
		if (OM.ScrollOrientation != ScrollOrientation.Vertical)
		{
			return asGridState.EffectiveItemHeight + minItemSpacing;
		}
		return asGridState.EffectiveItemWidth + minItemSpacing;
	}

	private double GetMajorSizeWithSpacing(VirtualizingLayoutContext context)
	{
		double lineSpacing = LineSpacing;
		UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
		if (OM.ScrollOrientation != ScrollOrientation.Vertical)
		{
			return asGridState.EffectiveItemWidth + lineSpacing;
		}
		return asGridState.EffectiveItemHeight + lineSpacing;
	}

	private Rect GetLayoutRectForDataIndex(Size availableSize, int index, Rect lastExtent, VirtualizingLayoutContext context)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)Math.Min(Math.Max(1u, (uint)(OM.Minor(availableSize) / GetMinorSizeWithSpacing(context))), Math.Max(1u, m_maximumRowsOrColumns));
		int num2 = index / num;
		int num3 = index - num2 * num;
		UniformGridLayoutState asGridState = GetAsGridState(context.LayoutState);
		return OM.MinorMajorRect((double)num3 * GetMinorSizeWithSpacing(context) + OM.MinorStart(lastExtent), (double)num2 * GetMajorSizeWithSpacing(context) + OM.MajorStart(lastExtent), (OM.ScrollOrientation == ScrollOrientation.Vertical) ? asGridState.EffectiveItemWidth : asGridState.EffectiveItemHeight, (OM.ScrollOrientation == ScrollOrientation.Vertical) ? asGridState.EffectiveItemHeight : asGridState.EffectiveItemWidth);
	}

	private UniformGridLayoutState GetAsGridState(object state)
	{
		return state as UniformGridLayoutState;
	}

	private FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
	{
		return GetAsGridState(context.LayoutState).FlowAlgorithm;
	}

	private void InvalidateLayout()
	{
		InvalidateMeasure();
	}
}
