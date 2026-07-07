using System;

namespace Device.Net.Exceptions;

public class DeviceFactoriesNotRegisteredException : Exception
{
	public DeviceFactoriesNotRegisteredException()
		: base("No device factories have been registered")
	{
	}

	public DeviceFactoriesNotRegisteredException(string message)
		: base(message)
	{
	}

	public DeviceFactoriesNotRegisteredException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
