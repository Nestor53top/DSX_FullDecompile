using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Squirrel;

internal static class NativeMethods
{
	public static int GetParentProcessId()
	{
		PROCESS_BASIC_INFORMATION pbi = default(PROCESS_BASIC_INFORMATION);
		IntPtr intPtr = OpenProcess(ProcessAccess.All, bInheritHandle: false, Process.GetCurrentProcess().Id);
		try
		{
			NtQueryInformationProcess(intPtr, PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, pbi.Size, out var _);
		}
		finally
		{
			if (!intPtr.Equals((object?)(nint)IntPtr.Zero))
			{
				CloseHandle(intPtr);
				intPtr = IntPtr.Zero;
			}
		}
		return (int)pbi.InheritedFromUniqueProcessId;
	}

	[DllImport("version.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetFileVersionInfo(string lpszFileName, int dwHandleIgnored, int dwLen, [MarshalAs(UnmanagedType.LPArray)] byte[] lpData);

	[DllImport("version.dll", SetLastError = true)]
	internal static extern int GetFileVersionInfoSize(string lpszFileName, IntPtr dwHandleIgnored);

	[DllImport("version.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out IntPtr pValue, out int len);

	[DllImport("psapi.dll", SetLastError = true)]
	internal static extern bool EnumProcesses(IntPtr pProcessIds, int cb, out int pBytesReturned);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool QueryFullProcessImageName(IntPtr hProcess, [In] int justPassZeroHere, [Out] StringBuilder lpImageFileName, [In][MarshalAs(UnmanagedType.U4)] ref int nSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr OpenProcess(ProcessAccess processAccess, bool bInheritHandle, int processId);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool CloseHandle(IntPtr hHandle);

	[DllImport("NTDLL.DLL", SetLastError = true)]
	internal static extern int NtQueryInformationProcess(IntPtr hProcess, PROCESSINFOCLASS pic, ref PROCESS_BASIC_INFORMATION pbi, int cb, out int pSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

	[DllImport("kernel32.dll")]
	internal static extern IntPtr GetStdHandle(StandardHandles nStdHandle);

	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool AllocConsole();

	[DllImport("kernel32.dll")]
	internal static extern bool AttachConsole(int pid);

	[DllImport("Kernel32.dll", SetLastError = true)]
	internal static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

	[DllImport("Kernel32.dll", SetLastError = true)]
	internal static extern bool UpdateResource(IntPtr handle, string pType, IntPtr pName, short language, [MarshalAs(UnmanagedType.LPArray)] byte[] pData, int dwSize);

	[DllImport("Kernel32.dll", SetLastError = true)]
	internal static extern bool EndUpdateResource(IntPtr handle, bool discard);
}
