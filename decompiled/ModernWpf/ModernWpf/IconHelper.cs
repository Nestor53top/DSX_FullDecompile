using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ModernWpf;

internal static class IconHelper
{
	private static class NativeMethods
	{
		[DllImport("Shell32.dll", CharSet = CharSet.Auto)]
		public static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
	}

	internal const int MAX_PATH = 260;

	public static void GetDefaultIconHandles(IntPtr[] largeIconHandle, IntPtr[] smallIconHandle)
	{
		NativeMethods.ExtractIconEx(GetModuleFileName(default(HandleRef)), 0, largeIconHandle, smallIconHandle, 1u);
	}

	public static bool DestroyIcon(IntPtr icon)
	{
		bool result = NativeMethods.DestroyIcon(icon);
		Marshal.GetLastWin32Error();
		return result;
	}

	private static string GetModuleFileName(HandleRef hModule)
	{
		StringBuilder stringBuilder = new StringBuilder(260);
		while (true)
		{
			int moduleFileName = NativeMethods.GetModuleFileName(hModule, stringBuilder, stringBuilder.Capacity);
			if (moduleFileName == 0)
			{
				throw new Win32Exception();
			}
			if (moduleFileName != stringBuilder.Capacity)
			{
				break;
			}
			stringBuilder.EnsureCapacity(stringBuilder.Capacity * 2);
		}
		return stringBuilder.ToString();
	}
}
