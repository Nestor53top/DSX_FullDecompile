using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernWpf.Controls.Primitives;

public abstract class FlyoutBase : DependencyObject
{
	private class FullPlacementWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			double num = (double)values[0];
			Thickness val = (Thickness)values[1];
			return num - ((Thickness)(ref val)).Left - ((Thickness)(ref val)).Right;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	private class FullPlacementHeightConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			double num = (double)values[0];
			Thickness val = (Thickness)values[1];
			return num - ((Thickness)(ref val)).Top - ((Thickness)(ref val)).Bottom;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	private class PlacementConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (CustomPlacementMode)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(FlyoutPlacementMode), typeof(FlyoutBase), new PropertyMetadata((object)FlyoutPlacementMode.Top));

	public static readonly DependencyProperty AreOpenCloseAnimationsEnabledProperty = DependencyProperty.Register("AreOpenCloseAnimationsEnabled", typeof(bool), typeof(FlyoutBase), new PropertyMetadata((object)true, new PropertyChangedCallback(OnAreOpenCloseAnimationsEnabledChanged)));

	private static readonly DependencyPropertyKey IsOpenPropertyKey = DependencyProperty.RegisterReadOnly("IsOpen", typeof(bool), typeof(FlyoutBase), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsOpenChanged)));

	public static readonly DependencyProperty IsOpenProperty = IsOpenPropertyKey.DependencyProperty;

	public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(FlyoutShowMode), typeof(FlyoutBase), new PropertyMetadata((object)FlyoutShowMode.Standard));

	public static readonly DependencyProperty AttachedFlyoutProperty = DependencyProperty.RegisterAttached("AttachedFlyout", typeof(FlyoutBase), typeof(FlyoutBase));

	private static readonly IMultiValueConverter s_fullPlacementWidthConverter = (IMultiValueConverter)(object)new FullPlacementWidthConverter();

	private static readonly IMultiValueConverter s_fullPlacementHeightConverter = (IMultiValueConverter)(object)new FullPlacementHeightConverter();

	private static readonly IValueConverter s_placementConverter = (IValueConverter)(object)new PlacementConverter();

	private const double s_offset = 4.0;

	private Control m_presenter;

	private PopupEx m_popup;

	private FrameworkElement m_target;

	private bool m_showingAsContextFlyout;

	private WeakReference<IInputElement> m_weakRefToPreviousFocus;

	private bool m_closing;

	private Action m_pendingShow;

	private DispatcherOperation m_asyncShow;

	public FlyoutPlacementMode Placement
	{
		get
		{
			return (FlyoutPlacementMode)((DependencyObject)this).GetValue(PlacementProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlacementProperty, (object)value);
		}
	}

	public bool AreOpenCloseAnimationsEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(AreOpenCloseAnimationsEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AreOpenCloseAnimationsEnabledProperty, (object)value);
		}
	}

	public bool IsOpen
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsOpenProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(IsOpenPropertyKey, (object)value);
		}
	}

	public FlyoutShowMode ShowMode
	{
		get
		{
			return (FlyoutShowMode)((DependencyObject)this).GetValue(ShowModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowModeProperty, (object)value);
		}
	}

	internal virtual PopupAnimation DesiredPopupAnimation => (PopupAnimation)1;

	internal PopupEx InternalPopup => m_popup;

	internal double Offset { get; set; } = 4.0;

	public event EventHandler<object> Opening;

	public event EventHandler<object> Opened;

	public event EventHandler<object> Closed;

	internal event TypedEventHandler<FlyoutBase, FlyoutBaseClosingEventArgs> Closing;

	private static void OnAreOpenCloseAnimationsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((FlyoutBase)(object)d).OnAreOpenCloseAnimationsEnabledChanged(e);
	}

	internal virtual void OnAreOpenCloseAnimationsEnabledChanged(DependencyPropertyChangedEventArgs e)
	{
		UpdatePopupAnimation();
	}

	private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((FlyoutBase)(object)d).OnIsOpenChanged();
	}

	internal virtual void OnIsOpenChanged()
	{
		if (IsOpen)
		{
			if (Keyboard.FocusedElement != null)
			{
				m_weakRefToPreviousFocus = new WeakReference<IInputElement>(Keyboard.FocusedElement);
			}
			Control presenter = m_presenter;
			if (presenter != null)
			{
				((UIElement)presenter).Focus();
			}
		}
		else if (m_weakRefToPreviousFocus != null)
		{
			if (m_weakRefToPreviousFocus.TryGetTarget(out var target))
			{
				target.Focus();
			}
			m_weakRefToPreviousFocus = null;
		}
	}

	internal virtual void UpdateIsOpen()
	{
		IsOpen = m_popup != null && ((Popup)m_popup).IsOpen;
	}

	public static FlyoutBase GetAttachedFlyout(FrameworkElement element)
	{
		return (FlyoutBase)((DependencyObject)element).GetValue(AttachedFlyoutProperty);
	}

	public static void SetAttachedFlyout(FrameworkElement element, FlyoutBase value)
	{
		((DependencyObject)element).SetValue(AttachedFlyoutProperty, (object)value);
	}

	public static void ShowAttachedFlyout(FrameworkElement flyoutOwner)
	{
		GetAttachedFlyout(flyoutOwner)?.ShowAt(flyoutOwner);
	}

	public void ShowAt(FrameworkElement placementTarget)
	{
		if (placementTarget == null)
		{
			throw new ArgumentNullException("placementTarget");
		}
		ShowAtCore(placementTarget, showAsContextFlyout: false);
	}

	public void Hide()
	{
		CancelAsyncShow();
		HideCore();
	}

	protected abstract Control CreatePresenter();

	internal void ShowAsContextFlyout(FrameworkElement placementTarget)
	{
		if (placementTarget == null)
		{
			throw new ArgumentNullException("placementTarget");
		}
		ShowAtCore(placementTarget, showAsContextFlyout: true);
	}

	internal virtual void ShowAtCore(FrameworkElement placementTarget, bool showAsContextFlyout)
	{
		CancelAsyncShow();
		if (m_popup != null && ((Popup)m_popup).IsOpen && m_target == placementTarget && m_showingAsContextFlyout == showAsContextFlyout)
		{
			return;
		}
		if (m_closing)
		{
			m_pendingShow = delegate
			{
				ShowAtCore(placementTarget, showAsContextFlyout);
			};
			return;
		}
		PreparePopup(placementTarget, showAsContextFlyout);
		m_target = placementTarget;
		m_showingAsContextFlyout = showAsContextFlyout;
		OnOpening();
		((Popup)m_popup).IsOpen = true;
	}

	internal virtual void HideCore()
	{
		if (m_popup != null && ((Popup)m_popup).IsOpen)
		{
			((Popup)m_popup).IsOpen = false;
		}
	}

	internal virtual void OnOpening()
	{
		this.Opening?.Invoke(this, null);
	}

	internal virtual void OnOpened()
	{
		this.Opened?.Invoke(this, null);
	}

	internal virtual void OnClosed()
	{
		this.Closed?.Invoke(this, null);
		Action pendingShow = m_pendingShow;
		CancelAsyncShow();
		if (pendingShow != null)
		{
			m_asyncShow = ((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)pendingShow, Array.Empty<object>());
		}
	}

	internal void BindPlacement(Control presenter)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		((FrameworkElement)presenter).SetBinding(CustomPopupPlacementHelper.PlacementProperty, (BindingBase)new Binding
		{
			Path = new PropertyPath((object)PlacementProperty),
			Source = this,
			Converter = s_placementConverter
		});
	}

	private void EnsurePresenter()
	{
		if (m_presenter == null)
		{
			m_presenter = CreatePresenter();
			BindPlacement(m_presenter);
		}
	}

	private void EnsurePopup()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		if (m_popup == null)
		{
			EnsurePresenter();
			PopupEx popupEx = new PopupEx();
			((Popup)popupEx).Child = (UIElement)(object)m_presenter;
			((Popup)popupEx).StaysOpen = false;
			((Popup)popupEx).AllowsTransparency = true;
			((Popup)popupEx).CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PositionPopup);
			m_popup = popupEx;
			((Popup)m_popup).Opened += OnPopupOpened;
			m_popup.Closing += OnPopupClosing;
			((Popup)m_popup).Closed += OnPopupClosed;
			m_popup.IsOpenChanged += OnPopupIsOpenChanged;
		}
	}

	private void PreparePopup(FrameworkElement placementTarget, bool showAsContextFlyout)
	{
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0167: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01d6: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		EnsurePopup();
		if (((Popup)m_popup).IsOpen)
		{
			((Popup)m_popup).IsOpen = false;
		}
		UpdatePopupAnimation();
		if (showAsContextFlyout)
		{
			((DependencyObject)m_presenter).ClearValue(FrameworkElement.WidthProperty);
			((DependencyObject)m_presenter).ClearValue(FrameworkElement.HeightProperty);
			((Popup)m_popup).Placement = (PlacementMode)8;
			((Popup)m_popup).PlacementTarget = (UIElement)(object)placementTarget;
			((DependencyObject)m_popup).ClearValue(Popup.PlacementRectangleProperty);
			return;
		}
		if (Placement == FlyoutPlacementMode.Full)
		{
			Window window = Window.GetWindow((DependencyObject)(object)placementTarget);
			if (window != null)
			{
				AdornerDecorator val = ((DependencyObject)(object)window).FindDescendant<AdornerDecorator>();
				if (val != null)
				{
					placementTarget = (FrameworkElement)(object)val;
					((FrameworkElement)m_presenter).SetBinding(FrameworkElement.WidthProperty, (BindingBase)new Binding
					{
						Path = new PropertyPath((object)FrameworkElement.ActualWidthProperty),
						Source = val
					});
					((FrameworkElement)m_presenter).SetBinding(FrameworkElement.HeightProperty, (BindingBase)new Binding
					{
						Path = new PropertyPath((object)FrameworkElement.ActualHeightProperty),
						Source = val
					});
				}
				else
				{
					placementTarget = (FrameworkElement)(object)window;
					((FrameworkElement)m_presenter).SetBinding(FrameworkElement.WidthProperty, (BindingBase)new MultiBinding
					{
						Converter = s_fullPlacementWidthConverter,
						Bindings = { (BindingBase)new Binding
						{
							Path = new PropertyPath((object)FrameworkElement.ActualWidthProperty),
							Source = window
						} },
						Bindings = { (BindingBase)new Binding
						{
							Path = new PropertyPath((object)Control.BorderThicknessProperty),
							Source = window
						} }
					});
					((FrameworkElement)m_presenter).SetBinding(FrameworkElement.HeightProperty, (BindingBase)new MultiBinding
					{
						Converter = s_fullPlacementHeightConverter,
						Bindings = { (BindingBase)new Binding
						{
							Path = new PropertyPath((object)FrameworkElement.ActualHeightProperty),
							Source = window
						} },
						Bindings = { (BindingBase)new Binding
						{
							Path = new PropertyPath((object)Control.BorderThicknessProperty),
							Source = window
						} }
					});
				}
				((Popup)m_popup).Placement = (PlacementMode)3;
				((Popup)m_popup).PlacementTarget = (UIElement)(object)placementTarget;
				((DependencyObject)m_popup).ClearValue(Popup.PlacementRectangleProperty);
				return;
			}
		}
		((DependencyObject)m_presenter).ClearValue(FrameworkElement.WidthProperty);
		((DependencyObject)m_presenter).ClearValue(FrameworkElement.HeightProperty);
		((Popup)m_popup).Placement = (PlacementMode)11;
		((Popup)m_popup).PlacementTarget = (UIElement)(object)placementTarget;
		((Popup)m_popup).PlacementRectangle = GetPlacementRectangle((UIElement)(object)placementTarget);
	}

	private void UpdatePopupAnimation()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (m_popup != null)
		{
			((Popup)m_popup).PopupAnimation = (PopupAnimation)((AreOpenCloseAnimationsEnabled && SharedHelpers.IsAnimationsEnabled) ? ((int)DesiredPopupAnimation) : 0);
		}
	}

	internal Rect GetPlacementRectangle(UIElement target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Rect empty = Rect.Empty;
		if (target != null)
		{
			Size renderSize = target.RenderSize;
			switch (Placement)
			{
			case FlyoutPlacementMode.Top:
			case FlyoutPlacementMode.Bottom:
			case FlyoutPlacementMode.TopEdgeAlignedLeft:
			case FlyoutPlacementMode.TopEdgeAlignedRight:
			case FlyoutPlacementMode.BottomEdgeAlignedLeft:
			case FlyoutPlacementMode.BottomEdgeAlignedRight:
				((Rect)(ref empty))._002Ector(new Point(0.0, 0.0 - Offset), new Point(((Size)(ref renderSize)).Width, ((Size)(ref renderSize)).Height + Offset));
				break;
			case FlyoutPlacementMode.Left:
			case FlyoutPlacementMode.Right:
			case FlyoutPlacementMode.LeftEdgeAlignedTop:
			case FlyoutPlacementMode.LeftEdgeAlignedBottom:
			case FlyoutPlacementMode.RightEdgeAlignedTop:
			case FlyoutPlacementMode.RightEdgeAlignedBottom:
				((Rect)(ref empty))._002Ector(new Point(0.0 - Offset, 0.0), new Point(((Size)(ref renderSize)).Width + Offset, ((Size)(ref renderSize)).Height));
				break;
			}
		}
		return empty;
	}

	private void OnPopupOpened(object sender, EventArgs e)
	{
		OnOpened();
	}

	private void OnPopupClosing(object sender, EventArgs e)
	{
		this.Closing?.Invoke(this, new FlyoutBaseClosingEventArgs());
		m_closing = true;
	}

	private void OnPopupClosed(object sender, EventArgs e)
	{
		m_closing = false;
		if (!((Popup)m_popup).IsOpen)
		{
			((DependencyObject)m_popup).ClearValue(Popup.PlacementProperty);
			((DependencyObject)m_popup).ClearValue(Popup.PlacementTargetProperty);
			((DependencyObject)m_popup).ClearValue(Popup.PlacementRectangleProperty);
			((DependencyObject)m_popup).ClearValue(FrameworkElement.WidthProperty);
			((DependencyObject)m_popup).ClearValue(FrameworkElement.HeightProperty);
			m_target = null;
			m_showingAsContextFlyout = false;
		}
		OnClosed();
	}

	private void OnPopupIsOpenChanged(object sender, EventArgs e)
	{
		UpdateIsOpen();
	}

	private CustomPopupPlacement[] PositionPopup(Size popupSize, Size targetSize, Point offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return PositionPopup(popupSize, targetSize, offset, (FrameworkElement)(object)m_presenter);
	}

	internal CustomPopupPlacement[] PositionPopup(Size popupSize, Size targetSize, Point offset, FrameworkElement child)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return CustomPopupPlacementHelper.PositionPopup((CustomPlacementMode)Placement, popupSize, targetSize, offset, child);
	}

	private void CancelAsyncShow()
	{
		m_pendingShow = null;
		if (m_asyncShow != null)
		{
			m_asyncShow.Abort();
			m_asyncShow = null;
		}
	}
}
