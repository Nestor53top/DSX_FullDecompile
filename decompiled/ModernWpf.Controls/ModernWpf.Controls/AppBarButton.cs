using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class AppBarButton : Button, ICommandBarElement, IAppBarElement
{
	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty FlyoutProperty;

	public static readonly DependencyProperty IconProperty;

	public static readonly DependencyProperty LabelProperty;

	public static readonly DependencyProperty LabelPositionProperty;

	public static readonly DependencyProperty IsCompactProperty;

	public static readonly DependencyProperty IsInOverflowProperty;

	private static readonly DependencyProperty ApplicationViewStateProperty;

	public static readonly DependencyProperty InputGestureTextProperty;

	private AppBarElementVisualStateManager _vsm;

	public bool UseSystemFocusVisuals
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UseSystemFocusVisualsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UseSystemFocusVisualsProperty, (object)value);
		}
	}

	public Thickness FocusVisualMargin
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(FocusVisualMarginProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FocusVisualMarginProperty, (object)value);
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

	public FlyoutBase Flyout
	{
		get
		{
			return (FlyoutBase)((DependencyObject)this).GetValue(FlyoutProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FlyoutProperty, (object)value);
		}
	}

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

	public string Label
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(LabelProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LabelProperty, (object)value);
		}
	}

	public CommandBarLabelPosition LabelPosition
	{
		get
		{
			return (CommandBarLabelPosition)((DependencyObject)this).GetValue(LabelPositionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LabelPositionProperty, (object)value);
		}
	}

	public bool IsCompact
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsCompactProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsCompactProperty, (object)value);
		}
	}

	public bool IsInOverflow => (bool)((DependencyObject)this).GetValue(IsInOverflowProperty);

	private AppBarElementApplicationViewState ApplicationViewState => (AppBarElementApplicationViewState)((DependencyObject)this).GetValue(ApplicationViewStateProperty);

	public string InputGestureText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(InputGestureTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InputGestureTextProperty, (object)value);
		}
	}

	static AppBarButton()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		//IL_01bc: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(AppBarButton));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(AppBarButton));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(AppBarButton));
		FlyoutProperty = FlyoutService.FlyoutProperty.AddOwner(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlyoutChanged)));
		IconProperty = AppBarElementProperties.IconProperty.AddOwner(typeof(AppBarButton));
		LabelProperty = AppBarElementProperties.LabelProperty.AddOwner(typeof(AppBarButton));
		LabelPositionProperty = AppBarElementProperties.LabelPositionProperty.AddOwner(typeof(AppBarButton));
		IsCompactProperty = AppBarElementProperties.IsCompactProperty.AddOwner(typeof(AppBarButton));
		IsInOverflowProperty = AppBarElementProperties.IsInOverflowProperty.AddOwner(typeof(AppBarButton));
		ApplicationViewStateProperty = AppBarElementProperties.ApplicationViewStateProperty.AddOwner(typeof(AppBarButton));
		InputGestureTextProperty = AppBarElementProperties.InputGestureTextProperty.AddOwner(typeof(AppBarButton));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AppBarButton)));
		UIElement.IsEnabledProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));
		ButtonBase.CommandProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandPropertyChanged)));
		FrameworkElement.ToolTipProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata
		{
			CoerceValueCallback = new CoerceValueCallback(AppBarElementProperties.CoerceToolTip)
		});
		ToolBar.OverflowModeProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverflowModePropertyChanged)));
		CommandBarToolBar.DefaultLabelPositionProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDefaultLabelPositionPropertyChanged)));
		AppBarElementProperties.IsInOverflowPropertyKey.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsInOverflowChanged)));
		AppBarElementProperties.ShowKeyboardAcceleratorTextProperty.OverrideMetadata(typeof(AppBarButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowKeyboardAcceleratorTextPropertyChanged)));
	}

	public AppBarButton()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
	}

	private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarButton)(object)d).OnFlyoutChanged();
	}

	private void OnFlyoutChanged()
	{
		UpdateVisualState();
	}

	private static void OnIsInOverflowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarButton)(object)d).UpdateCommonState();
	}

	private void UpdateApplicationViewState()
	{
		AppBarElementApplicationViewState appBarElementApplicationViewState;
		if (!IsInOverflow || !((UIElement)this).IsVisible || !(((Visual)this).VisualParent is CommandBarOverflowPanel commandBarOverflowPanel))
		{
			CommandBarDefaultLabelPosition commandBarDefaultLabelPosition = ((((Visual)this).VisualParent is ToolBarPanel) ? ((CommandBarDefaultLabelPosition)((DependencyObject)this).GetValue(CommandBarToolBar.DefaultLabelPositionProperty)) : CommandBarDefaultLabelPosition.Bottom);
			appBarElementApplicationViewState = ((LabelPosition == CommandBarLabelPosition.Collapsed || commandBarDefaultLabelPosition == CommandBarDefaultLabelPosition.Collapsed) ? AppBarElementApplicationViewState.LabelCollapsed : ((commandBarDefaultLabelPosition == CommandBarDefaultLabelPosition.Right) ? AppBarElementApplicationViewState.LabelOnRight : (IsCompact ? AppBarElementApplicationViewState.Compact : AppBarElementApplicationViewState.FullSize)));
		}
		else
		{
			appBarElementApplicationViewState = ComputeApplicationViewStateInOverflow(commandBarOverflowPanel.HasToggleButton, commandBarOverflowPanel.HasMenuIcon);
		}
		((DependencyObject)this).SetValue(AppBarElementProperties.ApplicationViewStatePropertyKey, (object)appBarElementApplicationViewState);
	}

	private AppBarElementApplicationViewState ComputeApplicationViewStateInOverflow(bool hasToggleButton, bool hasMenuIcon)
	{
		if (hasToggleButton && hasMenuIcon)
		{
			return AppBarElementApplicationViewState.OverflowWithToggleButtonsAndMenuIcons;
		}
		if (hasToggleButton)
		{
			return AppBarElementApplicationViewState.OverflowWithToggleButtons;
		}
		if (hasMenuIcon)
		{
			return AppBarElementApplicationViewState.OverflowWithMenuIcons;
		}
		return AppBarElementApplicationViewState.Overflow;
	}

	private void ApplyApplicationViewState(bool useTransitions = true)
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, ApplicationViewState.ToString(), useTransitions);
	}

	void IAppBarElement.UpdateApplicationViewState()
	{
		UpdateApplicationViewState();
	}

	void IAppBarElement.ApplyApplicationViewState()
	{
		ApplyApplicationViewState();
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		FrameworkElement templateRoot = ((Control)(object)this).GetTemplateRoot();
		if (templateRoot != null)
		{
			_vsm = new AppBarElementVisualStateManager();
			VisualStateManager.SetCustomVisualStateManager(templateRoot, (VisualStateManager)(object)_vsm);
		}
		UpdateVisualState(useTransitions: false);
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		((FrameworkElement)this).OnVisualParentChanged(oldParent);
		UpdateApplicationViewState();
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == UIElement.IsMouseOverProperty)
		{
			UpdateCommonState();
		}
		else if (((DependencyPropertyChangedEventArgs)(ref e)).Property == ToolBar.IsOverflowItemProperty)
		{
			AppBarElementProperties.UpdateIsInOverflow((DependencyObject)(object)this);
		}
	}

	protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((ButtonBase)this).OnIsPressedChanged(e);
		UpdateCommonState();
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarButton)(object)d).UpdateCommonState();
	}

	private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarButton obj = (AppBarButton)(object)d;
		((DependencyObject)obj).CoerceValue(LabelProperty);
		((DependencyObject)obj).CoerceValue(InputGestureTextProperty);
	}

	private static void OnOverflowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarElementProperties.UpdateIsInOverflow(d);
	}

	private static void OnDefaultLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarButton)(object)d).UpdateApplicationViewState();
	}

	private static void OnShowKeyboardAcceleratorTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarButton)(object)d).UpdateKeyboardAcceleratorTextVisibility();
	}

	private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateApplicationViewState();
	}

	private void UpdateVisualState(bool useTransitions = true)
	{
		ApplyApplicationViewState(useTransitions);
		UpdateCommonState(useTransitions);
		UpdateKeyboardAcceleratorTextVisibility(useTransitions);
		UpdateFlyoutState(useTransitions);
	}

	private void UpdateCommonState(bool useTransitions = true)
	{
		if (_vsm == null)
		{
			return;
		}
		string text;
		if (!((UIElement)this).IsEnabled)
		{
			text = "Disabled";
		}
		else
		{
			text = (((ButtonBase)this).IsPressed ? "Pressed" : ((!((UIElement)this).IsMouseOver) ? "Normal" : "PointerOver"));
			if (IsInOverflow)
			{
				text = "Overflow" + text;
			}
		}
		_vsm.CanChangeCommonState = true;
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
		_vsm.CanChangeCommonState = false;
	}

	private void UpdateKeyboardAcceleratorTextVisibility(bool useTransitions = true)
	{
		string text = (AppBarElementProperties.GetShowKeyboardAcceleratorText((DependencyObject)(object)this) ? "KeyboardAcceleratorTextVisible" : "KeyboardAcceleratorTextCollapsed");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void UpdateFlyoutState(bool useTransitions = true)
	{
		bool flag = Flyout != null && !ToolBar.GetIsOverflowItem((DependencyObject)(object)this);
		VisualStateManager.GoToState((FrameworkElement)(object)this, flag ? "HasFlyout" : "NoFlyout", useTransitions);
	}
}
