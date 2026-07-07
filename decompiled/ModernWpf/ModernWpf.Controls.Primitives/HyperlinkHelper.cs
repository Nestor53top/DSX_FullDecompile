using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public static class HyperlinkHelper
{
	private static readonly DependencyProperty IsPressEnabledProperty = DependencyProperty.RegisterAttached("IsPressEnabled", typeof(bool), typeof(HyperlinkHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsPressEnabledChanged)));

	private static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsPressed", typeof(bool), typeof(HyperlinkHelper), new PropertyMetadata((object)false));

	public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

	public static bool GetIsPressEnabled(Hyperlink hyperlink)
	{
		return (bool)((DependencyObject)hyperlink).GetValue(IsPressEnabledProperty);
	}

	public static void SetIsPressEnabled(Hyperlink hyperlink, bool value)
	{
		((DependencyObject)hyperlink).SetValue(IsPressEnabledProperty, (object)value);
	}

	private static void OnIsPressEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		Hyperlink val = (Hyperlink)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((ContentElement)val).AddHandler(ContentElement.MouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
			((ContentElement)val).AddHandler(ContentElement.MouseLeftButtonUpEvent, (Delegate)new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
		}
		else
		{
			((ContentElement)val).RemoveHandler(ContentElement.MouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(OnMouseLeftButtonDown));
			((ContentElement)val).RemoveHandler(ContentElement.MouseLeftButtonUpEvent, (Delegate)new MouseButtonEventHandler(OnMouseLeftButtonUp));
		}
	}

	private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		Hyperlink val = (Hyperlink)sender;
		if (((ContentElement)val).IsMouseCaptured && (int)e.ButtonState == 1)
		{
			((DependencyObject)val).SetValue(IsPressedPropertyKey, (object)true);
		}
	}

	private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Hyperlink val = (Hyperlink)sender;
		if (GetIsPressed(val))
		{
			((DependencyObject)val).SetValue(IsPressedPropertyKey, (object)false);
		}
	}

	public static bool GetIsPressed(Hyperlink hyperlink)
	{
		return (bool)((DependencyObject)hyperlink).GetValue(IsPressedProperty);
	}
}
