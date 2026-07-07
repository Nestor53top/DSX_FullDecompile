using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ModernWpf.Controls.Primitives;

public class ThemeShadowChrome : Decorator
{
	private class PopupControl : IDisposable
	{
		private ContextMenu _contextMenu;

		private ToolTip _toolTip;

		private Popup _popup;

		public FrameworkElement Control => (FrameworkElement)(_contextMenu ?? ((object)_toolTip) ?? ((object)_popup));

		public PlacementMode Placement
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				if (_contextMenu != null)
				{
					return _contextMenu.Placement;
				}
				if (_toolTip != null)
				{
					return _toolTip.Placement;
				}
				if (_popup != null)
				{
					return _popup.Placement;
				}
				return (PlacementMode)0;
			}
		}

		public UIElement PlacementTarget
		{
			get
			{
				if (_contextMenu != null)
				{
					return _contextMenu.PlacementTarget;
				}
				if (_toolTip != null)
				{
					return _toolTip.PlacementTarget;
				}
				if (_popup != null)
				{
					object obj = _popup.PlacementTarget;
					if (obj == null)
					{
						DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)_popup);
						obj = ((parent is UIElement) ? parent : null);
					}
					return (UIElement)obj;
				}
				return null;
			}
		}

		public Rect PlacementRectangle
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				if (_contextMenu != null)
				{
					return _contextMenu.PlacementRectangle;
				}
				if (_toolTip != null)
				{
					return _toolTip.PlacementRectangle;
				}
				if (_popup != null)
				{
					return _popup.PlacementRectangle;
				}
				return Rect.Empty;
			}
		}

		private FrameworkElement ChildAsFE
		{
			get
			{
				object obj = _contextMenu;
				if (obj == null)
				{
					obj = _toolTip;
					if (obj == null)
					{
						Popup popup = _popup;
						UIElement obj2 = ((popup != null) ? popup.Child : null);
						obj = ((obj2 is FrameworkElement) ? obj2 : null);
					}
				}
				return (FrameworkElement)obj;
			}
		}

		public event EventHandler Opened;

		public event EventHandler Closed;

		public PopupControl(ContextMenu contextMenu)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			_contextMenu = contextMenu;
			_contextMenu.Opened += new RoutedEventHandler(OnOpened);
			_contextMenu.Closed += new RoutedEventHandler(OnClosed);
		}

		public PopupControl(ToolTip toolTip)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			_toolTip = toolTip;
			_toolTip.Opened += new RoutedEventHandler(OnOpened);
			_toolTip.Closed += new RoutedEventHandler(OnClosed);
		}

		public PopupControl(Popup popup)
		{
			_popup = popup;
			_popup.Opened += OnOpened;
			_popup.Closed += OnClosed;
		}

		public void SetMargin(Thickness margin)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FrameworkElement childAsFE = ChildAsFE;
			if (childAsFE != null)
			{
				childAsFE.Margin = margin;
			}
		}

		public void ClearMargin()
		{
			FrameworkElement childAsFE = ChildAsFE;
			if (childAsFE != null)
			{
				((DependencyObject)childAsFE).ClearValue(FrameworkElement.MarginProperty);
			}
		}

		public void Dispose()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			if (_contextMenu != null)
			{
				_contextMenu.Opened -= new RoutedEventHandler(OnOpened);
				_contextMenu.Closed -= new RoutedEventHandler(OnClosed);
				_contextMenu = null;
			}
			else if (_toolTip != null)
			{
				_toolTip.Opened -= new RoutedEventHandler(OnOpened);
				_toolTip.Closed -= new RoutedEventHandler(OnClosed);
				_toolTip = null;
			}
			else if (_popup != null)
			{
				_popup.Opened -= OnOpened;
				_popup.Closed -= OnClosed;
				_popup = null;
			}
		}

		private void OnOpened(object sender, EventArgs e)
		{
			this.Opened?.Invoke(this, e);
		}

		private void OnClosed(object sender, EventArgs e)
		{
			this.Closed?.Invoke(this, e);
		}
	}

	public static readonly DependencyProperty IsShadowEnabledProperty;

	public static readonly DependencyProperty DepthProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	private static readonly DependencyProperty PopupMarginProperty;

	private readonly Grid _background;

	private readonly BitmapCache _bitmapCache;

	private Border _shadow1;

	private Border _shadow2;

	private PopupControl _parentPopupControl;

	private TranslateTransform _transform;

	private PopupPositioner _popupPositioner;

	private static readonly Brush s_bg1;

	private static readonly Brush s_bg2;

	private static readonly Brush s_bg3;

	private static readonly Brush s_bg4;

	private static readonly Vector s_noTranslation;

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

	public double Depth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(DepthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DepthProperty, (object)value);
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

	private Thickness PopupMargin
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(PopupMarginProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(PopupMarginProperty, (object)value);
		}
	}

	protected override int VisualChildrenCount
	{
		get
		{
			if (!IsShadowEnabled)
			{
				return ((Decorator)this).VisualChildrenCount;
			}
			if (((Decorator)this).Child != null)
			{
				return 2;
			}
			return 1;
		}
	}

	static ThemeShadowChrome()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		IsShadowEnabledProperty = DependencyProperty.Register("IsShadowEnabled", typeof(bool), typeof(ThemeShadowChrome), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsShadowEnabledChanged)));
		DepthProperty = DependencyProperty.Register("Depth", typeof(double), typeof(ThemeShadowChrome), new PropertyMetadata((object)32.0, new PropertyChangedCallback(OnDepthChanged)));
		CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ThemeShadowChrome), new PropertyMetadata((object)default(CornerRadius), new PropertyChangedCallback(OnCornerRadiusChanged)));
		PopupMarginProperty = DependencyProperty.Register("PopupMargin", typeof(Thickness), typeof(ThemeShadowChrome), new PropertyMetadata((object)default(Thickness), new PropertyChangedCallback(OnPopupMarginChanged)));
		s_noTranslation = new Vector(0.0, 0.0);
		s_bg1 = (Brush)new SolidColorBrush(Colors.Black)
		{
			Opacity = 0.11
		};
		s_bg2 = (Brush)new SolidColorBrush(Colors.Black)
		{
			Opacity = 0.13
		};
		s_bg3 = (Brush)new SolidColorBrush(Colors.Black)
		{
			Opacity = 0.18
		};
		s_bg4 = (Brush)new SolidColorBrush(Colors.Black)
		{
			Opacity = 0.22
		};
		((Freezable)s_bg1).Freeze();
		((Freezable)s_bg2).Freeze();
		((Freezable)s_bg3).Freeze();
		((Freezable)s_bg4).Freeze();
	}

	public ThemeShadowChrome()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)this);
		_bitmapCache = new BitmapCache(((DpiScale)(ref dpi)).PixelsPerDip);
		_background = new Grid
		{
			CacheMode = (CacheMode)(object)_bitmapCache,
			Focusable = false,
			IsHitTestVisible = false,
			SnapsToDevicePixels = false
		};
		((Visual)this).AddVisualChild((Visual)(object)_background);
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
	}

	private static void OnIsShadowEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ThemeShadowChrome)(object)d).OnIsShadowEnabledChanged();
	}

	private void OnIsShadowEnabledChanged()
	{
		if (((FrameworkElement)this).IsInitialized)
		{
			if (IsShadowEnabled)
			{
				EnsureShadows();
				((Panel)_background).Children.Add((UIElement)(object)_shadow1);
				((Panel)_background).Children.Add((UIElement)(object)_shadow2);
				((UIElement)_background).Visibility = (Visibility)0;
			}
			else
			{
				((Panel)_background).Children.Clear();
				((UIElement)_background).Visibility = (Visibility)2;
			}
			OnVisualParentChanged();
			UpdatePopupMargin();
		}
	}

	private static void OnDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ThemeShadowChrome)(object)d).OnDepthChanged();
	}

	private void OnDepthChanged()
	{
		if (((FrameworkElement)this).IsInitialized)
		{
			UpdateShadow1();
			UpdateShadow2();
			UpdatePopupMargin();
		}
	}

	private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ThemeShadowChrome)(object)d).OnCornerRadiusChanged(e);
	}

	private void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius cornerRadius = (CornerRadius)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (_shadow1 != null)
		{
			_shadow1.CornerRadius = cornerRadius;
		}
		if (_shadow2 != null)
		{
			_shadow2.CornerRadius = cornerRadius;
		}
	}

	private static void OnPopupMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ThemeShadowChrome)(object)d).OnPopupMarginChanged(e);
	}

	private void OnPopupMarginChanged(DependencyPropertyChangedEventArgs e)
	{
		ApplyPopupMargin();
	}

	private void UpdatePopupMargin()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (IsShadowEnabled)
		{
			double depth = Depth;
			double num = 0.9 * depth;
			double num2 = 0.4 * depth;
			PopupMargin = new Thickness(num, num, num, num + num2);
		}
		else
		{
			((DependencyObject)this).ClearValue(PopupMarginProperty);
		}
	}

	private void ApplyPopupMargin()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_parentPopupControl != null)
		{
			if (((DependencyObject)this).ReadLocalValue(PopupMarginProperty) == DependencyProperty.UnsetValue)
			{
				_parentPopupControl.ClearMargin();
			}
			else
			{
				_parentPopupControl.SetMargin(PopupMargin);
			}
		}
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		((FrameworkElement)this).OnVisualParentChanged(oldParent);
		if (((FrameworkElement)this).IsInitialized)
		{
			OnVisualParentChanged();
		}
	}

	protected override Visual GetVisualChild(int index)
	{
		if (IsShadowEnabled)
		{
			switch (index)
			{
			case 0:
				return (Visual)(object)_background;
			case 1:
				if (((Decorator)this).Child != null)
				{
					return (Visual)(object)((Decorator)this).Child;
				}
				break;
			}
			throw new ArgumentOutOfRangeException("index");
		}
		return ((Decorator)this).GetVisualChild(index);
	}

	protected override void OnInitialized(EventArgs e)
	{
		((FrameworkElement)this).OnInitialized(e);
		OnIsShadowEnabledChanged();
	}

	protected override Size MeasureOverride(Size constraint)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (IsShadowEnabled)
		{
			((UIElement)_background).Measure(constraint);
		}
		return ((Decorator)this).MeasureOverride(constraint);
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (IsShadowEnabled)
		{
			((UIElement)_background).Arrange(new Rect(arrangeSize));
		}
		return ((Decorator)this).ArrangeOverride(arrangeSize);
	}

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Visual)this).OnDpiChanged(oldDpi, newDpi);
		_bitmapCache.RenderAtScale = ((DpiScale)(ref newDpi)).PixelsPerDip;
	}

	private void OnVisualParentChanged()
	{
		if (IsShadowEnabled)
		{
			PopupControl parentPopupControl = null;
			DependencyObject visualParent = ((Visual)this).VisualParent;
			ContextMenu val = (ContextMenu)(object)((visualParent is ContextMenu) ? visualParent : null);
			if (val != null)
			{
				parentPopupControl = new PopupControl(val);
			}
			else
			{
				ToolTip val2 = (ToolTip)(object)((visualParent is ToolTip) ? visualParent : null);
				if (val2 != null)
				{
					parentPopupControl = new PopupControl(val2);
				}
				else
				{
					Popup val3 = FindParentPopup((FrameworkElement)(object)this);
					if (val3 != null)
					{
						parentPopupControl = new PopupControl(val3);
					}
				}
			}
			SetParentPopupControl(parentPopupControl);
		}
		else
		{
			SetParentPopupControl(null);
		}
	}

	private void EnsureShadows()
	{
		if (_shadow1 == null)
		{
			_shadow1 = CreateShadowElement();
			UpdateShadow1();
		}
		if (_shadow2 == null)
		{
			_shadow2 = CreateShadowElement();
			UpdateShadow2();
		}
	}

	private Border CreateShadowElement()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Expected O, but got Unknown
		return new Border
		{
			CornerRadius = CornerRadius,
			Effect = (Effect)new BlurEffect(),
			RenderTransform = (Transform)new TranslateTransform()
		};
	}

	private void UpdateShadow1()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_shadow1 != null)
		{
			double depth = Depth;
			((BlurEffect)((UIElement)_shadow1).Effect).Radius = 0.9 * depth;
			((TranslateTransform)((UIElement)_shadow1).RenderTransform).Y = 0.4 * depth;
			_shadow1.Background = ((depth >= 32.0) ? s_bg4 : s_bg2);
		}
	}

	private void UpdateShadow2()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_shadow2 != null)
		{
			double depth = Depth;
			((BlurEffect)((UIElement)_shadow2).Effect).Radius = 0.225 * depth;
			((TranslateTransform)((UIElement)_shadow2).RenderTransform).Y = 0.075 * depth;
			_shadow2.Background = ((depth >= 32.0) ? s_bg3 : s_bg1);
		}
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		ClearMarginAdjustment();
		((UIElement)this).UpdateLayout();
		AdjustMargin();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (((UIElement)this).IsVisible)
		{
			AdjustMargin();
		}
	}

	private void AdjustMargin()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		if (_parentPopupControl == null)
		{
			return;
		}
		Thickness margin = ((FrameworkElement)this).Margin;
		if (!(margin != default(Thickness)))
		{
			return;
		}
		DependencyObject visualParent = ((Visual)this).VisualParent;
		UIElement val = (UIElement)(object)((visualParent is UIElement) ? visualParent : null);
		if (val != null)
		{
			Size renderSize = val.RenderSize;
			double width = ((Size)(ref renderSize)).Width;
			double actualWidth = ((FrameworkElement)this).ActualWidth;
			if (width > 0.0 && actualWidth > 0.0 && width < actualWidth + ((Thickness)(ref margin)).Left + ((Thickness)(ref margin)).Right)
			{
				double num = (width - actualWidth) / 2.0;
				ThicknessAnimation val2 = new ThicknessAnimation(new Thickness(num, ((Thickness)(ref margin)).Top, num, ((Thickness)(ref margin)).Bottom), Duration.op_Implicit(TimeSpan.Zero));
				((UIElement)this).BeginAnimation(FrameworkElement.MarginProperty, (AnimationTimeline)(object)val2);
				((UIElement)this).UpdateLayout();
			}
		}
	}

	private void ClearMarginAdjustment()
	{
		((UIElement)this).BeginAnimation(FrameworkElement.MarginProperty, (AnimationTimeline)null);
	}

	private void SetParentPopupControl(PopupControl value)
	{
		if (_parentPopupControl != value)
		{
			if (_popupPositioner != null)
			{
				_popupPositioner.Dispose();
				_popupPositioner = null;
			}
			if (_parentPopupControl != null)
			{
				_parentPopupControl.Opened -= OnParentPopupControlOpened;
				_parentPopupControl.Closed -= OnParentPopupControlClosed;
				_parentPopupControl.ClearMargin();
				_parentPopupControl.Dispose();
			}
			_parentPopupControl = value;
			if (_parentPopupControl != null)
			{
				_parentPopupControl.Opened += OnParentPopupControlOpened;
				_parentPopupControl.Closed += OnParentPopupControlClosed;
				ApplyPopupMargin();
			}
		}
	}

	private void OnParentPopupControlOpened(object sender, EventArgs e)
	{
		if (_popupPositioner != null)
		{
			return;
		}
		if (_parentPopupControl != null)
		{
			FrameworkElement control = _parentPopupControl.Control;
			if (control != null)
			{
				ToolTip val = (ToolTip)(object)((control is ToolTip) ? control : null);
				if (val != null)
				{
					UIElement placementTarget = val.PlacementTarget;
					Thumb val2 = (Thumb)(object)((placementTarget is Thumb) ? placementTarget : null);
					if (val2 != null && ((FrameworkElement)val2).TemplatedParent is Slider)
					{
						return;
					}
				}
				object obj = ((control is Popup) ? control : null);
				if (obj == null)
				{
					DependencyObject parent = control.Parent;
					obj = ((parent is Popup) ? parent : null);
				}
				Popup val3 = (Popup)obj;
				if (val3 != null && PopupPositioner.IsSupported)
				{
					_popupPositioner = new PopupPositioner(val3);
				}
			}
		}
		if (_popupPositioner == null)
		{
			PositionParentPopupControl();
		}
	}

	private void OnParentPopupControlClosed(object sender, EventArgs e)
	{
		ClearMarginAdjustment();
		ResetTransform();
	}

	private void PositionParentPopupControl()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		PopupControl popup = _parentPopupControl;
		if (popup == null)
		{
			return;
		}
		CustomPlacementMode? customPlacementMode = null;
		PlacementMode placement = popup.Placement;
		if ((int)placement != 2)
		{
			if ((int)placement != 10)
			{
				if ((int)placement == 11 && TryGetCustomPlacementMode(out var placement2))
				{
					customPlacementMode = placement2;
				}
			}
			else
			{
				customPlacementMode = CustomPlacementMode.TopEdgeAlignedLeft;
			}
		}
		else
		{
			customPlacementMode = CustomPlacementMode.BottomEdgeAlignedLeft;
		}
		if (!customPlacementMode.HasValue || EnsureEdgesAligned(customPlacementMode.Value))
		{
			return;
		}
		if (customPlacementMode == CustomPlacementMode.BottomEdgeAlignedLeft)
		{
			if (shouldAlignRightEdges())
			{
				EnsureEdgesAligned(CustomPlacementMode.BottomEdgeAlignedRight);
			}
		}
		else if (customPlacementMode == CustomPlacementMode.TopEdgeAlignedLeft && shouldAlignRightEdges())
		{
			EnsureEdgesAligned(CustomPlacementMode.TopEdgeAlignedRight);
		}
		bool shouldAlignRightEdges()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			UIElement placementTarget = popup.PlacementTarget;
			if (placementTarget != null)
			{
				Size renderSize = placementTarget.RenderSize;
				double width = ((Size)(ref renderSize)).Width;
				if (((FrameworkElement)this).ActualWidth > 0.0 && width > 0.0)
				{
					if (((FrameworkElement)this).ActualWidth == width)
					{
						return true;
					}
					if (((FrameworkElement)this).ActualWidth > width && TryGetOffsetToTarget(InterestPoint.TopRight, InterestPoint.TopRight, out var offset) && ((Vector)(ref offset)).X < 0.0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private bool TryGetCustomPlacementMode(out CustomPlacementMode placement)
	{
		if (TryGetCustomPlacementMode((DependencyObject)(object)_parentPopupControl?.Control, out placement))
		{
			return true;
		}
		if (TryGetCustomPlacementMode(((Visual)this).VisualParent, out placement))
		{
			return true;
		}
		return false;
	}

	private bool TryGetCustomPlacementMode(DependencyObject element, out CustomPlacementMode placement)
	{
		if (element != null && element.ReadLocalValue(CustomPopupPlacementHelper.PlacementProperty) != DependencyProperty.UnsetValue)
		{
			placement = CustomPopupPlacementHelper.GetPlacement(element);
			return true;
		}
		placement = CustomPlacementMode.Top;
		return false;
	}

	private bool TryGetOffsetToTarget(InterestPoint targetInterestPoint, InterestPoint childInterestPoint, out Vector offset)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PopupControl parentPopupControl = _parentPopupControl;
		if (parentPopupControl != null)
		{
			UIElement placementTarget = parentPopupControl.PlacementTarget;
			if (placementTarget != null && ((UIElement)this).IsVisible && placementTarget.IsVisible)
			{
				offset = Helper.GetOffset((UIElement)(object)this, childInterestPoint, placementTarget, targetInterestPoint, parentPopupControl.PlacementRectangle);
				if (Math.Abs(((Vector)(ref offset)).X) < 0.5)
				{
					((Vector)(ref offset)).X = 0.0;
				}
				if (Math.Abs(((Vector)(ref offset)).Y) < 0.5)
				{
					((Vector)(ref offset)).Y = 0.0;
				}
				return true;
			}
		}
		offset = default(Vector);
		return false;
	}

	private bool EnsureEdgesAligned(CustomPlacementMode placement)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector val = s_noTranslation;
		Vector offset;
		switch (placement)
		{
		case CustomPlacementMode.TopEdgeAlignedLeft:
			if (TryGetOffsetToTarget(InterestPoint.TopLeft, InterestPoint.BottomLeft, out offset))
			{
				val = getTranslation(top: true, left: true, offset);
			}
			break;
		case CustomPlacementMode.TopEdgeAlignedRight:
			if (TryGetOffsetToTarget(InterestPoint.TopRight, InterestPoint.BottomRight, out offset))
			{
				val = getTranslation(top: true, left: false, offset);
			}
			break;
		case CustomPlacementMode.BottomEdgeAlignedLeft:
			if (TryGetOffsetToTarget(InterestPoint.BottomLeft, InterestPoint.TopLeft, out offset))
			{
				val = getTranslation(top: false, left: true, offset);
			}
			break;
		case CustomPlacementMode.BottomEdgeAlignedRight:
			if (TryGetOffsetToTarget(InterestPoint.BottomRight, InterestPoint.TopRight, out offset))
			{
				val = getTranslation(top: false, left: false, offset);
			}
			break;
		}
		if (val != s_noTranslation)
		{
			SetupTransform(val);
			return true;
		}
		ResetTransform();
		return false;
		Vector getTranslation(bool top, bool left, Vector val2)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			double num = 0.0;
			double num2 = 0.0;
			if ((left && ((Vector)(ref val2)).X > 0.0) || (!left && ((Vector)(ref val2)).X < 0.0) || Math.Abs(((Vector)(ref val2)).X) < 0.5)
			{
				num = 0.0 - ((Vector)(ref val2)).X;
			}
			Thickness popupMargin;
			if (top)
			{
				double y = ((Vector)(ref val2)).Y;
				popupMargin = PopupMargin;
				if (y < ((Thickness)(ref popupMargin)).Top)
				{
					goto IL_00aa;
				}
			}
			if (!top)
			{
				double y2 = ((Vector)(ref val2)).Y;
				popupMargin = PopupMargin;
				if (y2 > 0.0 - ((Thickness)(ref popupMargin)).Bottom)
				{
					goto IL_00aa;
				}
			}
			if (Math.Abs(((Vector)(ref val2)).Y) < 0.5)
			{
				goto IL_00aa;
			}
			goto IL_00b3;
			IL_00b3:
			return new Vector(num, num2);
			IL_00aa:
			num2 = 0.0 - ((Vector)(ref val2)).Y;
			goto IL_00b3;
		}
	}

	private void SetupTransform(Vector translation)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (_transform == null)
		{
			_transform = new TranslateTransform();
			((UIElement)this).RenderTransform = (Transform)(object)_transform;
		}
		_transform.X = ((Vector)(ref translation)).X;
		_transform.Y = ((Vector)(ref translation)).Y;
	}

	private void ResetTransform()
	{
		if (_transform != null)
		{
			((DependencyObject)_transform).ClearValue(TranslateTransform.XProperty);
			((DependencyObject)_transform).ClearValue(TranslateTransform.YProperty);
		}
	}

	private Popup FindParentPopup(FrameworkElement element)
	{
		DependencyObject parent = element.Parent;
		Popup val = (Popup)(object)((parent is Popup) ? parent : null);
		if (val != null)
		{
			return val;
		}
		FrameworkElement val2 = (FrameworkElement)(object)((parent is FrameworkElement) ? parent : null);
		if (val2 != null)
		{
			return FindParentPopup(val2);
		}
		DependencyObject parent2 = VisualTreeHelper.GetParent((DependencyObject)(object)element);
		FrameworkElement val3 = (FrameworkElement)(object)((parent2 is FrameworkElement) ? parent2 : null);
		if (val3 != null)
		{
			return FindParentPopup(val3);
		}
		return null;
	}
}
