using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NuGet;

internal class PackageSourceProvider : IPackageSourceProvider
{
	private class PackageSourceCredential
	{
		public string Username { get; private set; }

		public string Password { get; private set; }

		public bool IsPasswordClearText { get; private set; }

		public PackageSourceCredential(string username, string password, bool isPasswordClearText)
		{
			Username = username;
			Password = password;
			IsPasswordClearText = isPasswordClearText;
		}
	}

	private const string PackageSourcesSectionName = "packageSources";

	private const string DisabledPackageSourcesSectionName = "disabledPackageSources";

	private const string CredentialsSectionName = "packageSourceCredentials";

	private const string UsernameToken = "Username";

	private const string PasswordToken = "Password";

	private const string ClearTextPasswordToken = "ClearTextPassword";

	private readonly ISettings _settingsManager;

	private readonly IEnumerable<PackageSource> _providerDefaultSources;

	private readonly IDictionary<PackageSource, PackageSource> _migratePackageSources;

	private readonly IEnumerable<PackageSource> _configurationDefaultSources;

	private IEnvironmentVariableReader _environment;

	public event EventHandler PackageSourcesSaved;

	public PackageSourceProvider(ISettings settingsManager)
		: this(settingsManager, null)
	{
	}

	public PackageSourceProvider(ISettings settingsManager, IEnumerable<PackageSource> providerDefaultSources)
		: this(settingsManager, providerDefaultSources, null)
	{
	}

	public PackageSourceProvider(ISettings settingsManager, IEnumerable<PackageSource> providerDefaultSources, IDictionary<PackageSource, PackageSource> migratePackageSources)
		: this(settingsManager, providerDefaultSources, migratePackageSources, ConfigurationDefaults.Instance.DefaultPackageSources, new EnvironmentVariableWrapper())
	{
	}

	internal PackageSourceProvider(ISettings settingsManager, IEnumerable<PackageSource> providerDefaultSources, IDictionary<PackageSource, PackageSource> migratePackageSources, IEnumerable<PackageSource> configurationDefaultSources, IEnvironmentVariableReader environment)
	{
		if (settingsManager == null)
		{
			throw new ArgumentNullException("settingsManager");
		}
		_settingsManager = settingsManager;
		_providerDefaultSources = providerDefaultSources ?? Enumerable.Empty<PackageSource>();
		_migratePackageSources = migratePackageSources;
		_configurationDefaultSources = configurationDefaultSources ?? Enumerable.Empty<PackageSource>();
		_environment = environment;
	}

