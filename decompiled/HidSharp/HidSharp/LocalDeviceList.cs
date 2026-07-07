using System.Collections.Generic;
using HidSharp.Platform;

namespace HidSharp;

internal sealed class LocalDeviceList : DeviceList
{
	public override bool AreDriversBeingInstalled => HidSelector.Instance.AreDriversBeingInstalled;

	public override IEnumerable<Device> GetDevices(DeviceTypes types)
	{
		return HidSelector.Instance.GetDevices(types);
	}

	public override IEnumerable<Device> GetAllDevices()
	{
		return GetDevices(DeviceTypes.Hid | DeviceTypes.Serial | DeviceTypes.Ble);
	}

	public override string ToString()
	{
		return HidSelector.Instance.FriendlyName;
	}
}
