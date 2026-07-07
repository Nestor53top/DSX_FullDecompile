using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace ModernWpf.Controls;

internal class ElementManager
{
	private readonly List<UIElement> m_realizedElements = new List<UIElement>();

	private readonly List<Rect> m_realizedElementLayoutBounds = new List<Rect>();

	private int m_firstRealizedDataIndex = -1;

	private VirtualizingLayoutContext m_context;

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

	public void SetContext(VirtualizingLayoutContext virtualContext)
	{
		m_context = virtualContext;
	}

	public void OnBeginMeasure(ScrollOrientation orientation)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (m_context == null)
		{
			return;
		}
		if (IsVirtualizingContext)
		{
			DiscardElementsOutsideWindow(m_context.RealizationRect, orientation);
			return;
		}
		int itemCount = m_context.ItemCount;
		if (m_realizedElementLayoutBounds.Count != itemCount)
		{
			m_realizedElementLayoutBounds.Resize(itemCount);
		}
	}

	public int GetRealizedElementCount()
	{
		if (!IsVirtualizingContext)
		{
			return m_context.ItemCount;
		}
		return m_realizedElements.Count;
	}

	public UIElement GetAt(int realizedIndex)
	{
		UIElement val;
		if (IsVirtualizingContext)
		{
			if (m_realizedElements[realizedIndex] == null)
			{
				int dataIndexFromRealizedRangeIndex = GetDataIndexFromRealizedRangeIndex(realizedIndex);
				val = m_context.GetOrCreateElementAt(dataIndexFromRealizedRangeIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
				m_realizedElements[realizedIndex] = val;
			}
			else
			{
				val = m_realizedElements[realizedIndex];
			}
		}
		else
		{
			val = m_context.GetOrCreateElementAt(realizedIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
		}
		return val;
	}

	public void Add(UIElement element, int dataIndex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_realizedElements.Count == 0)
		{
			m_firstRealizedDataIndex = dataIndex;
		}
		m_realizedElements.Add(element);
		m_realizedElementLayoutBounds.Add(default(Rect));
	}

	public void Insert(int realizedIndex, int dataIndex, UIElement element)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (realizedIndex == 0)
		{
			m_firstRealizedDataIndex = dataIndex;
		}
		m_realizedElements.Insert(realizedIndex, element);
		m_realizedElementLayoutBounds.Insert(realizedIndex, Rect.Empty);
	}

	public void ClearRealizedRange(int realizedIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			int index = ((realizedIndex == 0) ? (realizedIndex + i) : (realizedIndex + count - 1 - i));
			UIElement val = m_realizedElements[index];
			if (val != null)
			{
				m_context.RecycleElement(val);
			}
		}
		m_realizedElements.RemoveRange(realizedIndex, count);
		m_realizedElementLayoutBounds.RemoveRange(realizedIndex, count);
		if (realizedIndex == 0)
		{
			m_firstRealizedDataIndex = ((m_realizedElements.Count == 0) ? (-1) : (m_firstRealizedDataIndex + count));
		}
	}

	public void DiscardElementsOutsideWindow(bool forward, int startIndex)
	{
		if (IsDataIndexRealized(startIndex))
		{
			int realizedRangeIndexFromDataIndex = GetRealizedRangeIndexFromDataIndex(startIndex);
			if (forward)
			{
				ClearRealizedRange(realizedRangeIndexFromDataIndex, GetRealizedElementCount() - realizedRangeIndexFromDataIndex);
			}
			else
			{
				ClearRealizedRange(0, realizedRangeIndexFromDataIndex + 1);
			}
		}
	}

	public void ClearRealizedRange()
	{
		ClearRealizedRange(0, GetRealizedElementCount());
	}

	public Rect GetLayoutBoundsForDataIndex(int dataIndex)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		int realizedRangeIndexFromDataIndex = GetRealizedRangeIndexFromDataIndex(dataIndex);
		return m_realizedElementLayoutBounds[realizedRangeIndexFromDataIndex];
	}

	public void SetLayoutBoundsForDataIndex(int dataIndex, Rect bounds)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		int realizedRangeIndexFromDataIndex = GetRealizedRangeIndexFromDataIndex(dataIndex);
		m_realizedElementLayoutBounds[realizedRangeIndexFromDataIndex] = bounds;
	}

	public Rect GetLayoutBoundsForRealizedIndex(int realizedIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_realizedElementLayoutBounds[realizedIndex];
	}

	public void SetLayoutBoundsForRealizedIndex(int realizedIndex, Rect bounds)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_realizedElementLayoutBounds[realizedIndex] = bounds;
	}

	public bool IsDataIndexRealized(int index)
	{
		if (IsVirtualizingContext)
		{
			int realizedElementCount = GetRealizedElementCount();
			if (realizedElementCount > 0 && GetDataIndexFromRealizedRangeIndex(0) <= index)
			{
				return GetDataIndexFromRealizedRangeIndex(realizedElementCount - 1) >= index;
			}
			return false;
		}
		if (index >= 0)
		{
			return index < m_context.ItemCount;
		}
		return false;
	}

	public bool IsIndexValidInData(int currentIndex)
	{
		if (currentIndex >= 0)
		{
			return currentIndex < m_context.ItemCount;
		}
		return false;
	}

	public UIElement GetRealizedElement(int dataIndex)
	{
		if (!IsVirtualizingContext)
		{
			return m_context.GetOrCreateElementAt(dataIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
		}
		return GetAt(GetRealizedRangeIndexFromDataIndex(dataIndex));
	}

	public void EnsureElementRealized(bool forward, int dataIndex, string layoutId)
	{
		if (!IsDataIndexRealized(dataIndex))
		{
			UIElement orCreateElementAt = m_context.GetOrCreateElementAt(dataIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
			if (forward)
			{
				Add(orCreateElementAt, dataIndex);
			}
			else
			{
				Insert(0, dataIndex, orCreateElementAt);
			}
		}
	}

	public bool IsWindowConnected(Rect window, ScrollOrientation orientation, bool scrollOrientationSameAsFlow)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Rect layoutBoundsForRealizedIndex;
		Rect layoutBoundsForRealizedIndex2;
		int num;
		double num2;
		if (m_realizedElementLayoutBounds.Count > 0)
		{
			layoutBoundsForRealizedIndex = GetLayoutBoundsForRealizedIndex(0);
			layoutBoundsForRealizedIndex2 = GetLayoutBoundsForRealizedIndex(GetRealizedElementCount() - 1);
			if (!scrollOrientationSameAsFlow)
			{
				num = (int)orientation;
				if (num != 0)
				{
					goto IL_003a;
				}
			}
			else
			{
				if (orientation == ScrollOrientation.Vertical)
				{
					num = 1;
					goto IL_003a;
				}
				num = 0;
			}
			num2 = ((Rect)(ref window)).Y;
			goto IL_004a;
		}
		goto IL_00bb;
		IL_004a:
		double num3 = num2;
		double num4 = ((num == 0) ? (((Rect)(ref window)).Y + ((Rect)(ref window)).Height) : (((Rect)(ref window)).X + ((Rect)(ref window)).Width));
		double num5 = ((num == 0) ? ((Rect)(ref layoutBoundsForRealizedIndex)).Y : ((Rect)(ref layoutBoundsForRealizedIndex)).X);
		double num6 = ((num == 0) ? (((Rect)(ref layoutBoundsForRealizedIndex2)).Y + ((Rect)(ref layoutBoundsForRealizedIndex2)).Height) : (((Rect)(ref layoutBoundsForRealizedIndex2)).X + ((Rect)(ref layoutBoundsForRealizedIndex2)).Width));
		result = num5 <= num4 && num6 >= num3;
		goto IL_00bb;
		IL_003a:
		num2 = ((Rect)(ref window)).X;
		goto IL_004a;
		IL_00bb:
		return result;
	}

	public void DataSourceChanged(object source, NotifyCollectionChangedEventArgs args)
	{
		if (m_realizedElements.Count <= 0)
		{
			return;
		}
		switch (args.Action)
		{
		case NotifyCollectionChangedAction.Add:
			OnItemsAdded(args.NewStartingIndex, args.NewItems.Count);
			break;
		case NotifyCollectionChangedAction.Replace:
		{
			int count2 = args.OldItems.Count;
			int count3 = args.NewItems.Count;
			int oldStartingIndex = args.OldStartingIndex;
			int newStartingIndex = args.NewStartingIndex;
			if (count2 == count3 && oldStartingIndex == newStartingIndex && IsDataIndexRealized(oldStartingIndex) && IsDataIndexRealized(oldStartingIndex + count2 - 1))
			{
				int realizedRangeIndexFromDataIndex = GetRealizedRangeIndexFromDataIndex(oldStartingIndex);
				for (int i = realizedRangeIndexFromDataIndex; i < realizedRangeIndexFromDataIndex + count2; i++)
				{
					UIElement val = m_realizedElements[i];
					if (val != null)
					{
						m_context.RecycleElement(val);
						m_realizedElements[i] = null;
					}
				}
			}
			else
			{
				OnItemsRemoved(oldStartingIndex, count2);
				OnItemsAdded(newStartingIndex, count3);
			}
			break;
		}
		case NotifyCollectionChangedAction.Remove:
			OnItemsRemoved(args.OldStartingIndex, args.OldItems.Count);
			break;
		case NotifyCollectionChangedAction.Reset:
			ClearRealizedRange();
			break;
		case NotifyCollectionChangedAction.Move:
		{
			int count = ((args.OldItems == null) ? 1 : args.OldItems.Count);
			OnItemsRemoved(args.OldStartingIndex, count);
			OnItemsAdded(args.NewStartingIndex, count);
			break;
		}
		}
	}

	public int GetElementDataIndex(UIElement suggestedAnchor)
	{
		int num = m_realizedElements.IndexOf(suggestedAnchor);
		if (num < 0)
		{
			return -1;
		}
		return GetDataIndexFromRealizedRangeIndex(num);
	}

	public int GetDataIndexFromRealizedRangeIndex(int rangeIndex)
	{
		if (!IsVirtualizingContext)
		{
			return rangeIndex;
		}
		return rangeIndex + m_firstRealizedDataIndex;
	}

	private int GetRealizedRangeIndexFromDataIndex(int dataIndex)
	{
		if (!IsVirtualizingContext)
		{
			return dataIndex;
		}
		return dataIndex - m_firstRealizedDataIndex;
	}

	private void DiscardElementsOutsideWindow(Rect window, ScrollOrientation orientation)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		int realizedElementCount = GetRealizedElementCount();
		int num = -1;
		int num2 = realizedElementCount;
		for (int i = 0; i < realizedElementCount && !Intersects(window, m_realizedElementLayoutBounds[i], orientation); i++)
		{
			num++;
		}
		int num3 = realizedElementCount - 1;
		while (num3 >= 0 && !Intersects(window, m_realizedElementLayoutBounds[num3], orientation))
		{
			num2--;
			num3--;
		}
		if (num2 < realizedElementCount - 1)
		{
			ClearRealizedRange(num2 + 1, realizedElementCount - num2 - 1);
		}
		if (num > 0)
		{
			ClearRealizedRange(0, Math.Min(num, GetRealizedElementCount()));
		}
	}

	private static bool Intersects(Rect lhs, Rect rhs, ScrollOrientation orientation)
	{
		double num = ((orientation == ScrollOrientation.Vertical) ? ((Rect)(ref lhs)).Y : ((Rect)(ref lhs)).X);
		double num2 = ((orientation == ScrollOrientation.Vertical) ? (((Rect)(ref lhs)).Y + ((Rect)(ref lhs)).Height) : (((Rect)(ref lhs)).X + ((Rect)(ref lhs)).Width));
		double num3 = ((orientation == ScrollOrientation.Vertical) ? ((Rect)(ref rhs)).Y : ((Rect)(ref rhs)).X);
		double num4 = ((orientation == ScrollOrientation.Vertical) ? (((Rect)(ref rhs)).Y + ((Rect)(ref rhs)).Height) : (((Rect)(ref rhs)).X + ((Rect)(ref rhs)).Width));
		if (num2 >= num3)
		{
			return num <= num4;
		}
		return false;
	}

	private void OnItemsAdded(int index, int count)
	{
		int num = m_firstRealizedDataIndex + GetRealizedElementCount() - 1;
		if (index >= m_firstRealizedDataIndex && index <= num)
		{
			int num2 = index - m_firstRealizedDataIndex;
			for (int i = 0; i < count; i++)
			{
				int realizedIndex = num2 + i;
				int dataIndex = index + i;
				Insert(realizedIndex, dataIndex, null);
			}
		}
		else if (index <= m_firstRealizedDataIndex)
		{
			m_firstRealizedDataIndex += count;
		}
	}

	private void OnItemsRemoved(int index, int count)
	{
		int val = m_firstRealizedDataIndex + m_realizedElements.Count - 1;
		int num = Math.Max(m_firstRealizedDataIndex, index);
		int num2 = Math.Min(val, index + count - 1);
		bool num3 = index <= m_firstRealizedDataIndex;
		if (num2 >= num)
		{
			ClearRealizedRange(GetRealizedRangeIndexFromDataIndex(num), num2 - num + 1);
		}
		if (num3 && m_firstRealizedDataIndex != -1)
		{
			m_firstRealizedDataIndex -= count;
		}
	}
}
