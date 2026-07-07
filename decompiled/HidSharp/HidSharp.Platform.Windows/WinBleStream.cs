using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HidSharp.Experimental;
using HidSharp.Utility;

namespace HidSharp.Platform.Windows;

internal sealed class WinBleStream : SysBleStream
{
	private const int WatchQueueLimit = 100;

	private object _readSync = new object();

	private object _writeSync = new object();

	private object _watchSync = new object();

	private IntPtr _handle;

	private IntPtr _closeEventHandle;

	private IntPtr _watchRegisterEventHandle;

	private IntPtr _watchEventHandle;

	private NativeMethods.BLUETOOTH_GATT_EVENT_CALLBACK _watchCallback;

	private Queue<BleEvent> _watchEvents;

	private Dictionary<ushort, BleCharacteristic> _watchMap;

	public sealed override int ReadTimeout { get; set; }

	public sealed override int WriteTimeout { get; set; }

	internal WinBleStream(WinBleDevice device, WinBleService service)
		: base(device, service)
	{
		_closeEventHandle = NativeMethods.CreateManualResetEventOrThrow();
		_watchEventHandle = NativeMethods.CreateManualResetEventOrThrow();
		_watchEvents = new Queue<BleEvent>();
		_watchMap = new Dictionary<ushort, BleCharacteristic>();
	}

	~WinBleStream()
	{
		Close();
		NativeMethods.CloseHandle(_closeEventHandle);
	}

