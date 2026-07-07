using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernWpf.Controls.Primitives;

[TemplatePart(Name = "OverflowPopup", Type = typeof(Popup))]
public class CommandBarToolBar : ToolBar
{
	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty DefaultLabelPositionProperty;

	public static readonly DependencyProperty IsDynamicOverflowEnabledProperty;

	public static readonly DependencyProperty OverflowButtonVisibilityProperty;

	public static readonly DependencyProperty OverflowPresenterStyleProperty;

	private static readonly DependencyPropertyKey OverflowContentMaxHeightPropertyKey;

	public static readonly DependencyProperty OverflowContentMaxHeightProperty;

	private static readonly DependencyPropertyKey EffectiveOverflowButtonVisibilityPropertyKey;

	public static readonly DependencyProperty EffectiveOverflowButtonVisibilityProperty;

	private FrameworkElement m_layoutRoot;

	private ButtonBase m_moreButton;

	private Popup m_overflowPopup;

	private CommandBarPanel m_toolBarPanel;

	private CommandBarOverflowPanel m_toolBarOverflowPanel;

	private readonly ToolTip m_moreButtonClosedToolTip;

	private readonly ToolTip m_moreButtonOpenToolTip;

	private const string OverflowPopupName = "OverflowPopup";

	private const string ToolBarPanelName = "PART_ToolBarPanel";

	private const string ToolBarOverflowPanelName = "PART_ToolBarOverflowPanel";

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

	public CommandBarDefaultLabelPosition DefaultLabelPosition
	{
		get
		{
			return (CommandBarDefaultLabelPosition)((DependencyObject)this).GetValue(DefaultLabelPositionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DefaultLabelPositionProperty, (object)value);
		}
	}

