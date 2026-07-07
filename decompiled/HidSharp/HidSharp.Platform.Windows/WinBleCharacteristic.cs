using HidSharp.Experimental;

namespace HidSharp.Platform.Windows;

internal sealed class WinBleCharacteristic : BleCharacteristic
{
	internal NativeMethods.BTH_LE_GATT_CHARACTERISTIC NativeData;

	internal WinBleDescriptor[] _characteristicDescriptors;

	private BleCharacteristicProperties _properties;

	public override BleUuid Uuid => NativeData.CharacteristicUuid.ToGuid();

	public override BleCharacteristicProperties Properties => _properties;

	public WinBleCharacteristic(NativeMethods.BTH_LE_GATT_CHARACTERISTIC nativeData)
	{
		NativeData = nativeData;
		_properties = (BleCharacteristicProperties)((byte)((byte)((byte)((byte)((byte)((byte)(((nativeData.IsBroadcastable != 0) ? 1 : 0) | ((nativeData.IsReadable != 0) ? 2 : 0)) | ((nativeData.IsWritableWithoutResponse != 0) ? 4 : 0)) | ((nativeData.IsWritable != 0) ? 8 : 0)) | ((nativeData.IsNotifiable != 0) ? 16 : 0)) | ((nativeData.IsIndicatable != 0) ? 32 : 0)) | ((nativeData.IsSignedWritable != 0) ? 64 : 0)) | ((nativeData.HasExtendedProperties != 0) ? 128 : 0));
	}

	public override BleDescriptor[] GetDescriptors()
	{
		return (BleDescriptor[])_characteristicDescriptors.Clone();
	}
}
