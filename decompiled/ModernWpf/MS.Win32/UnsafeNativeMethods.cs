using System;
using System.Runtime.InteropServices;

namespace MS.Win32;

internal class UnsafeNativeMethods
{
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

	[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ClientToScreen", ExactSpelling = true, SetLastError = true)]
	private static extern int IntClientToScreen(HandleRef hWnd, [In][Out] NativeMethods.POINT pt);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr GetActiveWindow();
}
