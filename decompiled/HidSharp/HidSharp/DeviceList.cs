using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using HidSharp.Experimental;

namespace HidSharp;

[ComVisible(true)]
[Guid("80614F94-0742-4DE4-8AE9-DF9D55F870F2")]
public abstract class DeviceList
{
	public abstract bool AreDriversBeingInstalled { get; }

	public static DeviceList Local { get; private set; }

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static event EventHandler<DeviceListChangedEventArgs> DeviceListChanged;

	public event EventHandler<DeviceListChangedEventArgs> Changed;

	static DeviceList()
	{
		Local = new LocalDeviceList();
	}

	public virtual IEnumerable<Device> GetDevices(DeviceTypes types)
	{
		return GetAllDevices().Where(delegate(Device device)
		{
			if (device is HidDevice && (types & DeviceTypes.Hid) != 0)
			{
				return true;
			}
			if (device is SerialDevice && (types & DeviceTypes.Serial) != 0)
			{
				return true;
			}
			return (device is BleDevice && (types & DeviceTypes.Ble) != 0) ? true : false;
		});
	}

	public IEnumerable<Device> GetDevices(DeviceTypes types, DeviceFilter filter)
	{
		Throw.If.Null(filter, "filter");
		return from device in GetDevices(types)
			where filter(device)
			select device;
	}

	public IEnumerable<BleDevice> GetBleDevices()
	{
		return GetDevices(DeviceTypes.Ble).Cast<BleDevice>();
	}

	public IEnumerable<HidDevice> GetHidDevices()
	{
		return GetDevices(DeviceTypes.Hid).Cast<HidDevice>();
	}

	public IEnumerable<HidDevice> GetHidDevices(int? vendorID = null, int? productID = null, int? releaseNumberBcd = null, string serialNumber = null)
	{
		return GetDevices(DeviceTypes.Hid, (Device d) => DeviceFilterHelper.MatchHidDevices(d, vendorID, productID, releaseNumberBcd, serialNumber)).Cast<HidDevice>();
	}

	public IEnumerable<SerialDevice> GetSerialDevices()
	{
		return GetDevices(DeviceTypes.Serial).Cast<SerialDevice>();
	}

	public abstract IEnumerable<Device> GetAllDevices();

	public IEnumerable<Device> GetAllDevices(DeviceFilter filter)
	{
		Throw.If.Null(filter, "filter");
		return from device in GetAllDevices()
			where filter(device)
			select device;
	}

	public HidDevice GetHidDeviceOrNull(int? vendorID = null, int? productID = null, int? releaseNumberBcd = null, string serialNumber = null)
	{
		return GetHidDevices(vendorID, productID, releaseNumberBcd, serialNumber).FirstOrDefault();
	}

	public bool TryGetHidDevice(out HidDevice device, int? vendorID = null, int? productID = null, int? releaseNumberBcd = null, string serialNumber = null)
	{
		device = GetHidDeviceOrNull(vendorID, productID, releaseNumberBcd, serialNumber);
		return device != null;
	}

	public SerialDevice GetSerialDeviceOrNull(string portName)
	{
		return GetSerialDevices().Where(delegate(SerialDevice d)
		{
			if (d.DevicePath == portName)
			{
				return true;
			}
			try
			{
				if (d.GetFileSystemName() == portName)
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}).FirstOrDefault();
	}

	public bool TryGetSerialDevice(out SerialDevice device, string portName)
	{
		device = GetSerialDeviceOrNull(portName);
		return device != null;
	}

	public void RaiseChanged()
	{
		this.Changed?.Invoke(this, new DeviceListChangedEventArgs());
		DeviceList.DeviceListChanged?.Invoke(this, new DeviceListChangedEventArgs());
	}
}
