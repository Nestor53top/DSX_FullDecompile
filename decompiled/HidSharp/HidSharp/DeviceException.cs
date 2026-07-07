using System;
using System.IO;
using HidSharp.Exceptions;

namespace HidSharp;

public static class DeviceException
{
	public static IOException CreateIOException(Device device, string message)
	{
		return new DeviceIOException(device, message);
	}

	public static IOException CreateIOException(Device device, string message, int hresult)
	{
		return new DeviceIOException(device, message, hresult);
	}

	public static UnauthorizedAccessException CreateUnauthorizedAccessException(Device device, string message)
	{
		return new DeviceUnauthorizedAccessException(device, message);
	}

	public static Device GetDevice(Exception exception)
	{
		if (!(exception is IDeviceException ex))
		{
			return null;
		}
		return ex.Device;
	}
}
