using System;
using Device.Net;

namespace Usb.Net;

public interface IUsbDevice : IDevice, IDisposable
{
	IUsbInterfaceManager UsbInterfaceManager { get; }
}
