namespace HidSharp.Experimental;

public abstract class BleService
{
	public abstract BleDevice Device { get; }

	public abstract BleUuid Uuid { get; }

	public override string ToString()
	{
		return Uuid.ToString();
	}

	public abstract BleCharacteristic[] GetCharacteristics();

	public BleCharacteristic GetCharacteristicOrNull(BleUuid uuid)
	{
		if (!TryGetCharacteristic(uuid, out var characteristic))
		{
			return null;
		}
		return characteristic;
	}

	public virtual bool HasCharacteristic(BleUuid uuid)
	{
		BleCharacteristic characteristic;
		return TryGetCharacteristic(uuid, out characteristic);
	}

	public virtual bool TryGetCharacteristic(BleUuid uuid, out BleCharacteristic characteristic)
	{
		BleCharacteristic[] characteristics = GetCharacteristics();
		foreach (BleCharacteristic bleCharacteristic in characteristics)
		{
			if (bleCharacteristic.Uuid == uuid)
			{
				characteristic = bleCharacteristic;
				return true;
			}
		}
		characteristic = null;
		return false;
	}
}
