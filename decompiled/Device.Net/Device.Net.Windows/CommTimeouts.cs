namespace Device.Net.Windows;

public struct CommTimeouts
{
	public uint ReadIntervalTimeout;

	public uint ReadTotalTimeoutMultiplier;

	public uint ReadTotalTimeoutConstant;

	public uint WriteTotalTimeoutMultiplier;

	public uint WriteTotalTimeoutConstant;
}
