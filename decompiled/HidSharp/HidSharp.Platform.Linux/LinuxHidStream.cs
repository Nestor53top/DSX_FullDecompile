using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidSharp.Platform.Linux;

internal sealed class LinuxHidStream : SysHidStream
{
	private Queue<byte[]> _inputQueue;

	private Queue<CommonOutputReport> _outputQueue;

	private int _handle;

	private Thread _readThread;

	private Thread _writeThread;

	private volatile bool _shutdown;

	internal LinuxHidStream(LinuxHidDevice device)
		: base(device)
	{
		_inputQueue = new Queue<byte[]>();
		_outputQueue = new Queue<CommonOutputReport>();
		_handle = -1;
		_readThread = new Thread(ReadThread)
		{
			IsBackground = true,
			Name = "HID Reader"
		};
		_writeThread = new Thread(WriteThread)
		{
			IsBackground = true,
			Name = "HID Writer"
		};
	}

	internal static int DeviceHandleFromPath(string path, HidDevice hidDevice, NativeMethods.oflag oflag)
	{
		IntPtr intPtr = NativeMethodsLibudev.Instance.udev_new();
		if (IntPtr.Zero != intPtr)
		{
			try
			{
				IntPtr intPtr2 = NativeMethodsLibudev.Instance.udev_device_new_from_syspath(intPtr, path);
				if (IntPtr.Zero != intPtr2)
				{
					try
					{
						string devnode = NativeMethodsLibudev.Instance.udev_device_get_devnode(intPtr2);
						if (devnode != null)
						{
							int num = NativeMethods.retry(() => NativeMethods.open(devnode, oflag));
							if (num < 0)
							{
								NativeMethods.error lastWin32Error = (NativeMethods.error)Marshal.GetLastWin32Error();
								if (lastWin32Error == NativeMethods.error.EACCES)
								{
									throw DeviceException.CreateUnauthorizedAccessException(hidDevice, "Not permitted to open HID class device at " + devnode + ".");
								}
								throw DeviceException.CreateIOException(hidDevice, "Unable to open HID class device (" + lastWin32Error.ToString() + ").");
							}
							return num;
						}
					}
					finally
					{
						NativeMethodsLibudev.Instance.udev_device_unref(intPtr2);
					}
				}
			}
			finally
			{
				NativeMethodsLibudev.Instance.udev_unref(intPtr);
			}
		}
		throw new FileNotFoundException("HID class device not found.");
	}

	internal void Init(string path)
	{
		int handle = DeviceHandleFromPath(path, base.Device, NativeMethods.oflag.RDWR | NativeMethods.oflag.NONBLOCK);
		_handle = handle;
		HandleInitAndOpen();
		_readThread.Start();
		_writeThread.Start();
	}

	protected override void Dispose(bool disposing)
	{
		if (!HandleClose())
		{
			return;
		}
		_shutdown = true;
		try
		{
			lock (_inputQueue)
			{
				Monitor.PulseAll(_inputQueue);
			}
		}
		catch
		{
		}
		try
		{
			lock (_outputQueue)
			{
				Monitor.PulseAll(_outputQueue);
			}
		}
		catch
		{
		}
		try
		{
			_readThread.Join();
		}
		catch
		{
		}
		try
		{
			_writeThread.Join();
		}
		catch
		{
		}
		HandleRelease();
		base.Dispose(disposing);
	}

	internal override void HandleFree()
	{
		NativeMethods.retry(() => NativeMethods.close(_handle));
		_handle = -1;
	}

