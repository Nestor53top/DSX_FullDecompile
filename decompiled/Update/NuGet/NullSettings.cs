using System;
using System.Collections.Generic;
using System.Globalization;
using NuGet.Resources;

namespace NuGet;

internal class NullSettings : ISettings
{
	private static readonly NullSettings _settings = new NullSettings();

	public static NullSettings Instance => _settings;

	public string GetValue(string section, string key)
	{
		return string.Empty;
	}

	public string GetValue(string section, string key, bool isPath)
	{
		return string.Empty;
	}

	public IList<KeyValuePair<string, string>> GetValues(string section)
	{
		return new List<KeyValuePair<string, string>>().AsReadOnly();
	}

	public IList<SettingValue> GetSettingValues(string section, bool isPath)
	{
		return new List<SettingValue>().AsReadOnly();
	}

	public IList<KeyValuePair<string, string>> GetNestedValues(string section, string key)
	{
		return new List<KeyValuePair<string, string>>().AsReadOnly();
	}

	public void SetValue(string section, string key, string value)
	{
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidNullSettingsOperation, new object[1] { "SetValue" }));
	}

	public void SetValues(string section, IList<KeyValuePair<string, string>> values)
	{
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidNullSettingsOperation, new object[1] { "SetValues" }));
	}

	public void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
	{
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidNullSettingsOperation, new object[1] { "SetNestedValues" }));
	}

	public bool DeleteValue(string section, string key)
	{
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidNullSettingsOperation, new object[1] { "DeleteValue" }));
	}

	public bool DeleteSection(string section)
	{
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidNullSettingsOperation, new object[1] { "DeleteSection" }));
	}
}
