using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public static class CalendarHelper
{
	public static readonly DependencyProperty AutoReleaseMouseCaptureProperty = DependencyProperty.RegisterAttached("AutoReleaseMouseCapture", typeof(bool), typeof(CalendarHelper), new PropertyMetadata(new PropertyChangedCallback(OnAutoReleaseMouseCaptureChanged)));

	public static bool GetAutoReleaseMouseCapture(Calendar calendar)
	{
		return (bool)((DependencyObject)calendar).GetValue(AutoReleaseMouseCaptureProperty);
	}

	public static void SetAutoReleaseMouseCapture(Calendar calendar, bool value)
	{
		((DependencyObject)calendar).SetValue(AutoReleaseMouseCaptureProperty, (object)value);
	}

	private static void OnAutoReleaseMouseCaptureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		Calendar val = (Calendar)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)val).GotMouseCapture += new MouseEventHandler(OnCalendarGotMouseCapture);
		}
		else
		{
			((UIElement)val).GotMouseCapture -= new MouseEventHandler(OnCalendarGotMouseCapture);
		}
	}

	private static void OnCalendarGotMouseCapture(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((Calendar)sender).SelectionMode != 2)
		{
			object originalSource = ((RoutedEventArgs)e).OriginalSource;
			UIElement val = (UIElement)((originalSource is UIElement) ? originalSource : null);
			if (val is CalendarDayButton || val is CalendarItem)
			{
				val.ReleaseMouseCapture();
			}
		}
	}
}
