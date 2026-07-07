using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Standard;

internal static class NativeMethods
{
	[DllImport("user32.dll", EntryPoint = "GetMonitorInfo", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In][Out] MONITORINFO lpmi);

	public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
	{
		MONITORINFO mONITORINFO = new MONITORINFO();
		if (!_GetMonitorInfo(hMonitor, mONITORINFO))
		{
			throw new Win32Exception();
		}
		return mONITORINFO;
	}

	[DllImport("user32.dll")]
	public static extern int GetSystemMetrics(SM nIndex);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

	public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
	{
		WINDOWPLACEMENT wINDOWPLACEMENT = new WINDOWPLACEMENT();
		if (GetWindowPlacement(hwnd, wINDOWPLACEMENT))
		{
			return wINDOWPLACEMENT;
		}
		throw new Win32Exception();
	}
}
