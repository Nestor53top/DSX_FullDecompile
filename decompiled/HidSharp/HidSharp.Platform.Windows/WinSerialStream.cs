using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidSharp.Platform.Windows;

internal sealed class WinSerialStream : SysSerialStream
{
	private object _lock = new object();

	private IntPtr _handle;

	private IntPtr _closeEventHandle;

	private SerialSettings _ser;

	private bool _settingsChanged;

	public sealed override int BaudRate
	{
		get
		{
			return _ser.BaudRate;
		}
		set
		{
			_ser.SetBaudRate(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int DataBits
	{
		get
		{
			return _ser.DataBits;
		}
		set
		{
			_ser.SetDataBits(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override SerialParity Parity
	{
		get
		{
			return _ser.Parity;
		}
		set
		{
			_ser.SetParity(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int StopBits
	{
		get
		{
			return _ser.StopBits;
		}
		set
		{
			_ser.SetStopBits(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int ReadTimeout { get; set; }

	public sealed override int WriteTimeout { get; set; }

	internal WinSerialStream(WinSerialDevice device)
		: base(device)
	{
		_closeEventHandle = NativeMethods.CreateManualResetEventOrThrow();
	}

	internal void Init(string devicePath)
	{
		IntPtr intPtr = NativeMethods.CreateFileFromDevice(devicePath, NativeMethods.EFileAccess.Read | NativeMethods.EFileAccess.Write, NativeMethods.EFileShare.None);
		if (intPtr == (IntPtr)(-1))
		{
			int hRForLastWin32Error = Marshal.GetHRForLastWin32Error();
			throw DeviceException.CreateIOException(base.Device, "Unable to open serial device (" + devicePath + ").", hRForLastWin32Error);
		}
		NativeMethods.COMMTIMEOUTS timeouts = new NativeMethods.COMMTIMEOUTS
		{
			ReadIntervalTimeout = uint.MaxValue,
			ReadTotalTimeoutConstant = 4294967294u,
			ReadTotalTimeoutMultiplier = uint.MaxValue
		};
		if (!NativeMethods.SetCommTimeouts(intPtr, out timeouts))
		{
			int hRForLastWin32Error2 = Marshal.GetHRForLastWin32Error();
			NativeMethods.CloseHandle(intPtr);
			throw DeviceException.CreateIOException(base.Device, "Unable to set serial timeouts.", hRForLastWin32Error2);
		}
		_handle = intPtr;
		HandleInitAndOpen();
	}

	~WinSerialStream()
	{
		Close();
		NativeMethods.CloseHandle(_closeEventHandle);
	}

	protected override void Dispose(bool disposing)
	{
		if (HandleClose())
		{
			NativeMethods.SetEvent(_closeEventHandle);
			HandleRelease();
			base.Dispose(disposing);
		}
	}

	internal override void HandleFree()
	{
		NativeMethods.CloseHandle(ref _handle);
		NativeMethods.CloseHandle(ref _closeEventHandle);
	}

	public override void Flush()
	{
		HandleAcquireIfOpenOrFail();
		try
		{
			NativeMethods.FlushFileBuffers(_handle);
		}
		finally
		{
			HandleRelease();
		}
	}

	public unsafe override int Read(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		IntPtr intPtr = NativeMethods.CreateManualResetEventOrThrow();
		HandleAcquireIfOpenOrFail();
		UpdateSettings();
		try
		{
			fixed (byte* ptr = buffer)
			{
				NativeOverlapped* ptr2 = stackalloc NativeOverlapped[1];
				ptr2->EventHandle = intPtr;
				NativeMethods.OverlappedOperation(_handle, intPtr, ReadTimeout, _closeEventHandle, NativeMethods.ReadFile(_handle, ptr + offset, count, IntPtr.Zero, ptr2), ptr2, out var bytesTransferred);
				return (int)bytesTransferred;
			}
		}
		finally
		{
			HandleRelease();
			NativeMethods.CloseHandle(intPtr);
		}
	}

	public unsafe override void Write(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		IntPtr intPtr = NativeMethods.CreateManualResetEventOrThrow();
		HandleAcquireIfOpenOrFail();
		UpdateSettings();
		try
		{
			fixed (byte* ptr = buffer)
			{
				NativeOverlapped* ptr2 = stackalloc NativeOverlapped[1];
				ptr2->EventHandle = intPtr;
				NativeMethods.OverlappedOperation(_handle, intPtr, WriteTimeout, _closeEventHandle, NativeMethods.WriteFile(_handle, ptr + offset, count, IntPtr.Zero, ptr2), ptr2, out var bytesTransferred);
				if (bytesTransferred != count)
				{
					throw new IOException("Write failed.");
				}
			}
		}
		finally
		{
			HandleRelease();
			NativeMethods.CloseHandle(intPtr);
		}
	}

	private static void SetDcbDefaults(ref NativeMethods.DCB dcb)
	{
		dcb.fFlags = 0u;
		dcb.fBinary = true;
	}

	private void UpdateSettings()
	{
		lock (_lock)
		{
			if (_settingsChanged)
			{
				_settingsChanged = false;
				NativeMethods.DCB dcb = new NativeMethods.DCB
				{
					DCBlength = Marshal.SizeOf(typeof(NativeMethods.DCB))
				};
				if (!NativeMethods.GetCommState(_handle, ref dcb))
				{
					int hRForLastWin32Error = Marshal.GetHRForLastWin32Error();
					throw DeviceException.CreateIOException(base.Device, "Failed to get serial state.", hRForLastWin32Error);
				}
				int baudRate = _ser.BaudRate;
				SerialParity parity = _ser.Parity;
				int stopBits = _ser.StopBits;
				SetDcbDefaults(ref dcb);
				checked
				{
					dcb.BaudRate = (uint)baudRate;
					dcb.ByteSize = (byte)_ser.DataBits;
					dcb.Parity = parity switch
					{
						SerialParity.Odd => 1, 
						SerialParity.Even => 2, 
						_ => 0, 
					};
				}
				dcb.StopBits = (byte)((stopBits == 2) ? 2 : 0);
				if (!NativeMethods.SetCommState(_handle, ref dcb))
				{
					int hRForLastWin32Error2 = Marshal.GetHRForLastWin32Error();
					throw DeviceException.CreateIOException(base.Device, "Failed to set serial state.", hRForLastWin32Error2);
				}
				uint flags = 15u;
				if (!NativeMethods.PurgeComm(_handle, flags))
				{
					int hRForLastWin32Error3 = Marshal.GetHRForLastWin32Error();
					throw DeviceException.CreateIOException(base.Device, "Failed to purge serial port.", hRForLastWin32Error3);
				}
			}
		}
	}
}
