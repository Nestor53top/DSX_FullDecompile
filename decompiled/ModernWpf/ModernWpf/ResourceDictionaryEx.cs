using System.Collections.Generic;
using System.Windows;

namespace ModernWpf;

public class ResourceDictionaryEx : ResourceDictionary
{
	private ResourceDictionary _mergedThemeDictionary;

	public Dictionary<object, ResourceDictionary> ThemeDictionaries { get; } = new Dictionary<object, ResourceDictionary>();

	internal ResourceDictionary MergedAppThemeDictionary { get; set; }

	internal void Update(string themeKey)
	{
		if (ThemeDictionaries.TryGetValue(themeKey, out var value))
		{
			if (_mergedThemeDictionary != null)
			{
				if (_mergedThemeDictionary != value)
				{
					int index = ((ResourceDictionary)this).MergedDictionaries.IndexOf(_mergedThemeDictionary);
					((ResourceDictionary)this).MergedDictionaries[index] = value;
					_mergedThemeDictionary = value;
				}
			}
			else
			{
				int index2 = ((MergedAppThemeDictionary != null) ? (((ResourceDictionary)this).MergedDictionaries.IndexOf(MergedAppThemeDictionary) + 1) : 0);
				((ResourceDictionary)this).MergedDictionaries.Insert(index2, value);
				_mergedThemeDictionary = value;
			}
		}
		else if (_mergedThemeDictionary != null)
		{
			((ResourceDictionary)this).MergedDictionaries.Remove(_mergedThemeDictionary);
			_mergedThemeDictionary = null;
		}
	}
}
