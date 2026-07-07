using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Usb.Net.Windows;

public class WindowsUsbDeviceFactory : WindowsDeviceFactoryBase, IDeviceFactory
{
	public ushort? ReadBufferSize { get; set; }

	public ushort? WriteBufferSize { get; set; }

	public override DeviceType DeviceType => DeviceType.Usb;

	protected override Guid GetClassGuid()
	{
		return WindowsDeviceConstants.WinUSBGuid;
	}

	public WindowsUsbDeviceFactory(ILogger logger, ITracer tracer)
		: base(logger, tracer)
	{
	}

	public IDevice GetDevice(ConnectedDeviceDefinition deviceDefinition)
	{
		if (deviceDefinition == null)
		{
			throw new ArgumentNullException("deviceDefinition");
		}
		if (deviceDefinition.DeviceType == DeviceType)
		{
			return new UsbDevice(deviceDefinition.DeviceId, new WindowsUsbInterfaceManager(deviceDefinition.DeviceId, base.Logger, base.Tracer, ReadBufferSize, WriteBufferSize), base.Logger, base.Tracer);
		}
		return null;
	}

	protected override ConnectedDeviceDefinition GetDeviceDefinition(string deviceId)
	{
		return WindowsDeviceFactoryBase.GetDeviceDefinitionFromWindowsDeviceId(deviceId, DeviceType.Usb, base.Logger);
	}

	public static void Register(ILogger logger, ITracer tracer)
	{
		DeviceManager.Current.DeviceFactories.Add(new WindowsUsbDeviceFactory(logger, tracer));
	}

	public static ConnectedDeviceDefinition GetDeviceDefinition(SafeFileHandle defaultInterfaceHandle, string deviceId)
	{
		ConnectedDeviceDefinition connectedDeviceDefinition = new ConnectedDeviceDefinition(deviceId)
		{
			DeviceType = DeviceType.Usb
		};
		uint bufferLength = (uint)Marshal.SizeOf(typeof(USB_DEVICE_DESCRIPTOR));
		WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_GetDescriptor(defaultInterfaceHandle, 1, 0, 1033, out var deviceDesc, bufferLength, out var _), "Couldn't get device descriptor");
		if (deviceDesc.iProduct > 0)
		{
			connectedDeviceDefinition.ProductName = WinUsbApiCalls.GetDescriptor(defaultInterfaceHandle, deviceDesc.iProduct, "Couldn't get product name");
		}
		if (deviceDesc.iSerialNumber > 0)
		{
			connectedDeviceDefinition.SerialNumber = WinUsbApiCalls.GetDescriptor(defaultInterfaceHandle, deviceDesc.iSerialNumber, "Couldn't get serial number");
		}
		if (deviceDesc.iManufacturer > 0)
		{
			connectedDeviceDefinition.Manufacturer = WinUsbApiCalls.GetDescriptor(defaultInterfaceHandle, deviceDesc.iManufacturer, "Couldn't get manufacturer");
		}
		connectedDeviceDefinition.VendorId = deviceDesc.idVendor;
		connectedDeviceDefinition.ProductId = deviceDesc.idProduct;
		connectedDeviceDefinition.WriteBufferSize = deviceDesc.bMaxPacketSize0;
		connectedDeviceDefinition.ReadBufferSize = deviceDesc.bMaxPacketSize0;
		return connectedDeviceDefinition;
	}

	Task<IEnumerable<ConnectedDeviceDefinition>> IDeviceFactory.GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition deviceDefinition)
	{
		return GetConnectedDeviceDefinitionsAsync(deviceDefinition);
	}
}