	public bool IsDynamicOverflowEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsDynamicOverflowEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsDynamicOverflowEnabledProperty, (object)value);
		}
	}

	public CommandBarOverflowButtonVisibility OverflowButtonVisibility
	{
		get
		{
			return (CommandBarOverflowButtonVisibility)((DependencyObject)this).GetValue(OverflowButtonVisibilityProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OverflowButtonVisibilityProperty, (object)value);
		}
	}

	public Style OverflowPresenterStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(OverflowPresenterStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OverflowPresenterStyleProperty, (object)value);
		}
	}

	public double OverflowContentMaxHeight
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OverflowContentMaxHeightProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(OverflowContentMaxHeightPropertyKey, (object)value);
		}
	}

	public Visibility EffectiveOverflowButtonVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(EffectiveOverflowButtonVisibilityProperty);
		}
		private set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(EffectiveOverflowButtonVisibilityPropertyKey, (object)value);
		}
	}

	internal Popup OverflowPopup => m_overflowPopup;

	internal bool HasPrimaryCommands
	{
		get
		{
			if (m_toolBarPanel != null)
			{
				return m_toolBarPanel.HasChildren;
			}
			return false;
		}
	}

	internal event EventHandler OverflowOpened;

	internal event EventHandler OverflowClosed;

	internal event EventHandler HasPrimaryCommandsChanged;

	internal event EventHandler HasOverflowItemsChanged;

	static CommandBarToolBar()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(CommandBarToolBar));
		DefaultLabelPositionProperty = DependencyProperty.RegisterAttached("DefaultLabelPosition", typeof(CommandBarDefaultLabelPosition), typeof(CommandBarToolBar), new PropertyMetadata((object)CommandBarDefaultLabelPosition.Right));
		IsDynamicOverflowEnabledProperty = DependencyProperty.Register("IsDynamicOverflowEnabled", typeof(bool), typeof(CommandBarToolBar), new PropertyMetadata((object)true));
		OverflowButtonVisibilityProperty = DependencyProperty.Register("OverflowButtonVisibility", typeof(CommandBarOverflowButtonVisibility), typeof(CommandBarToolBar), new PropertyMetadata((object)CommandBarOverflowButtonVisibility.Auto, new PropertyChangedCallback(OnOverflowButtonVisibilityChanged)));
		OverflowPresenterStyleProperty = DependencyProperty.Register("OverflowPresenterStyle", typeof(Style), typeof(CommandBarToolBar), (PropertyMetadata)null);
		OverflowContentMaxHeightPropertyKey = DependencyProperty.RegisterReadOnly("OverflowContentMaxHeight", typeof(double), typeof(CommandBarToolBar), new PropertyMetadata((object)CalculateOverflowContentMaxHeight()));
		OverflowContentMaxHeightProperty = OverflowContentMaxHeightPropertyKey.DependencyProperty;
		EffectiveOverflowButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("EffectiveOverflowButtonVisibility", typeof(Visibility), typeof(CommandBarToolBar), new PropertyMetadata((object)(Visibility)2, new PropertyChangedCallback(OnEffectiveOverflowButtonVisibilityChanged)));
		EffectiveOverflowButtonVisibilityProperty = EffectiveOverflowButtonVisibilityPropertyKey.DependencyProperty;
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBarToolBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandBarToolBar)));
		ToolBar.IsOverflowOpenProperty.OverrideMetadata(typeof(CommandBarToolBar), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsOverflowOpenChanged)));
		KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CommandBarToolBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)(KeyboardNavigationMode)4));
		KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CommandBarToolBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)(KeyboardNavigationMode)0));
	}

	public CommandBarToolBar()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		m_moreButtonClosedToolTip = new ToolTip
		{
			Content = Strings.AppBarMoreButtonClosedToolTip
		};
		m_moreButtonOpenToolTip = new ToolTip
		{
			Content = Strings.AppBarMoreButtonOpenToolTip
		};
	}

	private static void OnOverflowButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((CommandBarToolBar)(object)d).UpdateEffectiveOverflowButtonVisibility();
	}

	private void UpdateOverflowContentMaxHeight()
	{
		OverflowContentMaxHeight = CalculateOverflowContentMaxHeight();
	}

	private static double CalculateOverflowContentMaxHeight()
	{
		return SystemParameters.PrimaryScreenHeight / 2.0 + 20.0;
	}

	private static void OnEffectiveOverflowButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((CommandBarToolBar)(object)d).OnEffectiveOverflowButtonVisibilityChanged();
	}

	private void OnEffectiveOverflowButtonVisibilityChanged()
	{
		InvalidateLayout();
	}

	private void UpdateEffectiveOverflowButtonVisibility()
	{
		bool flag = true;
		switch (OverflowButtonVisibility)
		{
		case CommandBarOverflowButtonVisibility.Auto:
			flag = ((ToolBar)this).HasOverflowItems;
			break;
		case CommandBarOverflowButtonVisibility.Collapsed:
			flag = false;
			break;
		}
		EffectiveOverflowButtonVisibility = (Visibility)((!flag) ? 2 : 0);
	}

	public override void OnApplyTemplate()
	{
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		if (m_moreButton != null)
		{
			((DependencyObject)m_moreButton).ClearValue(FrameworkElement.ToolTipProperty);
		}
		if (m_overflowPopup != null)
		{
			((DependencyObject)m_overflowPopup).ClearValue(Popup.CustomPopupPlacementCallbackProperty);
			((DependencyObject)m_overflowPopup).ClearValue(CustomPopupPlacementHelper.PlacementProperty);
			m_overflowPopup.Opened -= OnOverflowPopupOpened;
			m_overflowPopup.Closed -= OnOverflowPopupClosed;
		}
		if (m_toolBarPanel != null)
		{
			m_toolBarPanel.HasChildrenChanged -= OnToolBarPanelHasChildrenChanged;
		}
		((FrameworkElement)this).OnApplyTemplate();
		m_layoutRoot = ((Control)(object)this).GetTemplateRoot();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("MoreButton");
		m_moreButton = (ButtonBase)(object)((templateChild is ButtonBase) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("OverflowPopup");
		m_overflowPopup = (Popup)(object)((templateChild2 is Popup) ? templateChild2 : null);
		m_toolBarPanel = ((FrameworkElement)this).GetTemplateChild("PART_ToolBarPanel") as CommandBarPanel;
		m_toolBarOverflowPanel = ((FrameworkElement)this).GetTemplateChild("PART_ToolBarOverflowPanel") as CommandBarOverflowPanel;
		if (m_moreButton != null)
		{
			AutomationProperties.SetName((DependencyObject)(object)m_moreButton, Strings.AppBarMoreButtonName);
			UpdateMoreButtonTooTip();
		}
		if (m_overflowPopup != null)
		{
			m_overflowPopup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PositionOverflowPopup);
			((DependencyObject)m_overflowPopup).SetValue(CustomPopupPlacementHelper.PlacementProperty, (object)CustomPlacementMode.BottomEdgeAlignedRight);
			m_overflowPopup.Opened += OnOverflowPopupOpened;
			m_overflowPopup.Closed += OnOverflowPopupClosed;
		}
		if (m_toolBarPanel != null)
		{
			m_toolBarPanel.HasChildrenChanged += OnToolBarPanelHasChildrenChanged;
		}
		if (((FrameworkElement)this).TemplatedParent is CommandBar commandBar)
		{
			commandBar.UpdateVisualState(useTransitions: false);
		}
		InvalidateLayout();
	}

	protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		((ToolBar)this).PrepareContainerForItemOverride(element, item);
		if (element is AppBarButton || element is AppBarToggleButton)
		{
			SharedHelpers.SetBinding((FrameworkElement)element, DefaultLabelPositionProperty, DefaultLabelPositionProperty, (DependencyObject)(object)this);
		}
	}

	protected override void ClearContainerForItemOverride(DependencyObject element, object item)
	{
		if (element is AppBarButton || element is AppBarToggleButton)
		{
			element.ClearValue(DefaultLabelPositionProperty);
		}
		((ItemsControl)this).ClearContainerForItemOverride(element, item);
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == ToolBar.HasOverflowItemsProperty)
		{
			UpdateEffectiveOverflowButtonVisibility();
			this.HasOverflowItemsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		if ((int)e.Key == 13)
		{
			ToolBarOverflowPanel toolBarOverflowPanel = (ToolBarOverflowPanel)(object)m_toolBarOverflowPanel;
			if (toolBarOverflowPanel != null && ((UIElement)toolBarOverflowPanel).IsKeyboardFocusWithin)
			{
				((UIElement)this).MoveFocus(new TraversalRequest((FocusNavigationDirection)3));
			}
			else
			{
				Keyboard.Focus((IInputElement)null);
			}
			((DependencyObject)this).SetCurrentValue(ToolBar.IsOverflowOpenProperty, (object)false);
		}
		((ToolBar)this).OnKeyDown(e);
	}

	private static void OnIsOverflowOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((CommandBarToolBar)(object)d).UpdateMoreButtonTooTip();
	}

	private void InvalidateLayout()
	{
		FrameworkElement layoutRoot = m_layoutRoot;
		if (layoutRoot == null)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
		{
			if (layoutRoot.ActualHeight > 0.0)
			{
				layoutRoot.Height = layoutRoot.ActualHeight;
				((UIElement)layoutRoot).UpdateLayout();
				((DependencyObject)layoutRoot).ClearValue(FrameworkElement.HeightProperty);
			}
		});
	}

	private void OnOverflowPopupOpened(object sender, EventArgs e)
	{
		UpdateOverflowContentMaxHeight();
		this.OverflowOpened?.Invoke(this, EventArgs.Empty);
	}

	private void OnOverflowPopupClosed(object sender, EventArgs e)
	{
		this.OverflowClosed?.Invoke(this, EventArgs.Empty);
	}

	private void OnToolBarPanelHasChildrenChanged(object sender, EventArgs e)
	{
		this.HasPrimaryCommandsChanged?.Invoke(this, e);
	}

	private CustomPopupPlacement[] PositionOverflowPopup(Size popupSize, Size targetSize, Point offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		UIElement child = m_overflowPopup.Child;
		return CustomPopupPlacementHelper.PositionPopup(CustomPlacementMode.BottomEdgeAlignedRight, popupSize, targetSize, offset, (FrameworkElement)(object)((child is FrameworkElement) ? child : null));
	}

	private void UpdateMoreButtonTooTip()
	{
		if (m_moreButton != null)
		{
			((FrameworkElement)m_moreButton).ToolTip = (((ToolBar)this).IsOverflowOpen ? m_moreButtonOpenToolTip : m_moreButtonClosedToolTip);
		}
	}
}
