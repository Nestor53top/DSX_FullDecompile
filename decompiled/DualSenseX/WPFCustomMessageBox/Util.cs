using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFCustomMessageBox;

internal static class Util
{
	internal static ImageSource ToImageSource(this Icon icon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return (ImageSource)(object)Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
	}

	internal static string TryAddKeyboardAccellerator(this string input)
	{
		if (input.Contains("_"))
		{
			return input;
		}
		return "_" + input;
	}
}
