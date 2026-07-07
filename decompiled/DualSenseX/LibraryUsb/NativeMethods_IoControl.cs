using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace LibraryUsb;

public class NativeMethods_IoControl
{
	public enum IoErrorCodes
	{
		ERROR_UNKNOWN = -1,
		ERROR_ACCESS_DENIED = 5,
		ERROR_INVALID_PARAMETER = 87,
		ERROR_OPERATION_ABORTED = 995,
		ERROR_IO_INCOMPLETE = 996,
		ERROR_IO_PENDING = 997,
		ERROR_NOACCESS = 998
	}

	public enum IoControlCodes : uint
	{
		IOCTL_STORAGE_EJECT_MEDIA = 2967560u,
		IOCTL_STORAGE_MEDIA_REMOVAL = 2967556u,
		IOCTL_BTH_DISCONNECT_DEVICE = 4259852u,
		IOCTL_HID_ACTIVATE_DEVICE = 720927u,
		IOCTL_HID_DEACTIVATE_DEVICE = 720931u
	}

	public const uint INFINITE = uint.MaxValue;

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeviceIoControl(SafeFileHandle hDevice, IoControlCodes dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, NativeOverlapped lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	public static extern bool GetOverlappedResult(SafeFileHandle hFile, ref NativeOverlapped lpOverlapped, out int lpBytesTransferred, bool bWait);

	[DllImport("kernel32.dll")]
	public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

	[DllImport("kernel32.dll")]
	public static extern bool SetEvent(IntPtr hEvent);

	[DllImport("kernel32.dll")]
	public static extern uint WaitForSingleObject(IntPtr hEvent, uint dwMilliseconds);
}
