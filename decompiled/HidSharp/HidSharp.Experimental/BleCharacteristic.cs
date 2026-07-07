namespace HidSharp.Experimental;

public abstract class BleCharacteristic
{
	public abstract BleUuid Uuid { get; }

	public abstract BleCharacteristicProperties Properties { get; }

	public bool IsReadable => (Properties & BleCharacteristicProperties.Read) != 0;

	public bool IsWritable => (Properties & BleCharacteristicProperties.Write) != 0;

	public bool IsWritableWithoutResponse => (Properties & BleCharacteristicProperties.WriteWithoutResponse) != 0;

	public bool IsNotifiable => (Properties & BleCharacteristicProperties.Notify) != 0;

	public bool IsIndicatable => (Properties & BleCharacteristicProperties.Indicate) != 0;

	public override string ToString()
	{
		return $"{Uuid} (properties: {Properties})";
	}

	public abstract BleDescriptor[] GetDescriptors();

	public bool HasDescriptor(BleUuid uuid)
	{
		BleDescriptor descriptor;
		return TryGetDescriptor(uuid, out descriptor);
	}

	public BleDescriptor GetDescriptorOrNull(BleUuid uuid)
	{
		if (!TryGetDescriptor(uuid, out var descriptor))
		{
			return null;
		}
		return descriptor;
	}

	public virtual bool TryGetDescriptor(BleUuid uuid, out BleDescriptor descriptor)
	{
		BleDescriptor[] descriptors = GetDescriptors();
		foreach (BleDescriptor bleDescriptor in descriptors)
		{
			if (bleDescriptor.Uuid == uuid)
			{
				descriptor = bleDescriptor;
				return true;
			}
		}
		descriptor = null;
		return false;
	}
}
