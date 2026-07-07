namespace Usb.Net.Windows;

public struct USB_CONFIGURATION_DESCRIPTOR
{
	public byte bLength;

	public byte bDescriptorType;

	public ushort wTotalLength;

	public byte bNumInterfaces;

	public byte bConfigurationValue;

	public byte iConfiguration;

	public byte bmAttributes;

	public byte MaxPower;
}
