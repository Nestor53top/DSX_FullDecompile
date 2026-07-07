using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ModernWpf;

public class ThemeResources : ResourceDictionaryEx, ISupportInitialize
{
	private static ThemeResources _current;

	private ResourceDictionary _lightResources;

	private ResourceDictionary _darkResources;

	private ResourceDictionary _highContrastResources;

	private bool _canBeAccessedAcrossThreads;

	internal static ThemeResources Current
	{
		get
		{
			return _current;
		}
		set
		{
			if (_current != null)
			{
				throw new InvalidOperationException("Current cannot be changed after it's set.");
			}
			_current = value;
		}
	}

	public ApplicationTheme? RequestedTheme
	{
		get
		{
			return ThemeManager.Current.ApplicationTheme;
		}
		set
		{
			if (ThemeManager.Current.ApplicationTheme != value)
			{
				((DependencyObject)ThemeManager.Current).SetCurrentValue(ThemeManager.ApplicationThemeProperty, (object)value);
				if (DesignMode.DesignModeEnabled)
				{
					UpdateDesignTimeThemeDictionary();
				}
			}
		}
	}

	public Color? AccentColor
	{
		get
		{
			return ThemeManager.Current.AccentColor;
		}
		set
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			Color? accentColor = ThemeManager.Current.AccentColor;
			Color? val = value;
			if (accentColor.HasValue != val.HasValue || (accentColor.HasValue && accentColor.GetValueOrDefault() != val.GetValueOrDefault()))
			{
				((DependencyObject)ThemeManager.Current).SetCurrentValue(ThemeManager.AccentColorProperty, (object)value);
				if (DesignMode.DesignModeEnabled)
				{
					UpdateDesignTimeSystemColors();
				}
			}
		}
	}

	public bool CanBeAccessedAcrossThreads
	{
		get
		{
			return _canBeAccessedAcrossThreads;
		}
		set
		{
			if (!DesignMode.DesignModeEnabled)
			{
				if (IsInitialized)
				{
					throw new InvalidOperationException();
				}
				_canBeAccessedAcrossThreads = value;
			}
		}
	}

	private bool IsInitialized { get; set; }

	private bool IsInitializePending { get; set; }

	private int MergedThemeDictionaryCount
	{
		get
		{
			int num = 0;
			if (IsMerged(_lightResources))
			{
				num++;
			}
			if (IsMerged(_darkResources))
			{
				num++;
			}
			if (IsMerged(_highContrastResources))
			{
				num++;
			}
			return num;
		}
	}

	public ThemeResources()
	{
		if (Current == null)
		{
			Current = this;
		}
	}

	private void DesignTimeInit()
	{
		UpdateDesignTimeSystemColors();
		UpdateDesignTimeThemeDictionary();
		SystemParameters.StaticPropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "HighContrast")
			{
				UpdateDesignTimeThemeDictionary();
			}
		};
	}

	private void UpdateDesignTimeSystemColors()
	{
		if (!IsInitializePending)
		{
			ResourceDictionary designTimeSystemColors = GetDesignTimeSystemColors();
			((ResourceDictionary)this).MergedDictionaries.InsertOrReplace(0, designTimeSystemColors);
			ThemeManager.UpdateThemeBrushes(designTimeSystemColors);
		}
	}

	private void UpdateDesignTimeThemeDictionary()
	{
		if (IsInitializePending)
		{
			return;
		}
		if (SystemParameters.HighContrast)
		{
			EnsureHighContrastResources();
			updateTo(_highContrastResources);
			return;
		}
		switch (RequestedTheme.GetValueOrDefault())
		{
		case ApplicationTheme.Light:
			EnsureLightResources();
			updateTo(_lightResources);
			break;
		case ApplicationTheme.Dark:
			EnsureDarkResources();
			updateTo(_darkResources);
			break;
		}
		void updateTo(ResourceDictionary themeDictionary)
		{
			((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_lightResources);
			((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_darkResources);
			((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_highContrastResources);
			((ResourceDictionary)this).MergedDictionaries.Insert(1, themeDictionary);
		}
	}

	private ResourceDictionary GetDesignTimeSystemColors()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		if (AccentColor.HasValue)
		{
			return (ResourceDictionary)(object)new ColorPaletteResources
			{
				Accent = AccentColor
			};
		}
		return new ResourceDictionary
		{
			Source = PackUriHelper.GetAbsoluteUri("DesignTime/SystemColors.xaml")
		};
	}

	public void BeginInit()
	{
		((ResourceDictionary)this).BeginInit();
		IsInitializePending = true;
		IsInitialized = false;
	}

	public void EndInit()
	{
		IsInitializePending = false;
		IsInitialized = true;
		if (DesignMode.DesignModeEnabled)
		{
			DesignTimeInit();
		}
		else
		{
			ThemeManager.Current.Initialize();
			if (CanBeAccessedAcrossThreads)
			{
				EnsureLightResources();
				EnsureDarkResources();
				EnsureHighContrastResources();
				_lightResources.SealValues();
				_darkResources.SealValues();
				_highContrastResources.SealValues();
			}
		}
		((ResourceDictionary)this).EndInit();
	}

	void ISupportInitialize.BeginInit()
	{
		BeginInit();
	}

	void ISupportInitialize.EndInit()
	{
		EndInit();
	}

	internal void ApplyApplicationTheme(ApplicationTheme theme)
	{
		int index = (DesignMode.DesignModeEnabled ? 1 : 0);
		if (SystemParameters.HighContrast)
		{
			EnsureHighContrastResources();
			if (IsMerged(_highContrastResources))
			{
				if (CanBeAccessedAcrossThreads)
				{
					RefreshHighContrastResources();
				}
			}
			else
			{
				((ResourceDictionary)this).MergedDictionaries.InsertOrReplace(index, _highContrastResources);
				((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_lightResources);
				((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_darkResources);
			}
			return;
		}
		switch (theme)
		{
		case ApplicationTheme.Light:
			EnsureLightResources();
			((ResourceDictionary)this).MergedDictionaries.InsertOrReplace(index, _lightResources);
			((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_darkResources);
			break;
		case ApplicationTheme.Dark:
			EnsureDarkResources();
			((ResourceDictionary)this).MergedDictionaries.InsertOrReplace(index, _darkResources);
			((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_lightResources);
			break;
		default:
			throw new ArgumentOutOfRangeException("theme");
		}
		((ResourceDictionary)this).MergedDictionaries.RemoveIfNotNull(_highContrastResources);
	}

	internal void ApplyElementTheme(ResourceDictionary target, ElementTheme theme)
	{
		ResourceDictionary mergedAppThemeDictionary = null;
		if (SystemParameters.HighContrast)
		{
			target.MergedDictionaries.RemoveIfNotNull(_lightResources);
			target.MergedDictionaries.RemoveIfNotNull(_darkResources);
		}
		else
		{
			switch (theme)
			{
			case ElementTheme.Light:
				EnsureLightResources();
				target.MergedDictionaries.RemoveIfNotNull(_darkResources);
				target.MergedDictionaries.InsertIfNotExists(0, _lightResources);
				mergedAppThemeDictionary = _lightResources;
				break;
			case ElementTheme.Dark:
				EnsureDarkResources();
				target.MergedDictionaries.RemoveIfNotNull(_lightResources);
				target.MergedDictionaries.InsertIfNotExists(0, _darkResources);
				mergedAppThemeDictionary = _darkResources;
				break;
			default:
				target.MergedDictionaries.RemoveIfNotNull(_lightResources);
				target.MergedDictionaries.RemoveIfNotNull(_darkResources);
				break;
			}
		}
		if (target is ResourceDictionaryEx resourceDictionaryEx)
		{
			resourceDictionaryEx.MergedAppThemeDictionary = mergedAppThemeDictionary;
		}
	}

	internal ResourceDictionary GetThemeDictionary(string key)
	{
		switch (key)
		{
		case "Light":
			EnsureLightResources();
			return _lightResources;
		case "Dark":
			EnsureDarkResources();
			return _darkResources;
		case "HighContrast":
			EnsureHighContrastResources();
			return _highContrastResources;
		default:
			throw new ArgumentException();
		}
	}

	internal ResourceDictionary TryGetThemeDictionary(string key)
	{
		return (ResourceDictionary)(key switch
		{
			"Light" => _lightResources, 
			"Dark" => _darkResources, 
			"HighContrast" => _highContrastResources, 
			_ => null, 
		});
	}

	private void EnsureLightResources()
	{
		if (_lightResources == null)
		{
			_lightResources = InitializeThemeDictionary("Light");
		}
	}

	private void EnsureDarkResources()
	{
		if (_darkResources == null)
		{
			_darkResources = InitializeThemeDictionary("Dark");
		}
	}

	private void EnsureHighContrastResources()
	{
		if (_highContrastResources == null)
		{
			_highContrastResources = InitializeThemeDictionary("HighContrast");
		}
	}

	private void RefreshHighContrastResources()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		Collection<ResourceDictionary> mergedDictionaries = _highContrastResources.MergedDictionaries;
		ResourceDictionary defaultThemeDictionary = ThemeManager.GetDefaultThemeDictionary("HighContrast");
		for (int i = 0; i < mergedDictionaries.Count; i++)
		{
			ResourceDictionary val = mergedDictionaries[i];
			if (val.Source != null)
			{
				ResourceDictionary val2 = new ResourceDictionary
				{
					Source = val.Source
				};
				val2.SealValues();
				if (val == defaultThemeDictionary)
				{
					ThemeManager.SetDefaultThemeDictionary("HighContrast", val2);
				}
				mergedDictionaries[i] = val2;
			}
		}
	}

	private bool IsMerged(ResourceDictionary dictionary)
	{
		if (dictionary != null)
		{
			return ((ResourceDictionary)this).MergedDictionaries.Contains(dictionary);
		}
		return false;
	}

	private ResourceDictionary InitializeThemeDictionary(string key)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		ResourceDictionary defaultThemeDictionary = ThemeManager.GetDefaultThemeDictionary(key);
		if (base.ThemeDictionaries.TryGetValue(key, out var value))
		{
			if (!ContainsDefaultThemeResources(value, defaultThemeDictionary))
			{
				value.MergedDictionaries.Insert(0, defaultThemeDictionary);
			}
		}
		else if (key == "HighContrast")
		{
			value = new ResourceDictionary();
			value.MergedDictionaries.Add(defaultThemeDictionary);
		}
		else
		{
			value = defaultThemeDictionary;
		}
		return value;
	}

	private static bool ContainsDefaultThemeResources(ResourceDictionary dictionary, ResourceDictionary defaultResources)
	{
		if (dictionary == defaultResources || SourceEquals(dictionary.Source, defaultResources.Source))
		{
			return true;
		}
		foreach (ResourceDictionary mergedDictionary in dictionary.MergedDictionaries)
		{
			if (mergedDictionary != null && ContainsDefaultThemeResources(mergedDictionary, defaultResources))
			{
				return true;
			}
		}
		return false;
		static bool SourceEquals(Uri x, Uri y)
		{
			if (x == null || y == null)
			{
				return false;
			}
			string a = (x.IsAbsoluteUri ? x.LocalPath : x.ToString());
			string b = (y.IsAbsoluteUri ? y.LocalPath : y.ToString());
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
