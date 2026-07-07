using System;

namespace HidSharp;

public abstract class SerialDevice : Device
{
	public new SerialStream Open()
	{
		return Open(null);
	}

	public new SerialStream Open(OpenConfiguration openConfig)
	{
		return (SerialStream)base.Open(openConfig);
	}

	public bool TryOpen(out SerialStream stream)
	{
		return TryOpen(null, out stream);
	}

	public bool TryOpen(OpenConfiguration openConfig, out SerialStream stream)
	{
		DeviceStream stream2;
		bool result = TryOpen(openConfig, out stream2);
		stream = (SerialStream)stream2;
		return result;
	}

	public override string GetFriendlyName()
	{
		return GetFileSystemName();
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.SerialDevice;
		}
		return true;
	}

	public override string ToString()
	{
		string arg = "(unknown filesystem name)";
		try
		{
			arg = GetFileSystemName();
		}
		catch
		{
		}
		return $"{arg} ({DevicePath})";
	}
}
