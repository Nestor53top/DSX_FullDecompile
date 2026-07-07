using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using HidSharp.Utility;

namespace HidSharp.Platform.Windows;

internal sealed class WinHidManager : HidManager
{
	private class DevicePathBase
	{
		public string DevicePath;

		public string DeviceID;

		public override bool Equals(object obj)
		{
			if (obj is DevicePathBase devicePathBase && DevicePath == devicePathBase.DevicePath)
			{
				return DeviceID == devicePathBase.DeviceID;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return DevicePath.GetHashCode();
		}

		public override string ToString()
		{
			return DevicePath;
		}
	}

	private sealed class BleDevicePath : DevicePathBase
	{
		public string FriendlyName;

		public override bool Equals(object obj)
		{
			if (obj is BleDevicePath bleDevicePath)
			{
				return FriendlyName == bleDevicePath.FriendlyName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	private sealed class HidDevicePath : DevicePathBase
	{
	}

	private sealed class SerialDevicePath : DevicePathBase
	{
		public string FileSystemName;

		public string FriendlyName;

		public override bool Equals(object obj)
		{
			if (obj is SerialDevicePath serialDevicePath && FileSystemName == serialDevicePath.FileSystemName)
			{
				return FriendlyName == serialDevicePath.FriendlyName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	private bool _isSupported;

	private bool _bleIsSupported;

	private static Thread _serialWatcherThread;

	private static IntPtr _serialWatcherShutdownEvent;

	private static Thread _notifyThread;

	private static volatile bool _notifyThreadShouldNotify;

	private static volatile bool _notifyThreadShuttingDown;

	private static object _hidNotifyObject;

	private static object _serNotifyObject;

	private static object _bleNotifyObject;

	private static object[] _hidDeviceKeysCache;

	private static object[] _serDeviceKeysCache;

	private static object[] _bleDeviceKeysCache;

	private static object _hidDeviceKeysCacheNotifyObject;

	private static object _serDeviceKeysCacheNotifyObject;

	private static object _bleDeviceKeysCacheNotifyObject;

	public override bool AreDriversBeingInstalled
	{
		get
		{
			try
			{
				return 258 == NativeMethods.CMP_WaitNoPendingInstallEvents(0u);
			}
			catch
			{
				return false;
			}
		}
	}

	public override string FriendlyName => "Windows HID";

	public override bool IsSupported => _isSupported;

	public WinHidManager()
	{
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			return;
		}
		NativeMethods.OSVERSIONINFO version = new NativeMethods.OSVERSIONINFO
		{
			OSVersionInfoSize = Marshal.SizeOf(typeof(NativeMethods.OSVERSIONINFO))
		};
		try
		{
			if (NativeMethods.GetVersionEx(ref version) && version.PlatformID == 2)
			{
				_isSupported = true;
				if (Environment.OSVersion.Version >= new Version(6, 2))
				{
					_bleIsSupported = true;
				}
			}
		}
		catch
		{
		}
	}

	protected override void Run(Action readyCallback)
	{
		NativeMethods.WindowProc windowProc = DeviceMonitorWindowProc;
		NativeMethods.WNDCLASS windowClass = new NativeMethods.WNDCLASS
		{
			ClassName = "HidSharpDeviceMonitor",
			WindowProc = windowProc
		};
		HidManager.RunAssert(0 != NativeMethods.RegisterClass(ref windowClass), "HidSharp RegisterClass failed.");
		IntPtr intPtr = NativeMethods.CreateWindowEx(0u, "HidSharpDeviceMonitor", "HidSharpDeviceMonitor", 0u, int.MinValue, int.MinValue, int.MinValue, int.MinValue, NativeMethods.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		HidManager.RunAssert(intPtr != IntPtr.Zero, "HidSharp CreateWindow failed.");
		IntPtr handle = RegisterDeviceNotification(intPtr, NativeMethods.HidD_GetHidGuid());
		IntPtr handle2 = RegisterDeviceNotification(intPtr, NativeMethods.GuidForBluetoothLEDevice);
		_serialWatcherShutdownEvent = NativeMethods.CreateManualResetEventOrThrow();
		Thread thread = new Thread(SerialWatcherThread);
		thread.IsBackground = true;
		thread.Name = "HidSharp Serial Watcher";
		_serialWatcherThread = thread;
		_serialWatcherThread.Start();
		_hidNotifyObject = new object();
		_serNotifyObject = new object();
		_bleNotifyObject = new object();
		Thread thread2 = new Thread(DeviceMonitorEventThread);
		thread2.IsBackground = true;
		thread2.Name = "HidSharp RaiseChanged";
		_notifyThread = thread2;
		_notifyThread.Start();
		readyCallback();
		while (true)
		{
			NativeMethods.MSG message2;
			int message = NativeMethods.GetMessage(out message2, intPtr, 0u, 0u);
			if (message == 0 || message == -1)
			{
				break;
			}
			NativeMethods.TranslateMessage(ref message2);
			NativeMethods.DispatchMessage(ref message2);
		}
		lock (_notifyThread)
		{
			_notifyThreadShuttingDown = true;
			Monitor.Pulse(_notifyThread);
		}
		NativeMethods.SetEvent(_serialWatcherShutdownEvent);
		_notifyThread.Join();
		_serialWatcherThread.Join();
		UnregisterDeviceNotification(handle);
		UnregisterDeviceNotification(handle2);
		HidManager.RunAssert(NativeMethods.DestroyWindow(intPtr), "HidSharp DestroyWindow failed.");
		HidManager.RunAssert(NativeMethods.UnregisterClass("HidSharpDeviceMonitor", IntPtr.Zero), "HidSharp UnregisterClass failed.");
		GC.KeepAlive(windowProc);
	}

	private static IntPtr RegisterDeviceNotification(IntPtr hwnd, Guid guid)
	{
		NativeMethods.DEV_BROADCAST_DEVICEINTERFACE notificationFilter = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE
		{
			Size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)),
			ClassGuid = guid,
			DeviceType = 5
		};
		IntPtr intPtr = NativeMethods.RegisterDeviceNotification(hwnd, ref notificationFilter, 0);
		HidManager.RunAssert(intPtr != IntPtr.Zero, "HidSharp RegisterDeviceNotification failed.");
		return intPtr;
	}

	private static IntPtr RegisterDeviceNotification(IntPtr hwnd, IntPtr handle)
	{
		NativeMethods.DEV_BROADCAST_HANDLE notificationFilter = new NativeMethods.DEV_BROADCAST_HANDLE
		{
			Size = Marshal.SizeOf(typeof(NativeMethods.DEV_BROADCAST_HANDLE)),
			DeviceHandle = handle,
			DeviceType = 6
		};
		IntPtr intPtr = NativeMethods.RegisterDeviceNotification(hwnd, ref notificationFilter, 0);
		HidManager.RunAssert(intPtr != IntPtr.Zero, "HidSharp RegisterDeviceNotification failed.");
		return intPtr;
	}

	private static void UnregisterDeviceNotification(IntPtr handle)
	{
		HidManager.RunAssert(NativeMethods.UnregisterDeviceNotification(handle), "HidSharp UnregisterDeviceNotification failed.");
	}

	private unsafe static IntPtr DeviceMonitorWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		if (message == 537)
		{
			NativeMethods.WM_DEVICECHANGE_wParam wM_DEVICECHANGE_wParam = (NativeMethods.WM_DEVICECHANGE_wParam)(long)wParam;
			HidSharpDiagnostics.Trace("Received a device change event, {0}.", wM_DEVICECHANGE_wParam);
			NativeMethods.DEV_BROADCAST_HDR* ptr = (NativeMethods.DEV_BROADCAST_HDR*)(void*)lParam;
			switch (wM_DEVICECHANGE_wParam)
			{
			case NativeMethods.WM_DEVICECHANGE_wParam.DBT_DEVICEARRIVAL:
			case NativeMethods.WM_DEVICECHANGE_wParam.DBT_DEVICEREMOVECOMPLETE:
				if (ptr->DeviceType == 5)
				{
					NativeMethods.DEV_BROADCAST_DEVICEINTERFACE* ptr6 = (NativeMethods.DEV_BROADCAST_DEVICEINTERFACE*)ptr;
					if (ptr6->ClassGuid == NativeMethods.HidD_GetHidGuid())
					{
						DeviceListDidChange(ref _hidNotifyObject);
					}
					else if (ptr6->ClassGuid == NativeMethods.GuidForBluetoothLEDevice)
					{
						DeviceListDidChange(ref _bleNotifyObject);
					}
				}
				break;
			case NativeMethods.WM_DEVICECHANGE_wParam.DBT_CUSTOMEVENT:
				if (ptr->DeviceType == 6)
				{
					NativeMethods.DEV_BROADCAST_HANDLE* ptr2 = (NativeMethods.DEV_BROADCAST_HANDLE*)ptr;
					if (ptr2->EventGuid == NativeMethods.GuidForBluetoothHciEvent)
					{
						NativeMethods.BTH_HCI_EVENT_INFO* ptr3 = (NativeMethods.BTH_HCI_EVENT_INFO*)ptr2->Data;
						HidSharpDiagnostics.Trace("Bluetooth HCI event: address {0:X}, type {1}, connected {2}", ptr3->bthAddress, ptr3->connectionType, ptr3->connected);
					}
					else if (ptr2->EventGuid == NativeMethods.GuidForBluetoothRadioInRange)
					{
						NativeMethods.BTH_RADIO_IN_RANGE* ptr4 = (NativeMethods.BTH_RADIO_IN_RANGE*)ptr2->Data;
						HidSharpDiagnostics.Trace("Radio in range event: address {0:X}, flags {1}, class {2}, name '{3}{4}'", ptr4->deviceInfo.address, ptr4->deviceInfo.flags, ptr4->deviceInfo.classOfDevice, (char)(*ptr4->deviceInfo.name), (char)ptr4->deviceInfo.name[1]);
					}
					else if (ptr2->EventGuid == NativeMethods.GuidForBluetoothRadioOutOfRange)
					{
						NativeMethods.BLUETOOTH_ADDRESS* ptr5 = (NativeMethods.BLUETOOTH_ADDRESS*)ptr2->Data;
						HidSharpDiagnostics.Trace("Radio out of range event: address {0:X}", ptr5->Addr);
					}
					else
					{
						HidSharpDiagnostics.Trace("Custom event: GUID {0}", ptr2->EventGuid);
					}
				}
				break;
			}
			return (IntPtr)1;
		}
		return NativeMethods.DefWindowProc(window, message, wParam, lParam);
	}

	private static void DeviceListDidChange(ref object notifyObject)
	{
		lock (_notifyThread)
		{
			notifyObject = new object();
			_notifyThreadShouldNotify = true;
			Monitor.Pulse(_notifyThread);
		}
	}

	private unsafe static void SerialWatcherThread()
	{
		IntPtr intPtr = NativeMethods.CreateAutoResetEventOrThrow();
		try
		{
			if (NativeMethods.RegOpenKeyEx(new IntPtr(-2147483646), "HARDWARE\\DEVICEMAP\\SERIALCOMM", 0u, 16u, out var handle) != 0)
			{
				return;
			}
			try
			{
				IntPtr* ptr = stackalloc IntPtr[2];
				*ptr = _serialWatcherShutdownEvent;
				ptr[1] = intPtr;
				while (NativeMethods.RegNotifyChangeKeyValue(handle, watchSubtree: false, 4u, intPtr, asynchronous: true) == 0)
				{
					switch (NativeMethods.WaitForMultipleObjects(2u, ptr, waitAll: false, uint.MaxValue))
					{
					default:
						return;
					case 1u:
						break;
					}
					HidSharpDiagnostics.Trace("Received a serial change event.");
					DeviceListDidChange(ref _serNotifyObject);
				}
			}
			finally
			{
				NativeMethods.RegCloseKey(handle);
			}
		}
		finally
		{
			NativeMethods.CloseHandle(intPtr);
		}
	}

	private static void DeviceMonitorEventThread()
	{
		lock (_notifyThread)
		{
			while (!_notifyThreadShuttingDown)
			{
				if (_notifyThreadShouldNotify)
				{
					_notifyThreadShouldNotify = false;
					Monitor.Exit(_notifyThread);
					try
					{
						DeviceList.Local.RaiseChanged();
					}
					finally
					{
						Monitor.Enter(_notifyThread);
					}
				}
				else
				{
					Monitor.Wait(_notifyThread);
				}
			}
		}
	}

	protected override object[] GetBleDeviceKeys()
	{
		object bleNotifyObject;
		lock (_notifyThread)
		{
			bleNotifyObject = _bleNotifyObject;
			if (bleNotifyObject == _bleDeviceKeysCacheNotifyObject)
			{
				return _bleDeviceKeysCache;
			}
		}
		List<object> paths = new List<object>();
		if (_bleIsSupported)
		{
			NativeMethods.EnumerateDeviceInterfaces(NativeMethods.GuidForBluetoothLEDevice, delegate(NativeMethods.HDEVINFO deviceInfoSet, NativeMethods.SP_DEVINFO_DATA deviceInfoData, NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData, string deviceID, string devicePath)
			{
				if (!NativeMethods.TryGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, 12u, out var value))
				{
					value = null;
				}
				if (!string.IsNullOrEmpty(value))
				{
					paths.Add(new BleDevicePath
					{
						DeviceID = deviceID,
						DevicePath = devicePath,
						FriendlyName = value
					});
				}
			});
		}
		object[] array = paths.ToArray();
		lock (_notifyThread)
		{
			_bleDeviceKeysCacheNotifyObject = bleNotifyObject;
			_bleDeviceKeysCache = array;
			return array;
		}
	}

	protected override object[] GetHidDeviceKeys()
	{
		object hidNotifyObject;
		lock (_notifyThread)
		{
			hidNotifyObject = _hidNotifyObject;
			if (hidNotifyObject == _hidDeviceKeysCacheNotifyObject)
			{
				return _hidDeviceKeysCache;
			}
		}
		List<object> paths = new List<object>();
		Guid guid = NativeMethods.HidD_GetHidGuid();
		NativeMethods.EnumerateDeviceInterfaces(guid, delegate(NativeMethods.HDEVINFO _, NativeMethods.SP_DEVINFO_DATA __, NativeMethods.SP_DEVICE_INTERFACE_DATA ___, string deviceID, string devicePath)
		{
			paths.Add(new HidDevicePath
			{
				DeviceID = deviceID,
				DevicePath = devicePath
			});
		});
		object[] array = paths.ToArray();
		lock (_notifyThread)
		{
			_hidDeviceKeysCacheNotifyObject = hidNotifyObject;
			_hidDeviceKeysCache = array;
			return array;
		}
	}

	protected override object[] GetSerialDeviceKeys()
	{
		object serNotifyObject;
		lock (_notifyThread)
		{
			serNotifyObject = _serNotifyObject;
			if (serNotifyObject == _serDeviceKeysCacheNotifyObject)
			{
				return _serDeviceKeysCache;
			}
		}
		List<object> paths = new List<object>();
		NativeMethods.EnumerateDevices(NativeMethods.GuidForPortsClass, delegate(NativeMethods.HDEVINFO deviceInfoSet, NativeMethods.SP_DEVINFO_DATA deviceInfoData, string deviceID)
		{
			if (NativeMethods.TryGetSerialPortFriendlyName(deviceInfoSet, ref deviceInfoData, out var friendlyName) && NativeMethods.TryGetSerialPortName(deviceInfoSet, ref deviceInfoData, out var portName))
			{
				paths.Add(new SerialDevicePath
				{
					DeviceID = deviceID,
					DevicePath = "\\\\.\\" + portName,
					FileSystemName = portName,
					FriendlyName = friendlyName
				});
			}
		});
		object[] array = paths.ToArray();
		lock (_notifyThread)
		{
			_serDeviceKeysCacheNotifyObject = serNotifyObject;
			_serDeviceKeysCache = array;
			return array;
		}
	}

	protected override bool TryCreateBleDevice(object key, out Device device)
	{
		BleDevicePath bleDevicePath = (BleDevicePath)key;
		device = WinBleDevice.TryCreate(bleDevicePath.DevicePath, bleDevicePath.DeviceID, bleDevicePath.FriendlyName);
		return device != null;
	}

	protected override bool TryCreateHidDevice(object key, out Device device)
	{
		HidDevicePath hidDevicePath = (HidDevicePath)key;
		device = WinHidDevice.TryCreate(hidDevicePath.DevicePath, hidDevicePath.DeviceID);
		return device != null;
	}

	protected override bool TryCreateSerialDevice(object key, out Device device)
	{
		SerialDevicePath serialDevicePath = (SerialDevicePath)key;
		device = WinSerialDevice.TryCreate(serialDevicePath.DevicePath, serialDevicePath.FileSystemName, serialDevicePath.FriendlyName);
		return true;
	}
}
