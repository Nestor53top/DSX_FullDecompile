using System.Windows;

namespace ModernWpf.Controls;

internal static class Util
{
	public static Visibility VisibilityFromBool(bool visible)
	{
		if (visible)
		{
			return (Visibility)0;
		}
		return (Visibility)2;
	}
}
