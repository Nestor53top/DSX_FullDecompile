using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace ModernWpf.Controls;

internal class ViewManager
{
	private struct PinnedElementInfo(UIElement element)
	{
		public UIElement PinnedElement { get; } = element;

		public VirtualizationInfo VirtualizationInfo { get; } = ItemsRepeater.GetVirtualizationInfo(element);
	}

	private readonly ItemsRepeater m_owner;

	private readonly List<PinnedElementInfo> m_pinnedPool = new List<PinnedElementInfo>();

	private readonly UniqueIdElementPool m_resetPool;

	private UIElement m_lastFocusedElement;

	private bool m_isDataSourceStableResetPending;

	private bool m_gotFocus;

	private bool m_lostFocus;

	private ElementFactoryGetArgs m_ElementFactoryGetArgs;

	private ElementFactoryRecycleArgs m_ElementFactoryRecycleArgs;

	private int m_firstRealizedElementIndexHeldByLayout = int.MaxValue;

	private int m_lastRealizedElementIndexHeldByLayout = int.MinValue;

	private const int FirstRealizedElementIndexDefault = int.MaxValue;

	private const int LastRealizedElementIndexDefault = int.MinValue;

	public ViewManager(ItemsRepeater owner)
	{
		m_owner = owner;
		m_resetPool = new UniqueIdElementPool(owner);
	}

	public UIElement GetElement(int index, bool forceCreate, bool suppressAutoRecycle)
	{
		UIElement val = (forceCreate ? null : GetElementIfAlreadyHeldByLayout(index));
		if (val == null)
		{
			UIElement madeAnchor = m_owner.MadeAnchor;
			if (madeAnchor != null && ItemsRepeater.TryGetVirtualizationInfo(madeAnchor).Index == index)
			{
				val = madeAnchor;
			}
		}
		if (val == null)
		{
			val = GetElementFromUniqueIdResetPool(index);
		}
		if (val == null)
		{
			val = GetElementFromPinnedElements(index);
		}
		if (val == null)
		{
			val = GetElementFromElementFactory(index);
		}
		VirtualizationInfo virtualizationInfo = ItemsRepeater.TryGetVirtualizationInfo(val);
		if (suppressAutoRecycle)
		{
			virtualizationInfo.AutoRecycleCandidate = false;
		}
		else
		{
			virtualizationInfo.AutoRecycleCandidate = true;
			virtualizationInfo.KeepAlive = true;
		}
		return val;
	}

	public void ClearElement(UIElement element, bool isClearedDueToCollectionChange)
	{
		VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(element);
		int index = virtualizationInfo.Index;
		if (!ClearElementToUniqueIdResetPool(element, virtualizationInfo) && !ClearElementToAnimator(element, virtualizationInfo) && !ClearElementToPinnedPool(element, virtualizationInfo, isClearedDueToCollectionChange))
		{
			ClearElementToElementFactory(element);
		}
		if (index == m_firstRealizedElementIndexHeldByLayout && index == m_lastRealizedElementIndexHeldByLayout)
		{
			InvalidateRealizedIndicesHeldByLayout();
		}
		else if (index == m_firstRealizedElementIndexHeldByLayout)
		{
			m_firstRealizedElementIndexHeldByLayout++;
		}
		else if (index == m_lastRealizedElementIndexHeldByLayout)
		{
			m_lastRealizedElementIndexHeldByLayout--;
		}
	}

