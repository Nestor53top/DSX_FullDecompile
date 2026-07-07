using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client.Exceptions;

namespace Nefarius.ViGEm.Client;

internal abstract class ViGEmTarget : IDisposable, IViGEmTarget
{
	private bool disposedValue;

	protected ViGEmClient Client { get; }

	protected IntPtr NativeHandle { get; set; }

	public ushort VendorId { get; protected set; }

	public ushort ProductId { get; protected set; }

	protected ViGEmTarget(ViGEmClient client)
	{
		Client = client;
	}

	public virtual void Connect()
	{
		if (VendorId > 0 && ProductId > 0)
		{
			ViGEmClient.vigem_target_set_vid(NativeHandle, VendorId);
			ViGEmClient.vigem_target_set_pid(NativeHandle, ProductId);
		}
		switch (ViGEmClient.vigem_target_add(Client.NativeHandle, NativeHandle))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_TARGET_UNINITIALIZED:
			throw new VigemTargetUninitializedException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_ALREADY_CONNECTED:
			throw new VigemAlreadyConnectedException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NO_FREE_SLOT:
			throw new VigemNoFreeSlotException();
		default:
			throw new Win32Exception(Marshal.GetLastWin32Error());
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
			break;
		}
	}

	public virtual void Disconnect()
	{
		switch (ViGEmClient.vigem_target_remove(Client.NativeHandle, NativeHandle))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_TARGET_UNINITIALIZED:
			throw new VigemTargetUninitializedException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_TARGET_NOT_PLUGGED_IN:
			throw new VigemTargetNotPluggedInException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_REMOVAL_FAILED:
			throw new VigemRemovalFailedException();
		default:
			throw new Win32Exception(Marshal.GetLastWin32Error());
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
			break;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			try
			{
				Disconnect();
			}
			catch
			{
			}
		}
		ViGEmClient.vigem_target_free(NativeHandle);
		disposedValue = true;
	}

	~ViGEmTarget()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