	public IEnumerable<PackageSource> LoadPackageSources()
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		List<SettingValue> list = new List<SettingValue>();
		IList<SettingValue> settingValues = _settingsManager.GetSettingValues("packageSources", isPath: true);
		int machineWideSourcesCount = 0;
		if (!settingValues.IsEmpty())
		{
			List<SettingValue> list2 = new List<SettingValue>();
			foreach (SettingValue item in settingValues.Reverse())
			{
				if (!hashSet.Contains(item.Key))
				{
					if (item.IsMachineWide)
					{
						list2.Add(item);
					}
					else
					{
						list.Add(item);
					}
					hashSet.Add(item.Key);
				}
			}
			list.Reverse();
			machineWideSourcesCount = list2.Count;
			list.AddRange(list2);
		}
		List<PackageSource> list3 = new List<PackageSource>();
		if (!list.IsEmpty())
		{
			IEnumerable<SettingValue> settingValues2 = _settingsManager.GetSettingValues("disabledPackageSources", isPath: false);
			Dictionary<string, SettingValue> disabledSources = (settingValues2 ?? Enumerable.Empty<SettingValue>()).ToDictionary<SettingValue, string>((SettingValue s) => s.Key, StringComparer.CurrentCultureIgnoreCase);
			list3 = list.Select(delegate(SettingValue p)
			{
				string key = p.Key;
				string value = p.Value;
				PackageSourceCredential packageSourceCredential = ReadCredential(key);
				bool isEnabled = true;
				if (disabledSources.TryGetValue(key, out var value2) && value2.Priority >= p.Priority)
				{
					isEnabled = false;
				}
				return new PackageSource(value, key, isEnabled)
				{
					UserName = packageSourceCredential?.Username,
					Password = packageSourceCredential?.Password,
					IsPasswordClearText = (packageSourceCredential?.IsPasswordClearText ?? false),
					IsMachineWide = p.IsMachineWide
				};
			}).ToList();
			if (_migratePackageSources != null)
			{
				MigrateSources(list3);
			}
		}
		SetDefaultPackageSources(list3, machineWideSourcesCount);
		return list3;
	}

	private PackageSourceCredential ReadCredential(string sourceName)
	{
		PackageSourceCredential packageSourceCredential = ReadCredentialFromEnvironment(sourceName);
		if (packageSourceCredential != null)
		{
			return packageSourceCredential;
		}
		IList<KeyValuePair<string, string>> nestedValues = _settingsManager.GetNestedValues("packageSourceCredentials", sourceName);
		if (!nestedValues.IsEmpty())
		{
			string value = nestedValues.FirstOrDefault((KeyValuePair<string, string> k) => k.Key.Equals("Username", StringComparison.OrdinalIgnoreCase)).Value;
			if (!string.IsNullOrEmpty(value))
			{
				string value2 = nestedValues.FirstOrDefault((KeyValuePair<string, string> k) => k.Key.Equals("Password", StringComparison.OrdinalIgnoreCase)).Value;
				if (!string.IsNullOrEmpty(value2))
				{
					return new PackageSourceCredential(value, EncryptionUtility.DecryptString(value2), isPasswordClearText: false);
				}
				string value3 = nestedValues.FirstOrDefault((KeyValuePair<string, string> k) => k.Key.Equals("ClearTextPassword", StringComparison.Ordinal)).Value;
				if (!string.IsNullOrEmpty(value3))
				{
					return new PackageSourceCredential(value, value3, isPasswordClearText: true);
				}
			}
		}
		return null;
	}

	private PackageSourceCredential ReadCredentialFromEnvironment(string sourceName)
	{
		string environmentVariable = _environment.GetEnvironmentVariable("NuGetPackageSourceCredentials_" + sourceName);
		if (string.IsNullOrEmpty(environmentVariable))
		{
			return null;
		}
		Match match = Regex.Match(environmentVariable.Trim(), "^Username=(?<user>.*?);\\s*Password=(?<pass>.*?)$", RegexOptions.IgnoreCase);
		if (!match.Success)
		{
			return null;
		}
		return new PackageSourceCredential(match.Groups["user"].Value, match.Groups["pass"].Value, isPasswordClearText: true);
	}

	private void MigrateSources(List<PackageSource> loadedPackageSources)
	{
		bool flag = false;
		List<PackageSource> list = new List<PackageSource>();
		for (int i = 0; i < loadedPackageSources.Count; i++)
		{
			PackageSource packageSource = loadedPackageSources[i];
			if (_migratePackageSources.TryGetValue(packageSource, out var targetPackageSource))
			{
				if (loadedPackageSources.Any((PackageSource p) => p.Equals(targetPackageSource)))
				{
					list.Add(loadedPackageSources[i]);
				}
				else
				{
					loadedPackageSources[i] = targetPackageSource.Clone();
					loadedPackageSources[i].IsEnabled = packageSource.IsEnabled;
				}
				flag = true;
			}
		}
		foreach (PackageSource item in list)
		{
			loadedPackageSources.Remove(item);
		}
		if (flag)
		{
			SavePackageSources(loadedPackageSources);
		}
	}

	private void SetDefaultPackageSources(List<PackageSource> loadedPackageSources, int machineWideSourcesCount)
	{
		IEnumerable<PackageSource> enumerable = _configurationDefaultSources;
		if (enumerable.IsEmpty())
		{
			UpdateProviderDefaultSources(loadedPackageSources);
			enumerable = _providerDefaultSources;
		}
		List<PackageSource> list = new List<PackageSource>();
		foreach (PackageSource packageSource in enumerable)
		{
			int num = loadedPackageSources.FindIndex((PackageSource p) => p.Source.Equals(packageSource.Source, StringComparison.OrdinalIgnoreCase));
			if (num != -1)
			{
				if (loadedPackageSources[num].Name.Equals(packageSource.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					loadedPackageSources[num].IsOfficial = true;
				}
				continue;
			}
			int num2 = loadedPackageSources.FindIndex((PackageSource p) => p.Name.Equals(packageSource.Name, StringComparison.CurrentCultureIgnoreCase));
			if (num2 != -1)
			{
				loadedPackageSources[num2] = packageSource;
			}
			else
			{
				list.Add(packageSource);
			}
		}
		loadedPackageSources.InsertRange(loadedPackageSources.Count - machineWideSourcesCount, list);
	}

	private void UpdateProviderDefaultSources(List<PackageSource> loadedSources)
	{
		bool isEnabled = loadedSources.Count == 0 || loadedSources.Where((PackageSource p) => !p.IsMachineWide).Count() == 0;
		foreach (PackageSource providerDefaultSource in _providerDefaultSources)
		{
			providerDefaultSource.IsEnabled = isEnabled;
			providerDefaultSource.IsOfficial = true;
		}
	}

	public void SavePackageSources(IEnumerable<PackageSource> sources)
	{
		_settingsManager.DeleteSection("packageSources");
		_settingsManager.SetValues("packageSources", (from p in sources
			where !p.IsMachineWide && p.IsPersistable
			select new KeyValuePair<string, string>(p.Name, p.Source)).ToList());
		_settingsManager.DeleteSection("disabledPackageSources");
		_settingsManager.SetValues("disabledPackageSources", (from p in sources
			where !p.IsEnabled
			select new KeyValuePair<string, string>(p.Name, "true")).ToList());
		_settingsManager.DeleteSection("packageSourceCredentials");
		foreach (PackageSource item in sources.Where((PackageSource s) => !string.IsNullOrEmpty(s.UserName) && !string.IsNullOrEmpty(s.Password)))
		{
			_settingsManager.SetNestedValues("packageSourceCredentials", item.Name, new KeyValuePair<string, string>[2]
			{
				new KeyValuePair<string, string>("Username", item.UserName),
				ReadPasswordValues(item)
			});
		}
		if (this.PackageSourcesSaved != null)
		{
			this.PackageSourcesSaved(this, EventArgs.Empty);
		}
	}

	private static KeyValuePair<string, string> ReadPasswordValues(PackageSource source)
	{
		string key = (source.IsPasswordClearText ? "ClearTextPassword" : "Password");
		string value = (source.IsPasswordClearText ? source.Password : EncryptionUtility.EncryptString(source.Password));
		return new KeyValuePair<string, string>(key, value);
	}

	public void DisablePackageSource(PackageSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_settingsManager.SetValue("disabledPackageSources", source.Name, "true");
	}

	public bool IsPackageSourceEnabled(PackageSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return string.IsNullOrEmpty(_settingsManager.GetValue("disabledPackageSources", source.Name));
	}
}
