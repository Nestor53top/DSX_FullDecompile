using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using MS.Win32;
using Standard;

namespace ModernWpf.Controls.Primitives;

internal class MaximizedWindowFixer
{
	private enum WindowMessage
	{
		WM_SETTINGCHANGE = 26,
		WM_WINDOWPOSCHANGING = 70,
		WM_WINDOWPOSCHANGED = 71
	}

	private enum ABEdge
	{
		ABE_LEFT,
		ABE_TOP,
		ABE_RIGHT,
		ABE_BOTTOM
	}

	private enum ABMsg
	{
		ABM_GETSTATE = 4,
		ABM_GETTASKBARPOS
	}

	private struct APPBARDATA
	{
		public int cbSize;

		public IntPtr hWnd;

		public int uCallbackMessage;

		public int uEdge;

		public RECT rc;

		public bool lParam;
	}

	public static readonly DependencyProperty MaximizedWindowFixerProperty = DependencyProperty.RegisterAttached("MaximizedWindowFixer", typeof(MaximizedWindowFixer), typeof(MaximizedWindowFixer), new PropertyMetadata(new PropertyChangedCallback(OnMaximizedWindowFixerChanged)));

	private const int MONITOR_DEFAULTTONEAREST = 2;

	private const int MONITORINFOF_PRIMARY = 1;

	private Window _window;

	private IntPtr _hwnd;

	private HwndSource _hwndSource;

	private Thickness? _maximizedWindowBorder;

	private bool _isWindowPosAdjusted;

