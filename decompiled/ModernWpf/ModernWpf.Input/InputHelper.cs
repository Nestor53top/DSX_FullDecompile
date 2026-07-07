using System;
using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Input;

internal static class InputHelper
{
	public static readonly DependencyProperty IsTapEnabledProperty = DependencyProperty.RegisterAttached("IsTapEnabled", typeof(bool), typeof(InputHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsTapEnabledChanged)));

	public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool), typeof(InputHelper), new PropertyMetadata((object)false));

	public static readonly RoutedEvent TappedEvent = EventManager.RegisterRoutedEvent("Tapped", (RoutingStrategy)1, typeof(TappedEventHandler), typeof(InputHelper));

	private static TappedRoutedEventArgs _lastTappedArgs;

	public static bool GetIsTapEnabled(UIElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsTapEnabledProperty);
	}

	public static void SetIsTapEnabled(UIElement element, bool value)
	{
		((DependencyObject)element).SetValue(IsTapEnabledProperty, (object)value);
	}

	private static void OnIsTapEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		UIElement val = (UIElement)d;
		_ = (bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			val.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
			val.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
			val.LostMouseCapture += new MouseEventHandler(OnLostMouseCapture);
			val.MouseLeave += new MouseEventHandler(OnMouseLeave);
		}
		else
		{
			val.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
			val.MouseLeftButtonUp -= new MouseButtonEventHandler(OnMouseLeftButtonUp);
			val.LostMouseCapture -= new MouseEventHandler(OnLostMouseCapture);
			val.MouseLeave -= new MouseEventHandler(OnMouseLeave);
		}
	}

	private static bool GetIsPressed(UIElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsPressedProperty);
	}

	private static void SetIsPressed(UIElement element, bool value)
	{
		if (value)
		{
			((DependencyObject)element).SetValue(IsPressedProperty, (object)value);
		}
		else
		{
			((DependencyObject)element).ClearValue(IsPressedProperty);
		}
	}

	public static void AddTappedHandler(UIElement element, TappedEventHandler handler)
	{
		element.AddHandler(TappedEvent, (Delegate)handler);
	}

	public static void RemoveTappedHandler(UIElement element, TappedEventHandler handler)
	{
		element.RemoveHandler(TappedEvent, (Delegate)handler);
	}

	private static void RaiseTapped(UIElement element, int timestamp)
	{
		TappedRoutedEventArgs e = new TappedRoutedEventArgs();
		((RoutedEventArgs)e).RoutedEvent = TappedEvent;
		((RoutedEventArgs)e).Source = element;
		e.Timestamp = timestamp;
		element.RaiseEvent((RoutedEventArgs)(object)(_lastTappedArgs = e));
	}

	private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		UIElement element = (UIElement)sender;
		if (!GetIsPressed(element))
		{
			SetIsPressed(element, value: true);
		}
	}

	private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		UIElement val = (UIElement)sender;
		if (!GetIsPressed(val))
		{
			return;
		}
		SetIsPressed((UIElement)sender, value: false);
		TappedRoutedEventArgs lastTappedArgs = _lastTappedArgs;
		if (lastTappedArgs == null || !((RoutedEventArgs)lastTappedArgs).Handled || lastTappedArgs.Timestamp != ((InputEventArgs)e).Timestamp)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(default(Point), val.RenderSize);
			if (((Rect)(ref val2)).Contains(((MouseEventArgs)e).GetPosition((IInputElement)(object)val)))
			{
				RaiseTapped(val, ((InputEventArgs)e).Timestamp);
			}
		}
	}

	private static void OnLostMouseCapture(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		SetIsPressed((UIElement)sender, value: false);
	}

	private static void OnMouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		SetIsPressed((UIElement)sender, value: false);
	}
}
