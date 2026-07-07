using System;

namespace HidSharp.Platform.Linux;

internal sealed class LinuxSerialDevice : SerialDevice
{
	private string _portName;

	public override string DevicePath => _portName;

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		return new LinuxSerialStream(this);
	}

	internal static LinuxSerialDevice TryCreate(string portName)
	{
		LinuxSerialDevice linuxSerialDevice = new LinuxSerialDevice();
		linuxSerialDevice._portName = portName;
		return linuxSerialDevice;
	}

	public override string GetFileSystemName()
	{
		return _portName;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.Linux;
		}
		return true;
	}
}
