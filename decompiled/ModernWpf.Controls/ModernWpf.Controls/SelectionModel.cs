using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ModernWpf.Controls;

public class SelectionModel : INotifyPropertyChanged
{
	private SelectionNode m_rootNode;

	private bool m_singleSelect;

	private IReadOnlyList<IndexPath> m_selectedIndicesCached;

	private IReadOnlyList<object> m_selectedItemsCached;

	private SelectionModelChildrenRequestedEventArgs m_childrenRequestedEventArgs;

	private SelectionModelSelectionChangedEventArgs m_selectionChangedEventArgs;

	private SelectionNode m_leafNode;

	public object Source
	{
		get
		{
			return m_rootNode.Source;
		}
		set
		{
			ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
			m_rootNode.Source = value;
			OnSelectionChanged();
			RaisePropertyChanged("Source");
		}
	}

	public bool SingleSelect
	{
		get
		{
			return m_singleSelect;
		}
		set
		{
			if (m_singleSelect != value)
			{
				m_singleSelect = value;
				IReadOnlyList<IndexPath> selectedIndices = SelectedIndices;
				if (value && selectedIndices != null && selectedIndices.Count > 1)
				{
					IndexPath index = selectedIndices[0];
					ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
					SelectWithPathImpl(index, select: true, raiseSelectionChanged: true);
				}
				RaisePropertyChanged("SingleSelect");
			}
		}
	}

	public IndexPath AnchorIndex
	{
		get
		{
			IndexPath result = null;
			if (m_rootNode.AnchorIndex >= 0)
			{
				List<int> list = new List<int>();
				SelectionNode selectionNode = m_rootNode;
				while (selectionNode != null && selectionNode.AnchorIndex >= 0)
				{
					list.Add(selectionNode.AnchorIndex);
					selectionNode = selectionNode.GetAt(selectionNode.AnchorIndex, realizeChild: false);
				}
				result = new IndexPath(list);
			}
			return result;
		}
		set
		{
			if (value != null)
			{
				SelectionTreeHelper.TraverseIndexPath(m_rootNode, value, realizeChildren: true, delegate(SelectionNode currentNode, IndexPath path, int depth, int childIndex)
				{
					currentNode.AnchorIndex = path.GetAt(depth);
				});
			}
			else
			{
				m_rootNode.AnchorIndex = -1;
			}
			RaisePropertyChanged("AnchorIndex");
		}
	}

	public IndexPath SelectedIndex
	{
		get
		{
			IndexPath result = null;
			IReadOnlyList<IndexPath> selectedIndices = SelectedIndices;
			if (selectedIndices != null && selectedIndices.Count > 0)
			{
				result = selectedIndices[0];
			}
			return result;
		}
		set
		{
			bool? flag = IsSelectedAt(value);
			if (!flag.HasValue || !flag.Value)
			{
				ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
				SelectWithPathImpl(value, select: true, raiseSelectionChanged: false);
				OnSelectionChanged();
			}
		}
	}

	public object SelectedItem
	{
		get
		{
			object result = null;
			IReadOnlyList<object> selectedItems = SelectedItems;
			if (selectedItems != null && selectedItems.Count > 0)
			{
				result = selectedItems[0];
			}
			return result;
		}
	}

	public IReadOnlyList<object> SelectedItems
	{
		get
		{
			if (m_selectedItemsCached == null)
			{
				List<SelectedItemInfo> selectedInfos = new List<SelectedItemInfo>();
				if (m_rootNode.Source != null)
				{
					SelectionTreeHelper.Traverse(m_rootNode, realizeChildren: false, delegate(SelectionTreeHelper.TreeWalkNodeInfo currentInfo)
					{
						if (currentInfo.Node.SelectedCount > 0)
						{
							selectedInfos.Add(new SelectedItemInfo(currentInfo.Node, currentInfo.Path));
						}
					});
				}
				SelectedItems<object> selectedItemsCached = new SelectedItems<object>(selectedInfos, delegate(List<SelectedItemInfo> infos, int index)
				{
					int num = 0;
					object result = null;
					foreach (SelectedItemInfo info in infos)
					{
						if (!info.Node.TryGetTarget(out var target))
						{
							throw new Exception("selection has changed since SelectedItems property was read.");
						}
						int selectedCount = target.SelectedCount;
						if (index >= num && index < num + selectedCount)
						{
							int index2 = target.SelectedIndices()[index - num];
							result = target.ItemsSourceView.GetAt(index2);
							break;
						}
						num += selectedCount;
					}
					return result;
				});
				m_selectedItemsCached = selectedItemsCached;
			}
			return m_selectedItemsCached;
		}
	}

