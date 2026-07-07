using System;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.Libusb;

internal static class NativeMethods
{
	public struct DeviceDescriptor
	{
		public byte bLength;

		public byte bDescriptorType;

		public ushort bcdUSB;

		public byte bDeviceClass;

		public byte bDeviceSubClass;

		public byte bDeviceProtocol;

		public byte bMaxPacketSize0;

		public ushort idVendor;

		public ushort idProduct;

		public ushort bcdDevice;

		public byte iManufacturer;

		public byte iProduct;

		public byte iSerialNumber;

		public byte bNumConfigurations;
	}

	public enum DeviceClass : byte
	{
		HID = 3,
		MassStorage = 8,
		VendorSpecific = byte.MaxValue
	}

	public enum DescriptorType : byte
	{
		Device = 1,
		Configuration = 2,
		String = 3,
		Interface = 4,
		Endpoint = 5,
		HID = 33,
		Report = 34,
		Physical = 35,
		Hub = 41
	}

	public enum EndpointDirection : byte
	{
		In = 128,
		Out = 0
	}

	public enum Request : byte
	{
		GetDescriptor = 6
	}

	public enum RequestRecipient : byte
	{
		Device,
		Interface,
		Endpoint,
		Other
	}

	public enum RequestType : byte
	{
		Standard = 0,
		Class = 0x20,
		Vendor = 0x40
	}

	public enum TransferType : byte
	{
		Control,
		Isochronous,
		Bulk,
		Interrupt
	}

	public struct Version
	{
		public ushort Major;

		public ushort Minor;

		public ushort Micro;

		public ushort Nano;
	}

	public enum Error
	{
		None = 0,
		IO = -1,
		InvalidParameter = -2,
		AccessDenied = -3,
		NoDevice = -4,
		NotFound = -5,
		Busy = -6,
		Timeout = -7,
		Overflow = -8,
		Pipe = -9,
		Interrupted = -10,
		OutOfMemory = -11,
		NotSupported = -12
	}

	private const string Libusb = "libusb-1.0";

	[DllImport("libusb-1.0")]
	public static extern Error libusb_init(out IntPtr context);

	[DllImport("libusb-1.0")]
	public static extern void libusb_set_debug(IntPtr context, int level);

	[DllImport("libusb-1.0")]
	public static extern void libusb_exit(IntPtr context);

	[DllImport("libusb-1.0")]
	public static extern IntPtr libusb_get_device_list(IntPtr context, out IntPtr list);

	[DllImport("libusb-1.0")]
	public static extern void libusb_free_device_list(IntPtr context, IntPtr list);

	[DllImport("libusb-1.0")]
	public static extern IntPtr libusb_ref_device(IntPtr device);

	[DllImport("libusb-1.0")]
	public static extern void libusb_unref_device(IntPtr device);

	[DllImport("libusb-1.0")]
	public static extern int libusb_get_max_packet_size(IntPtr device, byte endpoint);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_open(IntPtr device, out IntPtr deviceHandle);

	[DllImport("libusb-1.0")]
	public static extern void libusb_close(IntPtr deviceHandle);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_get_configuration(IntPtr deviceHandle, out int configuration);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_set_configuration(IntPtr deviceHandle, int configuration);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_claim_interface(IntPtr deviceHandle, int @interface);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_release_interface(IntPtr deviceHandle, int @interface);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_set_interface_alt_setting(IntPtr deviceHandle, int @interface, int altSetting);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_clear_halt(IntPtr deviceHandle, byte endpoint);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_reset_device(IntPtr deviceHandle);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_kernel_driver_active(IntPtr deviceHandle, int @interface);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_detach_kernel_driver(IntPtr deviceHandle, int @interface);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_attach_kernel_driver(IntPtr deviceHandle, int @interface);

	[DllImport("libusb-1.0")]
	public static extern IntPtr libusb_get_version();

	[DllImport("libusb-1.0")]
	public static extern Error libusb_get_device_descriptor(IntPtr device, out DeviceDescriptor descriptor);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_get_active_config_descriptor(IntPtr device, out IntPtr configuration);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_get_config_descriptor_by_value(IntPtr device, byte index, out IntPtr configuration);

	[DllImport("libusb-1.0")]
	public static extern void libusb_free_config_descriptor(IntPtr configuration);

	private static Error libusb_get_descriptor_core(IntPtr deviceHandle, DescriptorType type, byte index, byte[] data, ushort wLength, ushort wIndex)
	{
		return libusb_control_transfer(deviceHandle, 128, 6, (ushort)(0x300 | index), wIndex, data, wLength, 1000u);
	}

	public static Error libusb_get_descriptor(IntPtr deviceHandle, DescriptorType type, byte index, byte[] data, ushort wLength)
	{
		return libusb_get_descriptor_core(deviceHandle, type, index, data, wLength, 0);
	}

	public static Error libusb_get_string_descriptor(IntPtr deviceHandle, DescriptorType type, byte index, ushort languageID, byte[] data, ushort wLength)
	{
		return libusb_get_descriptor_core(deviceHandle, DescriptorType.String, index, data, wLength, languageID);
	}

	[DllImport("libusb-1.0")]
	public static extern Error libusb_control_transfer(IntPtr deviceHandle, byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, byte[] data, ushort wLength, uint timeout);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_bulk_transfer(IntPtr deviceHandle, byte endpoint, byte[] data, int length, out int transferred, uint timeout);

	[DllImport("libusb-1.0")]
	public static extern Error libusb_interrupt_transfer(IntPtr deviceHandle, byte endpoint, byte[] data, int length, out int transferred, uint timeout);
}
