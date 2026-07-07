using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernWpf.Controls.Primitives;

[TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_LeftSystemOverlay", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_RightSystemOverlay", Type = typeof(FrameworkElement))]
[StyleTypedProperty(Property = "ButtonStyle", StyleTargetType = typeof(TitleBarButton))]
[StyleTypedProperty(Property = "BackButtonStyle", StyleTargetType = typeof(TitleBarButton))]
public class TitleBarControl : Control
{
	private class GoBackCommand : ICommand
	{
		private readonly TitleBarControl _owner;

		public event EventHandler CanExecuteChanged;

		public GoBackCommand(TitleBarControl owner)
		{
			_owner = owner;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_owner.InvokeBack();
		}
	}

	private const string BackButtonName = "PART_BackButton";

	private const string LeftSystemOverlayName = "PART_LeftSystemOverlay";

	private const string RightSystemOverlayName = "PART_RightSystemOverlay";

	private Window _parentWindow;

	private KeyBinding _altLeftBinding;

	public static readonly DependencyProperty IsActiveProperty;

	public static readonly DependencyProperty InactiveBackgroundProperty;

	public static readonly DependencyProperty InactiveForegroundProperty;

	public static readonly DependencyProperty ButtonStyleProperty;

	public static readonly DependencyProperty TitleProperty;

	public static readonly DependencyProperty IconProperty;

	private static readonly DependencyPropertyKey ActualIconPropertyKey;

	public static readonly DependencyProperty ActualIconProperty;

	public static readonly DependencyProperty IsIconVisibleProperty;

	public static readonly DependencyProperty IsBackButtonVisibleProperty;

	public static readonly DependencyProperty IsBackEnabledProperty;

	public static readonly DependencyProperty BackButtonCommandProperty;

	public static readonly DependencyProperty BackButtonCommandParameterProperty;

	public static readonly DependencyProperty BackButtonCommandTargetProperty;

	public static readonly DependencyProperty BackButtonStyleProperty;

	public static readonly DependencyProperty ExtendViewIntoTitleBarProperty;

	internal static readonly DependencyProperty InsideTitleBarProperty;

