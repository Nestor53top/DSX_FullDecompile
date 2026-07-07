using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("Content")]
public class SplitView : Control
{
	private FrameworkElement _templateRoot;

	private VisualStateGroup _displayModeStates;

	private FrameworkElement _paneRoot;

	private RectangleGeometry _paneClipRectangle;

	private Window _window;

	private bool _isLightDismissActive;

	private bool _isPaneOpening;

	private bool _isPaneClosing;

	private bool _isDisplayModeStateChanging;

	private const string PaneRootName = "PaneRoot";

	private const string DisplayModeStatesName = "DisplayModeStates";

	private const string PaneClipRectangleName = "PaneClipRectangle";

	public static readonly DependencyProperty CompactPaneLengthProperty;

	public static readonly DependencyProperty ContentProperty;

	public static readonly DependencyProperty DisplayModeProperty;

	public static readonly DependencyProperty IsPaneOpenProperty;

	public static readonly DependencyProperty OpenPaneLengthProperty;

	public static readonly DependencyProperty PaneBackgroundProperty;

	public static readonly DependencyProperty PanePlacementProperty;

	public static readonly DependencyProperty PaneProperty;

	private static readonly DependencyPropertyKey TemplateSettingsPropertyKey;

	public static readonly DependencyProperty TemplateSettingsProperty;

	public static readonly DependencyProperty LightDismissOverlayModeProperty;

	private bool IsLightDismissible
	{
		get
		{
			SplitViewDisplayMode displayMode = DisplayMode;
			if (displayMode != SplitViewDisplayMode.Overlay)
			{
				return displayMode == SplitViewDisplayMode.CompactOverlay;
			}
			return true;
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

	public UIElement Content
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(ContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentProperty, (object)value);
		}
	}

	public SplitViewDisplayMode DisplayMode
	{
		get
		{
			return (SplitViewDisplayMode)((DependencyObject)this).GetValue(DisplayModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisplayModeProperty, (object)value);
		}
	}

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

