using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[StyleTypedProperty(Property = "Style", StyleTargetType = typeof(TitleBarControl))]
[StyleTypedProperty(Property = "ButtonStyle", StyleTargetType = typeof(TitleBarButton))]
[StyleTypedProperty(Property = "BackButtonStyle", StyleTargetType = typeof(TitleBarButton))]
public static class TitleBar
{
	private const string StylePropertyName = "Style";

	private const string ButtonStylePropertyName = "ButtonStyle";

	private const string BackButtonStylePropertyName = "BackButtonStyle";

	public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(TitleBar));

	public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(TitleBar));

	public static readonly DependencyProperty InactiveBackgroundProperty = DependencyProperty.RegisterAttached("InactiveBackground", typeof(Brush), typeof(TitleBar));

	public static readonly DependencyProperty InactiveForegroundProperty = DependencyProperty.RegisterAttached("InactiveForeground", typeof(Brush), typeof(TitleBar));

	public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(TitleBar));

	public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.RegisterAttached("ButtonStyle", typeof(Style), typeof(TitleBar));

	public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.RegisterAttached("IsIconVisible", typeof(bool), typeof(TitleBar), new PropertyMetadata((object)false));

	public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.RegisterAttached("IsBackButtonVisible", typeof(bool), typeof(TitleBar));

	public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.RegisterAttached("IsBackEnabled", typeof(bool), typeof(TitleBar), new PropertyMetadata((object)true));

	public static readonly DependencyProperty BackButtonCommandProperty = DependencyProperty.RegisterAttached("BackButtonCommand", typeof(ICommand), typeof(TitleBar));

	public static readonly DependencyProperty BackButtonCommandParameterProperty = DependencyProperty.RegisterAttached("BackButtonCommandParameter", typeof(object), typeof(TitleBar));

	public static readonly DependencyProperty BackButtonCommandTargetProperty = DependencyProperty.RegisterAttached("BackButtonCommandTarget", typeof(IInputElement), typeof(TitleBar));

	public static readonly DependencyProperty BackButtonStyleProperty = DependencyProperty.RegisterAttached("BackButtonStyle", typeof(Style), typeof(TitleBar));

	public static readonly DependencyProperty ExtendViewIntoTitleBarProperty = DependencyProperty.RegisterAttached("ExtendViewIntoTitleBar", typeof(bool), typeof(TitleBar), new PropertyMetadata((object)false));

	internal static readonly DependencyPropertyKey SystemOverlayLeftInsetPropertyKey = DependencyProperty.RegisterAttachedReadOnly("SystemOverlayLeftInset", typeof(double), typeof(TitleBar), new PropertyMetadata((object)0.0));

	public static readonly DependencyProperty SystemOverlayLeftInsetProperty = SystemOverlayLeftInsetPropertyKey.DependencyProperty;

	internal static readonly DependencyPropertyKey SystemOverlayRightInsetPropertyKey = DependencyProperty.RegisterAttachedReadOnly("SystemOverlayRightInset", typeof(double), typeof(TitleBar), new PropertyMetadata((object)0.0));

	public static readonly DependencyProperty SystemOverlayRightInsetProperty = SystemOverlayRightInsetPropertyKey.DependencyProperty;

	internal static readonly DependencyPropertyKey HeightPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Height", typeof(double), typeof(TitleBar), new PropertyMetadata((object)32.0));

	public static readonly DependencyProperty HeightProperty = HeightPropertyKey.DependencyProperty;

	public static readonly RoutedEvent BackRequestedEvent = EventManager.RegisterRoutedEvent("BackRequested", (RoutingStrategy)1, typeof(EventHandler<BackRequestedEventArgs>), typeof(TitleBar));

	public static ComponentResourceKey HeightKey { get; } = new ComponentResourceKey(typeof(TitleBar), (object)"HeightKey");

	public static Brush GetBackground(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)window).GetValue(BackgroundProperty);
	}

	public static void SetBackground(Window window, Brush value)
	{
		((DependencyObject)window).SetValue(BackgroundProperty, (object)value);
	}

	public static Brush GetForeground(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)window).GetValue(ForegroundProperty);
	}

	public static void SetForeground(Window window, Brush value)
	{
		((DependencyObject)window).SetValue(ForegroundProperty, (object)value);
	}

	public static Brush GetInactiveBackground(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)window).GetValue(InactiveBackgroundProperty);
	}

	public static void SetInactiveBackground(Window window, Brush value)
	{
		((DependencyObject)window).SetValue(InactiveBackgroundProperty, (object)value);
	}

	public static Brush GetInactiveForeground(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)window).GetValue(InactiveForegroundProperty);
	}

	public static void SetInactiveForeground(Window window, Brush value)
	{
		((DependencyObject)window).SetValue(InactiveForegroundProperty, (object)value);
	}

	public static Style GetStyle(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)window).GetValue(StyleProperty);
	}

	public static void SetStyle(Window window, Style value)
	{
		((DependencyObject)window).SetValue(StyleProperty, (object)value);
	}

	public static Style GetButtonStyle(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)window).GetValue(ButtonStyleProperty);
	}

	public static void SetButtonStyle(Window window, Style value)
	{
		((DependencyObject)window).SetValue(ButtonStyleProperty, (object)value);
	}

	public static bool GetIsIconVisible(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(IsIconVisibleProperty);
	}

	public static void SetIsIconVisible(Window window, bool value)
	{
		((DependencyObject)window).SetValue(IsIconVisibleProperty, (object)value);
	}

	public static bool GetIsBackButtonVisible(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(IsBackButtonVisibleProperty);
	}

	public static void SetIsBackButtonVisible(Window window, bool value)
	{
		((DependencyObject)window).SetValue(IsBackButtonVisibleProperty, (object)value);
	}

	public static bool GetIsBackEnabled(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(IsBackEnabledProperty);
	}

	public static void SetIsBackEnabled(Window window, bool value)
	{
		((DependencyObject)window).SetValue(IsBackEnabledProperty, (object)value);
	}

	public static ICommand GetBackButtonCommand(Window window)
	{
		return (ICommand)((DependencyObject)window).GetValue(BackButtonCommandProperty);
	}

	public static void SetBackButtonCommand(Window window, ICommand value)
	{
		((DependencyObject)window).SetValue(BackButtonCommandProperty, (object)value);
	}

	public static object GetBackButtonCommandParameter(Window window)
	{
		return ((DependencyObject)window).GetValue(BackButtonCommandParameterProperty);
	}

	public static void SetBackButtonCommandParameter(Window window, object value)
	{
		((DependencyObject)window).SetValue(BackButtonCommandParameterProperty, value);
	}

	public static IInputElement GetBackButtonCommandTarget(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (IInputElement)((DependencyObject)window).GetValue(BackButtonCommandTargetProperty);
	}

	public static void SetBackButtonCommandTarget(Window window, IInputElement value)
	{
		((DependencyObject)window).SetValue(BackButtonCommandTargetProperty, (object)value);
	}

	public static Style GetBackButtonStyle(Window window)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)window).GetValue(BackButtonStyleProperty);
	}

	public static void SetBackButtonStyle(Window window, Style value)
	{
		((DependencyObject)window).SetValue(BackButtonStyleProperty, (object)value);
	}

	public static bool GetExtendViewIntoTitleBar(Window window)
	{
		return (bool)((DependencyObject)window).GetValue(ExtendViewIntoTitleBarProperty);
	}

	public static void SetExtendViewIntoTitleBar(Window window, bool value)
	{
		((DependencyObject)window).SetValue(ExtendViewIntoTitleBarProperty, (object)value);
	}

	public static double GetSystemOverlayLeftInset(Window window)
	{
		return (double)((DependencyObject)window).GetValue(SystemOverlayLeftInsetProperty);
	}

	internal static void SetSystemOverlayLeftInset(Window window, double value)
	{
		((DependencyObject)window).SetValue(SystemOverlayLeftInsetPropertyKey, (object)value);
	}

	public static double GetSystemOverlayRightInset(Window window)
	{
		return (double)((DependencyObject)window).GetValue(SystemOverlayRightInsetProperty);
	}

	internal static void SetSystemOverlayRightInset(Window window, double value)
	{
		((DependencyObject)window).SetValue(SystemOverlayRightInsetPropertyKey, (object)value);
	}

	public static double GetHeight(Window window)
	{
		return (double)((DependencyObject)window).GetValue(HeightProperty);
	}

	internal static void SetHeight(Window window, double value)
	{
		((DependencyObject)window).SetValue(HeightPropertyKey, (object)value);
	}

	public static void AddBackRequestedHandler(Window window, EventHandler<BackRequestedEventArgs> handler)
	{
		((UIElement)window).AddHandler(BackRequestedEvent, (Delegate)handler);
	}

	public static void RemoveBackRequestedHandler(Window window, EventHandler<BackRequestedEventArgs> handler)
	{
		((UIElement)window).RemoveHandler(BackRequestedEvent, (Delegate)handler);
	}

	internal static void RaiseBackRequested(Window window)
	{
		((UIElement)window).RaiseEvent((RoutedEventArgs)(object)new BackRequestedEventArgs(window));
	}
}
