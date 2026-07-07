using System.ComponentModel;
using System.Windows;

namespace ModernWpf.DesignTime;

public static class Design
{
	public static readonly DependencyProperty RequestedThemeProperty = DependencyProperty.RegisterAttached("RequestedTheme", typeof(ElementTheme), typeof(Design), new PropertyMetadata((object)ElementTheme.Default, new PropertyChangedCallback(OnRequestedThemeChanged)));

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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (DesignerProperties.GetIsInDesignMode(d))
		{
			ThemeManager.SetRequestedTheme((FrameworkElement)d, (ElementTheme)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}
}
