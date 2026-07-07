using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

public class Device
{
	[JsonProperty(PropertyName = "sdkName")]
	public string SdkName { get; set; }

	[JsonProperty(PropertyName = "sdkVersion")]
	public string SdkVersion { get; set; }

	[JsonProperty(PropertyName = "wrapperSdkVersion")]
	public string WrapperSdkVersion { get; set; }

	[JsonProperty(PropertyName = "wrapperSdkName")]
	public string WrapperSdkName { get; set; }

	[JsonProperty(PropertyName = "model")]
	public string Model { get; set; }

	[JsonProperty(PropertyName = "oemName")]
	public string OemName { get; set; }

	[JsonProperty(PropertyName = "osName")]
	public string OsName { get; set; }

	[JsonProperty(PropertyName = "osVersion")]
	public string OsVersion { get; set; }

	[JsonProperty(PropertyName = "osBuild")]
	public string OsBuild { get; set; }

	[JsonProperty(PropertyName = "osApiLevel")]
	public int? OsApiLevel { get; set; }

	[JsonProperty(PropertyName = "locale")]
	public string Locale { get; set; }

	[JsonProperty(PropertyName = "timeZoneOffset")]
	public int TimeZoneOffset { get; set; }

	[JsonProperty(PropertyName = "screenSize")]
	public string ScreenSize { get; set; }

	[JsonProperty(PropertyName = "appVersion")]
	public string AppVersion { get; set; }

	[JsonProperty(PropertyName = "carrierName")]
	public string CarrierName { get; set; }

	[JsonProperty(PropertyName = "carrierCountry")]
	public string CarrierCountry { get; set; }

	[JsonProperty(PropertyName = "appBuild")]
	public string AppBuild { get; set; }

	[JsonProperty(PropertyName = "appNamespace")]
	public string AppNamespace { get; set; }

	[JsonProperty(PropertyName = "liveUpdateReleaseLabel")]
	public string LiveUpdateReleaseLabel { get; set; }

	[JsonProperty(PropertyName = "liveUpdateDeploymentKey")]
	public string LiveUpdateDeploymentKey { get; set; }

	[JsonProperty(PropertyName = "liveUpdatePackageHash")]
	public string LiveUpdatePackageHash { get; set; }

	[JsonProperty(PropertyName = "wrapperRuntimeVersion")]
	public string WrapperRuntimeVersion { get; set; }

	public Device()
	{
	}

	public Device(string sdkName, string sdkVersion, string osName, string osVersion, string locale, int timeZoneOffset, string appVersion, string appBuild, string wrapperSdkVersion = null, string wrapperSdkName = null, string model = null, string oemName = null, string osBuild = null, int? osApiLevel = null, string screenSize = null, string carrierName = null, string carrierCountry = null, string appNamespace = null, string liveUpdateReleaseLabel = null, string liveUpdateDeploymentKey = null, string liveUpdatePackageHash = null, string wrapperRuntimeVersion = null)
	{
		SdkName = sdkName;
		SdkVersion = sdkVersion;
		WrapperSdkVersion = wrapperSdkVersion;
		WrapperSdkName = wrapperSdkName;
		Model = model;
		OemName = oemName;
		OsName = osName;
		OsVersion = osVersion;
		OsBuild = osBuild;
		OsApiLevel = osApiLevel;
		Locale = locale;
		TimeZoneOffset = timeZoneOffset;
		ScreenSize = screenSize;
		AppVersion = appVersion;
		CarrierName = carrierName;
		CarrierCountry = carrierCountry;
		AppBuild = appBuild;
		AppNamespace = appNamespace;
		LiveUpdateReleaseLabel = liveUpdateReleaseLabel;
		LiveUpdateDeploymentKey = liveUpdateDeploymentKey;
		LiveUpdatePackageHash = liveUpdatePackageHash;
		WrapperRuntimeVersion = wrapperRuntimeVersion;
	}

	public virtual void Validate()
	{
		if (SdkName == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "SdkName");
		}
		if (SdkVersion == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "SdkVersion");
		}
		if (OsName == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "OsName");
		}
		if (OsVersion == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "OsVersion");
		}
		if (Locale == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Locale");
		}
		if (AppVersion == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "AppVersion");
		}
		if (AppBuild == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "AppBuild");
		}
		if (TimeZoneOffset > 840)
		{
			throw new ValidationException(ValidationException.Rule.InclusiveMaximum, "TimeZoneOffset", 840);
		}
		if (TimeZoneOffset < -840)
		{
			throw new ValidationException(ValidationException.Rule.InclusiveMinimum, "TimeZoneOffset", -840);
		}
	}
}
