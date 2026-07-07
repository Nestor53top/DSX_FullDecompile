using System.Windows;

namespace ModernWpf;

public static class ThemeDictionary
{
	public static void SetKey(ResourceDictionary themeDictionary, string key)
	{
		ResourceDictionary baseThemeDictionary = GetBaseThemeDictionary(key);
		themeDictionary.MergedDictionaries.Insert(0, baseThemeDictionary);
	}

	private static ResourceDictionary GetBaseThemeDictionary(string key)
	{
		return ThemeResources.Current?.TryGetThemeDictionary(key) ?? ThemeManager.GetDefaultThemeDictionary(key);
	}
}
