using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public static class PressHelper
{
	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PressHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

	private static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsPressed", typeof(bool), typeof(PressHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

	public static bool GetIsEnabled(UIElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(UIElement element, bool value)
	{
		((DependencyObject)element).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		UIElement val = (UIElement)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			val.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
			val.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
			val.MouseEnter += new MouseEventHandler(OnMouseEnter);
			val.MouseLeave += new MouseEventHandler(OnMouseLeave);
			UpdateIsPressed(val);
		}
		else
		{
			val.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
			val.MouseLeftButtonUp -= new MouseButtonEventHandler(OnMouseLeftButtonUp);
			val.MouseEnter -= new MouseEventHandler(OnMouseEnter);
			val.MouseLeave -= new MouseEventHandler(OnMouseLeave);
			((DependencyObject)val).ClearValue(IsPressedPropertyKey);
		}
	}

	public static bool GetIsPressed(UIElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsPressedProperty);
	}

	private static void SetIsPressed(UIElement element, bool value)
	{
		((DependencyObject)element).SetValue(IsPressedPropertyKey, (object)value);
	}

	private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateIsPressed((UIElement)sender);
	}

	private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateIsPressed((UIElement)sender);
	}

	private static void OnMouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateIsPressed((UIElement)sender);
	}

	private static void OnMouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateIsPressed((UIElement)sender);
	}

	private static void UpdateIsPressed(UIElement element)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(default(Point), element.RenderSize);
		if ((int)Mouse.LeftButton == 1 && element.IsMouseOver && ((Rect)(ref val)).Contains(Mouse.GetPosition((IInputElement)(object)element)))
		{
			SetIsPressed(element, value: true);
		}
		else
		{
			((DependencyObject)element).ClearValue(IsPressedPropertyKey);
		}
	}
}
