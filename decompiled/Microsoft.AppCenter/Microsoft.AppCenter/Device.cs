using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter;

public class Device
{
	public string SdkName { get; }

	public string SdkVersion { get; }

	public string Model { get; }

	public string OemName { get; }

	public string OsName { get; }

	public string OsVersion { get; }

	public string OsBuild { get; }

	public int? OsApiLevel { get; }

	public string Locale { get; }

	public int TimeZoneOffset { get; }

	public string ScreenSize { get; }

	public string AppVersion { get; }

	public string CarrierName { get; }

	public string CarrierCountry { get; }

	public string AppBuild { get; }

	public string AppNamespace { get; }

	public Device(Microsoft.AppCenter.Ingestion.Models.Device device)
	{
		SdkName = device.SdkName;
		SdkVersion = device.SdkVersion;
		Model = device.Model;
		OemName = device.OemName;
		OsName = device.OsName;
		OsVersion = device.OsVersion;
		OsBuild = device.OsBuild;
		OsApiLevel = device.OsApiLevel;
		Locale = device.Locale;
		TimeZoneOffset = device.TimeZoneOffset;
		ScreenSize = device.ScreenSize;
		AppVersion = device.AppVersion;
		CarrierName = device.CarrierName;
		CarrierCountry = device.CarrierCountry;
		AppBuild = device.AppBuild;
		AppNamespace = device.AppNamespace;
	}
}
