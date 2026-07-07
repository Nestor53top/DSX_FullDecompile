using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ModernWpf.Controls;

internal class TopNavigationViewDataProvider : SplitDataSourceBase<object, NavigationViewSplitVectorID, double>
{
	private ItemsSourceView m_dataSource;

	private object m_rawDataSource;

	private Action<NotifyCollectionChangedEventArgs> m_dataChangeCallback;

	private double m_overflowButtonCachedWidth;

	public TopNavigationViewDataProvider()
	{
		Func<object, int> indexOfFunction = (object value) => IndexOf(value);
		SplitVector<object, NavigationViewSplitVectorID> item = new SplitVector<object, NavigationViewSplitVectorID>(NavigationViewSplitVectorID.PrimaryList, indexOfFunction);
		SplitVector<object, NavigationViewSplitVectorID> item2 = new SplitVector<object, NavigationViewSplitVectorID>(NavigationViewSplitVectorID.OverflowList, indexOfFunction);
		InitializeSplitVectors(new List<SplitVector<object, NavigationViewSplitVectorID>> { item, item2 });
	}

	public IList GetPrimaryItems()
	{
		return GetVector(NavigationViewSplitVectorID.PrimaryList).GetVector();
	}

	public IList GetOverflowItems()
	{
		return GetVector(NavigationViewSplitVectorID.OverflowList).GetVector();
	}

	public void SetDataSource(object rawData)
	{
		if (ShouldChangeDataSource(rawData))
		{
			ItemsSourceView itemsSourceView = null;
			if (rawData != null)
			{
				itemsSourceView = new InspectingDataSource(rawData);
			}
			ChangeDataSource(itemsSourceView);
			m_rawDataSource = rawData;
			if (itemsSourceView != null)
			{
				MoveAllItemsToPrimaryList();
			}
		}
	}

	public bool ShouldChangeDataSource(object rawData)
	{
		return rawData != m_rawDataSource;
	}

	public void OnRawDataChanged(Action<NotifyCollectionChangedEventArgs> dataChangeCallback)
	{
		m_dataChangeCallback = dataChangeCallback;
	}

	internal override int IndexOf(object value)
	{
		return m_dataSource?.IndexOf(value) ?? (-1);
	}

	internal override object GetAt(int index)
	{
		return m_dataSource?.GetAt(index);
	}

	internal override int Size()
	{
		return m_dataSource?.Count ?? 0;
	}

	internal override NavigationViewSplitVectorID DefaultVectorIDOnInsert()
	{
		return NavigationViewSplitVectorID.NotInitialized;
	}

	internal override double DefaultAttachedData()
	{
		return double.MinValue;
	}

	public void MoveAllItemsToPrimaryList()
	{
		for (int i = 0; i < Size(); i++)
		{
			MoveItemToVector(i, NavigationViewSplitVectorID.PrimaryList);
		}
	}

	public List<int> ConvertPrimaryIndexToIndex(List<int> indexesInPrimary)
	{
		List<int> list = null;
		if (indexesInPrimary.Count > 0)
		{
			SplitVector<object, NavigationViewSplitVectorID> vector = GetVector(NavigationViewSplitVectorID.PrimaryList);
			if (vector != null)
			{
				list = indexesInPrimary.Select((int index) => vector.IndexToIndexInOriginalVector(index)).ToList();
			}
		}
		return list ?? new List<int>();
	}

	public int ConvertOriginalIndexToIndex(int originalIndex)
	{
		return GetVector(IsItemInPrimaryList(originalIndex) ? NavigationViewSplitVectorID.PrimaryList : NavigationViewSplitVectorID.OverflowList).IndexFromIndexInOriginalVector(originalIndex);
	}

	public void MoveItemsOutOfPrimaryList(List<int> indexes)
	{
		MoveItemsToList(indexes, NavigationViewSplitVectorID.OverflowList);
	}

	public void MoveItemsToPrimaryList(List<int> indexes)
	{
		MoveItemsToList(indexes, NavigationViewSplitVectorID.PrimaryList);
	}

	public void MoveItemsToList(List<int> indexes, NavigationViewSplitVectorID vectorID)
	{
		foreach (int index in indexes)
		{
			MoveItemToVector(index, vectorID);
		}
	}

	public int GetPrimaryListSize()
	{
		return GetPrimaryItems().Count;
	}

