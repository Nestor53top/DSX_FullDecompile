using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Exceptions;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Usb.Net.Windows;

public class WindowsUsbInterfaceManager : UsbInterfaceManager, IUsbInterfaceManager, IDeviceHandler, IDisposable
{
	private bool disposed;

	private SafeFileHandle _DeviceHandle;

	protected ushort? _ReadBufferSize { get; set; }

	protected ushort? _WriteBufferSize { get; set; }

	public bool IsInitialized
	{
		get
		{
			if (_DeviceHandle != null)
			{
				return !_DeviceHandle.IsInvalid;
			}
			return false;
		}
	}

	public string DeviceId { get; }

	public ushort WriteBufferSize => _WriteBufferSize ?? base.WriteUsbInterface.ReadBufferSize;

	public ushort ReadBufferSize => _ReadBufferSize ?? base.ReadUsbInterface.ReadBufferSize;

	public WindowsUsbInterfaceManager(string deviceId, ILogger logger, ITracer tracer, ushort? writeBufferLength, ushort? readBufferLength)
		: base(logger, tracer)
	{
		_ReadBufferSize = readBufferLength;
		_WriteBufferSize = writeBufferLength;
		DeviceId = deviceId;
	}

	private void Initialize()
	{
		try
		{
			Close();
			if (string.IsNullOrEmpty(DeviceId))
			{
				throw new ValidationException("DeviceDefinitionBase must be specified before InitializeAsync can be called.");
			}
			_DeviceHandle = APICalls.CreateFile(DeviceId, FileAccessRights.GenericRead | FileAccessRights.GenericWrite, 3u, IntPtr.Zero, 3u, 1073741952u, IntPtr.Zero);
			int lastWin32Error;
			if (_DeviceHandle.IsInvalid)
			{
				lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error > 0)
				{
					throw new ApiException($"Device handle no good. Error code: {lastWin32Error}");
				}
			}
			base.Logger?.Log("Successfully opened handle on device for reading and writing", "WindowsUsbInterfaceManager", null, LogLevel.Information);
			WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_Initialize(_DeviceHandle, out var InterfaceHandle), "Couldn't initialize device");
			ConnectedDeviceDefinition deviceDefinition = WindowsUsbDeviceFactory.GetDeviceDefinition(InterfaceHandle, DeviceId);
			if (!_WriteBufferSize.HasValue)
			{
				if (!deviceDefinition.WriteBufferSize.HasValue)
				{
					throw new ValidationException("Write buffer size not specified");
				}
				_WriteBufferSize = (ushort)deviceDefinition.WriteBufferSize.Value;
			}
			if (!_ReadBufferSize.HasValue)
			{
				if (!deviceDefinition.ReadBufferSize.HasValue)
				{
					throw new ValidationException("Read buffer size not specified");
				}
				_ReadBufferSize = (ushort)deviceDefinition.ReadBufferSize.Value;
			}
			WindowsUsbInterface item = GetInterface(InterfaceHandle);
			base.UsbInterfaces.Add(item);
			byte b = 0;
			SafeFileHandle AssociatedInterfaceHandle;
			while (WinUsbApiCalls.WinUsb_GetAssociatedInterface(InterfaceHandle, b, out AssociatedInterfaceHandle))
			{
				WindowsUsbInterface item2 = GetInterface(AssociatedInterfaceHandle);
				base.UsbInterfaces.Add(item2);
				b++;
			}
			lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 259)
			{
				throw new ApiException($"Could not enumerate interfaces for device. Error code: {lastWin32Error}");
			}
			RegisterDefaultInterfaces();
		}
		catch (Exception ex)
		{
			base.Logger?.Log("Initialize error. DeviceId " + DeviceId, "UsbDevice", ex, LogLevel.Error);
			throw;
		}
	}

	private WindowsUsbInterface GetInterface(SafeFileHandle interfaceHandle)
	{
		USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor;
		bool isSuccess = WinUsbApiCalls.WinUsb_QueryInterfaceSettings(interfaceHandle, 0, out UsbAltInterfaceDescriptor);
		WindowsUsbInterface windowsUsbInterface = new WindowsUsbInterface(interfaceHandle, base.Logger, base.Tracer, UsbAltInterfaceDescriptor.bInterfaceNumber, _ReadBufferSize, _WriteBufferSize);
		WindowsDeviceBase.HandleError(isSuccess, "Couldn't query interface");
		for (byte b = 0; b < UsbAltInterfaceDescriptor.bNumEndpoints; b++)
		{
			WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_QueryPipe(interfaceHandle, 0, b, out var PipeInformation), "Couldn't query endpoint");
			windowsUsbInterface.UsbInterfaceEndpoints.Add(new WindowsUsbInterfaceEndpoint(PipeInformation.PipeId, PipeInformation.PipeType));
		}
		return windowsUsbInterface;
	}

	public void Close()
	{
		foreach (IUsbInterface usbInterface in base.UsbInterfaces)
		{
			usbInterface.Dispose();
		}
		base.UsbInterfaces.Clear();
		_DeviceHandle?.Dispose();
		_DeviceHandle = null;
	}

	public override void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			Close();
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public async Task InitializeAsync()
	{
		await Task.Run((Action)Initialize);
	}

	public Task<ConnectedDeviceDefinitionBase> GetConnectedDeviceDefinitionAsync()
	{
		if (_DeviceHandle == null)
		{
			throw new NotInitializedException();
		}
		return Task.Run((Func<ConnectedDeviceDefinitionBase>)(() => WindowsDeviceFactoryBase.GetDeviceDefinitionFromWindowsDeviceId(DeviceId, DeviceType.Usb, base.Logger)));
	}
}