	public IReadOnlyList<IndexPath> SelectedIndices
	{
		get
		{
			if (m_selectedIndicesCached == null)
			{
				List<SelectedItemInfo> selectedInfos = new List<SelectedItemInfo>();
				SelectionTreeHelper.Traverse(m_rootNode, realizeChildren: false, delegate(SelectionTreeHelper.TreeWalkNodeInfo currentInfo)
				{
					if (currentInfo.Node.SelectedCount > 0)
					{
						selectedInfos.Add(new SelectedItemInfo(currentInfo.Node, currentInfo.Path));
					}
				});
				SelectedItems<IndexPath> selectedIndicesCached = new SelectedItems<IndexPath>(selectedInfos, delegate(List<SelectedItemInfo> infos, int index)
				{
					int num = 0;
					IndexPath result = null;
					foreach (SelectedItemInfo info in infos)
					{
						if (!info.Node.TryGetTarget(out var target))
						{
							throw new Exception("selection has changed since SelectedIndices property was read.");
						}
						int selectedCount = target.SelectedCount;
						if (index >= num && index < num + selectedCount)
						{
							int childIndex = target.SelectedIndices()[index - num];
							result = info.Path.CloneWithChildIndex(childIndex);
							break;
						}
						num += selectedCount;
					}
					return result;
				});
				m_selectedIndicesCached = selectedIndicesCached;
			}
			return m_selectedIndicesCached;
		}
	}

	public event TypedEventHandler<SelectionModel, SelectionModelSelectionChangedEventArgs> SelectionChanged;

	public event TypedEventHandler<SelectionModel, SelectionModelChildrenRequestedEventArgs> ChildrenRequested;

	public event PropertyChangedEventHandler PropertyChanged;

	public SelectionModel()
	{
		m_rootNode = new SelectionNode(this, null);
		m_leafNode = new SelectionNode(this, null);
	}

	~SelectionModel()
	{
		ClearSelection(resetAnchor: false, raiseSelectionChanged: false);
		m_rootNode = null;
		m_leafNode = null;
		m_selectedIndicesCached = null;
		m_selectedItemsCached = null;
	}

	public void SetAnchorIndex(int index)
	{
		AnchorIndex = new IndexPath(index);
	}

	public void SetAnchorIndex(int groupIndex, int itemIndex)
	{
		AnchorIndex = new IndexPath(groupIndex, itemIndex);
	}

	public void Select(int index)
	{
		SelectImpl(index, select: true);
	}

	public void Select(int groupIndex, int itemIndex)
	{
		SelectWithGroupImpl(groupIndex, itemIndex, select: true);
	}

	public void SelectAt(IndexPath index)
	{
		SelectWithPathImpl(index, select: true, raiseSelectionChanged: true);
	}

	public void Deselect(int index)
	{
		SelectImpl(index, select: false);
	}

	public void Deselect(int groupIndex, int itemIndex)
	{
		SelectWithGroupImpl(groupIndex, itemIndex, select: false);
	}

	public void DeselectAt(IndexPath index)
	{
		SelectWithPathImpl(index, select: false, raiseSelectionChanged: true);
	}

	public bool? IsSelected(int index)
	{
		return m_rootNode.IsSelectedWithPartial(index);
	}

	public bool? IsSelected(int groupIndex, int itemIndex)
	{
		bool? result = false;
		SelectionNode at = m_rootNode.GetAt(groupIndex, realizeChild: false);
		if (at != null)
		{
			return at.IsSelectedWithPartial(itemIndex);
		}
		return result;
	}

	public bool? IsSelectedAt(IndexPath index)
	{
		bool flag = true;
		SelectionNode selectionNode = m_rootNode;
		for (int i = 0; i < index.GetSize() - 1; i++)
		{
			int at = index.GetAt(i);
			selectionNode = selectionNode.GetAt(at, realizeChild: false);
			if (selectionNode == null)
			{
				flag = false;
				break;
			}
		}
		bool? result = false;
		if (flag)
		{
			int size = index.GetSize();
			if (size == 0)
			{
				return SelectionNode.ConvertToNullableBool(selectionNode.EvaluateIsSelectedBasedOnChildrenNodes());
			}
			return selectionNode.IsSelectedWithPartial(index.GetAt(size - 1));
		}
		return result;
	}