	public void ClearElementToElementFactory(UIElement element)
	{
		m_owner.OnElementClearing(element);
		VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(element);
		virtualizationInfo.MoveOwnershipToElementFactory();
		if (virtualizationInfo.MustClearDataContext)
		{
			FrameworkElement val = (FrameworkElement)(object)((element is FrameworkElement) ? element : null);
			if (val != null)
			{
				val.DataContext = null;
			}
		}
		if (m_owner.ItemTemplateShim != null)
		{
			if (m_ElementFactoryRecycleArgs == null)
			{
				m_ElementFactoryRecycleArgs = new ElementFactoryRecycleArgs();
			}
			ElementFactoryRecycleArgs elementFactoryRecycleArgs = m_ElementFactoryRecycleArgs;
			elementFactoryRecycleArgs.Element = element;
			elementFactoryRecycleArgs.Parent = (UIElement)(object)m_owner;
			m_owner.ItemTemplateShim.RecycleElement(elementFactoryRecycleArgs);
			elementFactoryRecycleArgs.Element = null;
			elementFactoryRecycleArgs.Parent = null;
		}
		else
		{
			UIElementCollection children = ((Panel)m_owner).Children;
			int index = 0;
			if (!children.IndexOf(element, out index))
			{
				throw new Exception("ItemsRepeater's child not found in its Children collection.");
			}
			children.RemoveAt(index);
		}
		if (m_lastFocusedElement == element)
		{
			int index2 = virtualizationInfo.Index;
			MoveFocusFromClearedIndex(index2);
		}
	}

	public int GetElementIndex(VirtualizationInfo virtInfo)
	{
		if (virtInfo == null)
		{
			return -1;
		}
		if (!virtInfo.IsRealized && !virtInfo.IsInUniqueIdResetPool)
		{
			return -1;
		}
		return virtInfo.Index;
	}

	public void PrunePinnedElements()
	{
		EnsureEventSubscriptions();
		for (int i = 0; i < m_pinnedPool.Count; i++)
		{
			PinnedElementInfo pinnedElementInfo = m_pinnedPool[i];
			if (!pinnedElementInfo.VirtualizationInfo.IsPinned)
			{
				m_pinnedPool.RemoveAt(i);
				i--;
				ClearElementToElementFactory(pinnedElementInfo.PinnedElement);
			}
		}
	}

