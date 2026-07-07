using System;
using HidSharp.Utility;

namespace HidSharp;

public abstract class Device
{
	public abstract string DevicePath { get; }

	public DeviceStream Open()
	{
		return Open(null);
	}

	public DeviceStream Open(OpenConfiguration openConfig)
	{
		return OpenDeviceAndRestrictAccess(openConfig ?? new OpenConfiguration());
	}

	protected virtual DeviceStream OpenDeviceAndRestrictAccess(OpenConfiguration openConfig)
	{
		bool flag = (bool)openConfig.GetOption(OpenOption.Exclusive);
		DeviceOpenUtility openUtility = null;
		if (flag)
		{
			string streamPath = GetStreamPath(openConfig);
			openUtility = new DeviceOpenUtility(this, streamPath, openConfig);
			openUtility.Open();
		}
		DeviceStream stream;
		try
		{
			stream = OpenDeviceDirectly(openConfig);
			if (flag)
			{
				stream.Closed += delegate
				{
					openUtility.Close();
				};
				openUtility.InterruptRequested += delegate
				{
					stream.OnInterruptRequested();
					HidSharpDiagnostics.Trace("Delivered an interrupt request.");
				};
			}
		}
		catch
		{
			if (flag)
			{
				openUtility.Close();
			}
			throw;
		}
		return stream;
	}

	protected abstract DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig);

	protected virtual string GetStreamPath(OpenConfiguration openConfig)
	{
		return DevicePath;
	}

	public bool TryOpen(out DeviceStream stream)
	{
		return TryOpen(null, out stream);
	}

	public bool TryOpen(OpenConfiguration openConfig, out DeviceStream stream)
	{
		Exception exception;
		return TryOpen(openConfig, out stream, out exception);
	}

	public bool TryOpen(OpenConfiguration openConfig, out DeviceStream stream, out Exception exception)
	{
		try
		{
			stream = Open(openConfig);
			exception = null;
			return true;
		}
		catch (Exception ex)
		{
			stream = null;
			exception = ex;
			return false;
		}
	}

	public abstract string GetFileSystemName();

	public abstract string GetFriendlyName();

	public virtual bool HasImplementationDetail(Guid detail)
	{
		return false;
	}
}
