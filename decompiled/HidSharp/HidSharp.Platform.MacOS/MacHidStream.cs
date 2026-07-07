using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using HidSharp.Utility;

namespace HidSharp.Platform.MacOS;

internal sealed class MacHidStream : SysHidStream
{
	private Queue<byte[]> _inputQueue;

	private Queue<CommonOutputReport> _outputQueue;

	private IntPtr _handle;

	private IntPtr _readRunLoop;

	private Thread _readThread;

	private Thread _writeThread;

	private volatile bool _shutdown;

	internal MacHidStream(MacHidDevice device)
		: base(device)
	{
		_inputQueue = new Queue<byte[]>();
		_outputQueue = new Queue<CommonOutputReport>();
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

	internal void Init(NativeMethods.io_string_t path)
	{
		int num = 0;
		int num2 = 10;
		IntPtr intPtr;
		while (true)
		{
			NativeMethods.io_string_t path2 = path.Clone();
			using NativeMethods.IOObject iOObject = NativeMethods.IORegistryEntryFromPath(0, ref path2).ToIOObject();
			string text;
			if (iOObject.IsSet)
			{
				intPtr = NativeMethods.IOHIDDeviceCreate(IntPtr.Zero, iOObject);
				if (intPtr != IntPtr.Zero)
				{
					NativeMethods.IOReturn iOReturn = NativeMethods.IOHIDDeviceOpen(intPtr);
					if (iOReturn == NativeMethods.IOReturn.Success)
					{
						break;
					}
					NativeMethods.CFRelease(intPtr);
					text = string.Format("Unable to open HID class device (error {1}): {0}", path2.ToString(), iOReturn);
					goto IL_00b2;
				}
				text = $"HID class device not found: {path2.ToString()}";
				goto IL_00b2;
			}
			text = $"HID class device path not found: {path2.ToString()}";
			goto IL_00b2;
			IL_00b2:
			if (++num == num2)
			{
				throw DeviceException.CreateIOException(base.Device, text);
			}
			HidSharpDiagnostics.Trace("Retrying ({0})", text);
			Thread.Sleep(100);
			continue;
		}
		_handle = intPtr;
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
		while (true)
		{
			IntPtr readRunLoop = _readRunLoop;
			NativeMethods.CFRunLoopStop(readRunLoop);
			try
			{
				if (_readThread.Join(25))
				{
					break;
				}
			}
			catch
			{
				break;
			}
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
		NativeMethods.CFRelease(_handle);
		_handle = IntPtr.Zero;
	}

	private void ReadThreadCallback(IntPtr context, NativeMethods.IOReturn result, IntPtr sender, NativeMethods.IOHIDReportType type, uint reportID, IntPtr report, IntPtr reportLength)
	{
		if (result != NativeMethods.IOReturn.Success || !(reportLength != IntPtr.Zero) || type != NativeMethods.IOHIDReportType.Input)
		{
			return;
		}
		int num = ((!((MacHidDevice)base.Device).ReportsUseID) ? 1 : 0);
		byte[] array = new byte[checked((int)reportLength + num)];
		Marshal.Copy(report, array, num, (int)reportLength);
		Queue<byte[]> inputQueue = _inputQueue;
		lock (inputQueue)
		{
			if (inputQueue.Count < 512)
			{
				inputQueue.Enqueue(array);
				Monitor.PulseAll(inputQueue);
			}
		}
	}

	private void RemovalCallback(IntPtr context, NativeMethods.IOReturn result, IntPtr sender)
	{
		CommonDisconnected(_inputQueue);
	}

	private unsafe void ReadThread()
	{
		if (!HandleAcquire())
		{
			return;
		}
		_readRunLoop = NativeMethods.CFRunLoopGetCurrent();
		try
		{
			NativeMethods.IOHIDReportCallback iOHIDReportCallback = ReadThreadCallback;
			NativeMethods.IOHIDCallback iOHIDCallback = RemovalCallback;
			byte[] array = new byte[base.Device.GetMaxInputReportLength()];
			fixed (byte* ptr = array)
			{
				NativeMethods.IOHIDDeviceRegisterInputReportCallback(_handle, (IntPtr)ptr, (IntPtr)array.Length, iOHIDReportCallback, IntPtr.Zero);
				NativeMethods.IOHIDDeviceRegisterRemovalCallback(_handle, iOHIDCallback, IntPtr.Zero);
				NativeMethods.IOHIDDeviceScheduleWithRunLoop(_handle, _readRunLoop, NativeMethods.kCFRunLoopDefaultMode);
				NativeMethods.CFRunLoopRun();
				NativeMethods.IOHIDDeviceUnscheduleFromRunLoop(_handle, _readRunLoop, NativeMethods.kCFRunLoopDefaultMode);
			}
			GC.KeepAlive(this);
			GC.KeepAlive(iOHIDReportCallback);
			GC.KeepAlive(iOHIDCallback);
			GC.KeepAlive(_inputQueue);
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
		Throw.If.OutOfRange(buffer, offset, count);
		HandleAcquireIfOpenOrFail();
		try
		{
			fixed (byte* ptr = buffer)
			{
				int num = buffer[offset];
				int num2 = ((!((MacHidDevice)base.Device).ReportsUseID) ? 1 : 0);
				IntPtr report = (IntPtr)(ptr + offset + num2);
				count -= num2;
				if (count <= 0)
				{
					throw new ArgumentException();
				}
				IntPtr reportLength = (IntPtr)count;
				if (NativeMethods.IOHIDDeviceGetReport(_handle, NativeMethods.IOHIDReportType.Feature, (IntPtr)num, report, ref reportLength) != NativeMethods.IOReturn.Success)
				{
					throw new IOException("GetFeature failed.");
				}
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
					if (!_shutdown && _outputQueue.Count == 0)
					{
						Monitor.Wait(_outputQueue);
						continue;
					}
					if (_shutdown)
					{
						break;
					}
					CommonOutputReport commonOutputReport = _outputQueue.Peek();
					try
					{
						fixed (byte* bytes = commonOutputReport.Bytes)
						{
							Monitor.Exit(_outputQueue);
							try
							{
								int num = commonOutputReport.Bytes[0];
								int num2 = ((!((MacHidDevice)base.Device).ReportsUseID) ? 1 : 0);
								IntPtr report = (IntPtr)(bytes + num2);
								int num3 = commonOutputReport.Bytes.Length - num2;
								if (num3 > 0 && NativeMethods.IOHIDDeviceSetReport(_handle, (!commonOutputReport.Feature) ? NativeMethods.IOHIDReportType.Output : NativeMethods.IOHIDReportType.Feature, (IntPtr)num, report, (IntPtr)num3) == NativeMethods.IOReturn.Success)
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

	public override void SetFeature(byte[] buffer, int offset, int count)
	{
		CommonWrite(buffer, offset, count, _outputQueue, feature: true, base.Device.GetMaxFeatureReportLength());
	}
}
