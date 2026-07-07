using System;
using System.Collections.Generic;
using System.Linq;
using HidSharp.Platform.SystemEvents;

namespace HidSharp.Platform.MacOS;

internal sealed class MacHidManager : HidManager
{
	public override string FriendlyName => "Mac OS HID";

	public override bool IsSupported
	{
		get
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				try
				{
					IntPtr response;
					NativeMethods.OSErr oSErr = NativeMethods.Gestalt(NativeMethods.OSType.gestaltSystemVersionMajor, out response);
					IntPtr response2;
					NativeMethods.OSErr oSErr2 = NativeMethods.Gestalt(NativeMethods.OSType.gestaltSystemVersionMinor, out response2);
					if (oSErr == NativeMethods.OSErr.noErr && oSErr2 == NativeMethods.OSErr.noErr)
					{
						return (long)response >= 10 || ((long)response == 10 && (long)response2 >= 6);
					}
				}
				catch
				{
				}
			}
			return false;
		}
	}

	protected override EventManager CreateEventManager()
	{
		return new MacOSEventManager();
	}

	protected override void Run(Action readyCallback)
	{
		using NativeMethods.CFType cFType = NativeMethods.IOHIDManagerCreate(IntPtr.Zero).ToCFType();
		HidManager.RunAssert(cFType.IsSet, "HidSharp IOHIDManagerCreate failed.");
		using NativeMethods.CFType cFType2 = NativeMethods.IOServiceMatching("IOHIDDevice").ToCFType();
		HidManager.RunAssert(cFType2.IsSet, "HidSharp IOServiceMatching failed.");
		NativeMethods.IOHIDDeviceCallback iOHIDDeviceCallback = DevicesChangedCallback;
		NativeMethods.IOHIDManagerSetDeviceMatching(cFType.Handle, cFType2.Handle);
		NativeMethods.IOHIDManagerRegisterDeviceMatchingCallback(cFType.Handle, iOHIDDeviceCallback, IntPtr.Zero);
		NativeMethods.IOHIDManagerRegisterDeviceRemovalCallback(cFType.Handle, iOHIDDeviceCallback, IntPtr.Zero);
		IntPtr intPtr = NativeMethods.CFRunLoopGetCurrent();
		NativeMethods.CFRetain(intPtr);
		NativeMethods.IOHIDManagerScheduleWithRunLoop(cFType, intPtr, NativeMethods.kCFRunLoopDefaultMode);
		try
		{
			readyCallback();
			NativeMethods.CFRunLoopRun();
		}
		finally
		{
			NativeMethods.IOHIDManagerUnscheduleFromRunLoop(cFType, intPtr, NativeMethods.kCFRunLoopDefaultMode);
			NativeMethods.CFRelease(intPtr);
		}
		GC.KeepAlive(iOHIDDeviceCallback);
	}

	private static void DevicesChangedCallback(IntPtr context, NativeMethods.IOReturn result, IntPtr sender, IntPtr device)
	{
		DeviceList.Local.RaiseChanged();
	}

	private object[] GetDeviceKeys(string kind)
	{
		List<NativeMethods.io_string_t> list = new List<NativeMethods.io_string_t>();
		NativeMethods.CFType cFType = NativeMethods.IOServiceMatching(kind).ToCFType();
		if (cFType.IsSet && NativeMethods.IOServiceGetMatchingServices(0, cFType, out var iterator) == NativeMethods.IOReturn.Success)
		{
			using NativeMethods.IOObject iOObject = iterator.ToIOObject();
			while (true)
			{
				using NativeMethods.IOObject iOObject2 = NativeMethods.IOIteratorNext(iOObject).ToIOObject();
				if (!iOObject2.IsSet)
				{
					break;
				}
				if (NativeMethods.IORegistryEntryGetPath(iOObject2, "IOService", out var path) == NativeMethods.IOReturn.Success)
				{
					list.Add(path);
				}
				continue;
			}
		}
		return list.Cast<object>().ToArray();
	}

	protected override object[] GetBleDeviceKeys()
	{
		return new object[0];
	}

	protected override object[] GetHidDeviceKeys()
	{
		return GetDeviceKeys("IOHIDDevice");
	}

	protected override object[] GetSerialDeviceKeys()
	{
		return GetDeviceKeys("IOSerialBSDClient");
	}

	protected override bool TryCreateBleDevice(object key, out Device device)
	{
		throw new NotImplementedException();
	}

	protected override bool TryCreateHidDevice(object key, out Device device)
	{
		device = MacHidDevice.TryCreate((NativeMethods.io_string_t)key);
		return device != null;
	}

	protected override bool TryCreateSerialDevice(object key, out Device device)
	{
		device = MacSerialDevice.TryCreate((NativeMethods.io_string_t)key);
		return device != null;
	}
}
