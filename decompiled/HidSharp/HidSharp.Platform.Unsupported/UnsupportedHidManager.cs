using System;

namespace HidSharp.Platform.Unsupported;

internal sealed class UnsupportedHidManager : HidManager
{
	public override string FriendlyName => "Platform Not Supported";

	public override bool IsSupported => true;

	protected override object[] GetBleDeviceKeys()
	{
		return new object[0];
	}

	protected override object[] GetHidDeviceKeys()
	{
		return new object[0];
	}

	protected override object[] GetSerialDeviceKeys()
	{
		return new object[0];
	}

	protected override bool TryCreateBleDevice(object key, out Device device)
	{
		throw new NotImplementedException();
	}

	protected override bool TryCreateHidDevice(object key, out Device device)
	{
		throw new NotImplementedException();
	}

	protected override bool TryCreateSerialDevice(object key, out Device device)
	{
		throw new NotImplementedException();
	}
}
