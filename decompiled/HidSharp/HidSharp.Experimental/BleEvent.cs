namespace HidSharp.Experimental;

public struct BleEvent(BleCharacteristic characteristic, byte[] value)
{
	private BleCharacteristic _characteristic = characteristic;

	private byte[] _value = value;

	public BleCharacteristic Characteristic => _characteristic;

	public byte[] Value => _value;
}
