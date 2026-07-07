namespace Usb.Net.Windows;

public struct WINUSB_PIPE_INFORMATION
{
	public WinUsbApiCalls.USBD_PIPE_TYPE PipeType;

	public byte PipeId;

	public ushort MaximumPacketSize;

	public byte Interval;
}
