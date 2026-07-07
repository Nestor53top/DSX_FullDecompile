using System;
using System.Collections.Specialized;
using System.Windows;

namespace ModernWpf.Controls;

internal class FlowLayoutAlgorithm
{
	public enum LineAlignment
	{
		Start,
		Center,
		End,
		SpaceAround,
		SpaceBetween,
		SpaceEvenly
	}

	private enum GenerateDirection
	{
		Forward,
		Backward
	}

	private readonly ElementManager m_elementManager;

	private Size m_lastAvailableSize;

	private double m_lastItemSpacing;

	private bool m_collectionChangePending;

	private VirtualizingLayoutContext m_context;

	private IFlowLayoutAlgorithmDelegates m_algorithmCallbacks;

	private Rect m_lastExtent;

	private int m_firstRealizedDataIndexInsideRealizationWindow = -1;

	private int m_lastRealizedDataIndexInsideRealizationWindow = -1;

	private bool m_scrollOrientationSameAsFlow;

	public Rect LastExtent => m_lastExtent;

	private bool IsReflowRequired
	{
		get
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (m_elementManager.GetRealizedElementCount() > 0 && m_elementManager.GetDataIndexFromRealizedRangeIndex(0) == 0)
			{
				return OM.MinorStart(m_elementManager.GetLayoutBoundsForRealizedIndex(0)) != 0.0;
			}
			return false;
		}
	}

	private Rect RealizationRect
	{
		get
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (!IsVirtualizingContext)
			{
				return new Rect(0.0, 0.0, double.PositiveInfinity, double.PositiveInfinity);
			}
			return m_context.RealizationRect;
		}
	}

	private bool IsVirtualizingContext
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (m_context != null)
			{
				Rect realizationRect = m_context.RealizationRect;
				return !double.IsInfinity(((Rect)(ref realizationRect)).Height) && !double.IsInfinity(((Rect)(ref realizationRect)).Width);
			}
			return false;
		}
	}

	private OrientationBasedMeasures OM { get; } = new OrientationBasedMeasures();

	public FlowLayoutAlgorithm()
	{
		m_elementManager = new ElementManager();
	}

	public void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
	{
		m_algorithmCallbacks = callbacks;
		m_context = context;
		m_elementManager.SetContext(context);
	}

	public void UninitializeForContext(VirtualizingLayoutContext context)
	{
		if (IsVirtualizingContext)
		{
			m_elementManager.ClearRealizedRange();
		}
		((ILayoutContextOverrides)context).LayoutStateCore = null;
	}

	public Size Measure(Size availableSize, VirtualizingLayoutContext context, bool isWrapping, double minItemSpacing, double lineSpacing, uint maxItemsPerLine, ScrollOrientation orientation, bool disableVirtualization, string layoutId)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		OM.ScrollOrientation = orientation;
		m_scrollOrientationSameAsFlow = double.IsInfinity(OM.Minor(availableSize));
		int recommendedAnchorIndex = m_context.RecommendedAnchorIndex;
		if (m_elementManager.IsIndexValidInData(recommendedAnchorIndex) && !m_elementManager.IsDataIndexRealized(recommendedAnchorIndex))
		{
			MakeAnchor(m_context, recommendedAnchorIndex, availableSize);
		}
		m_elementManager.OnBeginMeasure(orientation);
		int anchorIndex = GetAnchorIndex(availableSize, isWrapping, minItemSpacing, layoutId);
		Generate(GenerateDirection.Forward, anchorIndex, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
		Generate(GenerateDirection.Backward, anchorIndex, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
		if (isWrapping && IsReflowRequired)
		{
			Rect rect = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
			OM.SetMinorStart(ref rect, 0.0);
			m_elementManager.SetLayoutBoundsForRealizedIndex(0, rect);
			Generate(GenerateDirection.Forward, 0, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
		}
		RaiseLineArranged();
		m_collectionChangePending = false;
		m_lastExtent = EstimateExtent(availableSize, layoutId);
		SetLayoutOrigin();
		return new Size(((Rect)(ref m_lastExtent)).Width, ((Rect)(ref m_lastExtent)).Height);
	}

	public Size Arrange(Size finalSize, VirtualizingLayoutContext context, bool isWrapping, LineAlignment lineAlignment, string layoutId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		ArrangeVirtualizingLayout(finalSize, lineAlignment, isWrapping, layoutId);
		return new Size(Math.Max(((Size)(ref finalSize)).Width, ((Rect)(ref m_lastExtent)).Width), Math.Max(((Size)(ref finalSize)).Height, ((Rect)(ref m_lastExtent)).Height));
	}

	public void OnItemsSourceChanged(object source, NotifyCollectionChangedEventArgs args, VirtualizingLayoutContext context)
	{
		m_elementManager.DataSourceChanged(source, args);
		m_collectionChangePending = true;
	}

	public Size MeasureElement(UIElement element, int index, Size availableSize, VirtualizingLayoutContext context)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Size val = m_algorithmCallbacks.Algorithm_GetMeasureSize(index, availableSize, context);
		element.Measure(val);
		Size val2 = m_algorithmCallbacks.Algorithm_GetProvisionalArrangeSize(index, val, element.DesiredSize, context);
		m_algorithmCallbacks.Algorithm_OnElementMeasured(element, index, availableSize, val, element.DesiredSize, val2, context);
		return val2;
	}

	public UIElement GetElementIfRealized(int dataIndex)
	{
		if (m_elementManager.IsDataIndexRealized(dataIndex))
		{
			return m_elementManager.GetRealizedElement(dataIndex);
		}
		return null;
	}

	public bool TryAddElement0(UIElement element)
	{
		if (m_elementManager.GetRealizedElementCount() == 0)
		{
			m_elementManager.Add(element, 0);
			return true;
		}
		return false;
	}

	private int GetAnchorIndex(Size availableSize, bool isWrapping, double minItemSpacing, string layoutId)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		Point val = default(Point);
		if (!IsVirtualizingContext)
		{
			num = ((((IVirtualizingLayoutContextOverrides)m_context).ItemCountCore() <= 0) ? (-1) : 0);
		}
		else
		{
			bool flag = m_elementManager.IsWindowConnected(RealizationRect, OM.ScrollOrientation, m_scrollOrientationSameAsFlow);
			bool flag2 = isWrapping && (OM.Minor(m_lastAvailableSize) != OM.Minor(availableSize) || m_lastItemSpacing != minItemSpacing || m_collectionChangePending);
			int recommendedAnchorIndex = m_context.RecommendedAnchorIndex;
			if (recommendedAnchorIndex >= 0 && m_elementManager.IsDataIndexRealized(recommendedAnchorIndex))
			{
				num = m_algorithmCallbacks.Algorithm_GetAnchorForTargetElement(recommendedAnchorIndex, availableSize, m_context).Index;
				if (m_elementManager.IsDataIndexRealized(num))
				{
					Rect layoutBoundsForDataIndex = m_elementManager.GetLayoutBoundsForDataIndex(num);
					if (flag2)
					{
						val = OM.MinorMajorPoint(0.0, OM.MajorStart(layoutBoundsForDataIndex));
					}
					else
					{
						((Point)(ref val))._002Ector(((Rect)(ref layoutBoundsForDataIndex)).X, ((Rect)(ref layoutBoundsForDataIndex)).Y);
					}
				}
				else
				{
					for (int num2 = m_elementManager.GetDataIndexFromRealizedRangeIndex(0) - 1; num2 >= num; num2--)
					{
						m_elementManager.EnsureElementRealized(forward: false, num2, layoutId);
					}
					Rect layoutBoundsForDataIndex2 = m_elementManager.GetLayoutBoundsForDataIndex(recommendedAnchorIndex);
					val = OM.MinorMajorPoint(0.0, OM.MajorStart(layoutBoundsForDataIndex2));
				}
			}
			else if (flag2 || !flag)
			{
				FlowLayoutAnchorInfo flowLayoutAnchorInfo = m_algorithmCallbacks.Algorithm_GetAnchorForRealizationRect(availableSize, m_context);
				num = flowLayoutAnchorInfo.Index;
				val = OM.MinorMajorPoint(0.0, flowLayoutAnchorInfo.Offset);
			}
			else
			{
				num = m_elementManager.GetDataIndexFromRealizedRangeIndex(0);
				Rect layoutBoundsForRealizedIndex = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
				((Point)(ref val))._002Ector(((Rect)(ref layoutBoundsForRealizedIndex)).X, ((Rect)(ref layoutBoundsForRealizedIndex)).Y);
			}
		}
		m_firstRealizedDataIndexInsideRealizationWindow = (m_lastRealizedDataIndexInsideRealizationWindow = num);
		if (m_elementManager.IsIndexValidInData(num))
		{
			if (!m_elementManager.IsDataIndexRealized(num))
			{
				m_elementManager.ClearRealizedRange();
				UIElement orCreateElementAt = m_context.GetOrCreateElementAt(num, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
				m_elementManager.Add(orCreateElementAt, num);
			}
			UIElement realizedElement = m_elementManager.GetRealizedElement(num);
			Size val2 = MeasureElement(realizedElement, num, availableSize, m_context);
			Rect bounds = default(Rect);
			((Rect)(ref bounds))._002Ector(((Point)(ref val)).X, ((Point)(ref val)).Y, ((Size)(ref val2)).Width, ((Size)(ref val2)).Height);
			m_elementManager.SetLayoutBoundsForDataIndex(num, bounds);
		}
		else
		{
			m_elementManager.ClearRealizedRange();
		}
		m_lastAvailableSize = availableSize;
		m_lastItemSpacing = minItemSpacing;
		return num;
	}

	private void Generate(GenerateDirection direction, int anchorIndex, Size availableSize, double minItemSpacing, double lineSpacing, uint maxItemsPerLine, bool disableVirtualization, string layoutId)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		if (anchorIndex == -1)
		{
			return;
		}
		int num = ((direction == GenerateDirection.Forward) ? 1 : (-1));
		int num2 = anchorIndex;
		int i = anchorIndex + num;
		Rect layoutBoundsForDataIndex = m_elementManager.GetLayoutBoundsForDataIndex(anchorIndex);
		double num3 = OM.MajorStart(layoutBoundsForDataIndex);
		double num4 = OM.MajorSize(layoutBoundsForDataIndex);
		uint num5 = 1u;
		bool flag = false;
		Rect rect = default(Rect);
		for (; m_elementManager.IsIndexValidInData(i); i += num)
		{
			if (!disableVirtualization && !ShouldContinueFillingUpSpace(num2, direction))
			{
				break;
			}
			m_elementManager.EnsureElementRealized(direction == GenerateDirection.Forward, i, layoutId);
			UIElement realizedElement = m_elementManager.GetRealizedElement(i);
			Size size = MeasureElement(realizedElement, i, availableSize, m_context);
			m_elementManager.GetRealizedElement(num2);
			((Rect)(ref rect))._002Ector(0.0, 0.0, ((Size)(ref size)).Width, ((Size)(ref size)).Height);
			Rect layoutBoundsForDataIndex2 = m_elementManager.GetLayoutBoundsForDataIndex(num2);
			if (direction == GenerateDirection.Forward)
			{
				double remainingSpace = OM.Minor(availableSize) - (OM.MinorStart(layoutBoundsForDataIndex2) + OM.MinorSize(layoutBoundsForDataIndex2) + minItemSpacing + OM.Minor(size));
				if (num5 >= maxItemsPerLine || m_algorithmCallbacks.Algorithm_ShouldBreakLine(i, remainingSpace))
				{
					OM.SetMinorStart(ref rect, 0.0);
					OM.SetMajorStart(ref rect, OM.MajorStart(layoutBoundsForDataIndex2) + num4 + lineSpacing);
					if (flag)
					{
						for (uint num6 = 0u; num6 < num5; num6++)
						{
							long num7 = i - 1 - num6;
							Rect rect2 = m_elementManager.GetLayoutBoundsForDataIndex((int)num7);
							OM.SetMajorSize(ref rect2, num4);
							m_elementManager.SetLayoutBoundsForDataIndex((int)num7, rect2);
						}
					}
					num4 = OM.MajorSize(rect);
					num3 = OM.MajorStart(rect);
					flag = false;
					num5 = 1u;
				}
				else
				{
					OM.SetMinorStart(ref rect, OM.MinorStart(layoutBoundsForDataIndex2) + OM.MinorSize(layoutBoundsForDataIndex2) + minItemSpacing);
					OM.SetMajorStart(ref rect, num3);
					num4 = Math.Max(num4, OM.MajorSize(rect));
					flag = OM.MajorSize(layoutBoundsForDataIndex2) != OM.MajorSize(rect);
					num5++;
				}
			}
			else
			{
				double remainingSpace2 = OM.MinorStart(layoutBoundsForDataIndex2) - (OM.Minor(size) + minItemSpacing);
				if (num5 >= maxItemsPerLine || m_algorithmCallbacks.Algorithm_ShouldBreakLine(i, remainingSpace2))
				{
					double num8 = OM.Minor(availableSize);
					OM.SetMinorStart(ref rect, (!double.IsInfinity(num8)) ? (num8 - OM.Minor(size)) : 0.0);
					OM.SetMajorStart(ref rect, num3 - OM.Major(size) - lineSpacing);
					if (flag)
					{
						double num9 = OM.MajorStart(m_elementManager.GetLayoutBoundsForDataIndex((int)(i + num5 + 1)));
						for (uint num10 = 0u; num10 < num5; num10++)
						{
							int num11 = i + 1 + (int)num10;
							if (num11 != anchorIndex)
							{
								Rect rect3 = m_elementManager.GetLayoutBoundsForDataIndex(num11);
								OM.SetMajorStart(ref rect3, num9 - num4 - lineSpacing);
								OM.SetMajorSize(ref rect3, num4);
								m_elementManager.SetLayoutBoundsForDataIndex(num11, rect3);
							}
						}
					}
					num4 = OM.MajorSize(rect);
					num3 = OM.MajorStart(rect);
					flag = false;
					num5 = 1u;
				}
				else
				{
					OM.SetMinorStart(ref rect, OM.MinorStart(layoutBoundsForDataIndex2) - OM.Minor(size) - minItemSpacing);
					OM.SetMajorStart(ref rect, num3);
					num4 = Math.Max(num4, OM.MajorSize(rect));
					flag = OM.MajorSize(layoutBoundsForDataIndex2) != OM.MajorSize(rect);
					num5++;
				}
			}
			m_elementManager.SetLayoutBoundsForDataIndex(i, rect);
			num2 = i;
		}
		if (direction == GenerateDirection.Forward)
		{
			int itemCount = m_context.ItemCount;
			m_lastRealizedDataIndexInsideRealizationWindow = ((num2 == itemCount - 1) ? (itemCount - 1) : (num2 - 1));
			m_lastRealizedDataIndexInsideRealizationWindow = Math.Max(0, m_lastRealizedDataIndexInsideRealizationWindow);
		}
		else
		{
			int itemCount2 = m_context.ItemCount;
			m_firstRealizedDataIndexInsideRealizationWindow = ((num2 != 0) ? (num2 + 1) : 0);
			m_firstRealizedDataIndexInsideRealizationWindow = Math.Min(itemCount2 - 1, m_firstRealizedDataIndexInsideRealizationWindow);
		}
		m_elementManager.DiscardElementsOutsideWindow(direction == GenerateDirection.Forward, i);
	}

	private void MakeAnchor(VirtualizingLayoutContext context, int index, Size availableSize)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_elementManager.ClearRealizedRange();
		for (int i = m_algorithmCallbacks.Algorithm_GetAnchorForTargetElement(index, availableSize, context).Index; i < index + 1; i++)
		{
			UIElement orCreateElementAt = context.GetOrCreateElementAt(i, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
			orCreateElementAt.Measure(m_algorithmCallbacks.Algorithm_GetMeasureSize(i, availableSize, context));
			m_elementManager.Add(orCreateElementAt, i);
		}
	}

	private bool ShouldContinueFillingUpSpace(int index, GenerateDirection direction)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsVirtualizingContext)
		{
			return true;
		}
		Rect realizationRect = m_context.RealizationRect;
		Rect layoutBoundsForDataIndex = m_elementManager.GetLayoutBoundsForDataIndex(index);
		double num = OM.MajorStart(layoutBoundsForDataIndex);
		double num2 = OM.MajorEnd(layoutBoundsForDataIndex);
		double num3 = OM.MajorStart(realizationRect);
		double num4 = OM.MajorEnd(realizationRect);
		double num5 = OM.MinorStart(layoutBoundsForDataIndex);
		double num6 = OM.MinorEnd(layoutBoundsForDataIndex);
		double num7 = OM.MinorStart(realizationRect);
		double num8 = OM.MinorEnd(realizationRect);
		return (direction == GenerateDirection.Forward && num < num4 && num5 < num8) || (direction == GenerateDirection.Backward && num2 > num3 && num6 > num7);
	}

	private Rect EstimateExtent(Size availableSize, string layoutId)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		UIElement firstRealized = null;
		Rect firstRealizedLayoutBounds = default(Rect);
		UIElement lastRealized = null;
		Rect lastRealizedLayoutBounds = default(Rect);
		int firstRealizedItemIndex = -1;
		int lastRealizedItemIndex = -1;
		if (m_elementManager.GetRealizedElementCount() > 0)
		{
			firstRealized = m_elementManager.GetAt(0);
			firstRealizedLayoutBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
			firstRealizedItemIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(0);
			int num = m_elementManager.GetRealizedElementCount() - 1;
			lastRealized = m_elementManager.GetAt(num);
			lastRealizedItemIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(num);
			lastRealizedLayoutBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(num);
		}
		return m_algorithmCallbacks.Algorithm_GetExtent(availableSize, m_context, firstRealized, firstRealizedItemIndex, firstRealizedLayoutBounds, lastRealized, lastRealizedItemIndex, lastRealizedLayoutBounds);
	}

	private void RaiseLineArranged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Rect realizationRect = RealizationRect;
		if ((((Rect)(ref realizationRect)).Width == 0.0 && ((Rect)(ref realizationRect)).Height == 0.0) || m_elementManager.GetRealizedElementCount() <= 0)
		{
			return;
		}
		int num = 0;
		Rect layoutBoundsForDataIndex = m_elementManager.GetLayoutBoundsForDataIndex(m_firstRealizedDataIndexInsideRealizationWindow);
		double num2 = OM.MajorStart(layoutBoundsForDataIndex);
		double num3 = OM.MajorSize(layoutBoundsForDataIndex);
		for (int i = m_firstRealizedDataIndexInsideRealizationWindow; i <= m_lastRealizedDataIndexInsideRealizationWindow; i++)
		{
			Rect layoutBoundsForDataIndex2 = m_elementManager.GetLayoutBoundsForDataIndex(i);
			if (OM.MajorStart(layoutBoundsForDataIndex2) != num2)
			{
				m_algorithmCallbacks.Algorithm_OnLineArranged(i - num, num, num3, m_context);
				num = 0;
				num2 = OM.MajorStart(layoutBoundsForDataIndex2);
				num3 = 0.0;
			}
			num3 = Math.Max(num3, OM.MajorSize(layoutBoundsForDataIndex2));
			num++;
		}
		m_algorithmCallbacks.Algorithm_OnLineArranged(m_lastRealizedDataIndexInsideRealizationWindow - num + 1, num, num3, m_context);
	}

	private void ArrangeVirtualizingLayout(Size finalSize, LineAlignment lineAlignment, bool isWrapping, string layoutId)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		int realizedElementCount = m_elementManager.GetRealizedElementCount();
		if (realizedElementCount <= 0)
		{
			return;
		}
		int num = 1;
		Rect rect = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
		double num2 = OM.MajorStart(rect);
		double spaceAtLineStart = OM.MinorStart(rect);
		double num3 = 0.0;
		double num4 = OM.MajorSize(rect);
		for (int i = 1; i < realizedElementCount; i++)
		{
			Rect layoutBoundsForRealizedIndex = m_elementManager.GetLayoutBoundsForRealizedIndex(i);
			if (OM.MajorStart(layoutBoundsForRealizedIndex) != num2)
			{
				num3 = OM.Minor(finalSize) - OM.MinorStart(rect) - OM.MinorSize(rect);
				PerformLineAlignment(i - num, num, spaceAtLineStart, num3, num4, lineAlignment, isWrapping, finalSize, layoutId);
				spaceAtLineStart = OM.MinorStart(layoutBoundsForRealizedIndex);
				num = 0;
				num2 = OM.MajorStart(layoutBoundsForRealizedIndex);
				num4 = 0.0;
			}
			num++;
			num4 = Math.Max(num4, OM.MajorSize(layoutBoundsForRealizedIndex));
			rect = layoutBoundsForRealizedIndex;
		}
		if (num > 0)
		{
			double spaceAtLineEnd = OM.Minor(finalSize) - OM.MinorStart(rect) - OM.MinorSize(rect);
			PerformLineAlignment(realizedElementCount - num, num, spaceAtLineStart, spaceAtLineEnd, num4, lineAlignment, isWrapping, finalSize, layoutId);
		}
	}

	private void PerformLineAlignment(int lineStartIndex, int countInLine, double spaceAtLineStart, double spaceAtLineEnd, double lineSize, LineAlignment lineAlignment, bool isWrapping, Size finalSize, string layoutId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		for (int i = lineStartIndex; i < lineStartIndex + countInLine; i++)
		{
			Rect rect = m_elementManager.GetLayoutBoundsForRealizedIndex(i);
			OM.SetMajorSize(ref rect, lineSize);
			if (!m_scrollOrientationSameAsFlow && (spaceAtLineStart != 0.0 || spaceAtLineEnd != 0.0))
			{
				double num = spaceAtLineStart + spaceAtLineEnd;
				switch (lineAlignment)
				{
				case LineAlignment.Start:
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) - spaceAtLineStart);
					break;
				case LineAlignment.End:
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) + spaceAtLineEnd);
					break;
				case LineAlignment.Center:
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) - spaceAtLineStart);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) + num / 2.0);
					break;
				case LineAlignment.SpaceAround:
				{
					double num3 = ((countInLine >= 1) ? (num / (double)(countInLine * 2)) : 0.0);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) - spaceAtLineStart);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) + num3 * (double)((i - lineStartIndex + 1) * 2 - 1));
					break;
				}
				case LineAlignment.SpaceBetween:
				{
					double num4 = ((countInLine > 1) ? (num / (double)(countInLine - 1)) : 0.0);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) - spaceAtLineStart);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) + num4 * (double)(i - lineStartIndex));
					break;
				}
				case LineAlignment.SpaceEvenly:
				{
					double num2 = ((countInLine >= 1) ? (num / (double)(countInLine + 1)) : 0.0);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) - spaceAtLineStart);
					OM.SetMinorStart(ref rect, OM.MinorStart(rect) + num2 * (double)(i - lineStartIndex + 1));
					break;
				}
				}
			}
			((Rect)(ref rect)).X = ((Rect)(ref rect)).X - ((Rect)(ref m_lastExtent)).X;
			((Rect)(ref rect)).Y = ((Rect)(ref rect)).Y - ((Rect)(ref m_lastExtent)).Y;
			if (!isWrapping)
			{
				OM.SetMinorSize(ref rect, Math.Max(OM.MinorSize(rect), OM.Minor(finalSize)));
			}
			m_elementManager.GetAt(i).Arrange(rect);
		}
	}

	private void SetLayoutOrigin()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (IsVirtualizingContext)
		{
			m_context.LayoutOrigin = new Point(((Rect)(ref m_lastExtent)).X, ((Rect)(ref m_lastExtent)).Y);
		}
	}
}
