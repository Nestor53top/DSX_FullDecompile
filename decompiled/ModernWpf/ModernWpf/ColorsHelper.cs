using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using ModernWpf.Media.ColorPalette;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace ModernWpf;

internal class ColorsHelper : DispatcherObject
{
	private const string AccentKey = "SystemAccentColor";

	private const string AccentDark1Key = "SystemAccentColorDark1";

	private const string AccentDark2Key = "SystemAccentColorDark2";

	private const string AccentDark3Key = "SystemAccentColorDark3";

	private const string AccentLight1Key = "SystemAccentColorLight1";

	private const string AccentLight2Key = "SystemAccentColorLight2";

	private const string AccentLight3Key = "SystemAccentColorLight3";

	internal static readonly Color DefaultAccentColor = Color.FromRgb((byte)0, (byte)120, (byte)215);

	private readonly ResourceDictionary _colors = new ResourceDictionary();

	private UISettings _uiSettings;

	private Color _systemBackground;

	private Color _systemAccent;

	public static bool SystemColorsSupported { get; } = OSVersionHelper.IsWindows10OrGreater;

	public static ColorsHelper Current { get; } = new ColorsHelper();

	public ResourceDictionary Colors => _colors;

	public ApplicationTheme? SystemTheme { get; private set; }

	public Color SystemAccentColor => _systemAccent;

	public event EventHandler SystemThemeChanged;

	public event EventHandler SystemAccentColorChanged;

	private ColorsHelper()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		if (SystemColorsSupported)
		{
			ListenToSystemColorChanges();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void FetchSystemAccentColors()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		UISettings val = new UISettings();
		_colors[(object)"SystemAccentColor"] = val.GetColorValue((UIColorType)5).ToColor();
		_colors[(object)"SystemAccentColorDark1"] = val.GetColorValue((UIColorType)4).ToColor();
		_colors[(object)"SystemAccentColorDark2"] = val.GetColorValue((UIColorType)3).ToColor();
		_colors[(object)"SystemAccentColorDark3"] = val.GetColorValue((UIColorType)2).ToColor();
		_colors[(object)"SystemAccentColorLight1"] = val.GetColorValue((UIColorType)6).ToColor();
		_colors[(object)"SystemAccentColorLight2"] = val.GetColorValue((UIColorType)7).ToColor();
		_colors[(object)"SystemAccentColorLight3"] = val.GetColorValue((UIColorType)8).ToColor();
	}

	public void SetAccent(Color accent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_colors[(object)"SystemAccentColor"] = accent;
		UpdateShades(_colors, accent);
	}

	public static void UpdateShades(ResourceDictionary colors, Color accent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		ColorPalette colorPalette = new ColorPalette(11, accent);
		colors[(object)"SystemAccentColorDark1"] = colorPalette.Palette[6].ActiveColor;
		colors[(object)"SystemAccentColorDark2"] = colorPalette.Palette[7].ActiveColor;
		colors[(object)"SystemAccentColorDark3"] = colorPalette.Palette[8].ActiveColor;
		colors[(object)"SystemAccentColorLight1"] = colorPalette.Palette[4].ActiveColor;
		colors[(object)"SystemAccentColorLight2"] = colorPalette.Palette[3].ActiveColor;
		colors[(object)"SystemAccentColorLight3"] = colorPalette.Palette[2].ActiveColor;
	}

	public static void RemoveShades(ResourceDictionary colors)
	{
		colors.Remove((object)"SystemAccentColorDark3");
		colors.Remove((object)"SystemAccentColorDark2");
		colors.Remove((object)"SystemAccentColorDark1");
		colors.Remove((object)"SystemAccentColorLight1");
		colors.Remove((object)"SystemAccentColorLight2");
		colors.Remove((object)"SystemAccentColorLight3");
	}

	public void UpdateBrushes(ResourceDictionary themeDictionary)
	{
		UpdateBrushes(themeDictionary, _colors);
	}

	public static void UpdateBrushes(ResourceDictionary themeDictionary, ResourceDictionary colors)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		foreach (DictionaryEntry item in themeDictionary)
		{
			object? value = item.Value;
			SolidColorBrush val = (SolidColorBrush)((value is SolidColorBrush) ? value : null);
			if (val != null && !((Freezable)val).IsFrozen)
			{
				object colorKey = ThemeResourceHelper.GetColorKey(val);
				if (colorKey != null && colors.Contains(colorKey))
				{
					((DependencyObject)val).SetCurrentValue(SolidColorBrush.ColorProperty, (object)(Color)colors[colorKey]);
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ListenToSystemColorChanges()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		_uiSettings = new UISettings();
		UISettings uiSettings = _uiSettings;
		WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<UISettings, object>>((Func<TypedEventHandler<UISettings, object>, EventRegistrationToken>)uiSettings.add_ColorValuesChanged, (Action<EventRegistrationToken>)uiSettings.remove_ColorValuesChanged, (TypedEventHandler<UISettings, object>)OnColorValuesChanged);
		if (PackagedAppHelper.IsPackagedApp)
		{
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
		}
		_systemBackground = _uiSettings.GetColorValue((UIColorType)0).ToColor();
		_systemAccent = _uiSettings.GetColorValue((UIColorType)5).ToColor();
		UpdateSystemAppTheme();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnColorValuesChanged(UISettings sender, object args)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke(UpdateColorValues);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.Category == 4)
		{
			UpdateColorValues();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UpdateColorValues()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Color val = _uiSettings.GetColorValue((UIColorType)0).ToColor();
		if (_systemBackground != val)
		{
			_systemBackground = val;
			UpdateSystemAppTheme();
			this.SystemThemeChanged?.Invoke(null, EventArgs.Empty);
		}
		Color val2 = _uiSettings.GetColorValue((UIColorType)5).ToColor();
		if (_systemAccent != val2)
		{
			_systemAccent = val2;
			this.SystemAccentColorChanged?.Invoke(null, EventArgs.Empty);
		}
	}

	private void UpdateSystemAppTheme()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SystemTheme = (IsDarkBackground(_systemBackground) ? ApplicationTheme.Dark : ApplicationTheme.Light);
	}

	private static bool IsDarkBackground(Color color)
	{
		return ((Color)(ref color)).R + ((Color)(ref color)).G + ((Color)(ref color)).B < 765 - ((Color)(ref color)).R - ((Color)(ref color)).G - ((Color)(ref color)).B;
	}
}
