using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class RowDefinitionHelper
{
	public static readonly DependencyProperty PixelHeightProperty = DependencyProperty.RegisterAttached("PixelHeight", typeof(double), typeof(RowDefinitionHelper), new PropertyMetadata((object)double.NaN, new PropertyChangedCallback(OnPixelHeightChanged)));

	public static double GetPixelHeight(RowDefinition rowDefinition)
	{
		return (double)((DependencyObject)rowDefinition).GetValue(PixelHeightProperty);
	}

	public static void SetPixelHeight(RowDefinition rowDefinition, double value)
	{
		((DependencyObject)rowDefinition).SetValue(PixelHeightProperty, (object)value);
	}

	private static void OnPixelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		RowDefinition val = (RowDefinition)d;
		double num = (double)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			((DependencyObject)val).ClearValue(RowDefinition.HeightProperty);
		}
		else
		{
			val.Height = new GridLength(num);
		}
	}
}
