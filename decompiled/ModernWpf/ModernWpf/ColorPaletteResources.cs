using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace ModernWpf;

public class ColorPaletteResources : ResourceDictionary, ISupportInitialize
{
	private ApplicationTheme? _targetTheme;

	private Color? _accent;

	private Color? _altHigh;

	private Color? _altLow;

	private Color? _altMedium;

	private Color? _altMediumHigh;

	private Color? _altMediumLow;

	private Color? _baseHigh;

	private Color? _baseLow;

	private Color? _baseMedium;

	private Color? _baseMediumHigh;

	private Color? _baseMediumLow;

	private Color? _chromeAltLow;

	private Color? _chromeBlackHigh;

	private Color? _chromeBlackLow;

	private Color? _chromeBlackMedium;

	private Color? _chromeBlackMediumLow;

	private Color? _chromeDisabledHigh;

	private Color? _chromeDisabledLow;

	private Color? _chromeGray;

	private Color? _chromeHigh;

	private Color? _chromeLow;

	private Color? _chromeMedium;

	private Color? _chromeMediumLow;

	private Color? _chromeWhite;

	private Color? _errorText;

	private Color? _listLow;

	private Color? _listMedium;

	public ApplicationTheme? TargetTheme
	{
		get
		{
			return _targetTheme;
		}
		set
		{
			if (_targetTheme.HasValue)
			{
				throw new InvalidOperationException("TargetTheme cannot be changed after it's set.");
			}
			if (_targetTheme != value)
			{
				_targetTheme = value;
				UpdateBrushes();
			}
		}
	}

	public Color? Accent
	{
		get
		{
			return _accent;
		}
		set
		{
			if (Set(ref _accent, value, updateBrushes: false, "Accent"))
			{
				UpdateAccentShades();
				if (TargetTheme.HasValue)
				{
					UpdateBrushes();
				}
			}
		}
	}

	public Color? AltHigh
	{
		get
		{
			return _altHigh;
		}
		set
		{
			Set(ref _altHigh, value, updateBrushes: true, "AltHigh");
		}
	}

	public Color? AltLow
	{
		get
		{
			return _altLow;
		}
		set
		{
			Set(ref _altLow, value, updateBrushes: true, "AltLow");
		}
	}

	public Color? AltMedium
	{
		get
		{
			return _altMedium;
		}
		set
		{
			Set(ref _altMedium, value, updateBrushes: true, "AltMedium");
		}
	}

	public Color? AltMediumHigh
	{
		get
		{
			return _altMediumHigh;
		}
		set
		{
			Set(ref _altMediumHigh, value, updateBrushes: true, "AltMediumHigh");
		}
	}

	public Color? AltMediumLow
	{
		get
		{
			return _altMediumLow;
		}
		set
		{
			Set(ref _altMediumLow, value, updateBrushes: true, "AltMediumLow");
		}
	}

	public Color? BaseHigh
	{
		get
		{
			return _baseHigh;
		}
		set
		{
			Set(ref _baseHigh, value, updateBrushes: true, "BaseHigh");
		}
	}

	public Color? BaseLow
	{
		get
		{
			return _baseLow;
		}
		set
		{
			Set(ref _baseLow, value, updateBrushes: true, "BaseLow");
		}
	}

	public Color? BaseMedium
	{
		get
		{
			return _baseMedium;
		}
		set
		{
			Set(ref _baseMedium, value, updateBrushes: true, "BaseMedium");
		}
	}

	public Color? BaseMediumHigh
	{
		get
		{
			return _baseMediumHigh;
		}
		set
		{
			Set(ref _baseMediumHigh, value, updateBrushes: true, "BaseMediumHigh");
		}
	}

	public Color? BaseMediumLow
	{
		get
		{
			return _baseMediumLow;
		}
		set
		{
			Set(ref _baseMediumLow, value, updateBrushes: true, "BaseMediumLow");
		}
	}

	public Color? ChromeAltLow
	{
		get
		{
			return _chromeAltLow;
		}
		set
		{
			Set(ref _chromeAltLow, value, updateBrushes: true, "ChromeAltLow");
		}
	}

	public Color? ChromeBlackHigh
	{
		get
		{
			return _chromeBlackHigh;
		}
		set
		{
			Set(ref _chromeBlackHigh, value, updateBrushes: true, "ChromeBlackHigh");
		}
	}

	public Color? ChromeBlackLow
	{
		get
		{
			return _chromeBlackLow;
		}
		set
		{
			Set(ref _chromeBlackLow, value, updateBrushes: true, "ChromeBlackLow");
		}
	}

	public Color? ChromeBlackMedium
	{
		get
		{
			return _chromeBlackMedium;
		}
		set
		{
			Set(ref _chromeBlackMedium, value, updateBrushes: true, "ChromeBlackMedium");
		}
	}

	public Color? ChromeBlackMediumLow
	{
		get
		{
			return _chromeBlackMediumLow;
		}
		set
		{
			Set(ref _chromeBlackMediumLow, value, updateBrushes: true, "ChromeBlackMediumLow");
		}
	}

	public Color? ChromeDisabledHigh
	{
		get
		{
			return _chromeDisabledHigh;
		}
		set
		{
			Set(ref _chromeDisabledHigh, value, updateBrushes: true, "ChromeDisabledHigh");
		}
	}

