using System.Windows.Media;

namespace ModernWpf.Media.Utils;

internal readonly struct ColorScaleStop
{
	public readonly Color Color;

	public readonly double Position;

	public ColorScaleStop(Color color, double position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		Position = position;
	}

	public ColorScaleStop(ColorScaleStop source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Color = source.Color;
		Position = source.Position;
	}
}
