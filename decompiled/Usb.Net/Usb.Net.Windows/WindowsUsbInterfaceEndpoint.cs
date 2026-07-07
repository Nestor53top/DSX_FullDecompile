using System;

namespace Usb.Net.Windows;

public class WindowsUsbInterfaceEndpoint : IUsbInterfaceEndpoint
{
	public byte PipeId { get; }

	public bool IsRead => (PipeId & 0x80) != 0;

	public bool IsWrite => (PipeId & 0x80) == 0;

	public ushort MaxPacketSize
	{
		get
		{
			throw new NotImplementedException("Need to call WinUsb_GetPipePolicy. https://github.com/MelbourneDeveloper/Device.Net/issues/72");
		}
	}

	public bool IsInterrupt { get; }

	internal WindowsUsbInterfaceEndpoint(byte pipeId, WinUsbApiCalls.USBD_PIPE_TYPE usbPipeType)
	{
		PipeId = pipeId;
		IsInterrupt = usbPipeType == WinUsbApiCalls.USBD_PIPE_TYPE.UsbdPipeTypeInterrupt;
	}

	public override string ToString()
	{
		return PipeId.ToString();
	}
}
