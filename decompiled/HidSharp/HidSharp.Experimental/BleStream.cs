using System;
using System.ComponentModel;
using HidSharp.Utility;

namespace HidSharp.Experimental;

public abstract class BleStream : DeviceStream
{
	public new BleDevice Device => (BleDevice)base.Device;

	public BleService Service { get; private set; }

	public BleRequestFlags RequestFlags { get; set; }

	protected BleStream(BleDevice device, BleService service)
		: base(device)
	{
		Throw.If.Null(service).False(service.Device == device);
		Service = service;
		ReadTimeout = 3000;
		WriteTimeout = 3000;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed override void Flush()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed override int Read(byte[] buffer, int offset, int count)
	{
		throw new InvalidOperationException();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed override void Write(byte[] buffer, int offset, int count)
	{
		throw new InvalidOperationException();
	}

	public byte[] ReadCharacteristic(BleCharacteristic characteristic)
	{
		return ReadCharacteristic(characteristic, RequestFlags);
	}

	public abstract byte[] ReadCharacteristic(BleCharacteristic characteristic, BleRequestFlags requestFlags);

	public void WriteCharacteristic(BleCharacteristic characteristic, byte[] value)
	{
		Throw.If.Null(value, "value");
		WriteCharacteristic(characteristic, value, 0, value.Length);
	}

	public void WriteCharacteristic(BleCharacteristic characteristic, byte[] value, int offset, int count)
	{
		WriteCharacteristic(characteristic, value, offset, count, RequestFlags);
	}

	public abstract void WriteCharacteristic(BleCharacteristic characteristic, byte[] value, int offset, int count, BleRequestFlags requestFlags);

	public void WriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value)
	{
		Throw.If.Null(value, "value");
		WriteCharacteristicWithoutResponse(characteristic, value, 0, value.Length);
	}

	public void WriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value, int offset, int count)
	{
		WriteCharacteristicWithoutResponse(characteristic, value, offset, count, RequestFlags);
	}

	public abstract void WriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value, int offset, int count, BleRequestFlags requestFlags);

	public IAsyncResult BeginWriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value, int offset, int count, AsyncCallback callback, object state)
	{
		return BeginWriteCharacteristicWithoutResponse(characteristic, value, offset, count, RequestFlags, callback, state);
	}

	public virtual IAsyncResult BeginWriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value, int offset, int count, BleRequestFlags requestFlags, AsyncCallback callback, object state)
	{
		return AsyncResult<int>.BeginOperation(delegate
		{
			WriteCharacteristicWithoutResponse(characteristic, value, offset, count, requestFlags);
			return 0;
		}, callback, state);
	}

	public virtual void EndWriteCharacteristicWithoutResponse(IAsyncResult asyncResult)
	{
		AsyncResult<int>.EndOperation(asyncResult);
	}

	public byte[] ReadDescriptor(BleDescriptor descriptor)
	{
		return ReadDescriptor(descriptor, RequestFlags);
	}

	public abstract byte[] ReadDescriptor(BleDescriptor descriptor, BleRequestFlags requestFlags);

	public void WriteDescriptor(BleDescriptor descriptor, byte[] value)
	{
		Throw.If.Null(value, "value");
		WriteDescriptor(descriptor, value, 0, value.Length);
	}

	public void WriteDescriptor(BleDescriptor descriptor, byte[] value, int offset, int count)
	{
		WriteDescriptor(descriptor, value, offset, count, RequestFlags);
	}

	public abstract void WriteDescriptor(BleDescriptor descriptor, byte[] value, int offset, int count, BleRequestFlags requestFlags);

	public abstract bool CanReadEventNow();

	public abstract BleEvent ReadEvent();

	public virtual IAsyncResult BeginReadEvent(AsyncCallback callback, object state)
	{
		return AsyncResult<BleEvent>.BeginOperation(() => ReadEvent(), callback, state);
	}

	public virtual BleEvent EndReadEvent(IAsyncResult asyncResult)
	{
		return AsyncResult<BleEvent>.EndOperation(asyncResult);
	}

	public BleCccd ReadCccd(BleCharacteristic characteristic)
	{
		return ReadCccd(characteristic, RequestFlags);
	}

	public BleCccd ReadCccd(BleCharacteristic characteristic, BleRequestFlags requestFlags)
	{
		if (!characteristic.TryGetDescriptor(BleUuids.Cccd, out var descriptor))
		{
			HidSharpDiagnostics.Trace("Characteristic {0} does not have a CCCD, so it could not be read.", characteristic);
			return BleCccd.None;
		}
		byte[] array = ReadDescriptor(descriptor, requestFlags);
		BleCccd result = BleCccd.None;
		if (array.Length >= 1 && array[0] == 1)
		{
			result = BleCccd.Notification;
		}
		if (array.Length >= 1 && array[0] == 2)
		{
			result = BleCccd.Indication;
		}
		return result;
	}

	public void WriteCccd(BleCharacteristic characteristic, BleCccd cccd)
	{
		WriteCccd(characteristic, cccd, RequestFlags);
	}

	public void WriteCccd(BleCharacteristic characteristic, BleCccd cccd, BleRequestFlags requestFlags)
	{
		Throw.If.Null(characteristic, "characteristic");
		if (!characteristic.TryGetDescriptor(BleUuids.Cccd, out var descriptor))
		{
			HidSharpDiagnostics.Trace("Characteristic {0} does not have a CCCD, so {1} could not be written.", characteristic, cccd);
			return;
		}
		if (cccd == BleCccd.Notification)
		{
			HidSharpDiagnostics.PerformStrictCheck(characteristic.IsNotifiable, "Characteristic doesn't support Notify.");
		}
		if (cccd == BleCccd.Indication)
		{
			HidSharpDiagnostics.PerformStrictCheck(characteristic.IsIndicatable, "Characteristic doesn't support Indicate.");
		}
		WriteDescriptor(value: new byte[2]
		{
			(byte)cccd,
			(byte)((int)cccd >> 8)
		}, descriptor: descriptor);
	}
}
