using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.AppCenter.Utils;

public class DefaultApplicationSettings : IApplicationSettings
{
	private const string FileName = "AppCenter.config";

	private const string CorruptedConfigurationWarning = "Configuration is corrupted. App Center could work incorrectly.";

	private static readonly object configLock = new object();

	private static Configuration configuration;

	internal static string FilePath { get; private set; }

	public DefaultApplicationSettings()
	{
		lock (configLock)
		{
			try
			{
				configuration = OpenConfiguration();
			}
			catch (Exception exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Configuration could be corrupted.", exception);
				RestoreConfigurationFile();
			}
		}
	}

	public T GetValue<T>(string key, T defaultValue = default(T))
	{
		lock (configLock)
		{
			if (configuration != null)
			{
				KeyValueConfigurationElement val = configuration.AppSettings.Settings[key];
				if (val != null)
				{
					return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(val.Value);
				}
			}
			else
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Configuration is corrupted. App Center could work incorrectly. Method 'GetValue', key:" + key);
			}
		}
		return defaultValue;
	}

	public void SetValue(string key, object value)
	{
		string value2 = ((value != null) ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null);
		lock (configLock)
		{
			SaveValue(key, value2);
		}
	}

	public bool ContainsKey(string key)
	{
		lock (configLock)
		{
			if (configuration != null)
			{
				return configuration.AppSettings.Settings[key] != null;
			}
			AppCenterLog.Warn(AppCenterLog.LogTag, "Configuration is corrupted. App Center could work incorrectly. Method 'ContainsKey', key:" + key);
		}
		return false;
	}

	public void Remove(string key)
	{
		lock (configLock)
		{
			if (configuration != null)
			{
				configuration.AppSettings.Settings.Remove(key);
				SaveConfiguration();
			}
			else
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Configuration is corrupted. App Center could work incorrectly. Method 'Remove', key:" + key);
			}
		}
	}

	private void SaveValue(string key, string value)
	{
		lock (configLock)
		{
			if (configuration != null)
			{
				KeyValueConfigurationElement val = configuration.AppSettings.Settings[key];
				if (val == null)
				{
					configuration.AppSettings.Settings.Add(key, value);
				}
				else
				{
					val.Value = value;
				}
				SaveConfiguration();
			}
			else
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Configuration is corrupted. App Center could work incorrectly. Method 'SaveValue', key:" + key + ", value:" + value);
			}
		}
	}

	private void SaveConfiguration()
	{
		//IL_000d: Expected O, but got Unknown
		try
		{
			configuration.Save();
		}
		catch (ConfigurationErrorsException ex)
		{
			ConfigurationErrorsException ex2 = ex;
			AppCenterLog.Warn(AppCenterLog.LogTag, "Configuration file can't be saved. Failure reason: " + ((Exception)(object)ex2).Message);
		}
	}

	private static Configuration OpenConfiguration()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		string text = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration((ConfigurationUserLevel)20).FilePath);
		string directoryName = Path.GetDirectoryName(text);
		if (directoryName != null)
		{
			text = directoryName;
		}
		FilePath = Path.Combine(text, "AppCenter.config");
		try
		{
			string text2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AppCenter.config");
			if (File.Exists(text2))
			{
				if (File.Exists(FilePath))
				{
					File.Delete(text2);
				}
				else
				{
					File.Move(text2, FilePath);
				}
			}
		}
		catch (Exception exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not check/migrate old config file", exception);
		}
		return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
		{
			ExeConfigFilename = FilePath
		}, (ConfigurationUserLevel)0);
	}

	private void RestoreConfigurationFile()
	{
		try
		{
			if (File.Exists(FilePath))
			{
				File.Delete(FilePath);
				configuration = OpenConfiguration();
				AppCenterLog.Info(AppCenterLog.LogTag, "Configuration is successfully restored.");
			}
		}
		catch (Exception exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Could not restore configuration.", exception);
		}
	}
}
