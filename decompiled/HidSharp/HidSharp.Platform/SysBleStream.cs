using HidSharp.Experimental;

namespace HidSharp.Platform;

internal abstract class SysBleStream : BleStream
{
	private SysRefCountHelper _rch;

	internal SysBleStream(BleDevice device, BleService service)
		: base(device, service)
	{
	}

	internal void HandleInitAndOpen()
	{
		_rch.HandleInitAndOpen();
	}

	internal bool HandleClose()
	{
		return _rch.HandleClose();
	}

	internal bool HandleAcquire()
	{
		return _rch.HandleAcquire();
	}

	internal void HandleAcquireIfOpenOrFail()
	{
		_rch.HandleAcquireIfOpenOrFail();
	}

	internal void HandleRelease()
	{
		if (_rch.HandleRelease())
		{
			HandleFree();
		}
	}

	internal abstract void HandleFree();
}
