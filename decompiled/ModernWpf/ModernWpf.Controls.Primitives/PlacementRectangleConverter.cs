using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernWpf.Controls.Primitives;

public class PlacementRectangleConverter : IMultiValueConverter
{
	public Thickness Margin { get; set; }

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (values.Length == 2 && values[0] is double num && values[1] is double num2)
		{
			Thickness margin = Margin;
			Point val = default(Point);
			((Point)(ref val))._002Ector(((Thickness)(ref margin)).Left, ((Thickness)(ref margin)).Top);
			Point val2 = default(Point);
			((Point)(ref val2))._002Ector(num - ((Thickness)(ref margin)).Right, num2 - ((Thickness)(ref margin)).Bottom);
			return (object)new Rect(val, val2);
		}
		return Rect.Empty;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