	private unsafe void ReadThread()
	{
		if (!HandleAcquire())
		{
			return;
		}
		try
		{
			lock (_inputQueue)
			{
				NativeMethods.pollfd fds = new NativeMethods.pollfd
				{
					fd = _handle,
					events = NativeMethods.pollev.IN
				};
				while (!_shutdown)
				{
					while (true)
					{
						Monitor.Exit(_inputQueue);
						int num;
						try
						{
							num = NativeMethods.poll(ref fds, (IntPtr)1, 250);
						}
						finally
						{
							Monitor.Enter(_inputQueue);
						}
						if (num != 1)
						{
							break;
						}
						if ((fds.revents & (NativeMethods.pollev.ERR | NativeMethods.pollev.HUP | NativeMethods.pollev.NVAL)) != 0)
						{
							goto end_IL_01a4;
						}
						if ((fds.revents & NativeMethods.pollev.IN) == 0)
						{
							break;
						}
						int num2 = base.Device.GetMaxInputReportLength();
						if (num2 > 0 && !((LinuxHidDevice)base.Device).ReportsUseID)
						{
							num2--;
						}
						byte[] inputReport = new byte[num2];
						fixed (byte* ptr = inputReport)
						{
							IntPtr inputBytesPtr = (IntPtr)ptr;
							IntPtr intPtr = NativeMethods.retry(() => NativeMethods.read(_handle, inputBytesPtr, (UIntPtr)(ulong)inputReport.Length));
							if ((long)intPtr < 0)
							{
								NativeMethods.error lastWin32Error = (NativeMethods.error)Marshal.GetLastWin32Error();
								if (lastWin32Error != NativeMethods.error.EAGAIN)
								{
									goto end_IL_01a4;
								}
								continue;
							}
							Array.Resize(ref inputReport, (int)intPtr);
							if (!((LinuxHidDevice)base.Device).ReportsUseID)
							{
								inputReport = new byte[1].Concat(inputReport).ToArray();
							}
							_inputQueue.Enqueue(inputReport);
							Monitor.PulseAll(_inputQueue);
						}
						break;
					}
					continue;
					end_IL_01a4:
					break;
				}
				CommonDisconnected(_inputQueue);
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return CommonRead(buffer, offset, count, _inputQueue);
	}

	public unsafe override void GetFeature(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count).False(count >= 2);
		HandleAcquireIfOpenOrFail();
		try
		{
			byte b = buffer[offset];
			fixed (byte* ptr = buffer)
			{
				buffer[offset + 1] = b;
				int num = NativeMethods.ioctl(_handle, NativeMethods.HIDIOCGFEATURE(count - 1), (IntPtr)(ptr + offset + 1));
				if (num < 0)
				{
					throw new IOException("GetFeature failed.");
				}
				Array.Clear(buffer, 1 + num, count - (1 + num));
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	private unsafe void WriteThread()
	{
		if (!HandleAcquire())
		{
			return;
		}
		try
		{
			lock (_outputQueue)
			{
				while (true)
				{
					while (!_shutdown && _outputQueue.Count == 0)
					{
						Monitor.Wait(_outputQueue);
					}
					if (_shutdown)
					{
						break;
					}
					CommonOutputReport commonOutputReport = _outputQueue.Peek();
					byte[] outputBytesRaw = commonOutputReport.Bytes;
					try
					{
						fixed (byte* ptr = outputBytesRaw)
						{
							Monitor.Exit(_outputQueue);
							try
							{
								IntPtr outputBytesPtr = (IntPtr)ptr;
								IntPtr intPtr = NativeMethods.retry(() => NativeMethods.write(_handle, outputBytesPtr, (UIntPtr)(ulong)outputBytesRaw.Length));
								if ((long)intPtr == outputBytesRaw.Length)
								{
									commonOutputReport.DoneOK = true;
								}
							}
							finally
							{
								Monitor.Enter(_outputQueue);
							}
						}
					}
					finally
					{
						_outputQueue.Dequeue();
						commonOutputReport.Done = true;
						Monitor.PulseAll(_outputQueue);
					}
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		CommonWrite(buffer, offset, count, _outputQueue, feature: false, base.Device.GetMaxOutputReportLength());
	}

	public unsafe override void SetFeature(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		HandleAcquireIfOpenOrFail();
		try
		{
			fixed (byte* ptr = buffer)
			{
				if (NativeMethods.ioctl(_handle, NativeMethods.HIDIOCSFEATURE(count), (IntPtr)(ptr + offset)) < 0)
				{
					throw new IOException("SetFeature failed.");
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}
}
