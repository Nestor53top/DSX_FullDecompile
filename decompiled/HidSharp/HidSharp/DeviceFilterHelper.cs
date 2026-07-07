namespace HidSharp;

public static class DeviceFilterHelper
{
	public static bool MatchHidDevices(Device device, int? vendorID = null, int? productID = null, int? releaseNumberBcd = null, string serialNumber = null)
	{
		if (device is HidDevice hidDevice)
		{
			int num = vendorID ?? (-1);
			int num2 = productID ?? (-1);
			int num3 = releaseNumberBcd ?? (-1);
			if ((num < 0 || hidDevice.VendorID == vendorID) && (num2 < 0 || hidDevice.ProductID == productID) && (num3 < 0 || hidDevice.ReleaseNumberBcd == releaseNumberBcd))
			{
				try
				{
					if (string.IsNullOrEmpty(serialNumber) || hidDevice.GetSerialNumber() == serialNumber)
					{
						return true;
					}
				}
				catch
				{
				}
			}
		}
		return false;
	}

	public static bool MatchSerialDevices(Device device, string portName = null)
	{
		if (device is SerialDevice serialDevice && (string.IsNullOrEmpty(portName) || serialDevice.DevicePath == portName))
		{
			return true;
		}
		return false;
	}
}