	public Brush PaneBackground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(PaneBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneBackgroundProperty, (object)value);
		}
	}

	public SplitViewPanePlacement PanePlacement
	{
		get
		{
			return (SplitViewPanePlacement)((DependencyObject)this).GetValue(PanePlacementProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PanePlacementProperty, (object)value);
		}
	}

	public UIElement Pane
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(PaneProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PaneProperty, (object)value);
		}
	}

	public SplitViewTemplateSettings TemplateSettings
	{
		get
		{
			return (SplitViewTemplateSettings)((DependencyObject)this).GetValue(TemplateSettingsProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)value);
		}
	}

	public LightDismissOverlayMode LightDismissOverlayMode
	{
		get
		{
			return (LightDismissOverlayMode)((DependencyObject)this).GetValue(LightDismissOverlayModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LightDismissOverlayModeProperty, (object)value);
		}
	}

	public event TypedEventHandler<SplitView, object> PaneOpening;

	public event TypedEventHandler<SplitView, object> PaneOpened;

	public event TypedEventHandler<SplitView, SplitViewPaneClosingEventArgs> PaneClosing;

	public event TypedEventHandler<SplitView, object> PaneClosed;

	internal event DependencyPropertyChangedCallback IsPaneOpenChanged;

	internal event DependencyPropertyChangedCallback DisplayModeChanged;

	internal event DependencyPropertyChangedCallback CompactPaneLengthChanged;

	static SplitView()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		CompactPaneLengthProperty = DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(SplitView), new PropertyMetadata(new PropertyChangedCallback(OnCompactPaneLengthPropertyChanged)));
		ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(SplitView));
		DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(SplitViewDisplayMode), typeof(SplitView), new PropertyMetadata((object)SplitViewDisplayMode.Overlay, new PropertyChangedCallback(OnDisplayModePropertyChanged)));
		IsPaneOpenProperty = DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(SplitView), new PropertyMetadata(new PropertyChangedCallback(OnIsPaneOpenPropertyChanged)));
		OpenPaneLengthProperty = DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(SplitView), new PropertyMetadata(new PropertyChangedCallback(OnOpenPaneLengthPropertyChanged)));
		PaneBackgroundProperty = DependencyProperty.Register("PaneBackground", typeof(Brush), typeof(SplitView));
		PanePlacementProperty = DependencyProperty.Register("PanePlacement", typeof(SplitViewPanePlacement), typeof(SplitView), new PropertyMetadata((object)SplitViewPanePlacement.Left, new PropertyChangedCallback(OnPanePlacementPropertyChanged)));
		PaneProperty = DependencyProperty.Register("Pane", typeof(UIElement), typeof(SplitView));
		TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(SplitViewTemplateSettings), typeof(SplitView), (PropertyMetadata)null);
		TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;
		LightDismissOverlayModeProperty = DependencyProperty.Register("LightDismissOverlayMode", typeof(LightDismissOverlayMode), typeof(SplitView), new PropertyMetadata((object)LightDismissOverlayMode.Auto, new PropertyChangedCallback(OnLightDismissOverlayModePropertyChanged)));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitView), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitView)));
	}

	public SplitView()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		TemplateSettings = new SplitViewTemplateSettings();
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
	}

	public override void OnApplyTemplate()
	{
		if (_displayModeStates != null)
		{
			_displayModeStates.CurrentStateChanging -= OnDisplayModeStatesCurrentStateChanging;
			_displayModeStates.CurrentStateChanged -= OnDisplayModeStatesCurrentStateChanged;
		}
		((FrameworkElement)this).OnApplyTemplate();
		_templateRoot = ((Control)(object)this).GetTemplateRoot();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("PaneRoot");
		_paneRoot = (FrameworkElement)(object)((templateChild is FrameworkElement) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("DisplayModeStates");
		_displayModeStates = (VisualStateGroup)(object)((templateChild2 is VisualStateGroup) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("PaneClipRectangle");
		_paneClipRectangle = (RectangleGeometry)(object)((templateChild3 is RectangleGeometry) ? templateChild3 : null);
		if (_displayModeStates != null)
		{
			_displayModeStates.CurrentStateChanging += OnDisplayModeStatesCurrentStateChanging;
			_displayModeStates.CurrentStateChanged += OnDisplayModeStatesCurrentStateChanged;
			AnimationHelper.DeferTransitions(_displayModeStates);
		}
		UpdateTemplateSettings();
		UpdatePaneClipRectangle();
		UpdateVisualState(useTransitions: false);
		((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
		{
			ReapplyDisplayModeState(waitForDataBinding: false);
		}, (DispatcherPriority)8);
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		UpdatePaneClipRectangle();
	}

	private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateIsLightDismissActive();
	}

	private void OnDisplayModeStatesCurrentStateChanging(object sender, VisualStateChangedEventArgs e)
	{
		_isDisplayModeStateChanging = true;
	}

	private void OnDisplayModeStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
	{
		_isDisplayModeStateChanging = false;
		if (_isPaneOpening)
		{
			_isPaneOpening = false;
			this.PaneOpened?.Invoke(this, null);
		}
		else if (_isPaneClosing)
		{
			_isPaneClosing = false;
			this.PaneClosed?.Invoke(this, null);
		}
	}

	private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (_paneRoot != null)
		{
			Point position = ((MouseEventArgs)e).GetPosition((IInputElement)(object)_paneRoot);
			if (((Point)(ref position)).X >= 0.0 && ((Point)(ref position)).X <= _paneRoot.ActualWidth && ((Point)(ref position)).Y >= 0.0 && ((Point)(ref position)).Y <= _paneRoot.ActualHeight)
			{
				return;
			}
		}
		object originalSource = ((RoutedEventArgs)e).OriginalSource;
		UIElement val = (UIElement)((originalSource is UIElement) ? originalSource : null);
		if ((val == null || !TitleBarControl.GetInsideTitleBar(val)) && IsPaneOpen)
		{
			((RoutedEventArgs)e).Handled = true;
			ClosePane();
		}
	}

	private void OpenPane()
	{
		if (!_isPaneOpening)
		{
			this.PaneOpening?.Invoke(this, null);
			if (UpdateDisplayModeState())
			{
				_isPaneOpening = true;
			}
			else
			{
				this.PaneOpened?.Invoke(this, null);
			}
		}
	}

	private void ClosePane()
	{
		if (_isPaneClosing)
		{
			return;
		}
		TypedEventHandler<SplitView, SplitViewPaneClosingEventArgs> typedEventHandler = this.PaneClosing;
		if (typedEventHandler != null)
		{
			SplitViewPaneClosingEventArgs e = new SplitViewPaneClosingEventArgs();
			typedEventHandler(this, e);
			if (e.Cancel && IsPaneOpen)
			{
				return;
			}
		}
		_isPaneClosing = true;
		if (IsPaneOpen)
		{
			((DependencyObject)this).SetCurrentValue(IsPaneOpenProperty, (object)false);
		}
		if (!UpdateDisplayModeState())
		{
			_isPaneClosing = false;
			this.PaneClosed?.Invoke(this, null);
		}
	}

	private void UpdateTemplateSettings()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		double compactPaneLength = CompactPaneLength;
		double openPaneLength = OpenPaneLength;
		double num = openPaneLength - compactPaneLength;
		SplitViewTemplateSettings templateSettings = TemplateSettings;
		templateSettings.CompactPaneGridLength = new GridLength(compactPaneLength);
		templateSettings.NegativeOpenPaneLength = 0.0 - openPaneLength;
		templateSettings.NegativeOpenPaneLengthMinusCompactLength = 0.0 - num;
		templateSettings.OpenPaneGridLength = new GridLength(openPaneLength);
		templateSettings.OpenPaneLength = openPaneLength;
		templateSettings.OpenPaneLengthMinusCompactLength = num;
		ReapplyDisplayModeState();
	}

	private void UpdatePaneClipRectangle()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (_paneClipRectangle != null)
		{
			_paneClipRectangle.Rect = new Rect(0.0, 0.0, OpenPaneLength, ((FrameworkElement)this).ActualHeight);
		}
	}

	private void UpdateIsLightDismissActive()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		bool flag = ((UIElement)this).IsVisible && IsPaneOpen && IsLightDismissible;
		if (_isLightDismissActive == flag)
		{
			return;
		}
		_isLightDismissActive = flag;
		if (_isLightDismissActive)
		{
			_window = Window.GetWindow((DependencyObject)(object)this);
			if (_window != null)
			{
				((UIElement)_window).PreviewMouseDown += new MouseButtonEventHandler(OnWindowPreviewMouseDown);
			}
		}
		else if (_window != null)
		{
			((UIElement)_window).PreviewMouseDown -= new MouseButtonEventHandler(OnWindowPreviewMouseDown);
			_window = null;
		}
	}

	private bool UpdateDisplayModeState(bool useTransitions = true)
	{
		string text = null;
		SplitViewDisplayMode displayMode = DisplayMode;
		if (!IsPaneOpen)
		{
			text = ((displayMode != SplitViewDisplayMode.CompactOverlay && displayMode != SplitViewDisplayMode.CompactInline) ? "Closed" : ((PanePlacement != SplitViewPanePlacement.Left) ? "ClosedCompactRight" : "ClosedCompactLeft"));
		}
		else
		{
			switch (displayMode)
			{
			case SplitViewDisplayMode.Overlay:
				text = ((PanePlacement != SplitViewPanePlacement.Left) ? "OpenOverlayRight" : "OpenOverlayLeft");
				break;
			case SplitViewDisplayMode.Inline:
			case SplitViewDisplayMode.CompactInline:
				text = ((PanePlacement != SplitViewPanePlacement.Left) ? "OpenInlineRight" : "OpenInlineLeft");
				break;
			case SplitViewDisplayMode.CompactOverlay:
				text = ((PanePlacement != SplitViewPanePlacement.Left) ? "OpenCompactOverlayRight" : "OpenCompactOverlayLeft");
				break;
			}
		}
		return VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void UpdateOverlayVisibilityState(bool useTransitions = true)
	{
		string text = ((!IsPaneOpen || !IsLightDismissible || LightDismissOverlayMode != LightDismissOverlayMode.On) ? "OverlayNotVisible" : "OverlayVisible");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void UpdateVisualState(bool useTransitions = true)
	{
		UpdateDisplayModeState(useTransitions);
		UpdateOverlayVisibilityState(useTransitions);
	}

	private void ReapplyDisplayModeState(bool waitForDataBinding = true)
	{
		if (_isDisplayModeStateChanging)
		{
			return;
		}
		VisualStateGroup displayModeStates = _displayModeStates;
		object obj;
		if (displayModeStates == null)
		{
			obj = null;
		}
		else
		{
			VisualState currentState = displayModeStates.CurrentState;
			obj = ((currentState != null) ? currentState.Storyboard : null);
		}
		Storyboard val = (Storyboard)obj;
		if (val != null && _templateRoot != null && !((Freezable)val).CanFreeze)
		{
			if (waitForDataBinding)
			{
				DispatcherHelper.DoEvents((DispatcherPriority)8);
			}
			val.Begin(_templateRoot, true);
		}
	}

	private static void OnCompactPaneLengthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnCompactPaneLengthPropertyChanged(args);
	}

	private void OnCompactPaneLengthPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateTemplateSettings();
		this.CompactPaneLengthChanged?.Invoke((DependencyObject)(object)this, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
	}

	private static void OnDisplayModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnDisplayModePropertyChanged(args);
	}

	private void OnDisplayModePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateIsLightDismissActive();
		UpdateVisualState();
		this.DisplayModeChanged?.Invoke((DependencyObject)(object)this, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
	}

	private static void OnIsPaneOpenPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnIsPaneOpenPropertyChanged(args);
	}

	private void OnIsPaneOpenPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if ((bool)((DependencyPropertyChangedEventArgs)(ref args)).NewValue)
		{
			_isPaneClosing = false;
			OpenPane();
		}
		else
		{
			_isPaneOpening = false;
			ClosePane();
		}
		UpdateIsLightDismissActive();
		UpdateOverlayVisibilityState();
		this.IsPaneOpenChanged?.Invoke((DependencyObject)(object)this, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
	}

	private static void OnOpenPaneLengthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnOpenPaneLengthPropertyChanged(args);
	}

	private void OnOpenPaneLengthPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateTemplateSettings();
		UpdatePaneClipRectangle();
	}

	private static void OnPanePlacementPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnPanePlacementPropertyChanged(args);
	}

	private void OnPanePlacementPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateDisplayModeState();
	}

	private static void OnLightDismissOverlayModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SplitView)(object)sender).OnLightDismissOverlayModePropertyChanged(args);
	}

	private void OnLightDismissOverlayModePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateOverlayVisibilityState();
	}
}
