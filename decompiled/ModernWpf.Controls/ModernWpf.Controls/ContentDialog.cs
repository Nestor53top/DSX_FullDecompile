using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ModernWpf.Controls;

[TemplatePart(Name = "Container", Type = typeof(Border))]
[TemplatePart(Name = "LayoutRoot", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PrimaryButton", Type = typeof(Button))]
[TemplatePart(Name = "SecondaryButton", Type = typeof(Button))]
[TemplatePart(Name = "CloseButton", Type = typeof(Button))]
[TemplateVisualState(GroupName = "DialogShowingStates", Name = "DialogHidden")]
[TemplateVisualState(GroupName = "DialogShowingStates", Name = "DialogShowing")]
[TemplateVisualState(GroupName = "DialogShowingStates", Name = "DialogShowingWithoutSmokeLayer")]
[TemplateVisualState(GroupName = "DialogSizingStates", Name = "DefaultDialogSizing")]
[TemplateVisualState(GroupName = "DialogSizingStates", Name = "FullDialogSizing")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "AllVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "NoneVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "PrimaryVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "SecondaryVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "CloseVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "PrimaryAndSecondaryVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "PrimaryAndCloseVisible")]
[TemplateVisualState(GroupName = "ButtonsVisibilityStates", Name = "SecondaryAndCloseVisible")]
[TemplateVisualState(GroupName = "DefaultButtonStates", Name = "NoDefaultButton")]
[TemplateVisualState(GroupName = "DefaultButtonStates", Name = "PrimaryAsDefaultButton")]
[TemplateVisualState(GroupName = "DefaultButtonStates", Name = "SecondaryAsDefaultButton")]
[TemplateVisualState(GroupName = "DefaultButtonStates", Name = "CloseAsDefaultButton")]
[TemplateVisualState(GroupName = "DialogBorderStates", Name = "NoBorder")]
[TemplateVisualState(GroupName = "DialogBorderStates", Name = "AccentColorBorder")]
[StyleTypedProperty(Property = "PrimaryButtonStyle", StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = "SecondaryButtonStyle", StyleTargetType = typeof(Button))]
[StyleTypedProperty(Property = "CloseButtonStyle", StyleTargetType = typeof(Button))]
public class ContentDialog : ContentControl
{
	private class ContentDialogAdorner : Adorner
	{
		private UIElement _child;

		public UIElement Child
		{
			get
			{
				return _child;
			}
			set
			{
				if (_child != value)
				{
					if (_child != null)
					{
						((Visual)this).RemoveVisualChild((Visual)(object)_child);
					}
					_child = value;
					if (_child != null)
					{
						((Visual)this).AddVisualChild((Visual)(object)_child);
					}
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get
			{
				if (_child == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public ContentDialogAdorner(UIElement adornedElement, UIElement child)
			: base(adornedElement)
		{
			Child = child ?? throw new ArgumentNullException("child");
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index == 0 && _child != null)
			{
				return (Visual)(object)_child;
			}
			throw new ArgumentOutOfRangeException("index");
		}

		protected override Size MeasureOverride(Size constraint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Size result = (constraint = ((Adorner)this).AdornedElement.RenderSize);
			UIElement child = Child;
			if (child != null)
			{
				child.Measure(constraint);
				return result;
			}
			return result;
		}

		protected override Size ArrangeOverride(Size size)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Size val = ((FrameworkElement)this).ArrangeOverride(size);
			UIElement child = Child;
			if (child != null)
			{
				child.Arrange(new Rect(default(Point), val));
			}
			return val;
		}
	}

	public static readonly DependencyProperty TitleProperty;

	public static readonly DependencyProperty TitleTemplateProperty;

	public static readonly DependencyProperty PrimaryButtonTextProperty;

	public static readonly DependencyProperty PrimaryButtonCommandProperty;

	public static readonly DependencyProperty PrimaryButtonCommandParameterProperty;

	public static readonly DependencyProperty PrimaryButtonStyleProperty;

	public static readonly DependencyProperty IsPrimaryButtonEnabledProperty;

	public static readonly DependencyProperty SecondaryButtonTextProperty;

	public static readonly DependencyProperty SecondaryButtonCommandProperty;

	public static readonly DependencyProperty SecondaryButtonCommandParameterProperty;

	public static readonly DependencyProperty SecondaryButtonStyleProperty;

	public static readonly DependencyProperty IsSecondaryButtonEnabledProperty;

	public static readonly DependencyProperty CloseButtonTextProperty;

	public static readonly DependencyProperty CloseButtonCommandProperty;

	public static readonly DependencyProperty CloseButtonCommandParameterProperty;

	public static readonly DependencyProperty CloseButtonStyleProperty;

	public static readonly DependencyProperty DefaultButtonProperty;

	public static readonly DependencyProperty FullSizeDesiredProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty IsShadowEnabledProperty;

	private static readonly DependencyProperty OpenDialogProperty;

	private const string DialogShowingStatesGroup = "DialogShowingStates";

	private const string DialogHiddenState = "DialogHidden";

	private const string DialogShowingState = "DialogShowing";

	private const string DialogShowingWithoutSmokeLayerState = "DialogShowingWithoutSmokeLayer";

	private const string DialogSizingStatesGroup = "DialogSizingStates";

	private const string DefaultDialogSizingState = "DefaultDialogSizing";

	private const string FullDialogSizingState = "FullDialogSizing";

	private const string ButtonsVisibilityStatesGroup = "ButtonsVisibilityStates";

	private const string AllVisibleState = "AllVisible";

	private const string NoneVisibleState = "NoneVisible";

	private const string PrimaryVisibleState = "PrimaryVisible";

	private const string SecondaryVisibleState = "SecondaryVisible";

	private const string CloseVisibleState = "CloseVisible";

	private const string PrimaryAndSecondaryVisibleState = "PrimaryAndSecondaryVisible";

	private const string PrimaryAndCloseVisibleState = "PrimaryAndCloseVisible";

	private const string SecondaryAndCloseVisibleState = "SecondaryAndCloseVisible";

	private const string DefaultButtonStatesGroup = "DefaultButtonStates";

	private const string NoDefaultButtonState = "NoDefaultButton";

	private const string PrimaryAsDefaultButtonState = "PrimaryAsDefaultButton";

	private const string SecondaryAsDefaultButtonState = "SecondaryAsDefaultButton";

	private const string CloseAsDefaultButtonState = "CloseAsDefaultButton";

	private const string DialogBorderStatesGroup = "DialogBorderStates";

	private const string NoBorderState = "NoBorder";

	private const string AccentColorBorderState = "AccentColorBorder";

	private TaskCompletionSource<ContentDialogResult> m_showTcs;

	private TaskCompletionSource<bool> m_activatedTcs;

	private ContentDialogAdorner m_adorner;

	private AdornerLayer m_adornerLayer;

	private Popup m_popup;

	private bool m_opening;

	private bool m_isShowing;

	private bool m_isShowingInPlace;

	private Window m_openDialogOwner;

	private ContentDialogResult m_result;

	private readonly DispatcherTimer m_closeTimer;

	private WeakReference<IInputElement> m_weakRefToPreviousFocus;

	public object Title
	{
		get
		{
			return ((DependencyObject)this).GetValue(TitleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TitleProperty, value);
		}
	}

	public DataTemplate TitleTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(TitleTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TitleTemplateProperty, (object)value);
		}
	}

	public string PrimaryButtonText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PrimaryButtonTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PrimaryButtonTextProperty, (object)value);
		}
	}

	public ICommand PrimaryButtonCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(PrimaryButtonCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PrimaryButtonCommandProperty, (object)value);
		}
	}

	public object PrimaryButtonCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(PrimaryButtonCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PrimaryButtonCommandParameterProperty, value);
		}
	}

	public Style PrimaryButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(PrimaryButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PrimaryButtonStyleProperty, (object)value);
		}
	}

	public bool IsPrimaryButtonEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsPrimaryButtonEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsPrimaryButtonEnabledProperty, (object)value);
		}
	}

	public string SecondaryButtonText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(SecondaryButtonTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SecondaryButtonTextProperty, (object)value);
		}
	}

	public ICommand SecondaryButtonCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(SecondaryButtonCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SecondaryButtonCommandProperty, (object)value);
		}
	}

	public object SecondaryButtonCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(SecondaryButtonCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SecondaryButtonCommandParameterProperty, value);
		}
	}

	public Style SecondaryButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(SecondaryButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SecondaryButtonStyleProperty, (object)value);
		}
	}

	public bool IsSecondaryButtonEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSecondaryButtonEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsSecondaryButtonEnabledProperty, (object)value);
		}
	}

	public string CloseButtonText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(CloseButtonTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CloseButtonTextProperty, (object)value);
		}
	}

	public ICommand CloseButtonCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(CloseButtonCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CloseButtonCommandProperty, (object)value);
		}
	}

	public object CloseButtonCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(CloseButtonCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CloseButtonCommandParameterProperty, value);
		}
	}

	public Style CloseButtonStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(CloseButtonStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CloseButtonStyleProperty, (object)value);
		}
	}

	public ContentDialogButton DefaultButton
	{
		get
		{
			return (ContentDialogButton)((DependencyObject)this).GetValue(DefaultButtonProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DefaultButtonProperty, (object)value);
		}
	}

	public bool FullSizeDesired
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(FullSizeDesiredProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FullSizeDesiredProperty, (object)value);
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

	public bool IsShadowEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsShadowEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsShadowEnabledProperty, (object)value);
		}
	}

	public Window Owner { get; set; }

	private Window ActualOwner => Owner ?? SharedHelpers.GetActiveWindow();

	private Border Container { get; set; }

	private FrameworkElement LayoutRoot { get; set; }

	private Button PrimaryButton { get; set; }

	private Button SecondaryButton { get; set; }

	private Button CloseButton { get; set; }

	private bool IsShowing
	{
		get
		{
			return m_isShowing;
		}
		set
		{
			if (m_isShowing == value)
			{
				return;
			}
			m_isShowing = value;
			m_opening = m_isShowing;
			if (m_isShowing)
			{
				if (Keyboard.FocusedElement != null)
				{
					m_weakRefToPreviousFocus = new WeakReference<IInputElement>(Keyboard.FocusedElement);
				}
			}
			else
			{
				if (m_isShowingInPlace)
				{
					m_isShowingInPlace = false;
				}
				else if (m_openDialogOwner != null)
				{
					((DependencyObject)m_openDialogOwner).ClearValue(OpenDialogProperty);
					m_openDialogOwner = null;
				}
				m_closeTimer.Start();
				if (m_weakRefToPreviousFocus != null)
				{
					if (m_weakRefToPreviousFocus.TryGetTarget(out var target))
					{
						target.Focus();
					}
					m_weakRefToPreviousFocus = null;
				}
			}
			UpdateDialogShowingStates(useTransitions: true);
		}
	}

	public event TypedEventHandler<ContentDialog, ContentDialogOpenedEventArgs> Opened;

	public event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> Closing;

	public event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> Closed;

	public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> PrimaryButtonClick;

	public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> SecondaryButtonClick;

	public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> CloseButtonClick;

	static ContentDialog()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Expected O, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Expected O, but got Unknown
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(ContentDialog), (PropertyMetadata)null);
		TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(ContentDialog), (PropertyMetadata)null);
		PrimaryButtonTextProperty = DependencyProperty.Register("PrimaryButtonText", typeof(string), typeof(ContentDialog), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnButtonTextChanged)));
		PrimaryButtonCommandProperty = DependencyProperty.Register("PrimaryButtonCommand", typeof(ICommand), typeof(ContentDialog), (PropertyMetadata)null);
		PrimaryButtonCommandParameterProperty = DependencyProperty.Register("PrimaryButtonCommandParameter", typeof(object), typeof(ContentDialog), (PropertyMetadata)null);
		PrimaryButtonStyleProperty = DependencyProperty.Register("PrimaryButtonStyle", typeof(Style), typeof(ContentDialog), (PropertyMetadata)null);
		IsPrimaryButtonEnabledProperty = DependencyProperty.Register("IsPrimaryButtonEnabled", typeof(bool), typeof(ContentDialog), new PropertyMetadata((object)true));
		SecondaryButtonTextProperty = DependencyProperty.Register("SecondaryButtonText", typeof(string), typeof(ContentDialog), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnButtonTextChanged)));
		SecondaryButtonCommandProperty = DependencyProperty.Register("SecondaryButtonCommand", typeof(ICommand), typeof(ContentDialog), (PropertyMetadata)null);
		SecondaryButtonCommandParameterProperty = DependencyProperty.Register("SecondaryButtonCommandParameter", typeof(object), typeof(ContentDialog), (PropertyMetadata)null);
		SecondaryButtonStyleProperty = DependencyProperty.Register("SecondaryButtonStyle", typeof(Style), typeof(ContentDialog), (PropertyMetadata)null);
		IsSecondaryButtonEnabledProperty = DependencyProperty.Register("IsSecondaryButtonEnabled", typeof(bool), typeof(ContentDialog), new PropertyMetadata((object)true));
		CloseButtonTextProperty = DependencyProperty.Register("CloseButtonText", typeof(string), typeof(ContentDialog), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnButtonTextChanged)));
		CloseButtonCommandProperty = DependencyProperty.Register("CloseButtonCommand", typeof(ICommand), typeof(ContentDialog), (PropertyMetadata)null);
		CloseButtonCommandParameterProperty = DependencyProperty.Register("CloseButtonCommandParameter", typeof(object), typeof(ContentDialog), (PropertyMetadata)null);
		CloseButtonStyleProperty = DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(ContentDialog), (PropertyMetadata)null);
		DefaultButtonProperty = DependencyProperty.Register("DefaultButton", typeof(ContentDialogButton), typeof(ContentDialog), new PropertyMetadata(new PropertyChangedCallback(OnDefaultButtonChanged)));
		FullSizeDesiredProperty = DependencyProperty.Register("FullSizeDesired", typeof(bool), typeof(ContentDialog), new PropertyMetadata(new PropertyChangedCallback(OnFullSizeDesiredChanged)));
		CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ContentDialog), (PropertyMetadata)null);
		IsShadowEnabledProperty = DependencyProperty.Register("IsShadowEnabled", typeof(bool), typeof(ContentDialog), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
		OpenDialogProperty = DependencyProperty.RegisterAttached("OpenDialog", typeof(ContentDialog), typeof(ContentDialog));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentDialog), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ContentDialog)));
		EventManager.RegisterClassHandler(typeof(Window), TitleBar.BackRequestedEvent, (Delegate)new EventHandler<BackRequestedEventArgs>(OnBackRequested));
	}

	public ContentDialog()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		m_closeTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(0.6)
		};
		m_closeTimer.Tick += OnCloseTimerTick;
	}

	private static void OnDefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ContentDialog)(object)d).UpdateDefaultButtonStates(useTransitions: true);
	}

	private static void OnFullSizeDesiredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ContentDialog)(object)d).UpdateVisualStates(useTransitions: true);
	}

	private static ContentDialog GetOpenDialog(Window window)
	{
		return (ContentDialog)((DependencyObject)window).GetValue(OpenDialogProperty);
	}

	private static void SetOpenDialog(Window window, ContentDialog value)
	{
		((DependencyObject)window).SetValue(OpenDialogProperty, (object)value);
	}

	public async Task<ContentDialogResult> ShowAsync()
	{
		Window owner = ActualOwner;
		if (owner == null)
		{
			await WaitUntilApplicationActivated();
			owner = ActualOwner;
		}
		if (owner == null)
		{
			throw new InvalidOperationException("Could not find an owner window for this ContentDialog.");
		}
		ThrowIfHasOpenDialog(owner);
		ContentPresenter val = FindContentPresenter(owner);
		if (val == null && !owner.IsActive)
		{
			await WaitUntilOwnerActivated(owner);
			val = FindContentPresenter(owner);
		}
		if (val == null)
		{
			throw new InvalidOperationException("Cound not find the ContentPresenter in the owner window.");
		}
		UIElement child;
		if (((FrameworkElement)this).Parent != null)
		{
			AddPopup();
			child = (UIElement)(object)LayoutRoot;
		}
		else
		{
			RemovePopup();
			child = (UIElement)(object)this;
		}
		EnsureAdornerLayer(val);
		EnsureAdornerChild(val, child);
		m_adornerLayer.Add((Adorner)(object)m_adorner);
		DisableKeyboardNavigation((DependencyObject)(object)val);
		IsShowing = true;
		m_openDialogOwner = owner;
		SetOpenDialog(owner, this);
		return await CreateAsyncOperation();
	}

	public Task<ContentDialogResult> ShowAsync(ContentDialogPlacement placement)
	{
		if (placement == ContentDialogPlacement.InPlace && ((FrameworkElement)this).Parent != null)
		{
			if (IsShowing)
			{
				ThrowAlreadyOpenException();
			}
			RemovePopup();
			IsShowing = true;
			m_isShowingInPlace = true;
			return CreateAsyncOperation();
		}
		return ShowAsync();
	}

	public void Hide()
	{
		Hide(ContentDialogResult.None);
	}

	public override void OnApplyTemplate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		if (LayoutRoot != null)
		{
			((UIElement)LayoutRoot).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnLayoutRootIsVisibleChanged);
			LayoutRoot.Loaded -= new RoutedEventHandler(OnLayoutRootLoaded);
			((UIElement)LayoutRoot).KeyDown -= new KeyEventHandler(OnLayoutRootKeyDown);
		}
		if (PrimaryButton != null)
		{
			((ButtonBase)PrimaryButton).Click -= new RoutedEventHandler(OnButtonClick);
		}
		if (SecondaryButton != null)
		{
			((ButtonBase)SecondaryButton).Click -= new RoutedEventHandler(OnButtonClick);
		}
		if (CloseButton != null)
		{
			((ButtonBase)CloseButton).Click -= new RoutedEventHandler(OnButtonClick);
		}
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("Container");
		Container = (Border)(object)((templateChild is Border) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("LayoutRoot");
		LayoutRoot = (FrameworkElement)(object)((templateChild2 is FrameworkElement) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("PrimaryButton");
		PrimaryButton = (Button)(object)((templateChild3 is Button) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("SecondaryButton");
		SecondaryButton = (Button)(object)((templateChild4 is Button) ? templateChild4 : null);
		DependencyObject templateChild5 = ((FrameworkElement)this).GetTemplateChild("CloseButton");
		CloseButton = (Button)(object)((templateChild5 is Button) ? templateChild5 : null);
		if (LayoutRoot != null)
		{
			((UIElement)LayoutRoot).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnLayoutRootIsVisibleChanged);
			LayoutRoot.Loaded += new RoutedEventHandler(OnLayoutRootLoaded);
			((UIElement)LayoutRoot).KeyDown += new KeyEventHandler(OnLayoutRootKeyDown);
		}
		if (PrimaryButton != null)
		{
			((ButtonBase)PrimaryButton).Click += new RoutedEventHandler(OnButtonClick);
		}
		if (SecondaryButton != null)
		{
			((ButtonBase)SecondaryButton).Click += new RoutedEventHandler(OnButtonClick);
		}
		if (CloseButton != null)
		{
			((ButtonBase)CloseButton).Click += new RoutedEventHandler(OnButtonClick);
		}
		UpdateVisualStates(useTransitions: false);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		HandleKeyDown(e);
		((UIElement)this).OnKeyDown(e);
	}

	private void Hide(ContentDialogResult result)
	{
		if (!IsShowing)
		{
			return;
		}
		OnOpened();
		TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> typedEventHandler = this.Closing;
		if (typedEventHandler != null)
		{
			ContentDialogClosingEventArgs args = new ContentDialogClosingEventArgs(result);
			ContentDialogClosingDeferral deferral = new ContentDialogClosingDeferral(delegate
			{
				if (!args.Cancel)
				{
					m_result = result;
					IsShowing = false;
				}
			});
			args.SetDeferral(deferral);
			args.IncrementDeferralCount();
			typedEventHandler(this, args);
			args.DecrementDeferralCount();
		}
		else
		{
			m_result = result;
			IsShowing = false;
		}
	}

	private void OnButtonClick(object sender, RoutedEventArgs e)
	{
		if (sender == PrimaryButton)
		{
			HandleButtonClick(this.PrimaryButtonClick, PrimaryButtonCommand, PrimaryButtonCommandParameter, ContentDialogResult.Primary);
		}
		else if (sender == SecondaryButton)
		{
			HandleButtonClick(this.SecondaryButtonClick, SecondaryButtonCommand, SecondaryButtonCommandParameter, ContentDialogResult.Secondary);
		}
		else if (sender == CloseButton)
		{
			HandleButtonClick(this.CloseButtonClick, CloseButtonCommand, CloseButtonCommandParameter, ContentDialogResult.None);
		}
	}

	private void HandleButtonClick(TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> handler, ICommand command, object commandParameter, ContentDialogResult result)
	{
		if (!IsShowing)
		{
			return;
		}
		if (handler != null)
		{
			ContentDialogButtonClickEventArgs args = new ContentDialogButtonClickEventArgs();
			ContentDialogButtonClickDeferral deferral = new ContentDialogButtonClickDeferral(delegate
			{
				if (!args.Cancel)
				{
					TryExecuteCommand(command, commandParameter);
					Hide(result);
				}
			});
			args.SetDeferral(deferral);
			args.IncrementDeferralCount();
			handler(this, args);
			args.DecrementDeferralCount();
		}
		else
		{
			TryExecuteCommand(command, commandParameter);
			Hide(result);
		}
	}

	private void OnLayoutRootLoaded(object sender, RoutedEventArgs e)
	{
		UpdateVisualStates(useTransitions: true);
	}

	private void OnLayoutRootIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			if (LayoutRoot.Parent is Popup)
			{
				((UIElement)LayoutRoot).Focusable = true;
				((UIElement)LayoutRoot).Focus();
			}
			else
			{
				((UIElement)LayoutRoot).Focusable = false;
				((UIElement)this).Focus();
			}
			OnOpened();
		}
		else
		{
			m_closeTimer.Stop();
			OnClosed();
		}
	}

	private void OnLayoutRootKeyDown(object sender, KeyEventArgs e)
	{
		HandleKeyDown(e);
	}

	private void OnCloseTimerTick(object sender, EventArgs e)
	{
		m_closeTimer.Stop();
		UpdateVisualStates(useTransitions: false);
		OnClosed();
	}

	private void OnOpened()
	{
		if (m_opening)
		{
			m_opening = false;
			this.Opened?.Invoke(this, new ContentDialogOpenedEventArgs());
		}
	}

	private void OnClosed()
	{
		if (m_adornerLayer != null)
		{
			RestoreKeyboardNavigation(((Adorner)m_adorner).AdornedElement);
			m_adornerLayer.Remove((Adorner)(object)m_adorner);
			m_adornerLayer = null;
		}
		if (m_showTcs != null)
		{
			this.Closed?.Invoke(this, new ContentDialogClosedEventArgs(m_result));
			m_showTcs.TrySetResult(m_result);
			m_showTcs = null;
			m_result = ContentDialogResult.None;
		}
	}

	private void HandleKeyDown(KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		Key key = e.Key;
		if ((int)key != 6)
		{
			if ((int)key == 13)
			{
				Hide();
				((RoutedEventArgs)e).Handled = true;
			}
		}
		else if (IsShowing)
		{
			Button val = null;
			switch (DefaultButton)
			{
			case ContentDialogButton.Primary:
				val = PrimaryButton;
				break;
			case ContentDialogButton.Secondary:
				val = SecondaryButton;
				break;
			case ContentDialogButton.Close:
				val = CloseButton;
				break;
			}
			if (val == null)
			{
				val = PrimaryButton ?? SecondaryButton ?? CloseButton;
			}
			if (val != null)
			{
				OnButtonClick(val, null);
				((RoutedEventArgs)e).Handled = true;
			}
		}
	}

	private void UpdateVisualStates(bool useTransitions)
	{
		UpdateDialogShowingStates(useTransitions);
		VisualStateManager.GoToState((FrameworkElement)(object)this, FullSizeDesired ? "FullDialogSizing" : "DefaultDialogSizing", useTransitions);
		UpdateButtonsVisibilityStates(useTransitions);
		UpdateDefaultButtonStates(useTransitions);
	}

	private void UpdateDialogShowingStates(bool useTransitions)
	{
		string text = ((IsShowing && ((FrameworkElement)this).IsLoaded) ? "DialogShowing" : "DialogHidden");
		if (DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			text = "DialogShowing";
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void UpdateButtonsVisibilityStates(bool useTransitions)
	{
		bool flag = !string.IsNullOrEmpty(PrimaryButtonText);
		bool flag2 = !string.IsNullOrEmpty(SecondaryButtonText);
		bool flag3 = !string.IsNullOrEmpty(CloseButtonText);
		string text = ((flag && flag2 && flag3) ? "AllVisible" : ((!flag && !flag2 && !flag3) ? "NoneVisible" : ((flag && flag2) ? "PrimaryAndSecondaryVisible" : ((flag && flag3) ? "PrimaryAndCloseVisible" : ((flag2 && flag3) ? "SecondaryAndCloseVisible" : (flag ? "PrimaryVisible" : (flag2 ? "SecondaryVisible" : ((!flag3) ? "AllVisible" : "CloseVisible"))))))));
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void UpdateDefaultButtonStates(bool useTransitions)
	{
		string text = "NoDefaultButton";
		switch (DefaultButton)
		{
		case ContentDialogButton.Primary:
			text = "PrimaryAsDefaultButton";
			break;
		case ContentDialogButton.Secondary:
			text = "SecondaryAsDefaultButton";
			break;
		case ContentDialogButton.Close:
			text = "CloseAsDefaultButton";
			break;
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private void EnsureAdornerLayer(ContentPresenter contentPresenter)
	{
		m_adornerLayer = AdornerLayer.GetAdornerLayer((Visual)(object)contentPresenter);
		if (m_adornerLayer == null)
		{
			throw new InvalidOperationException("AdornerLayer not found.");
		}
	}

	private void DisconnectAdornerChild()
	{
		if (m_adorner != null)
		{
			m_adorner.Child = null;
		}
	}

	private void EnsureAdornerChild(ContentPresenter cp, UIElement child)
	{
		if (m_adorner == null)
		{
			m_adorner = new ContentDialogAdorner((UIElement)(object)cp, child);
		}
		else
		{
			m_adorner.Child = child;
		}
	}

	private void AddPopup()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (m_popup == null && Container != null && LayoutRoot != null)
		{
			((Decorator)Container).Child = null;
			m_popup = new Popup
			{
				Child = (UIElement)(object)LayoutRoot
			};
			((Decorator)Container).Child = (UIElement)(object)m_popup;
		}
	}

	private void RemovePopup()
	{
		if (m_popup != null && Container != null && LayoutRoot != null)
		{
			m_popup.Child = null;
			m_popup = null;
			DisconnectAdornerChild();
			((Decorator)Container).Child = (UIElement)(object)LayoutRoot;
		}
	}

	private static void OnBackRequested(object sender, BackRequestedEventArgs e)
	{
		object source = ((RoutedEventArgs)e).Source;
		Window val = (Window)((source is Window) ? source : null);
		if (val != null)
		{
			ContentDialog openDialog = GetOpenDialog(val);
			if (openDialog != null)
			{
				((RoutedEventArgs)e).Handled = true;
				openDialog.Hide();
			}
		}
	}

	private void OnApplicationActivated(object sender, EventArgs e)
	{
		Application.Current.Activated -= OnApplicationActivated;
		if (m_activatedTcs != null)
		{
			m_activatedTcs.TrySetResult(result: true);
			m_activatedTcs = null;
		}
	}

	private void OnOwnerActivated(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Window)sender).Activated -= OnOwnerActivated;
		if (m_activatedTcs != null)
		{
			m_activatedTcs.TrySetResult(result: true);
			m_activatedTcs = null;
		}
	}

	private Task WaitUntilApplicationActivated()
	{
		m_activatedTcs = new TaskCompletionSource<bool>();
		Application.Current.Activated += OnApplicationActivated;
		return m_activatedTcs.Task;
	}

	private Task WaitUntilOwnerActivated(Window owner)
	{
		m_activatedTcs = new TaskCompletionSource<bool>();
		owner.Activated += OnOwnerActivated;
		return m_activatedTcs.Task;
	}

	private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ContentDialog)(object)d).UpdateButtonsVisibilityStates(useTransitions: true);
	}

	private static void TryExecuteCommand(ICommand command, object parameter)
	{
		if (command != null && command.CanExecute(parameter))
		{
			command.Execute(parameter);
		}
	}

	private void ThrowIfHasOpenDialog(Window owner)
	{
		if (GetOpenDialog(owner) != null)
		{
			ThrowAlreadyOpenException();
		}
	}

	private static void ThrowAlreadyOpenException()
	{
		throw new InvalidOperationException("Only a single ContentDialog can be open at any time.");
	}

	private static ContentPresenter FindContentPresenter(Window window)
	{
		ContentPresenter val = null;
		object content = ((ContentControl)window).Content;
		UIElement val2 = (UIElement)((content is UIElement) ? content : null);
		if (val2 != null)
		{
			DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)val2);
			val = (ContentPresenter)(object)((parent is ContentPresenter) ? parent : null);
		}
		if (val == null)
		{
			AdornerDecorator val3 = ((DependencyObject)(object)window).FindDescendant<AdornerDecorator>();
			if (val3 != null)
			{
				val = ((DependencyObject)(object)val3).FindDescendant<ContentPresenter>();
			}
		}
		return val;
	}

	private Task<ContentDialogResult> CreateAsyncOperation()
	{
		m_showTcs = new TaskCompletionSource<ContentDialogResult>();
		return m_showTcs.Task;
	}

	private static void DisableKeyboardNavigation(DependencyObject element)
	{
		KeyboardNavigation.SetDirectionalNavigation(element, (KeyboardNavigationMode)3);
		KeyboardNavigation.SetTabNavigation(element, (KeyboardNavigationMode)3);
		KeyboardNavigation.SetControlTabNavigation(element, (KeyboardNavigationMode)3);
	}

	private static void RestoreKeyboardNavigation(UIElement element)
	{
		((DependencyObject)element).ClearValue(KeyboardNavigation.DirectionalNavigationProperty);
		((DependencyObject)element).ClearValue(KeyboardNavigation.TabNavigationProperty);
		((DependencyObject)element).ClearValue(KeyboardNavigation.ControlTabNavigationProperty);
	}
}
