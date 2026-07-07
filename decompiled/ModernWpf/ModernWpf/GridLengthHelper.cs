using System.Windows;

namespace ModernWpf;

internal static class GridLengthHelper
{
	public static GridLength FromPixels(double pixels)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new GridLength(pixels);
	}

	public static GridLength FromValueAndType(double value, GridUnitType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new GridLength(value, type);
	}
}