	public bool IsActive
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsActiveProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsActiveProperty, (object)value);
		}
	}

	public Brush InactiveBackground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(InactiveBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InactiveBackgroundProperty, (object)value);
		}
	}

	public Brush InactiveForeground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(InactiveForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InactiveForegroundProperty, (object)value);
		}
	}

	public Style ButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(ButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ButtonStyleProperty, (object)value);
		}
	}

	public string Title
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TitleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TitleProperty, (object)value);
		}
	}

	public ImageSource Icon
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(IconProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IconProperty, (object)value);
		}
	}

	public ImageSource ActualIcon
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(ActualIconProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(ActualIconPropertyKey, (object)value);
		}
	}

	public bool IsIconVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsIconVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsIconVisibleProperty, (object)value);
		}
	}

	public bool IsBackButtonVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsBackButtonVisibleProperty);
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

	public ICommand BackButtonCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(BackButtonCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BackButtonCommandProperty, (object)value);
		}
	}

	public object BackButtonCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(BackButtonCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BackButtonCommandParameterProperty, value);
		}
	}

	public IInputElement BackButtonCommandTarget
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (IInputElement)((DependencyObject)this).GetValue(BackButtonCommandTargetProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BackButtonCommandTargetProperty, (object)value);
		}
	}

	public Style BackButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(BackButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BackButtonStyleProperty, (object)value);
		}
	}

	public bool ExtendViewIntoTitleBar
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ExtendViewIntoTitleBarProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ExtendViewIntoTitleBarProperty, (object)value);
		}
	}

	private Button BackButton { get; set; }

	private FrameworkElement LeftSystemOverlay { get; set; }

	private FrameworkElement RightSystemOverlay { get; set; }

	static TitleBarControl()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TitleBarControl), new PropertyMetadata((object)false));
		InactiveBackgroundProperty = TitleBar.InactiveBackgroundProperty.AddOwner(typeof(TitleBarControl));
		InactiveForegroundProperty = TitleBar.InactiveForegroundProperty.AddOwner(typeof(TitleBarControl));
		ButtonStyleProperty = TitleBar.ButtonStyleProperty.AddOwner(typeof(TitleBarControl));
		TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleBarControl), new PropertyMetadata((object)string.Empty));
		IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TitleBarControl), new PropertyMetadata(new PropertyChangedCallback(OnIconChanged)));
		ActualIconPropertyKey = DependencyProperty.RegisterReadOnly("ActualIcon", typeof(ImageSource), typeof(TitleBarControl), (PropertyMetadata)null);
		ActualIconProperty = ActualIconPropertyKey.DependencyProperty;
		IsIconVisibleProperty = TitleBar.IsIconVisibleProperty.AddOwner(typeof(TitleBarControl));
		IsBackButtonVisibleProperty = TitleBar.IsBackButtonVisibleProperty.AddOwner(typeof(TitleBarControl));
		IsBackEnabledProperty = TitleBar.IsBackEnabledProperty.AddOwner(typeof(TitleBarControl));
		BackButtonCommandProperty = TitleBar.BackButtonCommandProperty.AddOwner(typeof(TitleBarControl));
		BackButtonCommandParameterProperty = TitleBar.BackButtonCommandParameterProperty.AddOwner(typeof(TitleBarControl));
		BackButtonCommandTargetProperty = TitleBar.BackButtonCommandTargetProperty.AddOwner(typeof(TitleBarControl));
		BackButtonStyleProperty = TitleBar.BackButtonStyleProperty.AddOwner(typeof(TitleBarControl));
		ExtendViewIntoTitleBarProperty = TitleBar.ExtendViewIntoTitleBarProperty.AddOwner(typeof(TitleBarControl));
		InsideTitleBarProperty = DependencyProperty.RegisterAttached("InsideTitleBar", typeof(bool), typeof(TitleBarControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)32));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(TitleBarControl)));
	}

	public TitleBarControl()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		((UIElement)this).CommandBindings.Add(new CommandBinding((ICommand)SystemCommands.MinimizeWindowCommand, new ExecutedRoutedEventHandler(MinimizeWindow)));
		((UIElement)this).CommandBindings.Add(new CommandBinding((ICommand)SystemCommands.MaximizeWindowCommand, new ExecutedRoutedEventHandler(MaximizeWindow)));
		((UIElement)this).CommandBindings.Add(new CommandBinding((ICommand)SystemCommands.RestoreWindowCommand, new ExecutedRoutedEventHandler(RestoreWindow)));
		((UIElement)this).CommandBindings.Add(new CommandBinding((ICommand)SystemCommands.CloseWindowCommand, new ExecutedRoutedEventHandler(CloseWindow)));
		SetInsideTitleBar((UIElement)(object)this, value: true);
	}

	private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((TitleBarControl)(object)d).UpdateActualIcon();
	}

	private void UpdateActualIcon()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (Icon != null)
		{
			ActualIcon = Icon;
			return;
		}
		ImageSource actualIcon = null;
		IntPtr[] array = new IntPtr[1];
		IconHelper.GetDefaultIconHandles(null, array);
		IntPtr intPtr = array[0];
		if (intPtr != IntPtr.Zero)
		{
			try
			{
				actualIcon = (ImageSource)(object)Imaging.CreateBitmapSourceFromHIcon(intPtr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				IconHelper.DestroyIcon(intPtr);
			}
		}
		ActualIcon = actualIcon;
	}

	internal static bool GetInsideTitleBar(UIElement element)
	{
		return (bool)((DependencyObject)element).GetValue(InsideTitleBarProperty);
	}

	internal static void SetInsideTitleBar(UIElement element, bool value)
	{
		((DependencyObject)element).SetValue(InsideTitleBarProperty, (object)value);
	}

	public override void OnApplyTemplate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		if (BackButton != null)
		{
			((ButtonBase)BackButton).Click -= new RoutedEventHandler(OnBackButtonClick);
		}
		if (LeftSystemOverlay != null)
		{
			LeftSystemOverlay.SizeChanged -= new SizeChangedEventHandler(OnLeftSystemOverlaySizeChanged);
		}
		if (RightSystemOverlay != null)
		{
			RightSystemOverlay.SizeChanged -= new SizeChangedEventHandler(OnRightSystemOverlaySizeChanged);
		}
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("PART_BackButton");
		BackButton = (Button)(object)((templateChild is Button) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("PART_LeftSystemOverlay");
		LeftSystemOverlay = (FrameworkElement)(object)((templateChild2 is FrameworkElement) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("PART_RightSystemOverlay");
		RightSystemOverlay = (FrameworkElement)(object)((templateChild3 is FrameworkElement) ? templateChild3 : null);
		if (BackButton != null)
		{
			((ButtonBase)BackButton).Click += new RoutedEventHandler(OnBackButtonClick);
		}
		if (LeftSystemOverlay != null)
		{
			LeftSystemOverlay.SizeChanged += new SizeChangedEventHandler(OnLeftSystemOverlaySizeChanged);
			UpdateSystemOverlayLeftInset(LeftSystemOverlay.ActualWidth);
		}
		if (RightSystemOverlay != null)
		{
			RightSystemOverlay.SizeChanged += new SizeChangedEventHandler(OnRightSystemOverlaySizeChanged);
			UpdateSystemOverlayRightInset(RightSystemOverlay.ActualWidth);
		}
	}

	protected override void OnInitialized(EventArgs e)
	{
		UpdateActualIcon();
		((FrameworkElement)this).OnInitialized(e);
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		if (_parentWindow != null && _altLeftBinding != null)
		{
			((UIElement)_parentWindow).InputBindings.Remove((InputBinding)(object)_altLeftBinding);
			_altLeftBinding = null;
		}
		((FrameworkElement)this).OnVisualParentChanged(oldParent);
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		_parentWindow = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (_parentWindow != null)
		{
			_altLeftBinding = new KeyBinding((ICommand)new GoBackCommand(this), (Key)23, (ModifierKeys)1);
			((UIElement)_parentWindow).InputBindings.Add((InputBinding)(object)_altLeftBinding);
		}
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnRenderSizeChanged(sizeInfo);
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			Size newSize = sizeInfo.NewSize;
			TitleBar.SetHeight(val, ((Size)(ref newSize)).Height);
		}
	}

	private void OnBackButtonClick(object sender, RoutedEventArgs e)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			TitleBar.RaiseBackRequested(val);
		}
	}

	private void OnLeftSystemOverlaySizeChanged(object sender, SizeChangedEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Size newSize = e.NewSize;
		UpdateSystemOverlayLeftInset(((Size)(ref newSize)).Width);
	}

	private void OnRightSystemOverlaySizeChanged(object sender, SizeChangedEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Size newSize = e.NewSize;
		UpdateSystemOverlayRightInset(((Size)(ref newSize)).Width);
	}

	private void UpdateSystemOverlayLeftInset(double value)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			TitleBar.SetSystemOverlayLeftInset(val, value);
		}
	}

	private void UpdateSystemOverlayRightInset(double value)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			TitleBar.SetSystemOverlayRightInset(val, value);
		}
	}

	private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			SystemCommands.MinimizeWindow(val);
		}
	}

	private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			SystemCommands.MaximizeWindow(val);
		}
	}

	private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			SystemCommands.RestoreWindow(val);
		}
	}

	private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
	{
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		Window val = (Window)(object)((templatedParent is Window) ? templatedParent : null);
		if (val != null)
		{
			SystemCommands.CloseWindow(val);
		}
	}

	private void InvokeBack()
	{
		if (BackButton != null && ((UIElement)BackButton).IsEnabled)
		{
			AutomationPeer obj = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)BackButton);
			object obj2 = ((obj != null) ? obj.GetPattern((PatternInterface)0) : null);
			object obj3 = ((obj2 is IInvokeProvider) ? obj2 : null);
			if (obj3 != null)
			{
				((IInvokeProvider)obj3).Invoke();
			}
		}
	}
}
