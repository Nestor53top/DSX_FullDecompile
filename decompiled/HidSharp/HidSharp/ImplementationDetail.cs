using System;

namespace HidSharp;

public static class ImplementationDetail
{
	public static Guid Windows { get; private set; }

	public static Guid MacOS { get; private set; }

	public static Guid Linux { get; private set; }

	public static Guid BleDevice { get; private set; }

	public static Guid HidDevice { get; private set; }

	public static Guid SerialDevice { get; private set; }

	public static Guid HidrawApi { get; private set; }

	static ImplementationDetail()
	{
		Windows = new Guid("{3540D886-E329-419F-8033-1D7355D53A7E}");
		MacOS = new Guid("{9FE992E5-F804-41B6-A35F-3B60F7CAC9E2}");
		Linux = new Guid("{A4123219-6BC8-49B7-84D3-699A66373109}");
		BleDevice = new Guid("{AAFD1479-29A0-42B8-A0A9-5C88A18B5504}");
		HidDevice = new Guid("{DFF209D7-131E-4958-8F47-C23DAC7B62DA}");
		SerialDevice = new Guid("{45A96DA9-AA48-4BF7-978D-A845F185F38C}");
		HidrawApi = new Guid("{1199D7C6-F99F-471F-9730-B16BA615938F}");
	}
}
