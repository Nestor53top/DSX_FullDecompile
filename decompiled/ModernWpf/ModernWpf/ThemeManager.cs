using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using ModernWpf.DesignTime;

namespace ModernWpf;

public class ThemeManager : DependencyObject
{
	private class Data : INotifyPropertyChanged
	{
		private ApplicationTheme _actualApplicationTheme;

		public ApplicationTheme ActualApplicationTheme
		{
			get
			{
				return _actualApplicationTheme;
			}
			set
			{
				Set(ref _actualApplicationTheme, value, "ActualApplicationTheme");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Data(ThemeManager owner)
		{
			_actualApplicationTheme = owner.ActualApplicationTheme;
		}

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (!object.Equals(storage, value))
			{
				storage = value;
				RaisePropertyChanged(propertyName);
			}
		}
	}

	internal const string LightKey = "Light";

	internal const string DarkKey = "Dark";

	internal const string HighContrastKey = "HighContrast";

	private static readonly Binding _highContrastBinding;

	private static readonly RoutedEventArgs _actualThemeChangedEventArgs;

	private static readonly Dictionary<string, ResourceDictionary> _defaultThemeDictionaries;

	private readonly Data _data;

	private bool _isInitialized;

	private bool _applicationInitialized;

	public static readonly DependencyProperty ApplicationThemeProperty;

	private static readonly DependencyPropertyKey ActualApplicationThemePropertyKey;

	public static readonly DependencyProperty ActualApplicationThemeProperty;

	public static readonly DependencyProperty AccentColorProperty;

	private static readonly DependencyPropertyKey ActualAccentColorPropertyKey;

	public static readonly DependencyProperty ActualAccentColorProperty;

	public static readonly DependencyProperty RequestedThemeProperty;

	private static readonly DependencyProperty ThemeProperty;

	private static readonly DependencyPropertyKey ActualThemePropertyKey;

	public static readonly DependencyProperty ActualThemeProperty;

	private ElementTheme _defaultActualTheme = ElementTheme.Light;

	public static readonly RoutedEvent ActualThemeChangedEvent;

	public static readonly DependencyProperty IsThemeAwareProperty;

	private static readonly DependencyProperty InheritedApplicationThemeProperty;

	public static readonly DependencyProperty HasThemeResourcesProperty;

	private static readonly DependencyProperty SubscribedToInitializedProperty;

	private static readonly DependencyProperty ElementHighContrastProperty;

	private static readonly DependencyProperty IsListeningForHighContrastChangesProperty;

	public TypedEventHandler<ThemeManager, object> ActualApplicationThemeChanged;

	public TypedEventHandler<ThemeManager, object> ActualAccentColorChanged;

	public ApplicationTheme? ApplicationTheme
	{
		get
		{
			return (ApplicationTheme?)((DependencyObject)this).GetValue(ApplicationThemeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ApplicationThemeProperty, (object)value);
		}
	}

