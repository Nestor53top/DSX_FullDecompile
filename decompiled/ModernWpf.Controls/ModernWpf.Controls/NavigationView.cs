using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Windows.Threading;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;
using ModernWpf.Input;
using ModernWpf.Media.Animation;

namespace ModernWpf.Controls;

public class NavigationView : ContentControl, IControlProtected
{
	private const string c_togglePaneButtonName = "TogglePaneButton";

	private const string c_paneTitleHolderFrameworkElement = "PaneTitleHolder";

	private const string c_paneTitleFrameworkElement = "PaneTitleTextBlock";

	private const string c_rootSplitViewName = "RootSplitView";

	private const string c_menuItemsHost = "MenuItemsHost";

	private const string c_footerMenuItemsHost = "FooterMenuItemsHost";

	private const string c_selectionIndicatorName = "SelectionIndicator";

	private const string c_paneContentGridName = "PaneContentGrid";

	private const string c_rootGridName = "RootGrid";

	private const string c_contentGridName = "ContentGrid";

	private const string c_searchButtonName = "PaneAutoSuggestButton";

	private const string c_paneToggleButtonIconGridColumnName = "PaneToggleButtonIconWidthColumn";

	private const string c_togglePaneTopPadding = "TogglePaneTopPadding";

	private const string c_contentPaneTopPadding = "ContentPaneTopPadding";

	private const string c_contentLeftPadding = "ContentLeftPadding";

	private const string c_navViewBackButton = "NavigationViewBackButton";

	private const string c_navViewBackButtonToolTip = "NavigationViewBackButtonToolTip";

	private const string c_navViewCloseButton = "NavigationViewCloseButton";

	private const string c_navViewCloseButtonToolTip = "NavigationViewCloseButtonToolTip";

	private const string c_paneShadowReceiverCanvas = "PaneShadowReceiver";

	private const string c_flyoutRootGrid = "FlyoutRootGrid";

	private const string c_topNavMenuItemsHost = "TopNavMenuItemsHost";

	private const string c_topNavFooterMenuItemsHost = "TopFooterMenuItemsHost";

	private const string c_topNavOverflowButton = "TopNavOverflowButton";

	private const string c_topNavMenuItemsOverflowHost = "TopNavMenuItemsOverflowHost";

	private const string c_topNavGrid = "TopNavGrid";

	private const string c_topNavContentOverlayAreaGrid = "TopNavContentOverlayAreaGrid";

	private const string c_leftNavPaneAutoSuggestBoxPresenter = "PaneAutoSuggestBoxPresenter";

	private const string c_topNavPaneAutoSuggestBoxPresenter = "TopPaneAutoSuggestBoxPresenter";

	private const string c_paneTitlePresenter = "PaneTitlePresenter";

	private const string c_leftNavFooterContentBorder = "FooterContentBorder";

	private const string c_leftNavPaneHeaderContentBorder = "PaneHeaderContentBorder";

	private const string c_leftNavPaneCustomContentBorder = "PaneCustomContentBorder";

	private const string c_itemsContainer = "ItemsContainerGrid";

	private const string c_itemsContainerRow = "ItemsContainerRow";

	private const string c_visualItemsSeparator = "VisualItemsSeparator";

	private const string c_menuItemsScrollViewer = "MenuItemsScrollViewer";

	private const string c_footerItemsScrollViewer = "FooterItemsScrollViewer";

	private const string c_paneHeaderOnTopPane = "PaneHeaderOnTopPane";

	private const string c_paneTitleOnTopPane = "PaneTitleOnTopPane";

	private const string c_paneCustomContentOnTopPane = "PaneCustomContentOnTopPane";

	private const string c_paneFooterOnTopPane = "PaneFooterOnTopPane";

	private const string c_paneHeaderCloseButtonColumn = "PaneHeaderCloseButtonColumn";

	private const string c_paneHeaderToggleButtonColumn = "PaneHeaderToggleButtonColumn";

	private const string c_paneHeaderContentBorderRow = "PaneHeaderContentBorderRow";

	private const int c_backButtonHeight = 40;

	private const int c_backButtonWidth = 40;

	private const int c_paneToggleButtonHeight = 40;

	private const int c_paneToggleButtonWidth = 40;

	private const int c_toggleButtonHeightWhenShouldPreserveNavigationViewRS3Behavior = 56;

	private const int c_backButtonRowDefinition = 1;

	private const float c_paneElevationTranslationZ = 32f;

	private const int c_mainMenuBlockIndex = 0;

	private const int c_footerMenuBlockIndex = 1;

	private const int s_itemNotFound = -1;

	private static readonly Size c_infSize;

	private static readonly ResourceAccessor ResourceAccessor;

	private static readonly Point c_frame1point1;

	private static readonly Point c_frame1point2;

	private static readonly Point c_frame2point1;

	private static readonly Point c_frame2point2;

	private bool m_InitialNonForcedModeUpdate = true;

	private NavigationViewItemsFactory m_navigationViewItemsFactory;

	private Button m_paneToggleButton;

	private SplitView m_rootSplitView;

	private NavigationViewItem m_settingsItem;

	private RowDefinition m_itemsContainerRow;

	private FrameworkElement m_menuItemsScrollViewer;

	private FrameworkElement m_footerItemsScrollViewer;

	private UIElement m_paneContentGrid;

	private ColumnDefinition m_paneToggleButtonIconGridColumn;

	private FrameworkElement m_paneTitleHolderFrameworkElement;

	private FrameworkElement m_paneTitleFrameworkElement;

	private FrameworkElement m_visualItemsSeparator;

	private Button m_paneSearchButton;

	private Button m_backButton;

	private Button m_closeButton;

	private ItemsRepeater m_leftNavRepeater;

	private ItemsRepeater m_topNavRepeater;

	private ItemsRepeater m_leftNavFooterMenuRepeater;

	private ItemsRepeater m_topNavFooterMenuRepeater;

	private Button m_topNavOverflowButton;

	private ItemsRepeater m_topNavRepeaterOverflowView;

	private Grid m_topNavGrid;

	private Border m_topNavContentOverlayAreaGrid;

	private UIElement m_prevIndicator;

	private UIElement m_nextIndicator;

	private UIElement m_activeIndicator;

	private object m_lastSelectedItemPendingAnimationInTopNav;

	private FrameworkElement m_togglePaneTopPadding;

	private FrameworkElement m_contentPaneTopPadding;

	private FrameworkElement m_contentLeftPadding;

	private CoreApplicationViewTitleBar m_coreTitleBar;

	private ContentControl m_leftNavPaneAutoSuggestBoxPresenter;

	private ContentControl m_topNavPaneAutoSuggestBoxPresenter;

	private ContentControl m_leftNavPaneHeaderContentBorder;

	private ContentControl m_leftNavPaneCustomContentBorder;

	private ContentControl m_leftNavFooterContentBorder;

	private ContentControl m_paneHeaderOnTopPane;

	private ContentControl m_paneTitleOnTopPane;

	private ContentControl m_paneCustomContentOnTopPane;

	private ContentControl m_paneFooterOnTopPane;

	private ContentControl m_paneTitlePresenter;

	private ColumnDefinition m_paneHeaderCloseButtonColumn;

	private ColumnDefinition m_paneHeaderToggleButtonColumn;

	private RowDefinition m_paneHeaderContentBorderRow;

	private NavigationViewItem m_lastItemExpandedIntoFlyout;

	private bool m_layoutUpdatedToken;

	private FrameworkElementSizeChangedRevoker m_itemsContainerSizeChangedRevoker;

	private ItemsSourceView.CollectionChangedRevoker m_menuItemsCollectionChangedRevoker;

	private ItemsSourceView.CollectionChangedRevoker m_footerItemsCollectionChangedRevoker;

	private ItemsSourceView.CollectionChangedRevoker m_topNavOverflowItemsCollectionChangedRevoker;

	private bool m_wasForceClosed;

	private bool m_isClosedCompact;

	private bool m_blockNextClosingEvent;

	private bool m_initialListSizeStateSet;

	private TopNavigationViewDataProvider m_topDataProvider = new TopNavigationViewDataProvider();

	private SelectionModel m_selectionModel = new SelectionModel();

	private List<object> m_selectionModelSource;

	private ItemsSourceView m_menuItemsSource;

	private ItemsSourceView m_footerItemsSource;

	private bool m_appliedTemplate;

	private bool m_shouldIgnoreNextSelectionChange;

	private bool m_shouldIgnoreNextSelectionChangeBecauseSettingsRestore;

	private bool m_selectionChangeFromOverflowMenu;

	private bool m_shouldRaiseItemInvokedAfterSelection;

	private TopNavigationViewLayoutState m_topNavigationMode;

	private float m_topNavigationRecoveryGracePeriodWidth = 5f;

	private bool m_isOpenPaneForInteraction;

	private bool m_moveTopNavOverflowItemOnFlyoutClose;

	private bool m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise;

	private bool m_OrientationChangedPendingAnimation;

	private bool m_TabKeyPrecedesFocusChange;

	private GettingFocusHelper m_leftNavRepeaterGettingFocusHelper;

	private GettingFocusHelper m_topNavRepeaterGettingFocusHelper;

	private GettingFocusHelper m_leftNavFooterMenuRepeaterGettingFocusHelper;

	private GettingFocusHelper m_topNavFooterMenuRepeaterGettingFocusHelper;

	private readonly BitmapCache m_bitmapCache;

	private static readonly PropertyPath s_opacityPath;

	private static readonly PropertyPath s_centerXPath;

	private static readonly PropertyPath s_centerYPath;

	private static readonly PropertyPath s_scaleXPath;

	private static readonly PropertyPath s_scaleYPath;

	private static readonly PropertyPath s_translateXPath;

	private static readonly PropertyPath s_translateYPath;

	public static readonly DependencyProperty IsPaneOpenProperty;

	public static readonly DependencyProperty CompactModeThresholdWidthProperty;

	public static readonly DependencyProperty ExpandedModeThresholdWidthProperty;

	private static readonly DependencyPropertyKey FooterMenuItemsPropertyKey;

	private static readonly DependencyProperty FooterMenuItemsProperty;

	public static readonly DependencyProperty FooterMenuItemsSourceProperty;

	public static readonly DependencyProperty PaneFooterProperty;

	public static readonly DependencyProperty HeaderProperty;

	public static readonly DependencyProperty HeaderTemplateProperty;

	private static readonly DependencyPropertyKey DisplayModePropertyKey;

	public static readonly DependencyProperty DisplayModeProperty;

	public static readonly DependencyProperty IsSettingsVisibleProperty;

	public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty;

	public static readonly DependencyProperty AlwaysShowHeaderProperty;

	public static readonly DependencyProperty CompactPaneLengthProperty;

	public static readonly DependencyProperty OpenPaneLengthProperty;

	public static readonly DependencyProperty PaneToggleButtonStyleProperty;

	public static readonly DependencyProperty SelectedItemProperty;

	private static readonly DependencyPropertyKey MenuItemsPropertyKey;

	private static readonly DependencyProperty MenuItemsProperty;

	public static readonly DependencyProperty MenuItemsSourceProperty;

	private static readonly DependencyPropertyKey SettingsItemPropertyKey;

	public static readonly DependencyProperty SettingsItemProperty;

	public static readonly DependencyProperty AutoSuggestBoxProperty;

	public static readonly DependencyProperty MenuItemTemplateProperty;

	public static readonly DependencyProperty MenuItemTemplateSelectorProperty;

	public static readonly DependencyProperty MenuItemContainerStyleProperty;

	public static readonly DependencyProperty MenuItemContainerStyleSelectorProperty;

	public static readonly DependencyProperty IsBackButtonVisibleProperty;

	public static readonly DependencyProperty IsBackEnabledProperty;

	public static readonly DependencyProperty PaneTitleProperty;

	public static readonly DependencyProperty PaneDisplayModeProperty;

	public static readonly DependencyProperty PaneHeaderProperty;

	public static readonly DependencyProperty PaneCustomContentProperty;

	public static readonly DependencyProperty ContentOverlayProperty;

	public static readonly DependencyProperty IsPaneVisibleProperty;

	public static readonly DependencyProperty SelectionFollowsFocusProperty;

	private static readonly DependencyPropertyKey TemplateSettingsPropertyKey;

	public static readonly DependencyProperty TemplateSettingsProperty;

	public static readonly DependencyProperty ShoulderNavigationEnabledProperty;

	public static readonly DependencyProperty OverflowLabelModeProperty;

	public static readonly DependencyProperty IsTitleBarAutoPaddingEnabledProperty;

