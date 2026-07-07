using System;

namespace HidSharp.Platform.MacOS;

internal sealed class MacSerialDevice : SerialDevice
{
	private NativeMethods.io_string_t _path;

	private string _fileSystemName;

	public override string DevicePath => _path.ToString();

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		return new MacSerialStream(this);
	}

	internal static MacSerialDevice TryCreate(NativeMethods.io_string_t path)
	{
		MacSerialDevice macSerialDevice = new MacSerialDevice();
		macSerialDevice._path = path;
		MacSerialDevice macSerialDevice2 = macSerialDevice;
		NativeMethods.IOObject iOObject = NativeMethods.IORegistryEntryFromPath(0, ref path).ToIOObject();
		if (!iOObject.IsSet)
		{
			return null;
		}
		using (iOObject)
		{
			macSerialDevice2._fileSystemName = NativeMethods.IORegistryEntryGetCFProperty_String(iOObject, NativeMethods.kIOCalloutDeviceKey);
			if (macSerialDevice2._fileSystemName == null)
			{
				return null;
			}
			return macSerialDevice2;
		}
	}

	public override string GetFileSystemName()
	{
		return _fileSystemName;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.MacOS;
		}
		return true;
	}
}
