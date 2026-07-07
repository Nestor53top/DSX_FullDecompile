using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("Header")]
[TemplatePart(Name = "HeaderContentPresenter", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "SwitchKnobBounds", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "SwitchKnob", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "KnobTranslateTransform", Type = typeof(TranslateTransform))]
[TemplatePart(Name = "SwitchThumb", Type = typeof(Thumb))]
[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
[TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
[TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
[TemplateVisualState(GroupName = "ContentStates", Name = "OffContent")]
[TemplateVisualState(GroupName = "ContentStates", Name = "OnContent")]
[TemplateVisualState(GroupName = "ToggleStates", Name = "Dragging")]
[TemplateVisualState(GroupName = "ToggleStates", Name = "Off")]
[TemplateVisualState(GroupName = "ToggleStates", Name = "On")]
public class ToggleSwitch : Control
{
	private const string ContentStatesGroup = "ContentStates";

	private const string OffContentState = "OffContent";

	private const string OnContentState = "OnContent";

	private const string ToggleStatesGroup = "ToggleStates";

	private const string DraggingState = "Dragging";

	private const string OffState = "Off";

	private const string OnState = "On";

	private const double _offTranslation = 0.0;

	private double _onTranslation;

	private double _startTranslation;

	private bool _wasDragged;

	private BitmapCache _bitmapCache;

	public static readonly RoutedEvent ToggledEvent;

	public static readonly DependencyProperty HeaderProperty;

	public static readonly DependencyProperty HeaderTemplateProperty;

	public static readonly DependencyProperty IsOnProperty;

	public static readonly DependencyProperty OffContentProperty;

	public static readonly DependencyProperty OffContentTemplateProperty;

	public static readonly DependencyProperty OnContentProperty;

	public static readonly DependencyProperty OnContentTemplateProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

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

	public bool IsOn
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsOnProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsOnProperty, (object)value);
		}
	}

	public object OffContent
	{
		get
		{
			return ((DependencyObject)this).GetValue(OffContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OffContentProperty, value);
		}
	}

	public DataTemplate OffContentTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(OffContentTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OffContentTemplateProperty, (object)value);
		}
	}

	public object OnContent
	{
		get
		{
			return ((DependencyObject)this).GetValue(OnContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OnContentProperty, value);
		}
	}

	public DataTemplate OnContentTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(OnContentTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OnContentTemplateProperty, (object)value);
		}
	}

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

	private ContentPresenter HeaderContentPresenter { get; set; }

	private FrameworkElement SwitchKnobBounds { get; set; }

	private FrameworkElement SwitchKnob { get; set; }

	private TranslateTransform KnobTranslateTransform { get; set; }

	private Thumb SwitchThumb { get; set; }

	public event RoutedEventHandler Toggled
	{
		add
		{
			((UIElement)this).AddHandler(ToggledEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(ToggledEvent, (Delegate)(object)value);
		}
	}

	static ToggleSwitch()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		ToggledEvent = EventManager.RegisterRoutedEvent("Toggled", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(ToggleSwitch));
		HeaderProperty = ControlHelper.HeaderProperty.AddOwner(typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderChanged)));
		HeaderTemplateProperty = ControlHelper.HeaderTemplateProperty.AddOwner(typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderTemplateChanged)));
		IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)1280, new PropertyChangedCallback(OnIsOnChanged)));
		OffContentProperty = DependencyProperty.Register("OffContent", typeof(object), typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOffContentChanged)));
		OffContentTemplateProperty = DependencyProperty.Register("OffContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch), (PropertyMetadata)null);
		OnContentProperty = DependencyProperty.Register("OnContent", typeof(object), typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOnContentChanged)));
		OnContentTemplateProperty = DependencyProperty.Register("OnContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch), (PropertyMetadata)null);
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(ToggleSwitch));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(ToggleSwitch));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(ToggleSwitch));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ToggleSwitch)));
		EventManager.RegisterClassHandler(typeof(ToggleSwitch), UIElement.MouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
	}

	public ToggleSwitch()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		((DependencyObject)this).SetCurrentValue(OffContentProperty, (object)Strings.ToggleSwitchOff);
		((DependencyObject)this).SetCurrentValue(OnContentProperty, (object)Strings.ToggleSwitchOn);
		((UIElement)this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
	}

	private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ToggleSwitch obj = (ToggleSwitch)(object)d;
		obj.UpdateHeaderContentPresenterVisibility();
		obj.OnHeaderChanged(((DependencyPropertyChangedEventArgs)(ref e)).OldValue, ((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	protected virtual void OnHeaderChanged(object oldContent, object newContent)
	{
	}

	private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ToggleSwitch)(object)d).UpdateHeaderContentPresenterVisibility();
	}

	private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ToggleSwitch obj = (ToggleSwitch)(object)d;
		obj.OnToggled();
		obj.UpdateVisualStates(useTransitions: true);
	}

	private static void OnOffContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ToggleSwitch)(object)d).OnOffContentChanged(((DependencyPropertyChangedEventArgs)(ref e)).OldValue, ((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	protected virtual void OnOffContentChanged(object oldContent, object newContent)
	{
	}

	private static void OnOnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ToggleSwitch)(object)d).OnOffContentChanged(((DependencyPropertyChangedEventArgs)(ref e)).OldValue, ((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	protected virtual void OnOnContentChanged(object oldContent, object newContent)
	{
	}

	public override void OnApplyTemplate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
		{
			SwitchThumb.DragStarted -= new DragStartedEventHandler(OnSwitchThumbDragStarted);
			SwitchThumb.DragDelta -= new DragDeltaEventHandler(OnSwitchThumbDragDelta);
			SwitchThumb.DragCompleted -= new DragCompletedEventHandler(OnSwitchThumbDragCompleted);
			((DependencyObject)SwitchThumb).ClearValue(UIElement.CacheModeProperty);
		}
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("HeaderContentPresenter");
		HeaderContentPresenter = (ContentPresenter)(object)((templateChild is ContentPresenter) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("SwitchKnobBounds");
		SwitchKnobBounds = (FrameworkElement)(object)((templateChild2 is FrameworkElement) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("SwitchKnob");
		SwitchKnob = (FrameworkElement)(object)((templateChild3 is FrameworkElement) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("KnobTranslateTransform");
		KnobTranslateTransform = (TranslateTransform)(object)((templateChild4 is TranslateTransform) ? templateChild4 : null);
		DependencyObject templateChild5 = ((FrameworkElement)this).GetTemplateChild("SwitchThumb");
		SwitchThumb = (Thumb)(object)((templateChild5 is Thumb) ? templateChild5 : null);
		if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
		{
			SwitchThumb.DragStarted += new DragStartedEventHandler(OnSwitchThumbDragStarted);
			SwitchThumb.DragDelta += new DragDeltaEventHandler(OnSwitchThumbDragDelta);
			SwitchThumb.DragCompleted += new DragCompletedEventHandler(OnSwitchThumbDragCompleted);
			if (_bitmapCache == null)
			{
				DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)this);
				_bitmapCache = new BitmapCache(((DpiScale)(ref dpi)).PixelsPerDip);
			}
			((UIElement)SwitchThumb).CacheMode = (CacheMode)(object)_bitmapCache;
		}
		UpdateHeaderContentPresenterVisibility();
		UpdateVisualStates(useTransitions: false);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new ToggleSwitchAutomationPeer(this);
	}

	protected virtual void OnToggled()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)this).RaiseEvent(new RoutedEventArgs(ToggledEvent));
		AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
		if (val != null)
		{
			ToggleState val2 = (ToggleState)(IsOn ? 1 : 0);
			ToggleState val3 = (ToggleState)((int)val2 != 1);
			val.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, (object)val3, (object)val2);
		}
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == UIElement.IsMouseOverProperty)
		{
			UpdateVisualStates(useTransitions: true);
		}
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnRenderSizeChanged(sizeInfo);
		if (SwitchKnobBounds != null && SwitchKnob != null)
		{
			double num = SwitchKnobBounds.ActualWidth - SwitchKnob.ActualWidth;
			Thickness margin = SwitchKnob.Margin;
			double num2 = num - ((Thickness)(ref margin)).Left;
			margin = SwitchKnob.Margin;
			_onTranslation = num2 - ((Thickness)(ref margin)).Right;
		}
	}

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Visual)this).OnDpiChanged(oldDpi, newDpi);
		if (_bitmapCache != null)
		{
			_bitmapCache.RenderAtScale = ((DpiScale)(ref newDpi)).PixelsPerDip;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Key == 18)
		{
			((RoutedEventArgs)e).Handled = true;
			Toggle();
		}
		((UIElement)this).OnKeyUp(e);
	}

	private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
		if (!((UIElement)toggleSwitch).IsKeyboardFocusWithin)
		{
			((RoutedEventArgs)e).Handled = ((UIElement)toggleSwitch).Focus() || ((RoutedEventArgs)e).Handled;
		}
	}

	private void OnSwitchThumbDragStarted(object sender, DragStartedEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
		_startTranslation = KnobTranslateTransform.X;
		UpdateVisualStates(useTransitions: true);
		KnobTranslateTransform.X = _startTranslation;
	}

	private void OnSwitchThumbDragDelta(object sender, DragDeltaEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
		if (e.HorizontalChange != 0.0)
		{
			_wasDragged = true;
			double val = _startTranslation + e.HorizontalChange;
			KnobTranslateTransform.X = Math.Max(0.0, Math.Min(_onTranslation, val));
		}
	}

	private void OnSwitchThumbDragCompleted(object sender, DragCompletedEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
		bool flag = false;
		if (_wasDragged)
		{
			double num = (IsOn ? _onTranslation : 0.0);
			if (KnobTranslateTransform.X != num)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			Toggle();
		}
		_wasDragged = false;
	}

	private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateVisualStates(useTransitions: true);
	}

	private void UpdateHeaderContentPresenterVisibility()
	{
		if (HeaderContentPresenter != null)
		{
			bool flag = !ControlHelper.IsNullOrEmptyString(Header) || HeaderTemplate != null;
			((UIElement)HeaderContentPresenter).Visibility = (Visibility)((!flag) ? 2 : 0);
		}
	}

	private void UpdateVisualStates(bool useTransitions)
	{
		string text = ((!((UIElement)this).IsEnabled) ? "Disabled" : ((!((UIElement)this).IsMouseOver) ? "Normal" : "MouseOver"));
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
		text = ((SwitchThumb == null || !SwitchThumb.IsDragging) ? (IsOn ? "On" : "Off") : "Dragging");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
		VisualStateManager.GoToState((FrameworkElement)(object)this, IsOn ? "OnContent" : "OffContent", useTransitions);
	}

	internal void Toggle()
	{
		((DependencyObject)this).SetCurrentValue(IsOnProperty, (object)(!IsOn));
	}
}
