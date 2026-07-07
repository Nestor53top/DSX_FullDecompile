using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device.Net.Exceptions;

namespace Device.Net;

public class DeviceManager
{
	public List<IDeviceFactory> DeviceFactories { get; } = new List<IDeviceFactory>();

	public static DeviceManager Current { get; } = new DeviceManager();

	public async Task<IEnumerable<ConnectedDeviceDefinition>> GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition deviceDefinition)
	{
		if (DeviceFactories.Count == 0)
		{
			throw new DeviceFactoriesNotRegisteredException();
		}
		List<ConnectedDeviceDefinition> retVal = new List<ConnectedDeviceDefinition>();
		foreach (IDeviceFactory deviceFactory in DeviceFactories)
		{
			foreach (ConnectedDeviceDefinition item in await deviceFactory.GetConnectedDeviceDefinitionsAsync(deviceDefinition))
			{
				if (!retVal.Select((ConnectedDeviceDefinition d) => d.DeviceId).Contains(item.DeviceId))
				{
					retVal.Add(item);
				}
			}
		}
		return retVal;
	}

	public IDevice GetDevice(ConnectedDeviceDefinition connectedDeviceDefinition)
	{
		if (connectedDeviceDefinition == null)
		{
			throw new ArgumentNullException("connectedDeviceDefinition");
		}
		using (IEnumerator<IDeviceFactory> enumerator = DeviceFactories.Where((IDeviceFactory deviceFactory) => !connectedDeviceDefinition.DeviceType.HasValue || deviceFactory.DeviceType == connectedDeviceDefinition.DeviceType).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current.GetDevice(connectedDeviceDefinition);
			}
		}
		throw new DeviceException("Couldn't get a device");
	}

	public async Task<List<IDevice>> GetDevicesAsync(IList<FilterDeviceDefinition> deviceDefinitions)
	{
		if (deviceDefinitions == null)
		{
			throw new ArgumentNullException("deviceDefinitions", "GetConnectedDeviceDefinitionsAsync can be used to enumerate all devices without specifying definitions.");
		}
		List<IDevice> retVal = new List<IDevice>();
		foreach (IDeviceFactory deviceFactory in DeviceFactories)
		{
			foreach (FilterDeviceDefinition deviceDefinition in deviceDefinitions)
			{
				if (!deviceDefinition.DeviceType.HasValue || deviceFactory.DeviceType == deviceDefinition.DeviceType)
				{
					retVal.AddRange(from connectedDeviceDefinition in await deviceFactory.GetConnectedDeviceDefinitionsAsync(deviceDefinition)
						select deviceFactory.GetDevice(connectedDeviceDefinition) into device
						where device != null
						select device);
				}
			}
		}
		return retVal;
	}

	public static bool IsDefinitionMatch(FilterDeviceDefinition filterDevice, ConnectedDeviceDefinition actualDevice)
	{
		if (actualDevice == null)
		{
			throw new ArgumentNullException("actualDevice");
		}
		if (filterDevice == null)
		{
			return true;
		}
		bool num = !filterDevice.VendorId.HasValue || filterDevice.VendorId == actualDevice.VendorId;
		bool flag = !filterDevice.ProductId.HasValue || filterDevice.ProductId == actualDevice.ProductId;
		bool flag2 = !filterDevice.DeviceType.HasValue || filterDevice.DeviceType == actualDevice.DeviceType;
		bool flag3 = !filterDevice.UsagePage.HasValue || filterDevice.UsagePage == actualDevice.UsagePage;
		return num && flag && flag2 && flag3;
	}
}
