using System;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Exceptions;

namespace Usb.Net;

public class UsbDevice : DeviceBase, IUsbDevice, IDevice, IDisposable
{
	private bool disposed;

	private bool _IsClosing;

	public override bool IsInitialized => UsbInterfaceManager.IsInitialized;

	public IUsbInterfaceManager UsbInterfaceManager { get; }

	public override ushort WriteBufferSize => UsbInterfaceManager.WriteBufferSize;

	public override ushort ReadBufferSize => UsbInterfaceManager.ReadBufferSize;

	public UsbDevice(string deviceId, IUsbInterfaceManager usbInterfaceManager, ILogger logger, ITracer tracer)
		: base(deviceId, logger, tracer)
	{
		UsbInterfaceManager = usbInterfaceManager ?? throw new ArgumentNullException("usbInterfaceManager");
	}

	public async Task InitializeAsync()
	{
		await UsbInterfaceManager.InitializeAsync();
		base.ConnectedDeviceDefinition = await UsbInterfaceManager.GetConnectedDeviceDefinitionAsync();
	}

	public override async Task<ReadResult> ReadAsync()
	{
		if (UsbInterfaceManager.ReadUsbInterface == null)
		{
			throw new DeviceException("There was no read Usb Interface specified for the device.");
		}
		return await UsbInterfaceManager.ReadUsbInterface.ReadAsync(ReadBufferSize);
	}

	public override Task WriteAsync(byte[] data)
	{
		if (UsbInterfaceManager.WriteUsbInterface == null)
		{
			throw new DeviceException("There was no write Usb Interface specified for the device.");
		}
		return UsbInterfaceManager.WriteUsbInterface.WriteAsync(data);
	}

	public void Close()
	{
		if (!_IsClosing)
		{
			_IsClosing = true;
			try
			{
				UsbInterfaceManager?.Close();
			}
			catch (Exception)
			{
			}
			_IsClosing = false;
		}
	}

	public sealed override void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			Close();
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	~UsbDevice()
	{
		Dispose();
	}

	Task<ReadResult> IDevice.WriteAndReadAsync(byte[] writeBuffer)
	{
		return WriteAndReadAsync(writeBuffer);
	}
}
