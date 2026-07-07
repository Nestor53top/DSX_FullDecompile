using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;
using ModernWpf.Input;

namespace ModernWpf.Controls;

public class NavigationViewItem : NavigationViewItemBase
{
	private const string c_navigationViewItemPresenterName = "NavigationViewItemPresenter";

	private const string c_repeater = "NavigationViewItemMenuItemsHost";

	private const string c_rootGrid = "NVIRootGrid";

	private const string c_childrenFlyout = "ChildrenFlyout";

	private const string c_flyoutContentGrid = "FlyoutContentGrid";

	private const string c_pressedSelected = "PressedSelected";

	private const string c_pointerOverSelected = "PointerOverSelected";

	private const string c_selected = "Selected";

	private const string c_pressed = "Pressed";

	private const string c_pointerOver = "PointerOver";

	private const string c_disabled = "Disabled";

	private const string c_enabled = "Enabled";

	private const string c_normal = "Normal";

	private const string c_chevronHidden = "ChevronHidden";

	private const string c_chevronVisibleOpen = "ChevronVisibleOpen";

	private const string c_chevronVisibleClosed = "ChevronVisibleClosed";

	private SplitViewIsPaneOpenChangedRevoker m_splitViewIsPaneOpenChangedRevoker;

	private SplitViewDisplayModeChangedRevoker m_splitViewDisplayModeChangedRevoker;

	private SplitViewCompactPaneLengthChangedRevoker m_splitViewCompactPaneLengthChangedRevoker;

	private ItemsRepeaterElementPreparedRevoker m_repeaterElementPreparedRevoker;

	private ItemsRepeaterElementClearingRevoker m_repeaterElementClearingRevoker;

	private ItemsSourceView.CollectionChangedRevoker m_itemsSourceViewCollectionChangedRevoker;

	private FlyoutBaseClosingRevoker m_flyoutClosingRevoker;

	private ToolTip m_toolTip;

	private NavigationViewItemHelper<NavigationViewItem> m_helper = new NavigationViewItemHelper<NavigationViewItem>();

	private NavigationViewItemPresenter m_navigationViewItemPresenter;

	private object m_suggestedToolTipContent;

	private ItemsRepeater m_repeater;

	private Grid m_flyoutContentGrid;

	private Grid m_rootGrid;

	private bool m_isClosedCompact;

	private bool m_appliedTemplate;

	private bool m_hasKeyboardFocus;

	private bool m_isMouseCaptured;

	private bool m_isPressed;

	private bool m_isPointerOver;

	private bool m_isRepeaterParentedToFlyout;

	public static readonly DependencyProperty IconProperty;

	private static readonly DependencyPropertyKey CompactPaneLengthPropertyKey;

	public static readonly DependencyProperty CompactPaneLengthProperty;

	public static readonly DependencyProperty SelectsOnInvokedProperty;

	public static readonly DependencyProperty IsExpandedProperty;

	public static readonly DependencyProperty HasUnrealizedChildrenProperty;

	public static readonly DependencyProperty IsChildSelectedProperty;

	private static readonly DependencyPropertyKey MenuItemsPropertyKey;

	public static readonly DependencyProperty MenuItemsProperty;

	public static readonly DependencyProperty MenuItemsSourceProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public IconElement Icon
	{
		get
		{
			return (IconElement)((DependencyObject)this).GetValue(IconProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IconProperty, (object)value);
		}
	}

