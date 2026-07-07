using System;
using System.Runtime.InteropServices;

namespace ModernWpf;

internal static class OSVersionHelper
{
	private struct RTL_OSVERSIONINFOEX
	{
		internal uint dwOSVersionInfoSize;

		internal uint dwMajorVersion;

		internal uint dwMinorVersion;

		internal uint dwBuildNumber;

		internal uint dwPlatformId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string szCSDVersion;
	}

	private static readonly Version _osVersion = GetOSVersion();

	internal static bool IsWindowsNT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

	internal static bool IsWindows8OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 2);

	internal static bool IsWindows10OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0);

	private static Version GetOSVersion()
	{
		RTL_OSVERSIONINFOEX lpVersionInformation = default(RTL_OSVERSIONINFOEX);
		lpVersionInformation.dwOSVersionInfoSize = (uint)Marshal.SizeOf(lpVersionInformation);
		RtlGetVersion(out lpVersionInformation);
		return new Version((int)lpVersionInformation.dwMajorVersion, (int)lpVersionInformation.dwMinorVersion, (int)lpVersionInformation.dwBuildNumber);
	}

	[DllImport("ntdll.dll")]
	private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);
}
