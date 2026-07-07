using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HidSharp.Experimental;

namespace HidSharp.Platform.Windows;

internal sealed class WinBleDevice : BleDevice
{
	private string _path;

	private string _id;

	private string _friendlyName;

	private WinBleService[] _services;

	private object _syncObject;

	public override string DevicePath => _path;

	internal static WinBleDevice TryCreate(string path, string id, string friendlyName)
	{
		WinBleDevice winBleDevice = new WinBleDevice();
		winBleDevice._path = path;
		winBleDevice._id = id;
		winBleDevice._friendlyName = friendlyName;
		winBleDevice._syncObject = new object();
		return winBleDevice;
	}

	internal bool TryOpenToGetInfo(Func<IntPtr, bool> action)
	{
		return NativeMethods.TryOpenToGetInfo(_path, action);
	}

	private WinBleService GetService(OpenConfiguration openConfig)
	{
		return (WinBleService)openConfig.GetOption(OpenOption.BleService);
	}

	protected override string GetStreamPath(OpenConfiguration openConfig)
	{
		WinBleService service = GetService(openConfig);
		if (service == null)
		{
			throw DeviceException.CreateIOException(this, "BLE service not specified.");
		}
		if (service.Device != this)
		{
			throw DeviceException.CreateIOException(this, "BLE service is on a different device.");
		}
		if (NativeMethods.CM_Locate_DevNode(out var devInst, _id) == 0 && NativeMethods.CM_Get_Child(out devInst, devInst) == 0)
		{
			do
			{
				if (NativeMethods.CM_Get_Device_ID(devInst, out var deviceID) != 0)
				{
					continue;
				}
				NativeMethods.HDEVINFO deviceInfoSet = NativeMethods.SetupDiGetClassDevs(service.Uuid, deviceID, IntPtr.Zero, NativeMethods.DIGCF.Present | NativeMethods.DIGCF.DeviceInterface);
				if (!deviceInfoSet.IsValid)
				{
					continue;
				}
				try
				{
					NativeMethods.SP_DEVINFO_DATA deviceInfoData = default(NativeMethods.SP_DEVINFO_DATA);
					deviceInfoData.Size = Marshal.SizeOf((object)deviceInfoData);
					for (int i = 0; NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
					{
						NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = default(NativeMethods.SP_DEVICE_INTERFACE_DATA);
						deviceInterfaceData.Size = Marshal.SizeOf((object)deviceInterfaceData);
						for (int j = 0; NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, service.Uuid, j, ref deviceInterfaceData); j++)
						{
							if (NativeMethods.SetupDiGetDeviceInterfaceDevicePath(deviceInfoSet, ref deviceInterfaceData, out var devicePath))
							{
								return devicePath;
							}
						}
					}
				}
				finally
				{
					NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
				}
			}
			while (NativeMethods.CM_Get_Sibling(out devInst, devInst) == 0);
		}
		throw DeviceException.CreateIOException(this, $"BLE service {service.Uuid} not found.");
	}

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		string streamPath = GetStreamPath(openConfig);
		WinBleStream winBleStream = new WinBleStream(this, GetService(openConfig));
		try
		{
			winBleStream.Init(streamPath);
			return winBleStream;
		}
		catch
		{
			winBleStream.Close();
			throw;
		}
	}

	private void RequiresServices()
	{
		lock (_syncObject)
		{
			if (!TryOpenToGetInfo(delegate(IntPtr handle)
			{
				NativeMethods.BTH_LE_GATT_SERVICE[] array = NativeMethods.BluetoothGATTGetServices(handle);
				if (array == null)
				{
					return false;
				}
				List<WinBleService> list = new List<WinBleService>();
				NativeMethods.BTH_LE_GATT_SERVICE[] array2 = array;
				foreach (NativeMethods.BTH_LE_GATT_SERVICE nativeData in array2)
				{
					WinBleService item = new WinBleService(this, nativeData);
					list.Add(item);
				}
				_services = list.ToArray();
				return true;
			}))
			{
				throw DeviceException.CreateIOException(this, "BLE service list could not be retrieved.");
			}
		}
	}

	public override BleService[] GetServices()
	{
		RequiresServices();
		return (BleService[])_services.Clone();
	}

	public override bool HasService(BleUuid service)
	{
		RequiresServices();
		WinBleService[] services = _services;
		for (int i = 0; i < services.Length; i++)
		{
			if (services[i].Uuid == service)
			{
				return true;
			}
		}
		return false;
	}

	public override bool TryGetService(BleUuid guid, out BleService service)
	{
		RequiresServices();
		WinBleService[] services = _services;
		for (int i = 0; i < services.Length; i++)
		{
			if (services[i].Uuid == guid)
			{
				service = services[i];
				return true;
			}
		}
		service = null;
		return false;
	}

	public override string GetFriendlyName()
	{
		return _friendlyName;
	}

	public override string GetFileSystemName()
	{
		return DevicePath;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.Windows;
		}
		return true;
	}
}