	public int GetNavigationViewItemCountInPrimaryList()
	{
		int num = 0;
		for (int i = 0; i < Size(); i++)
		{
			if (IsItemInPrimaryList(i) && IsContainerNavigationViewItem(i))
			{
				num++;
			}
		}
		return num;
	}

	public int GetNavigationViewItemCountInTopNav()
	{
		int num = 0;
		for (int i = 0; i < Size(); i++)
		{
			if (IsContainerNavigationViewItem(i))
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateWidthForPrimaryItem(int indexInPrimary, double width)
	{
		SplitVector<object, NavigationViewSplitVectorID> vector = GetVector(NavigationViewSplitVectorID.PrimaryList);
		if (vector != null)
		{
			int index = vector.IndexToIndexInOriginalVector(indexInPrimary);
			SetWidthForItem(index, width);
		}
	}

	public double WidthRequiredToRecoveryAllItemsToPrimary()
	{
		double num = 0.0;
		for (int i = 0; i < Size(); i++)
		{
			if (!IsItemInPrimaryList(i))
			{
				num += GetWidthForItem(i);
			}
		}
		num -= m_overflowButtonCachedWidth;
		return Math.Max(0.0, num);
	}

	public bool HasInvalidWidth(List<int> items)
	{
		bool result = false;
		foreach (int item in items)
		{
			if (!IsValidWidthForItem(item))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public double GetWidthForItem(int index)
	{
		double num = AttachedData(index);
		if (!IsValidWidth(num))
		{
			num = 0.0;
		}
		return num;
	}

	public double CalculateWidthForItems(List<int> items)
	{
		double num = 0.0;
		foreach (int item in items)
		{
			num += GetWidthForItem(item);
		}
		return num;
	}

	public void InvalidWidthCache()
	{
		ResetAttachedData(-1.0);
	}

	public double OverflowButtonWidth()
	{
		return m_overflowButtonCachedWidth;
	}

	public void OverflowButtonWidth(double width)
	{
		m_overflowButtonCachedWidth = width;
	}

	public bool IsItemSelectableInPrimaryList(object value)
	{
		return IndexOf(value) != -1;
	}

	public int IndexOf(object value, NavigationViewSplitVectorID vectorID)
	{
		return IndexOfImpl(value, vectorID);
	}

	private void OnDataSourceChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		switch (args.Action)
		{
		case NotifyCollectionChangedAction.Add:
			OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
			break;
		case NotifyCollectionChangedAction.Remove:
			OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
			break;
		case NotifyCollectionChangedAction.Reset:
			OnClear();
			break;
		case NotifyCollectionChangedAction.Replace:
			OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
			OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
			break;
		}
		if (m_dataChangeCallback != null)
		{
			m_dataChangeCallback(args);
		}
	}

	private bool IsValidWidth(double width)
	{
		if (width >= 0.0)
		{
			return width < double.MaxValue;
		}
		return false;
	}

	public bool IsValidWidthForItem(int index)
	{
		double width = AttachedData(index);
		return IsValidWidth(width);
	}

	private void SetWidthForItem(int index, double width)
	{
		if (IsValidWidth(width))
		{
			AttachedData(index, width);
		}
	}

	private void ChangeDataSource(ItemsSourceView newValue)
	{
		ItemsSourceView dataSource = m_dataSource;
		if (dataSource != newValue)
		{
			if (dataSource != null)
			{
				dataSource.CollectionChanged -= OnDataSourceChanged;
			}
			Clear();
			m_dataSource = newValue;
			SyncAndInitVectorFlagsWithID(NavigationViewSplitVectorID.NotInitialized, DefaultAttachedData());
			if (newValue != null)
			{
				newValue.CollectionChanged += OnDataSourceChanged;
			}
		}
		MoveItemsToVector(NavigationViewSplitVectorID.NotInitialized);
	}

	public bool IsItemInPrimaryList(int index)
	{
		return GetVectorIDForItem(index) == NavigationViewSplitVectorID.PrimaryList;
	}

	private bool IsContainerNavigationViewItem(int index)
	{
		bool result = true;
		object at = GetAt(index);
		if (at != null && (at is NavigationViewItemHeader || at is NavigationViewItemSeparator))
		{
			result = false;
		}
		return result;
	}

	private bool IsContainerNavigationViewHeader(int index)
	{
		bool result = false;
		object at = GetAt(index);
		if (at != null && at is NavigationViewItemHeader)
		{
			result = true;
		}
		return result;
	}
}
