using System.IO;

namespace HidSharp.Exceptions;

internal sealed class DeviceIOException : IOException, IDeviceException
{
	public Device Device { get; private set; }

	public DeviceIOException(Device device, string message)
		: base(message)
	{
		Device = device;
	}

	public DeviceIOException(Device device, string message, int hresult)
		: base(message, hresult)
	{
		Device = device;
	}
}
