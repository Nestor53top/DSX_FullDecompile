using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernWpf.Controls.Primitives;

public class SharedSizeGroupConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)(Visibility)value == 2)
		{
			return null;
		}
		return (string)parameter;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
