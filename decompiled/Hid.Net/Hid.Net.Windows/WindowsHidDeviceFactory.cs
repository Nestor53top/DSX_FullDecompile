using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Exceptions;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Hid.Net.Windows;

public class WindowsHidDeviceFactory : WindowsDeviceFactoryBase, IDeviceFactory
{
	public override DeviceType DeviceType => DeviceType.Hid;

	public IHidApiService HidService { get; }

	protected override ConnectedDeviceDefinition GetDeviceDefinition(string deviceId)
	{
		try
		{
			using SafeFileHandle safeFileHandle = HidService.CreateReadConnection(deviceId, FileAccessRights.None);
			if (safeFileHandle.IsInvalid)
			{
				throw new DeviceException("CreateReadConnection call with Id of " + deviceId + " failed.");
			}
			base.Logger?.Log("Found device " + deviceId, "WindowsHidDeviceFactory", null, LogLevel.Information);
			return HidService.GetDeviceDefinition(deviceId, safeFileHandle);
		}
		catch (Exception ex)
		{
			base.Logger?.Log("GetDeviceDefinition error. Device Id: " + deviceId, "WindowsHidDeviceFactory", ex, LogLevel.Error);
			return null;
		}
	}

	protected override Guid GetClassGuid()
	{
		return HidService.GetHidGuid();
	}

	public WindowsHidDeviceFactory(ILogger logger, ITracer tracer)
		: this(logger, tracer, null)
	{
	}

	public WindowsHidDeviceFactory(ILogger logger, ITracer tracer, IHidApiService hidService)
		: base(logger, tracer)
	{
		HidService = hidService;
		if (HidService == null)
		{
			HidService = new WindowsHidApiService(logger);
		}
	}

	public IDevice GetDevice(ConnectedDeviceDefinition deviceDefinition)
	{
		if (deviceDefinition == null)
		{
			throw new ArgumentNullException("deviceDefinition");
		}
		if (deviceDefinition.DeviceType == DeviceType)
		{
			return new WindowsHidDevice(deviceDefinition.DeviceId, base.Logger, base.Tracer);
		}
		return null;
	}

	public static void Register(ILogger logger, ITracer tracer)
	{
		DeviceManager.Current.DeviceFactories.Add(new WindowsHidDeviceFactory(logger, tracer));
	}

	Task<IEnumerable<ConnectedDeviceDefinition>> IDeviceFactory.GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition deviceDefinition)
	{
		return GetConnectedDeviceDefinitionsAsync(deviceDefinition);
	}
}