	public ApplicationTheme ActualApplicationTheme
	{
		get
		{
			return (ApplicationTheme)((DependencyObject)this).GetValue(ActualApplicationThemeProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(ActualApplicationThemePropertyKey, (object)value);
		}
	}

	public Color? AccentColor
	{
		get
		{
			return (Color?)((DependencyObject)this).GetValue(AccentColorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AccentColorProperty, (object)value);
		}
	}

	public Color ActualAccentColor
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Color)((DependencyObject)this).GetValue(ActualAccentColorProperty);
		}
		private set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(ActualAccentColorPropertyKey, (object)value);
		}
	}

	public static ThemeManager Current { get; }

	internal bool UsingSystemTheme
	{
		get
		{
			if (ColorsHelper.SystemColorsSupported)
			{
				return !ApplicationTheme.HasValue;
			}
			return false;
		}
	}

	internal bool UsingSystemAccentColor
	{
		get
		{
			if (ColorsHelper.SystemColorsSupported)
			{
				return !AccentColor.HasValue;
			}
			return false;
		}
	}

	static ThemeManager()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Expected O, but got Unknown
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Expected O, but got Unknown
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Expected O, but got Unknown
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Expected O, but got Unknown
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		_highContrastBinding = new Binding("(SystemParameters.HighContrast)");
		_defaultThemeDictionaries = new Dictionary<string, ResourceDictionary>();
		ApplicationThemeProperty = DependencyProperty.Register("ApplicationTheme", typeof(ApplicationTheme?), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnApplicationThemeChanged)));
		ActualApplicationThemePropertyKey = DependencyProperty.RegisterReadOnly("ActualApplicationTheme", typeof(ApplicationTheme), typeof(ThemeManager), new PropertyMetadata((object)ModernWpf.ApplicationTheme.Light, new PropertyChangedCallback(OnActualApplicationThemeChanged)));
		ActualApplicationThemeProperty = ActualApplicationThemePropertyKey.DependencyProperty;
		AccentColorProperty = DependencyProperty.Register("AccentColor", typeof(Color?), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnAccentColorChanged)));
		ActualAccentColorPropertyKey = DependencyProperty.RegisterReadOnly("ActualAccentColor", typeof(Color), typeof(ThemeManager), new PropertyMetadata((object)ColorsHelper.DefaultAccentColor, new PropertyChangedCallback(OnActualAccentColorChanged)));
		ActualAccentColorProperty = ActualAccentColorPropertyKey.DependencyProperty;
		RequestedThemeProperty = DependencyProperty.RegisterAttached("RequestedTheme", typeof(ElementTheme), typeof(ThemeManager), new PropertyMetadata((object)ElementTheme.Default, new PropertyChangedCallback(OnRequestedThemeChanged)));
		ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(ElementTheme), typeof(ThemeManager), (PropertyMetadata)new FrameworkPropertyMetadata((object)ElementTheme.Default, (FrameworkPropertyMetadataOptions)32));
		ActualThemePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualTheme", typeof(ElementTheme), typeof(ThemeManager), (PropertyMetadata)new FrameworkPropertyMetadata((object)ElementTheme.Light, new PropertyChangedCallback(OnActualThemeChanged)));
		ActualThemeProperty = ActualThemePropertyKey.DependencyProperty;
		ActualThemeChangedEvent = EventManager.RegisterRoutedEvent("ActualThemeChanged", (RoutingStrategy)2, typeof(RoutedEventHandler), typeof(ThemeManager));
		IsThemeAwareProperty = DependencyProperty.RegisterAttached("IsThemeAware", typeof(bool), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnIsThemeAwareChanged)));
		InheritedApplicationThemeProperty = DependencyProperty.RegisterAttached("InheritedApplicationTheme", typeof(ApplicationTheme), typeof(ThemeManager), new PropertyMetadata((object)ModernWpf.ApplicationTheme.Light, new PropertyChangedCallback(OnInheritedApplicationThemeChanged)));
		HasThemeResourcesProperty = DependencyProperty.RegisterAttached("HasThemeResources", typeof(bool), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnHasThemeResourcesChanged)));
		SubscribedToInitializedProperty = DependencyProperty.RegisterAttached("SubscribedToInitialized", typeof(bool), typeof(ThemeManager), new PropertyMetadata((object)false, new PropertyChangedCallback(OnSubscribedToInitializedChanged)));
		ElementHighContrastProperty = DependencyProperty.RegisterAttached("ElementHighContrast", typeof(bool), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnElementHighContrastChanged)));
		IsListeningForHighContrastChangesProperty = DependencyProperty.RegisterAttached("IsListeningForHighContrastChanges", typeof(bool), typeof(ThemeManager), new PropertyMetadata(new PropertyChangedCallback(OnIsListeningForHighContrastChangesChanged)));
		Current = new ThemeManager();
		ThemeProperty.OverrideMetadata(typeof(FrameworkElement), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnThemeChanged)));
		_actualThemeChangedEventArgs = new RoutedEventArgs(ActualThemeChangedEvent);
		MenuDropAlignmentHelper.EnsureStandardPopupAlignment();
		if (DesignMode.DesignModeEnabled)
		{
			GetDefaultThemeDictionary("Light");
			GetDefaultThemeDictionary("Dark");
			GetDefaultThemeDictionary("HighContrast");
		}
	}

	private ThemeManager()
	{
		_data = new Data(this);
	}

	private static void OnApplicationThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ThemeManager)(object)d).UpdateActualApplicationTheme();
	}

	private static void OnActualApplicationThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ThemeManager themeManager = (ThemeManager)(object)d;
		ApplicationTheme applicationTheme = (ApplicationTheme)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		switch (applicationTheme)
		{
		case ModernWpf.ApplicationTheme.Light:
			themeManager._defaultActualTheme = ElementTheme.Light;
			break;
		case ModernWpf.ApplicationTheme.Dark:
			themeManager._defaultActualTheme = ElementTheme.Dark;
			break;
		}
		themeManager._data.ActualApplicationTheme = applicationTheme;
		themeManager.ApplyApplicationTheme();
		themeManager.ActualApplicationThemeChanged?.Invoke(themeManager, null);
	}

	private void UpdateActualApplicationTheme()
	{
		if (UsingSystemTheme)
		{
			ActualApplicationTheme = GetDefaultAppTheme();
		}
		else
		{
			ActualApplicationTheme = ApplicationTheme.GetValueOrDefault();
		}
	}

	private void ApplyApplicationTheme()
	{
		if (_applicationInitialized)
		{
			ThemeResources.Current.ApplyApplicationTheme(ActualApplicationTheme);
		}
	}

	private static void OnAccentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ThemeManager)(object)d).UpdateActualAccentColor();
	}

	private static void OnActualAccentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ThemeManager themeManager = (ThemeManager)(object)d;
		themeManager.ApplyAccentColor();
		themeManager.ActualAccentColorChanged?.Invoke(themeManager, null);
	}

	private void ApplyAccentColor()
	{
		if (!_applicationInitialized)
		{
			return;
		}
		UpdateAccentColors();
		foreach (ResourceDictionary value in _defaultThemeDictionaries.Values)
		{
			ColorsHelper.Current.UpdateBrushes(value);
		}
	}

	private void UpdateActualAccentColor()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (UsingSystemAccentColor)
		{
			ActualAccentColor = ColorsHelper.Current.SystemAccentColor;
		}
		else
		{
			ActualAccentColor = (Color)(((_003F?)AccentColor) ?? ColorsHelper.DefaultAccentColor);
		}
	}

	private void UpdateAccentColors()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (UsingSystemAccentColor)
		{
			ColorsHelper.Current.FetchSystemAccentColors();
		}
		else
		{
			ColorsHelper.Current.SetAccent(ActualAccentColor);
		}
	}

	public static ElementTheme GetRequestedTheme(FrameworkElement element)
	{
		return (ElementTheme)((DependencyObject)element).GetValue(RequestedThemeProperty);
	}

	public static void SetRequestedTheme(FrameworkElement element, ElementTheme value)
	{
		((DependencyObject)element).SetValue(RequestedThemeProperty, (object)value);
	}

	private static void OnRequestedThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		StartOrStopListeningForHighContrastChanges(val);
		if (val.IsInitialized)
		{
			ApplyRequestedTheme(val);
		}
		else
		{
			SetSubscribedToInitialized(val, value: true);
		}
	}

	private static void ApplyRequestedTheme(FrameworkElement element)
	{
		ResourceDictionary resources = element.Resources;
		ElementTheme requestedTheme = GetRequestedTheme(element);
		ThemeResources.Current.ApplyElementTheme(resources, requestedTheme);
		Window val = (Window)(object)((element is Window) ? element : null);
		if (val != null)
		{
			UpdateWindowTheme(val);
		}
		else if (requestedTheme != ElementTheme.Default)
		{
			SetTheme(element, requestedTheme);
		}
		else
		{
			((DependencyObject)element).ClearValue(ThemeProperty);
		}
	}

	private static void SetTheme(FrameworkElement element, ElementTheme value)
	{
		((DependencyObject)element).SetValue(ThemeProperty, (object)value);
	}

	private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FrameworkElement val = (FrameworkElement)(object)((d is FrameworkElement) ? d : null);
		if (val != null)
		{
			UpdateActualTheme(val, (ElementTheme)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}

	private static void UpdateWindowTheme(Window window)
	{
		ElementTheme requestedTheme = GetRequestedTheme((FrameworkElement)(object)window);
		if (requestedTheme != ElementTheme.Default)
		{
			SetTheme((FrameworkElement)(object)window, requestedTheme);
		}
		else
		{
			SetTheme((FrameworkElement)(object)window, Current._defaultActualTheme);
		}
	}

	public static ElementTheme GetActualTheme(FrameworkElement element)
	{
		return (ElementTheme)((DependencyObject)element).GetValue(ActualThemeProperty);
	}

	private static void OnActualThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FrameworkElement val = (FrameworkElement)(object)((d is FrameworkElement) ? d : null);
		if (val != null)
		{
			if (GetHasThemeResources(val))
			{
				UpdateThemeResourcesForElement(val);
			}
			RaiseActualThemeChanged(val);
		}
	}

	private static void UpdateActualTheme(FrameworkElement element, ElementTheme theme)
	{
		ElementTheme elementTheme = ((theme == ElementTheme.Default) ? Current._defaultActualTheme : theme);
		((DependencyObject)element).SetValue(ActualThemePropertyKey, (object)elementTheme);
	}

	public static void AddActualThemeChangedHandler(FrameworkElement element, RoutedEventHandler handler)
	{
		((UIElement)element).AddHandler(ActualThemeChangedEvent, (Delegate)(object)handler);
	}

	public static void RemoveActualThemeChangedHandler(FrameworkElement element, RoutedEventHandler handler)
	{
		((UIElement)element).RemoveHandler(ActualThemeChangedEvent, (Delegate)(object)handler);
	}

	private static void RaiseActualThemeChanged(FrameworkElement element)
	{
		((UIElement)element).RaiseEvent(_actualThemeChangedEventArgs);
	}

	public static bool GetIsThemeAware(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(IsThemeAwareProperty);
	}

	public static void SetIsThemeAware(Window window, bool value)
	{
		((DependencyObject)window).SetValue(IsThemeAwareProperty, (object)value);
	}

	private static void OnIsThemeAwareChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		Window val = (Window)(object)((d is Window) ? d : null);
		if (val != null)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
			{
				((FrameworkElement)val).SetBinding(InheritedApplicationThemeProperty, (BindingBase)new Binding("ActualApplicationTheme")
				{
					Source = Current._data
				});
				UpdateWindowTheme((Window)d);
			}
			else
			{
				((DependencyObject)val).ClearValue(InheritedApplicationThemeProperty);
			}
		}
	}

	private static void OnInheritedApplicationThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateWindowTheme((Window)d);
	}

	public static bool GetHasThemeResources(FrameworkElement element)
	{
		return (bool)((DependencyObject)element).GetValue(HasThemeResourcesProperty);
	}

	public static void SetHasThemeResources(FrameworkElement element, bool value)
	{
		((DependencyObject)element).SetValue(HasThemeResourcesProperty, (object)value);
	}

	private static void OnHasThemeResourcesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			if (val.IsInitialized)
			{
				UpdateThemeResourcesForElement(val);
			}
			else
			{
				SetSubscribedToInitialized(val, value: true);
			}
		}
		else
		{
			UpdateSubscribedToInitialized(val);
		}
	}

	private static void UpdateThemeResourcesForElement(FrameworkElement element)
	{
		UpdateElementThemeResources(element.Resources, GetEffectiveThemeKey(element));
	}

	private static void UpdateElementThemeResources(ResourceDictionary resources, string themeKey)
	{
		if (resources is ResourceDictionaryEx resourceDictionaryEx)
		{
			resourceDictionaryEx.Update(themeKey);
		}
		foreach (ResourceDictionary mergedDictionary in resources.MergedDictionaries)
		{
			UpdateElementThemeResources(mergedDictionary, themeKey);
		}
	}

	private static string GetEffectiveThemeKey(FrameworkElement element)
	{
		if (SystemParameters.HighContrast)
		{
			return "HighContrast";
		}
		return GetActualTheme(element) switch
		{
			ElementTheme.Light => "Light", 
			ElementTheme.Dark => "Dark", 
			_ => throw new InvalidOperationException(), 
		};
	}

	private static void SetSubscribedToInitialized(FrameworkElement element, bool value)
	{
		((DependencyObject)element).SetValue(SubscribedToInitializedProperty, (object)value);
	}

	private static void OnSubscribedToInitializedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			val.Initialized += OnElementInitialized;
		}
		else
		{
			val.Initialized -= OnElementInitialized;
		}
	}

	private static void UpdateSubscribedToInitialized(FrameworkElement element)
	{
		if (ShouldSubscribeToInitialized(element))
		{
			SetSubscribedToInitialized(element, value: true);
		}
		else
		{
			((DependencyObject)element).ClearValue(SubscribedToInitializedProperty);
		}
	}

	private static bool ShouldSubscribeToInitialized(FrameworkElement element)
	{
		if (!element.IsInitialized)
		{
			if (GetRequestedTheme(element) == ElementTheme.Default)
			{
				return GetHasThemeResources(element);
			}
			return true;
		}
		return false;
	}

	private static void OnElementInitialized(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)sender;
		((DependencyObject)val).ClearValue(SubscribedToInitializedProperty);
		if (GetRequestedTheme(val) != ElementTheme.Default)
		{
			ApplyRequestedTheme(val);
		}
		if (GetHasThemeResources(val))
		{
			UpdateThemeResourcesForElement(val);
		}
	}

	private static void OnElementHighContrastChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if (val.IsInitialized)
		{
			ApplyRequestedTheme(val);
			UpdateThemeResourcesForElement(val);
		}
		else
		{
			SetSubscribedToInitialized(val, value: true);
		}
	}

	private static void OnIsListeningForHighContrastChangesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue)
		{
			((DependencyObject)val).ClearValue(ElementHighContrastProperty);
		}
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			val.SetBinding(ElementHighContrastProperty, (BindingBase)(object)_highContrastBinding);
		}
	}

	private static bool ShouldListenForHighContrastChanges(FrameworkElement element)
	{
		if (GetRequestedTheme(element) == ElementTheme.Default)
		{
			return GetHasThemeResources(element);
		}
		return true;
	}

	private static void StartOrStopListeningForHighContrastChanges(FrameworkElement element)
	{
		if (ShouldListenForHighContrastChanges(element))
		{
			((DependencyObject)element).SetValue(IsListeningForHighContrastChangesProperty, (object)true);
		}
		else
		{
			((DependencyObject)element).ClearValue(IsListeningForHighContrastChangesProperty);
		}
	}

	internal static void UpdateThemeBrushes(ResourceDictionary colors)
	{
		foreach (ResourceDictionary value in _defaultThemeDictionaries.Values)
		{
			ColorsHelper.UpdateBrushes(value, colors);
		}
	}

	internal static ResourceDictionary GetDefaultThemeDictionary(string key)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (!_defaultThemeDictionaries.TryGetValue(key, out var value))
		{
			value = new ResourceDictionary
			{
				Source = GetDefaultSource(key)
			};
			_defaultThemeDictionaries[key] = value;
		}
		return value;
	}

	internal static void SetDefaultThemeDictionary(string key, ResourceDictionary dictionary)
	{
		_defaultThemeDictionaries[key] = dictionary;
	}

	private static ApplicationTheme GetDefaultAppTheme()
	{
		return ColorsHelper.Current.SystemTheme ?? ModernWpf.ApplicationTheme.Light;
	}

	private static Uri GetDefaultSource(string theme)
	{
		return PackUriHelper.GetAbsoluteUri("ThemeResources/" + theme + ".xaml");
	}

	private static ResourceDictionary FindDictionary(ResourceDictionary parent, Uri source)
	{
		if (parent.Source == source)
		{
			return parent;
		}
		foreach (ResourceDictionary mergedDictionary in parent.MergedDictionaries)
		{
			if (mergedDictionary != null)
			{
				if (mergedDictionary.Source == source)
				{
					return mergedDictionary;
				}
				ResourceDictionary val = FindDictionary(mergedDictionary, source);
				if (val != null)
				{
					return val;
				}
			}
		}
		return null;
	}

	internal void Initialize()
	{
		if (!_isInitialized)
		{
			SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
			if (Application.Current != null)
			{
				ResourceDictionary resources = Application.Current.Resources;
				resources.MergedDictionaries.RemoveAll<IntellisenseResourcesBase>();
				ColorsHelper.Current.SystemThemeChanged += OnSystemThemeChanged;
				ColorsHelper.Current.SystemAccentColorChanged += OnSystemAccentColorChanged;
				resources.MergedDictionaries.Insert(0, ColorsHelper.Current.Colors);
				UpdateActualAccentColor();
				UpdateActualApplicationTheme();
				_applicationInitialized = true;
				ApplyAccentColor();
				ApplyApplicationTheme();
			}
			_isInitialized = true;
		}
	}

	private void OnSystemThemeChanged(object sender, EventArgs e)
	{
		if (UsingSystemTheme)
		{
			UpdateActualApplicationTheme();
		}
	}

	private void OnSystemAccentColorChanged(object sender, EventArgs e)
	{
		if (UsingSystemAccentColor)
		{
			UpdateActualAccentColor();
		}
	}

	private void OnSystemParametersChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "HighContrast")
		{
			RunOnMainThread(ApplyApplicationTheme);
		}
	}

	private void RunOnMainThread(Action action)
	{
		if (((DispatcherObject)this).Dispatcher.CheckAccess())
		{
			action();
		}
		else
		{
			((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)action, Array.Empty<object>());
		}
	}
}
