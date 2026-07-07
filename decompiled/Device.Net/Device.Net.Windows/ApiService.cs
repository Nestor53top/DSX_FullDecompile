using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Device.Net.Windows;

public class ApiService : IApiService
{
	public ILogger Logger { get; }

	public ApiService(ILogger logger)
	{
		Logger = logger;
	}

	public SafeFileHandle CreateWriteConnection(string deviceId)
	{
		return CreateConnection(deviceId, FileAccessRights.GenericRead | FileAccessRights.GenericWrite, 3u, 3u);
	}

	public SafeFileHandle CreateReadConnection(string deviceId, FileAccessRights desiredAccess)
	{
		return CreateConnection(deviceId, desiredAccess, 3u, 3u);
	}

	public bool AGetCommState(SafeFileHandle hFile, ref Dcb lpDCB)
	{
		return GetCommState(hFile, ref lpDCB);
	}

	public bool APurgeComm(SafeFileHandle hFile, int dwFlags)
	{
		return PurgeComm(hFile, dwFlags);
	}

	public bool ASetCommTimeouts(SafeFileHandle hFile, ref CommTimeouts lpCommTimeouts)
	{
		return SetCommTimeouts(hFile, ref lpCommTimeouts);
	}

	public bool AWriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, int lpOverlapped)
	{
		return WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, out lpNumberOfBytesWritten, lpOverlapped);
	}

	public bool AReadFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out uint lpNumberOfBytesRead, int lpOverlapped)
	{
		return ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, out lpNumberOfBytesRead, lpOverlapped);
	}

	public bool ASetCommState(SafeFileHandle hFile, [In] ref Dcb lpDCB)
	{
		return SetCommState(hFile, ref lpDCB);
	}

	private SafeFileHandle CreateConnection(string deviceId, FileAccessRights desiredAccess, uint shareMode, uint creationDisposition)
	{
		Logger?.Log(string.Format("Calling {0} for DeviceId: {1}. Desired Access: {2}. Share mode: {3}. Creation Disposition: {4}", new object[5] { "CreateFile", deviceId, desiredAccess, shareMode, creationDisposition }), "ApiService", null, LogLevel.Information);
		return APICalls.CreateFile(deviceId, desiredAccess, shareMode, IntPtr.Zero, creationDisposition, 0u, IntPtr.Zero);
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool PurgeComm(SafeFileHandle hFile, int dwFlags);

	[DllImport("kernel32.dll")]
	private static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDCB);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetCommTimeouts(SafeFileHandle hFile, ref CommTimeouts lpCommTimeouts);

	[DllImport("kernel32.dll")]
	private static extern bool SetCommState(SafeFileHandle hFile, [In] ref Dcb lpDCB);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, int lpOverlapped);

	[DllImport("kernel32.dll")]
	private static extern bool ReadFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out uint lpNumberOfBytesRead, int lpOverlapped);
}
