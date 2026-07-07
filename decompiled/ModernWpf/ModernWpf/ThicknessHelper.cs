using System.Windows;

namespace ModernWpf;

internal static class ThicknessHelper
{
	public static Thickness FromLengths(double left, double top, double right, double bottom)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return new Thickness(left, top, right, bottom);
	}

	public static Thickness FromUniformLength(double uniformLength)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new Thickness(uniformLength);
	}
}