	public void SelectRangeFromAnchor(int index)
	{
		SelectRangeFromAnchorImpl(index, select: true);
	}

	public void SelectRangeFromAnchor(int groupIndex, int itemIndex)
	{
		SelectRangeFromAnchorWithGroupImpl(groupIndex, itemIndex, select: true);
	}

	public void SelectRangeFromAnchorTo(IndexPath index)
	{
		SelectRangeImpl(AnchorIndex, index, select: true);
	}

	public void DeselectRangeFromAnchor(int index)
	{
		SelectRangeFromAnchorImpl(index, select: false);
	}

	public void DeselectRangeFromAnchor(int groupIndex, int itemIndex)
	{
		SelectRangeFromAnchorWithGroupImpl(groupIndex, itemIndex, select: false);
	}

	public void DeselectRangeFromAnchorTo(IndexPath index)
	{
		SelectRangeImpl(AnchorIndex, index, select: false);
	}

	public void SelectRange(IndexPath start, IndexPath end)
	{
		SelectRangeImpl(start, end, select: true);
	}

	public void DeselectRange(IndexPath start, IndexPath end)
	{
		SelectRangeImpl(start, end, select: false);
	}

	public void SelectAll()
	{
		SelectionTreeHelper.Traverse(m_rootNode, realizeChildren: true, delegate(SelectionTreeHelper.TreeWalkNodeInfo info)
		{
			if (info.Node.DataCount > 0)
			{
				info.Node.SelectAll();
			}
		});
		OnSelectionChanged();
	}

	public void ClearSelection()
	{
		ClearSelection(resetAnchor: true, raiseSelectionChanged: true);
	}

	protected void OnPropertyChanged(string propertyName)
	{
		RaisePropertyChanged(propertyName);
	}

	private void RaisePropertyChanged(string name)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	internal void OnSelectionInvalidatedDueToCollectionChange()
	{
		OnSelectionChanged();
	}

	internal SelectionNode SharedLeafNode()
	{
		return m_leafNode;
	}

	internal object ResolvePath(object data, IndexPath dataIndexPath)
	{
		object result = null;
		TypedEventHandler<SelectionModel, SelectionModelChildrenRequestedEventArgs> typedEventHandler = this.ChildrenRequested;
		if (typedEventHandler != null)
		{
			if (m_childrenRequestedEventArgs == null)
			{
				m_childrenRequestedEventArgs = new SelectionModelChildrenRequestedEventArgs(data, dataIndexPath, throwOnAccess: false);
			}
			else
			{
				m_childrenRequestedEventArgs.Initialize(data, dataIndexPath, throwOnAccess: false);
			}
			typedEventHandler(this, m_childrenRequestedEventArgs);
			result = m_childrenRequestedEventArgs.Children;
			m_childrenRequestedEventArgs.Initialize(null, null, throwOnAccess: true);
		}
		else if (data is ItemsSourceView || data is IList || data is IEnumerable)
		{
			result = data;
		}
		return result;
	}

	private void ClearSelection(bool resetAnchor, bool raiseSelectionChanged)
	{
		SelectionTreeHelper.Traverse(m_rootNode, realizeChildren: false, delegate(SelectionTreeHelper.TreeWalkNodeInfo info)
		{
			info.Node.Clear();
		});
		if (resetAnchor)
		{
			AnchorIndex = null;
		}
		if (raiseSelectionChanged)
		{
			OnSelectionChanged();
		}
	}

	private void OnSelectionChanged()
	{
		m_selectedIndicesCached = null;
		m_selectedItemsCached = null;
		TypedEventHandler<SelectionModel, SelectionModelSelectionChangedEventArgs> typedEventHandler = this.SelectionChanged;
		if (typedEventHandler != null)
		{
			if (m_selectionChangedEventArgs == null)
			{
				m_selectionChangedEventArgs = new SelectionModelSelectionChangedEventArgs();
			}
			typedEventHandler(this, m_selectionChangedEventArgs);
		}
		RaisePropertyChanged("SelectedIndex");
		RaisePropertyChanged("SelectedIndices");
		if (m_rootNode.Source != null)
		{
			RaisePropertyChanged("SelectedItem");
			RaisePropertyChanged("SelectedItems");
		}
	}

