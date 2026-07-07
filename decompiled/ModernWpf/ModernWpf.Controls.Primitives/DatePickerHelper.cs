using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ModernWpf.Controls.Primitives;

public static class DatePickerHelper
{
	private class FirstNotNullOrEmptyConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			foreach (object obj in values)
			{
				if (obj is string text)
				{
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				else if (obj != null)
				{
					return obj;
				}
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	private static readonly FirstNotNullOrEmptyConverter _watermarkConverter = new FirstNotNullOrEmptyConverter();

	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DatePickerHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

	public static bool GetIsEnabled(DatePicker datePicker)
	{
		return (bool)((DependencyObject)datePicker).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(DatePicker datePicker, bool value)
	{
		((DependencyObject)datePicker).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		DatePicker val = (DatePicker)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((FrameworkElement)val).Loaded += new RoutedEventHandler(OnLoaded);
		}
		else
		{
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(OnLoaded);
		}
	}

	private static void OnLoaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		DatePicker val = (DatePicker)sender;
		((FrameworkElement)val).Loaded -= new RoutedEventHandler(OnLoaded);
		DatePickerTextBox templateChild = ((Control)(object)val).GetTemplateChild<DatePickerTextBox>("PART_TextBox");
		if (templateChild != null)
		{
			ContentControl templateChild2 = ((Control)(object)templateChild).GetTemplateChild<ContentControl>("PART_Watermark");
			if (templateChild2 != null)
			{
				Binding val2 = new Binding
				{
					Path = new PropertyPath((object)ControlHelper.PlaceholderTextProperty),
					Source = val
				};
				BindingExpression bindingExpression = ((FrameworkElement)templateChild2).GetBindingExpression(ContentControl.ContentProperty);
				BindingBase val3 = (BindingBase)((bindingExpression == null) ? ((object)val2) : ((object)new MultiBinding
				{
					Bindings = { (BindingBase)(object)val2 },
					Bindings = { (BindingBase)(object)bindingExpression.ParentBinding },
					Converter = (IMultiValueConverter)(object)_watermarkConverter
				}));
				((FrameworkElement)templateChild2).SetBinding(ContentControl.ContentProperty, val3);
			}
		}
	}
}