	public void UpdatePin(UIElement element, bool addPin)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		DependencyObject parent = CachedVisualTreeHelpers.GetParent((DependencyObject)(object)element);
		DependencyObject val = (DependencyObject)(object)element;
		while (parent != null)
		{
			if (parent is ItemsRepeater itemsRepeater)
			{
				VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo((UIElement)val);
				if (virtualizationInfo.IsRealized)
				{
					if (addPin)
					{
						virtualizationInfo.AddPin();
					}
					else if (virtualizationInfo.IsPinned && virtualizationInfo.RemovePin() == 0)
					{
						((UIElement)itemsRepeater).InvalidateMeasure();
					}
				}
			}
			val = parent;
			parent = CachedVisualTreeHelpers.GetParent(val);
		}
	}

	public void OnItemsSourceChanged(object source, NotifyCollectionChangedEventArgs args)
	{
		switch (args.Action)
		{
		case NotifyCollectionChangedAction.Add:
		{
			int newStartingIndex = args.NewStartingIndex;
			int count = args.NewItems.Count;
			EnsureFirstLastRealizedIndices();
			if (newStartingIndex <= m_lastRealizedElementIndexHeldByLayout)
			{
				m_lastRealizedElementIndexHeldByLayout += count;
				UIElementCollection children2 = ((Panel)m_owner).Children;
				int count2 = children2.Count;
				for (int j = 0; j < count2; j++)
				{
					UIElement element2 = children2[j];
					VirtualizationInfo virtualizationInfo2 = ItemsRepeater.GetVirtualizationInfo(element2);
					int index = virtualizationInfo2.Index;
					if (virtualizationInfo2.IsRealized && index >= newStartingIndex)
					{
						UpdateElementIndex(element2, virtualizationInfo2, index + count);
					}
				}
				break;
			}
			for (int k = 0; k < m_pinnedPool.Count; k++)
			{
				PinnedElementInfo pinnedElementInfo = m_pinnedPool[k];
				VirtualizationInfo virtualizationInfo3 = pinnedElementInfo.VirtualizationInfo;
				int index2 = virtualizationInfo3.Index;
				if (virtualizationInfo3.IsRealized && index2 >= newStartingIndex)
				{
					UIElement pinnedElement = pinnedElementInfo.PinnedElement;
					UpdateElementIndex(pinnedElement, virtualizationInfo3, index2 + count);
				}
			}
			break;
		}
		case NotifyCollectionChangedAction.Replace:
		{
			int oldStartingIndex = args.OldStartingIndex;
			int newStartingIndex2 = args.NewStartingIndex;
			int count3 = args.OldItems.Count;
			int count4 = args.NewItems.Count;
			if (oldStartingIndex != newStartingIndex2)
			{
				throw new Exception("Replace is only allowed with OldStartingIndex equals to NewStartingIndex.");
			}
			if (count3 == 0)
			{
				throw new Exception("Replace notification with args.OldItemsCount value of 0 is not allowed. Use Insert action instead.");
			}
			if (count4 == 0)
			{
				throw new Exception("Replace notification with args.NewItemCount value of 0 is not allowed. Use Remove action instead.");
			}
			int num = count4 - count3;
			if (num == 0)
			{
				break;
			}
			UIElementCollection children3 = ((Panel)m_owner).Children;
			for (int l = 0; l < children3.Count; l++)
			{
				UIElement element3 = children3[l];
				VirtualizationInfo virtualizationInfo4 = ItemsRepeater.GetVirtualizationInfo(element3);
				int index3 = virtualizationInfo4.Index;
				if (virtualizationInfo4.IsRealized && index3 >= oldStartingIndex + count3)
				{
					UpdateElementIndex(element3, virtualizationInfo4, index3 + num);
				}
			}
			EnsureFirstLastRealizedIndices();
			m_lastRealizedElementIndexHeldByLayout += num;
			break;
		}
		case NotifyCollectionChangedAction.Remove:
		{
			int oldStartingIndex2 = args.OldStartingIndex;
			int count5 = args.OldItems.Count;
			UIElementCollection children4 = ((Panel)m_owner).Children;
			for (int m = 0; m < children4.Count; m++)
			{
				UIElement element4 = children4[m];
				VirtualizationInfo virtualizationInfo5 = ItemsRepeater.GetVirtualizationInfo(element4);
				int index4 = virtualizationInfo5.Index;
				if (virtualizationInfo5.IsRealized)
				{
					if (virtualizationInfo5.AutoRecycleCandidate && oldStartingIndex2 <= index4 && index4 < oldStartingIndex2 + count5)
					{
						m_owner.ClearElementImpl(element4);
					}
					else if (index4 >= oldStartingIndex2 + count5)
					{
						UpdateElementIndex(element4, virtualizationInfo5, index4 - count5);
					}
				}
			}
			InvalidateRealizedIndicesHeldByLayout();
			break;
		}
		case NotifyCollectionChangedAction.Reset:
			if (!m_isDataSourceStableResetPending)
			{
				if (m_owner.ItemsSourceView.HasKeyIndexMapping)
				{
					m_isDataSourceStableResetPending = true;
				}
				UIElementCollection children = ((Panel)m_owner).Children;
				for (int i = 0; i < children.Count; i++)
				{
					UIElement element = children[i];
					VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(element);
					if (virtualizationInfo.IsRealized && virtualizationInfo.AutoRecycleCandidate)
					{
						m_owner.ClearElementImpl(element);
					}
				}
			}
			InvalidateRealizedIndicesHeldByLayout();
			break;
		case NotifyCollectionChangedAction.Move:
			break;
		}
	}

	public void OnLayoutChanging()
	{
		if (m_owner.ItemsSourceView != null && m_owner.ItemsSourceView.HasKeyIndexMapping)
		{
			m_isDataSourceStableResetPending = true;
		}
	}

	public void OnOwnerArranged()
	{
		if (!m_isDataSourceStableResetPending)
		{
			return;
		}
		m_isDataSourceStableResetPending = false;
		foreach (KeyValuePair<string, UIElement> item in m_resetPool)
		{
			ClearElement(item.Value, isClearedDueToCollectionChange: true);
		}
		m_resetPool.Clear();
		InvalidateRealizedIndicesHeldByLayout();
	}

	private UIElement GetElementIfAlreadyHeldByLayout(int index)
	{
		UIElement result = null;
		bool flag = m_firstRealizedElementIndexHeldByLayout == int.MaxValue;
		bool flag2 = m_firstRealizedElementIndexHeldByLayout <= index && index <= m_lastRealizedElementIndexHeldByLayout;
		if (flag || flag2)
		{
			UIElementCollection children = ((Panel)m_owner).Children;
			for (int i = 0; i < children.Count; i++)
			{
				UIElement val = children[i];
				VirtualizationInfo virtualizationInfo = ItemsRepeater.TryGetVirtualizationInfo(val);
				if (virtualizationInfo == null || !virtualizationInfo.IsHeldByLayout)
				{
					continue;
				}
				int index2 = virtualizationInfo.Index;
				m_firstRealizedElementIndexHeldByLayout = Math.Min(m_firstRealizedElementIndexHeldByLayout, index2);
				m_lastRealizedElementIndexHeldByLayout = Math.Max(m_lastRealizedElementIndexHeldByLayout, index2);
				if (virtualizationInfo.Index == index)
				{
					result = val;
					if (!flag)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	private UIElement GetElementFromUniqueIdResetPool(int index)
	{
		UIElement val = null;
		if (m_isDataSourceStableResetPending)
		{
			val = m_resetPool.Remove(index);
			if (val != null)
			{
				VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(val);
				virtualizationInfo.MoveOwnershipToLayoutFromUniqueIdResetPool();
				UpdateElementIndex(val, virtualizationInfo, index);
				m_firstRealizedElementIndexHeldByLayout = Math.Min(m_firstRealizedElementIndexHeldByLayout, index);
				m_lastRealizedElementIndexHeldByLayout = Math.Max(m_lastRealizedElementIndexHeldByLayout, index);
			}
		}
		return val;
	}

	private UIElement GetElementFromPinnedElements(int index)
	{
		UIElement result = null;
		for (int i = 0; i < m_pinnedPool.Count; i++)
		{
			PinnedElementInfo pinnedElementInfo = m_pinnedPool[i];
			if (pinnedElementInfo.VirtualizationInfo.Index == index)
			{
				m_pinnedPool.RemoveAt(i);
				result = pinnedElementInfo.PinnedElement;
				pinnedElementInfo.VirtualizationInfo.MoveOwnershipToLayoutFromPinnedPool();
				m_firstRealizedElementIndexHeldByLayout = Math.Min(m_firstRealizedElementIndexHeldByLayout, index);
				m_lastRealizedElementIndexHeldByLayout = Math.Max(m_lastRealizedElementIndexHeldByLayout, index);
				break;
			}
		}
		return result;
	}

	private UIElement GetElementFromElementFactory(int index)
	{
		object data = m_owner.ItemsSourceView.GetAt(index);
		UIElement val = initElement();
		VirtualizationInfo virtualizationInfo = ItemsRepeater.TryGetVirtualizationInfo(val);
		if (virtualizationInfo == null)
		{
			virtualizationInfo = ItemsRepeater.CreateAndInitializeVirtualizationInfo(val);
		}
		virtualizationInfo.MustClearDataContext = false;
		if (data != val)
		{
			FrameworkElement val2 = (FrameworkElement)(object)((val is FrameworkElement) ? val : null);
			if (val2 != null)
			{
				object dataContext = initElementDataContext();
				val2.DataContext = dataContext;
				virtualizationInfo.MustClearDataContext = true;
			}
		}
		virtualizationInfo.MoveOwnershipToLayoutFromElementFactory(index, m_owner.ItemsSourceView.HasKeyIndexMapping ? m_owner.ItemsSourceView.KeyFromIndex(index) : string.Empty);
		ItemsRepeater owner = m_owner;
		UIElementCollection children = ((Panel)owner).Children;
		if ((object)CachedVisualTreeHelpers.GetParent((DependencyObject)(object)val) != owner)
		{
			children.Add(val);
		}
		owner.AnimationManager.OnElementPrepared(val);
		owner.OnElementPrepared(val, index);
		m_firstRealizedElementIndexHeldByLayout = Math.Min(m_firstRealizedElementIndexHeldByLayout, index);
		m_lastRealizedElementIndexHeldByLayout = Math.Max(m_lastRealizedElementIndexHeldByLayout, index);
		return val;
		ElementFactoryGetArgs initArgs()
		{
			if (m_ElementFactoryGetArgs == null)
			{
				m_ElementFactoryGetArgs = new ElementFactoryGetArgs();
			}
			return m_ElementFactoryGetArgs;
		}
		UIElement initElement()
		{
			IElementFactoryShim providedElementFactory = m_owner.ItemTemplateShim;
			if (providedElementFactory == null)
			{
				object obj = data;
				UIElement val3 = (UIElement)((obj is UIElement) ? obj : null);
				if (val3 != null)
				{
					return val3;
				}
			}
			IElementFactoryShim elementFactoryShim = initElementFactory();
			ElementFactoryGetArgs elementFactoryGetArgs = initArgs();
			try
			{
				elementFactoryGetArgs.Data = data;
				elementFactoryGetArgs.Parent = (UIElement)(object)m_owner;
				elementFactoryGetArgs.Index = index;
				return elementFactoryShim.GetElement(elementFactoryGetArgs);
			}
			finally
			{
				elementFactoryGetArgs.Data = null;
				elementFactoryGetArgs.Parent = null;
			}
			IElementFactoryShim initElementFactory()
			{
				if (providedElementFactory == null)
				{
					object obj2 = XamlReader.Parse("<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><TextBlock Text='{Binding}'/></DataTemplate>");
					DataTemplate itemTemplate = (DataTemplate)((obj2 is DataTemplate) ? obj2 : null);
					m_owner.ItemTemplate = itemTemplate;
					return m_owner.ItemTemplateShim;
				}
				return providedElementFactory;
			}
		}
		object initElementDataContext()
		{
			object obj = data;
			FrameworkElement val3 = (FrameworkElement)((obj is FrameworkElement) ? obj : null);
			if (val3 != null)
			{
				object dataContext2 = val3.DataContext;
				if (dataContext2 != null)
				{
					return dataContext2;
				}
			}
			return data;
		}
	}

	private bool ClearElementToUniqueIdResetPool(UIElement element, VirtualizationInfo virtInfo)
	{
		if (m_isDataSourceStableResetPending)
		{
			m_resetPool.Add(element);
			virtInfo.MoveOwnershipToUniqueIdResetPoolFromLayout();
		}
		return m_isDataSourceStableResetPending;
	}

	private bool ClearElementToAnimator(UIElement element, VirtualizationInfo virtInfo)
	{
		bool num = m_owner.AnimationManager.ClearElement(element);
		if (num)
		{
			int index = virtInfo.Index;
			virtInfo.MoveOwnershipToAnimator();
			if (m_lastFocusedElement == element)
			{
				MoveFocusFromClearedIndex(index);
			}
		}
		return num;
	}

	private bool ClearElementToPinnedPool(UIElement element, VirtualizationInfo virtInfo, bool isClearedDueToCollectionChange)
	{
		int num;
		if (!isClearedDueToCollectionChange)
		{
			num = (virtInfo.IsPinned ? 1 : 0);
			if (num != 0)
			{
				m_pinnedPool.Add(new PinnedElementInfo(element));
				virtInfo.MoveOwnershipToPinnedPool();
			}
		}
		else
		{
			num = 0;
		}
		return (byte)num != 0;
	}

	private void UpdateFocusedElement()
	{
		UIElement val = null;
		IInputElement focusedElement = Keyboard.FocusedElement;
		DependencyObject val2 = (DependencyObject)(object)((focusedElement is DependencyObject) ? focusedElement : null);
		if (val2 != null)
		{
			DependencyObject parent = CachedVisualTreeHelpers.GetParent(val2);
			ItemsRepeater owner = m_owner;
			while (parent != null)
			{
				if (parent is ItemsRepeater itemsRepeater)
				{
					UIElement val3 = (UIElement)(object)((val2 is UIElement) ? val2 : null);
					if (itemsRepeater == owner && ItemsRepeater.GetVirtualizationInfo(val3).IsRealized)
					{
						val = val3;
					}
					break;
				}
				val2 = parent;
				parent = CachedVisualTreeHelpers.GetParent(val2);
			}
		}
		if (m_lastFocusedElement != val)
		{
			if (m_lastFocusedElement != null)
			{
				UpdatePin(m_lastFocusedElement, addPin: false);
			}
			if (val != null)
			{
				UpdatePin(val, addPin: true);
			}
			m_lastFocusedElement = val;
		}
	}

	private void OnFocusChanged(object sender, RoutedEventArgs args)
	{
		UpdateFocusedElement();
	}

	private void MoveFocusFromClearedIndex(int clearedIndex)
	{
		UIElement focusedChild = null;
		if (FindFocusCandidate(clearedIndex, ref focusedChild) != null)
		{
			m_lastFocusedElement = focusedChild;
			UpdatePin(focusedChild, addPin: true);
		}
		else
		{
			m_lastFocusedElement = null;
		}
	}

	private UIElement FindFocusCandidate(int clearedIndex, ref UIElement focusedChild)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		int num = int.MinValue;
		int num2 = int.MaxValue;
		UIElement val = null;
		UIElement val2 = null;
		UIElementCollection children = ((Panel)m_owner).Children;
		for (int i = 0; i < children.Count; i++)
		{
			UIElement val3 = children[i];
			VirtualizationInfo virtualizationInfo = ItemsRepeater.TryGetVirtualizationInfo(val3);
			if (virtualizationInfo == null || !virtualizationInfo.IsHeldByLayout)
			{
				continue;
			}
			int index = virtualizationInfo.Index;
			if (index < clearedIndex)
			{
				if (index > num)
				{
					num = index;
					val2 = val3;
				}
			}
			else if (index >= clearedIndex && index < num2)
			{
				num2 = index;
				val = val3;
			}
		}
		UIElement val4 = null;
		if (val != null)
		{
			focusedChild = val;
			if (val.Focus())
			{
				val4 = val;
			}
			else if (val.MoveFocus(new TraversalRequest((FocusNavigationDirection)2)))
			{
				IInputElement focusedElement = FocusManager.GetFocusedElement((DependencyObject)(object)val);
				val4 = (UIElement)(object)((focusedElement is UIElement) ? focusedElement : null);
			}
		}
		if (val4 == null && val2 != null)
		{
			focusedChild = val2;
			if (val2.Focus())
			{
				val4 = val2;
			}
			else if (val2.MoveFocus(new TraversalRequest((FocusNavigationDirection)3)))
			{
				IInputElement focusedElement2 = FocusManager.GetFocusedElement((DependencyObject)(object)val2);
				val4 = (UIElement)(object)((focusedElement2 is UIElement) ? focusedElement2 : null);
			}
		}
		return val4;
	}

	private void EnsureEventSubscriptions()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		if (!m_gotFocus)
		{
			((UIElement)m_owner).GotFocus += new RoutedEventHandler(OnFocusChanged);
			m_gotFocus = true;
			((UIElement)m_owner).LostFocus += new RoutedEventHandler(OnFocusChanged);
			m_lostFocus = true;
		}
	}

	private void UpdateElementIndex(UIElement element, VirtualizationInfo virtInfo, int index)
	{
		int index2 = virtInfo.Index;
		if (index2 != index)
		{
			virtInfo.UpdateIndex(index);
			m_owner.OnElementIndexChanged(element, index2, index);
		}
	}

	private void InvalidateRealizedIndicesHeldByLayout()
	{
		m_firstRealizedElementIndexHeldByLayout = int.MaxValue;
		m_lastRealizedElementIndexHeldByLayout = int.MinValue;
	}

	private void EnsureFirstLastRealizedIndices()
	{
		if (m_firstRealizedElementIndexHeldByLayout == int.MaxValue)
		{
			GetElementIfAlreadyHeldByLayout(0);
		}
	}
}
