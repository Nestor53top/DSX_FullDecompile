using System;
using System.Threading;

namespace HidSharp.Platform;

internal struct SysRefCountHelper
{
	private int _opened;

	private int _closed;

	private int _refCount;

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

	internal bool HandleRelease()
	{
		if (Interlocked.Decrement(ref _refCount) == 0 && _opened != 0)
		{
			return true;
		}
		return false;
	}

	internal void ThrowIfClosed()
	{
		if (_closed != 0)
		{
			throw ExceptionForClosed();
		}
	}

	private static Exception ExceptionForClosed()
	{
		return CommonException.CreateClosedException();
	}
}
