namespace Usb.Net.Windows;

public struct USB_INTERFACE_DESCRIPTOR
{
	public byte bLength;

	public byte bDescriptorType;

	public byte bInterfaceNumber;

	public byte bAlternateSetting;

	public byte bNumEndpoints;

	public byte bInterfaceClass;

	public byte bInterfaceSubClass;

	public byte bInterfaceProtocol;

	public byte iInterface;
}
