using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class AppBarToggleButton : ToggleButton, ICommandBarElement, IAppBarElement
{
	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

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

	static AppBarToggleButton()
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01b7: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(AppBarToggleButton));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(AppBarToggleButton));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(AppBarToggleButton));
		IconProperty = AppBarElementProperties.IconProperty.AddOwner(typeof(AppBarToggleButton));
		LabelProperty = AppBarElementProperties.LabelProperty.AddOwner(typeof(AppBarToggleButton));
		LabelPositionProperty = AppBarElementProperties.LabelPositionProperty.AddOwner(typeof(AppBarToggleButton));
		IsCompactProperty = AppBarElementProperties.IsCompactProperty.AddOwner(typeof(AppBarToggleButton));
		IsInOverflowProperty = AppBarElementProperties.IsInOverflowProperty.AddOwner(typeof(AppBarToggleButton));
		ApplicationViewStateProperty = AppBarElementProperties.ApplicationViewStateProperty.AddOwner(typeof(AppBarToggleButton));
		InputGestureTextProperty = AppBarElementProperties.InputGestureTextProperty.AddOwner(typeof(AppBarToggleButton));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AppBarToggleButton)));
		UIElement.IsEnabledProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));
		ButtonBase.CommandProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandPropertyChanged)));
		ToggleButton.IsCheckedProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsCheckedChanged)));
		FrameworkElement.ToolTipProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata
		{
			CoerceValueCallback = new CoerceValueCallback(AppBarElementProperties.CoerceToolTip)
		});
		ToolBar.OverflowModeProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverflowModePropertyChanged)));
		CommandBarToolBar.DefaultLabelPositionProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDefaultLabelPositionPropertyChanged)));
		AppBarElementProperties.IsInOverflowPropertyKey.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsInOverflowChanged)));
		AppBarElementProperties.ShowKeyboardAcceleratorTextProperty.OverrideMetadata(typeof(AppBarToggleButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowKeyboardAcceleratorTextPropertyChanged)));
	}

	public AppBarToggleButton()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
	}

	private static void OnIsInOverflowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarToggleButton)(object)d).UpdateCommonState();
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
			appBarElementApplicationViewState = ComputeApplicationViewStateInOverflow(commandBarOverflowPanel.HasMenuIcon);
		}
		((DependencyObject)this).SetValue(AppBarElementProperties.ApplicationViewStatePropertyKey, (object)appBarElementApplicationViewState);
	}

	private AppBarElementApplicationViewState ComputeApplicationViewStateInOverflow(bool hasMenuIcon)
	{
		if (!hasMenuIcon)
		{
			return AppBarElementApplicationViewState.Overflow;
		}
		return AppBarElementApplicationViewState.OverflowWithMenuIcons;
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
		((AppBarToggleButton)(object)d).UpdateCommonState();
	}

	private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarToggleButton obj = (AppBarToggleButton)(object)d;
		((DependencyObject)obj).CoerceValue(LabelProperty);
		((DependencyObject)obj).CoerceValue(InputGestureTextProperty);
	}

	private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarToggleButton)(object)d).UpdateCommonState();
	}

	private static void OnOverflowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarElementProperties.UpdateIsInOverflow(d);
	}

	private static void OnDefaultLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarToggleButton)(object)d).UpdateApplicationViewState();
	}

	private static void OnShowKeyboardAcceleratorTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((AppBarToggleButton)(object)d).UpdateKeyboardAcceleratorTextVisibility();
	}

	private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateApplicationViewState();
	}

	private void UpdateVisualState(bool useTransitions = true)
	{
		ApplyApplicationViewState(useTransitions);
		UpdateCommonState(useTransitions);
	}

	private void UpdateCommonState(bool useTransitions = true)
	{
		if (_vsm != null)
		{
			bool isEnabled = ((UIElement)this).IsEnabled;
			bool flag = ((ToggleButton)this).IsChecked != false;
			string text = ((!isEnabled) ? "Disabled" : (((ButtonBase)this).IsPressed ? "Pressed" : (((UIElement)this).IsMouseOver ? "PointerOver" : (flag ? string.Empty : "Normal"))));
			if (flag)
			{
				text = "Checked" + text;
			}
			if (isEnabled && IsInOverflow)
			{
				text = "Overflow" + text;
			}
			_vsm.CanChangeCommonState = true;
			VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
			_vsm.CanChangeCommonState = false;
		}
	}

	private void UpdateKeyboardAcceleratorTextVisibility(bool useTransitions = true)
	{
		string text = (AppBarElementProperties.GetShowKeyboardAcceleratorText((DependencyObject)(object)this) ? "KeyboardAcceleratorTextVisible" : "KeyboardAcceleratorTextCollapsed");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}
}
