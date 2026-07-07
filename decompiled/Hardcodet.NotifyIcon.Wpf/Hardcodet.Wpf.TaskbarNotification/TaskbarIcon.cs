using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace Hardcodet.Wpf.TaskbarNotification;

public class TaskbarIcon : FrameworkElement, IDisposable
{
	public delegate Hardcodet.Wpf.TaskbarNotification.Interop.Point GetCustomPopupPosition();

	private readonly object lockObject = new object();

	private NotifyIconData iconData;

	private readonly WindowMessageSink messageSink;

	private Action singleClickTimerAction;

	private readonly Timer singleClickTimer;

	private readonly Timer balloonCloseTimer;

	public const string CategoryName = "NotifyIcon";

	private static readonly DependencyPropertyKey TrayPopupResolvedPropertyKey;

	public static readonly DependencyProperty TrayPopupResolvedProperty;

	private static readonly DependencyPropertyKey TrayToolTipResolvedPropertyKey;

	public static readonly DependencyProperty TrayToolTipResolvedProperty;

	private static readonly DependencyPropertyKey CustomBalloonPropertyKey;

	public static readonly DependencyProperty CustomBalloonProperty;

	private Icon icon;

	public static readonly DependencyProperty IconSourceProperty;

	public static readonly DependencyProperty ToolTipTextProperty;

	public static readonly DependencyProperty TrayToolTipProperty;

	public static readonly DependencyProperty TrayPopupProperty;

	public static readonly DependencyProperty MenuActivationProperty;

	public static readonly DependencyProperty PopupActivationProperty;

	public static readonly DependencyProperty DoubleClickCommandProperty;

	public static readonly DependencyProperty DoubleClickCommandParameterProperty;

	public static readonly DependencyProperty DoubleClickCommandTargetProperty;

	public static readonly DependencyProperty LeftClickCommandProperty;

	public static readonly DependencyProperty LeftClickCommandParameterProperty;

	public static readonly DependencyProperty LeftClickCommandTargetProperty;

	public static readonly DependencyProperty NoLeftClickDelayProperty;

	public static readonly RoutedEvent TrayLeftMouseDownEvent;

	public static readonly RoutedEvent TrayRightMouseDownEvent;

	public static readonly RoutedEvent TrayMiddleMouseDownEvent;

	public static readonly RoutedEvent TrayLeftMouseUpEvent;

	public static readonly RoutedEvent TrayRightMouseUpEvent;

	public static readonly RoutedEvent TrayMiddleMouseUpEvent;

	public static readonly RoutedEvent TrayMouseDoubleClickEvent;

	public static readonly RoutedEvent TrayMouseMoveEvent;

	public static readonly RoutedEvent TrayBalloonTipShownEvent;

	public static readonly RoutedEvent TrayBalloonTipClosedEvent;

	public static readonly RoutedEvent TrayBalloonTipClickedEvent;

	public static readonly RoutedEvent TrayContextMenuOpenEvent;

	public static readonly RoutedEvent PreviewTrayContextMenuOpenEvent;

	public static readonly RoutedEvent TrayPopupOpenEvent;

	public static readonly RoutedEvent PreviewTrayPopupOpenEvent;

	public static readonly RoutedEvent TrayToolTipOpenEvent;

	public static readonly RoutedEvent PreviewTrayToolTipOpenEvent;

	public static readonly RoutedEvent TrayToolTipCloseEvent;

	public static readonly RoutedEvent PreviewTrayToolTipCloseEvent;

	public static readonly RoutedEvent PopupOpenedEvent;

	public static readonly RoutedEvent ToolTipOpenedEvent;

	public static readonly RoutedEvent ToolTipCloseEvent;

	public static readonly RoutedEvent BalloonShowingEvent;

	public static readonly RoutedEvent BalloonClosingEvent;

	public static readonly DependencyProperty ParentTaskbarIconProperty;

	private int DoubleClickWaitTime
	{
		get
		{
			if (!NoLeftClickDelay)
			{
				return WinApi.GetDoubleClickTime();
			}
			return 0;
		}
	}

	public bool IsTaskbarIconCreated { get; private set; }

	public bool SupportsCustomToolTips => messageSink.Version == NotifyIconVersion.Vista;

	private bool IsPopupOpen
	{
		get
		{
			Popup trayPopupResolved = TrayPopupResolved;
			ContextMenu contextMenu = ((FrameworkElement)this).ContextMenu;
			Popup customBalloon = CustomBalloon;
			if ((trayPopupResolved == null || !trayPopupResolved.IsOpen) && (contextMenu == null || !contextMenu.IsOpen))
			{
				if (customBalloon != null)
				{
					return customBalloon.IsOpen;
				}
				return false;
			}
			return true;
		}
	}

	public GetCustomPopupPosition CustomPopupPosition { get; set; }

	public bool IsDisposed { get; private set; }

	[Category("NotifyIcon")]
	public Popup TrayPopupResolved => (Popup)((DependencyObject)this).GetValue(TrayPopupResolvedProperty);

	[Category("NotifyIcon")]
	[Browsable(true)]
	[Bindable(true)]
	public ToolTip TrayToolTipResolved => (ToolTip)((DependencyObject)this).GetValue(TrayToolTipResolvedProperty);

	public Popup CustomBalloon => (Popup)((DependencyObject)this).GetValue(CustomBalloonProperty);

