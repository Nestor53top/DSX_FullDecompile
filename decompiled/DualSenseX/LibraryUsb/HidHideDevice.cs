using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace LibraryUsb;

public class HidHideDevice
{
	public enum IO_FUNCTION : uint
	{
		IOCTL_GET_WHITELIST = 2048u,
		IOCTL_SET_WHITELIST,
		IOCTL_GET_BLACKLIST,
		IOCTL_SET_BLACKLIST,
		IOCTL_GET_ACTIVE,
		IOCTL_SET_ACTIVE
	}

	public enum IO_METHOD : uint
	{
		METHOD_BUFFERED,
		METHOD_IN_DIRECT,
		METHOD_OUT_DIRECT,
		METHOD_NEITHER
	}

	public enum FILE_DEVICE_TYPE : uint
	{
		DEVICE_TYPE_HIDHIDE = 32769u
	}

	public enum FILE_ACCESS_DATA : uint
	{
		FILE_READ_DATA = 1u,
		FILE_WRITE_DATA = 2u,
		FILE_APPEND_DATA = 4u
	}

	public bool Connected;

	public bool Installed;

	public bool Exclusive;

	private SafeFileHandle FileHandle;

	public HidHideDevice()
	{
		try
		{
			OpenDevice();
		}
		catch (Exception)
		{
		}
	}

	private bool OpenDevice()
	{
		try
		{
			NativeMethods_File.FileShareMode dwShareMode = NativeMethods_File.FileShareMode.FILE_SHARE_NONE;
			NativeMethods_File.FileShareMode dwShareMode2 = NativeMethods_File.FileShareMode.FILE_SHARE_READ_WRITE;
			NativeMethods_File.FileDesiredAccess dwDesiredAccess = (NativeMethods_File.FileDesiredAccess)3221225472u;
			NativeMethods_File.FileCreationDisposition dwCreationDisposition = NativeMethods_File.FileCreationDisposition.OPEN_EXISTING;
			NativeMethods_File.FileFlagsAndAttributes dwFlagsAndAttributes = (NativeMethods_File.FileFlagsAndAttributes)1610612736u;
			FileHandle = NativeMethods_File.CreateFile("\\\\.\\HidHide", dwDesiredAccess, dwShareMode, IntPtr.Zero, dwCreationDisposition, dwFlagsAndAttributes, 0u);
			Exclusive = true;
			if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
			{
				FileHandle = NativeMethods_File.CreateFile("\\\\.\\HidHide", dwDesiredAccess, dwShareMode2, IntPtr.Zero, dwCreationDisposition, dwFlagsAndAttributes, 0u);
				Exclusive = false;
			}
			if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
			{
				Connected = false;
				Installed = false;
				return false;
			}
			Connected = true;
			Installed = true;
			return true;
		}
		catch (Exception)
		{
			Connected = false;
			Installed = false;
			return false;
		}
	}