	internal unsafe void Init(string path)
	{
		IntPtr intPtr = NativeMethods.CreateFileFromDevice(path, NativeMethods.EFileAccess.Read | NativeMethods.EFileAccess.Write, NativeMethods.EFileShare.None);
		if (intPtr == (IntPtr)(-1))
		{
			throw DeviceException.CreateIOException(base.Device, "Unable to open BLE service (" + path + ").");
		}
		_handle = intPtr;
		HandleInitAndOpen();
		List<WinBleCharacteristic> list = new List<WinBleCharacteristic>();
		BleCharacteristic[] characteristics = base.Service.GetCharacteristics();
		foreach (BleCharacteristic bleCharacteristic in characteristics)
		{
			if (bleCharacteristic.IsNotifiable || bleCharacteristic.IsIndicatable)
			{
				list.Add((WinBleCharacteristic)bleCharacteristic);
			}
		}
		if (list.Count > 0)
		{
			byte* ptr = stackalloc byte[(int)(uint)checked(4 + 36 * list.Count)];
			NativeMethods.BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION* ptr2 = (NativeMethods.BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION*)ptr;
			ptr2->NumCharacteristics = (ushort)list.Count;
			ptr += 4;
			for (int j = 0; j < list.Count; j++)
			{
				WinBleCharacteristic winBleCharacteristic = list[j];
				Marshal.StructureToPtr((object)winBleCharacteristic.NativeData, (IntPtr)ptr, false);
				ptr += 36;
				_watchMap[winBleCharacteristic.NativeData.AttributeHandle] = winBleCharacteristic;
			}
			_watchCallback = EventCallback;
			if (NativeMethods.BluetoothGATTRegisterEvent(intPtr, NativeMethods.BTH_LE_GATT_EVENT_TYPE.CharacteristicValueChangedEvent, ptr2, _watchCallback, IntPtr.Zero, out _watchRegisterEventHandle) != 0)
			{
				_watchRegisterEventHandle = IntPtr.Zero;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (HandleClose())
		{
			if (_watchRegisterEventHandle != IntPtr.Zero)
			{
				NativeMethods.BluetoothGATTUnregisterEvent(_watchRegisterEventHandle);
			}
			NativeMethods.SetEvent(_closeEventHandle);
			HandleRelease();
			base.Dispose(disposing);
		}
	}

	internal override void HandleFree()
	{
		NativeMethods.CloseHandle(ref _handle);
		NativeMethods.CloseHandle(ref _closeEventHandle);
		NativeMethods.CloseHandle(ref _watchEventHandle);
	}

	public unsafe override byte[] ReadCharacteristic(BleCharacteristic characteristic, BleRequestFlags requestFlags)
	{
		Throw.If.Null(characteristic, "characteristic");
		HidSharpDiagnostics.PerformStrictCheck(characteristic.IsReadable, "Characteristic doesn't support Read.");
		NativeMethods.BLUETOOTH_GATT_FLAGS gattFlags = GetGattFlags(requestFlags);
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (_readSync)
			{
				WinBleCharacteristic winBleCharacteristic = (WinBleCharacteristic)characteristic;
				int num = NativeMethods.BluetoothGATTGetCharacteristicValue(_handle, ref winBleCharacteristic.NativeData, 0u, null, out var valueSizeRequired, (NativeMethods.BLUETOOTH_GATT_FLAGS)((uint)gattFlags | (uint)(((requestFlags & BleRequestFlags.Cacheable) == 0) ? 4 : 0)));
				if (num != -2147024662 || valueSizeRequired < 4)
				{
					string message = $"Failed to read characteristic {characteristic}.";
					throw DeviceException.CreateIOException(base.Device, message, num);
				}
				byte* ptr = stackalloc byte[(int)valueSizeRequired];
				NativeMethods.BTH_LE_GATT_CHARACTERISTIC_VALUE* ptr2 = (NativeMethods.BTH_LE_GATT_CHARACTERISTIC_VALUE*)ptr;
				num = NativeMethods.BluetoothGATTGetCharacteristicValue(_handle, ref winBleCharacteristic.NativeData, valueSizeRequired, ptr2, out var valueSizeRequired2, gattFlags);
				if (num != 0 || valueSizeRequired != valueSizeRequired2 || ptr2->DataSize > valueSizeRequired - 4)
				{
					string message2 = $"Failed to read characteristic {characteristic}.";
					throw DeviceException.CreateIOException(base.Device, message2, num);
				}
				byte[] array = new byte[ptr2->DataSize];
				Marshal.Copy((IntPtr)ptr2->Data, array, 0, checked((int)ptr2->DataSize));
				return array;
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public override void WriteCharacteristic(BleCharacteristic characteristic, byte[] value, int offset, int count, BleRequestFlags requestFlags)
	{
		Throw.If.Null(characteristic, "characteristic");
		HidSharpDiagnostics.PerformStrictCheck(characteristic.IsWritable, "Characteristic doesn't support Write.");
		NativeMethods.BLUETOOTH_GATT_FLAGS gattFlags = GetGattFlags(requestFlags);
		WriteCharacteristic(characteristic, value, offset, count, gattFlags);
	}

	public override void WriteCharacteristicWithoutResponse(BleCharacteristic characteristic, byte[] value, int offset, int count, BleRequestFlags requestFlags)
	{
		Throw.If.Null(characteristic, "characteristic");
		HidSharpDiagnostics.PerformStrictCheck(characteristic.IsWritableWithoutResponse, "Characteristic doesn't support Write Without Response.");
		NativeMethods.BLUETOOTH_GATT_FLAGS gattFlags = GetGattFlags(requestFlags);
		WriteCharacteristic(characteristic, value, offset, count, gattFlags | NativeMethods.BLUETOOTH_GATT_FLAGS.WRITE_WITHOUT_RESPONSE);
	}

	private unsafe void WriteCharacteristic(BleCharacteristic characteristic, byte[] value, int offset, int count, NativeMethods.BLUETOOTH_GATT_FLAGS flags)
	{
		Throw.If.Null(characteristic, "characteristic").Null(value, "value").OutOfRange(value, offset, count);
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (_writeSync)
			{
				WinBleCharacteristic winBleCharacteristic = (WinBleCharacteristic)characteristic;
				byte* ptr = stackalloc byte[(int)(uint)(4 + count)];
				NativeMethods.BTH_LE_GATT_CHARACTERISTIC_VALUE* ptr2 = (NativeMethods.BTH_LE_GATT_CHARACTERISTIC_VALUE*)ptr;
				ptr2->DataSize = (uint)count;
				Marshal.Copy(value, offset, (IntPtr)ptr2->Data, count);
				int num = NativeMethods.BluetoothGATTSetCharacteristicValue(_handle, ref winBleCharacteristic.NativeData, ptr2, 0uL, flags);
				if (num != 0)
				{
					string message = $"Failed to write {count} bytes to characteristic {characteristic}.";
					throw DeviceException.CreateIOException(base.Device, message, num);
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public unsafe override byte[] ReadDescriptor(BleDescriptor descriptor, BleRequestFlags requestFlags)
	{
		Throw.If.Null(descriptor, "descriptor");
		NativeMethods.BLUETOOTH_GATT_FLAGS gattFlags = GetGattFlags(requestFlags);
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (_readSync)
			{
				WinBleDescriptor winBleDescriptor = (WinBleDescriptor)descriptor;
				int num = NativeMethods.BluetoothGATTGetDescriptorValue(_handle, ref winBleDescriptor.NativeData, 0u, null, out var valueSizeRequired, (NativeMethods.BLUETOOTH_GATT_FLAGS)((uint)gattFlags | (uint)(((requestFlags & BleRequestFlags.Cacheable) == 0) ? 4 : 0)));
				if (num != -2147024662 || valueSizeRequired < 76)
				{
					string message = $"Failed to read descriptor {descriptor}.";
					throw DeviceException.CreateIOException(base.Device, message, num);
				}
				byte* ptr = stackalloc byte[(int)valueSizeRequired];
				NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE* ptr2 = (NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE*)ptr;
				num = NativeMethods.BluetoothGATTGetDescriptorValue(_handle, ref winBleDescriptor.NativeData, valueSizeRequired, ptr2, out var valueSizeRequired2, gattFlags);
				if (num != 0 || valueSizeRequired != valueSizeRequired2 || ptr2->Value.DataSize > valueSizeRequired - 76)
				{
					string message2 = $"Failed to read descriptor {descriptor}.";
					throw DeviceException.CreateIOException(base.Device, message2, num);
				}
				byte[] array;
				switch (ptr2->DescriptorType)
				{
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.CharacteristicExtendedProperties:
					array = new byte[2];
					if (ptr2->Params.ExtendedProperties.IsReliableWriteEnabled != 0)
					{
						array[0] |= 1;
					}
					if (ptr2->Params.ExtendedProperties.IsAuxiliariesWritable != 0)
					{
						array[0] |= 2;
					}
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.ClientCharacteristicConfiguration:
					array = new byte[2];
					if (ptr2->Params.Cccd.IsSubscribeToNotification != 0)
					{
						array[0] |= 1;
					}
					if (ptr2->Params.Cccd.IsSubscribeToIndication != 0)
					{
						array[0] |= 2;
					}
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.ServerCharacteristicConfiguration:
					array = new byte[2];
					if (ptr2->Params.Sccd.IsBroadcast != 0)
					{
						array[0] |= 1;
					}
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.CharacteristicFormat:
					throw new NotImplementedException();
				default:
					array = new byte[ptr2->Value.DataSize];
					Marshal.Copy((IntPtr)ptr2->Value.Data, array, 0, array.Length);
					break;
				}
				return array;
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public unsafe override void WriteDescriptor(BleDescriptor descriptor, byte[] value, int offset, int count, BleRequestFlags requestFlags)
	{
		Throw.If.Null(descriptor, "descriptor").Null(value, "value").OutOfRange(value, offset, count);
		NativeMethods.BLUETOOTH_GATT_FLAGS gattFlags = GetGattFlags(requestFlags);
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (_writeSync)
			{
				WinBleDescriptor winBleDescriptor = (WinBleDescriptor)descriptor;
				NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS = default(NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS);
				byte[] array = null;
				int startIndex = 0;
				int num = 0;
				switch (winBleDescriptor.NativeData.DescriptorType)
				{
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.CharacteristicExtendedProperties:
					bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS.ExtendedProperties.IsReliableWriteEnabled = (byte)((value.Length >= 1 && (value[offset] & 1) != 0) ? 1u : 0u);
					bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS.ExtendedProperties.IsAuxiliariesWritable = (byte)((value.Length >= 1 && (value[offset] & 2) != 0) ? 1u : 0u);
					array = new byte[0];
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.ClientCharacteristicConfiguration:
					bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS.Cccd.IsSubscribeToNotification = (byte)((value.Length >= 1 && (value[offset] & 1) != 0) ? 1u : 0u);
					bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS.Cccd.IsSubscribeToIndication = (byte)((value.Length >= 1 && (value[offset] & 2) != 0) ? 1u : 0u);
					array = new byte[0];
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.ServerCharacteristicConfiguration:
					bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS.Sccd.IsBroadcast = (byte)((value.Length >= 1 && (value[offset] & 1) != 0) ? 1u : 0u);
					array = new byte[0];
					break;
				case NativeMethods.BTH_LE_GATT_DESCRIPTOR_TYPE.CharacteristicFormat:
					throw new NotImplementedException();
				default:
					array = value;
					startIndex = offset;
					num = count;
					break;
				}
				byte* ptr = stackalloc byte[(int)(uint)(76 + num)];
				NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE* ptr2 = (NativeMethods.BTH_LE_GATT_DESCRIPTOR_VALUE*)ptr;
				ptr2->DescriptorType = winBleDescriptor.NativeData.DescriptorType;
				ptr2->DescriptorUuid = winBleDescriptor.NativeData.DescriptorUuid;
				ptr2->Params = bTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS;
				ptr2->Value.DataSize = (uint)num;
				if (array != null)
				{
					Marshal.Copy(array, startIndex, (IntPtr)ptr2->Value.Data, num);
				}
				int num2 = NativeMethods.BluetoothGATTSetDescriptorValue(_handle, ref winBleDescriptor.NativeData, ptr2, gattFlags);
				if (num2 != 0)
				{
					string message = $"Failed to write {count} bytes to descriptor {descriptor}.";
					throw DeviceException.CreateIOException(base.Device, message, num2);
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	private unsafe void EventCallback(NativeMethods.BTH_LE_GATT_EVENT_TYPE eventType, NativeMethods.BLUETOOTH_GATT_VALUE_CHANGED_EVENT* eventParameter, IntPtr context)
	{
		if (eventType != NativeMethods.BTH_LE_GATT_EVENT_TYPE.CharacteristicValueChangedEvent || !_watchMap.TryGetValue(eventParameter->ChangedAttributeHandle, out var value) || !(eventParameter->CharacteristicValueDataSize == (UIntPtr)eventParameter->CharacteristicValue->DataSize))
		{
			return;
		}
		byte[] array = new byte[eventParameter->CharacteristicValue->DataSize];
		Marshal.Copy((IntPtr)eventParameter->CharacteristicValue->Data, array, 0, array.Length);
		BleEvent item = new BleEvent(value, array);
		if (!HandleAcquire())
		{
			return;
		}
		try
		{
			lock (_watchSync)
			{
				while (_watchEvents.Count >= 100)
				{
					_watchEvents.Dequeue();
				}
				_watchEvents.Enqueue(item);
				NativeMethods.SetEvent(_watchEventHandle);
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	public override bool CanReadEventNow()
	{
		lock (_watchSync)
		{
			return _watchEvents.Count > 0;
		}
	}

	public unsafe override BleEvent ReadEvent()
	{
		HandleAcquireIfOpenOrFail();
		try
		{
			while (true)
			{
				IntPtr* ptr = stackalloc IntPtr[2];
				*ptr = _watchEventHandle;
				ptr[1] = _closeEventHandle;
				switch (NativeMethods.WaitForMultipleObjects(2u, ptr, waitAll: false, NativeMethods.WaitForMultipleObjectsGetTimeout(ReadTimeout)))
				{
				case 1u:
					throw CommonException.CreateClosedException();
				default:
					throw new TimeoutException();
				case 0u:
					lock (_watchSync)
					{
						if (_watchEvents.Count == 0)
						{
							NativeMethods.ResetEvent(_watchEventHandle);
							break;
						}
						BleEvent result = _watchEvents.Dequeue();
						if (_watchEvents.Count == 0)
						{
							NativeMethods.ResetEvent(_watchEventHandle);
						}
						return result;
					}
				}
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	private static NativeMethods.BLUETOOTH_GATT_FLAGS GetGattFlags(BleRequestFlags requestFlags)
	{
		NativeMethods.BLUETOOTH_GATT_FLAGS bLUETOOTH_GATT_FLAGS = (NativeMethods.BLUETOOTH_GATT_FLAGS)0u;
		if ((requestFlags & BleRequestFlags.Authenticated) != BleRequestFlags.None)
		{
			bLUETOOTH_GATT_FLAGS |= NativeMethods.BLUETOOTH_GATT_FLAGS.AUTHENTICATED;
		}
		if ((requestFlags & BleRequestFlags.Encrypted) != BleRequestFlags.None)
		{
			bLUETOOTH_GATT_FLAGS |= NativeMethods.BLUETOOTH_GATT_FLAGS.ENCRYPTED;
		}
		return bLUETOOTH_GATT_FLAGS;
	}
}
