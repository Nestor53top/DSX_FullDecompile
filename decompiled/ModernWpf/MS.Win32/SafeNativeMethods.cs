using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MS.Win32;

internal static class SafeNativeMethods
{
	private class SafeNativeMethodsPrivate
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowRect", ExactSpelling = true, SetLastError = true)]
		public static extern bool IntGetWindowRect(HandleRef hWnd, [In][Out] ref NativeMethods.RECT rect);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ScreenToClient", ExactSpelling = true, SetLastError = true)]
		public static extern int IntScreenToClient(HandleRef hWnd, [In][Out] NativeMethods.POINT pt);
	}

	public static IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags)
	{
		return SafeNativeMethodsPrivate.MonitorFromRect(ref rect, flags);
	}

	internal static void GetWindowRect(HandleRef hWnd, [In][Out] ref NativeMethods.RECT rect)
	{
		if (!SafeNativeMethodsPrivate.IntGetWindowRect(hWnd, ref rect))
		{
			throw new Win32Exception();
		}
	}
}
