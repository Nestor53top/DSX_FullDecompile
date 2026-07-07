using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace HidSharp.Platform.Windows;

internal sealed class WinHidStream : SysHidStream
{
	private object _readSync = new object();

	private object _writeSync = new object();

	private byte[] _readBuffer;

	private byte[] _writeBuffer;

	private IntPtr _handle;

	private IntPtr _closeEventHandle;

	internal WinHidStream(WinHidDevice device)
		: base(device)
	{
		_closeEventHandle = NativeMethods.CreateManualResetEventOrThrow();
	}

	~WinHidStream()
	{
		Close();
		NativeMethods.CloseHandle(_closeEventHandle);
	}

	internal void Init(string path)
	{
		IntPtr intPtr = NativeMethods.CreateFileFromDevice(path, NativeMethods.EFileAccess.Read | NativeMethods.EFileAccess.Write, NativeMethods.EFileShare.Read | NativeMethods.EFileShare.Write);
		if (intPtr == (IntPtr)(-1))
		{
			throw DeviceException.CreateIOException(base.Device, "Unable to open HID class device (" + path + ").");
		}
		int count = ((Environment.OSVersion.Version >= new Version(5, 1)) ? 512 : 200);
		if (!NativeMethods.HidD_SetNumInputBuffers(intPtr, count))
		{
			NativeMethods.CloseHandle(intPtr);
			throw new IOException("Failed to set input buffers.", new Win32Exception());
		}
		_handle = intPtr;
		HandleInitAndOpen();
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

	public unsafe override void GetFeature(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		HandleAcquireIfOpenOrFail();
		try
		{
			fixed (byte* ptr = buffer)
			{
				if (!NativeMethods.HidD_GetFeature(_handle, ptr + offset, count))
				{
					throw new IOException("GetFeature failed.", new Win32Exception());
				}
			}
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
		try
		{
			lock (_readSync)
			{
				int maxInputReportLength = base.Device.GetMaxInputReportLength();
				if (maxInputReportLength <= 0)
				{
					throw new IOException("Can't read from this device.");
				}
				if (_readBuffer == null || _readBuffer.Length < Math.Max(count, maxInputReportLength))
				{
					Array.Resize(ref _readBuffer, Math.Max(count, maxInputReportLength));
				}
				fixed (byte* readBuffer = _readBuffer)
				{
					NativeOverlapped* ptr = stackalloc NativeOverlapped[1];
					ptr->EventHandle = intPtr;
					NativeMethods.OverlappedOperation(_handle, intPtr, ReadTimeout, _closeEventHandle, NativeMethods.ReadFile(_handle, readBuffer, Math.Max(count, maxInputReportLength), IntPtr.Zero, ptr), ptr, out var bytesTransferred);
					if (count > (int)bytesTransferred)
					{
						count = (int)bytesTransferred;
					}
					Array.Copy(_readBuffer, 0, buffer, offset, count);
					return count;
				}
			}
		}
		finally
		{
			HandleRelease();
			NativeMethods.CloseHandle(intPtr);
		}
	}

	public unsafe override void SetFeature(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		HandleAcquireIfOpenOrFail();
		try
		{
			fixed (byte* ptr = buffer)
			{
				if (!NativeMethods.HidD_SetFeature(_handle, ptr + offset, count))
				{
					throw new IOException("SetFeature failed.", new Win32Exception());
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public unsafe override void Write(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		IntPtr intPtr = NativeMethods.CreateManualResetEventOrThrow();
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (_writeSync)
			{
				int maxOutputReportLength = base.Device.GetMaxOutputReportLength();
				if (maxOutputReportLength <= 0)
				{
					throw new IOException("Can't write to this device.");
				}
				if (_writeBuffer == null || _writeBuffer.Length < Math.Max(count, maxOutputReportLength))
				{
					Array.Resize(ref _writeBuffer, Math.Max(count, maxOutputReportLength));
				}
				Array.Copy(buffer, offset, _writeBuffer, 0, count);
				if (count < maxOutputReportLength)
				{
					Array.Clear(_writeBuffer, count, maxOutputReportLength - count);
					count = maxOutputReportLength;
				}
				fixed (byte* writeBuffer = _writeBuffer)
				{
					int num = 0;
					while (count > 0)
					{
						NativeOverlapped* ptr = stackalloc NativeOverlapped[1];
						ptr->EventHandle = intPtr;
						NativeMethods.OverlappedOperation(_handle, intPtr, WriteTimeout, _closeEventHandle, NativeMethods.WriteFile(_handle, writeBuffer + num, Math.Min(maxOutputReportLength, count), IntPtr.Zero, ptr), ptr, out var bytesTransferred);
						count -= (int)bytesTransferred;
						num += (int)bytesTransferred;
					}
				}
			}
		}
		finally
		{
			HandleRelease();
			NativeMethods.CloseHandle(intPtr);
		}
	}
}
