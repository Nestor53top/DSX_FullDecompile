using System;
using System.Globalization;

namespace NuGet;

internal class PackageRestoreConsent
{
	private const string EnvironmentVariableName = "EnableNuGetPackageRestore";

	private const string PackageRestoreSection = "packageRestore";

	private const string PackageRestoreConsentKey = "enabled";

	private const string PackageRestoreAutomaticKey = "automatic";

	private readonly ISettings _settings;

	private readonly IEnvironmentVariableReader _environmentReader;

	private readonly ConfigurationDefaults _configurationDefaults;

	public bool IsGranted
	{
		get
		{
			string value = _environmentReader.GetEnvironmentVariable("EnableNuGetPackageRestore").SafeTrim();
			if (!IsGrantedInSettings)
			{
				return IsSet(value);
			}
			return true;
		}
	}

	public bool IsGrantedInSettings
	{
		get
		{
			string value = _settings.GetValue("packageRestore", "enabled");
			if (string.IsNullOrWhiteSpace(value))
			{
				value = _configurationDefaults.DefaultPackageRestoreConsent;
			}
			value = value.SafeTrim();
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			return IsSet(value);
		}
		set
		{
			_settings.SetValue("packageRestore", "enabled", value.ToString());
		}
	}

	public bool IsAutomatic
	{
		get
		{
			string value = _settings.GetValue("packageRestore", "automatic");
			if (string.IsNullOrWhiteSpace(value))
			{
				return IsGrantedInSettings;
			}
			value = value.SafeTrim();
			return IsSet(value);
		}
		set
		{
			_settings.SetValue("packageRestore", "automatic", value.ToString());
		}
	}

	public PackageRestoreConsent(ISettings settings)
		: this(settings, new EnvironmentVariableWrapper())
	{
	}

	public PackageRestoreConsent(ISettings settings, IEnvironmentVariableReader environmentReader)
		: this(settings, environmentReader, ConfigurationDefaults.Instance)
	{
	}

	public PackageRestoreConsent(ISettings settings, IEnvironmentVariableReader environmentReader, ConfigurationDefaults configurationDefaults)
	{
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		if (environmentReader == null)
		{
			throw new ArgumentNullException("environmentReader");
		}
		if (configurationDefaults == null)
		{
			throw new ArgumentNullException("configurationDefaults");
		}
		_settings = settings;
		_environmentReader = environmentReader;
		_configurationDefaults = configurationDefaults;
	}

	private static bool IsSet(string value)
	{
		if (!(bool.TryParse(value, out var result) && result))
		{
			if (int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result2))
			{
				return result2 == 1;
			}
			return false;
		}
		return true;
	}
}
