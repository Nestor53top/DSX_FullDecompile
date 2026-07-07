using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernWpf.Controls.Primitives;

public class CornerRadiusFilterConverter : DependencyObject, IValueConverter
{
	public CornerRadiusFilterKind Filter { get; set; }

	public double Scale { get; set; } = 1.0;

	public CornerRadius Convert(CornerRadius radius, CornerRadiusFilterKind filterKind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius result = radius;
		switch (filterKind)
		{
		case CornerRadiusFilterKind.Top:
			((CornerRadius)(ref result)).BottomLeft = 0.0;
			((CornerRadius)(ref result)).BottomRight = 0.0;
			break;
		case CornerRadiusFilterKind.Right:
			((CornerRadius)(ref result)).TopLeft = 0.0;
			((CornerRadius)(ref result)).BottomLeft = 0.0;
			break;
		case CornerRadiusFilterKind.Bottom:
			((CornerRadius)(ref result)).TopLeft = 0.0;
			((CornerRadius)(ref result)).TopRight = 0.0;
			break;
		case CornerRadiusFilterKind.Left:
			((CornerRadius)(ref result)).TopRight = 0.0;
			((CornerRadius)(ref result)).BottomRight = 0.0;
			break;
		}
		return result;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius radius = (CornerRadius)value;
		double scale = Scale;
		if (!double.IsNaN(scale))
		{
			((CornerRadius)(ref radius)).TopLeft = ((CornerRadius)(ref radius)).TopLeft * scale;
			((CornerRadius)(ref radius)).TopRight = ((CornerRadius)(ref radius)).TopRight * scale;
			((CornerRadius)(ref radius)).BottomRight = ((CornerRadius)(ref radius)).BottomRight * scale;
			((CornerRadius)(ref radius)).BottomLeft = ((CornerRadius)(ref radius)).BottomLeft * scale;
		}
		CornerRadiusFilterKind filter = Filter;
		if (filter == CornerRadiusFilterKind.TopLeftValue || filter == CornerRadiusFilterKind.BottomRightValue)
		{
			return GetDoubleValue(radius, filter);
		}
		return Convert(radius, filter);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	private double GetDoubleValue(CornerRadius radius, CornerRadiusFilterKind filterKind)
	{
		return filterKind switch
		{
			CornerRadiusFilterKind.TopLeftValue => ((CornerRadius)(ref radius)).TopLeft, 
			CornerRadiusFilterKind.BottomRightValue => ((CornerRadius)(ref radius)).BottomRight, 
			_ => 0.0, 
		};
	}
}
