using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device.Net;

namespace Usb.Net;

public interface IUsbInterface : IDisposable
{
	IUsbInterfaceEndpoint ReadEndpoint { get; set; }

	IList<IUsbInterfaceEndpoint> UsbInterfaceEndpoints { get; }

	IUsbInterfaceEndpoint WriteEndpoint { get; set; }

	IUsbInterfaceEndpoint InterruptWriteEndpoint { get; set; }

	IUsbInterfaceEndpoint InterruptReadEndpoint { get; set; }

	ushort ReadBufferSize { get; }

	ushort WriteBufferSize { get; }

	byte InterfaceNumber { get; }

	Task WriteAsync(byte[] data);

	Task<ReadResult> ReadAsync(uint bufferLength);

	Task ClaimInterface();

	void RegisterDefaultEndpoints();
}
