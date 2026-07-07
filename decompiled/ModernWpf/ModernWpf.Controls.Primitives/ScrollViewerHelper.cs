using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class ScrollViewerHelper
{
	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsEnabledChanged)));

	public static readonly DependencyProperty AutoHideScrollBarsProperty = DependencyProperty.RegisterAttached("AutoHideScrollBars", typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnAutoHideScrollBarsChanged)));

	public static bool GetIsEnabled(ScrollViewer scrollViewer)
	{
		return (bool)((DependencyObject)scrollViewer).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(ScrollViewer scrollViewer, bool value)
	{
		((DependencyObject)scrollViewer).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		ScrollViewer val = (ScrollViewer)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((FrameworkElement)val).Loaded += new RoutedEventHandler(OnLoaded);
		}
		else
		{
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(OnLoaded);
		}
	}

	public static bool GetAutoHideScrollBars(DependencyObject element)
	{
		return (bool)element.GetValue(AutoHideScrollBarsProperty);
	}

	public static void SetAutoHideScrollBars(DependencyObject element, bool value)
	{
		element.SetValue(AutoHideScrollBarsProperty, (object)value);
	}

	private static void OnAutoHideScrollBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ScrollViewer val = (ScrollViewer)(object)((d is ScrollViewer) ? d : null);
		if (val != null)
		{
			UpdateVisualState(val);
		}
	}

	private static void OnLoaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		ScrollViewer val = (ScrollViewer)sender;
		((FrameworkElement)val).ApplyTemplate();
		UpdateVisualState(val, useTransitions: false);
	}

	private static void UpdateVisualState(ScrollViewer sv, bool useTransitions = true)
	{
		string text = (GetAutoHideScrollBars((DependencyObject)(object)sv) ? "NoIndicator" : "MouseIndicator");
		VisualStateManager.GoToState((FrameworkElement)(object)sv, text, useTransitions);
	}
}
