using System;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Usb.Net.Windows;

public class WindowsUsbInterface : UsbInterfaceBase, IUsbInterface, IDisposable
{
	private bool _IsDisposed;

	private readonly SafeFileHandle _SafeFileHandle;

	public override byte InterfaceNumber { get; }

	public WindowsUsbInterface(SafeFileHandle handle, ILogger logger, ITracer tracer, byte interfaceNumber, ushort? readBufferSize, ushort? writeBufferSzie)
		: base(logger, tracer, readBufferSize, writeBufferSzie)
	{
		_SafeFileHandle = handle;
		InterfaceNumber = interfaceNumber;
	}

	public async Task<ReadResult> ReadAsync(uint bufferLength)
	{
		return await Task.Run(delegate
		{
			byte[] array = new byte[bufferLength];
			WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_ReadPipe(_SafeFileHandle, base.ReadEndpoint.PipeId, array, bufferLength, out var LengthTransferred, IntPtr.Zero), "Couldn't read data");
			base.Tracer?.Trace(isWrite: false, array);
			return new ReadResult(array, LengthTransferred);
		});
	}

	public async Task WriteAsync(byte[] data)
	{
		await Task.Run(delegate
		{
			WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_WritePipe(_SafeFileHandle, base.WriteEndpoint.PipeId, data, (uint)data.Length, out var _, IntPtr.Zero), "Couldn't write data");
			base.Tracer?.Trace(isWrite: true, data);
		});
	}

	public void Dispose()
	{
		if (!_IsDisposed)
		{
			_IsDisposed = true;
			WindowsDeviceBase.HandleError(WinUsbApiCalls.WinUsb_Free(_SafeFileHandle), "Interface could not be disposed");
			GC.SuppressFinalize(this);
		}
	}
}