	[Browsable(false)]
	public Icon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
			iconData.IconHandle = ((value == null) ? IntPtr.Zero : icon.Handle);
			Util.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Icon);
		}
	}

	[Category("NotifyIcon")]
	[Description("Sets the displayed taskbar icon.")]
	public ImageSource IconSource
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(IconSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IconSourceProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Alternative to a fully blown ToolTip, which is only displayed on Vista and above.")]
	public string ToolTipText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(ToolTipTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ToolTipTextProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Custom UI element that is displayed as a tooltip. Only on Vista and above")]
	public UIElement TrayToolTip
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(TrayToolTipProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TrayToolTipProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Displayed as a Popup if the user clicks on the taskbar icon.")]
	public UIElement TrayPopup
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(TrayPopupProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TrayPopupProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Defines what mouse events display the context menu.")]
	public PopupActivationMode MenuActivation
	{
		get
		{
			return (PopupActivationMode)((DependencyObject)this).GetValue(MenuActivationProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuActivationProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Defines what mouse events display the TaskbarIconPopup.")]
	public PopupActivationMode PopupActivation
	{
		get
		{
			return (PopupActivationMode)((DependencyObject)this).GetValue(PopupActivationProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PopupActivationProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("A command that is being executed if the tray icon is being double-clicked.")]
	public ICommand DoubleClickCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(DoubleClickCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DoubleClickCommandProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.")]
	public object DoubleClickCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(DoubleClickCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DoubleClickCommandParameterProperty, value);
		}
	}

	[Category("NotifyIcon")]
	[Description("The target of the command that is fired if the notify icon is double clicked.")]
	public IInputElement DoubleClickCommandTarget
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (IInputElement)((DependencyObject)this).GetValue(DoubleClickCommandTargetProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DoubleClickCommandTargetProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("A command that is being executed if the tray icon is being left-clicked.")]
	public ICommand LeftClickCommand
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(LeftClickCommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LeftClickCommandProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("The target of the command that is fired if the notify icon is clicked with the left mouse button.")]
	public object LeftClickCommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(LeftClickCommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LeftClickCommandParameterProperty, value);
		}
	}

	[Category("NotifyIcon")]
	[Description("The target of the command that is fired if the notify icon is clicked with the left mouse button.")]
	public IInputElement LeftClickCommandTarget
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (IInputElement)((DependencyObject)this).GetValue(LeftClickCommandTargetProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LeftClickCommandTargetProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	[Description("Set to true to make left clicks work without delay.")]
	public bool NoLeftClickDelay
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(NoLeftClickDelayProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(NoLeftClickDelayProperty, (object)value);
		}
	}

	[Category("NotifyIcon")]
	public event RoutedEventHandler TrayLeftMouseDown
	{
		add
		{
			((UIElement)this).AddHandler(TrayLeftMouseDownEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayLeftMouseDownEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayRightMouseDown
	{
		add
		{
			((UIElement)this).AddHandler(TrayRightMouseDownEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayRightMouseDownEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayMiddleMouseDown
	{
		add
		{
			((UIElement)this).AddHandler(TrayMiddleMouseDownEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayMiddleMouseDownEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayLeftMouseUp
	{
		add
		{
			((UIElement)this).AddHandler(TrayLeftMouseUpEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayLeftMouseUpEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayRightMouseUp
	{
		add
		{
			((UIElement)this).AddHandler(TrayRightMouseUpEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayRightMouseUpEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayMiddleMouseUp
	{
		add
		{
			((UIElement)this).AddHandler(TrayMiddleMouseUpEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayMiddleMouseUpEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayMouseDoubleClick
	{
		add
		{
			((UIElement)this).AddHandler(TrayMouseDoubleClickEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayMouseDoubleClickEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayMouseMove
	{
		add
		{
			((UIElement)this).AddHandler(TrayMouseMoveEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayMouseMoveEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayBalloonTipShown
	{
		add
		{
			((UIElement)this).AddHandler(TrayBalloonTipShownEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayBalloonTipShownEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayBalloonTipClosed
	{
		add
		{
			((UIElement)this).AddHandler(TrayBalloonTipClosedEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayBalloonTipClosedEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayBalloonTipClicked
	{
		add
		{
			((UIElement)this).AddHandler(TrayBalloonTipClickedEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayBalloonTipClickedEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayContextMenuOpen
	{
		add
		{
			((UIElement)this).AddHandler(TrayContextMenuOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayContextMenuOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler PreviewTrayContextMenuOpen
	{
		add
		{
			((UIElement)this).AddHandler(PreviewTrayContextMenuOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(PreviewTrayContextMenuOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayPopupOpen
	{
		add
		{
			((UIElement)this).AddHandler(TrayPopupOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayPopupOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler PreviewTrayPopupOpen
	{
		add
		{
			((UIElement)this).AddHandler(PreviewTrayPopupOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(PreviewTrayPopupOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayToolTipOpen
	{
		add
		{
			((UIElement)this).AddHandler(TrayToolTipOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayToolTipOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler PreviewTrayToolTipOpen
	{
		add
		{
			((UIElement)this).AddHandler(PreviewTrayToolTipOpenEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(PreviewTrayToolTipOpenEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler TrayToolTipClose
	{
		add
		{
			((UIElement)this).AddHandler(TrayToolTipCloseEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(TrayToolTipCloseEvent, (Delegate)(object)value);
		}
	}

	public event RoutedEventHandler PreviewTrayToolTipClose
	{
		add
		{
			((UIElement)this).AddHandler(PreviewTrayToolTipCloseEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(PreviewTrayToolTipCloseEvent, (Delegate)(object)value);
		}
	}

	public TaskbarIcon()
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		messageSink = (Util.IsDesignMode ? WindowMessageSink.CreateEmpty() : new WindowMessageSink(NotifyIconVersion.Win95));
		iconData = NotifyIconData.CreateDefault(messageSink.MessageWindowHandle);
		CreateTaskbarIcon();
		messageSink.MouseEventReceived += OnMouseEvent;
		messageSink.TaskbarCreated += OnTaskbarCreated;
		messageSink.ChangeToolTipStateRequest += OnToolTipChange;
		messageSink.BalloonToolTipChanged += OnBalloonToolTipChanged;
		singleClickTimer = new Timer(DoSingleClickAction);
		balloonCloseTimer = new Timer(CloseBalloonCallback);
		if (Application.Current != null)
		{
			Application.Current.Exit += new ExitEventHandler(OnExit);
		}
	}

	public Hardcodet.Wpf.TaskbarNotification.Interop.Point GetPopupTrayPosition()
	{
		return TrayInfo.GetTrayLocation();
	}

	public void ShowCustomBalloon(UIElement balloon, PopupAnimation animation, int? timeout)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Dispatcher dispatcher = ((DispatcherObject)(object)this).GetDispatcher();
		if (!dispatcher.CheckAccess())
		{
			Action action = delegate
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				ShowCustomBalloon(balloon, animation, timeout);
			};
			dispatcher.Invoke((DispatcherPriority)9, (Delegate)action);
			return;
		}
		if (balloon == null)
		{
			throw new ArgumentNullException("balloon");
		}
		if (timeout.HasValue && timeout < 500)
		{
			string format = "Invalid timeout of {0} milliseconds. Timeout must be at least 500 ms";
			format = string.Format(format, timeout);
			throw new ArgumentOutOfRangeException("timeout", format);
		}
		EnsureNotDisposed();
		lock (lockObject)
		{
			CloseBalloon();
		}
		Popup val = new Popup
		{
			AllowsTransparency = true
		};
		UpdateDataContext((FrameworkElement)(object)val, null, ((FrameworkElement)this).DataContext);
		val.PopupAnimation = animation;
		DependencyObject parent = LogicalTreeHelper.GetParent((DependencyObject)(object)balloon);
		Popup val2 = (Popup)(object)((parent is Popup) ? parent : null);
		if (val2 != null)
		{
			val2.Child = null;
		}
		if (val2 != null)
		{
			string format2 = "Cannot display control [{0}] in a new balloon popup - that control already has a parent. You may consider creating new balloons every time you want to show one.";
			format2 = string.Format(format2, balloon);
			throw new InvalidOperationException(format2);
		}
		val.Child = balloon;
		val.Placement = (PlacementMode)5;
		val.StaysOpen = true;
		Hardcodet.Wpf.TaskbarNotification.Interop.Point point = ((CustomPopupPosition != null) ? CustomPopupPosition() : GetPopupTrayPosition());
		val.HorizontalOffset = point.X - 1;
		val.VerticalOffset = point.Y - 1;
		lock (lockObject)
		{
			SetCustomBalloon(val);
		}
		SetParentTaskbarIcon((DependencyObject)(object)balloon, this);
		RaiseBalloonShowingEvent((DependencyObject)(object)balloon, this);
		val.IsOpen = true;
		if (timeout.HasValue)
		{
			balloonCloseTimer.Change(timeout.Value, -1);
		}
	}

	public void ResetBalloonCloseTimer()
	{
		if (IsDisposed)
		{
			return;
		}
		lock (lockObject)
		{
			balloonCloseTimer.Change(-1, -1);
		}
	}

	public void CloseBalloon()
	{
		if (IsDisposed)
		{
			return;
		}
		Dispatcher dispatcher = ((DispatcherObject)(object)this).GetDispatcher();
		if (!dispatcher.CheckAccess())
		{
			Action action = CloseBalloon;
			dispatcher.Invoke((DispatcherPriority)9, (Delegate)action);
			return;
		}
		lock (lockObject)
		{
			balloonCloseTimer.Change(-1, -1);
			Popup customBalloon = CustomBalloon;
			if (customBalloon == null)
			{
				return;
			}
			UIElement child = customBalloon.Child;
			RoutedEventArgs e = RaiseBalloonClosingEvent((DependencyObject)(object)child, this);
			if (!e.Handled)
			{
				customBalloon.IsOpen = false;
				customBalloon.Child = null;
				if (child != null)
				{
					SetParentTaskbarIcon((DependencyObject)(object)child, null);
				}
			}
			SetCustomBalloon(null);
		}
	}

	private void CloseBalloonCallback(object state)
	{
		if (!IsDisposed)
		{
			Action action = CloseBalloon;
			((DispatcherObject)(object)this).GetDispatcher().Invoke(action);
		}
	}

	private void OnMouseEvent(MouseEvent me)
	{
		if (IsDisposed)
		{
			return;
		}
		switch (me)
		{
		case MouseEvent.MouseMove:
			RaiseTrayMouseMoveEvent();
			return;
		case MouseEvent.IconRightMouseDown:
			RaiseTrayRightMouseDownEvent();
			break;
		case MouseEvent.IconLeftMouseDown:
			RaiseTrayLeftMouseDownEvent();
			break;
		case MouseEvent.IconRightMouseUp:
			RaiseTrayRightMouseUpEvent();
			break;
		case MouseEvent.IconLeftMouseUp:
			RaiseTrayLeftMouseUpEvent();
			break;
		case MouseEvent.IconMiddleMouseDown:
			RaiseTrayMiddleMouseDownEvent();
			break;
		case MouseEvent.IconMiddleMouseUp:
			RaiseTrayMiddleMouseUpEvent();
			break;
		case MouseEvent.IconDoubleClick:
			singleClickTimer.Change(-1, -1);
			RaiseTrayMouseDoubleClickEvent();
			break;
		case MouseEvent.BalloonToolTipClicked:
			RaiseTrayBalloonTipClickedEvent();
			break;
		default:
			throw new ArgumentOutOfRangeException("me", "Missing handler for mouse event flag: " + me);
		}
		Hardcodet.Wpf.TaskbarNotification.Interop.Point cursorPosition = default(Hardcodet.Wpf.TaskbarNotification.Interop.Point);
		if (messageSink.Version == NotifyIconVersion.Vista)
		{
			WinApi.GetPhysicalCursorPos(ref cursorPosition);
		}
		else
		{
			WinApi.GetCursorPos(ref cursorPosition);
		}
		cursorPosition = TrayInfo.GetDeviceCoordinates(cursorPosition);
		bool flag = false;
		if (me.IsMatch(PopupActivation))
		{
			if (me == MouseEvent.IconLeftMouseUp)
			{
				singleClickTimerAction = delegate
				{
					LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, (IInputElement)(((object)LeftClickCommandTarget) ?? ((object)this)));
					ShowTrayPopup(cursorPosition);
				};
				singleClickTimer.Change(DoubleClickWaitTime, -1);
				flag = true;
			}
			else
			{
				ShowTrayPopup(cursorPosition);
			}
		}
		if (me.IsMatch(MenuActivation))
		{
			if (me == MouseEvent.IconLeftMouseUp)
			{
				singleClickTimerAction = delegate
				{
					LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, (IInputElement)(((object)LeftClickCommandTarget) ?? ((object)this)));
					ShowContextMenu(cursorPosition);
				};
				singleClickTimer.Change(DoubleClickWaitTime, -1);
				flag = true;
			}
			else
			{
				ShowContextMenu(cursorPosition);
			}
		}
		if (me == MouseEvent.IconLeftMouseUp && !flag)
		{
			singleClickTimerAction = delegate
			{
				LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, (IInputElement)(((object)LeftClickCommandTarget) ?? ((object)this)));
			};
			singleClickTimer.Change(DoubleClickWaitTime, -1);
		}
	}

	private void OnToolTipChange(bool visible)
	{
		if (TrayToolTipResolved == null)
		{
			return;
		}
		if (visible)
		{
			if (IsPopupOpen)
			{
				return;
			}
			RoutedEventArgs e = RaisePreviewTrayToolTipOpenEvent();
			if (!e.Handled)
			{
				TrayToolTipResolved.IsOpen = true;
				if (TrayToolTip != null)
				{
					RaiseToolTipOpenedEvent((DependencyObject)(object)TrayToolTip);
				}
				RaiseTrayToolTipOpenEvent();
			}
			return;
		}
		RoutedEventArgs e2 = RaisePreviewTrayToolTipCloseEvent();
		if (!e2.Handled)
		{
			if (TrayToolTip != null)
			{
				RaiseToolTipCloseEvent((DependencyObject)(object)TrayToolTip);
			}
			TrayToolTipResolved.IsOpen = false;
			RaiseTrayToolTipCloseEvent();
		}
	}

	private void CreateCustomToolTip()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		UIElement trayToolTip = TrayToolTip;
		ToolTip val = (ToolTip)(object)((trayToolTip is ToolTip) ? trayToolTip : null);
		if (val == null && TrayToolTip != null)
		{
			val = new ToolTip
			{
				Placement = (PlacementMode)7,
				HasDropShadow = false,
				BorderThickness = new Thickness(0.0),
				Background = (Brush)(object)Brushes.Transparent,
				StaysOpen = true,
				Content = TrayToolTip
			};
		}
		else if (val == null && !string.IsNullOrEmpty(ToolTipText))
		{
			val = new ToolTip
			{
				Content = ToolTipText
			};
		}
		if (val != null)
		{
			UpdateDataContext((FrameworkElement)(object)val, null, ((FrameworkElement)this).DataContext);
		}
		SetTrayToolTipResolved(val);
	}

	private void WriteToolTipSettings()
	{
		iconData.ToolTipText = ToolTipText;
		if (messageSink.Version == NotifyIconVersion.Vista && string.IsNullOrEmpty(iconData.ToolTipText) && TrayToolTipResolved != null)
		{
			iconData.ToolTipText = "ToolTip";
		}
		Util.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Tip);
	}

	private void CreatePopup()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		UIElement trayPopup = TrayPopup;
		Popup val = (Popup)(object)((trayPopup is Popup) ? trayPopup : null);
		if (val == null && TrayPopup != null)
		{
			val = new Popup
			{
				AllowsTransparency = true,
				PopupAnimation = (PopupAnimation)0,
				Child = TrayPopup,
				Placement = (PlacementMode)5,
				StaysOpen = false
			};
		}
		if (val != null)
		{
			UpdateDataContext((FrameworkElement)(object)val, null, ((FrameworkElement)this).DataContext);
		}
		SetTrayPopupResolved(val);
	}

	private void ShowTrayPopup(Hardcodet.Wpf.TaskbarNotification.Interop.Point cursorPosition)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		if (IsDisposed)
		{
			return;
		}
		RoutedEventArgs e = RaisePreviewTrayPopupOpenEvent();
		if (e.Handled || TrayPopup == null)
		{
			return;
		}
		TrayPopupResolved.Placement = (PlacementMode)5;
		TrayPopupResolved.HorizontalOffset = cursorPosition.X;
		TrayPopupResolved.VerticalOffset = cursorPosition.Y;
		TrayPopupResolved.IsOpen = true;
		IntPtr intPtr = IntPtr.Zero;
		if (TrayPopupResolved.Child != null)
		{
			HwndSource val = (HwndSource)PresentationSource.FromVisual((Visual)(object)TrayPopupResolved.Child);
			if (val != null)
			{
				intPtr = val.Handle;
			}
		}
		if (intPtr == IntPtr.Zero)
		{
			intPtr = messageSink.MessageWindowHandle;
		}
		WinApi.SetForegroundWindow(intPtr);
		if (TrayPopup != null)
		{
			RaisePopupOpenedEvent((DependencyObject)(object)TrayPopup);
		}
		RaiseTrayPopupOpenEvent();
	}

	private void ShowContextMenu(Hardcodet.Wpf.TaskbarNotification.Interop.Point cursorPosition)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		if (IsDisposed)
		{
			return;
		}
		RoutedEventArgs e = RaisePreviewTrayContextMenuOpenEvent();
		if (!e.Handled && ((FrameworkElement)this).ContextMenu != null)
		{
			((FrameworkElement)this).ContextMenu.Placement = (PlacementMode)5;
			((FrameworkElement)this).ContextMenu.HorizontalOffset = cursorPosition.X;
			((FrameworkElement)this).ContextMenu.VerticalOffset = cursorPosition.Y;
			((FrameworkElement)this).ContextMenu.IsOpen = true;
			IntPtr intPtr = IntPtr.Zero;
			HwndSource val = (HwndSource)PresentationSource.FromVisual((Visual)(object)((FrameworkElement)this).ContextMenu);
			if (val != null)
			{
				intPtr = val.Handle;
			}
			if (intPtr == IntPtr.Zero)
			{
				intPtr = messageSink.MessageWindowHandle;
			}
			WinApi.SetForegroundWindow(intPtr);
			RaiseTrayContextMenuOpenEvent();
		}
	}

	private void OnBalloonToolTipChanged(bool visible)
	{
		if (visible)
		{
			RaiseTrayBalloonTipShownEvent();
		}
		else
		{
			RaiseTrayBalloonTipClosedEvent();
		}
	}

	public void ShowBalloonTip(string title, string message, BalloonIcon symbol)
	{
		lock (lockObject)
		{
			ShowBalloonTip(title, message, symbol.GetBalloonFlag(), IntPtr.Zero);
		}
	}

	public void ShowBalloonTip(string title, string message, Icon customIcon, bool largeIcon = false)
	{
		if (customIcon == null)
		{
			throw new ArgumentNullException("customIcon");
		}
		lock (lockObject)
		{
			BalloonFlags balloonFlags = BalloonFlags.User;
			if (largeIcon)
			{
				balloonFlags |= BalloonFlags.LargeIcon;
			}
			ShowBalloonTip(title, message, balloonFlags, customIcon.Handle);
		}
	}

	private void ShowBalloonTip(string title, string message, BalloonFlags flags, IntPtr balloonIconHandle)
	{
		EnsureNotDisposed();
		iconData.BalloonText = message ?? string.Empty;
		iconData.BalloonTitle = title ?? string.Empty;
		iconData.BalloonFlags = flags;
		iconData.CustomBalloonIconHandle = balloonIconHandle;
		Util.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Icon | IconDataMembers.Info);
	}

	public void HideBalloonTip()
	{
		EnsureNotDisposed();
		iconData.BalloonText = (iconData.BalloonTitle = string.Empty);
		Util.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info);
	}

	private void DoSingleClickAction(object state)
	{
		if (!IsDisposed)
		{
			Action action = singleClickTimerAction;
			if (action != null)
			{
				singleClickTimerAction = null;
				((DispatcherObject)(object)this).GetDispatcher().Invoke(action);
			}
		}
	}

	private void SetVersion()
	{
		iconData.VersionOrTimeout = 4u;
		bool flag = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);
		if (!flag)
		{
			iconData.VersionOrTimeout = 3u;
			flag = Util.WriteIconData(ref iconData, NotifyCommand.SetVersion);
		}
		if (!flag)
		{
			iconData.VersionOrTimeout = 0u;
			flag = Util.WriteIconData(ref iconData, NotifyCommand.SetVersion);
		}
	}

	private void OnTaskbarCreated()
	{
		RemoveTaskbarIcon();
		CreateTaskbarIcon();
	}

	private void CreateTaskbarIcon()
	{
		lock (lockObject)
		{
			if (!IsTaskbarIconCreated && Util.WriteIconData(ref iconData, NotifyCommand.Add, IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip))
			{
				SetVersion();
				messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;
				IsTaskbarIconCreated = true;
			}
		}
	}

	private void RemoveTaskbarIcon()
	{
		lock (lockObject)
		{
			if (IsTaskbarIconCreated)
			{
				Util.WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
				IsTaskbarIconCreated = false;
			}
		}
	}

	private void EnsureNotDisposed()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(((FrameworkElement)this).Name ?? ((object)this).GetType().FullName);
		}
	}

	private void OnExit(object sender, EventArgs e)
	{
		Dispose();
	}

	~TaskbarIcon()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		if (IsDisposed || !disposing)
		{
			return;
		}
		lock (lockObject)
		{
			IsDisposed = true;
			if (Application.Current != null)
			{
				Application.Current.Exit -= new ExitEventHandler(OnExit);
			}
			singleClickTimer.Dispose();
			balloonCloseTimer.Dispose();
			messageSink.Dispose();
			RemoveTaskbarIcon();
		}
	}

	protected void SetTrayPopupResolved(Popup value)
	{
		((DependencyObject)this).SetValue(TrayPopupResolvedPropertyKey, (object)value);
	}

	protected void SetTrayToolTipResolved(ToolTip value)
	{
		((DependencyObject)this).SetValue(TrayToolTipResolvedPropertyKey, (object)value);
	}

	protected void SetCustomBalloon(Popup value)
	{
		((DependencyObject)this).SetValue(CustomBalloonPropertyKey, (object)value);
	}

	private static void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnIconSourcePropertyChanged(e);
	}

	private void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		ImageSource imageSource = (ImageSource)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (!Util.IsDesignMode)
		{
			Icon = imageSource.ToIcon();
		}
	}

	private static void ToolTipTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnToolTipTextPropertyChanged(e);
	}

	private void OnToolTipTextPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		if (TrayToolTip == null)
		{
			ToolTip trayToolTipResolved = TrayToolTipResolved;
			if (trayToolTipResolved == null)
			{
				CreateCustomToolTip();
			}
			else
			{
				((ContentControl)trayToolTipResolved).Content = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
			}
		}
		WriteToolTipSettings();
	}

	private static void TrayToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnTrayToolTipPropertyChanged(e);
	}

	private void OnTrayToolTipPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		CreateCustomToolTip();
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, null);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).NewValue, this);
		}
		WriteToolTipSettings();
	}

	private static void TrayPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnTrayPopupPropertyChanged(e);
	}

	private void OnTrayPopupPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, null);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).NewValue, this);
		}
		CreatePopup();
	}

	private static void VisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnVisibilityPropertyChanged(e);
	}

	private void OnVisibilityPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Visibility val = (Visibility)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if ((int)val == 0)
		{
			CreateTaskbarIcon();
		}
		else
		{
			RemoveTaskbarIcon();
		}
	}

	private void UpdateDataContext(FrameworkElement target, object oldDataContextValue, object newDataContextValue)
	{
		if (target != null && !target.IsDataContextDataBound() && (this == target.DataContext || object.Equals(oldDataContextValue, target.DataContext)))
		{
			target.DataContext = newDataContextValue ?? this;
		}
	}

	private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnDataContextPropertyChanged(e);
	}

	private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		object newValue = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		object oldValue = ((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		UpdateDataContext((FrameworkElement)(object)TrayPopupResolved, oldValue, newValue);
		UpdateDataContext((FrameworkElement)(object)TrayToolTipResolved, oldValue, newValue);
		UpdateDataContext((FrameworkElement)(object)((FrameworkElement)this).ContextMenu, oldValue, newValue);
	}

	private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TaskbarIcon taskbarIcon = (TaskbarIcon)(object)d;
		taskbarIcon.OnContextMenuPropertyChanged(e);
	}

	private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, null);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue != null)
		{
			SetParentTaskbarIcon((DependencyObject)((DependencyPropertyChangedEventArgs)(ref e)).NewValue, this);
		}
		UpdateDataContext((FrameworkElement)(ContextMenu)((DependencyPropertyChangedEventArgs)(ref e)).NewValue, null, ((FrameworkElement)this).DataContext);
	}

	protected RoutedEventArgs RaiseTrayLeftMouseDownEvent()
	{
		return RaiseTrayLeftMouseDownEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayLeftMouseDownEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayLeftMouseDownEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayRightMouseDownEvent()
	{
		return RaiseTrayRightMouseDownEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayRightMouseDownEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayRightMouseDownEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayMiddleMouseDownEvent()
	{
		return RaiseTrayMiddleMouseDownEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayMiddleMouseDownEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayMiddleMouseDownEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayLeftMouseUpEvent()
	{
		return RaiseTrayLeftMouseUpEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayLeftMouseUpEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayLeftMouseUpEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayRightMouseUpEvent()
	{
		return RaiseTrayRightMouseUpEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayRightMouseUpEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayRightMouseUpEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayMiddleMouseUpEvent()
	{
		return RaiseTrayMiddleMouseUpEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayMiddleMouseUpEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayMiddleMouseUpEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayMouseDoubleClickEvent()
	{
		RoutedEventArgs result = RaiseTrayMouseDoubleClickEvent((DependencyObject)(object)this);
		DoubleClickCommand.ExecuteIfEnabled(DoubleClickCommandParameter, (IInputElement)(((object)DoubleClickCommandTarget) ?? ((object)this)));
		return result;
	}

	internal static RoutedEventArgs RaiseTrayMouseDoubleClickEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayMouseDoubleClickEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayMouseMoveEvent()
	{
		return RaiseTrayMouseMoveEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayMouseMoveEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayMouseMoveEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayBalloonTipShownEvent()
	{
		return RaiseTrayBalloonTipShownEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayBalloonTipShownEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayBalloonTipShownEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayBalloonTipClosedEvent()
	{
		return RaiseTrayBalloonTipClosedEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayBalloonTipClosedEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayBalloonTipClosedEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayBalloonTipClickedEvent()
	{
		return RaiseTrayBalloonTipClickedEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayBalloonTipClickedEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayBalloonTipClickedEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayContextMenuOpenEvent()
	{
		return RaiseTrayContextMenuOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayContextMenuOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayContextMenuOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaisePreviewTrayContextMenuOpenEvent()
	{
		return RaisePreviewTrayContextMenuOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaisePreviewTrayContextMenuOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(PreviewTrayContextMenuOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayPopupOpenEvent()
	{
		return RaiseTrayPopupOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayPopupOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayPopupOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaisePreviewTrayPopupOpenEvent()
	{
		return RaisePreviewTrayPopupOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaisePreviewTrayPopupOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(PreviewTrayPopupOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayToolTipOpenEvent()
	{
		return RaiseTrayToolTipOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayToolTipOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayToolTipOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaisePreviewTrayToolTipOpenEvent()
	{
		return RaisePreviewTrayToolTipOpenEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaisePreviewTrayToolTipOpenEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(PreviewTrayToolTipOpenEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaiseTrayToolTipCloseEvent()
	{
		return RaiseTrayToolTipCloseEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaiseTrayToolTipCloseEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(TrayToolTipCloseEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	protected RoutedEventArgs RaisePreviewTrayToolTipCloseEvent()
	{
		return RaisePreviewTrayToolTipCloseEvent((DependencyObject)(object)this);
	}

	internal static RoutedEventArgs RaisePreviewTrayToolTipCloseEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(PreviewTrayToolTipCloseEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static void AddPopupOpenedHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.AddHandler(element, PopupOpenedEvent, (Delegate)(object)handler);
	}

	public static void RemovePopupOpenedHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.RemoveHandler(element, PopupOpenedEvent, (Delegate)(object)handler);
	}

	internal static RoutedEventArgs RaisePopupOpenedEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(PopupOpenedEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static void AddToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.AddHandler(element, ToolTipOpenedEvent, (Delegate)(object)handler);
	}

	public static void RemoveToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.RemoveHandler(element, ToolTipOpenedEvent, (Delegate)(object)handler);
	}

	internal static RoutedEventArgs RaiseToolTipOpenedEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(ToolTipOpenedEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static void AddToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.AddHandler(element, ToolTipCloseEvent, (Delegate)(object)handler);
	}

	public static void RemoveToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.RemoveHandler(element, ToolTipCloseEvent, (Delegate)(object)handler);
	}

	internal static RoutedEventArgs RaiseToolTipCloseEvent(DependencyObject target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(ToolTipCloseEvent);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static void AddBalloonShowingHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.AddHandler(element, BalloonShowingEvent, (Delegate)(object)handler);
	}

	public static void RemoveBalloonShowingHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.RemoveHandler(element, BalloonShowingEvent, (Delegate)(object)handler);
	}

	internal static RoutedEventArgs RaiseBalloonShowingEvent(DependencyObject target, TaskbarIcon source)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(BalloonShowingEvent, (object)source);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static void AddBalloonClosingHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.AddHandler(element, BalloonClosingEvent, (Delegate)(object)handler);
	}

	public static void RemoveBalloonClosingHandler(DependencyObject element, RoutedEventHandler handler)
	{
		RoutedEventHelper.RemoveHandler(element, BalloonClosingEvent, (Delegate)(object)handler);
	}

	internal static RoutedEventArgs RaiseBalloonClosingEvent(DependencyObject target, TaskbarIcon source)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		if (target == null)
		{
			return null;
		}
		RoutedEventArgs e = new RoutedEventArgs(BalloonClosingEvent, (object)source);
		RoutedEventHelper.RaiseEvent(target, e);
		return e;
	}

	public static TaskbarIcon GetParentTaskbarIcon(DependencyObject d)
	{
		return (TaskbarIcon)d.GetValue(ParentTaskbarIconProperty);
	}

	public static void SetParentTaskbarIcon(DependencyObject d, TaskbarIcon value)
	{
		d.SetValue(ParentTaskbarIconProperty, (object)value);
	}

	static TaskbarIcon()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Expected O, but got Unknown
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Expected O, but got Unknown
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Expected O, but got Unknown
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Expected O, but got Unknown
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Expected O, but got Unknown
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Expected O, but got Unknown
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Expected O, but got Unknown
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Expected O, but got Unknown
		TrayPopupResolvedPropertyKey = DependencyProperty.RegisterReadOnly("TrayPopupResolved", typeof(Popup), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		TrayPopupResolvedProperty = TrayPopupResolvedPropertyKey.DependencyProperty;
		TrayToolTipResolvedPropertyKey = DependencyProperty.RegisterReadOnly("TrayToolTipResolved", typeof(ToolTip), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		TrayToolTipResolvedProperty = TrayToolTipResolvedPropertyKey.DependencyProperty;
		CustomBalloonPropertyKey = DependencyProperty.RegisterReadOnly("CustomBalloon", typeof(Popup), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		CustomBalloonProperty = CustomBalloonPropertyKey.DependencyProperty;
		IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(IconSourcePropertyChanged)));
		ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(ToolTipTextPropertyChanged)));
		TrayToolTipProperty = DependencyProperty.Register("TrayToolTip", typeof(UIElement), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(TrayToolTipPropertyChanged)));
		TrayPopupProperty = DependencyProperty.Register("TrayPopup", typeof(UIElement), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(TrayPopupPropertyChanged)));
		MenuActivationProperty = DependencyProperty.Register("MenuActivation", typeof(PopupActivationMode), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)PopupActivationMode.RightClick));
		PopupActivationProperty = DependencyProperty.Register("PopupActivation", typeof(PopupActivationMode), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)PopupActivationMode.LeftClick));
		DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		DoubleClickCommandParameterProperty = DependencyProperty.Register("DoubleClickCommandParameter", typeof(object), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		DoubleClickCommandTargetProperty = DependencyProperty.Register("DoubleClickCommandTarget", typeof(IInputElement), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		LeftClickCommandProperty = DependencyProperty.Register("LeftClickCommand", typeof(ICommand), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		LeftClickCommandParameterProperty = DependencyProperty.Register("LeftClickCommandParameter", typeof(object), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		LeftClickCommandTargetProperty = DependencyProperty.Register("LeftClickCommandTarget", typeof(IInputElement), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
		NoLeftClickDelayProperty = DependencyProperty.Register("NoLeftClickDelay", typeof(bool), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
		TrayLeftMouseDownEvent = EventManager.RegisterRoutedEvent("TrayLeftMouseDown", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayRightMouseDownEvent = EventManager.RegisterRoutedEvent("TrayRightMouseDown", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayMiddleMouseDownEvent = EventManager.RegisterRoutedEvent("TrayMiddleMouseDown", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayLeftMouseUpEvent = EventManager.RegisterRoutedEvent("TrayLeftMouseUp", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayRightMouseUpEvent = EventManager.RegisterRoutedEvent("TrayRightMouseUp", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayMiddleMouseUpEvent = EventManager.RegisterRoutedEvent("TrayMiddleMouseUp", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayMouseDoubleClickEvent = EventManager.RegisterRoutedEvent("TrayMouseDoubleClick", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayMouseMoveEvent = EventManager.RegisterRoutedEvent("TrayMouseMove", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayBalloonTipShownEvent = EventManager.RegisterRoutedEvent("TrayBalloonTipShown", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayBalloonTipClosedEvent = EventManager.RegisterRoutedEvent("TrayBalloonTipClosed", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayBalloonTipClickedEvent = EventManager.RegisterRoutedEvent("TrayBalloonTipClicked", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayContextMenuOpenEvent = EventManager.RegisterRoutedEvent("TrayContextMenuOpen", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		PreviewTrayContextMenuOpenEvent = EventManager.RegisterRoutedEvent("PreviewTrayContextMenuOpen", (RoutingStrategy)0, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayPopupOpenEvent = EventManager.RegisterRoutedEvent("TrayPopupOpen", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		PreviewTrayPopupOpenEvent = EventManager.RegisterRoutedEvent("PreviewTrayPopupOpen", (RoutingStrategy)0, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayToolTipOpenEvent = EventManager.RegisterRoutedEvent("TrayToolTipOpen", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		PreviewTrayToolTipOpenEvent = EventManager.RegisterRoutedEvent("PreviewTrayToolTipOpen", (RoutingStrategy)0, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		TrayToolTipCloseEvent = EventManager.RegisterRoutedEvent("TrayToolTipClose", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		PreviewTrayToolTipCloseEvent = EventManager.RegisterRoutedEvent("PreviewTrayToolTipClose", (RoutingStrategy)0, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		PopupOpenedEvent = EventManager.RegisterRoutedEvent("PopupOpened", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		ToolTipOpenedEvent = EventManager.RegisterRoutedEvent("ToolTipOpened", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		ToolTipCloseEvent = EventManager.RegisterRoutedEvent("ToolTipClose", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		BalloonShowingEvent = EventManager.RegisterRoutedEvent("BalloonShowing", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		BalloonClosingEvent = EventManager.RegisterRoutedEvent("BalloonClosing", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(TaskbarIcon));
		ParentTaskbarIconProperty = DependencyProperty.RegisterAttached("ParentTaskbarIcon", typeof(TaskbarIcon), typeof(TaskbarIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)32));
		PropertyMetadata val = new PropertyMetadata((object)(Visibility)0, new PropertyChangedCallback(VisibilityPropertyChanged));
		UIElement.VisibilityProperty.OverrideMetadata(typeof(TaskbarIcon), val);
		val = (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(DataContextPropertyChanged));
		FrameworkElement.DataContextProperty.OverrideMetadata(typeof(TaskbarIcon), val);
		val = (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(ContextMenuPropertyChanged));
		FrameworkElement.ContextMenuProperty.OverrideMetadata(typeof(TaskbarIcon), val);
	}
}
