using System;
using System.Collections.Generic;
using System.Linq;
using Device.Net;
using Device.Net.Exceptions;

namespace Usb.Net;

public class UsbInterfaceManager : IDisposable
{
	private bool disposed;

	private IUsbInterface _ReadUsbInterface;

	private IUsbInterface _WriteUsbInterface;

	private IUsbInterface _ReadInterruptUsbInterface;

	private IUsbInterface _WriteInterruptUsbInterface;

	public ITracer Tracer { get; }

	public ILogger Logger { get; }

	public IList<IUsbInterface> UsbInterfaces { get; } = new List<IUsbInterface>();

	public IUsbInterface ReadUsbInterface
	{
		get
		{
			return _ReadUsbInterface;
		}
		set
		{
			if (value != null && !UsbInterfaces.Contains(value))
			{
				throw new ValidationException("The interface is not contained the list of valid interfaces.");
			}
			_ReadUsbInterface = value;
		}
	}

	public IUsbInterface WriteUsbInterface
	{
		get
		{
			return _WriteUsbInterface;
		}
		set
		{
			if (value != null && !UsbInterfaces.Contains(value))
			{
				throw new ValidationException("The interface is not contained the list of valid interfaces.");
			}
			_WriteUsbInterface = value;
		}
	}

	public IUsbInterface ReadInterruptUsbInterface
	{
		get
		{
			return _ReadInterruptUsbInterface;
		}
		set
		{
			if (value != null && !UsbInterfaces.Contains(value))
			{
				throw new ValidationException("The interface is not contained the list of valid interfaces.");
			}
			_ReadInterruptUsbInterface = value;
		}
	}

	public IUsbInterface WriteInterruptUsbInterface
	{
		get
		{
			return _WriteInterruptUsbInterface;
		}
		set
		{
			if (value != null && !UsbInterfaces.Contains(value))
			{
				throw new ValidationException("The interface is not contained the list of valid interfaces.");
			}
			_WriteInterruptUsbInterface = value;
		}
	}

	public UsbInterfaceManager(ILogger logger, ITracer tracer)
	{
		Tracer = tracer;
		Logger = logger;
	}

	public void RegisterDefaultInterfaces()
	{
		foreach (IUsbInterface usbInterface in UsbInterfaces)
		{
			usbInterface.RegisterDefaultEndpoints();
		}
		ReadUsbInterface = UsbInterfaces.FirstOrDefault((IUsbInterface i) => i.ReadEndpoint != null);
		WriteUsbInterface = UsbInterfaces.FirstOrDefault((IUsbInterface i) => i.WriteEndpoint != null);
		ReadInterruptUsbInterface = UsbInterfaces.FirstOrDefault((IUsbInterface i) => i.InterruptReadEndpoint != null);
		WriteInterruptUsbInterface = UsbInterfaces.FirstOrDefault((IUsbInterface i) => i.InterruptWriteEndpoint != null);
	}

	public virtual void Dispose()
	{
		if (disposed)
		{
			return;
		}
		disposed = true;
		foreach (IUsbInterface usbInterface in UsbInterfaces)
		{
			usbInterface.Dispose();
		}
		GC.SuppressFinalize(this);
	}
}
