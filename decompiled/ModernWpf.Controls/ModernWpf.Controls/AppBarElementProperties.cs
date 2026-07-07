using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls;

internal static class AppBarElementProperties
{
	public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(IconElement), typeof(AppBarElementProperties), new PropertyMetadata(new PropertyChangedCallback(OnIconChanged)));

	public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), typeof(AppBarElementProperties), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnLabelChanged), new CoerceValueCallback(CoerceLabel)));

	public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.RegisterAttached("LabelPosition", typeof(CommandBarLabelPosition), typeof(AppBarElementProperties), new PropertyMetadata((object)CommandBarLabelPosition.Default, new PropertyChangedCallback(OnLabelPositionChanged)));

	public static readonly DependencyProperty IsCompactProperty = DependencyProperty.RegisterAttached("IsCompact", typeof(bool), typeof(AppBarElementProperties), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsCompactChanged)));

	internal static readonly DependencyPropertyKey IsInOverflowPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsInOverflow", typeof(bool), typeof(AppBarElementProperties), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsInOverflowChanged)));

	public static readonly DependencyProperty IsInOverflowProperty = IsInOverflowPropertyKey.DependencyProperty;

	internal static readonly DependencyPropertyKey ApplicationViewStatePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ApplicationViewState", typeof(AppBarElementApplicationViewState), typeof(AppBarElementProperties), new PropertyMetadata((object)AppBarElementApplicationViewState.FullSize, new PropertyChangedCallback(OnApplicationViewStateChanged)));

	internal static readonly DependencyProperty ApplicationViewStateProperty = ApplicationViewStatePropertyKey.DependencyProperty;

	public static readonly DependencyProperty InputGestureTextProperty = DependencyProperty.RegisterAttached("InputGestureText", typeof(string), typeof(AppBarElementProperties), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnInputGestureTextChanged), new CoerceValueCallback(CoerceInputGestureText)));

	private static readonly DependencyPropertyKey HasInputGestureTextPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasInputGestureText", typeof(bool), typeof(AppBarElementProperties), new PropertyMetadata((object)false, new PropertyChangedCallback(OnHasInputGestureTextChanged)));

	public static readonly DependencyProperty HasInputGestureTextProperty = HasInputGestureTextPropertyKey.DependencyProperty;

	internal static readonly DependencyProperty ShowKeyboardAcceleratorTextProperty = DependencyProperty.RegisterAttached("ShowKeyboardAcceleratorText", typeof(bool), typeof(AppBarElementProperties));

	private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as IAppBarElement)?.UpdateApplicationViewState();
	}

	private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		DependencyObject obj = ((d is FrameworkElement) ? d : null);
		if (obj != null)
		{
			obj.CoerceValue(FrameworkElement.ToolTipProperty);
		}
	}

	private static object CoerceLabel(DependencyObject d, object value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ButtonBase val = (ButtonBase)d;
		if (string.IsNullOrEmpty(value as string) && !((DependencyObject)(object)val).HasNonDefaultValue(LabelProperty))
		{
			ICommand command = val.Command;
			RoutedUICommand val2 = (RoutedUICommand)((command is RoutedUICommand) ? command : null);
			if (val2 != null)
			{
				value = val2.Text;
			}
			return value;
		}
		return value;
	}

	private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as IAppBarElement)?.UpdateApplicationViewState();
	}

	private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as IAppBarElement)?.UpdateApplicationViewState();
	}

	private static void OnIsInOverflowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as IAppBarElement)?.UpdateApplicationViewState();
		UpdateShowKeyboardAcceleratorText((FrameworkElement)(object)((d is FrameworkElement) ? d : null));
		DependencyObject obj = ((d is FrameworkElement) ? d : null);
		if (obj != null)
		{
			obj.CoerceValue(FrameworkElement.ToolTipProperty);
		}
	}

	internal static void UpdateIsInOverflow(DependencyObject element)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		bool flag = ToolBar.GetIsOverflowItem(element) || (int)ToolBar.GetOverflowMode(element) == 1;
		element.SetValue(IsInOverflowPropertyKey, (object)flag);
	}

	private static void OnApplicationViewStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as IAppBarElement)?.ApplyApplicationViewState();
	}

	private static void OnInputGestureTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		UpdateHasInputGestureText(d, (string)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		DependencyObject obj = ((d is FrameworkElement) ? d : null);
		if (obj != null)
		{
			obj.CoerceValue(FrameworkElement.ToolTipProperty);
		}
	}

	private static object CoerceInputGestureText(DependencyObject d, object value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ButtonBase val = (ButtonBase)d;
		if (string.IsNullOrEmpty((string)value) && !((DependencyObject)(object)val).HasNonDefaultValue(InputGestureTextProperty))
		{
			ICommand command = val.Command;
			RoutedCommand val2;
			if ((val2 = (RoutedCommand)((command is RoutedCommand) ? command : null)) != null)
			{
				InputGestureCollection inputGestures = val2.InputGestures;
				if (inputGestures != null && inputGestures.Count >= 1)
				{
					for (int i = 0; i < inputGestures.Count; i++)
					{
						object? obj = ((IList)inputGestures)[i];
						KeyGesture val3 = (KeyGesture)((obj is KeyGesture) ? obj : null);
						if (val3 != null)
						{
							return val3.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
						}
					}
				}
			}
		}
		return value;
	}

	private static void OnHasInputGestureTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		UpdateShowKeyboardAcceleratorText((FrameworkElement)(object)((d is FrameworkElement) ? d : null));
	}

	private static void UpdateHasInputGestureText(DependencyObject element, string inputGestureText)
	{
		element.SetValue(HasInputGestureTextPropertyKey, (object)(!string.IsNullOrEmpty(inputGestureText)));
	}

	internal static bool GetShowKeyboardAcceleratorText(DependencyObject element)
	{
		return (bool)element.GetValue(ShowKeyboardAcceleratorTextProperty);
	}

	private static void SetShowKeyboardAcceleratorText(DependencyObject element, bool value)
	{
		element.SetValue(ShowKeyboardAcceleratorTextProperty, (object)value);
	}

	private static void UpdateShowKeyboardAcceleratorText(FrameworkElement element)
	{
		if (element != null)
		{
			bool value = (bool)((DependencyObject)element).GetValue(HasInputGestureTextProperty) && (bool)((DependencyObject)element).GetValue(IsInOverflowProperty);
			SetShowKeyboardAcceleratorText((DependencyObject)(object)element, value);
		}
	}

	internal static object CoerceToolTip(DependencyObject d, object baseValue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ButtonBase val = (ButtonBase)d;
		if (baseValue == null && ((DependencyObject)(object)val).HasDefaultValue(FrameworkElement.ToolTipProperty) && (bool)((DependencyObject)val).GetValue(HasInputGestureTextProperty) && !(bool)((DependencyObject)val).GetValue(IsInOverflowProperty))
		{
			string obj = (string)((DependencyObject)val).GetValue(LabelProperty);
			string text = (string)((DependencyObject)val).GetValue(InputGestureTextProperty);
			return (obj + " (" + text + ")").Trim();
		}
		return baseValue;
	}
}
