using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class WindowHelper
{
	private const string DefaultWindowStyleKey = "DefaultWindowStyle";

	public static readonly DependencyProperty UseModernWindowStyleProperty = DependencyProperty.RegisterAttached("UseModernWindowStyle", typeof(bool), typeof(WindowHelper), new PropertyMetadata(new PropertyChangedCallback(OnUseModernWindowStyleChanged)));

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly DependencyProperty FixMaximizedWindowProperty = DependencyProperty.RegisterAttached("FixMaximizedWindow", typeof(bool), typeof(WindowHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnFixMaximizedWindowChanged)));

	public static bool GetUseModernWindowStyle(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(UseModernWindowStyleProperty);
	}

	public static void SetUseModernWindowStyle(Window window, bool value)
	{
		((DependencyObject)window).SetValue(UseModernWindowStyleProperty, (object)value);
	}

	private static void OnUseModernWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		bool flag = (bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (DesignerProperties.GetIsInDesignMode(d))
		{
			Control val = (Control)(object)((d is Control) ? d : null);
			if (val == null)
			{
				return;
			}
			if (flag)
			{
				object obj = ((FrameworkElement)val).TryFindResource((object)"DefaultWindowStyle");
				Style val2 = (Style)((obj is Style) ? obj : null);
				if (val2 == null)
				{
					return;
				}
				Style val3 = new Style();
				foreach (Setter item in (Collection<SetterBase>)(object)val2.Setters)
				{
					Setter val4 = item;
					if (val4.Property == Control.BackgroundProperty || val4.Property == Control.ForegroundProperty)
					{
						((Collection<SetterBase>)(object)val3.Setters).Add((SetterBase)(object)val4);
					}
				}
				((FrameworkElement)val).Style = val3;
			}
			else
			{
				((DependencyObject)val).ClearValue(FrameworkElement.StyleProperty);
			}
		}
		else
		{
			Window val5 = (Window)d;
			if (flag)
			{
				((FrameworkElement)val5).SetResourceReference(FrameworkElement.StyleProperty, (object)"DefaultWindowStyle");
			}
			else
			{
				((DependencyObject)val5).ClearValue(FrameworkElement.StyleProperty);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool GetFixMaximizedWindow(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(FixMaximizedWindowProperty);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void SetFixMaximizedWindow(Window window, bool value)
	{
		((DependencyObject)window).SetValue(FixMaximizedWindowProperty, (object)value);
	}

	private static void OnFixMaximizedWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		Window val = (Window)(object)((d is Window) ? d : null);
		if (val != null)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
			{
				MaximizedWindowFixer.SetMaximizedWindowFixer(val, new MaximizedWindowFixer());
			}
			else
			{
				((DependencyObject)val).ClearValue(MaximizedWindowFixer.MaximizedWindowFixerProperty);
			}
		}
	}
}
