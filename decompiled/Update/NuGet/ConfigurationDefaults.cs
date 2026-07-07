using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet;

internal class ConfigurationDefaults
{
	private ISettings _settingsManager = NullSettings.Instance;

	private const string ConfigurationDefaultsFile = "NuGetDefaults.config";

	private static readonly ConfigurationDefaults _instance = InitializeInstance();

	private bool _defaultPackageSourceInitialized;

	private List<PackageSource> _defaultPackageSources;

	private string _defaultPushSource;

	public static ConfigurationDefaults Instance => _instance;

	public IEnumerable<PackageSource> DefaultPackageSources
	{
		get
		{
			if (_defaultPackageSources == null)
			{
				_defaultPackageSources = new List<PackageSource>();
				IList<SettingValue> settingValues = _settingsManager.GetSettingValues("disabledPackageSources", isPath: false);
				foreach (SettingValue settingValue in _settingsManager.GetSettingValues("packageSources", isPath: false))
				{
					_defaultPackageSources.Add(new PackageSource(settingValue.Value, settingValue.Key, !settingValues.Any((SettingValue p) => p.Key.Equals(settingValue.Key, StringComparison.CurrentCultureIgnoreCase)), isOfficial: true));
				}
			}
			return _defaultPackageSources;
		}
	}

	public string DefaultPushSource
	{
		get
		{
			if (_defaultPushSource == null && !_defaultPackageSourceInitialized)
			{
				_defaultPackageSourceInitialized = true;
				_defaultPushSource = _settingsManager.GetConfigValue("DefaultPushSource");
			}
			return _defaultPushSource;
		}
	}

	public string DefaultPackageRestoreConsent => _settingsManager.GetValue("packageRestore", "enabled");

	private static ConfigurationDefaults InitializeInstance()
	{
		return new ConfigurationDefaults(new PhysicalFileSystem(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NuGet")), "NuGetDefaults.config");
	}

	internal ConfigurationDefaults(IFileSystem fileSystem, string path)
	{
		try
		{
			if (fileSystem.FileExists(path))
			{
				_settingsManager = new Settings(fileSystem, path);
			}
		}
		catch (FileNotFoundException)
		{
		}
	}
}