	public bool CloseDevice()
	{
		try
		{
			if (FileHandle != null)
			{
				FileHandle.Dispose();
				FileHandle = null;
			}
			Connected = false;
			Exclusive = false;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public uint CTL_CODE(FILE_DEVICE_TYPE DeviceType, FILE_ACCESS_DATA Access, IO_FUNCTION Function, IO_METHOD Method)
	{
		return ((uint)DeviceType << 16) | ((uint)Access << 14) | ((uint)Function << 2) | (uint)Method;
	}

	private string DosPathToDevicePath(string dosPath)
	{
		try
		{
			string text = Path.GetPathRoot(dosPath).Replace("\\", string.Empty);
			StringBuilder stringBuilder = new StringBuilder(1024);
			NativeMethods_File.QueryDosDevice(text, stringBuilder, 1024);
			return dosPath.Replace(text, stringBuilder.ToString());
		}
		catch
		{
			return dosPath;
		}
	}

	public bool DeviceHideToggle(bool enableHide)
	{
		int num = 1;
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			if (!Connected)
			{
				return false;
			}
			if (enableHide)
			{
				Marshal.WriteByte(intPtr, 1);
			}
			else
			{
				Marshal.WriteByte(intPtr, 0);
			}
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_ACTIVE, IO_METHOD.METHOD_BUFFERED);
			int lpBytesReturned;
			return NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, intPtr, num, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) && lpBytesReturned > 0;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public List<string> ListDeviceGet()
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!Connected)
			{
				return null;
			}
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_GET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);
			NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, IntPtr.Zero, 0, IntPtr.Zero, 0, out var lpBytesReturned, IntPtr.Zero);
			if (lpBytesReturned < 10)
			{
				return new List<string>();
			}
			intPtr = Marshal.AllocHGlobal(lpBytesReturned);
			NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, IntPtr.Zero, 0, intPtr, lpBytesReturned, out var _, IntPtr.Zero);
			return MultiSzPointerToStringArray(intPtr, lpBytesReturned).ToList();
		}
		catch (Exception)
		{
			return new List<string>();
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public bool ListDeviceReset()
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!Connected)
			{
				return false;
			}
			List<string> stringArray = new List<string>();
			intPtr = StringArrayToMultiSzPointer(stringArray, out var length);
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);
			int lpBytesReturned;
			return NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, intPtr, length, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) && lpBytesReturned > 0;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public async Task<bool> ListDeviceAdd(string pathString)
	{
		IntPtr controlIntPtr = IntPtr.Zero;
		try
		{
			if (!Connected)
			{
				return false;
			}
			List<string> list = ListDeviceGet();
			if (list.Contains(pathString))
			{
				return true;
			}
			list.Add(pathString);
			controlIntPtr = StringArrayToMultiSzPointer(list, out var length);
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);
			int lpBytesReturned;
			bool hideResult = NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, controlIntPtr, length, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) && lpBytesReturned > 0;
			if (hideResult)
			{
				await Task.Delay(500);
			}
			return hideResult;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (controlIntPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(controlIntPtr);
			}
		}
	}

	public bool ListApplicationReset()
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!Connected)
			{
				return false;
			}
			List<string> stringArray = new List<string>();
			intPtr = StringArrayToMultiSzPointer(stringArray, out var length);
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_WHITELIST, IO_METHOD.METHOD_BUFFERED);
			int lpBytesReturned;
			return NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, intPtr, length, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) && lpBytesReturned > 0;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public bool ListApplicationAdd(string pathString)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!Connected)
			{
				return false;
			}
			List<string> list = new List<string>();
			list.Add(DosPathToDevicePath(pathString));
			intPtr = StringArrayToMultiSzPointer(list, out var length);
			uint dwIoControlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_WHITELIST, IO_METHOD.METHOD_BUFFERED);
			int lpBytesReturned;
			return NativeMethods_IoControl.DeviceIoControl(FileHandle, dwIoControlCode, intPtr, length, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) && lpBytesReturned > 0;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public IntPtr StringArrayToMultiSzPointer(IEnumerable<string> stringArray, out int length)
	{
		try
		{
			if (!stringArray.Any())
			{
				length = 0;
				return IntPtr.Zero;
			}
			IEnumerable<byte> first = new List<byte>();
			byte[] bytes = Encoding.Unicode.GetBytes(new char[1]);
			foreach (string item in stringArray)
			{
				first = first.Concat(Encoding.Unicode.GetBytes(item)).Concat(bytes);
			}
			first = first.Concat(bytes);
			byte[] array = first.ToArray();
			IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
			Marshal.Copy(array, 0, intPtr, array.Length);
			length = array.Length;
			return intPtr;
		}
		catch
		{
			length = 0;
			return IntPtr.Zero;
		}
	}

	public IEnumerable<string> MultiSzPointerToStringArray(IntPtr buffer, int length)
	{
		try
		{
			byte[] array = new byte[length];
			Marshal.Copy(buffer, array, 0, length);
			return Encoding.Unicode.GetString(array).TrimEnd(new char[1]).Split(new char[1]);
		}
		catch
		{
		}
		return null;
	}
}
