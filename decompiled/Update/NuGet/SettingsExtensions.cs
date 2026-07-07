using System;
using System.IO;

namespace NuGet;

internal static class SettingsExtensions
{
	private const string ConfigSection = "config";

	public static string GetRepositoryPath(this ISettings settings)
	{
		string text = settings.GetValue("config", "repositoryPath", isPath: true);
		if (!string.IsNullOrEmpty(text))
		{
			text = text.Replace('/', Path.DirectorySeparatorChar);
		}
		return text;
	}

	public static string GetDecryptedValue(this ISettings settings, string section, string key, bool isPath = false)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		string value = settings.GetValue(section, key, isPath);
		if (value == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		return EncryptionUtility.DecryptString(value);
	}

	public static void SetEncryptedValue(this ISettings settings, string section, string key, string value)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (string.IsNullOrEmpty(value))
		{
			settings.SetValue(section, key, string.Empty);
			return;
		}
		string value2 = EncryptionUtility.EncryptString(value);
		settings.SetValue(section, key, value2);
	}

	public static string GetConfigValue(this ISettings settings, string key, bool decrypt = false, bool isPath = false)
	{
		if (!decrypt)
		{
			return settings.GetValue("config", key, isPath);
		}
		return settings.GetDecryptedValue("config", key, isPath);
	}

	public static void SetConfigValue(this ISettings settings, string key, string value, bool encrypt = false)
	{
		if (encrypt)
		{
			settings.SetEncryptedValue("config", key, value);
		}
		else
		{
			settings.SetValue("config", key, value);
		}
	}

	public static bool DeleteConfigValue(this ISettings settings, string key)
	{
		return settings.DeleteValue("config", key);
	}
}