	public double CompactPaneLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CompactPaneLengthProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(CompactPaneLengthPropertyKey, (object)value);
		}
	}

	public bool SelectsOnInvoked
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(SelectsOnInvokedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectsOnInvokedProperty, (object)value);
		}
	}

	public bool IsExpanded
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsExpandedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsExpandedProperty, (object)value);
		}
	}

	public bool HasUnrealizedChildren
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(HasUnrealizedChildrenProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HasUnrealizedChildrenProperty, (object)value);
		}
	}

	public bool IsChildSelected
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsChildSelectedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsChildSelectedProperty, (object)value);
		}
	}

	public IList MenuItems => (IList)((DependencyObject)this).GetValue(MenuItemsProperty);

	public object MenuItemsSource
	{
		get
		{
			return ((DependencyObject)this).GetValue(MenuItemsSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuItemsSourceProperty, value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	internal event DependencyPropertyChangedCallback IsExpandedChanged;

	static NavigationViewItem()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(NavigationViewItem), new PropertyMetadata(new PropertyChangedCallback(OnIconPropertyChanged)));
		CompactPaneLengthPropertyKey = DependencyProperty.RegisterReadOnly("CompactPaneLength", typeof(double), typeof(NavigationViewItem), new PropertyMetadata((object)48.0));
		CompactPaneLengthProperty = CompactPaneLengthPropertyKey.DependencyProperty;
		SelectsOnInvokedProperty = DependencyProperty.Register("SelectsOnInvoked", typeof(bool), typeof(NavigationViewItem), new PropertyMetadata((object)true));
		IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(NavigationViewItem), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsExpandedPropertyChanged)));
		HasUnrealizedChildrenProperty = DependencyProperty.Register("HasUnrealizedChildren", typeof(bool), typeof(NavigationViewItem), new PropertyMetadata((object)false, new PropertyChangedCallback(OnHasUnrealizedChildrenPropertyChanged)));
		IsChildSelectedProperty = DependencyProperty.Register("IsChildSelected", typeof(bool), typeof(NavigationViewItem), new PropertyMetadata((object)false));
		MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(IList), typeof(NavigationViewItem), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemsPropertyChanged)));
		MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;
		MenuItemsSourceProperty = DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(NavigationViewItem), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemsSourcePropertyChanged)));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(NavigationViewItem));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NavigationViewItem)));
	}

	public NavigationViewItem()
	{
		((DependencyObject)this).SetValue(MenuItemsPropertyKey, (object)new ObservableCollection<object>());
	}

	internal void UpdateVisualStateNoTransition()
	{
		UpdateVisualState(useTransitions: false);
	}

	private protected override void OnNavigationViewItemBaseDepthChanged()
	{
		UpdateItemIndentation();
		PropagateDepthToChildren(base.Depth + 1);
	}

	private protected override void OnNavigationViewItemBaseIsSelectedChanged()
	{
		UpdateVisualStateForPointer();
	}

	private protected override void OnNavigationViewItemBasePositionChanged()
	{
		UpdateVisualStateNoTransition();
		ReparentRepeater();
	}

	public override void OnApplyTemplate()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		m_appliedTemplate = false;
		UnhookEventsAndClearFields();
		((FrameworkElement)this).OnApplyTemplate();
		m_helper.Init(this);
		Grid templateChildT = CppWinRTHelpers.GetTemplateChildT<Grid>("NVIRootGrid", (IControlProtected)this);
		if (templateChildT != null)
		{
			m_rootGrid = templateChildT;
			FlyoutBase attachedFlyout = FlyoutBase.GetAttachedFlyout((FrameworkElement)(object)templateChildT);
			if (attachedFlyout != null)
			{
				m_flyoutClosingRevoker = new FlyoutBaseClosingRevoker(attachedFlyout, OnFlyoutClosing);
			}
		}
		HookInputEvents(this);
		((UIElement)this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		m_toolTip = CppWinRTHelpers.GetTemplateChildT<ToolTip>("ToolTip", (IControlProtected)this);
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
			splitView.DisplayModeChanged += OnSplitViewPropertyChanged;
			splitView.CompactPaneLengthChanged += OnSplitViewPropertyChanged;
			UpdateCompactPaneLength();
			UpdateIsClosedCompact();
		}
		NavigationView navigationView = GetNavigationView();
		if (navigationView != null)
		{
			ItemsRepeater templateChildT2 = CppWinRTHelpers.GetTemplateChildT<ItemsRepeater>("NavigationViewItemMenuItemsHost", (IControlProtected)this);
			if (templateChildT2 != null)
			{
				m_repeater = templateChildT2;
				m_repeaterElementPreparedRevoker = new ItemsRepeaterElementPreparedRevoker(templateChildT2, navigationView.OnRepeaterElementPrepared);
				m_repeaterElementClearingRevoker = new ItemsRepeaterElementClearingRevoker(templateChildT2, navigationView.OnRepeaterElementClearing);
				templateChildT2.ItemTemplate = navigationView.GetNavigationViewItemsFactory();
			}
			UpdateRepeaterItemsSource();
		}
		FlyoutBase templateChildT3 = CppWinRTHelpers.GetTemplateChildT<FlyoutBase>("ChildrenFlyout", this);
		if (templateChildT3 != null)
		{
			templateChildT3.Offset = 0.0;
		}
		m_flyoutContentGrid = CppWinRTHelpers.GetTemplateChildT<Grid>("FlyoutContentGrid", (IControlProtected)this);
		m_appliedTemplate = true;
		UpdateItemIndentation();
		UpdateVisualStateNoTransition();
		ReparentRepeater();
		if (!ShouldRepeaterShowInFlyout())
		{
			ShowHideChildren();
		}
	}

	private void UpdateRepeaterItemsSource()
	{
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			object itemsSource = init();
			m_itemsSourceViewCollectionChangedRevoker?.Revoke();
			repeater.ItemsSource = itemsSource;
			m_itemsSourceViewCollectionChangedRevoker = new ItemsSourceView.CollectionChangedRevoker(repeater.ItemsSourceView, OnItemsSourceViewChanged);
		}
		object init()
		{
			object menuItemsSource = MenuItemsSource;
			if (menuItemsSource != null)
			{
				return menuItemsSource;
			}
			return MenuItems;
		}
	}

	private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		UpdateVisualStateForChevron();
	}

	internal UIElement GetSelectionIndicator()
	{
		UIElement selectionIndicator = m_helper.GetSelectionIndicator();
		NavigationViewItemPresenter presenter = GetPresenter();
		if (presenter != null)
		{
			selectionIndicator = presenter.GetSelectionIndicator();
		}
		return selectionIndicator;
	}

	private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
	{
		if (args == SplitView.CompactPaneLengthProperty)
		{
			UpdateCompactPaneLength();
		}
		else if (args == SplitView.IsPaneOpenProperty || args == SplitView.DisplayModeProperty)
		{
			UpdateIsClosedCompact();
			ReparentRepeater();
		}
	}

	private void UpdateCompactPaneLength()
	{
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			((DependencyObject)this).SetValue(CompactPaneLengthPropertyKey, (object)splitView.CompactPaneLength);
			GetPresenter()?.UpdateCompactPaneLength(splitView.CompactPaneLength, IsOnLeftNav());
		}
	}

	internal void UpdateIsClosedCompact()
	{
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			m_isClosedCompact = !splitView.IsPaneOpen && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);
			UpdateVisualState(useTransitions: true);
			GetPresenter()?.UpdateClosedCompactVisualState(base.IsTopLevelItem, m_isClosedCompact);
		}
	}

	private void UpdateNavigationViewItemToolTip()
	{
		object toolTip = ToolTipService.GetToolTip((DependencyObject)(object)this);
		if (toolTip == null || toolTip == m_suggestedToolTipContent)
		{
			if (ShouldEnableToolTip())
			{
				ToolTipService.SetToolTip((DependencyObject)(object)this, m_suggestedToolTipContent);
			}
			else
			{
				ToolTipService.SetToolTip((DependencyObject)(object)this, (object)null);
			}
		}
	}

	private void SuggestedToolTipChanged(object newContent)
	{
		bool num = newContent != null && newContent is string;
		object suggestedToolTipContent = null;
		if (num)
		{
			suggestedToolTipContent = newContent;
		}
		object toolTip = ToolTipService.GetToolTip((DependencyObject)(object)this);
		object suggestedToolTipContent2 = m_suggestedToolTipContent;
		if (suggestedToolTipContent2 != null && suggestedToolTipContent2 == toolTip)
		{
			ToolTipService.SetToolTip((DependencyObject)(object)this, (object)null);
		}
		m_suggestedToolTipContent = suggestedToolTipContent;
	}

	private void OnIsExpandedPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
		if (val != null)
		{
			((NavigationViewItemAutomationPeer)(object)val).RaiseExpandCollapseAutomationEvent((ExpandCollapseState)(IsExpanded ? 1 : 0));
		}
	}

	private void OnIconPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateVisualStateNoTransition();
	}

	private void OnMenuItemsPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateRepeaterItemsSource();
		UpdateVisualStateForChevron();
	}

	private void OnMenuItemsSourcePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateRepeaterItemsSource();
		UpdateVisualStateForChevron();
	}

	private void OnHasUnrealizedChildrenPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateVisualStateForChevron();
	}

	private void ShowSelectionIndicator(bool visible)
	{
		UIElement selectionIndicator = GetSelectionIndicator();
		if (selectionIndicator != null)
		{
			selectionIndicator.Opacity = (visible ? 1.0 : 0.0);
		}
	}

	private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
	{
		NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
		if (navigationViewItemPresenter != null)
		{
			string text = ((!showIcon) ? "ContentOnly" : (showContent ? "IconOnLeft" : "IconOnly"));
			VisualStateManager.GoToState((FrameworkElement)(object)navigationViewItemPresenter, text, false);
		}
	}

	private void UpdateVisualStateForNavigationViewPositionChange()
	{
		NavigationViewRepeaterPosition position = base.Position;
		string text = "OnLeftNavigation";
		bool flag = false;
		switch (position)
		{
		case NavigationViewRepeaterPosition.LeftNav:
		case NavigationViewRepeaterPosition.LeftFooter:
			if (!SharedHelpers.IsRS4OrHigher())
			{
			}
			break;
		case NavigationViewRepeaterPosition.TopPrimary:
		case NavigationViewRepeaterPosition.TopFooter:
			SharedHelpers.IsRS4OrHigher();
			text = "OnTopNavigationPrimary";
			break;
		case NavigationViewRepeaterPosition.TopOverflow:
			text = "OnTopNavigationOverflow";
			break;
		}
		if (!flag)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, text, false);
		}
	}

	private void UpdateVisualStateForKeyboardFocusedState()
	{
		string text = "KeyboardNormal";
		if (m_hasKeyboardFocus)
		{
			text = "KeyboardFocused";
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, false);
	}

	private void UpdateVisualStateForToolTip()
	{
		ToolTip toolTip = m_toolTip;
		if (toolTip != null)
		{
			bool num = ShouldEnableToolTip();
			object suggestedToolTipContent = m_suggestedToolTipContent;
			if (num && suggestedToolTipContent != null)
			{
				((ContentControl)toolTip).Content = suggestedToolTipContent;
				((UIElement)toolTip).IsEnabled = true;
			}
			else
			{
				((ContentControl)toolTip).Content = null;
				((UIElement)toolTip).IsEnabled = false;
			}
		}
		else
		{
			UpdateNavigationViewItemToolTip();
		}
	}

	private void UpdateVisualStateForPointer()
	{
		string text = "Enabled";
		bool isSelected = base.IsSelected;
		string text2 = "Normal";
		if (((UIElement)this).IsEnabled)
		{
			if (isSelected)
			{
				text2 = (m_isPressed ? "PressedSelected" : ((!m_isPointerOver) ? "Selected" : "PointerOverSelected"));
			}
			else if (m_isPointerOver)
			{
				text2 = ((!m_isPressed) ? "PointerOver" : "Pressed");
			}
			else if (m_isPressed)
			{
				text2 = "Pressed";
			}
		}
		else
		{
			text = "Disabled";
			if (isSelected)
			{
				text2 = "Selected";
			}
		}
		NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
		if (navigationViewItemPresenter != null)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)navigationViewItemPresenter, text, true);
			VisualStateManager.GoToState((FrameworkElement)(object)navigationViewItemPresenter, text2, true);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, text, true);
			VisualStateManager.GoToState((FrameworkElement)(object)this, text2, true);
		}
	}

	private void UpdateVisualState(bool useTransitions)
	{
		if (!m_appliedTemplate)
		{
			return;
		}
		UpdateVisualStateForPointer();
		UpdateVisualStateForNavigationViewPositionChange();
		bool flag = ShouldShowIcon();
		bool showContent = ShouldShowContent();
		if (IsOnLeftNav())
		{
			NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
			if (navigationViewItemPresenter != null)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)navigationViewItemPresenter, flag ? "IconVisible" : "IconCollapsed", useTransitions);
			}
		}
		UpdateVisualStateForToolTip();
		UpdateVisualStateForIconAndContent(flag, showContent);
		UpdateVisualStateForKeyboardFocusedState();
		UpdateVisualStateForChevron();
	}

	private void UpdateVisualStateForChevron()
	{
		NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
		if (navigationViewItemPresenter != null)
		{
			string text = ((!HasChildren() || (m_isClosedCompact && ShouldRepeaterShowInFlyout())) ? "ChevronHidden" : (IsExpanded ? "ChevronVisibleOpen" : "ChevronVisibleClosed"));
			VisualStateManager.GoToState((FrameworkElement)(object)navigationViewItemPresenter, text, true);
		}
	}

	internal bool HasChildren()
	{
		if (MenuItems.Count <= 0 && (MenuItemsSource == null || m_repeater == null || m_repeater.ItemsSourceView.Count <= 0))
		{
			return HasUnrealizedChildren;
		}
		return true;
	}

	private bool ShouldShowIcon()
	{
		return Icon != null;
	}

	private bool ShouldEnableToolTip()
	{
		if (IsOnLeftNav())
		{
			return m_isClosedCompact;
		}
		return false;
	}

	private bool ShouldShowContent()
	{
		return ((ContentControl)this).Content != null;
	}

	private bool IsOnLeftNav()
	{
		NavigationViewRepeaterPosition position = base.Position;
		if (position != NavigationViewRepeaterPosition.LeftNav)
		{
			return position == NavigationViewRepeaterPosition.LeftFooter;
		}
		return true;
	}

	private bool IsOnTopPrimary()
	{
		return base.Position == NavigationViewRepeaterPosition.TopPrimary;
	}

	private UIElement GetPresenterOrItem()
	{
		NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
		if (navigationViewItemPresenter != null)
		{
			return (UIElement)(object)navigationViewItemPresenter;
		}
		return (UIElement)(object)this;
	}

	private NavigationViewItemPresenter GetPresenter()
	{
		NavigationViewItemPresenter result = null;
		if (m_navigationViewItemPresenter != null)
		{
			result = m_navigationViewItemPresenter;
		}
		return result;
	}

	internal ItemsRepeater GetRepeater()
	{
		return m_repeater;
	}

	internal void ShowHideChildren()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ItemsRepeater repeater = m_repeater;
		if (repeater == null)
		{
			return;
		}
		bool isExpanded = IsExpanded;
		Visibility visibility = (Visibility)((!isExpanded) ? 2 : 0);
		((UIElement)repeater).Visibility = visibility;
		if (!ShouldRepeaterShowInFlyout())
		{
			return;
		}
		if (isExpanded)
		{
			if (!m_isRepeaterParentedToFlyout)
			{
				ReparentRepeater();
			}
			SharedHelpers.QueueCallbackForCompositionRendering(delegate
			{
				FlyoutBase.ShowAttachedFlyout((FrameworkElement)(object)m_rootGrid);
			});
		}
		else
		{
			FlyoutBase.GetAttachedFlyout((FrameworkElement)(object)m_rootGrid)?.Hide();
		}
	}

	private void ReparentRepeater()
	{
		if (!HasChildren())
		{
			return;
		}
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			if (ShouldRepeaterShowInFlyout() && !m_isRepeaterParentedToFlyout)
			{
				((Panel)m_rootGrid).Children.Remove((UIElement)(object)repeater);
				((Panel)m_flyoutContentGrid).Children.Add((UIElement)(object)repeater);
				m_isRepeaterParentedToFlyout = true;
				PropagateDepthToChildren(0);
			}
			else if (!ShouldRepeaterShowInFlyout() && m_isRepeaterParentedToFlyout)
			{
				((Panel)m_flyoutContentGrid).Children.Remove((UIElement)(object)repeater);
				((Panel)m_rootGrid).Children.Add((UIElement)(object)repeater);
				m_isRepeaterParentedToFlyout = false;
				PropagateDepthToChildren(1);
			}
		}
	}

	internal bool ShouldRepeaterShowInFlyout()
	{
		if (!m_isClosedCompact || !base.IsTopLevelItem)
		{
			return IsOnTopPrimary();
		}
		return true;
	}

	internal bool IsRepeaterVisible()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			return (int)((UIElement)repeater).Visibility == 0;
		}
		return false;
	}

	private void UpdateItemIndentation()
	{
		NavigationViewItemPresenter navigationViewItemPresenter = m_navigationViewItemPresenter;
		if (navigationViewItemPresenter != null)
		{
			int num = base.Depth * 25;
			navigationViewItemPresenter.UpdateContentLeftIndentation(num);
		}
	}

	internal void PropagateDepthToChildren(int depth)
	{
		ItemsRepeater repeater = m_repeater;
		if (repeater == null)
		{
			return;
		}
		int count = repeater.ItemsSourceView.Count;
		for (int i = 0; i < count; i++)
		{
			UIElement val = repeater.TryGetElement(i);
			if (val != null && val is NavigationViewItemBase navigationViewItemBase)
			{
				navigationViewItemBase.Depth = depth;
			}
		}
	}

	internal void OnExpandCollapseChevronTapped(object sender, TappedRoutedEventArgs args)
	{
		IsExpanded = !IsExpanded;
		((RoutedEventArgs)args).Handled = true;
	}

	private void OnFlyoutClosing(object sender, FlyoutBaseClosingEventArgs args)
	{
		IsExpanded = false;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new NavigationViewItemAutomationPeer(this);
	}

	protected override void OnContentChanged(object oldContent, object newContent)
	{
		((ContentControl)this).OnContentChanged(oldContent, newContent);
		SuggestedToolTipChanged(newContent);
		UpdateVisualStateNoTransition();
		if (!IsOnLeftNav())
		{
			GetNavigationView()?.TopNavigationViewItemContentChanged();
		}
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		((FrameworkElement)this).OnGotFocus(e);
		object originalSource = e.OriginalSource;
		Control val = (Control)((originalSource is Control) ? originalSource : null);
		if (val != null && ((UIElement)val).IsKeyboardFocused && InputManager.Current.MostRecentInputDevice is KeyboardDevice)
		{
			m_hasKeyboardFocus = true;
			UpdateVisualStateNoTransition();
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		((UIElement)this).OnLostFocus(e);
		if (m_hasKeyboardFocus)
		{
			m_hasKeyboardFocus = false;
			UpdateVisualStateNoTransition();
		}
	}

	private void OnPresenterPointerPressed(object sender, MouseEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		m_isPressed = (int)args.LeftButton == 1 || (int)args.RightButton == 1;
		if (GetPresenterOrItem().CaptureMouse())
		{
			m_isMouseCaptured = true;
		}
		UpdateVisualState(useTransitions: true);
	}

	private void OnPresenterPointerReleased(object sender, MouseEventArgs args)
	{
		if (m_isPressed)
		{
			m_isPressed = false;
			if (m_isMouseCaptured)
			{
				GetPresenterOrItem().ReleaseMouseCapture();
			}
		}
		UpdateVisualState(useTransitions: true);
	}

	private void OnPresenterPointerEntered(object sender, MouseEventArgs args)
	{
		ProcessPointerOver(args);
	}

	private void OnPresenterPointerMoved(object sender, MouseEventArgs args)
	{
		ProcessPointerOver(args);
	}

	private void OnPresenterPointerExited(object sender, MouseEventArgs args)
	{
		m_isPointerOver = false;
		UpdateVisualState(useTransitions: true);
	}

	private void OnPresenterPointerCanceled(object sender, MouseEventArgs args)
	{
		ProcessPointerCanceled(args);
	}

	private void OnPresenterPointerCaptureLost(object sender, MouseEventArgs args)
	{
		ProcessPointerCanceled(args);
	}

	private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
	{
		if (!((UIElement)this).IsEnabled)
		{
			m_isPressed = false;
			m_isPointerOver = false;
			if (m_isMouseCaptured)
			{
				GetPresenterOrItem().ReleaseMouseCapture();
				m_isMouseCaptured = false;
			}
		}
		UpdateVisualState(useTransitions: true);
	}

	internal void RotateExpandCollapseChevron(bool isExpanded)
	{
		GetPresenter()?.RotateExpandCollapseChevron(isExpanded);
	}

	private void ProcessPointerCanceled(MouseEventArgs args)
	{
		m_isPressed = false;
		m_isPointerOver = false;
		m_isMouseCaptured = false;
		UpdateVisualState(useTransitions: true);
	}

	private void ProcessPointerOver(MouseEventArgs args)
	{
		if (!m_isPointerOver)
		{
			m_isPointerOver = true;
			UpdateVisualState(useTransitions: true);
		}
	}

	private void HookInputEvents(IControlProtected controlProtected)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		UIElement obj = init();
		obj.MouseDown += new MouseButtonEventHandler(OnPresenterPointerPressed);
		obj.MouseEnter += new MouseEventHandler(OnPresenterPointerEntered);
		obj.MouseMove += new MouseEventHandler(OnPresenterPointerMoved);
		obj.AddHandler(UIElement.MouseUpEvent, (Delegate)new MouseButtonEventHandler(OnPresenterPointerReleased), true);
		obj.AddHandler(UIElement.MouseLeaveEvent, (Delegate)new MouseEventHandler(OnPresenterPointerExited), true);
		obj.AddHandler(UIElement.LostMouseCaptureEvent, (Delegate)new MouseEventHandler(OnPresenterPointerCaptureLost), true);
		UIElement init()
		{
			NavigationViewItemPresenter templateChildT = CppWinRTHelpers.GetTemplateChildT<NavigationViewItemPresenter>("NavigationViewItemPresenter", controlProtected);
			if (templateChildT != null)
			{
				m_navigationViewItemPresenter = templateChildT;
				return (UIElement)(object)templateChildT;
			}
			return (UIElement)(object)this;
		}
	}

	private void UnhookInputEvents()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		object obj = ((object)m_navigationViewItemPresenter) ?? ((object)this);
		((UIElement)obj).MouseDown -= new MouseButtonEventHandler(OnPresenterPointerPressed);
		((UIElement)obj).MouseEnter -= new MouseEventHandler(OnPresenterPointerEntered);
		((UIElement)obj).MouseMove -= new MouseEventHandler(OnPresenterPointerMoved);
		((UIElement)obj).RemoveHandler(UIElement.MouseUpEvent, (Delegate)new MouseButtonEventHandler(OnPresenterPointerReleased));
		((UIElement)obj).RemoveHandler(UIElement.MouseLeaveEvent, (Delegate)new MouseEventHandler(OnPresenterPointerExited));
		((UIElement)obj).RemoveHandler(UIElement.LostMouseCaptureEvent, (Delegate)new MouseEventHandler(OnPresenterPointerCaptureLost));
	}

	private void UnhookEventsAndClearFields()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		UnhookInputEvents();
		m_flyoutClosingRevoker?.Revoke();
		m_splitViewIsPaneOpenChangedRevoker?.Revoke();
		m_splitViewDisplayModeChangedRevoker?.Revoke();
		m_splitViewCompactPaneLengthChangedRevoker?.Revoke();
		m_repeaterElementPreparedRevoker?.Revoke();
		m_repeaterElementClearingRevoker?.Revoke();
		((UIElement)this).IsEnabledChanged -= new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		m_itemsSourceViewCollectionChangedRevoker?.Revoke();
		m_rootGrid = null;
		m_navigationViewItemPresenter = null;
		m_toolTip = null;
		m_repeater = null;
		m_flyoutContentGrid = null;
	}

	private static void OnIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationViewItem)(object)sender).OnIconPropertyChanged(args);
	}

	private static void OnIsExpandedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		NavigationViewItem navigationViewItem = (NavigationViewItem)(object)sender;
		navigationViewItem.OnIsExpandedPropertyChanged(args);
		navigationViewItem.IsExpandedChanged?.Invoke((DependencyObject)(object)navigationViewItem, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
	}

	private static void OnHasUnrealizedChildrenPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationViewItem)(object)sender).OnHasUnrealizedChildrenPropertyChanged(args);
	}

	private static void OnMenuItemsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationViewItem)(object)sender).OnMenuItemsPropertyChanged(args);
	}

	private static void OnMenuItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationViewItem)(object)sender).OnMenuItemsSourcePropertyChanged(args);
	}
}