	public bool IsPaneOpen
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsPaneOpenProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsPaneOpenProperty, (object)value);
		}
	}

	public double CompactModeThresholdWidth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CompactModeThresholdWidthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CompactModeThresholdWidthProperty, (object)value);
		}
	}

	public double ExpandedModeThresholdWidth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandedModeThresholdWidthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ExpandedModeThresholdWidthProperty, (object)value);
		}
	}

	public IList FooterMenuItems => (IList)((DependencyObject)this).GetValue(FooterMenuItemsProperty);

	public object FooterMenuItemsSource
	{
		get
		{
			return ((DependencyObject)this).GetValue(FooterMenuItemsSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FooterMenuItemsSourceProperty, value);
		}
	}

	public UIElement PaneFooter
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(PaneFooterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneFooterProperty, (object)value);
		}
	}

	public object Header
	{
		get
		{
			return ((DependencyObject)this).GetValue(HeaderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HeaderProperty, value);
		}
	}

	public DataTemplate HeaderTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(HeaderTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HeaderTemplateProperty, (object)value);
		}
	}

	public NavigationViewDisplayMode DisplayMode => (NavigationViewDisplayMode)((DependencyObject)this).GetValue(DisplayModeProperty);

	public bool IsSettingsVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSettingsVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsSettingsVisibleProperty, (object)value);
		}
	}

	public bool IsPaneToggleButtonVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsPaneToggleButtonVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsPaneToggleButtonVisibleProperty, (object)value);
		}
	}

	public bool AlwaysShowHeader
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(AlwaysShowHeaderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AlwaysShowHeaderProperty, (object)value);
		}
	}

	public double CompactPaneLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CompactPaneLengthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CompactPaneLengthProperty, (object)value);
		}
	}

	public double OpenPaneLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OpenPaneLengthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OpenPaneLengthProperty, (object)value);
		}
	}

	public Style PaneToggleButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(PaneToggleButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneToggleButtonStyleProperty, (object)value);
		}
	}

	public object SelectedItem
	{
		get
		{
			return ((DependencyObject)this).GetValue(SelectedItemProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectedItemProperty, value);
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

	public object SettingsItem => ((DependencyObject)this).GetValue(SettingsItemProperty);

	public AutoSuggestBox AutoSuggestBox
	{
		get
		{
			return (AutoSuggestBox)((DependencyObject)this).GetValue(AutoSuggestBoxProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AutoSuggestBoxProperty, (object)value);
		}
	}

	public DataTemplate MenuItemTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(MenuItemTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuItemTemplateProperty, (object)value);
		}
	}

	public DataTemplateSelector MenuItemTemplateSelector
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplateSelector)((DependencyObject)this).GetValue(MenuItemTemplateSelectorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuItemTemplateSelectorProperty, (object)value);
		}
	}

	public Style MenuItemContainerStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(MenuItemContainerStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuItemContainerStyleProperty, (object)value);
		}
	}

	public StyleSelector MenuItemContainerStyleSelector
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (StyleSelector)((DependencyObject)this).GetValue(MenuItemContainerStyleSelectorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuItemContainerStyleSelectorProperty, (object)value);
		}
	}

	public NavigationViewBackButtonVisible IsBackButtonVisible
	{
		get
		{
			return (NavigationViewBackButtonVisible)((DependencyObject)this).GetValue(IsBackButtonVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsBackButtonVisibleProperty, (object)value);
		}
	}

	public bool IsBackEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsBackEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsBackEnabledProperty, (object)value);
		}
	}

	public string PaneTitle
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PaneTitleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneTitleProperty, (object)value);
		}
	}

	public NavigationViewPaneDisplayMode PaneDisplayMode
	{
		get
		{
			return (NavigationViewPaneDisplayMode)((DependencyObject)this).GetValue(PaneDisplayModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneDisplayModeProperty, (object)value);
		}
	}

	public UIElement PaneHeader
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(PaneHeaderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneHeaderProperty, (object)value);
		}
	}

	public UIElement PaneCustomContent
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(PaneCustomContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneCustomContentProperty, (object)value);
		}
	}

	public UIElement ContentOverlay
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(ContentOverlayProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentOverlayProperty, (object)value);
		}
	}

	public bool IsPaneVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsPaneVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsPaneVisibleProperty, (object)value);
		}
	}

	public NavigationViewSelectionFollowsFocus SelectionFollowsFocus
	{
		get
		{
			return (NavigationViewSelectionFollowsFocus)((DependencyObject)this).GetValue(SelectionFollowsFocusProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectionFollowsFocusProperty, (object)value);
		}
	}

	public NavigationViewTemplateSettings TemplateSettings
	{
		get
		{
			return (NavigationViewTemplateSettings)((DependencyObject)this).GetValue(TemplateSettingsProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)value);
		}
	}

	public NavigationViewShoulderNavigationEnabled ShoulderNavigationEnabled
	{
		get
		{
			return (NavigationViewShoulderNavigationEnabled)((DependencyObject)this).GetValue(ShoulderNavigationEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShoulderNavigationEnabledProperty, (object)value);
		}
	}

	public NavigationViewOverflowLabelMode OverflowLabelMode
	{
		get
		{
			return (NavigationViewOverflowLabelMode)((DependencyObject)this).GetValue(OverflowLabelModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OverflowLabelModeProperty, (object)value);
		}
	}

	public bool IsTitleBarAutoPaddingEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsTitleBarAutoPaddingEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsTitleBarAutoPaddingEnabledProperty, (object)value);
		}
	}

	public event TypedEventHandler<NavigationView, NavigationViewSelectionChangedEventArgs> SelectionChanged;

	public event TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs> ItemInvoked;

	public event TypedEventHandler<NavigationView, NavigationViewDisplayModeChangedEventArgs> DisplayModeChanged;

	public event TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs> BackRequested;

	public event TypedEventHandler<NavigationView, object> PaneClosed;

	public event TypedEventHandler<NavigationView, NavigationViewPaneClosingEventArgs> PaneClosing;

	public event TypedEventHandler<NavigationView, object> PaneOpened;

	public event TypedEventHandler<NavigationView, object> PaneOpening;

	public event TypedEventHandler<NavigationView, NavigationViewItemExpandingEventArgs> Expanding;

	public event TypedEventHandler<NavigationView, NavigationViewItemCollapsedEventArgs> Collapsed;

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new NavigationViewAutomationPeer(this);
	}

	private void UnhookEventsAndClearFields(bool isFromDestructor = false)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		if (m_coreTitleBar != null)
		{
			m_coreTitleBar.LayoutMetricsChanged -= OnTitleBarMetricsChanged;
			m_coreTitleBar.IsVisibleChanged -= OnTitleBarIsVisibleChanged;
		}
		if (m_paneToggleButton != null)
		{
			((ButtonBase)m_paneToggleButton).Click -= new RoutedEventHandler(OnPaneToggleButtonClick);
		}
		m_settingsItem = null;
		if (m_paneSearchButton != null)
		{
			((ButtonBase)m_paneSearchButton).Click -= new RoutedEventHandler(OnPaneSearchButtonClick);
			m_paneSearchButton = null;
		}
		m_paneHeaderOnTopPane = null;
		m_paneTitleOnTopPane = null;
		m_itemsContainerSizeChangedRevoker?.Revoke();
		if (m_paneTitleHolderFrameworkElement != null)
		{
			m_paneTitleHolderFrameworkElement.SizeChanged -= new SizeChangedEventHandler(OnPaneTitleHolderSizeChanged);
			m_paneTitleHolderFrameworkElement = null;
		}
		m_paneTitleFrameworkElement = null;
		m_paneTitlePresenter = null;
		m_paneHeaderCloseButtonColumn = null;
		m_paneHeaderToggleButtonColumn = null;
		m_paneHeaderContentBorderRow = null;
		if (m_leftNavRepeater != null)
		{
			m_leftNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;
			m_leftNavRepeater.ElementClearing -= OnRepeaterElementClearing;
			((UIElement)m_leftNavRepeater).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_leftNavRepeaterGettingFocusHelper?.Dispose();
			m_leftNavRepeater = null;
		}
		if (m_topNavRepeater != null)
		{
			m_topNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;
			m_topNavRepeater.ElementClearing -= OnRepeaterElementClearing;
			((UIElement)m_topNavRepeater).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_topNavRepeaterGettingFocusHelper?.Dispose();
			m_topNavRepeater = null;
		}
		if (m_leftNavFooterMenuRepeater != null)
		{
			m_leftNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;
			m_leftNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
			((UIElement)m_leftNavFooterMenuRepeater).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_leftNavFooterMenuRepeaterGettingFocusHelper?.Dispose();
			m_leftNavFooterMenuRepeater = null;
		}
		if (m_topNavFooterMenuRepeater != null)
		{
			m_topNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;
			m_topNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
			((UIElement)m_topNavFooterMenuRepeater).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_topNavFooterMenuRepeaterGettingFocusHelper?.Dispose();
			m_topNavFooterMenuRepeater = null;
		}
		m_footerItemsCollectionChangedRevoker?.Revoke();
		m_menuItemsCollectionChangedRevoker?.Revoke();
		if (m_topNavRepeaterOverflowView != null)
		{
			m_topNavRepeaterOverflowView.ElementPrepared -= OnRepeaterElementPrepared;
			m_topNavRepeaterOverflowView.ElementClearing -= OnRepeaterElementClearing;
			m_topNavRepeaterOverflowView = null;
		}
		m_topNavOverflowItemsCollectionChangedRevoker?.Revoke();
		if (isFromDestructor)
		{
			m_selectionModel.SelectionChanged -= OnSelectionModelSelectionChanged;
		}
	}

	static NavigationView()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01a5: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_01f3: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Expected O, but got Unknown
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected O, but got Unknown
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Expected O, but got Unknown
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Expected O, but got Unknown
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Expected O, but got Unknown
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Expected O, but got Unknown
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Expected O, but got Unknown
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Expected O, but got Unknown
		//IL_044b: Expected O, but got Unknown
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Expected O, but got Unknown
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Expected O, but got Unknown
		//IL_0499: Expected O, but got Unknown
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Expected O, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected O, but got Unknown
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Expected O, but got Unknown
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Expected O, but got Unknown
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Expected O, but got Unknown
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Expected O, but got Unknown
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Expected O, but got Unknown
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Expected O, but got Unknown
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Expected O, but got Unknown
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Expected O, but got Unknown
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Expected O, but got Unknown
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Expected O, but got Unknown
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Expected O, but got Unknown
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Expected O, but got Unknown
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Expected O, but got Unknown
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Expected O, but got Unknown
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Expected O, but got Unknown
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Expected O, but got Unknown
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Expected O, but got Unknown
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Expected O, but got Unknown
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Expected O, but got Unknown
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Expected O, but got Unknown
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Expected O, but got Unknown
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Expected O, but got Unknown
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Expected O, but got Unknown
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Expected O, but got Unknown
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Expected O, but got Unknown
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Expected O, but got Unknown
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Expected O, but got Unknown
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Expected O, but got Unknown
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Expected O, but got Unknown
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Expected O, but got Unknown
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Expected O, but got Unknown
		//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Expected O, but got Unknown
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Expected O, but got Unknown
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Expected O, but got Unknown
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_0961: Expected O, but got Unknown
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Expected O, but got Unknown
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Expected O, but got Unknown
		c_infSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
		ResourceAccessor = new ResourceAccessor(typeof(NavigationView));
		c_frame1point1 = new Point(0.9, 0.1);
		c_frame1point2 = new Point(1.0, 0.2);
		c_frame2point1 = new Point(0.1, 0.9);
		c_frame2point2 = new Point(0.2, 1.0);
		s_opacityPath = new PropertyPath((object)UIElement.OpacityProperty);
		s_centerXPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.CenterX)", Array.Empty<object>());
		s_centerYPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.CenterY)", Array.Empty<object>());
		s_scaleXPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", Array.Empty<object>());
		s_scaleYPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", Array.Empty<object>());
		s_translateXPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)", Array.Empty<object>());
		s_translateYPath = new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)", Array.Empty<object>());
		IsPaneOpenProperty = DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsPaneOpenPropertyChanged)));
		CompactModeThresholdWidthProperty = DependencyProperty.Register("CompactModeThresholdWidth", typeof(double), typeof(NavigationView), new PropertyMetadata((object)641.0, new PropertyChangedCallback(OnCompactModeThresholdWidthPropertyChanged), new CoerceValueCallback(CoerceToGreaterThanZero)));
		ExpandedModeThresholdWidthProperty = DependencyProperty.Register("ExpandedModeThresholdWidth", typeof(double), typeof(NavigationView), new PropertyMetadata((object)1008.0, new PropertyChangedCallback(OnExpandedModeThresholdWidthPropertyChanged), new CoerceValueCallback(CoerceToGreaterThanZero)));
		FooterMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("FooterMenuItems", typeof(IList), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnFooterMenuItemsPropertyChanged)));
		FooterMenuItemsProperty = FooterMenuItemsPropertyKey.DependencyProperty;
		FooterMenuItemsSourceProperty = DependencyProperty.Register("FooterMenuItemsSource", typeof(object), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnFooterMenuItemsSourcePropertyChanged)));
		PaneFooterProperty = DependencyProperty.Register("PaneFooter", typeof(UIElement), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnPaneFooterPropertyChanged)));
		HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnHeaderPropertyChanged)));
		HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnHeaderTemplatePropertyChanged)));
		DisplayModePropertyKey = DependencyProperty.RegisterReadOnly("DisplayMode", typeof(NavigationViewDisplayMode), typeof(NavigationView), new PropertyMetadata((object)NavigationViewDisplayMode.Minimal, new PropertyChangedCallback(OnDisplayModePropertyChanged)));
		DisplayModeProperty = DisplayModePropertyKey.DependencyProperty;
		IsSettingsVisibleProperty = DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsSettingsVisiblePropertyChanged)));
		IsPaneToggleButtonVisibleProperty = DependencyProperty.Register("IsPaneToggleButtonVisible", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsPaneToggleButtonVisiblePropertyChanged)));
		AlwaysShowHeaderProperty = DependencyProperty.Register("AlwaysShowHeader", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnAlwaysShowHeaderPropertyChanged)));
		CompactPaneLengthProperty = DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(NavigationView), new PropertyMetadata((object)48.0, new PropertyChangedCallback(OnCompactPaneLengthPropertyChanged), new CoerceValueCallback(CoerceToGreaterThanZero)));
		OpenPaneLengthProperty = DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(NavigationView), new PropertyMetadata((object)320.0, new PropertyChangedCallback(OnOpenPaneLengthPropertyChanged), new CoerceValueCallback(CoerceToGreaterThanZero)));
		PaneToggleButtonStyleProperty = DependencyProperty.Register("PaneToggleButtonStyle", typeof(Style), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnPaneToggleButtonStylePropertyChanged)));
		SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemPropertyChanged)));
		MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(IList), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemsPropertyChanged)));
		MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;
		MenuItemsSourceProperty = DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemsSourcePropertyChanged)));
		SettingsItemPropertyKey = DependencyProperty.RegisterReadOnly("SettingsItem", typeof(object), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnSettingsItemPropertyChanged)));
		SettingsItemProperty = SettingsItemPropertyKey.DependencyProperty;
		AutoSuggestBoxProperty = DependencyProperty.Register("AutoSuggestBox", typeof(AutoSuggestBox), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnAutoSuggestBoxPropertyChanged)));
		MenuItemTemplateProperty = DependencyProperty.Register("MenuItemTemplate", typeof(DataTemplate), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemTemplatePropertyChanged)));
		MenuItemTemplateSelectorProperty = DependencyProperty.Register("MenuItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemTemplateSelectorPropertyChanged)));
		MenuItemContainerStyleProperty = DependencyProperty.Register("MenuItemContainerStyle", typeof(Style), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemContainerStylePropertyChanged)));
		MenuItemContainerStyleSelectorProperty = DependencyProperty.Register("MenuItemContainerStyleSelector", typeof(StyleSelector), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnMenuItemContainerStyleSelectorPropertyChanged)));
		IsBackButtonVisibleProperty = DependencyProperty.Register("IsBackButtonVisible", typeof(NavigationViewBackButtonVisible), typeof(NavigationView), new PropertyMetadata((object)NavigationViewBackButtonVisible.Auto, new PropertyChangedCallback(OnIsBackButtonVisiblePropertyChanged)));
		IsBackEnabledProperty = DependencyProperty.Register("IsBackEnabled", typeof(bool), typeof(NavigationView), new PropertyMetadata(new PropertyChangedCallback(OnIsBackEnabledPropertyChanged)));
		PaneTitleProperty = DependencyProperty.Register("PaneTitle", typeof(string), typeof(NavigationView), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnPaneTitlePropertyChanged)));
		PaneDisplayModeProperty = DependencyProperty.Register("PaneDisplayMode", typeof(NavigationViewPaneDisplayMode), typeof(NavigationView), new PropertyMetadata((object)NavigationViewPaneDisplayMode.Auto, new PropertyChangedCallback(OnPaneDisplayModePropertyChanged)));
		PaneHeaderProperty = DependencyProperty.Register("PaneHeader", typeof(UIElement), typeof(NavigationView), (PropertyMetadata)null);
		PaneCustomContentProperty = DependencyProperty.Register("PaneCustomContent", typeof(UIElement), typeof(NavigationView), (PropertyMetadata)null);
		ContentOverlayProperty = DependencyProperty.Register("ContentOverlay", typeof(UIElement), typeof(NavigationView), (PropertyMetadata)null);
		IsPaneVisibleProperty = DependencyProperty.Register("IsPaneVisible", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsPaneVisiblePropertyChanged)));
		SelectionFollowsFocusProperty = DependencyProperty.Register("SelectionFollowsFocus", typeof(NavigationViewSelectionFollowsFocus), typeof(NavigationView), new PropertyMetadata((object)NavigationViewSelectionFollowsFocus.Disabled, new PropertyChangedCallback(OnSelectionFollowsFocusPropertyChanged)));
		TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(NavigationViewTemplateSettings), typeof(NavigationView), (PropertyMetadata)null);
		TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;
		ShoulderNavigationEnabledProperty = DependencyProperty.Register("ShoulderNavigationEnabled", typeof(NavigationViewShoulderNavigationEnabled), typeof(NavigationView), new PropertyMetadata((object)NavigationViewShoulderNavigationEnabled.Never, new PropertyChangedCallback(OnShoulderNavigationEnabledPropertyChanged)));
		OverflowLabelModeProperty = DependencyProperty.Register("OverflowLabelMode", typeof(NavigationViewOverflowLabelMode), typeof(NavigationView), new PropertyMetadata((object)NavigationViewOverflowLabelMode.MoreLabel, new PropertyChangedCallback(OnOverflowLabelModePropertyChanged)));
		IsTitleBarAutoPaddingEnabledProperty = DependencyProperty.Register("IsTitleBarAutoPaddingEnabled", typeof(bool), typeof(NavigationView), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsTitleBarAutoPaddingEnabledPropertyChanged)));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationView), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NavigationView)));
	}

	public NavigationView()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)new NavigationViewTemplateSettings());
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		m_selectionModelSource = new List<object>(2);
		m_selectionModelSource.Add(null);
		m_selectionModelSource.Add(null);
		ObservableCollection<object> observableCollection = new ObservableCollection<object>();
		((DependencyObject)this).SetValue(MenuItemsPropertyKey, (object)observableCollection);
		ObservableCollection<object> observableCollection2 = new ObservableCollection<object>();
		((DependencyObject)this).SetValue(FooterMenuItemsPropertyKey, (object)observableCollection2);
		WeakReference<NavigationView> weakThis = new WeakReference<NavigationView>(this);
		m_topDataProvider.OnRawDataChanged(delegate(NotifyCollectionChangedEventArgs args)
		{
			if (weakThis.TryGetTarget(out var target))
			{
				target.OnTopNavDataSourceChanged(args);
			}
		});
		((FrameworkElement)this).Unloaded += new RoutedEventHandler(OnUnloaded);
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
		m_selectionModel.SingleSelect = true;
		m_selectionModel.Source = m_selectionModelSource;
		m_selectionModel.SelectionChanged += OnSelectionModelSelectionChanged;
		m_selectionModel.ChildrenRequested += OnSelectionModelChildrenRequested;
		m_navigationViewItemsFactory = new NavigationViewItemsFactory();
		m_bitmapCache = new BitmapCache();
		BitmapCache bitmapCache = m_bitmapCache;
		DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)this);
		bitmapCache.RenderAtScale = ((DpiScale)(ref dpi)).PixelsPerDip;
	}

	private void OnSelectionModelChildrenRequested(SelectionModel selectionModel, SelectionModelChildrenRequestedEventArgs e)
	{
		if (e.SourceIndex.GetSize() == 1)
		{
			e.Children = e.Source;
			return;
		}
		if (e.Source is NavigationViewItem nvi)
		{
			e.Children = GetChildren(nvi);
			return;
		}
		object childrenForItemInIndexPath = GetChildrenForItemInIndexPath(e.SourceIndex, forceRealize: true);
		if (childrenForItemInIndexPath != null)
		{
			e.Children = childrenForItemInIndexPath;
		}
	}

	private void OnFooterItemsSourceCollectionChanged(object sender, object e)
	{
		UpdateFooterRepeaterItemsSource(sourceCollectionReset: false, sourceCollectionChanged: true);
		UpdatePaneLayout();
	}

	private void OnOverflowItemsSourceCollectionChanged(object sender, object e)
	{
		if (m_topNavRepeaterOverflowView.ItemsSourceView.Count == 0)
		{
			SetOverflowButtonVisibility((Visibility)2);
		}
	}

	private void OnSelectionModelSelectionChanged(SelectionModel selectionModel, SelectionModelSelectionChangedEventArgs e)
	{
		object selectedItem = selectionModel.SelectedItem;
		if (m_shouldIgnoreNextSelectionChange || selectedItem == SelectedItem || !m_appliedTemplate)
		{
			return;
		}
		bool flag = true;
		IndexPath selectedIndex = selectionModel.SelectedIndex;
		if (IsTopNavigationView() && selectedIndex != null && selectedIndex.GetSize() > 1 && selectedIndex.GetAt(0) == 0 && !m_topDataProvider.IsItemInPrimaryList(selectedIndex.GetAt(1)))
		{
			if (init())
			{
				SelectandMoveOverflowItem(selectedItem, selectedIndex, closeFlyout: true);
				flag = false;
			}
			else
			{
				m_moveTopNavOverflowItemOnFlyoutClose = true;
			}
		}
		if (flag)
		{
			SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(selectedItem);
		}
		bool init()
		{
			NavigationViewItemBase containerForIndexPath = GetContainerForIndexPath(selectedIndex);
			if (containerForIndexPath != null && containerForIndexPath is NavigationViewItem nvi && DoesNavigationViewItemHaveChildren(nvi))
			{
				return false;
			}
			return true;
		}
	}

	private void SelectandMoveOverflowItem(object selectedItem, IndexPath selectedIndex, bool closeFlyout)
	{
		try
		{
			m_selectionChangeFromOverflowMenu = true;
			if (closeFlyout)
			{
				CloseTopNavigationViewFlyout();
			}
			if (!IsSelectionSuppressed(selectedItem))
			{
				SelectOverflowItem(selectedItem, selectedIndex);
			}
		}
		finally
		{
			m_selectionChangeFromOverflowMenu = false;
		}
	}

	private void CloseFlyoutIfRequired(NavigationViewItem selectedItem)
	{
		IndexPath selectedIndex = m_selectionModel.SelectedIndex;
		if (init() && selectedIndex != null && !DoesNavigationViewItemHaveChildren(selectedItem))
		{
			UIElement containerForIndex = GetContainerForIndex(selectedIndex.GetAt(1), selectedIndex.GetAt(0) == 1);
			if (containerForIndex != null && containerForIndex is NavigationViewItem navigationViewItem && navigationViewItem.ShouldRepeaterShowInFlyout())
			{
				navigationViewItem.IsExpanded = false;
			}
		}
		bool init()
		{
			SplitView rootSplitView = m_rootSplitView;
			if (rootSplitView != null)
			{
				SplitViewDisplayMode displayMode = rootSplitView.DisplayMode;
				if (rootSplitView.IsPaneOpen || (displayMode != SplitViewDisplayMode.CompactOverlay && displayMode != SplitViewDisplayMode.CompactInline))
				{
					return PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
				}
				return true;
			}
			return false;
		}
	}

	public override void OnApplyTemplate()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Expected O, but got Unknown
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Expected O, but got Unknown
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Expected O, but got Unknown
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Expected O, but got Unknown
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Expected O, but got Unknown
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Expected O, but got Unknown
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Expected O, but got Unknown
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Expected O, but got Unknown
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Expected O, but got Unknown
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Expected O, but got Unknown
		((FrameworkElement)this).OnApplyTemplate();
		m_appliedTemplate = false;
		UnhookEventsAndClearFields();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("TogglePaneButton");
		Button val = (Button)(object)((templateChild is Button) ? templateChild : null);
		if (val != null)
		{
			m_paneToggleButton = val;
			((ButtonBase)val).Click += new RoutedEventHandler(OnPaneToggleButtonClick);
			SetPaneToggleButtonAutomationName();
			WindowChrome.SetIsHitTestVisibleInChrome((IInputElement)(object)val, true);
		}
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("PaneHeaderContentBorder");
		m_leftNavPaneHeaderContentBorder = (ContentControl)(object)((templateChild2 is ContentControl) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("PaneCustomContentBorder");
		m_leftNavPaneCustomContentBorder = (ContentControl)(object)((templateChild3 is ContentControl) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("FooterContentBorder");
		m_leftNavFooterContentBorder = (ContentControl)(object)((templateChild4 is ContentControl) ? templateChild4 : null);
		DependencyObject templateChild5 = ((FrameworkElement)this).GetTemplateChild("PaneHeaderOnTopPane");
		m_paneHeaderOnTopPane = (ContentControl)(object)((templateChild5 is ContentControl) ? templateChild5 : null);
		DependencyObject templateChild6 = ((FrameworkElement)this).GetTemplateChild("PaneTitleOnTopPane");
		m_paneTitleOnTopPane = (ContentControl)(object)((templateChild6 is ContentControl) ? templateChild6 : null);
		DependencyObject templateChild7 = ((FrameworkElement)this).GetTemplateChild("PaneCustomContentOnTopPane");
		m_paneCustomContentOnTopPane = (ContentControl)(object)((templateChild7 is ContentControl) ? templateChild7 : null);
		DependencyObject templateChild8 = ((FrameworkElement)this).GetTemplateChild("PaneFooterOnTopPane");
		m_paneFooterOnTopPane = (ContentControl)(object)((templateChild8 is ContentControl) ? templateChild8 : null);
		if (((FrameworkElement)this).GetTemplateChild("RootSplitView") is SplitView splitView)
		{
			m_rootSplitView = splitView;
			splitView.IsPaneOpenChanged += OnSplitViewClosedCompactChanged;
			splitView.DisplayModeChanged += OnSplitViewClosedCompactChanged;
			if (SharedHelpers.IsRS3OrHigher())
			{
				splitView.PaneClosed += OnSplitViewPaneClosed;
				splitView.PaneClosing += OnSplitViewPaneClosing;
				splitView.PaneOpened += OnSplitViewPaneOpened;
				splitView.PaneOpening += OnSplitViewPaneOpening;
			}
			UpdateIsClosedCompact();
		}
		DependencyObject templateChild9 = ((FrameworkElement)this).GetTemplateChild("TopNavGrid");
		m_topNavGrid = (Grid)(object)((templateChild9 is Grid) ? templateChild9 : null);
		if (((FrameworkElement)this).GetTemplateChild("MenuItemsHost") is ItemsRepeater itemsRepeater)
		{
			m_leftNavRepeater = itemsRepeater;
			if (itemsRepeater.Layout is StackLayout stackLayout)
			{
				stackLayout.DisableVirtualization = true;
			}
			itemsRepeater.ElementPrepared += OnRepeaterElementPrepared;
			itemsRepeater.ElementClearing += OnRepeaterElementClearing;
			((UIElement)itemsRepeater).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_leftNavRepeaterGettingFocusHelper = new GettingFocusHelper((UIElement)(object)itemsRepeater);
			m_leftNavRepeaterGettingFocusHelper.GettingFocus += OnRepeaterGettingFocus;
			itemsRepeater.ItemTemplate = m_navigationViewItemsFactory;
		}
		if (((FrameworkElement)this).GetTemplateChild("TopNavMenuItemsHost") is ItemsRepeater itemsRepeater2)
		{
			m_topNavRepeater = itemsRepeater2;
			if (itemsRepeater2.Layout is StackLayout stackLayout2)
			{
				stackLayout2.DisableVirtualization = true;
			}
			itemsRepeater2.ElementPrepared += OnRepeaterElementPrepared;
			itemsRepeater2.ElementClearing += OnRepeaterElementClearing;
			((UIElement)itemsRepeater2).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_topNavRepeaterGettingFocusHelper = new GettingFocusHelper((UIElement)(object)itemsRepeater2);
			m_topNavRepeaterGettingFocusHelper.GettingFocus += OnRepeaterGettingFocus;
			itemsRepeater2.ItemTemplate = m_navigationViewItemsFactory;
		}
		if (((FrameworkElement)this).GetTemplateChild("TopNavMenuItemsOverflowHost") is ItemsRepeater itemsRepeater3)
		{
			m_topNavRepeaterOverflowView = itemsRepeater3;
			if (itemsRepeater3.Layout is StackLayout stackLayout3)
			{
				stackLayout3.DisableVirtualization = true;
			}
			itemsRepeater3.ElementPrepared += OnRepeaterElementPrepared;
			itemsRepeater3.ElementClearing += OnRepeaterElementClearing;
			itemsRepeater3.ItemTemplate = m_navigationViewItemsFactory;
		}
		DependencyObject templateChild10 = ((FrameworkElement)this).GetTemplateChild("TopNavOverflowButton");
		Button val2 = (Button)(object)((templateChild10 is Button) ? templateChild10 : null);
		if (val2 != null)
		{
			m_topNavOverflowButton = val2;
			AutomationProperties.SetName((DependencyObject)(object)val2, ResourceAccessor.GetLocalizedStringResource("NavigationOverflowButtonName"));
			((ContentControl)val2).Content = ResourceAccessor.GetLocalizedStringResource("NavigationOverflowButtonText");
			if (ToolTipService.GetToolTip((DependencyObject)(object)val2) == null)
			{
				ToolTip val3 = new ToolTip();
				((ContentControl)val3).Content = ResourceAccessor.GetLocalizedStringResource("NavigationOverflowButtonToolTip");
				ToolTipService.SetToolTip((DependencyObject)(object)val2, (object)val3);
			}
			FlyoutBase flyout = FlyoutService.GetFlyout(val2);
			if (flyout != null)
			{
				flyout.Closing += OnFlyoutClosing;
				flyout.Offset = 0.0;
			}
		}
		ItemsRepeater templateChildT = CppWinRTHelpers.GetTemplateChildT<ItemsRepeater>("FooterMenuItemsHost", (IControlProtected)this);
		if (templateChildT != null)
		{
			m_leftNavFooterMenuRepeater = templateChildT;
			if (templateChildT.Layout is StackLayout stackLayout4)
			{
				stackLayout4.DisableVirtualization = true;
			}
			templateChildT.ElementPrepared += OnRepeaterElementPrepared;
			templateChildT.ElementClearing += OnRepeaterElementClearing;
			((UIElement)templateChildT).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_leftNavFooterMenuRepeaterGettingFocusHelper = new GettingFocusHelper((UIElement)(object)templateChildT);
			m_leftNavFooterMenuRepeaterGettingFocusHelper.GettingFocus += OnRepeaterGettingFocus;
			templateChildT.ItemTemplate = m_navigationViewItemsFactory;
		}
		ItemsRepeater templateChildT2 = CppWinRTHelpers.GetTemplateChildT<ItemsRepeater>("TopFooterMenuItemsHost", (IControlProtected)this);
		if (templateChildT2 != null)
		{
			m_topNavFooterMenuRepeater = templateChildT2;
			if (templateChildT2.Layout is StackLayout stackLayout5)
			{
				stackLayout5.DisableVirtualization = true;
			}
			templateChildT2.ElementPrepared += OnRepeaterElementPrepared;
			templateChildT2.ElementClearing += OnRepeaterElementClearing;
			((UIElement)templateChildT2).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnRepeaterIsVisibleChanged);
			m_topNavFooterMenuRepeaterGettingFocusHelper = new GettingFocusHelper((UIElement)(object)templateChildT2);
			m_topNavFooterMenuRepeaterGettingFocusHelper.GettingFocus += OnRepeaterGettingFocus;
			templateChildT2.ItemTemplate = m_navigationViewItemsFactory;
		}
		DependencyObject templateChild11 = ((FrameworkElement)this).GetTemplateChild("TopNavContentOverlayAreaGrid");
		m_topNavContentOverlayAreaGrid = (Border)(object)((templateChild11 is Border) ? templateChild11 : null);
		DependencyObject templateChild12 = ((FrameworkElement)this).GetTemplateChild("PaneAutoSuggestBoxPresenter");
		m_leftNavPaneAutoSuggestBoxPresenter = (ContentControl)(object)((templateChild12 is ContentControl) ? templateChild12 : null);
		DependencyObject templateChild13 = ((FrameworkElement)this).GetTemplateChild("TopPaneAutoSuggestBoxPresenter");
		m_topNavPaneAutoSuggestBoxPresenter = (ContentControl)(object)((templateChild13 is ContentControl) ? templateChild13 : null);
		DependencyObject templateChild14 = ((FrameworkElement)this).GetTemplateChild("PaneContentGrid");
		m_paneContentGrid = (UIElement)(object)((templateChild14 is UIElement) ? templateChild14 : null);
		DependencyObject templateChild15 = ((FrameworkElement)this).GetTemplateChild("ContentLeftPadding");
		m_contentLeftPadding = (FrameworkElement)(object)((templateChild15 is FrameworkElement) ? templateChild15 : null);
		DependencyObject templateChild16 = ((FrameworkElement)this).GetTemplateChild("PaneHeaderCloseButtonColumn");
		m_paneHeaderCloseButtonColumn = (ColumnDefinition)(object)((templateChild16 is ColumnDefinition) ? templateChild16 : null);
		DependencyObject templateChild17 = ((FrameworkElement)this).GetTemplateChild("PaneHeaderToggleButtonColumn");
		m_paneHeaderToggleButtonColumn = (ColumnDefinition)(object)((templateChild17 is ColumnDefinition) ? templateChild17 : null);
		DependencyObject templateChild18 = ((FrameworkElement)this).GetTemplateChild("PaneHeaderContentBorderRow");
		m_paneHeaderContentBorderRow = (RowDefinition)(object)((templateChild18 is RowDefinition) ? templateChild18 : null);
		DependencyObject templateChild19 = ((FrameworkElement)this).GetTemplateChild("PaneTitleTextBlock");
		m_paneTitleFrameworkElement = (FrameworkElement)(object)((templateChild19 is FrameworkElement) ? templateChild19 : null);
		DependencyObject templateChild20 = ((FrameworkElement)this).GetTemplateChild("PaneTitlePresenter");
		m_paneTitlePresenter = (ContentControl)(object)((templateChild20 is ContentControl) ? templateChild20 : null);
		DependencyObject templateChild21 = ((FrameworkElement)this).GetTemplateChild("PaneTitleHolder");
		FrameworkElement val4 = (FrameworkElement)(object)((templateChild21 is FrameworkElement) ? templateChild21 : null);
		if (val4 != null)
		{
			m_paneTitleHolderFrameworkElement = val4;
			val4.SizeChanged += new SizeChangedEventHandler(OnPaneTitleHolderSizeChanged);
		}
		DependencyObject templateChild22 = ((FrameworkElement)this).GetTemplateChild("PaneAutoSuggestButton");
		Button val5 = (Button)(object)((templateChild22 is Button) ? templateChild22 : null);
		if (val5 != null)
		{
			m_paneSearchButton = val5;
			((ButtonBase)val5).Click += new RoutedEventHandler(OnPaneSearchButtonClick);
			string localizedStringResource = ResourceAccessor.GetLocalizedStringResource("NavigationViewSearchButtonName");
			AutomationProperties.SetName((DependencyObject)(object)val5, localizedStringResource);
			ToolTip val6 = new ToolTip();
			((ContentControl)val6).Content = localizedStringResource;
			ToolTipService.SetToolTip((DependencyObject)(object)val5, (object)val6);
		}
		DependencyObject templateChild23 = ((FrameworkElement)this).GetTemplateChild("NavigationViewBackButton");
		Button val7 = (Button)(object)((templateChild23 is Button) ? templateChild23 : null);
		if (val7 != null)
		{
			m_backButton = val7;
			((ButtonBase)val7).Click += new RoutedEventHandler(OnBackButtonClicked);
			string localizedStringResource2 = ResourceAccessor.GetLocalizedStringResource("NavigationBackButtonName");
			AutomationProperties.SetName((DependencyObject)(object)val7, localizedStringResource2);
			WindowChrome.SetIsHitTestVisibleInChrome((IInputElement)(object)val7, true);
		}
		CoreApplicationViewTitleBar titleBar = CoreApplicationViewTitleBar.GetTitleBar((DependencyObject)(object)this);
		if (titleBar != null)
		{
			m_coreTitleBar = titleBar;
			titleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
			titleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;
			if (ShouldPreserveNavigationViewRS4Behavior())
			{
				DependencyObject templateChild24 = ((FrameworkElement)this).GetTemplateChild("TogglePaneTopPadding");
				m_togglePaneTopPadding = (FrameworkElement)(object)((templateChild24 is FrameworkElement) ? templateChild24 : null);
				DependencyObject templateChild25 = ((FrameworkElement)this).GetTemplateChild("ContentPaneTopPadding");
				m_contentPaneTopPadding = (FrameworkElement)(object)((templateChild25 is FrameworkElement) ? templateChild25 : null);
			}
		}
		DependencyObject templateChild26 = ((FrameworkElement)this).GetTemplateChild("NavigationViewBackButtonToolTip");
		ToolTip val8 = (ToolTip)(object)((templateChild26 is ToolTip) ? templateChild26 : null);
		if (val8 != null)
		{
			string localizedStringResource3 = ResourceAccessor.GetLocalizedStringResource("NavigationBackButtonToolTip");
			((ContentControl)val8).Content = localizedStringResource3;
		}
		DependencyObject templateChild27 = ((FrameworkElement)this).GetTemplateChild("NavigationViewCloseButton");
		Button val9 = (Button)(object)((templateChild27 is Button) ? templateChild27 : null);
		if (val9 != null)
		{
			m_closeButton = val9;
			((ButtonBase)val9).Click += new RoutedEventHandler(OnPaneToggleButtonClick);
			string localizedStringResource4 = ResourceAccessor.GetLocalizedStringResource("NavigationCloseButtonName");
			AutomationProperties.SetName((DependencyObject)(object)val9, localizedStringResource4);
			WindowChrome.SetIsHitTestVisibleInChrome((IInputElement)(object)val9, true);
		}
		DependencyObject templateChild28 = ((FrameworkElement)this).GetTemplateChild("NavigationViewCloseButtonToolTip");
		ToolTip val10 = (ToolTip)(object)((templateChild28 is ToolTip) ? templateChild28 : null);
		if (val10 != null)
		{
			string localizedStringResource5 = ResourceAccessor.GetLocalizedStringResource("NavigationButtonOpenName");
			((ContentControl)val10).Content = localizedStringResource5;
		}
		m_itemsContainerRow = CppWinRTHelpers.GetTemplateChildT<RowDefinition>("ItemsContainerRow", (IControlProtected)this);
		m_menuItemsScrollViewer = CppWinRTHelpers.GetTemplateChildT<FrameworkElement>("MenuItemsScrollViewer", (IControlProtected)this);
		m_footerItemsScrollViewer = CppWinRTHelpers.GetTemplateChildT<FrameworkElement>("FooterItemsScrollViewer", (IControlProtected)this);
		m_visualItemsSeparator = CppWinRTHelpers.GetTemplateChildT<FrameworkElement>("VisualItemsSeparator", (IControlProtected)this);
		m_itemsContainerSizeChangedRevoker?.Revoke();
		FrameworkElement templateChildT3 = CppWinRTHelpers.GetTemplateChildT<FrameworkElement>("ItemsContainerGrid", (IControlProtected)this);
		if (templateChildT3 != null)
		{
			m_itemsContainerSizeChangedRevoker = new FrameworkElementSizeChangedRevoker(templateChildT3, new SizeChangedEventHandler(OnItemsContainerSizeChanged));
		}
		if (SharedHelpers.IsRS2OrHigher())
		{
			Grid templateChildT4 = CppWinRTHelpers.GetTemplateChildT<Grid>("RootGrid", (IControlProtected)this);
			if (templateChildT4 != null)
			{
				KeyboardNavigation.SetDirectionalNavigation((DependencyObject)(object)templateChildT4, (KeyboardNavigationMode)4);
			}
			Grid templateChildT5 = CppWinRTHelpers.GetTemplateChildT<Grid>("ContentGrid", (IControlProtected)this);
			if (templateChildT5 != null)
			{
				KeyboardNavigation.SetDirectionalNavigation((DependencyObject)(object)templateChildT5, (KeyboardNavigationMode)3);
			}
		}
		UpdatePaneShadow();
		m_appliedTemplate = true;
		UpdatePaneDisplayMode();
		UpdateHeaderVisibility();
		UpdatePaneTitleFrameworkElementParents();
		UpdateTitleBarPadding();
		UpdatePaneTabFocusNavigation();
		UpdateBackAndCloseButtonsVisibility();
		UpdateSingleSelectionFollowsFocusTemplateSetting();
		UpdatePaneVisibility();
		UpdateVisualState();
		UpdatePaneTitleMargins();
		UpdatePaneLayout();
	}

	private void UpdateRepeaterItemsSource(bool forceSelectionModelUpdate)
	{
		object obj = init();
		if (forceSelectionModelUpdate)
		{
			m_selectionModelSource[0] = obj;
		}
		m_menuItemsCollectionChangedRevoker?.Revoke();
		m_menuItemsSource = new InspectingDataSource(obj);
		m_menuItemsCollectionChangedRevoker = new ItemsSourceView.CollectionChangedRevoker(m_menuItemsSource, OnMenuItemsSourceCollectionChanged);
		if (IsTopNavigationView())
		{
			UpdateLeftRepeaterItemSource(null);
			UpdateTopNavRepeatersItemSource(obj);
			InvalidateTopNavPrimaryLayout();
		}
		else
		{
			UpdateTopNavRepeatersItemSource(null);
			UpdateLeftRepeaterItemSource(obj);
		}
		object init()
		{
			object menuItemsSource = MenuItemsSource;
			if (menuItemsSource != null)
			{
				return menuItemsSource;
			}
			UpdateSelectionForMenuItems();
			return MenuItems;
		}
	}

	private void UpdateLeftRepeaterItemSource(object items)
	{
		UpdateItemsRepeaterItemsSource(m_leftNavRepeater, items);
		UpdatePaneLayout();
	}

	private void UpdateTopNavRepeatersItemSource(object items)
	{
		m_topDataProvider.SetDataSource(items);
		UpdateTopNavPrimaryRepeaterItemsSource(items);
		UpdateTopNavOverflowRepeaterItemsSource(items);
	}

	private void UpdateTopNavPrimaryRepeaterItemsSource(object items)
	{
		if (items != null)
		{
			UpdateItemsRepeaterItemsSource(m_topNavRepeater, m_topDataProvider.GetPrimaryItems());
		}
		else
		{
			UpdateItemsRepeaterItemsSource(m_topNavRepeater, null);
		}
	}

	private void UpdateTopNavOverflowRepeaterItemsSource(object items)
	{
		m_topNavOverflowItemsCollectionChangedRevoker?.Revoke();
		ItemsRepeater topNavRepeaterOverflowView = m_topNavRepeaterOverflowView;
		if (topNavRepeaterOverflowView != null)
		{
			if (items != null)
			{
				IList overflowItems = m_topDataProvider.GetOverflowItems();
				topNavRepeaterOverflowView.ItemsSource = overflowItems;
				m_topNavOverflowItemsCollectionChangedRevoker = new ItemsSourceView.CollectionChangedRevoker(topNavRepeaterOverflowView.ItemsSourceView, OnOverflowItemsSourceCollectionChanged);
			}
			else
			{
				topNavRepeaterOverflowView.ItemsSource = null;
			}
		}
	}

	private void UpdateItemsRepeaterItemsSource(ItemsRepeater ir, object itemsSource)
	{
		if (ir != null)
		{
			ir.ItemsSource = itemsSource;
		}
	}

	private void UpdateFooterRepeaterItemsSource(bool sourceCollectionReset, bool sourceCollectionChanged)
	{
		if (!m_appliedTemplate)
		{
			return;
		}
		object source = init();
		UpdateItemsRepeaterItemsSource(m_leftNavFooterMenuRepeater, null);
		UpdateItemsRepeaterItemsSource(m_topNavFooterMenuRepeater, null);
		if (m_settingsItem == null || sourceCollectionChanged || sourceCollectionReset)
		{
			List<object> list = new List<object>();
			if (m_settingsItem == null)
			{
				m_settingsItem = new NavigationViewItem();
				NavigationViewItem settingsItem = m_settingsItem;
				((FrameworkElement)settingsItem).Name = "SettingsItem";
				m_navigationViewItemsFactory.SettingsItem(settingsItem);
			}
			if (sourceCollectionReset)
			{
				if (m_footerItemsSource != null)
				{
					m_footerItemsSource.CollectionChanged -= OnFooterItemsSourceCollectionChanged;
				}
				m_footerItemsSource = null;
			}
			if (m_footerItemsSource == null)
			{
				m_footerItemsSource = new InspectingDataSource(source);
				m_footerItemsCollectionChangedRevoker = new ItemsSourceView.CollectionChangedRevoker(m_footerItemsSource, OnFooterItemsSourceCollectionChanged);
			}
			if (m_footerItemsSource != null)
			{
				NavigationViewItem settingsItem2 = m_settingsItem;
				int count = m_footerItemsSource.Count;
				for (int i = 0; i < count; i++)
				{
					object at = m_footerItemsSource.GetAt(i);
					list.Add(at);
				}
				if (IsSettingsVisible)
				{
					CreateAndHookEventsToSettings();
					list.Add(settingsItem2);
				}
			}
			m_selectionModelSource[1] = list;
		}
		if (IsTopNavigationView())
		{
			UpdateItemsRepeaterItemsSource(m_topNavFooterMenuRepeater, m_selectionModelSource[1]);
			return;
		}
		ItemsRepeater leftNavFooterMenuRepeater = m_leftNavFooterMenuRepeater;
		if (leftNavFooterMenuRepeater != null)
		{
			UpdateItemsRepeaterItemsSource(m_leftNavFooterMenuRepeater, m_selectionModelSource[1]);
			((UIElement)leftNavFooterMenuRepeater).InvalidateMeasure();
			((UIElement)leftNavFooterMenuRepeater).UpdateLayout();
			UpdatePaneLayout();
		}
		NavigationViewItem settingsItem3 = m_settingsItem;
		if (settingsItem3 != null)
		{
			((FrameworkElement)settingsItem3).BringIntoView();
		}
		object init()
		{
			object footerMenuItemsSource = FooterMenuItemsSource;
			if (footerMenuItemsSource != null)
			{
				return footerMenuItemsSource;
			}
			UpdateSelectionForMenuItems();
			return FooterMenuItems;
		}
	}

	private void OnFlyoutClosing(object sender, FlyoutBaseClosingEventArgs args)
	{
		if (!m_moveTopNavOverflowItemOnFlyoutClose || m_selectionChangeFromOverflowMenu)
		{
			return;
		}
		m_moveTopNavOverflowItemOnFlyoutClose = false;
		IndexPath selectedIndex = m_selectionModel.SelectedIndex;
		if (selectedIndex.GetSize() > 0)
		{
			UIElement containerForIndex = GetContainerForIndex(selectedIndex.GetAt(1), inFooter: false);
			if (containerForIndex != null && containerForIndex is NavigationViewItem navigationViewItem)
			{
				navigationViewItem.IsExpanded = false;
			}
			SelectandMoveOverflowItem(SelectedItem, selectedIndex, closeFlyout: false);
		}
	}

	private void OnNavigationViewItemIsSelectedPropertyChanged(DependencyObject sender, DependencyProperty args)
	{
		if (!(sender is NavigationViewItem navigationViewItem))
		{
			return;
		}
		bool flag = IsContainerTheSelectedItemInTheSelectionModel(navigationViewItem);
		bool isSelected = navigationViewItem.IsSelected;
		if (isSelected && !flag)
		{
			IndexPath indexPathForContainer = GetIndexPathForContainer(navigationViewItem);
			UpdateSelectionModelSelection(indexPathForContainer);
		}
		else if (!isSelected && flag)
		{
			IndexPath indexPathForContainer2 = GetIndexPathForContainer(navigationViewItem);
			IndexPath selectedIndex = m_selectionModel.SelectedIndex;
			if (selectedIndex != null && indexPathForContainer2.CompareTo(selectedIndex) == 0)
			{
				m_selectionModel.DeselectAt(indexPathForContainer2);
			}
		}
		if (isSelected)
		{
			navigationViewItem.IsChildSelected = false;
		}
	}

	private void OnNavigationViewItemExpandedPropertyChanged(DependencyObject sender, DependencyProperty args)
	{
		if (sender is NavigationViewItem navigationViewItem)
		{
			if (navigationViewItem.IsExpanded)
			{
				RaiseExpandingEvent(navigationViewItem);
			}
			ShowHideChildrenItemsRepeater(navigationViewItem);
			if (!navigationViewItem.IsExpanded)
			{
				RaiseCollapsedEvent(navigationViewItem);
			}
		}
	}

	private void RaiseItemInvokedForNavigationViewItem(NavigationViewItem nvi)
	{
		object item = null;
		object prevItem = SelectedItem;
		ItemsRepeater parentIR = GetParentItemsRepeaterForContainer(nvi);
		ItemsSourceView itemsSourceView = parentIR.ItemsSourceView;
		if (itemsSourceView != null)
		{
			InspectingDataSource inspectingDataSource = (InspectingDataSource)itemsSourceView;
			int elementIndex = parentIR.GetElementIndex((UIElement)(object)nvi);
			if (elementIndex != -1)
			{
				item = inspectingDataSource.GetAt(elementIndex);
			}
		}
		NavigationRecommendedTransitionDirection recommendedDirection = init();
		RaiseItemInvoked(item, IsSettingsItem(nvi), nvi, recommendedDirection);
		NavigationRecommendedTransitionDirection init()
		{
			if (IsTopNavigationView() && nvi.SelectsOnInvoked)
			{
				if (parentIR == m_topNavRepeaterOverflowView)
				{
					return NavigationRecommendedTransitionDirection.FromOverflow;
				}
				if (prevItem != null)
				{
					return GetRecommendedTransitionDirection((DependencyObject)(object)NavigationViewItemBaseOrSettingsContentFromData(prevItem), (DependencyObject)(object)nvi);
				}
			}
			return NavigationRecommendedTransitionDirection.Default;
		}
	}

	internal void OnNavigationViewItemInvoked(NavigationViewItem nvi)
	{
		m_shouldRaiseItemInvokedAfterSelection = true;
		object selectedItem = SelectedItem;
		bool flag = m_selectionModel != null && nvi.SelectsOnInvoked;
		if (flag)
		{
			IndexPath indexPathForContainer = GetIndexPathForContainer(nvi);
			if (DoesNavigationViewItemHaveChildren(nvi))
			{
				m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = true;
			}
			UpdateSelectionModelSelection(indexPathForContainer);
		}
		if (selectedItem == SelectedItem)
		{
			RaiseItemInvokedForNavigationViewItem(nvi);
		}
		ToggleIsExpandedNavigationViewItem(nvi);
		ClosePaneIfNeccessaryAfterItemIsClicked(nvi);
		if (flag)
		{
			CloseFlyoutIfRequired(nvi);
		}
	}

	private bool IsRootItemsRepeater(DependencyObject element)
	{
		if (element != null)
		{
			if ((object)element != m_topNavRepeater && (object)element != m_leftNavRepeater && (object)element != m_topNavRepeaterOverflowView && (object)element != m_leftNavFooterMenuRepeater)
			{
				return (object)element == m_topNavFooterMenuRepeater;
			}
			return true;
		}
		return false;
	}

	private bool IsRootGridOfFlyout(DependencyObject element)
	{
		Grid val = (Grid)(object)((element is Grid) ? element : null);
		if (val != null)
		{
			return ((FrameworkElement)val).Name == "FlyoutRootGrid";
		}
		return false;
	}

	private ItemsRepeater GetParentRootItemsRepeaterForContainer(NavigationViewItemBase nvib)
	{
		ItemsRepeater parentItemsRepeaterForContainer = GetParentItemsRepeaterForContainer(nvib);
		NavigationViewItemBase navigationViewItemBase = nvib;
		while (!IsRootItemsRepeater((DependencyObject)(object)parentItemsRepeaterForContainer))
		{
			navigationViewItemBase = GetParentNavigationViewItemForContainer(navigationViewItemBase);
			if (navigationViewItemBase == null)
			{
				return null;
			}
			parentItemsRepeaterForContainer = GetParentItemsRepeaterForContainer(navigationViewItemBase);
		}
		return parentItemsRepeaterForContainer;
	}

	internal ItemsRepeater GetParentItemsRepeaterForContainer(NavigationViewItemBase nvib)
	{
		DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)nvib);
		if (parent != null && parent is ItemsRepeater result)
		{
			return result;
		}
		return null;
	}

	private NavigationViewItem GetParentNavigationViewItemForContainer(NavigationViewItemBase nvib)
	{
		DependencyObject val = (DependencyObject)(object)GetParentItemsRepeaterForContainer(nvib);
		if (!IsRootItemsRepeater(val))
		{
			while (val != null)
			{
				val = VisualTreeHelper.GetParent(val);
				if (val is NavigationViewItem result)
				{
					return result;
				}
			}
		}
		return null;
	}

	private IndexPath GetIndexPathForContainer(NavigationViewItemBase nvib)
	{
		List<int> list = new List<int>();
		bool flag = false;
		DependencyObject val = (DependencyObject)(object)nvib;
		DependencyObject val2 = VisualTreeHelper.GetParent(val);
		if (val2 == null)
		{
			return IndexPath.CreateFromIndices(list);
		}
		while (val2 != null && !IsRootItemsRepeater(val2) && !IsRootGridOfFlyout(val2))
		{
			if (val2 is ItemsRepeater itemsRepeater)
			{
				UIElement val3 = (UIElement)(object)((val is UIElement) ? val : null);
				if (val3 != null)
				{
					list.Insert(0, itemsRepeater.GetElementIndex(val3));
				}
			}
			val = val2;
			val2 = VisualTreeHelper.GetParent(val2);
		}
		if (IsRootGridOfFlyout(val2))
		{
			NavigationViewItem lastItemExpandedIntoFlyout = m_lastItemExpandedIntoFlyout;
			if (lastItemExpandedIntoFlyout != null)
			{
				val = (DependencyObject)(object)lastItemExpandedIntoFlyout;
				val2 = (DependencyObject)(object)(IsTopNavigationView() ? m_topNavRepeater : m_leftNavRepeater);
			}
		}
		if ((object)val2 == m_topNavRepeaterOverflowView)
		{
			int elementIndex = m_topNavRepeaterOverflowView.GetElementIndex((UIElement)(object)((val is UIElement) ? val : null));
			object value = m_topDataProvider.GetOverflowItems()[elementIndex];
			int item = m_topDataProvider.IndexOf(value);
			list.Insert(0, item);
		}
		else if ((object)val2 == m_topNavRepeater)
		{
			int elementIndex2 = m_topNavRepeater.GetElementIndex((UIElement)(object)((val is UIElement) ? val : null));
			object value2 = m_topDataProvider.GetPrimaryItems()[elementIndex2];
			int item2 = m_topDataProvider.IndexOf(value2);
			list.Insert(0, item2);
		}
		else if (val2 is ItemsRepeater itemsRepeater2)
		{
			list.Insert(0, itemsRepeater2.GetElementIndex((UIElement)(object)((val is UIElement) ? val : null)));
		}
		flag = (object)val2 == m_leftNavFooterMenuRepeater || (object)val2 == m_topNavFooterMenuRepeater;
		list.Insert(0, flag ? 1 : 0);
		return IndexPath.CreateFromIndices(list);
	}

	internal void OnRepeaterElementPrepared(ItemsRepeater ir, ItemsRepeaterElementPreparedEventArgs args)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		NavigationViewItemBase nvibImpl;
		NavigationViewRepeaterPosition position;
		if (args.Element is NavigationViewItemBase navigationViewItemBase)
		{
			nvibImpl = navigationViewItemBase;
			nvibImpl.SetNavigationViewParent(this);
			nvibImpl.IsTopLevelItem = IsTopLevelItem(navigationViewItemBase);
			position = init();
			nvibImpl.Position = position;
			NavigationViewItem parentNavigationViewItemForContainer = GetParentNavigationViewItemForContainer(navigationViewItemBase);
			if (parentNavigationViewItemForContainer != null)
			{
				NavigationViewItem navigationViewItem = parentNavigationViewItemForContainer;
				int depth = ((!navigationViewItem.ShouldRepeaterShowInFlyout()) ? (navigationViewItem.Depth + 1) : 0);
				nvibImpl.Depth = depth;
			}
			else
			{
				nvibImpl.Depth = 0;
			}
			ApplyCustomMenuItemContainerStyling(navigationViewItemBase, ir, args.Index);
			if (args.Element is NavigationViewItem navigationViewItem2)
			{
				int depth2 = init2();
				navigationViewItem2.PropagateDepthToChildren(depth2);
				InputHelper.AddTappedHandler((UIElement)(object)navigationViewItem2, OnNavigationViewItemTapped);
				((UIElement)navigationViewItem2).KeyDown += new KeyEventHandler(OnNavigationViewItemKeyDown);
				((UIElement)navigationViewItem2).GotFocus += new RoutedEventHandler(OnNavigationViewItemOnGotFocus);
				navigationViewItem2.IsSelectedChanged += OnNavigationViewItemIsSelectedPropertyChanged;
				navigationViewItem2.IsExpandedChanged += OnNavigationViewItemExpandedPropertyChanged;
			}
		}
		NavigationViewRepeaterPosition init()
		{
			if (IsTopNavigationView())
			{
				if (ir == m_topNavRepeater)
				{
					return NavigationViewRepeaterPosition.TopPrimary;
				}
				if (ir == m_topNavFooterMenuRepeater)
				{
					return NavigationViewRepeaterPosition.TopFooter;
				}
				return NavigationViewRepeaterPosition.TopOverflow;
			}
			if (ir == m_leftNavFooterMenuRepeater)
			{
				return NavigationViewRepeaterPosition.LeftFooter;
			}
			return NavigationViewRepeaterPosition.LeftNav;
		}
		int init2()
		{
			if (position == NavigationViewRepeaterPosition.TopPrimary)
			{
				return 0;
			}
			return nvibImpl.Depth + 1;
		}
	}

	private void ApplyCustomMenuItemContainerStyling(NavigationViewItemBase nvib, ItemsRepeater ir, int index)
	{
		Style menuItemContainerStyle = MenuItemContainerStyle;
		if (menuItemContainerStyle != null)
		{
			((FrameworkElement)nvib).Style = menuItemContainerStyle;
			return;
		}
		StyleSelector menuItemContainerStyleSelector = MenuItemContainerStyleSelector;
		if (menuItemContainerStyleSelector == null)
		{
			return;
		}
		ItemsSourceView itemsSourceView = ir.ItemsSourceView;
		if (itemsSourceView == null)
		{
			return;
		}
		object at = itemsSourceView.GetAt(index);
		if (at != null)
		{
			Style val = menuItemContainerStyleSelector.SelectStyle(at, (DependencyObject)(object)nvib);
			if (val != null)
			{
				((FrameworkElement)nvib).Style = val;
			}
		}
	}

	internal void OnRepeaterElementClearing(ItemsRepeater ir, ItemsRepeaterElementClearingEventArgs args)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		if (args.Element is NavigationViewItemBase navigationViewItemBase)
		{
			navigationViewItemBase.Depth = 0;
			navigationViewItemBase.IsTopLevelItem = false;
			if (navigationViewItemBase is NavigationViewItem navigationViewItem)
			{
				InputHelper.RemoveTappedHandler((UIElement)(object)navigationViewItem, OnNavigationViewItemTapped);
				((UIElement)navigationViewItem).KeyDown -= new KeyEventHandler(OnNavigationViewItemKeyDown);
				((UIElement)navigationViewItem).GotFocus -= new RoutedEventHandler(OnNavigationViewItemOnGotFocus);
				navigationViewItem.IsSelectedChanged -= OnNavigationViewItemIsSelectedPropertyChanged;
				navigationViewItem.IsExpandedChanged -= OnNavigationViewItemExpandedPropertyChanged;
			}
		}
	}

	internal NavigationViewItemsFactory GetNavigationViewItemsFactory()
	{
		return m_navigationViewItemsFactory;
	}

	private void CreateAndHookEventsToSettings()
	{
		if (m_settingsItem != null)
		{
			NavigationViewItem settingsItem = m_settingsItem;
			SymbolIcon icon = new SymbolIcon(Symbol.Setting);
			settingsItem.Icon = icon;
			string localizedStringResource = ResourceAccessor.GetLocalizedStringResource("SettingsButtonName");
			AutomationProperties.SetName((DependencyObject)(object)settingsItem, localizedStringResource);
			((FrameworkElement)settingsItem).Tag = localizedStringResource;
			UpdateSettingsItemToolTip();
			if (!IsTopNavigationView())
			{
				((ContentControl)settingsItem).Content = localizedStringResource;
			}
			else
			{
				((ContentControl)settingsItem).Content = null;
			}
			((DependencyObject)this).SetValue(SettingsItemPropertyKey, (object)settingsItem);
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (IsTopNavigationView() && IsTopPrimaryListVisible())
		{
			if (double.IsInfinity(((Size)(ref availableSize)).Width))
			{
				m_topDataProvider.MoveAllItemsToPrimaryList();
			}
			else
			{
				HandleTopNavigationMeasureOverride(availableSize);
			}
		}
		((UIElement)this).LayoutUpdated -= OnLayoutUpdated;
		((UIElement)this).LayoutUpdated += OnLayoutUpdated;
		m_layoutUpdatedToken = true;
		return ((Control)this).MeasureOverride(availableSize);
	}

	private void OnLayoutUpdated(object sender, object e)
	{
		((UIElement)this).LayoutUpdated -= OnLayoutUpdated;
		m_layoutUpdatedToken = false;
		object lastSelectedItemInTopNav = m_lastSelectedItemPendingAnimationInTopNav;
		if (lastSelectedItemInTopNav != null)
		{
			m_lastSelectedItemPendingAnimationInTopNav = null;
			((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
			{
				AnimateSelectionChanged(lastSelectedItemInTopNav);
			}, (DispatcherPriority)10);
		}
		if (m_OrientationChangedPendingAnimation)
		{
			m_OrientationChangedPendingAnimation = false;
			AnimateSelectionChanged(SelectedItem);
		}
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Size newSize = args.NewSize;
		double width = ((Size)(ref newSize)).Width;
		UpdateAdaptiveLayout(width);
		UpdateTitleBarPadding();
		UpdateBackAndCloseButtonsVisibility();
		UpdatePaneLayout();
	}

	private void OnItemsContainerSizeChanged(object sender, SizeChangedEventArgs e)
	{
		UpdatePaneLayout();
	}

	private void UpdateAdaptiveLayout(double width, bool forceSetDisplayMode = false)
	{
		if (IsTopNavigationView() || m_rootSplitView == null)
		{
			return;
		}
		m_initialListSizeStateSet = false;
		NavigationViewDisplayMode navigationViewDisplayMode = NavigationViewDisplayMode.Compact;
		switch (PaneDisplayMode)
		{
		case NavigationViewPaneDisplayMode.Auto:
			if (width >= ExpandedModeThresholdWidth)
			{
				navigationViewDisplayMode = NavigationViewDisplayMode.Expanded;
			}
			else if (width < CompactModeThresholdWidth)
			{
				navigationViewDisplayMode = NavigationViewDisplayMode.Minimal;
			}
			break;
		case NavigationViewPaneDisplayMode.Left:
			navigationViewDisplayMode = NavigationViewDisplayMode.Expanded;
			break;
		case NavigationViewPaneDisplayMode.LeftCompact:
			navigationViewDisplayMode = NavigationViewDisplayMode.Compact;
			break;
		case NavigationViewPaneDisplayMode.LeftMinimal:
			navigationViewDisplayMode = NavigationViewDisplayMode.Minimal;
			break;
		default:
			Environment.FailFast(null);
			break;
		}
		if (!forceSetDisplayMode && m_InitialNonForcedModeUpdate)
		{
			if (navigationViewDisplayMode == NavigationViewDisplayMode.Minimal || navigationViewDisplayMode == NavigationViewDisplayMode.Compact)
			{
				ClosePane();
			}
			m_InitialNonForcedModeUpdate = false;
		}
		NavigationViewDisplayMode displayMode = DisplayMode;
		SetDisplayMode(navigationViewDisplayMode, forceSetDisplayMode);
		if (navigationViewDisplayMode == NavigationViewDisplayMode.Expanded && IsPaneVisible && !m_wasForceClosed)
		{
			OpenPane();
		}
		if (displayMode == NavigationViewDisplayMode.Expanded && navigationViewDisplayMode == NavigationViewDisplayMode.Compact)
		{
			m_initialListSizeStateSet = false;
			ClosePane();
		}
	}

	private void UpdatePaneLayout()
	{
		if (IsTopNavigationView())
		{
			return;
		}
		double totalAvailableHeight = init();
		double totalAvailableHeightHalf;
		if (totalAvailableHeight > 0.0)
		{
			totalAvailableHeightHalf = totalAvailableHeight / 2.0;
			double maxHeight = init2();
			FrameworkElement menuItemsScrollViewer = m_menuItemsScrollViewer;
			if (menuItemsScrollViewer != null)
			{
				menuItemsScrollViewer.MaxHeight = maxHeight;
			}
		}
		double init()
		{
			RowDefinition itemsContainerRow = m_itemsContainerRow;
			if (itemsContainerRow != null)
			{
				ContentControl leftNavFooterContentBorder = m_leftNavFooterContentBorder;
				if (leftNavFooterContentBorder != null)
				{
					return itemsContainerRow.ActualHeight - 29.0 - ((FrameworkElement)leftNavFooterContentBorder).ActualHeight;
				}
				return itemsContainerRow.ActualHeight - 29.0;
			}
			return 0.0;
		}
		double init2()
		{
			FrameworkElement footerItemsScrollViewer = m_footerItemsScrollViewer;
			if (footerItemsScrollViewer != null)
			{
				ItemsRepeater leftNavFooterMenuRepeater = m_leftNavFooterMenuRepeater;
				if (leftNavFooterMenuRepeater != null)
				{
					ItemsRepeater leftNavRepeater = m_leftNavRepeater;
					if (leftNavRepeater != null)
					{
						double actualHeight = ((FrameworkElement)leftNavFooterMenuRepeater).ActualHeight;
						double actualHeight2 = ((FrameworkElement)leftNavRepeater).ActualHeight;
						if (totalAvailableHeight > actualHeight2 + actualHeight)
						{
							footerItemsScrollViewer.MaxHeight = actualHeight;
							FrameworkElement visualItemsSeparator = m_visualItemsSeparator;
							if (visualItemsSeparator != null)
							{
								((UIElement)visualItemsSeparator).Visibility = (Visibility)2;
							}
							return totalAvailableHeight - actualHeight;
						}
						if (actualHeight2 <= totalAvailableHeightHalf)
						{
							footerItemsScrollViewer.MaxHeight = totalAvailableHeight - actualHeight2;
							FrameworkElement visualItemsSeparator2 = m_visualItemsSeparator;
							if (visualItemsSeparator2 != null)
							{
								((UIElement)visualItemsSeparator2).Visibility = (Visibility)0;
							}
							return actualHeight2;
						}
						if (actualHeight <= totalAvailableHeightHalf)
						{
							footerItemsScrollViewer.MaxHeight = actualHeight;
							FrameworkElement visualItemsSeparator3 = m_visualItemsSeparator;
							if (visualItemsSeparator3 != null)
							{
								((UIElement)visualItemsSeparator3).Visibility = (Visibility)0;
							}
							return totalAvailableHeight - actualHeight;
						}
						footerItemsScrollViewer.MaxHeight = totalAvailableHeightHalf;
						FrameworkElement visualItemsSeparator4 = m_visualItemsSeparator;
						if (visualItemsSeparator4 != null)
						{
							((UIElement)visualItemsSeparator4).Visibility = (Visibility)0;
						}
						return totalAvailableHeightHalf;
					}
					return totalAvailableHeight - ((FrameworkElement)leftNavFooterMenuRepeater).ActualHeight;
				}
				footerItemsScrollViewer.MaxHeight = totalAvailableHeightHalf;
			}
			return totalAvailableHeightHalf;
		}
	}

	private void OnPaneToggleButtonClick(object sender, RoutedEventArgs args)
	{
		if (IsPaneOpen)
		{
			m_wasForceClosed = true;
			ClosePane();
		}
		else
		{
			m_wasForceClosed = false;
			OpenPane();
		}
	}

	private void OnPaneSearchButtonClick(object sender, RoutedEventArgs args)
	{
		m_wasForceClosed = false;
		OpenPane();
		AutoSuggestBox autoSuggestBox = AutoSuggestBox;
		if (autoSuggestBox != null)
		{
			((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
			{
				((UIElement)autoSuggestBox).Focus();
			}, (DispatcherPriority)6);
		}
	}

	private void OnPaneTitleHolderSizeChanged(object sender, SizeChangedEventArgs args)
	{
		UpdateBackAndCloseButtonsVisibility();
	}

	private void OpenPane()
	{
		try
		{
			m_isOpenPaneForInteraction = true;
			IsPaneOpen = true;
		}
		finally
		{
			m_isOpenPaneForInteraction = false;
		}
	}

	private void ClosePane()
	{
		CollapseMenuItemsInRepeater(m_leftNavRepeater);
		try
		{
			m_isOpenPaneForInteraction = true;
			IsPaneOpen = false;
		}
		finally
		{
			m_isOpenPaneForInteraction = false;
		}
	}

	private bool AttemptClosePaneLightly()
	{
		NavigationViewPaneClosingEventArgs e = new NavigationViewPaneClosingEventArgs();
		this.PaneClosing?.Invoke(this, e);
		if (!e.Cancel || m_wasForceClosed)
		{
			m_blockNextClosingEvent = true;
			ClosePane();
			return true;
		}
		return false;
	}

	private void OnSplitViewClosedCompactChanged(DependencyObject sender, DependencyProperty args)
	{
		if (args == SplitView.IsPaneOpenProperty || args == SplitView.DisplayModeProperty)
		{
			UpdateIsClosedCompact();
		}
	}

	private void OnSplitViewPaneClosed(DependencyObject sender, object obj)
	{
		this.PaneClosed?.Invoke(this, null);
	}

	private void OnSplitViewPaneClosing(DependencyObject sender, SplitViewPaneClosingEventArgs args)
	{
		bool flag = false;
		if (this.PaneClosing != null)
		{
			if (!m_blockNextClosingEvent)
			{
				NavigationViewPaneClosingEventArgs e = new NavigationViewPaneClosingEventArgs();
				e.SplitViewClosingArgs(args);
				this.PaneClosing(this, e);
				flag = e.Cancel;
			}
			else
			{
				m_blockNextClosingEvent = false;
			}
		}
		if (!flag)
		{
			SplitView rootSplitView = m_rootSplitView;
			if (rootSplitView != null && m_leftNavRepeater != null && (rootSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || rootSplitView.DisplayMode == SplitViewDisplayMode.CompactInline))
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "ListSizeCompact", true);
				UpdatePaneToggleSize();
			}
		}
	}

	private void OnSplitViewPaneOpened(DependencyObject sender, object obj)
	{
		this.PaneOpened?.Invoke(this, null);
	}

	private void OnSplitViewPaneOpening(DependencyObject sender, object obj)
	{
		if (m_leftNavRepeater != null)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "ListSizeFull", true);
		}
		this.PaneOpening?.Invoke(this, null);
	}

	private void UpdateIsClosedCompact()
	{
		SplitView rootSplitView = m_rootSplitView;
		if (rootSplitView != null)
		{
			SplitViewDisplayMode displayMode = rootSplitView.DisplayMode;
			m_isClosedCompact = !rootSplitView.IsPaneOpen && (displayMode == SplitViewDisplayMode.CompactOverlay || displayMode == SplitViewDisplayMode.CompactInline);
			VisualStateManager.GoToState((FrameworkElement)(object)this, m_isClosedCompact ? "ClosedCompact" : "NotClosedCompact", true);
			if (!m_initialListSizeStateSet)
			{
				m_initialListSizeStateSet = true;
				VisualStateManager.GoToState((FrameworkElement)(object)this, m_isClosedCompact ? "ListSizeCompact" : "ListSizeFull", true);
			}
			UpdateTitleBarPadding();
			UpdateBackAndCloseButtonsVisibility();
			UpdatePaneTitleMargins();
			UpdatePaneToggleSize();
		}
	}

	private void UpdatePaneButtonsWidths()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		double num = init();
		Button backButton = m_backButton;
		if (backButton != null)
		{
			((FrameworkElement)backButton).Width = num;
		}
		Button paneToggleButton = m_paneToggleButton;
		if (paneToggleButton != null)
		{
			((FrameworkElement)paneToggleButton).MinWidth = num;
			ColumnDefinition templateChild = ((Control)(object)paneToggleButton).GetTemplateChild<ColumnDefinition>("PaneToggleButtonIconWidthColumn");
			if (templateChild != null)
			{
				templateChild.Width = new GridLength(num);
			}
		}
		double init()
		{
			if (DisplayMode == NavigationViewDisplayMode.Minimal)
			{
				return 40.0;
			}
			return CompactPaneLength;
		}
	}

	private void OnBackButtonClicked(object sender, RoutedEventArgs args)
	{
		NavigationViewBackRequestedEventArgs args2 = new NavigationViewBackRequestedEventArgs();
		this.BackRequested?.Invoke(this, args2);
	}

	private bool IsOverlay()
	{
		SplitView rootSplitView = m_rootSplitView;
		if (rootSplitView != null)
		{
			return rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay;
		}
		return false;
	}

	private bool IsLightDismissible()
	{
		SplitView rootSplitView = m_rootSplitView;
		if (rootSplitView != null)
		{
			if (rootSplitView.DisplayMode != SplitViewDisplayMode.Inline)
			{
				return rootSplitView.DisplayMode != SplitViewDisplayMode.CompactInline;
			}
			return false;
		}
		return false;
	}

	private bool ShouldShowBackButton()
	{
		if (m_backButton != null && !ShouldPreserveNavigationViewRS3Behavior())
		{
			if (DisplayMode == NavigationViewDisplayMode.Minimal && IsPaneOpen)
			{
				return false;
			}
			return ShouldShowBackOrCloseButton();
		}
		return false;
	}

	private bool ShouldShowCloseButton()
	{
		if (m_backButton != null && !ShouldPreserveNavigationViewRS3Behavior() && m_closeButton != null)
		{
			if (!IsPaneOpen)
			{
				return false;
			}
			NavigationViewPaneDisplayMode paneDisplayMode = PaneDisplayMode;
			if (paneDisplayMode != NavigationViewPaneDisplayMode.LeftMinimal && (paneDisplayMode != NavigationViewPaneDisplayMode.Auto || DisplayMode != NavigationViewDisplayMode.Minimal))
			{
				return false;
			}
			return ShouldShowBackOrCloseButton();
		}
		return false;
	}

	private bool ShouldShowBackOrCloseButton()
	{
		return IsBackButtonVisible switch
		{
			NavigationViewBackButtonVisible.Auto => !SharedHelpers.IsOnXbox(), 
			NavigationViewBackButtonVisible.Visible => true, 
			_ => false, 
		};
	}

	private void SetPaneToggleButtonAutomationName()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		string text = ((!IsPaneOpen) ? ResourceAccessor.GetLocalizedStringResource("NavigationButtonClosedName") : ResourceAccessor.GetLocalizedStringResource("NavigationButtonOpenName"));
		Button paneToggleButton = m_paneToggleButton;
		if (paneToggleButton != null)
		{
			AutomationProperties.SetName((DependencyObject)(object)paneToggleButton, text);
			ToolTip val = new ToolTip();
			((ContentControl)val).Content = text;
			ToolTipService.SetToolTip((DependencyObject)(object)paneToggleButton, (object)val);
		}
	}

	private void UpdateSettingsItemToolTip()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		NavigationViewItem settingsItem = m_settingsItem;
		if (settingsItem != null)
		{
			if (!IsTopNavigationView() && IsPaneOpen)
			{
				ToolTipService.SetToolTip((DependencyObject)(object)settingsItem, (object)null);
				return;
			}
			string localizedStringResource = ResourceAccessor.GetLocalizedStringResource("SettingsButtonName");
			ToolTip val = new ToolTip();
			((ContentControl)val).Content = localizedStringResource;
			ToolTipService.SetToolTip((DependencyObject)(object)settingsItem, (object)val);
		}
	}

	private void UpdatePaneTitleFrameworkElementParents()
	{
		FrameworkElement paneTitleHolderFrameworkElement = m_paneTitleHolderFrameworkElement;
		if (paneTitleHolderFrameworkElement != null)
		{
			bool isPaneToggleButtonVisible = IsPaneToggleButtonVisible;
			bool flag = IsTopNavigationView();
			((UIElement)paneTitleHolderFrameworkElement).Visibility = (Visibility)((isPaneToggleButtonVisible || flag || PaneTitle.Length == 0 || (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !IsPaneOpen)) ? 2 : 0);
			FrameworkElement paneTitleFrameworkElement = m_paneTitleFrameworkElement;
			if (paneTitleFrameworkElement != null)
			{
				Action action = SetPaneTitleFrameworkElementParent((ContentControl)(object)m_paneToggleButton, paneTitleFrameworkElement, flag || !isPaneToggleButtonVisible);
				Action action2 = SetPaneTitleFrameworkElementParent(m_paneTitlePresenter, paneTitleFrameworkElement, flag || isPaneToggleButtonVisible);
				Action action3 = SetPaneTitleFrameworkElementParent(m_paneTitleOnTopPane, paneTitleFrameworkElement, !flag || isPaneToggleButtonVisible);
				Action action4 = action;
				(action4 ?? action2 ?? action3)?.Invoke();
			}
		}
	}

	private Action SetPaneTitleFrameworkElementParent(ContentControl parent, FrameworkElement paneTitle, bool shouldNotContainPaneTitle)
	{
		if (parent != null && parent.Content == paneTitle == shouldNotContainPaneTitle)
		{
			if (!shouldNotContainPaneTitle)
			{
				return delegate
				{
					parent.Content = paneTitle;
				};
			}
			parent.Content = null;
		}
		return null;
	}

	private void AnimateSelectionChangedToItem(object selectedItem)
	{
		if (selectedItem != null && !IsSelectionSuppressed(selectedItem))
		{
			AnimateSelectionChanged(selectedItem);
		}
	}

	private void AnimateSelectionChanged(object nextItem)
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		if (m_lastSelectedItemPendingAnimationInTopNav != null)
		{
			return;
		}
		UIElement activeIndicator = m_activeIndicator;
		UIElement val = FindSelectionIndicator(nextItem);
		bool flag = false;
		if (m_prevIndicator != null || m_nextIndicator != null)
		{
			if (val != null && m_nextIndicator == val)
			{
				if (activeIndicator != null && activeIndicator != m_prevIndicator)
				{
					ResetElementAnimationProperties(activeIndicator, 0.0);
				}
				flag = true;
			}
			else
			{
				OnAnimationComplete(null, null);
			}
		}
		if (flag)
		{
			return;
		}
		UIElement paneContentGrid = m_paneContentGrid;
		if (activeIndicator != val && paneContentGrid != null && activeIndicator != null && val != null && SharedHelpers.IsAnimationsEnabled)
		{
			ResetElementAnimationProperties(activeIndicator, 1.0);
			ResetElementAnimationProperties(val, 1.0);
			Point val2 = default(Point);
			((Point)(ref val2))._002Ector(0.0, 0.0);
			Point val3 = ((Visual)(object)activeIndicator).SafeTransformToVisual((Visual)(object)paneContentGrid).Transform(val2);
			Point val4 = ((Visual)(object)val).SafeTransformToVisual((Visual)(object)paneContentGrid).Transform(val2);
			Size renderSize = activeIndicator.RenderSize;
			Size renderSize2 = val.RenderSize;
			bool flag2 = false;
			double num;
			double num2;
			if (IsTopNavigationView())
			{
				num = ((Point)(ref val3)).X;
				num2 = ((Point)(ref val4)).X;
				flag2 = ((Point)(ref val3)).Y == ((Point)(ref val4)).Y;
			}
			else
			{
				num = ((Point)(ref val3)).Y;
				num2 = ((Point)(ref val4)).Y;
				flag2 = ((Point)(ref val3)).X == ((Point)(ref val4)).X;
			}
			Storyboard storyboard = new Storyboard
			{
				FillBehavior = (FillBehavior)1
			};
			if (!flag2)
			{
				bool flag3 = ((Point)(ref val3)).Y < ((Point)(ref val4)).Y;
				Size renderSize3 = activeIndicator.RenderSize;
				double height = ((Size)(ref renderSize3)).Height;
				renderSize3 = activeIndicator.RenderSize;
				if (height > ((Size)(ref renderSize3)).Width)
				{
					PlayIndicatorNonSameLevelAnimations(activeIndicator, isOutgoing: true, !flag3, ((TimelineGroup)storyboard).Children);
				}
				else
				{
					PlayIndicatorNonSameLevelTopPrimaryAnimation(activeIndicator, isOutgoing: true, ((TimelineGroup)storyboard).Children);
				}
				renderSize3 = val.RenderSize;
				double height2 = ((Size)(ref renderSize3)).Height;
				renderSize3 = val.RenderSize;
				if (height2 > ((Size)(ref renderSize3)).Width)
				{
					PlayIndicatorNonSameLevelAnimations(val, isOutgoing: false, flag3 ? true : false, ((TimelineGroup)storyboard).Children);
				}
				else
				{
					PlayIndicatorNonSameLevelTopPrimaryAnimation(val, isOutgoing: false, ((TimelineGroup)storyboard).Children);
				}
			}
			else
			{
				double to = num2 - num;
				double num3 = num - num2;
				PlayIndicatorAnimations(activeIndicator, 0.0, to, renderSize, renderSize2, isOutgoing: true, ((TimelineGroup)storyboard).Children);
				PlayIndicatorAnimations(val, num3, 0.0, renderSize, renderSize2, isOutgoing: false, ((TimelineGroup)storyboard).Children);
			}
			m_prevIndicator = activeIndicator;
			m_nextIndicator = val;
			((Timeline)storyboard).Completed += OnAnimationComplete;
			storyboard.Begin((FrameworkElement)(object)this, true);
			storyboard.Pause((FrameworkElement)(object)this);
			storyboard.SeekAlignedToLastTick((FrameworkElement)(object)this, TimeSpan.Zero, (TimeSeekOrigin)0);
			((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
			{
				storyboard.Resume((FrameworkElement)(object)this);
			}, (DispatcherPriority)6);
		}
		else
		{
			ResetElementAnimationProperties(activeIndicator, 0.0);
			ResetElementAnimationProperties(val, 1.0);
		}
		m_activeIndicator = val;
	}

	private void PlayIndicatorNonSameLevelAnimations(UIElement indicator, bool isOutgoing, bool fromTop, TimelineCollection animations)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		double num = (isOutgoing ? 1.0 : 0.0);
		double num2 = (isOutgoing ? 0.0 : 1.0);
		DoubleAnimationUsingKeyFrames val = new DoubleAnimationUsingKeyFrames();
		val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(num, KeyTime.FromPercent(0.0)));
		val.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(num2, KeyTime.FromPercent(1.0), new KeySpline(new Point(0.8, 0.0), c_frame2point2)));
		((Timeline)val).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(600.0));
		DoubleAnimationUsingKeyFrames val2 = val;
		animations.Add((Timeline)(object)val2);
		Size renderSize = indicator.RenderSize;
		double num3 = (IsTopNavigationView() ? ((Size)(ref renderSize)).Width : ((Size)(ref renderSize)).Height);
		double y = (fromTop ? 0.0 : num3);
		Point value = default(Point);
		((Point)(ref value)).Y = y;
		Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)indicator);
		Storyboard.SetTargetProperty((DependencyObject)(object)val2, s_scaleYPath);
		PrepareIndicatorForAnimation(indicator, value);
	}

	private void PlayIndicatorNonSameLevelTopPrimaryAnimation(UIElement indicator, bool isOutgoing, TimelineCollection animations)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		double num = (isOutgoing ? 1.0 : 0.0);
		double num2 = (isOutgoing ? 0.0 : 1.0);
		DoubleAnimationUsingKeyFrames val = new DoubleAnimationUsingKeyFrames();
		val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(num, KeyTime.FromPercent(0.0)));
		val.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(num2, KeyTime.FromPercent(1.0), new KeySpline(new Point(0.8, 0.0), c_frame2point2)));
		((Timeline)val).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(600.0));
		DoubleAnimationUsingKeyFrames val2 = val;
		animations.Add((Timeline)(object)val2);
		Size renderSize = indicator.RenderSize;
		double y = ((Size)(ref renderSize)).Width / 2.0;
		Point value = default(Point);
		((Point)(ref value)).Y = y;
		Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)indicator);
		Storyboard.SetTargetProperty((DependencyObject)(object)val2, s_scaleXPath);
		PrepareIndicatorForAnimation(indicator, value);
	}

	private void PlayIndicatorAnimations(UIElement indicator, double from, double to, Size beginSize, Size endSize, bool isOutgoing, TimelineCollection animations)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Expected O, but got Unknown
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		Size renderSize = indicator.RenderSize;
		double num = (IsTopNavigationView() ? ((Size)(ref renderSize)).Width : ((Size)(ref renderSize)).Height);
		double num2 = 1.0;
		double num3 = 1.0;
		if (IsTopNavigationView() && Math.Abs(((Size)(ref renderSize)).Width) > 0.0010000000474974513)
		{
			num2 = ((Size)(ref beginSize)).Width / ((Size)(ref renderSize)).Width;
			num3 = ((Size)(ref endSize)).Width / ((Size)(ref renderSize)).Width;
		}
		DoubleAnimationUsingKeyFrames val = new DoubleAnimationUsingKeyFrames();
		val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame((from < to) ? from : (from + num * (num2 - 1.0)), KeyTime.FromPercent(0.0)));
		val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame((from < to) ? (to + num * (num3 - 1.0)) : to, KeyTime.FromPercent(0.333)));
		((Timeline)val).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(600.0));
		DoubleAnimationUsingKeyFrames val2 = val;
		Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)indicator);
		animations.Add((Timeline)(object)val2);
		DoubleAnimationUsingKeyFrames val3 = new DoubleAnimationUsingKeyFrames();
		val3.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(num2, KeyTime.FromPercent(0.0)));
		val3.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(Math.Abs(to - from) / num + ((from < to) ? num3 : num2), KeyTime.FromPercent(0.333), new KeySpline(c_frame1point1, c_frame1point2)));
		val3.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(num3, KeyTime.FromPercent(1.0), new KeySpline(c_frame2point1, c_frame2point2)));
		((Timeline)val3).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(600.0));
		DoubleAnimationUsingKeyFrames val4 = val3;
		Storyboard.SetTarget((DependencyObject)(object)val4, (DependencyObject)(object)indicator);
		animations.Add((Timeline)(object)val4);
		DoubleAnimationUsingKeyFrames val5 = new DoubleAnimationUsingKeyFrames();
		val5.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame((from < to) ? 0.0 : num, KeyTime.FromPercent(0.0)));
		val5.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame((from < to) ? num : 0.0, KeyTime.FromPercent(1.0)));
		((Timeline)val5).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(200.0));
		DoubleAnimationUsingKeyFrames val6 = val5;
		Storyboard.SetTarget((DependencyObject)(object)val6, (DependencyObject)(object)indicator);
		animations.Add((Timeline)(object)val6);
		if (isOutgoing)
		{
			DoubleAnimationUsingKeyFrames val7 = new DoubleAnimationUsingKeyFrames();
			val7.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.FromPercent(0.0)));
			val7.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.FromPercent(0.333)));
			val7.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.FromPercent(1.0), new KeySpline(c_frame2point1, c_frame2point2)));
			((Timeline)val7).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(600.0));
			DoubleAnimationUsingKeyFrames val8 = val7;
			Storyboard.SetTarget((DependencyObject)(object)val8, (DependencyObject)(object)indicator);
			Storyboard.SetTargetProperty((DependencyObject)(object)val8, s_opacityPath);
			animations.Add((Timeline)(object)val8);
		}
		if (IsTopNavigationView())
		{
			Storyboard.SetTargetProperty((DependencyObject)(object)val2, s_translateXPath);
			Storyboard.SetTargetProperty((DependencyObject)(object)val4, s_scaleXPath);
			Storyboard.SetTargetProperty((DependencyObject)(object)val6, s_centerXPath);
		}
		else
		{
			Storyboard.SetTargetProperty((DependencyObject)(object)val2, s_translateYPath);
			Storyboard.SetTargetProperty((DependencyObject)(object)val4, s_scaleYPath);
			Storyboard.SetTargetProperty((DependencyObject)(object)val6, s_centerYPath);
		}
		PrepareIndicatorForAnimation(indicator);
	}

	private void PrepareIndicatorForAnimation(UIElement indicator, Point? centerPoint = null)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006e: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Transform renderTransform = indicator.RenderTransform;
		TransformGroup val = (TransformGroup)(object)((renderTransform is TransformGroup) ? renderTransform : null);
		if (val == null || val.Children.Count != 2 || !(val.Children[0] is ScaleTransform) || !(val.Children[1] is TranslateTransform))
		{
			TransformGroup val2 = new TransformGroup();
			val2.Children.Add((Transform)new ScaleTransform());
			val2.Children.Add((Transform)new TranslateTransform());
			indicator.RenderTransform = (Transform)val2;
		}
		if (centerPoint.HasValue)
		{
			ScaleTransform val3 = (ScaleTransform)((TransformGroup)indicator.RenderTransform).Children[0];
			Point value = centerPoint.Value;
			val3.CenterX = ((Point)(ref value)).X;
			value = centerPoint.Value;
			val3.CenterY = ((Point)(ref value)).Y;
		}
		if (indicator.CacheMode == null)
		{
			indicator.CacheMode = (CacheMode)(object)m_bitmapCache;
		}
	}

	private void OnAnimationComplete(object sender, EventArgs args)
	{
		UIElement prevIndicator = m_prevIndicator;
		ResetElementAnimationProperties(prevIndicator, 0.0);
		m_prevIndicator = null;
		prevIndicator = m_nextIndicator;
		ResetElementAnimationProperties(prevIndicator, 1.0);
		m_nextIndicator = null;
	}

	private void ResetElementAnimationProperties(UIElement element, double desiredOpacity)
	{
		if (element == null)
		{
			return;
		}
		element.Opacity = desiredOpacity;
		Transform renderTransform = element.RenderTransform;
		TransformGroup val = (TransformGroup)(object)((renderTransform is TransformGroup) ? renderTransform : null);
		if (val != null && val.Children.Count == 2)
		{
			Transform obj = val.Children[0];
			ScaleTransform val2 = (ScaleTransform)(object)((obj is ScaleTransform) ? obj : null);
			if (val2 != null)
			{
				Transform obj2 = val.Children[1];
				TranslateTransform val3 = (TranslateTransform)(object)((obj2 is TranslateTransform) ? obj2 : null);
				if (val3 != null)
				{
					((Animatable)val3).BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline)null);
					((Animatable)val3).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)null);
					((Animatable)val2).BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline)null);
					((Animatable)val2).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)null);
					((DependencyObject)val2).ClearValue(ScaleTransform.CenterXProperty);
					((DependencyObject)val2).ClearValue(ScaleTransform.CenterYProperty);
					goto IL_00aa;
				}
			}
		}
		((DependencyObject)element).ClearValue(UIElement.RenderTransformProperty);
		goto IL_00aa;
		IL_00aa:
		element.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)null);
	}

	private NavigationViewItemBase NavigationViewItemBaseOrSettingsContentFromData(object data)
	{
		return GetContainerForData<NavigationViewItemBase>(data);
	}

	private NavigationViewItem NavigationViewItemOrSettingsContentFromData(object data)
	{
		return GetContainerForData<NavigationViewItem>(data);
	}

	private bool IsSelectionSuppressed(object item)
	{
		if (item != null)
		{
			NavigationViewItem navigationViewItem = NavigationViewItemOrSettingsContentFromData(item);
			if (navigationViewItem != null)
			{
				return !navigationViewItem.SelectsOnInvoked;
			}
		}
		return false;
	}

	private bool ShouldPreserveNavigationViewRS4Behavior()
	{
		return m_topNavGrid == null;
	}

	private bool ShouldPreserveNavigationViewRS3Behavior()
	{
		return m_backButton == null;
	}

	private UIElement FindSelectionIndicator(object item)
	{
		if (item != null)
		{
			NavigationViewItem navigationViewItem = NavigationViewItemOrSettingsContentFromData(item);
			if (navigationViewItem != null)
			{
				UIElement selectionIndicator = navigationViewItem.GetSelectionIndicator();
				if (selectionIndicator != null)
				{
					return selectionIndicator;
				}
				((UIElement)navigationViewItem).UpdateLayout();
				return navigationViewItem.GetSelectionIndicator();
			}
		}
		return null;
	}

	private void RaiseSelectionChangedEvent(object nextItem, bool isSettingsItem, NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default)
	{
		NavigationViewSelectionChangedEventArgs e = new NavigationViewSelectionChangedEventArgs();
		e.SelectedItem = nextItem;
		e.IsSettingsSelected = isSettingsItem;
		NavigationViewItemBase navigationViewItemBase = NavigationViewItemBaseOrSettingsContentFromData(nextItem);
		if (navigationViewItemBase != null)
		{
			e.SelectedItemContainer = navigationViewItemBase;
		}
		e.RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recommendedDirection);
		this.SelectionChanged?.Invoke(this, e);
	}

	private void ChangeSelection(object prevItem, object nextItem)
	{
		bool flag = IsSettingsItem(nextItem);
		if (IsSelectionSuppressed(nextItem))
		{
			UndoSelectionAndRevertSelectionTo(prevItem, nextItem);
			RaiseItemInvoked(nextItem, flag);
			return;
		}
		NavigationRecommendedTransitionDirection recommendedDirection = init();
		object selectedItem = SelectedItem;
		if (m_shouldRaiseItemInvokedAfterSelection)
		{
			m_shouldRaiseItemInvokedAfterSelection = false;
			RaiseItemInvoked(nextItem, flag, NavigationViewItemOrSettingsContentFromData(nextItem), recommendedDirection);
		}
		if (selectedItem != SelectedItem)
		{
			return;
		}
		UnselectPrevItem(prevItem, nextItem);
		ChangeSelectStatusForItem(nextItem, selected: true);
		try
		{
			if (!m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise)
			{
				AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
				if (val != null)
				{
					((NavigationViewAutomationPeer)(object)val).RaiseSelectionChangedEvent(prevItem, nextItem);
				}
			}
		}
		finally
		{
			m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = false;
		}
		RaiseSelectionChangedEvent(nextItem, flag, recommendedDirection);
		AnimateSelectionChanged(nextItem);
		NavigationViewItem navigationViewItem = NavigationViewItemOrSettingsContentFromData(nextItem);
		if (navigationViewItem != null)
		{
			ClosePaneIfNeccessaryAfterItemIsClicked(navigationViewItem);
		}
		NavigationRecommendedTransitionDirection init()
		{
			if (IsTopNavigationView())
			{
				if (m_selectionChangeFromOverflowMenu)
				{
					return NavigationRecommendedTransitionDirection.FromOverflow;
				}
				if (prevItem != null && nextItem != null)
				{
					return GetRecommendedTransitionDirection((DependencyObject)(object)NavigationViewItemBaseOrSettingsContentFromData(prevItem), (DependencyObject)(object)NavigationViewItemBaseOrSettingsContentFromData(nextItem));
				}
			}
			return NavigationRecommendedTransitionDirection.Default;
		}
	}

	private void UpdateSelectionModelSelection(IndexPath ip)
	{
		IndexPath selectedIndex = m_selectionModel.SelectedIndex;
		m_selectionModel.SelectAt(ip);
		UpdateIsChildSelected(selectedIndex, ip);
	}

	private void UpdateIsChildSelected(IndexPath prevIP, IndexPath nextIP)
	{
		if (prevIP != null && prevIP.GetSize() > 0)
		{
			UpdateIsChildSelectedForIndexPath(prevIP, isChildSelected: false);
		}
		if (nextIP != null && nextIP.GetSize() > 0)
		{
			UpdateIsChildSelectedForIndexPath(nextIP, isChildSelected: true);
		}
	}

	private void UpdateIsChildSelectedForIndexPath(IndexPath ip, bool isChildSelected)
	{
		UIElement val = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == 1);
		int num = 2;
		while (val != null)
		{
			if (val is NavigationViewItem navigationViewItem)
			{
				navigationViewItem.IsChildSelected = isChildSelected;
				ItemsRepeater repeater = navigationViewItem.GetRepeater();
				if (repeater != null && num < ip.GetSize() - 1)
				{
					val = repeater.TryGetElement(ip.GetAt(num));
					num++;
					continue;
				}
			}
			val = null;
		}
	}

	private void RaiseItemInvoked(object item, bool isSettings, NavigationViewItemBase container = null, NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default)
	{
		object invokedItem = item;
		NavigationViewItemBase invokedItemContainer = container;
		NavigationViewItemInvokedEventArgs e = new NavigationViewItemInvokedEventArgs();
		if (container != null)
		{
			invokedItem = ((ContentControl)container).Content;
		}
		else if (!isSettings)
		{
			NavigationViewItemBase navigationViewItemBase = NavigationViewItemBaseOrSettingsContentFromData(item);
			if (navigationViewItemBase != null)
			{
				invokedItem = ((ContentControl)navigationViewItemBase).Content;
				invokedItemContainer = navigationViewItemBase;
			}
		}
		else
		{
			invokedItemContainer = item as NavigationViewItemBase;
		}
		e.InvokedItem = invokedItem;
		e.InvokedItemContainer = invokedItemContainer;
		e.IsSettingsInvoked = isSettings;
		e.RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recommendedDirection);
		this.ItemInvoked?.Invoke(this, e);
	}

	private void SetDisplayMode(NavigationViewDisplayMode displayMode, bool forceSetDisplayMode = false)
	{
		UpdateVisualStateForDisplayModeGroup(displayMode);
		if (forceSetDisplayMode || DisplayMode != displayMode)
		{
			UpdateHeaderVisibility(displayMode);
			UpdatePaneTabFocusNavigation();
			UpdatePaneToggleSize();
			RaiseDisplayModeChanged(displayMode);
		}
	}

	private NavigationViewVisualStateDisplayMode GetVisualStateDisplayMode(NavigationViewDisplayMode displayMode)
	{
		NavigationViewPaneDisplayMode paneDisplayMode = PaneDisplayMode;
		if (IsTopNavigationView())
		{
			return NavigationViewVisualStateDisplayMode.Minimal;
		}
		if (paneDisplayMode == NavigationViewPaneDisplayMode.Left || (paneDisplayMode == NavigationViewPaneDisplayMode.Auto && displayMode == NavigationViewDisplayMode.Expanded))
		{
			return NavigationViewVisualStateDisplayMode.Expanded;
		}
		if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact || (paneDisplayMode == NavigationViewPaneDisplayMode.Auto && displayMode == NavigationViewDisplayMode.Compact))
		{
			return NavigationViewVisualStateDisplayMode.Compact;
		}
		if (ShouldShowBackButton() || ShouldShowCloseButton())
		{
			return NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
		}
		return NavigationViewVisualStateDisplayMode.Minimal;
	}

	private void UpdateVisualStateForDisplayModeGroup(NavigationViewDisplayMode displayMode)
	{
		SplitView rootSplitView = m_rootSplitView;
		if (rootSplitView != null)
		{
			NavigationViewVisualStateDisplayMode visualStateDisplayMode = GetVisualStateDisplayMode(displayMode);
			string text = "";
			SplitViewDisplayMode displayMode2 = SplitViewDisplayMode.Overlay;
			string text2 = "Minimal";
			switch (visualStateDisplayMode)
			{
			case NavigationViewVisualStateDisplayMode.MinimalWithBackButton:
				text = "MinimalWithBackButton";
				displayMode2 = SplitViewDisplayMode.Overlay;
				break;
			case NavigationViewVisualStateDisplayMode.Minimal:
				text = text2;
				displayMode2 = SplitViewDisplayMode.Overlay;
				break;
			case NavigationViewVisualStateDisplayMode.Compact:
				text = "Compact";
				displayMode2 = SplitViewDisplayMode.CompactOverlay;
				break;
			case NavigationViewVisualStateDisplayMode.Expanded:
				text = "Expanded";
				displayMode2 = SplitViewDisplayMode.CompactInline;
				break;
			}
			if (!IsPaneVisible)
			{
				displayMode2 = SplitViewDisplayMode.CompactOverlay;
			}
			bool flag = false;
			if (text == text2 && IsTopNavigationView())
			{
				flag = VisualStateManager.GoToState((FrameworkElement)(object)this, "TopNavigationMinimal", false);
			}
			if (!flag)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, text, false);
			}
			rootSplitView.DisplayMode = displayMode2;
		}
	}

	private void OnNavigationViewItemTapped(object sender, TappedRoutedEventArgs args)
	{
		if (sender is NavigationViewItem navigationViewItem)
		{
			OnNavigationViewItemInvoked(navigationViewItem);
			((UIElement)navigationViewItem).Focus();
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void OnNavigationViewItemKeyDown(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		if ((((int)args.Key != 6 && (int)args.Key != 18) || !args.IsRepeat) && sender is NavigationViewItem nvi)
		{
			HandleKeyEventForNavigationViewItem(nvi, args);
		}
	}

	private void HandleKeyEventForNavigationViewItem(NavigationViewItem nvi, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected I4, but got Unknown
		Key key = args.Key;
		if ((int)key != 6)
		{
			switch (key - 18)
			{
			default:
				return;
			case 0:
				break;
			case 4:
				((RoutedEventArgs)args).Handled = true;
				KeyboardFocusFirstItemFromItem(nvi);
				return;
			case 3:
				((RoutedEventArgs)args).Handled = true;
				KeyboardFocusLastItemFromItem(nvi);
				return;
			case 8:
				FocusNextDownItem(nvi, args);
				return;
			case 6:
				FocusNextUpItem(nvi, args);
				return;
			case 7:
				FocusNextRightItem(nvi, args);
				return;
			case 1:
			case 2:
			case 5:
				return;
			}
		}
		((RoutedEventArgs)args).Handled = true;
		OnNavigationViewItemInvoked(nvi);
	}

	private void FocusNextUpItem(NavigationViewItem nvi, KeyEventArgs args)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (((RoutedEventArgs)args).OriginalSource != nvi)
		{
			return;
		}
		bool flag = true;
		if (FocusManagerEx.FindNextFocusableElement((FocusNavigationDirection)6) is NavigationViewItem navigationViewItem)
		{
			NavigationViewItem navigationViewItem2 = navigationViewItem;
			if (navigationViewItem2.Depth == nvi.Depth)
			{
				if (DoesNavigationViewItemHaveChildren(navigationViewItem))
				{
					ItemsRepeater repeater = navigationViewItem2.GetRepeater();
					if (repeater != null)
					{
						if (((UIElement)repeater).MoveFocus(new TraversalRequest((FocusNavigationDirection)3)))
						{
							((RoutedEventArgs)args).Handled = true;
						}
						else
						{
							((RoutedEventArgs)args).Handled = ((UIElement)navigationViewItem2).Focus();
						}
					}
				}
				else
				{
					flag = false;
				}
			}
		}
		if (flag && !((RoutedEventArgs)args).Handled && nvi.Depth > 0)
		{
			NavigationViewItem parentNavigationViewItemForContainer = GetParentNavigationViewItemForContainer(nvi);
			if (parentNavigationViewItemForContainer != null)
			{
				((RoutedEventArgs)args).Handled = ((UIElement)parentNavigationViewItemForContainer).Focus();
			}
		}
	}

	private void FocusNextDownItem(NavigationViewItem nvi, KeyEventArgs args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		if (((RoutedEventArgs)args).OriginalSource != nvi)
		{
			return;
		}
		if (DoesNavigationViewItemHaveChildren(nvi))
		{
			ItemsRepeater repeater = nvi.GetRepeater();
			if (repeater != null)
			{
				((RoutedEventArgs)args).Handled = ((UIElement)repeater).MoveFocus(new TraversalRequest((FocusNavigationDirection)2));
			}
		}
		if (!((RoutedEventArgs)args).Handled)
		{
			((RoutedEventArgs)args).Handled = ((UIElement)nvi).MoveFocus(new TraversalRequest((FocusNavigationDirection)7));
		}
	}

	private void FocusNextRightItem(NavigationViewItem nvi, KeyEventArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		((RoutedEventArgs)args).Handled = ((UIElement)nvi).MoveFocus(new TraversalRequest((FocusNavigationDirection)5));
	}

	private void KeyboardFocusFirstItemFromItem(NavigationViewItemBase nvib)
	{
		UIElement obj = init();
		Control val = (Control)(object)((obj is Control) ? obj : null);
		if (val != null)
		{
			((UIElement)val).Focus();
		}
		UIElement init()
		{
			return GetParentRootItemsRepeaterForContainer(nvib).TryGetElement(0);
		}
	}

	private void KeyboardFocusLastItemFromItem(NavigationViewItemBase nvib)
	{
		ItemsRepeater parentRootItemsRepeaterForContainer = GetParentRootItemsRepeaterForContainer(nvib);
		ItemsSourceView itemsSourceView = parentRootItemsRepeaterForContainer.ItemsSourceView;
		if (itemsSourceView == null)
		{
			return;
		}
		int index = itemsSourceView.Count - 1;
		UIElement val = parentRootItemsRepeaterForContainer.TryGetElement(index);
		if (val != null)
		{
			Control val2 = (Control)(object)((val is Control) ? val : null);
			if (val2 != null)
			{
				((UIElement)val2).Focus();
			}
		}
	}

	private void OnRepeaterGettingFocus(object sender, GettingFocusEventArgs args)
	{
		DependencyObject oldFocusedElement;
		ItemsRepeater newRootItemsRepeater;
		if (m_TabKeyPrecedesFocusChange && args.InputDevice == FocusInputDeviceKind.Keyboard && m_selectionModel.SelectedIndex != null)
		{
			oldFocusedElement = args.OldFocusedElement;
			if (oldFocusedElement != null)
			{
				newRootItemsRepeater = sender as ItemsRepeater;
				if (newRootItemsRepeater != null)
				{
					bool flag = init();
					object obj = init2();
					if (args != null && newRootItemsRepeater == obj && flag)
					{
						NavigationViewItemBase containerForIndexPath = GetContainerForIndexPath(m_selectionModel.SelectedIndex, lastVisible: true);
						if (((IGettingFocusEventArgs2)args).TrySetNewFocusedElement((DependencyObject)(object)containerForIndexPath))
						{
							args.Handled = true;
						}
					}
				}
			}
		}
		m_TabKeyPrecedesFocusChange = false;
		bool init()
		{
			bool result = true;
			for (DependencyObject val = oldFocusedElement; val != null; val = VisualTreeHelper.GetParent(val))
			{
				if (val is NavigationViewItemBase nvib)
				{
					result = GetParentRootItemsRepeaterForContainer(nvib) != newRootItemsRepeater;
					break;
				}
			}
			return result;
		}
		object init2()
		{
			if (IsTopNavigationView())
			{
				if (m_selectionModel.SelectedIndex.GetAt(0) != 0)
				{
					return m_topNavFooterMenuRepeater;
				}
				return m_topNavRepeater;
			}
			if (m_selectionModel.SelectedIndex.GetAt(0) != 0)
			{
				return m_leftNavFooterMenuRepeater;
			}
			return m_leftNavRepeater;
		}
	}

	private void OnNavigationViewItemOnGotFocus(object sender, RoutedEventArgs e)
	{
		if (!(sender is NavigationViewItem navigationViewItem) || !IsNavigationViewListSingleSelectionFollowsFocus() || !navigationViewItem.SelectsOnInvoked || navigationViewItem.IsSelected)
		{
			return;
		}
		if (IsTopNavigationView())
		{
			ItemsRepeater parentItemsRepeaterForContainer = GetParentItemsRepeaterForContainer(navigationViewItem);
			if (parentItemsRepeaterForContainer != null && parentItemsRepeaterForContainer != m_topNavRepeaterOverflowView)
			{
				OnNavigationViewItemInvoked(navigationViewItem);
			}
		}
		else
		{
			OnNavigationViewItemInvoked(navigationViewItem);
		}
	}

	internal void OnSettingsInvoked()
	{
		NavigationViewItem settingsItem = m_settingsItem;
		if (settingsItem != null)
		{
			OnNavigationViewItemInvoked(settingsItem);
		}
	}

	protected override void OnPreviewKeyDown(KeyEventArgs e)
	{
		m_TabKeyPrecedesFocusChange = false;
		((UIElement)this).OnPreviewKeyDown(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Key key = e.Key;
		bool handled = false;
		m_TabKeyPrecedesFocusChange = false;
		if ((int)key != 3)
		{
			if ((int)key == 23 && ((Enum)Keyboard.Modifiers).HasFlag((Enum)(object)(ModifierKeys)1) && IsPaneOpen && IsLightDismissible())
			{
				handled = AttemptClosePaneLightly();
			}
		}
		else
		{
			m_TabKeyPrecedesFocusChange = true;
		}
		((RoutedEventArgs)e).Handled = handled;
		((UIElement)this).OnKeyDown(e);
	}

	private bool SelectSelectableItemWithOffset(int startIndex, int offset, ItemsRepeater repeater, int repeaterCollectionSize)
	{
		startIndex += offset;
		while (startIndex > -1 && startIndex < repeaterCollectionSize)
		{
			if (repeater.TryGetElement(startIndex) is NavigationViewItem { SelectsOnInvoked: not false } navigationViewItem)
			{
				navigationViewItem.IsSelected = true;
				return true;
			}
			startIndex += offset;
		}
		return false;
	}

	internal object MenuItemFromContainer(DependencyObject container)
	{
		if (container != null && container is NavigationViewItemBase navigationViewItemBase)
		{
			ItemsRepeater parentItemsRepeaterForContainer = GetParentItemsRepeaterForContainer(navigationViewItemBase);
			if (parentItemsRepeaterForContainer != null)
			{
				int elementIndex = parentItemsRepeaterForContainer.GetElementIndex((UIElement)(object)navigationViewItemBase);
				if (elementIndex >= 0)
				{
					return GetItemFromIndex(parentItemsRepeaterForContainer, elementIndex);
				}
			}
		}
		return null;
	}

	internal DependencyObject ContainerFromMenuItem(object item)
	{
		if (item != null)
		{
			return (DependencyObject)(object)NavigationViewItemBaseOrSettingsContentFromData(item);
		}
		return null;
	}

	private void OnTopNavDataSourceChanged(NotifyCollectionChangedEventArgs args)
	{
		CloseTopNavigationViewFlyout();
		if (m_topNavigationMode != TopNavigationViewLayoutState.Uninitialized)
		{
			m_topDataProvider.MoveAllItemsToPrimaryList();
		}
		m_lastSelectedItemPendingAnimationInTopNav = null;
	}

	internal int GetNavigationViewItemCountInPrimaryList()
	{
		return m_topDataProvider.GetNavigationViewItemCountInPrimaryList();
	}

	internal int GetNavigationViewItemCountInTopNav()
	{
		return m_topDataProvider.GetNavigationViewItemCountInTopNav();
	}

	internal SplitView GetSplitView()
	{
		return m_rootSplitView;
	}

	internal TopNavigationViewDataProvider GetTopDataProvider()
	{
		return m_topDataProvider;
	}

	internal void TopNavigationViewItemContentChanged()
	{
		if (m_appliedTemplate)
		{
			m_topDataProvider.InvalidWidthCache();
			((UIElement)this).InvalidateMeasure();
		}
	}

	private NavigationTransitionInfo CreateNavigationTransitionInfo(NavigationRecommendedTransitionDirection recommendedTransitionDirection)
	{
		if (recommendedTransitionDirection == NavigationRecommendedTransitionDirection.FromOverflow)
		{
			recommendedTransitionDirection = NavigationRecommendedTransitionDirection.FromRight;
		}
		if ((recommendedTransitionDirection == NavigationRecommendedTransitionDirection.FromLeft || recommendedTransitionDirection == NavigationRecommendedTransitionDirection.FromRight) && SharedHelpers.IsRS5OrHigher())
		{
			SlideNavigationTransitionInfo slideNavigationTransitionInfo = new SlideNavigationTransitionInfo();
			SlideNavigationTransitionEffect effect = ((recommendedTransitionDirection != NavigationRecommendedTransitionDirection.FromRight) ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight);
			if (slideNavigationTransitionInfo != null)
			{
				slideNavigationTransitionInfo.Effect = effect;
			}
			return slideNavigationTransitionInfo;
		}
		return new EntranceNavigationTransitionInfo();
	}

	private NavigationRecommendedTransitionDirection GetRecommendedTransitionDirection(DependencyObject prev, DependencyObject next)
	{
		NavigationRecommendedTransitionDirection result = NavigationRecommendedTransitionDirection.Default;
		ItemsRepeater topNavRepeater = m_topNavRepeater;
		if (prev != null && next != null && topNavRepeater != null)
		{
			IndexPath indexPathForContainer = GetIndexPathForContainer(prev as NavigationViewItemBase);
			IndexPath indexPathForContainer2 = GetIndexPathForContainer(next as NavigationViewItemBase);
			result = indexPathForContainer.CompareTo(indexPathForContainer2) switch
			{
				-1 => NavigationRecommendedTransitionDirection.FromRight, 
				1 => NavigationRecommendedTransitionDirection.FromLeft, 
				_ => NavigationRecommendedTransitionDirection.Default, 
			};
		}
		return result;
	}

	private NavigationViewTemplateSettings GetTemplateSettings()
	{
		return TemplateSettings;
	}

	private bool IsNavigationViewListSingleSelectionFollowsFocus()
	{
		return SelectionFollowsFocus == NavigationViewSelectionFollowsFocus.Enabled;
	}

	private void UpdateSingleSelectionFollowsFocusTemplateSetting()
	{
		GetTemplateSettings().SingleSelectionFollowsFocus = IsNavigationViewListSingleSelectionFollowsFocus();
	}

	private void OnMenuItemsSourceCollectionChanged(object sender, object args)
	{
		if (!IsTopNavigationView())
		{
			ItemsRepeater leftNavRepeater = m_leftNavRepeater;
			if (leftNavRepeater != null)
			{
				((UIElement)leftNavRepeater).UpdateLayout();
			}
			UpdatePaneLayout();
		}
	}

	private void OnSelectedItemPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		object newValue = ((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		object oldValue = ((DependencyPropertyChangedEventArgs)(ref args)).OldValue;
		ChangeSelection(oldValue, newValue);
		if (m_appliedTemplate && IsTopNavigationView() && (!m_layoutUpdatedToken || (newValue != null && m_topDataProvider.IndexOf(newValue) != -1 && m_topDataProvider.IndexOf(newValue, NavigationViewSplitVectorID.PrimaryList) == -1)))
		{
			InvalidateTopNavPrimaryLayout();
		}
	}

	private void SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(object item)
	{
		SelectedItem = item;
	}

	private void ChangeSelectStatusForItem(object item, bool selected)
	{
		NavigationViewItem navigationViewItem = NavigationViewItemOrSettingsContentFromData(item);
		if (navigationViewItem != null)
		{
			navigationViewItem.IsSelected = selected;
		}
		else
		{
			if (!selected)
			{
				return;
			}
			IndexPath indexPathOfItem = GetIndexPathOfItem(item);
			if (indexPathOfItem != null && indexPathOfItem.GetSize() > 0)
			{
				try
				{
					m_shouldIgnoreNextSelectionChange = true;
					UpdateSelectionModelSelection(indexPathOfItem);
				}
				finally
				{
					m_shouldIgnoreNextSelectionChange = false;
				}
			}
		}
	}

	private bool IsSettingsItem(object item)
	{
		bool result = false;
		if (item != null)
		{
			NavigationViewItem settingsItem = m_settingsItem;
			if (settingsItem != null)
			{
				result = settingsItem == item || ((ContentControl)settingsItem).Content == item;
			}
		}
		return result;
	}

	private void UnselectPrevItem(object prevItem, object nextItem)
	{
		if (prevItem == null || prevItem == nextItem)
		{
			return;
		}
		bool flag = !m_shouldIgnoreNextSelectionChange;
		try
		{
			m_shouldIgnoreNextSelectionChange = true;
			ChangeSelectStatusForItem(prevItem, selected: false);
		}
		finally
		{
			if (flag)
			{
				m_shouldIgnoreNextSelectionChange = false;
			}
		}
	}

	private void UndoSelectionAndRevertSelectionTo(object prevSelectedItem, object nextItem)
	{
		object selectedItem = null;
		if (prevSelectedItem != null)
		{
			if (IsSelectionSuppressed(prevSelectedItem))
			{
				AnimateSelectionChanged(null);
			}
			else
			{
				ChangeSelectStatusForItem(prevSelectedItem, selected: true);
				AnimateSelectionChangedToItem(prevSelectedItem);
				selectedItem = prevSelectedItem;
			}
		}
		else
		{
			ChangeSelectStatusForItem(nextItem, selected: false);
		}
		SelectedItem = selectedItem;
	}

	private void CloseTopNavigationViewFlyout()
	{
		m_topNavOverflowButton?.Flyout()?.Hide();
	}

	private void UpdateVisualState(bool useTransitions = false)
	{
		if (m_appliedTemplate)
		{
			AutoSuggestBox autoSuggestBox = AutoSuggestBox;
			VisualStateManager.GoToState((FrameworkElement)(object)this, (autoSuggestBox != null) ? "AutoSuggestBoxVisible" : "AutoSuggestBoxCollapsed", false);
			bool isSettingsVisible = IsSettingsVisible;
			VisualStateManager.GoToState((FrameworkElement)(object)this, isSettingsVisible ? "SettingsVisible" : "SettingsCollapsed", false);
			if (IsTopNavigationView())
			{
				UpdateVisualStateForOverflowButton();
			}
			else
			{
				UpdateLeftNavigationOnlyVisualState(useTransitions);
			}
		}
	}

	private void UpdateVisualStateForOverflowButton()
	{
		string text = ((OverflowLabelMode == NavigationViewOverflowLabelMode.MoreLabel) ? "OverflowButtonWithLabel" : "OverflowButtonNoLabel");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, false);
	}

	private void UpdateLeftNavigationOnlyVisualState(bool useTransitions)
	{
		bool isPaneToggleButtonVisible = IsPaneToggleButtonVisible;
		VisualStateManager.GoToState((FrameworkElement)(object)this, isPaneToggleButtonVisible ? "TogglePaneButtonVisible" : "TogglePaneButtonCollapsed", false);
	}

	private void InvalidateTopNavPrimaryLayout()
	{
		if (m_appliedTemplate && IsTopNavigationView())
		{
			((UIElement)this).InvalidateMeasure();
		}
	}

	private double MeasureTopNavigationViewDesiredWidth(Size availableSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return LayoutUtils.MeasureAndGetDesiredWidthFor((UIElement)(object)m_topNavGrid, availableSize);
	}

	private double MeasureTopNavMenuItemsHostDesiredWidth(Size availableSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return LayoutUtils.MeasureAndGetDesiredWidthFor((UIElement)(object)m_topNavRepeater, availableSize);
	}

	private double GetTopNavigationViewActualWidth()
	{
		return LayoutUtils.GetActualWidthFor((FrameworkElement)(object)m_topNavGrid);
	}

	private bool HasTopNavigationViewItemNotInPrimaryList()
	{
		return m_topDataProvider.GetPrimaryListSize() != m_topDataProvider.Size();
	}

	private void ResetAndRearrangeTopNavItems(Size availableSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (HasTopNavigationViewItemNotInPrimaryList())
		{
			m_topDataProvider.MoveAllItemsToPrimaryList();
		}
		ArrangeTopNavItems(availableSize);
	}

	private void HandleTopNavigationMeasureOverride(Size availableSize)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (HasTopNavigationViewItemNotInPrimaryList())
		{
			HandleTopNavigationMeasureOverrideOverflow(availableSize);
		}
		else
		{
			HandleTopNavigationMeasureOverrideNormal(availableSize);
		}
		if (m_topNavigationMode == TopNavigationViewLayoutState.Uninitialized)
		{
			m_topNavigationMode = TopNavigationViewLayoutState.Initialized;
		}
	}

	private void HandleTopNavigationMeasureOverrideNormal(Size availableSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (MeasureTopNavigationViewDesiredWidth(c_infSize) > ((Size)(ref availableSize)).Width)
		{
			ResetAndRearrangeTopNavItems(availableSize);
		}
	}

	private void HandleTopNavigationMeasureOverrideOverflow(Size availableSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		double num = MeasureTopNavigationViewDesiredWidth(c_infSize);
		if (num > ((Size)(ref availableSize)).Width)
		{
			ShrinkTopNavigationSize(num, availableSize);
		}
		else if (num < ((Size)(ref availableSize)).Width)
		{
			double num2 = m_topDataProvider.WidthRequiredToRecoveryAllItemsToPrimary();
			if (((Size)(ref availableSize)).Width >= num + num2 + (double)m_topNavigationRecoveryGracePeriodWidth)
			{
				ResetAndRearrangeTopNavItems(availableSize);
				return;
			}
			List<int> indexes = FindMovableItemsRecoverToPrimaryList(((Size)(ref availableSize)).Width - num, new List<int>());
			m_topDataProvider.MoveItemsToPrimaryList(indexes);
		}
	}

	private void ArrangeTopNavItems(Size availableSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		SetOverflowButtonVisibility((Visibility)2);
		double num = MeasureTopNavigationViewDesiredWidth(c_infSize);
		if (!(num < ((Size)(ref availableSize)).Width))
		{
			SetOverflowButtonVisibility((Visibility)0);
			double num2 = MeasureTopNavigationViewDesiredWidth(c_infSize);
			m_topDataProvider.OverflowButtonWidth(num2 - num);
			ShrinkTopNavigationSize(num2, availableSize);
		}
	}

	private void SetOverflowButtonVisibility(Visibility visibility)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (visibility != TemplateSettings.OverflowButtonVisibility)
		{
			GetTemplateSettings().OverflowButtonVisibility = visibility;
		}
	}

	private void SelectOverflowItem(object item, IndexPath ip)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		object obj = init();
		int num = m_topDataProvider.IndexOf(obj);
		double widthForItem = m_topDataProvider.GetWidthForItem(num);
		bool flag = !m_topDataProvider.IsValidWidthForItem(num);
		if (!flag)
		{
			double topNavigationViewActualWidth = GetTopNavigationViewActualWidth();
			double num2 = MeasureTopNavigationViewDesiredWidth(c_infSize);
			int num3 = -1;
			object selectedItem = SelectedItem;
			if (selectedItem != null)
			{
				num3 = m_topDataProvider.IndexOf(selectedItem);
				if (num3 != -1)
				{
					m_topDataProvider.GetWidthForItem(num3);
				}
			}
			double num4 = num2 + widthForItem - topNavigationViewActualWidth;
			List<int> list = FindMovableItemsToBeRemovedFromPrimaryList(num4, new List<int>());
			double availableWidth = m_topDataProvider.CalculateWidthForItems(list) - num4;
			List<int> list2 = FindMovableItemsRecoverToPrimaryList(availableWidth, new List<int> { num });
			CollectionHelper.unique_push_back(list2, num);
			m_lastSelectedItemPendingAnimationInTopNav = obj;
			if (ip != null && ip.GetSize() > 0)
			{
				foreach (int item2 in list)
				{
					if (item2 == ip.GetAt(1))
					{
						if (m_activeIndicator != null)
						{
							AnimateSelectionChanged(null);
						}
						break;
					}
				}
			}
			if (m_topDataProvider.HasInvalidWidth(list2))
			{
				flag = true;
			}
			else
			{
				m_topDataProvider.MoveItemsToPrimaryList(list2);
				m_topDataProvider.MoveItemsOutOfPrimaryList(list);
				if (NeedRearrangeOfTopElementsAfterOverflowSelectionChanged(num))
				{
					flag = true;
				}
				if (!flag)
				{
					SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
					((UIElement)this).InvalidateMeasure();
				}
			}
		}
		if (flag)
		{
			m_topDataProvider.MoveAllItemsToPrimaryList();
			SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
			InvalidateTopNavPrimaryLayout();
		}
		object init()
		{
			if (ip.GetSize() > 2)
			{
				return GetItemFromIndex(m_topNavRepeaterOverflowView, m_topDataProvider.ConvertOriginalIndexToIndex(ip.GetAt(1)));
			}
			return item;
		}
	}

	private bool NeedRearrangeOfTopElementsAfterOverflowSelectionChanged(int selectedOriginalIndex)
	{
		bool flag = false;
		int count = m_topDataProvider.GetPrimaryItems().Count;
		int num = m_topDataProvider.ConvertOriginalIndexToIndex(selectedOriginalIndex);
		if (num < count - 1)
		{
			int num2 = num + 1;
			int num3 = selectedOriginalIndex + 1;
			int num4 = selectedOriginalIndex - 1;
			if (num > 0)
			{
				List<int> list = new List<int>();
				list.Add(num2 - 1);
				if (m_topDataProvider.ConvertPrimaryIndexToIndex(list)[0] != num4)
				{
					flag = true;
				}
			}
			while (!flag && num2 < count)
			{
				List<int> list2 = new List<int>();
				list2.Add(num2);
				List<int> list3 = m_topDataProvider.ConvertPrimaryIndexToIndex(list2);
				if (num3 != list3[0])
				{
					flag = true;
					break;
				}
				num2++;
				num3++;
			}
		}
		return flag;
	}

	private void ShrinkTopNavigationSize(double desiredWidth, Size availableSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		UpdateTopNavigationWidthCache();
		int selectedItemIndex = GetSelectedItemIndex();
		double num = MeasureTopNavMenuItemsHostDesiredWidth(c_infSize) - (desiredWidth - ((Size)(ref availableSize)).Width);
		if (num >= 0.0)
		{
			List<int> list = FindMovableItemsBeyondAvailableWidth(num);
			KeepAtLeastOneItemInPrimaryList(list, shouldKeepFirst: true);
			m_topDataProvider.MoveItemsOutOfPrimaryList(list);
		}
		desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);
		double num2 = desiredWidth - ((Size)(ref availableSize)).Width;
		if (num2 > 0.0)
		{
			List<int> list2 = FindMovableItemsToBeRemovedFromPrimaryList(num2, new List<int> { selectedItemIndex });
			KeepAtLeastOneItemInPrimaryList(list2, shouldKeepFirst: false);
			m_topDataProvider.MoveItemsOutOfPrimaryList(list2);
		}
	}

	private List<int> FindMovableItemsRecoverToPrimaryList(double availableWidth, List<int> includeItems)
	{
		List<int> list = new List<int>();
		int num = m_topDataProvider.Size();
		foreach (int includeItem in includeItems)
		{
			double widthForItem = m_topDataProvider.GetWidthForItem(includeItem);
			list.Add(includeItem);
			availableWidth -= widthForItem;
		}
		int i;
		for (i = 0; i < num; i++)
		{
			if (!(availableWidth > 0.0))
			{
				break;
			}
			if (!m_topDataProvider.IsItemInPrimaryList(i) && !CollectionHelper.contains(includeItems, i))
			{
				double widthForItem2 = m_topDataProvider.GetWidthForItem(i);
				if (!(availableWidth >= widthForItem2))
				{
					break;
				}
				list.Add(i);
				availableWidth -= widthForItem2;
			}
		}
		if (i == num && !list.Empty())
		{
			list.RemoveLast();
		}
		return list;
	}

	private List<int> FindMovableItemsToBeRemovedFromPrimaryList(double widthAtLeastToBeRemoved, List<int> excludeItems)
	{
		List<int> list = new List<int>();
		int num = m_topDataProvider.Size() - 1;
		while (num >= 0 && widthAtLeastToBeRemoved > 0.0)
		{
			if (m_topDataProvider.IsItemInPrimaryList(num) && !CollectionHelper.contains(excludeItems, num))
			{
				double widthForItem = m_topDataProvider.GetWidthForItem(num);
				list.Add(num);
				widthAtLeastToBeRemoved -= widthForItem;
			}
			num--;
		}
		return list;
	}

	private List<int> FindMovableItemsBeyondAvailableWidth(double availableWidth)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = new List<int>();
		ItemsRepeater topNavRepeater = m_topNavRepeater;
		if (topNavRepeater != null)
		{
			int num = m_topDataProvider.IndexOf(SelectedItem, NavigationViewSplitVectorID.PrimaryList);
			int primaryListSize = m_topDataProvider.GetPrimaryListSize();
			double num2 = 0.0;
			for (int i = 0; i < primaryListSize; i++)
			{
				if (i == num)
				{
					continue;
				}
				bool flag = true;
				if (num2 <= availableWidth)
				{
					UIElement val = topNavRepeater.TryGetElement(i);
					if (val != null && val != null)
					{
						UIElement val2 = val;
						Size desiredSize = val2.DesiredSize;
						double width = ((Size)(ref desiredSize)).Width;
						num2 += width;
						flag = num2 > availableWidth;
					}
				}
				if (flag)
				{
					list.Add(i);
				}
			}
		}
		return m_topDataProvider.ConvertPrimaryIndexToIndex(list);
	}

	private void KeepAtLeastOneItemInPrimaryList(List<int> itemInPrimaryToBeRemoved, bool shouldKeepFirst)
	{
		if (!itemInPrimaryToBeRemoved.Empty() && itemInPrimaryToBeRemoved.Count == m_topDataProvider.GetPrimaryListSize())
		{
			if (shouldKeepFirst)
			{
				itemInPrimaryToBeRemoved.RemoveAt(0);
			}
			else
			{
				itemInPrimaryToBeRemoved.RemoveLast();
			}
		}
	}

	private int GetSelectedItemIndex()
	{
		return m_topDataProvider.IndexOf(SelectedItem);
	}

	private double GetPaneToggleButtonWidth()
	{
		return (double)SharedHelpers.FindResource("PaneToggleButtonWidth", (FrameworkElement)(object)this, 40.0);
	}

	private double GetPaneToggleButtonHeight()
	{
		return (double)SharedHelpers.FindResource("PaneToggleButtonHeight", (FrameworkElement)(object)this, 40.0);
	}

	private void UpdateTopNavigationWidthCache()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int primaryListSize = m_topDataProvider.GetPrimaryListSize();
		ItemsRepeater topNavRepeater = m_topNavRepeater;
		if (topNavRepeater == null)
		{
			return;
		}
		for (int i = 0; i < primaryListSize; i++)
		{
			UIElement val = topNavRepeater.TryGetElement(i);
			if (val != null)
			{
				if (val != null)
				{
					UIElement val2 = val;
					Size desiredSize = val2.DesiredSize;
					double width = ((Size)(ref desiredSize)).Width;
					m_topDataProvider.UpdateWidthForPrimaryItem(i, width);
				}
				continue;
			}
			break;
		}
	}

	private bool IsTopNavigationView()
	{
		return PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
	}

	private bool IsTopPrimaryListVisible()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (m_topNavRepeater != null)
		{
			return (int)TemplateSettings.TopPaneVisibility == 0;
		}
		return false;
	}

	private void CoerceToGreaterThanZero(ref double value)
	{
		value = Math.Max(value, 0.0);
	}

	private void PropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == IsPaneOpenProperty)
		{
			OnIsPaneOpenChanged();
			UpdateVisualStateForDisplayModeGroup(DisplayMode);
		}
		else if (property == CompactModeThresholdWidthProperty || property == ExpandedModeThresholdWidthProperty)
		{
			UpdateAdaptiveLayout(((FrameworkElement)this).ActualWidth);
		}
		else if (property == AlwaysShowHeaderProperty || property == HeaderProperty)
		{
			UpdateHeaderVisibility();
		}
		else if (property == SelectedItemProperty)
		{
			OnSelectedItemPropertyChanged(args);
		}
		else if (property == PaneTitleProperty)
		{
			UpdatePaneTitleFrameworkElementParents();
			UpdateBackAndCloseButtonsVisibility();
			UpdatePaneToggleSize();
		}
		else if (property == IsBackButtonVisibleProperty)
		{
			UpdateBackAndCloseButtonsVisibility();
			UpdateAdaptiveLayout(((FrameworkElement)this).ActualWidth);
			if (IsTopNavigationView())
			{
				InvalidateTopNavPrimaryLayout();
			}
			Button backButton = m_backButton;
			if (backButton != null)
			{
				((UIElement)backButton).UpdateLayout();
			}
			UpdatePaneLayout();
		}
		else if (property == MenuItemsSourceProperty)
		{
			UpdateRepeaterItemsSource(forceSelectionModelUpdate: true);
		}
		else if (property == MenuItemsProperty)
		{
			UpdateRepeaterItemsSource(forceSelectionModelUpdate: true);
		}
		else if (property == FooterMenuItemsSourceProperty)
		{
			UpdateFooterRepeaterItemsSource(sourceCollectionReset: true, sourceCollectionChanged: true);
		}
		else if (property == FooterMenuItemsProperty)
		{
			UpdateFooterRepeaterItemsSource(sourceCollectionReset: true, sourceCollectionChanged: true);
		}
		else if (property == PaneDisplayModeProperty)
		{
			m_wasForceClosed = false;
			CollapseTopLevelMenuItems((NavigationViewPaneDisplayMode)((DependencyPropertyChangedEventArgs)(ref args)).OldValue);
			UpdatePaneToggleButtonVisibility();
			UpdatePaneDisplayMode((NavigationViewPaneDisplayMode)((DependencyPropertyChangedEventArgs)(ref args)).OldValue, (NavigationViewPaneDisplayMode)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
			UpdatePaneTitleFrameworkElementParents();
			UpdatePaneVisibility();
			UpdateVisualState();
			UpdatePaneButtonsWidths();
		}
		else if (property == IsPaneVisibleProperty)
		{
			UpdatePaneVisibility();
			UpdateVisualStateForDisplayModeGroup(DisplayMode);
			if (!IsPaneVisible && IsPaneOpen)
			{
				ClosePane();
			}
			if (IsPaneVisible && DisplayMode == NavigationViewDisplayMode.Expanded && !IsPaneOpen)
			{
				OpenPane();
			}
		}
		else if (property == OverflowLabelModeProperty)
		{
			if (m_appliedTemplate)
			{
				UpdateVisualStateForOverflowButton();
				InvalidateTopNavPrimaryLayout();
			}
		}
		else if (property == AutoSuggestBoxProperty)
		{
			InvalidateTopNavPrimaryLayout();
		}
		else if (property == SelectionFollowsFocusProperty)
		{
			UpdateSingleSelectionFollowsFocusTemplateSetting();
		}
		else if (property == IsPaneToggleButtonVisibleProperty)
		{
			UpdatePaneTitleFrameworkElementParents();
			UpdateBackAndCloseButtonsVisibility();
			UpdatePaneToggleButtonVisibility();
			UpdateVisualState();
		}
		else if (property == IsSettingsVisibleProperty)
		{
			UpdateFooterRepeaterItemsSource(sourceCollectionReset: false, sourceCollectionChanged: true);
		}
		else if (property == CompactPaneLengthProperty)
		{
			UpdatePaneShadow();
			UpdatePaneButtonsWidths();
		}
		else if (property == IsTitleBarAutoPaddingEnabledProperty)
		{
			UpdateTitleBarPadding();
		}
		else if (property == MenuItemTemplateProperty || property == MenuItemTemplateSelectorProperty)
		{
			SyncItemTemplates();
		}
	}

	private void UpdateNavigationViewItemsFactory()
	{
		object obj = MenuItemTemplate;
		if (obj == null)
		{
			obj = MenuItemTemplateSelector;
		}
		m_navigationViewItemsFactory.UserElementFactory(obj);
	}

	private void SyncItemTemplates()
	{
		UpdateNavigationViewItemsFactory();
	}

	private void OnRepeaterIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
		{
			if (((FrameworkElement)(ItemsRepeater)sender).IsLoaded)
			{
				OnRepeaterLoaded(sender, null);
			}
		}, (DispatcherPriority)6);
	}

	private void OnRepeaterLoaded(object sender, RoutedEventArgs args)
	{
		object selectedItem = SelectedItem;
		if (selectedItem == null)
		{
			return;
		}
		if (!IsSelectionSuppressed(selectedItem))
		{
			NavigationViewItem navigationViewItem = NavigationViewItemOrSettingsContentFromData(selectedItem);
			if (navigationViewItem != null)
			{
				navigationViewItem.IsSelected = true;
			}
		}
		AnimateSelectionChanged(selectedItem);
	}

	private void OnUnloaded(object sender, RoutedEventArgs args)
	{
		CoreApplicationViewTitleBar coreTitleBar = m_coreTitleBar;
		if (coreTitleBar != null)
		{
			coreTitleBar.LayoutMetricsChanged -= OnTitleBarMetricsChanged;
			coreTitleBar.IsVisibleChanged -= OnTitleBarIsVisibleChanged;
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs args)
	{
		CoreApplicationViewTitleBar coreTitleBar = m_coreTitleBar;
		if (coreTitleBar != null)
		{
			coreTitleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
			coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;
		}
		UpdatePaneButtonsWidths();
	}

	private void OnIsPaneOpenChanged()
	{
		bool isPaneOpen = IsPaneOpen;
		if (isPaneOpen && m_wasForceClosed)
		{
			m_wasForceClosed = false;
		}
		else if (!m_isOpenPaneForInteraction && !isPaneOpen)
		{
			SplitView rootSplitView = m_rootSplitView;
			if (rootSplitView != null)
			{
				m_wasForceClosed = rootSplitView.IsPaneOpen;
			}
			else
			{
				m_wasForceClosed = true;
			}
		}
		SetPaneToggleButtonAutomationName();
		UpdatePaneTabFocusNavigation();
		UpdateSettingsItemToolTip();
		UpdatePaneTitleFrameworkElementParents();
		if (SharedHelpers.IsThemeShadowAvailable())
		{
			SplitView rootSplitView2 = m_rootSplitView;
			if (rootSplitView2 != null)
			{
				SplitViewDisplayMode displayMode = rootSplitView2.DisplayMode;
				_ = rootSplitView2.Pane;
			}
		}
		UpdatePaneButtonsWidths();
	}

	private void UpdatePaneToggleButtonVisibility()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		bool visible = IsPaneToggleButtonVisible && !IsTopNavigationView();
		GetTemplateSettings().PaneToggleButtonVisibility = Util.VisibilityFromBool(visible);
	}

	private void UpdatePaneDisplayMode()
	{
		if (m_appliedTemplate)
		{
			if (!IsTopNavigationView())
			{
				UpdateAdaptiveLayout(((FrameworkElement)this).ActualWidth, forceSetDisplayMode: true);
				SwapPaneHeaderContent(m_leftNavPaneHeaderContentBorder, m_paneHeaderOnTopPane, "PaneHeader");
				SwapPaneHeaderContent(m_leftNavPaneCustomContentBorder, m_paneCustomContentOnTopPane, "PaneCustomContent");
				SwapPaneHeaderContent(m_leftNavFooterContentBorder, m_paneFooterOnTopPane, "PaneFooter");
				CreateAndHookEventsToSettings();
			}
			else
			{
				ClosePane();
				SetDisplayMode(NavigationViewDisplayMode.Minimal, forceSetDisplayMode: true);
				SwapPaneHeaderContent(m_paneHeaderOnTopPane, m_leftNavPaneHeaderContentBorder, "PaneHeader");
				SwapPaneHeaderContent(m_paneCustomContentOnTopPane, m_leftNavPaneCustomContentBorder, "PaneCustomContent");
				SwapPaneHeaderContent(m_paneFooterOnTopPane, m_leftNavFooterContentBorder, "PaneFooter");
				CreateAndHookEventsToSettings();
			}
			UpdateContentBindingsForPaneDisplayMode();
			UpdateRepeaterItemsSource(forceSelectionModelUpdate: false);
			UpdateFooterRepeaterItemsSource(sourceCollectionReset: false, sourceCollectionChanged: false);
			if (SelectedItem != null)
			{
				m_OrientationChangedPendingAnimation = true;
			}
		}
	}

	private void UpdatePaneDisplayMode(NavigationViewPaneDisplayMode oldDisplayMode, NavigationViewPaneDisplayMode newDisplayMode)
	{
		if (!m_appliedTemplate)
		{
			return;
		}
		UpdatePaneDisplayMode();
		if (IsTopNavigationView())
		{
			return;
		}
		if (IsPaneOpen)
		{
			if (newDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
			{
				ClosePane();
			}
		}
		else if (oldDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && newDisplayMode == NavigationViewPaneDisplayMode.Left)
		{
			OpenPane();
		}
	}

	private void UpdatePaneVisibility()
	{
		NavigationViewTemplateSettings templateSettings = GetTemplateSettings();
		if (IsPaneVisible)
		{
			if (IsTopNavigationView())
			{
				templateSettings.LeftPaneVisibility = (Visibility)2;
				templateSettings.TopPaneVisibility = (Visibility)0;
			}
			else
			{
				templateSettings.TopPaneVisibility = (Visibility)2;
				templateSettings.LeftPaneVisibility = (Visibility)0;
			}
			VisualStateManager.GoToState((FrameworkElement)(object)this, "PaneVisible", false);
		}
		else
		{
			templateSettings.TopPaneVisibility = (Visibility)2;
			templateSettings.LeftPaneVisibility = (Visibility)2;
			VisualStateManager.GoToState((FrameworkElement)(object)this, "PaneCollapsed", false);
		}
	}

	private void SwapPaneHeaderContent(ContentControl newParentTrackRef, ContentControl oldParentTrackRef, string propertyPathName)
	{
		if (newParentTrackRef != null)
		{
			if (oldParentTrackRef != null)
			{
				((DependencyObject)oldParentTrackRef).ClearValue(ContentControl.ContentProperty);
			}
			SharedHelpers.SetBinding(propertyPathName, (DependencyObject)(object)newParentTrackRef, ContentControl.ContentProperty);
		}
	}

	private void UpdateContentBindingsForPaneDisplayMode()
	{
		UIElement val = null;
		UIElement val2 = null;
		if (!IsTopNavigationView())
		{
			val = (UIElement)(object)m_leftNavPaneAutoSuggestBoxPresenter;
			val2 = (UIElement)(object)m_topNavPaneAutoSuggestBoxPresenter;
		}
		else
		{
			val = (UIElement)(object)m_topNavPaneAutoSuggestBoxPresenter;
			val2 = (UIElement)(object)m_leftNavPaneAutoSuggestBoxPresenter;
		}
		if (val != null)
		{
			if (val2 != null)
			{
				((DependencyObject)val2).ClearValue(ContentControl.ContentProperty);
			}
			SharedHelpers.SetBinding("AutoSuggestBox", (DependencyObject)(object)val, ContentControl.ContentProperty);
		}
	}

	private void UpdateHeaderVisibility()
	{
		if (m_appliedTemplate)
		{
			UpdateHeaderVisibility(DisplayMode);
		}
	}

	private void UpdateHeaderVisibility(NavigationViewDisplayMode displayMode)
	{
		bool flag = AlwaysShowHeader || (!IsTopNavigationView() && displayMode == NavigationViewDisplayMode.Minimal);
		if (SharedHelpers.IsRS5OrHigher())
		{
			flag = Header != null && flag;
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, flag ? "HeaderVisible" : "HeaderCollapsed", false);
	}

	private void UpdatePaneTabFocusNavigation()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (m_appliedTemplate && SharedHelpers.IsRS2OrHigher())
		{
			KeyboardNavigationMode val = (KeyboardNavigationMode)5;
			SplitView rootSplitView = m_rootSplitView;
			if (rootSplitView != null && IsPaneOpen && (rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay || rootSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay))
			{
				val = (KeyboardNavigationMode)2;
			}
			UIElement paneContentGrid = m_paneContentGrid;
			if (paneContentGrid != null)
			{
				KeyboardNavigation.SetTabNavigation((DependencyObject)(object)paneContentGrid, val);
			}
		}
	}

	private void UpdatePaneToggleSize()
	{
		if (ShouldPreserveNavigationViewRS3Behavior())
		{
			return;
		}
		SplitView rootSplitView = m_rootSplitView;
		if (rootSplitView == null)
		{
			return;
		}
		double paneToggleButtonWidth = GetPaneToggleButtonWidth();
		double width = paneToggleButtonWidth;
		if (ShouldShowBackButton() && rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
		{
			double num = 40.0;
			Button backButton = m_backButton;
			if (backButton != null)
			{
				num = ((FrameworkElement)backButton).Width;
			}
			paneToggleButtonWidth += num;
		}
		if (!m_isClosedCompact)
		{
			string paneTitle = PaneTitle;
			if (paneTitle != null && paneTitle.Length > 0)
			{
				if (rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay && IsPaneOpen)
				{
					paneToggleButtonWidth = OpenPaneLength;
					width = OpenPaneLength - (double)((ShouldShowBackButton() || ShouldShowCloseButton()) ? 40 : 0);
				}
				else if (rootSplitView.DisplayMode != SplitViewDisplayMode.Overlay || IsPaneOpen)
				{
					paneToggleButtonWidth = OpenPaneLength;
					width = OpenPaneLength;
				}
			}
		}
		Button paneToggleButton = m_paneToggleButton;
		if (paneToggleButton != null)
		{
			((FrameworkElement)paneToggleButton).Width = width;
		}
	}

	private void UpdateBackAndCloseButtonsVisibility()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (!m_appliedTemplate)
		{
			return;
		}
		bool flag = ShouldShowBackButton();
		Visibility val = Util.VisibilityFromBool(flag);
		NavigationViewVisualStateDisplayMode visualStateDisplayMode = GetVisualStateDisplayMode(DisplayMode);
		bool flag2 = (visualStateDisplayMode == NavigationViewVisualStateDisplayMode.Minimal && !IsTopNavigationView()) || visualStateDisplayMode == NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		GetTemplateSettings().BackButtonVisibility = val;
		if (m_paneToggleButton != null && IsPaneToggleButtonVisible)
		{
			num4 = GetPaneToggleButtonHeight();
			num2 = GetPaneToggleButtonWidth();
			if (flag2)
			{
				num = num2;
			}
		}
		Button backButton = m_backButton;
		if (backButton != null)
		{
			if (ShouldPreserveNavigationViewRS4Behavior())
			{
				((UIElement)backButton).Visibility = val;
			}
			if (flag2 && (int)val == 0)
			{
				num += ((FrameworkElement)backButton).Width;
			}
		}
		Button closeButton = m_closeButton;
		if (closeButton != null)
		{
			Visibility val2 = (((UIElement)closeButton).Visibility = Util.VisibilityFromBool(ShouldShowCloseButton()));
			if ((int)val2 == 0)
			{
				num4 = Math.Max(num4, ((FrameworkElement)closeButton).Height);
				if (flag2)
				{
					num3 = ((FrameworkElement)closeButton).Width;
					num += num3;
				}
			}
		}
		FrameworkElement contentLeftPadding = m_contentLeftPadding;
		if (contentLeftPadding != null)
		{
			contentLeftPadding.Width = num;
		}
		ColumnDefinition paneHeaderToggleButtonColumn = m_paneHeaderToggleButtonColumn;
		if (paneHeaderToggleButtonColumn != null)
		{
			paneHeaderToggleButtonColumn.Width = GridLengthHelper.FromValueAndType(num2, (GridUnitType)1);
		}
		ColumnDefinition paneHeaderCloseButtonColumn = m_paneHeaderCloseButtonColumn;
		if (paneHeaderCloseButtonColumn != null)
		{
			paneHeaderCloseButtonColumn.Width = GridLengthHelper.FromValueAndType(num3, (GridUnitType)1);
		}
		FrameworkElement paneTitleHolderFrameworkElement = m_paneTitleHolderFrameworkElement;
		if (paneTitleHolderFrameworkElement != null && num4 == 0.0 && (int)((UIElement)paneTitleHolderFrameworkElement).Visibility == 0)
		{
			num4 = paneTitleHolderFrameworkElement.ActualHeight;
		}
		RowDefinition paneHeaderContentBorderRow = m_paneHeaderContentBorderRow;
		if (paneHeaderContentBorderRow != null)
		{
			paneHeaderContentBorderRow.MinHeight = num4;
		}
		UIElement paneContentGrid = m_paneContentGrid;
		if (paneContentGrid != null)
		{
			Grid val4 = (Grid)(object)((paneContentGrid is Grid) ? paneContentGrid : null);
			if (val4 != null)
			{
				RowDefinitionCollection rowDefinitions = val4.RowDefinitions;
				if (rowDefinitions.Count >= 1)
				{
					RowDefinition obj = rowDefinitions[1];
					int num5 = 0;
					if (!IsOverlay() && flag)
					{
						num5 = 40;
					}
					else if (ShouldPreserveNavigationViewRS3Behavior())
					{
						num5 = 56;
					}
					GridLength height = GridLengthHelper.FromPixels(num5);
					obj.Height = height;
				}
			}
		}
		if (!ShouldPreserveNavigationViewRS4Behavior())
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, flag ? "BackButtonVisible" : "BackButtonCollapsed", false);
		}
		UpdateTitleBarPadding();
	}

	private void UpdatePaneTitleMargins()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldPreserveNavigationViewRS4Behavior())
		{
			return;
		}
		FrameworkElement paneTitleFrameworkElement = m_paneTitleFrameworkElement;
		if (paneTitleFrameworkElement != null)
		{
			double num = GetPaneToggleButtonWidth();
			if (ShouldShowBackButton() && IsOverlay())
			{
				num += 40.0;
			}
			paneTitleFrameworkElement.Margin = new Thickness(num, 0.0, 0.0, 0.0);
		}
	}

	private void UpdateSelectionForMenuItems()
	{
		if (SelectedItem == null)
		{
			bool foundFirstSelected = false;
			IList menuItems = MenuItems;
			if (menuItems != null)
			{
				foundFirstSelected = UpdateSelectedItemFromMenuItems(menuItems);
			}
			IList footerMenuItems = FooterMenuItems;
			if (footerMenuItems != null)
			{
				UpdateSelectedItemFromMenuItems(footerMenuItems, foundFirstSelected);
			}
		}
	}

	private bool UpdateSelectedItemFromMenuItems(IList menuItems, bool foundFirstSelected = false)
	{
		for (int i = 0; i < menuItems.Count; i++)
		{
			if (!(menuItems[i] is NavigationViewItem { IsSelected: not false } navigationViewItem))
			{
				continue;
			}
			if (!foundFirstSelected)
			{
				try
				{
					m_shouldIgnoreNextSelectionChange = true;
					SelectedItem = navigationViewItem;
					foundFirstSelected = true;
				}
				finally
				{
					m_shouldIgnoreNextSelectionChange = false;
				}
			}
			else
			{
				navigationViewItem.IsSelected = false;
			}
		}
		return foundFirstSelected;
	}

	private void OnTitleBarMetricsChanged(object sender, object args)
	{
		UpdateTitleBarPadding();
	}

	private void OnTitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
	{
		UpdateTitleBarPadding();
	}

	private void ClosePaneIfNeccessaryAfterItemIsClicked(NavigationViewItem selectedContainer)
	{
		if (IsPaneOpen && DisplayMode != NavigationViewDisplayMode.Expanded && !DoesNavigationViewItemHaveChildren(selectedContainer) && !m_shouldIgnoreNextSelectionChange)
		{
			ClosePane();
		}
	}

	private bool NeedTopPaddingForRS5OrHigher(CoreApplicationViewTitleBar coreTitleBar)
	{
		if (coreTitleBar.IsVisible && coreTitleBar.ExtendViewIntoTitleBar)
		{
			return !IsFullScreenOrTabletMode();
		}
		return false;
	}

	private void UpdateTitleBarPadding()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Invalid comparison between Unknown and I4
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		if (!m_appliedTemplate)
		{
			return;
		}
		double num = 0.0;
		CoreApplicationViewTitleBar coreTitleBar = m_coreTitleBar;
		if (coreTitleBar != null)
		{
			bool flag = false;
			if (IsTitleBarAutoPaddingEnabled)
			{
				flag = ShouldPreserveNavigationViewRS3Behavior() || ((!ShouldPreserveNavigationViewRS4Behavior()) ? NeedTopPaddingForRS5OrHigher(coreTitleBar) : (!coreTitleBar.ExtendViewIntoTitleBar));
			}
			if (flag)
			{
				object content = ((ContentControl)(Window.GetWindow((DependencyObject)(object)this) ?? Application.Current.MainWindow)).Content;
				UIElement visual = (UIElement)((content is UIElement) ? content : null);
				Point val = ((Visual)(object)this).SafeTransformToVisual((Visual)(object)visual).Transform(default(Point));
				if (((Point)(ref val)).Y == 0.0)
				{
					num = coreTitleBar.Height;
				}
			}
			if (ShouldPreserveNavigationViewRS4Behavior())
			{
				FrameworkElement togglePaneTopPadding = m_togglePaneTopPadding;
				if (togglePaneTopPadding != null)
				{
					togglePaneTopPadding.Height = num;
				}
				FrameworkElement contentPaneTopPadding = m_contentPaneTopPadding;
				if (contentPaneTopPadding != null)
				{
					contentPaneTopPadding.Height = num;
				}
			}
			FrameworkElement paneTitleHolderFrameworkElement = m_paneTitleHolderFrameworkElement;
			Button paneToggleButton = m_paneToggleButton;
			bool flag2 = paneTitleHolderFrameworkElement != null && (int)((UIElement)paneTitleHolderFrameworkElement).Visibility == 0;
			bool flag3 = !flag2 && paneToggleButton != null && (int)((UIElement)paneToggleButton).Visibility == 0;
			if (flag2 || flag3)
			{
				Thickness margin = ThicknessHelper.FromLengths(0.0, 0.0, 0.0, 0.0);
				if (ShouldShowBackButton())
				{
					margin = ((!IsOverlay()) ? ThicknessHelper.FromLengths(0.0, 40.0, 0.0, 0.0) : ThicknessHelper.FromLengths(40.0, 0.0, 0.0, 0.0));
				}
				else if (ShouldShowCloseButton() && IsOverlay())
				{
					margin = ThicknessHelper.FromLengths(40.0, 0.0, 0.0, 0.0);
				}
				if (flag2)
				{
					paneTitleHolderFrameworkElement.Margin = margin;
				}
				else
				{
					((FrameworkElement)paneToggleButton).Margin = margin;
				}
			}
		}
		NavigationViewTemplateSettings templateSettings = TemplateSettings;
		if (templateSettings != null && Math.Abs(templateSettings.TopPadding - num) > 0.1)
		{
			GetTemplateSettings().TopPadding = num;
		}
	}

	private void RaiseDisplayModeChanged(NavigationViewDisplayMode displayMode)
	{
		((DependencyObject)this).SetValue(DisplayModePropertyKey, (object)displayMode);
		NavigationViewDisplayModeChangedEventArgs e = new NavigationViewDisplayModeChangedEventArgs();
		e.DisplayMode = displayMode;
		this.DisplayModeChanged?.Invoke(this, e);
	}

	private void CreateAndAttachHeaderAnimation(Visual visual)
	{
	}

	private bool IsFullScreenOrTabletMode()
	{
		return false;
	}

	private void UpdatePaneShadow()
	{
	}

	private T GetContainerForData<T>(object data) where T : class
	{
		if (data == null)
		{
			return null;
		}
		if (data is T result)
		{
			return result;
		}
		NavigationViewItem settingsItem = m_settingsItem;
		if (settingsItem != null && (settingsItem == data || ((ContentControl)settingsItem).Content == data))
		{
			return settingsItem as T;
		}
		ItemsRepeater itemsRepeater = (IsTopNavigationView() ? m_topNavRepeater : m_leftNavRepeater);
		int indexFromItem = GetIndexFromItem(itemsRepeater, data);
		if (indexFromItem >= 0)
		{
			UIElement val = itemsRepeater.TryGetElement(indexFromItem);
			if (val != null)
			{
				return val as T;
			}
		}
		ItemsRepeater itemsRepeater2 = (IsTopNavigationView() ? m_topNavFooterMenuRepeater : m_leftNavFooterMenuRepeater);
		indexFromItem = GetIndexFromItem(itemsRepeater2, data);
		if (indexFromItem >= 0)
		{
			UIElement val2 = itemsRepeater2.TryGetElement(indexFromItem);
			if (val2 != null)
			{
				return val2 as T;
			}
		}
		UIElement val3 = SearchEntireTreeForContainer(itemsRepeater, data);
		if (val3 != null)
		{
			return val3 as T;
		}
		UIElement val4 = SearchEntireTreeForContainer(itemsRepeater2, data);
		if (val4 != null)
		{
			return val4 as T;
		}
		return null;
	}

	private UIElement SearchEntireTreeForContainer(ItemsRepeater rootRepeater, object data)
	{
		int indexFromItem = GetIndexFromItem(rootRepeater, data);
		if (indexFromItem != -1)
		{
			return rootRepeater.TryGetElement(indexFromItem);
		}
		for (int i = 0; i < GetContainerCountInRepeater(rootRepeater); i++)
		{
			UIElement val = rootRepeater.TryGetElement(i);
			if (val == null || !(val is NavigationViewItem navigationViewItem))
			{
				continue;
			}
			ItemsRepeater repeater = navigationViewItem.GetRepeater();
			if (repeater != null)
			{
				UIElement val2 = SearchEntireTreeForContainer(repeater, data);
				if (val2 != null)
				{
					return val2;
				}
			}
		}
		return null;
	}

	private IndexPath SearchEntireTreeForIndexPath(ItemsRepeater rootRepeater, object data, bool isFooterRepeater)
	{
		for (int i = 0; i < GetContainerCountInRepeater(rootRepeater); i++)
		{
			UIElement val = rootRepeater.TryGetElement(i);
			if (val != null && val is NavigationViewItem parentContainer)
			{
				IndexPath ip = new IndexPath(new List<int>
				{
					isFooterRepeater ? 1 : 0,
					i
				});
				IndexPath indexPath = SearchEntireTreeForIndexPath(parentContainer, data, ip);
				if (indexPath != null)
				{
					return indexPath;
				}
			}
		}
		return null;
	}

	private IndexPath SearchEntireTreeForIndexPath(NavigationViewItem parentContainer, object data, IndexPath ip)
	{
		bool flag = false;
		ItemsRepeater repeater = parentContainer.GetRepeater();
		if (repeater != null && DoesRepeaterHaveRealizedContainers(repeater))
		{
			flag = true;
			for (int i = 0; i < GetContainerCountInRepeater(repeater); i++)
			{
				UIElement val = repeater.TryGetElement(i);
				if (val != null && val is NavigationViewItem navigationViewItem)
				{
					IndexPath indexPath = ip.CloneWithChildIndex(i);
					if (((ContentControl)navigationViewItem).Content == data)
					{
						return indexPath;
					}
					IndexPath indexPath2 = SearchEntireTreeForIndexPath(navigationViewItem, data, indexPath);
					if (indexPath2 != null)
					{
						return indexPath2;
					}
				}
			}
		}
		if (!flag)
		{
			object children = GetChildren(parentContainer);
			if (children != null)
			{
				ItemsSourceView itemsSourceView = children as ItemsSourceView;
				if (children != null && itemsSourceView == null)
				{
					itemsSourceView = new InspectingDataSource(children);
				}
				for (int j = 0; j < itemsSourceView.Count; j++)
				{
					IndexPath indexPath3 = ip.CloneWithChildIndex(j);
					object at = itemsSourceView.GetAt(j);
					if (at == data)
					{
						return indexPath3;
					}
					NavigationViewItemBase navigationViewItemBase = ResolveContainerForItem(at, j);
					if (navigationViewItemBase != null && navigationViewItemBase is NavigationViewItem parentContainer2)
					{
						IndexPath indexPath4 = SearchEntireTreeForIndexPath(parentContainer2, data, indexPath3);
						if (indexPath4 != null)
						{
							return indexPath4;
						}
					}
				}
			}
		}
		return null;
	}

	private NavigationViewItemBase ResolveContainerForItem(object item, int index)
	{
		ElementFactoryGetArgs elementFactoryGetArgs = new ElementFactoryGetArgs();
		elementFactoryGetArgs.Data = item;
		elementFactoryGetArgs.Index = index;
		UIElement element = m_navigationViewItemsFactory.GetElement(elementFactoryGetArgs);
		if (element != null && element is NavigationViewItemBase result)
		{
			return result;
		}
		return null;
	}

	private void RecycleContainer(UIElement container)
	{
		ElementFactoryRecycleArgs elementFactoryRecycleArgs = new ElementFactoryRecycleArgs();
		elementFactoryRecycleArgs.Element = container;
		m_navigationViewItemsFactory.RecycleElement(elementFactoryRecycleArgs);
	}

	private int GetContainerCountInRepeater(ItemsRepeater ir)
	{
		if (ir != null)
		{
			ItemsSourceView itemsSourceView = ir.ItemsSourceView;
			if (itemsSourceView != null)
			{
				return itemsSourceView.Count;
			}
		}
		return -1;
	}

	private bool DoesRepeaterHaveRealizedContainers(ItemsRepeater ir)
	{
		if (ir != null && ir.TryGetElement(0) != null)
		{
			return true;
		}
		return false;
	}

	private int GetIndexFromItem(ItemsRepeater ir, object data)
	{
		if (ir != null)
		{
			ItemsSourceView itemsSourceView = ir.ItemsSourceView;
			if (itemsSourceView != null)
			{
				return itemsSourceView.IndexOf(data);
			}
		}
		return -1;
	}

	private object GetItemFromIndex(ItemsRepeater ir, int index)
	{
		if (ir != null)
		{
			ItemsSourceView itemsSourceView = ir.ItemsSourceView;
			if (itemsSourceView != null)
			{
				return itemsSourceView.GetAt(index);
			}
		}
		return null;
	}

	private IndexPath GetIndexPathOfItem(object data)
	{
		if (data is NavigationViewItemBase nvib)
		{
			return GetIndexPathForContainer(nvib);
		}
		if (IsTopNavigationView())
		{
			IndexPath indexPath = SearchEntireTreeForIndexPath(m_topNavRepeater, data, isFooterRepeater: false);
			if (indexPath != null)
			{
				return indexPath;
			}
			IndexPath indexPath2 = SearchEntireTreeForIndexPath(m_topNavRepeaterOverflowView, data, isFooterRepeater: false);
			if (indexPath2 != null)
			{
				return indexPath2;
			}
			IndexPath indexPath3 = SearchEntireTreeForIndexPath(m_topNavFooterMenuRepeater, data, isFooterRepeater: true);
			if (indexPath3 != null)
			{
				return indexPath3;
			}
		}
		else
		{
			IndexPath indexPath4 = SearchEntireTreeForIndexPath(m_leftNavRepeater, data, isFooterRepeater: false);
			if (indexPath4 != null)
			{
				return indexPath4;
			}
			IndexPath indexPath5 = SearchEntireTreeForIndexPath(m_leftNavFooterMenuRepeater, data, isFooterRepeater: true);
			if (indexPath5 != null)
			{
				return indexPath5;
			}
		}
		return new IndexPath(new List<int>(0));
	}

	private UIElement GetContainerForIndex(int index, bool inFooter)
	{
		if (IsTopNavigationView())
		{
			ItemsRepeater obj = (inFooter ? m_topNavFooterMenuRepeater : (m_topDataProvider.IsItemInPrimaryList(index) ? m_topNavRepeater : m_topNavRepeaterOverflowView));
			int index2 = (inFooter ? index : m_topDataProvider.ConvertOriginalIndexToIndex(index));
			UIElement val = obj.TryGetElement(index2);
			if (val != null)
			{
				return val;
			}
		}
		else
		{
			UIElement val2 = (inFooter ? m_leftNavFooterMenuRepeater.TryGetElement(index) : m_leftNavRepeater.TryGetElement(index));
			if (val2 != null)
			{
				return (UIElement)(object)(val2 as NavigationViewItemBase);
			}
		}
		return null;
	}

	private NavigationViewItemBase GetContainerForIndexPath(IndexPath ip, bool lastVisible = false)
	{
		if (ip != null && ip.GetSize() > 0)
		{
			UIElement containerForIndex = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == 1);
			if (containerForIndex != null)
			{
				if (lastVisible && containerForIndex is NavigationViewItem { IsExpanded: false } navigationViewItem)
				{
					return navigationViewItem;
				}
				return GetContainerForIndexPath(containerForIndex, ip, lastVisible);
			}
		}
		return null;
	}

	private NavigationViewItemBase GetContainerForIndexPath(UIElement firstContainer, IndexPath ip, bool lastVisible)
	{
		UIElement val = firstContainer;
		if (ip.GetSize() > 2)
		{
			for (int i = 2; i < ip.GetSize(); i++)
			{
				bool flag = false;
				if (val is NavigationViewItem navigationViewItem)
				{
					if (lastVisible && !navigationViewItem.IsExpanded)
					{
						return navigationViewItem;
					}
					ItemsRepeater repeater = navigationViewItem.GetRepeater();
					if (repeater != null)
					{
						UIElement val2 = repeater.TryGetElement(ip.GetAt(i));
						if (val2 != null)
						{
							val = val2;
							flag = true;
						}
					}
				}
				if (!flag)
				{
					return null;
				}
			}
		}
		return val as NavigationViewItemBase;
	}

	private bool IsContainerTheSelectedItemInTheSelectionModel(NavigationViewItemBase nvib)
	{
		object selectedItem = m_selectionModel.SelectedItem;
		if (selectedItem != null)
		{
			NavigationViewItemBase navigationViewItemBase = selectedItem as NavigationViewItemBase;
			if (navigationViewItemBase == null)
			{
				navigationViewItemBase = GetContainerForIndexPath(m_selectionModel.SelectedIndex);
			}
			return navigationViewItemBase == nvib;
		}
		return false;
	}

	internal ItemsRepeater LeftNavRepeater()
	{
		return m_leftNavRepeater;
	}

	internal NavigationViewItem GetSelectedContainer()
	{
		object selectedItem = SelectedItem;
		if (selectedItem != null)
		{
			if (selectedItem is NavigationViewItem result)
			{
				return result;
			}
			return NavigationViewItemOrSettingsContentFromData(selectedItem);
		}
		return null;
	}

	internal void Expand(NavigationViewItem item)
	{
		ChangeIsExpandedNavigationViewItem(item, isExpanded: true);
	}

	internal void Collapse(NavigationViewItem item)
	{
		ChangeIsExpandedNavigationViewItem(item, isExpanded: false);
	}

	private bool DoesNavigationViewItemHaveChildren(NavigationViewItem nvi)
	{
		if (nvi.MenuItems.Count <= 0 && nvi.MenuItemsSource == null)
		{
			return nvi.HasUnrealizedChildren;
		}
		return true;
	}

	private void ToggleIsExpandedNavigationViewItem(NavigationViewItem nvi)
	{
		ChangeIsExpandedNavigationViewItem(nvi, !nvi.IsExpanded);
	}

	private void ChangeIsExpandedNavigationViewItem(NavigationViewItem nvi, bool isExpanded)
	{
		if (DoesNavigationViewItemHaveChildren(nvi))
		{
			nvi.IsExpanded = isExpanded;
		}
	}

	private NavigationViewItem FindLowestLevelContainerToDisplaySelectionIndicator()
	{
		int num = 1;
		IndexPath selectedIndex = m_selectionModel.SelectedIndex;
		if (selectedIndex != null && selectedIndex.GetSize() > 1)
		{
			UIElement containerForIndex = GetContainerForIndex(selectedIndex.GetAt(num), selectedIndex.GetAt(0) == 1);
			if (containerForIndex != null)
			{
				NavigationViewItem navigationViewItem = containerForIndex as NavigationViewItem;
				if (navigationViewItem != null)
				{
					NavigationViewItem navigationViewItem2 = navigationViewItem;
					bool flag = navigationViewItem2.IsRepeaterVisible();
					while (navigationViewItem != null && flag && !navigationViewItem.IsSelected && navigationViewItem.IsChildSelected)
					{
						num++;
						flag = false;
						ItemsRepeater repeater = navigationViewItem2.GetRepeater();
						if (repeater != null)
						{
							UIElement val = repeater.TryGetElement(selectedIndex.GetAt(num));
							if (val != null)
							{
								navigationViewItem = val as NavigationViewItem;
								navigationViewItem2 = navigationViewItem;
								flag = navigationViewItem2.IsRepeaterVisible();
							}
						}
					}
					return navigationViewItem;
				}
			}
		}
		return null;
	}

	private void ShowHideChildrenItemsRepeater(NavigationViewItem nvi)
	{
		nvi.ShowHideChildren();
		if (nvi.ShouldRepeaterShowInFlyout())
		{
			if (nvi.IsExpanded)
			{
				m_lastItemExpandedIntoFlyout = nvi;
			}
			else
			{
				m_lastItemExpandedIntoFlyout = null;
			}
		}
		if (!nvi.IsSelected && nvi.IsChildSelected)
		{
			if (!nvi.IsRepeaterVisible() && nvi.IsChildSelected)
			{
				AnimateSelectionChanged(nvi);
			}
			else
			{
				AnimateSelectionChanged(FindLowestLevelContainerToDisplaySelectionIndicator());
			}
		}
		nvi.RotateExpandCollapseChevron(nvi.IsExpanded);
	}

	private object GetChildren(NavigationViewItem nvi)
	{
		if (nvi.MenuItems.Count > 0)
		{
			return nvi.MenuItems;
		}
		return nvi.MenuItemsSource;
	}

	private ItemsRepeater GetChildRepeaterForIndexPath(IndexPath ip)
	{
		if (GetContainerForIndexPath(ip) is NavigationViewItem navigationViewItem)
		{
			return navigationViewItem.GetRepeater();
		}
		return null;
	}

	private object GetChildrenForItemInIndexPath(IndexPath ip, bool forceRealize = false)
	{
		if (ip != null && ip.GetSize() > 1)
		{
			UIElement containerForIndex = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == 1);
			if (containerForIndex != null)
			{
				return GetChildrenForItemInIndexPath(containerForIndex, ip, forceRealize);
			}
		}
		return null;
	}

	private object GetChildrenForItemInIndexPath(UIElement firstContainer, IndexPath ip, bool forceRealize = false)
	{
		UIElement val = firstContainer;
		bool flag = false;
		if (ip.GetSize() > 2)
		{
			for (int i = 2; i < ip.GetSize(); i++)
			{
				bool flag2 = false;
				if (val is NavigationViewItem navigationViewItem)
				{
					int at = ip.GetAt(i);
					ItemsRepeater repeater = navigationViewItem.GetRepeater();
					if (repeater != null && DoesRepeaterHaveRealizedContainers(repeater))
					{
						UIElement val2 = repeater.TryGetElement(at);
						if (val2 != null)
						{
							val = val2;
							flag2 = true;
						}
					}
					else if (forceRealize)
					{
						object children = GetChildren(navigationViewItem);
						if (children != null)
						{
							if (flag)
							{
								RecycleContainer((UIElement)(object)navigationViewItem);
								flag = false;
							}
							ItemsSourceView itemsSourceView = children as ItemsSourceView;
							if (children != null && itemsSourceView == null)
							{
								itemsSourceView = new InspectingDataSource(children);
							}
							object at2 = itemsSourceView.GetAt(at);
							if (at2 != null)
							{
								NavigationViewItemBase navigationViewItemBase = ResolveContainerForItem(at2, at);
								if (navigationViewItemBase != null && navigationViewItemBase is NavigationViewItem navigationViewItem2)
								{
									val = (UIElement)(object)navigationViewItem2;
									flag = true;
									flag2 = true;
								}
							}
						}
					}
				}
				if (!flag2)
				{
					return null;
				}
			}
		}
		if (val is NavigationViewItem navigationViewItem3)
		{
			object children2 = GetChildren(navigationViewItem3);
			if (flag)
			{
				RecycleContainer((UIElement)(object)navigationViewItem3);
			}
			return children2;
		}
		return null;
	}

	private void CollapseTopLevelMenuItems(NavigationViewPaneDisplayMode oldDisplayMode)
	{
		if (oldDisplayMode == NavigationViewPaneDisplayMode.Top)
		{
			CollapseMenuItemsInRepeater(m_topNavRepeater);
			CollapseMenuItemsInRepeater(m_topNavRepeaterOverflowView);
		}
		else
		{
			CollapseMenuItemsInRepeater(m_leftNavRepeater);
		}
	}

	private void CollapseMenuItemsInRepeater(ItemsRepeater ir)
	{
		for (int i = 0; i < GetContainerCountInRepeater(ir); i++)
		{
			UIElement val = ir.TryGetElement(i);
			if (val != null && val is NavigationViewItem nvi)
			{
				ChangeIsExpandedNavigationViewItem(nvi, isExpanded: false);
			}
		}
	}

	private void RaiseExpandingEvent(NavigationViewItemBase container)
	{
		NavigationViewItemExpandingEventArgs e = new NavigationViewItemExpandingEventArgs(this);
		e.ExpandingItemContainer = container;
		this.Expanding?.Invoke(this, e);
	}

	private void RaiseCollapsedEvent(NavigationViewItemBase container)
	{
		NavigationViewItemCollapsedEventArgs e = new NavigationViewItemCollapsedEventArgs(this);
		e.CollapsedItemContainer = container;
		this.Collapsed?.Invoke(this, e);
	}

	private bool IsTopLevelItem(NavigationViewItemBase nvib)
	{
		return IsRootItemsRepeater((DependencyObject)(object)GetParentItemsRepeaterForContainer(nvib));
	}

	DependencyObject IControlProtected.GetTemplateChild(string childName)
	{
		return ((FrameworkElement)this).GetTemplateChild(childName);
	}

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Visual)this).OnDpiChanged(oldDpi, newDpi);
		m_bitmapCache.RenderAtScale = ((DpiScale)(ref newDpi)).PixelsPerDip;
	}

	private static void OnIsPaneOpenPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnCompactModeThresholdWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnExpandedModeThresholdWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnFooterMenuItemsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnFooterMenuItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnPaneFooterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnHeaderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnHeaderTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnDisplayModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnIsSettingsVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnIsPaneToggleButtonVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnAlwaysShowHeaderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnCompactPaneLengthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnOpenPaneLengthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnPaneToggleButtonStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnSelectedItemPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnSettingsItemPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnAutoSuggestBoxPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemTemplateSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemContainerStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnMenuItemContainerStyleSelectorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).OnMenuItemContainerStyleSelectorPropertyChanged(args);
	}

	private void OnMenuItemContainerStyleSelectorPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
	}

	private static void OnIsBackButtonVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnIsBackEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnPaneTitlePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnPaneDisplayModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnIsPaneVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnSelectionFollowsFocusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnShoulderNavigationEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnOverflowLabelModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static void OnIsTitleBarAutoPaddingEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationView)(object)sender).PropertyChanged(args);
	}

	private static object CoerceToGreaterThanZero(DependencyObject d, object baseValue)
	{
		if (baseValue is double value)
		{
			((NavigationView)(object)d).CoerceToGreaterThanZero(ref value);
			return value;
		}
		return baseValue;
	}
}
