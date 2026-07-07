namespace Device.Net;

public abstract class DeviceDefinitionBase
{
	public uint? VendorId { get; set; }

	public uint? ProductId { get; set; }

	public string Label { get; set; }

	public DeviceType? DeviceType { get; set; }

	public ushort? UsagePage { get; set; }
}
