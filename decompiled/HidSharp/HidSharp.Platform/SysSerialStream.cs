using System;
using System.Threading;

namespace HidSharp.Platform;

internal abstract class SysSerialStream : SerialStream
{
	private int _opened;

	private int _closed;

	private int _refCount;

	protected SysSerialStream(SerialDevice device)
		: base(device)
	{
	}

	internal void HandleInitAndOpen()
	{
		_opened = 1;
		_refCount = 1;
	}

	internal bool HandleClose()
	{
		if (Interlocked.CompareExchange(ref _closed, 1, 0) == 0)
		{
			return _opened != 0;
		}
		return false;
	}

	internal bool HandleAcquire()
	{
		int refCount;
		do
		{
			refCount = _refCount;
			if (refCount == 0)
			{
				return false;
			}
		}
		while (refCount != Interlocked.CompareExchange(ref _refCount, refCount + 1, refCount));
		return true;
	}

	internal void HandleAcquireIfOpenOrFail()
	{
		if (_closed != 0 || !HandleAcquire())
		{
			throw ExceptionForClosed();
		}
	}

	internal void HandleRelease()
	{
		if (Interlocked.Decrement(ref _refCount) == 0 && _opened != 0)
		{
			HandleFree();
		}
	}

	private static Exception ExceptionForClosed()
	{
		return CommonException.CreateClosedException();
	}

	internal abstract void HandleFree();
}