	private void SelectImpl(int index, bool select)
	{
		if (m_rootNode.IsSelected(index) != select)
		{
			if (m_singleSelect)
			{
				ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
			}
			if (m_rootNode.Select(index, select))
			{
				AnchorIndex = new IndexPath(index);
			}
			OnSelectionChanged();
		}
	}

	private void SelectWithGroupImpl(int groupIndex, int itemIndex, bool select)
	{
		if (m_singleSelect)
		{
			ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
		}
		if (m_rootNode.GetAt(groupIndex, realizeChild: true).Select(itemIndex, select))
		{
			AnchorIndex = new IndexPath(groupIndex, itemIndex);
		}
		OnSelectionChanged();
	}

	private void SelectWithPathImpl(IndexPath index, bool select, bool raiseSelectionChanged)
	{
		bool flag = true;
		if (m_singleSelect)
		{
			IndexPath selectedIndex = SelectedIndex;
			if (selectedIndex != null)
			{
				if (select && selectedIndex.CompareTo(index) == 0)
				{
					flag = false;
				}
			}
			else
			{
				flag = select;
			}
		}
		if (!flag)
		{
			return;
		}
		bool selected = false;
		bool changedSelection = false;
		if (m_singleSelect && select)
		{
			ClearSelection(resetAnchor: true, raiseSelectionChanged: false);
		}
		SelectionTreeHelper.TraverseIndexPath(m_rootNode, index, realizeChildren: true, delegate(SelectionNode currentNode, IndexPath path, int depth, int childIndex)
		{
			if (depth == path.GetSize() - 1)
			{
				if (currentNode.IsSelected(childIndex) != select)
				{
					changedSelection = true;
				}
				selected = currentNode.Select(childIndex, select);
			}
		});
		if (selected)
		{
			AnchorIndex = index;
		}
		m_selectedIndicesCached = null;
		m_selectedItemsCached = null;
		if (raiseSelectionChanged && changedSelection)
		{
			OnSelectionChanged();
		}
	}

	private void SelectRangeFromAnchorImpl(int index, bool select)
	{
		int begin = 0;
		IndexPath anchorIndex = AnchorIndex;
		if (anchorIndex != null)
		{
			begin = anchorIndex.GetAt(0);
		}
		if (m_rootNode.SelectRange(new IndexRange(begin, index), select))
		{
			OnSelectionChanged();
		}
	}

	private void SelectRangeFromAnchorWithGroupImpl(int endGroupIndex, int endItemIndex, bool select)
	{
		int num = 0;
		int num2 = 0;
		IndexPath anchorIndex = AnchorIndex;
		if (anchorIndex != null)
		{
			num = anchorIndex.GetAt(0);
			num2 = anchorIndex.GetAt(1);
		}
		if (num > endGroupIndex || (num == endGroupIndex && num2 > endItemIndex))
		{
			int num3 = num;
			num = endGroupIndex;
			endGroupIndex = num3;
			int num4 = num2;
			num2 = endItemIndex;
			endItemIndex = num4;
		}
		bool flag = false;
		for (int i = num; i <= endGroupIndex; i++)
		{
			SelectionNode at = m_rootNode.GetAt(i, realizeChild: true);
			int begin = ((i == num) ? num2 : 0);
			int end = ((i == endGroupIndex) ? endItemIndex : (at.DataCount - 1));
			flag |= at.SelectRange(new IndexRange(begin, end), select);
		}
		if (flag)
		{
			OnSelectionChanged();
		}
	}

	private void SelectRangeImpl(IndexPath start, IndexPath end, bool select)
	{
		IndexPath indexPath = start;
		IndexPath indexPath2 = end;
		if (indexPath2.CompareTo(indexPath) == -1)
		{
			IndexPath indexPath3 = indexPath;
			indexPath = indexPath2;
			indexPath2 = indexPath3;
		}
		SelectionTreeHelper.TraverseRangeRealizeChildren(m_rootNode, indexPath, indexPath2, delegate(SelectionTreeHelper.TreeWalkNodeInfo info)
		{
			if (info.Node.DataCount == 0)
			{
				info.ParentNode.Select(info.Path.GetAt(info.Path.GetSize() - 1), select);
			}
		});
		OnSelectionChanged();
	}
}
