using System;

namespace Device.Net;

public class ConnectedDeviceDefinition : ConnectedDeviceDefinitionBase
{
	public string DeviceId { get; }

	public ConnectedDeviceDefinition(string deviceId)
	{
		if (string.IsNullOrEmpty(deviceId))
		{
			throw new ArgumentNullException("deviceId");
		}
		DeviceId = deviceId;
	}
}
