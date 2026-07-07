using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Device.Net.Windows;

public static class APICalls
{
	public const int DigcfDeviceinterface = 16;

	public const int DigcfPresent = 2;

	public const uint FileShareRead = 1u;

	public const uint FileShareWrite = 2u;

	public const uint OpenExisting = 3u;

	public const int FileAttributeNormal = 128;

	public const int FileFlagOverlapped = 1073741824;

	public const int ERROR_NO_MORE_ITEMS = 259;

	public const int PURGE_TXCLEAR = 4;

	public const int PURGE_RXCLEAR = 8;

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern SafeFileHandle CreateFile(string lpFileName, FileAccessRights dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, int lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, int lpOverlapped);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SpDeviceInterfaceData deviceInterfaceData, ref SpDeviceInterfaceDetailData deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, ref SpDeviceInfoData deviceInfoData);
}
