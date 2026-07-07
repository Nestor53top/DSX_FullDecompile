using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Exceptions;

namespace Usb.Net;

public abstract class UsbInterfaceBase
{
	private IUsbInterfaceEndpoint _ReadEndpoint;

	private IUsbInterfaceEndpoint _WriteEndpoint;

	private IUsbInterfaceEndpoint _WriteInterruptEndpoint;

	private IUsbInterfaceEndpoint _ReadInterruptEndpoint;

	private readonly ushort? _ReadBufferSize;

	private readonly ushort? _WriteBufferSize;

	public abstract byte InterfaceNumber { get; }

	public ILogger Logger { get; }

	public ITracer Tracer { get; }

	public ushort ReadBufferSize
	{
		get
		{
			if (_ReadBufferSize.HasValue)
			{
				return _ReadBufferSize.Value;
			}
			if (ReadEndpoint != null)
			{
				return ReadEndpoint.MaxPacketSize;
			}
			throw new NotImplementedException();
		}
	}

	public ushort WriteBufferSize
	{
		get
		{
			if (_WriteBufferSize.HasValue)
			{
				return _WriteBufferSize.Value;
			}
			if (WriteEndpoint != null)
			{
				return WriteEndpoint.MaxPacketSize;
			}
			throw new NotImplementedException();
		}
	}

	public IList<IUsbInterfaceEndpoint> UsbInterfaceEndpoints { get; } = new List<IUsbInterfaceEndpoint>();

	public IUsbInterfaceEndpoint ReadEndpoint
	{
		get
		{
			return _ReadEndpoint ?? (_ReadEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint p) => p.IsRead && !p.IsInterrupt));
		}
		set
		{
			if (value != null && !UsbInterfaceEndpoints.Contains(value))
			{
				throw new ValidationException("This endpoint is not contained in the list of valid endpoints");
			}
			_ReadEndpoint = value;
		}
	}

	public IUsbInterfaceEndpoint WriteEndpoint
	{
		get
		{
			return _WriteEndpoint ?? (_WriteEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint p) => p.IsWrite && !p.IsInterrupt));
		}
		set
		{
			if (value != null && !UsbInterfaceEndpoints.Contains(value))
			{
				throw new ValidationException("This endpoint is not contained in the list of valid endpoints");
			}
			_WriteEndpoint = value;
		}
	}

	public IUsbInterfaceEndpoint InterruptWriteEndpoint
	{
		get
		{
			return _WriteInterruptEndpoint ?? (_WriteInterruptEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint p) => p.IsInterrupt && p.IsWrite));
		}
		set
		{
			if (value != null && !UsbInterfaceEndpoints.Contains(value))
			{
				throw new ValidationException("This endpoint is not contained in the list of valid endpoints");
			}
			_WriteInterruptEndpoint = value;
		}
	}

	public IUsbInterfaceEndpoint InterruptReadEndpoint
	{
		get
		{
			return _ReadInterruptEndpoint ?? (_ReadInterruptEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint p) => p.IsInterrupt && p.IsRead));
		}
		set
		{
			if (value != null && !UsbInterfaceEndpoints.Contains(value))
			{
				throw new ValidationException("This endpoint is not contained in the list of valid endpoints");
			}
			_ReadInterruptEndpoint = value;
		}
	}

	public void RegisterDefaultEndpoints()
	{
		ReadEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint e) => e.IsRead && !e.IsInterrupt);
		WriteEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint e) => e.IsWrite && !e.IsInterrupt);
		InterruptReadEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint e) => e.IsRead && e.IsInterrupt);
		InterruptWriteEndpoint = UsbInterfaceEndpoints.FirstOrDefault((IUsbInterfaceEndpoint e) => e.IsWrite && e.IsInterrupt);
		if (ReadEndpoint == null && InterruptReadEndpoint != null)
		{
			ReadEndpoint = InterruptReadEndpoint;
			Logger.Log(Messages.GetErrorMessageNoBulkPipe(InterfaceNumber, isRead: true), "UsbInterfaceBase", null, LogLevel.Warning);
		}
		if (WriteEndpoint == null && InterruptWriteEndpoint != null)
		{
			WriteEndpoint = InterruptWriteEndpoint;
			Logger.Log(Messages.GetErrorMessageNoBulkPipe(InterfaceNumber, isRead: false), "UsbInterfaceBase", null, LogLevel.Warning);
		}
	}

	public virtual async Task ClaimInterface()
	{
	}

	protected UsbInterfaceBase(ILogger logger, ITracer tracer, ushort? readBufferSize, ushort? writeBufferSize)
	{
		Tracer = tracer;
		Logger = logger;
		_ReadBufferSize = readBufferSize;
		_WriteBufferSize = writeBufferSize;
	}
}
