namespace HidSharp.Experimental;

public abstract class BleDescriptor
{
	public abstract BleUuid Uuid { get; }

	public override string ToString()
	{
		return Uuid.ToString();
	}
}
