using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidSharp.Experimental;
using HidSharp.Platform.SystemEvents;
using HidSharp.Utility;

namespace HidSharp.Platform;

internal abstract class HidManager
{
	private sealed class DeviceTypeInfo
	{
		public delegate object[] GetDeviceKeysCallbackType();

		public delegate bool TryCreateDeviceCallbackType(object key, out Device device);

		public object DevicesLock = new object();

		public Dictionary<object, Device> DeviceList = new Dictionary<object, Device>();

		public GetDeviceKeysCallbackType GetDeviceKeysCallback;

		public TryCreateDeviceCallbackType TryCreateDeviceCallback;
	}

	private DeviceTypeInfo _ble;

	private DeviceTypeInfo _hid;

	private DeviceTypeInfo _serial;

	public virtual bool AreDriversBeingInstalled => false;

	public EventManager EventManager { get; private set; }

	public abstract string FriendlyName { get; }

	public abstract bool IsSupported { get; }

	protected HidManager()
	{
		_ble = new DeviceTypeInfo
		{
			GetDeviceKeysCallback = GetBleDeviceKeys,
			TryCreateDeviceCallback = TryCreateBleDevice
		};
		_hid = new DeviceTypeInfo
		{
			GetDeviceKeysCallback = GetHidDeviceKeys,
			TryCreateDeviceCallback = TryCreateHidDevice
		};
		_serial = new DeviceTypeInfo
		{
			GetDeviceKeysCallback = GetSerialDeviceKeys,
			TryCreateDeviceCallback = TryCreateSerialDevice
		};
	}

	internal void InitializeEventManager()
	{
		EventManager = CreateEventManager();
		EventManager.Start();
	}

	protected virtual EventManager CreateEventManager()
	{
		return new DefaultEventManager();
	}

	protected virtual void Run(Action readyCallback)
	{
		readyCallback();
	}

	internal void RunImpl(object readyEvent)
	{
		Run(delegate
		{
			((ManualResetEvent)readyEvent).Set();
		});
	}

	protected static void RunAssert(bool condition, string error)
	{
		if (!condition)
		{
			throw new InvalidOperationException(error);
		}
	}

	public virtual BleDiscovery BeginBleDiscovery()
	{
		throw new NotSupportedException();
	}

	private IEnumerable<Device> GetDevices(DeviceTypeInfo type)
	{
		Dictionary<object, Device> _deviceList = type.DeviceList;
		object devicesLock = type.DevicesLock;
		DeviceTypeInfo.GetDeviceKeysCallbackType getDeviceKeysCallback = type.GetDeviceKeysCallback;
		DeviceTypeInfo.TryCreateDeviceCallbackType tryCreateDeviceCallback = type.TryCreateDeviceCallback;
		lock (devicesLock)
		{
			object[] array = getDeviceKeysCallback();
			object[] array2 = array.Except(_deviceList.Keys).ToArray();
			object[] array3 = _deviceList.Keys.Except(array).ToArray();
			if (array2.Length > 0)
			{
				int completedAdditions = 0;
				object[] array4 = array2;
				foreach (object state in array4)
				{
					ThreadPool.QueueUserWorkItem(delegate(object key)
					{
						if (tryCreateDeviceCallback(key, out var device))
						{
							lock (_deviceList)
							{
								_deviceList.Add(key, device);
								HidSharpDiagnostics.Trace("Detected a new device: {0}", key);
							}
						}
						lock (_deviceList)
						{
							completedAdditions++;
							Monitor.Pulse(_deviceList);
						}
					}, state);
				}
				lock (_deviceList)
				{
					while (completedAdditions != array2.Length)
					{
						Monitor.Wait(_deviceList);
					}
				}
			}
			object[] array5 = array3;
			foreach (object obj in array5)
			{
				_deviceList.Remove(obj);
				HidSharpDiagnostics.Trace("Detected a device removal: {0}", obj);
			}
			return _deviceList.Values.ToArray();
		}
	}

	public IEnumerable<Device> GetDevices(DeviceTypes types)
	{
		IEnumerable<Device> enumerable = Enumerable.Empty<Device>();
		if ((types & DeviceTypes.Hid) != 0)
		{
			enumerable = enumerable.Concat(GetDevices(_hid));
		}
		if ((types & DeviceTypes.Serial) != 0)
		{
			enumerable = enumerable.Concat(GetDevices(_serial));
		}
		if ((types & DeviceTypes.Ble) != 0)
		{
			enumerable = enumerable.Concat(GetDevices(_ble));
		}
		return enumerable;
	}

	protected abstract object[] GetBleDeviceKeys();

	protected abstract object[] GetHidDeviceKeys();

	protected abstract object[] GetSerialDeviceKeys();

	protected abstract bool TryCreateBleDevice(object key, out Device device);

	protected abstract bool TryCreateHidDevice(object key, out Device device);

	protected abstract bool TryCreateSerialDevice(object key, out Device device);
}
