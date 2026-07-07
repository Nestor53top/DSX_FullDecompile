namespace DualSenseX;

public class TriggerMode
{
	public string TriggerModeName { get; set; }

	public byte TriggerValue1 { get; set; }

	public byte TriggerValue2 { get; set; }

	public byte TriggerValue3 { get; set; }

	public byte TriggerValue4 { get; set; }

	public byte TriggerValue5 { get; set; }

	public byte TriggerValue6 { get; set; }

	public byte TriggerValue7 { get; set; }

	public byte TriggerValue8 { get; set; }

	public override string ToString()
	{
		return TriggerModeName;
	}
}
