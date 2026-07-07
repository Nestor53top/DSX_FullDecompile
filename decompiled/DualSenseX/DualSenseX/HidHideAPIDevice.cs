using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DualSenseX;

internal class HidHideAPIDevice : IDisposable
{
	private const uint IOCTL_GET_WHITELIST = 2147573760u;

	private const uint IOCTL_SET_WHITELIST = 2147573764u;

	private const uint IOCTL_GET_BLACKLIST = 2147573768u;

	private const uint IOCTL_SET_BLACKLIST = 2147573772u;

	private const uint IOCTL_GET_ACTIVE = 2147573776u;

	private const uint IOCTL_SET_ACTIVE = 2147573780u;

	private const string CONTROL_DEVICE_FILENAME = "\\\\.\\HidHide";

	private SafeHandle hidHideHandle;

	public HidHideAPIDevice()
	{
		hidHideHandle = NativeMethods.CreateFile("\\\\.\\HidHide", 2147483648u, 3, IntPtr.Zero, 3, 128u, 0);
	}

	public unsafe bool GetActiveState()
	{
		bool result = false;
		int BytesReturned = 0;
		NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573776u, IntPtr.Zero, 0, new IntPtr(&result), 1, ref BytesReturned, IntPtr.Zero);
		return result;
	}

	public unsafe bool SetActiveState(bool state)
	{
		int BytesReturned = 0;
		NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573780u, new IntPtr(&state), 1, IntPtr.Zero, 0, ref BytesReturned, IntPtr.Zero);
		return false;
	}

	public List<string> GetBlacklist()
	{
		List<string> result = new List<string>();
		int BytesReturned = 0;
		NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573768u, IntPtr.Zero, 0, IntPtr.Zero, 0, ref BytesReturned, IntPtr.Zero);
		if (BytesReturned > 0)
		{
			byte[] array = new byte[BytesReturned];
			int num = BytesReturned;
			BytesReturned = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573768u, IntPtr.Zero, 0, intPtr, num, ref BytesReturned, IntPtr.Zero);
			Marshal.Copy(intPtr, array, 0, num);
			result = Encoding.Unicode.GetString(array).TrimEnd(new char[1]).Split(new char[1])
				.ToList();
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public bool SetBlacklist(List<string> instances)
	{
		int BytesReturned = 0;
		int length;
		IntPtr intPtr = StringListToMultiSzPointer(instances, out length);
		bool result = NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573772u, intPtr, length, IntPtr.Zero, 0, ref BytesReturned, IntPtr.Zero);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public List<string> GetWhitelist()
	{
		List<string> result = new List<string>();
		int BytesReturned = 0;
		NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573760u, IntPtr.Zero, 0, IntPtr.Zero, 0, ref BytesReturned, IntPtr.Zero);
		if (BytesReturned > 0)
		{
			byte[] array = new byte[BytesReturned];
			int num = BytesReturned;
			BytesReturned = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573760u, IntPtr.Zero, 0, intPtr, num, ref BytesReturned, IntPtr.Zero);
			Marshal.Copy(intPtr, array, 0, num);
			result = Encoding.Unicode.GetString(array).TrimEnd(new char[1]).Split(new char[1])
				.ToList();
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public bool SetWhitelist(List<string> instances)
	{
		int BytesReturned = 0;
		int length;
		IntPtr intPtr = StringListToMultiSzPointer(instances, out length);
		bool result = NativeMethods.DeviceIoControl(hidHideHandle.DangerousGetHandle(), 2147573764u, intPtr, length, IntPtr.Zero, 0, ref BytesReturned, IntPtr.Zero);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool IsOpen()
	{
		if (hidHideHandle != null)
		{
			if (!hidHideHandle.IsClosed)
			{
				return !hidHideHandle.IsInvalid;
			}
			return false;
		}
		return false;
	}

	public void Close()
	{
		if (IsOpen())
		{
			hidHideHandle.Close();
			hidHideHandle.Dispose();
			hidHideHandle = null;
		}
	}

	public void Dispose()
	{
		Close();
	}

	private IntPtr StringListToMultiSzPointer(List<string> strList, out int length)
	{
		IEnumerable<byte> seed = new List<byte>();
		seed = strList.Aggregate(seed, (IEnumerable<byte> current, string entry) => current.Concat(Encoding.Unicode.GetBytes(entry)).Concat(Encoding.Unicode.GetBytes(new char[1])));
		seed = seed.Concat(Encoding.Unicode.GetBytes(new char[1]));
		byte[] array = seed.ToArray();
		length = array.Length;
		IntPtr intPtr = Marshal.AllocHGlobal(length);
		Marshal.Copy(array, 0, intPtr, length);
		return intPtr;
	}
}
