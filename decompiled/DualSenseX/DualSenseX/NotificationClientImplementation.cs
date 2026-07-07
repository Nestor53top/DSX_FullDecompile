using System;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace DualSenseX;

public class NotificationClientImplementation : IMMNotificationClient
{
	public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
	{
		try
		{
			GlobalVar.DidDefaultDeviceChange = true;
		}
		catch (Exception)
		{
		}
	}

	public void OnDeviceAdded(string deviceId)
	{
	}

	public void OnDeviceRemoved(string deviceId)
	{
	}

	public void OnDeviceStateChanged(string deviceId, DeviceState newState)
	{
	}

	public NotificationClientImplementation()
	{
		if (Environment.OSVersion.Version.Major < 6)
		{
			throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
		}
	}

	public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
	{
	}
}
