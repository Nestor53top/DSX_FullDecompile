using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device.Net;

namespace Usb.Net;

public interface IUsbInterfaceManager : IDeviceHandler, IDisposable
{
	IUsbInterface ReadUsbInterface { get; set; }

	IUsbInterface WriteUsbInterface { get; set; }

	IList<IUsbInterface> UsbInterfaces { get; }

	Task<ConnectedDeviceDefinitionBase> GetConnectedDeviceDefinitionAsync();
}
