using System;
using System.Collections.Generic;
using HidSharp.Experimental;

namespace HidSharp.Platform.Windows;

internal sealed class WinBleService : BleService
{
	internal NativeMethods.BTH_LE_GATT_SERVICE NativeData;

	private WinBleCharacteristic[] _characteristics;

	private WinBleDevice _device;

	private object _syncObject;

	public override BleDevice Device => _device;

	public override BleUuid Uuid => NativeData.ServiceUuid.ToGuid();

	public WinBleService(WinBleDevice device, NativeMethods.BTH_LE_GATT_SERVICE nativeData)
	{
		_device = device;
		NativeData = nativeData;
		_syncObject = new object();
	}

	public override BleCharacteristic[] GetCharacteristics()
	{
		lock (_syncObject)
		{
			if (_characteristics == null && !_device.TryOpenToGetInfo(delegate(IntPtr handle)
			{
				NativeMethods.BTH_LE_GATT_CHARACTERISTIC[] array = NativeMethods.BluetoothGATTGetCharacteristics(handle, ref NativeData);
				if (array == null)
				{
					return false;
				}
				List<WinBleCharacteristic> list = new List<WinBleCharacteristic>();
				NativeMethods.BTH_LE_GATT_CHARACTERISTIC[] array2 = array;
				foreach (NativeMethods.BTH_LE_GATT_CHARACTERISTIC nativeData in array2)
				{
					WinBleCharacteristic winBleCharacteristic = new WinBleCharacteristic(nativeData);
					list.Add(winBleCharacteristic);
					NativeMethods.BTH_LE_GATT_DESCRIPTOR[] array3 = NativeMethods.BluetoothGATTGetDescriptors(handle, ref winBleCharacteristic.NativeData);
					if (array3 == null)
					{
						return false;
					}
					List<WinBleDescriptor> list2 = new List<WinBleDescriptor>();
					NativeMethods.BTH_LE_GATT_DESCRIPTOR[] array4 = array3;
					foreach (NativeMethods.BTH_LE_GATT_DESCRIPTOR nativeData2 in array4)
					{
						WinBleDescriptor item = new WinBleDescriptor(nativeData2);
						list2.Add(item);
					}
					winBleCharacteristic._characteristicDescriptors = list2.ToArray();
				}
				_characteristics = list.ToArray();
				return true;
			}))
			{
				throw DeviceException.CreateIOException(_device, "BLE service information could not be retrieved.");
			}
		}
		return (BleCharacteristic[])_characteristics.Clone();
	}
}
