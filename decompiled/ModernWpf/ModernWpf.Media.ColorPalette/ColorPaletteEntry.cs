using System.Windows.Media;

namespace ModernWpf.Media.ColorPalette;

internal class ColorPaletteEntry : IColorPaletteEntry
{
	public Color ActiveColor { get; }

	public ColorPaletteEntry(Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ActiveColor = color;
	}
}
