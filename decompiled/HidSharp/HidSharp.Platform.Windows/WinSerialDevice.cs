namespace HidSharp.Platform.Windows;

internal sealed class WinSerialDevice : SerialDevice
{
	private string _path;

	private string _fileSystemName;

	private string _friendlyName;

	public override string DevicePath => _path;

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		WinSerialStream winSerialStream = new WinSerialStream(this);
		winSerialStream.Init(DevicePath);
		return winSerialStream;
	}

	internal static WinSerialDevice TryCreate(string portName, string fileSystemName, string friendlyName)
	{
		WinSerialDevice winSerialDevice = new WinSerialDevice();
		winSerialDevice._path = portName;
		winSerialDevice._fileSystemName = fileSystemName;
		winSerialDevice._friendlyName = friendlyName;
		return winSerialDevice;
	}

	public override string GetFileSystemName()
	{
		return _fileSystemName;
	}

	public override string GetFriendlyName()
	{
		return _friendlyName;
	}
}
