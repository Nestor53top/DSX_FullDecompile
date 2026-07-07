using System;

namespace Device.Net.Windows;

public struct SpDeviceInterfaceData
{
	public uint CbSize;

	public Guid InterfaceClassGuid;

	public uint Flags;

	public IntPtr Reserved;
}
