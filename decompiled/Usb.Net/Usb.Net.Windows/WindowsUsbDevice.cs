using System;
using Device.Net;

namespace Usb.Net.Windows;

[Obsolete("Platform specific USB Devices are being deprecated. Please construct a UsbDevice and pass the UsbInterfaceManager in to the constructor. This is to maintain the dependency injection pattern.")]
public class WindowsUsbDevice : UsbDevice
{
	public WindowsUsbDevice(string deviceId, ILogger logger, ITracer tracer, ushort? readBufferSize, ushort? writeBufferSize)
		: base(deviceId, new WindowsUsbInterfaceManager(deviceId, logger, tracer, readBufferSize, writeBufferSize), logger, tracer)
	{
	}
}
