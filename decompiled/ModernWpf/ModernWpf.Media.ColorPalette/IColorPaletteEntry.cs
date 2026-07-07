using System.Windows.Media;

namespace ModernWpf.Media.ColorPalette;

internal interface IColorPaletteEntry
{
	Color ActiveColor { get; }
}
