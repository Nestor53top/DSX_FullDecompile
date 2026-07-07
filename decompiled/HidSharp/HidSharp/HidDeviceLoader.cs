using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HidSharp;

[Guid("CD7CBD7D-7204-473c-AA2A-2B9622CFC6CC")]
[Obsolete]
[EditorBrowsable(EditorBrowsableState.Never)]
[ComVisible(true)]
public class HidDeviceLoader
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public HidDeviceLoader()
	{
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerable GetDevicesVB()
	{
		return DeviceList.Local.GetHidDevices();
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerable<HidDevice> GetDevices()
	{
		return DeviceList.Local.GetHidDevices();
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerable<HidDevice> GetDevices(int? vendorID = null, int? productID = null, int? productVersion = null, string serialNumber = null)
	{
		return DeviceList.Local.GetHidDevices(vendorID, productID, productVersion, serialNumber);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public HidDevice GetDeviceOrDefault(int? vendorID = null, int? productID = null, int? productVersion = null, string serialNumber = null)
	{
		return DeviceList.Local.GetHidDeviceOrNull(vendorID, productID, productVersion, serialNumber);
	}
}
