using System;

namespace Device.Net.Windows;

public struct SpDeviceInfoData
{
	public uint CbSize;

	public Guid ClassGuid;

	public uint DevInst;

	public IntPtr Reserved;
}
