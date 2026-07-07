namespace Device.Net;

public abstract class ConnectedDeviceDefinitionBase : DeviceDefinitionBase
{
	public string ProductName { get; set; }

	public string Manufacturer { get; set; }

	public string SerialNumber { get; set; }

	public ushort? Usage { get; set; }

	public ushort? VersionNumber { get; set; }

	public int? WriteBufferSize { get; set; }

	public int? ReadBufferSize { get; set; }
}
