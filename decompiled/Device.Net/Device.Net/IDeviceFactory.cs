using System.Collections.Generic;
using System.Threading.Tasks;

namespace Device.Net;

public interface IDeviceFactory
{
	DeviceType DeviceType { get; }

	Task<IEnumerable<ConnectedDeviceDefinition>> GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition deviceDefinition);

	IDevice GetDevice(ConnectedDeviceDefinition deviceDefinition);
}
