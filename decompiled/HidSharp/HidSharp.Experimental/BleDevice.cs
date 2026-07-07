using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HidSharp.Experimental;

[ComVisible(true)]
[Guid("A7AEE7B8-893D-41B6-84F7-6BDA4EE3AA3F")]
public abstract class BleDevice : Device
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new BleStream Open()
	{
		return (BleStream)base.Open();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new BleStream Open(OpenConfiguration openConfig)
	{
		return (BleStream)base.Open(openConfig);
	}

	public BleStream Open(BleService service)
	{
		return Open(service, new OpenConfiguration());
	}

	public BleStream Open(BleService service, OpenConfiguration openConfig)
	{
		Throw.If.Null(service).Null(openConfig);
		openConfig = openConfig.Clone();
		openConfig.SetOption(OpenOption.BleService, service);
		return Open(openConfig);
	}

	public abstract BleService[] GetServices();

	public BleService GetServiceOrNull(BleUuid uuid)
	{
		if (!TryGetService(uuid, out var service))
		{
			return null;
		}
		return service;
	}

	public virtual bool HasService(BleUuid uuid)
	{
		BleService service;
		return TryGetService(uuid, out service);
	}

	public virtual bool TryGetService(BleUuid uuid, out BleService service)
	{
		BleService[] services = GetServices();
		foreach (BleService bleService in services)
		{
			if (bleService.Uuid == uuid)
			{
				service = bleService;
				return true;
			}
		}
		service = null;
		return false;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.BleDevice;
		}
		return true;
	}

	public override string ToString()
	{
		string result = "(unknown friendly name)";
		try
		{
			result = GetFriendlyName();
		}
		catch
		{
		}
		return result;
	}
}
