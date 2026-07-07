using System;

namespace HidSharp.Exceptions;

internal sealed class DeviceUnauthorizedAccessException : UnauthorizedAccessException, IDeviceException
{
	public Device Device { get; private set; }

	public DeviceUnauthorizedAccessException(Device device, string message)
		: base(message)
	{
		Device = device;
	}
}
