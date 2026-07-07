using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class ColumnDefinitionHelper
{
	public static readonly DependencyProperty PixelWidthProperty = DependencyProperty.RegisterAttached("PixelWidth", typeof(double), typeof(ColumnDefinitionHelper), new PropertyMetadata((object)double.NaN, new PropertyChangedCallback(OnPixelWidthChanged)));

	public static double GetPixelWidth(ColumnDefinition columnDefinition)
	{
		return (double)((DependencyObject)columnDefinition).GetValue(PixelWidthProperty);
	}

	public static void SetPixelWidth(ColumnDefinition columnDefinition, double value)
	{
		((DependencyObject)columnDefinition).SetValue(PixelWidthProperty, (object)value);
	}

	private static void OnPixelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		ColumnDefinition val = (ColumnDefinition)d;
		double num = (double)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			((DependencyObject)val).ClearValue(ColumnDefinition.WidthProperty);
		}
		else
		{
			val.Width = new GridLength(num);
		}
	}
}