	public Color? ChromeDisabledLow
	{
		get
		{
			return _chromeDisabledLow;
		}
		set
		{
			Set(ref _chromeDisabledLow, value, updateBrushes: true, "ChromeDisabledLow");
		}
	}

	public Color? ChromeGray
	{
		get
		{
			return _chromeGray;
		}
		set
		{
			Set(ref _chromeGray, value, updateBrushes: true, "ChromeGray");
		}
	}

	public Color? ChromeHigh
	{
		get
		{
			return _chromeHigh;
		}
		set
		{
			Set(ref _chromeHigh, value, updateBrushes: true, "ChromeHigh");
		}
	}

	public Color? ChromeLow
	{
		get
		{
			return _chromeLow;
		}
		set
		{
			Set(ref _chromeLow, value, updateBrushes: true, "ChromeLow");
		}
	}

	public Color? ChromeMedium
	{
		get
		{
			return _chromeMedium;
		}
		set
		{
			Set(ref _chromeMedium, value, updateBrushes: true, "ChromeMedium");
		}
	}

	public Color? ChromeMediumLow
	{
		get
		{
			return _chromeMediumLow;
		}
		set
		{
			Set(ref _chromeMediumLow, value, updateBrushes: true, "ChromeMediumLow");
		}
	}

	public Color? ChromeWhite
	{
		get
		{
			return _chromeWhite;
		}
		set
		{
			Set(ref _chromeWhite, value, updateBrushes: true, "ChromeWhite");
		}
	}

	public Color? ErrorText
	{
		get
		{
			return _errorText;
		}
		set
		{
			Set(ref _errorText, value, updateBrushes: true, "ErrorText");
		}
	}

	public Color? ListLow
	{
		get
		{
			return _listLow;
		}
		set
		{
			Set(ref _listLow, value, updateBrushes: true, "ListLow");
		}
	}

	public Color? ListMedium
	{
		get
		{
			return _listMedium;
		}
		set
		{
			Set(ref _listMedium, value, updateBrushes: true, "ListMedium");
		}
	}

	private bool IsInitializePending { get; set; }

	private bool Set(ref Color? storage, Color? value, bool updateBrushes = true, [CallerMemberName] string propertyName = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Color? val = storage;
		Color? val2 = value;
		if (val.HasValue != val2.HasValue || (val.HasValue && val.GetValueOrDefault() != val2.GetValueOrDefault()))
		{
			string text = "System" + propertyName + "Color";
			if (storage.HasValue)
			{
				((ResourceDictionary)this).Remove((object)text);
			}
			storage = value;
			if (storage.HasValue)
			{
				((ResourceDictionary)this).Add((object)text, (object)storage.Value);
			}
			if (TargetTheme.HasValue && updateBrushes)
			{
				UpdateBrushes();
			}
			return true;
		}
		return false;
	}

	private void UpdateAccentShades()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInitializePending)
		{
			if (Accent.HasValue)
			{
				ColorsHelper.UpdateShades((ResourceDictionary)(object)this, Accent.Value);
			}
			else
			{
				ColorsHelper.RemoveShades((ResourceDictionary)(object)this);
			}
		}
	}

	private void UpdateBrushes()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (IsInitializePending)
		{
			return;
		}
		if (((ResourceDictionary)this).MergedDictionaries.Count > 0)
		{
			((ResourceDictionary)this).MergedDictionaries.Clear();
		}
		if (!TargetTheme.HasValue || ((ResourceDictionary)this).Count == 0)
		{
			return;
		}
		ResourceDictionary defaultThemeDictionary = ThemeManager.GetDefaultThemeDictionary(TargetTheme.Value.ToString());
		ResourceDictionary val = new ResourceDictionary();
		Dictionary<SolidColorBrush, SolidColorBrush> dictionary = new Dictionary<SolidColorBrush, SolidColorBrush>();
		foreach (DictionaryEntry item in defaultThemeDictionary)
		{
			object? value = item.Value;
			SolidColorBrush val2 = (SolidColorBrush)((value is SolidColorBrush) ? value : null);
			if (val2 == null)
			{
				continue;
			}
			object colorKey = ThemeResourceHelper.GetColorKey(val2);
			if (colorKey != null && ((ResourceDictionary)this).Contains(colorKey))
			{
				if (!dictionary.TryGetValue(val2, out var value2))
				{
					value2 = val2.CloneCurrentValue();
					value2.Color = (Color)((ResourceDictionary)this)[colorKey];
					dictionary[val2] = value2;
				}
				val.Add(item.Key, (object)value2);
			}
		}
		((ResourceDictionary)this).MergedDictionaries.Add(val);
	}

	public void BeginInit()
	{
		((ResourceDictionary)this).BeginInit();
		IsInitializePending = true;
	}

	public void EndInit()
	{
		((ResourceDictionary)this).EndInit();
		IsInitializePending = false;
		if (Accent.HasValue)
		{
			UpdateAccentShades();
		}
		UpdateBrushes();
	}

	void ISupportInitialize.BeginInit()
	{
		BeginInit();
	}

	void ISupportInitialize.EndInit()
	{
		EndInit();
	}
}
