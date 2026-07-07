using System;
using System.IO;
using HidSharp.Utility;

namespace HidSharp;

public abstract class DeviceStream : Stream
{
	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override bool CanTimeout => true;

	public Device Device { get; private set; }

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public abstract override int ReadTimeout { get; set; }

	public abstract override int WriteTimeout { get; set; }

	public object Tag { get; set; }

	public event EventHandler Closed;

	public event EventHandler InterruptRequested;

	protected DeviceStream(Device device)
	{
		Throw.If.Null(device);
		Device = device;
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			OnClosed();
		}
		catch (Exception arg)
		{
			HidSharpDiagnostics.Trace("OnClosed threw an exception: {0}", arg);
		}
		base.Dispose(disposing);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		return AsyncResult<int>.BeginOperation(() => Read(buffer, offset, count), callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return AsyncResult<int>.EndOperation(asyncResult);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		return AsyncResult<int>.BeginOperation(delegate
		{
			Write(buffer, offset, count);
			return 0;
		}, callback, state);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		AsyncResult<int>.EndOperation(asyncResult);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	protected virtual void OnClosed()
	{
		RaiseClosed();
	}

	protected void RaiseClosed()
	{
		this.Closed?.Invoke(this, EventArgs.Empty);
	}

	protected internal virtual void OnInterruptRequested()
	{
		RaiseInterruptRequested();
	}

	protected void RaiseInterruptRequested()
	{
		this.InterruptRequested?.Invoke(this, EventArgs.Empty);
	}
}