	private Thickness MaximizedWindowBorder
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Thickness valueOrDefault = _maximizedWindowBorder.GetValueOrDefault();
			if (!_maximizedWindowBorder.HasValue)
			{
				valueOrDefault = GetMaximizedWindowBorder();
				_maximizedWindowBorder = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	private bool IsWindowPosAdjusted
	{
		get
		{
			return _isWindowPosAdjusted;
		}
		set
		{
			if (_isWindowPosAdjusted != value)
			{
				_isWindowPosAdjusted = value;
				InvalidateMaximizedWindowBorder();
			}
		}
	}

	public static MaximizedWindowFixer GetMaximizedWindowFixer(Window window)
	{
		return (MaximizedWindowFixer)((DependencyObject)window).GetValue(MaximizedWindowFixerProperty);
	}

	public static void SetMaximizedWindowFixer(Window window, MaximizedWindowFixer value)
	{
		((DependencyObject)window).SetValue(MaximizedWindowFixerProperty, (object)value);
	}

	private static void OnMaximizedWindowFixerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is MaximizedWindowFixer maximizedWindowFixer)
		{
			maximizedWindowFixer.UnsetWindow();
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is MaximizedWindowFixer maximizedWindowFixer2)
		{
			maximizedWindowFixer2.SetWindow((Window)d);
		}
	}

	private void SetWindow(Window window)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		UnsubscribeWindowEvents();
		_window = window;
		_hwnd = new WindowInteropHelper(window).Handle;
		_window.StateChanged += WindowStateChanged;
		_window.DpiChanged += new DpiChangedEventHandler(WindowDpiChanged);
		_window.Closed += WindowClosed;
		if (_hwnd != IntPtr.Zero)
		{
			WindowSourceInitialized(null, null);
		}
		else
		{
			_window.SourceInitialized += WindowSourceInitialized;
		}
	}

	private void UnsetWindow()
	{
		UnsubscribeWindowEvents();
	}

	private void UnsubscribeWindowEvents()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		if (_window != null)
		{
			_window.SourceInitialized -= WindowSourceInitialized;
			_window.StateChanged -= WindowStateChanged;
			_window.DpiChanged -= new DpiChangedEventHandler(WindowDpiChanged);
			_window.Closed -= WindowClosed;
			((DependencyObject)_window).ClearValue(Control.PaddingProperty);
			_window = null;
		}
		if (_hwndSource != null)
		{
			_hwndSource.RemoveHook(new HwndSourceHook(WindowFilterMessage));
			_hwndSource = null;
		}
		_hwnd = IntPtr.Zero;
		_maximizedWindowBorder = null;
		_isWindowPosAdjusted = false;
	}

	private void WindowSourceInitialized(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		_hwnd = new WindowInteropHelper(_window).Handle;
		_hwndSource = HwndSource.FromHwnd(_hwnd);
		_hwndSource.AddHook(new HwndSourceHook(WindowFilterMessage));
		UpdateWindowPadding();
	}

	private void WindowStateChanged(object sender, EventArgs e)
	{
		UpdateWindowPadding();
	}

	private void WindowDpiChanged(object sender, DpiChangedEventArgs e)
	{
		InvalidateMaximizedWindowBorder();
		UpdateWindowPadding();
	}

	private void WindowClosed(object sender, EventArgs e)
	{
		UnsetWindow();
	}

	private IntPtr WindowFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		IntPtr zero = IntPtr.Zero;
		switch ((WindowMessage)msg)
		{
		case WindowMessage.WM_SETTINGCHANGE:
			InvalidateMaximizedWindowBorder();
			UpdateWindowPadding();
			break;
		case WindowMessage.WM_WINDOWPOSCHANGING:
			OnWindowPosChanging(lParam);
			break;
		case WindowMessage.WM_WINDOWPOSCHANGED:
			if (!_maximizedWindowBorder.HasValue)
			{
				UpdateWindowPadding();
			}
			break;
		}
		return zero;
	}

	private void OnWindowPosChanging(IntPtr lParam)
	{
		WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
		if ((pos.flags & 1) != 0)
		{
			return;
		}
		bool isWindowPosAdjusted = false;
		if (Standard.NativeMethods.GetWindowPlacement(pos.hwnd).showCmd == SW.SHOWMAXIMIZED && GetTaskbarAutoHide(out var edge))
		{
			MS.Win32.NativeMethods.RECT rect = new MS.Win32.NativeMethods.RECT(pos.x, pos.y, pos.x + pos.cx, pos.y + pos.cy);
			IntPtr intPtr = SafeNativeMethods.MonitorFromRect(ref rect, 2);
			if (intPtr != IntPtr.Zero)
			{
				MONITORINFO monitorInfo = Standard.NativeMethods.GetMonitorInfo(intPtr);
				if ((monitorInfo.dwFlags & 1) != 0)
				{
					if (pos.x < 0 && pos.y < 0 && pos.cx > monitorInfo.rcMonitor.Width && pos.cy > monitorInfo.rcMonitor.Height)
					{
						AdjustWindowPosForTaskbarAutoHide(ref pos, edge);
						Marshal.StructureToPtr(pos, lParam, fDeleteOld: true);
						isWindowPosAdjusted = true;
					}
					else if (pos.x == 0 && pos.y == 0)
					{
						isWindowPosAdjusted = true;
					}
				}
			}
		}
		IsWindowPosAdjusted = isWindowPosAdjusted;
	}

	private Thickness GetMaximizedWindowBorder()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (IsWindowPosAdjusted)
		{
			return default(Thickness);
		}
		DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)_window);
		double dpiScaleX = ((DpiScale)(ref dpi)).DpiScaleX;
		double dpiScaleY = ((DpiScale)(ref dpi)).DpiScaleY;
		int systemMetrics = Standard.NativeMethods.GetSystemMetrics(SM.CXFRAME);
		int systemMetrics2 = Standard.NativeMethods.GetSystemMetrics(SM.CYFRAME);
		int systemMetrics3 = Standard.NativeMethods.GetSystemMetrics(SM.CXPADDEDBORDER);
		Size val = DpiHelper.DeviceSizeToLogical(new Size((double)(systemMetrics + systemMetrics3), (double)(systemMetrics2 + systemMetrics3)), dpiScaleX, dpiScaleY);
		return new Thickness(((Size)(ref val)).Width, ((Size)(ref val)).Height, ((Size)(ref val)).Width, ((Size)(ref val)).Height);
	}

	private void InvalidateMaximizedWindowBorder()
	{
		_maximizedWindowBorder = null;
	}

	private void UpdateWindowPadding()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (_hwndSource != null && !((PresentationSource)_hwndSource).IsDisposed && _hwndSource.CompositionTarget != null)
		{
			if ((int)_window.WindowState == 2)
			{
				((Control)_window).Padding = MaximizedWindowBorder;
			}
			else
			{
				((DependencyObject)_window).ClearValue(Control.PaddingProperty);
			}
		}
	}

	private static bool GetTaskbarAutoHide(out ABEdge edge)
	{
		IntPtr intPtr = FindWindow("Shell_TrayWnd", null);
		if (intPtr != IntPtr.Zero)
		{
			APPBARDATA pData = default(APPBARDATA);
			pData.cbSize = Marshal.SizeOf(pData);
			pData.hWnd = intPtr;
			SHAppBarMessage(ABMsg.ABM_GETTASKBARPOS, ref pData);
			bool flag = Convert.ToBoolean(SHAppBarMessage(ABMsg.ABM_GETSTATE, ref pData));
			edge = (flag ? GetEdge(pData.rc) : ABEdge.ABE_LEFT);
			return flag;
		}
		edge = ABEdge.ABE_LEFT;
		return false;
		static ABEdge GetEdge(RECT rc)
		{
			if (rc.Top == rc.Left && rc.Bottom > rc.Right)
			{
				return ABEdge.ABE_LEFT;
			}
			if (rc.Top == rc.Left && rc.Bottom < rc.Right)
			{
				return ABEdge.ABE_TOP;
			}
			if (rc.Top > rc.Left)
			{
				return ABEdge.ABE_BOTTOM;
			}
			return ABEdge.ABE_RIGHT;
		}
	}

	private static void AdjustWindowPosForTaskbarAutoHide(ref WINDOWPOS pos, ABEdge edge)
	{
		pos.cx += pos.x * 2;
		pos.cy += pos.y * 2;
		pos.x = 0;
		pos.y = 0;
		switch (edge)
		{
		case ABEdge.ABE_LEFT:
			pos.x = 2;
			pos.cx -= 2;
			break;
		case ABEdge.ABE_TOP:
			pos.y = 2;
			pos.cy -= 2;
			break;
		case ABEdge.ABE_RIGHT:
			pos.cx -= 2;
			break;
		case ABEdge.ABE_BOTTOM:
			pos.cy -= 2;
			break;
		}
	}

	[DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
	private static extern uint SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

	[DllImport("user32", CharSet = CharSet.Auto)]
	private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
}
