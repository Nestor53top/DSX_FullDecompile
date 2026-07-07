using HidSharp.Experimental;

namespace HidSharp.Platform.Windows;

internal sealed class WinBleDescriptor : BleDescriptor
{
	internal NativeMethods.BTH_LE_GATT_DESCRIPTOR NativeData;

	public override BleUuid Uuid => NativeData.DescriptorUuid.ToGuid();

	public WinBleDescriptor(NativeMethods.BTH_LE_GATT_DESCRIPTOR nativeData)
	{
		NativeData = nativeData;
	}
}
