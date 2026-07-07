using System;
using System.Collections.Generic;
using System.Windows.Media;
using ModernWpf.Media.Utils;

namespace ModernWpf.Media.ColorPalette;

internal class ColorPalette
{
	protected readonly IColorPaletteEntry _baseColor;

	protected readonly int _steps = 11;

	protected readonly List<ColorPaletteEntry> _palette;

	protected ColorScaleInterpolationMode _interpolationMode;

	protected Color _scaleColorLight = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	protected Color _scaleColorDark = Color.FromArgb(byte.MaxValue, (byte)0, (byte)0, (byte)0);

	protected double _clipLight = 0.185;

	protected double _clipDark = 0.16;

	protected double _saturationAdjustmentCutoff = 0.05;

	protected double _saturationLight = 0.35;

	protected double _saturationDark = 1.25;

	protected double _overlayLight;

	protected double _overlayDark = 0.25;

	protected double _multiplyLight;

	protected double _multiplyDark;

	public IReadOnlyList<ColorPaletteEntry> Palette => _palette;

	public ColorPalette(int steps, Color baseColor)
		: this(steps, (IColorPaletteEntry)new ColorPaletteEntry(baseColor))
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public ColorPalette(int steps, IColorPaletteEntry baseColor)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (baseColor == null)
		{
			throw new ArgumentNullException("baseColor");
		}
		if (_steps <= 0)
		{
			throw new ArgumentException("steps must > 0");
		}
		_steps = steps;
		_baseColor = baseColor;
		ColorScale paletteScale = GetPaletteScale();
		_palette = new List<ColorPaletteEntry>(_steps);
		for (int i = 0; i < _steps; i++)
		{
			Color color = paletteScale.GetColor((double)i / (double)(_steps - 1), _interpolationMode);
			_palette.Add(new ColorPaletteEntry(color));
		}
	}

	public ColorScale GetPaletteScale()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		Color rgb = _baseColor.ActiveColor;
		HSL hSL = ColorUtils.RGBToHSL(rgb);
		NormalizedRGB bottom = new NormalizedRGB(in rgb);
		ColorScale colorScale = new ColorScale((IEnumerable<Color>)(object)new Color[3] { _scaleColorLight, rgb, _scaleColorDark }).Trim(_clipLight, 1.0 - _clipDark);
		NormalizedRGB normalizedRGB = new NormalizedRGB(colorScale.GetColor(0.0, _interpolationMode));
		NormalizedRGB normalizedRGB2 = new NormalizedRGB(colorScale.GetColor(1.0, _interpolationMode));
		NormalizedRGB input = normalizedRGB;
		NormalizedRGB input2 = normalizedRGB2;
		if (hSL.S >= _saturationAdjustmentCutoff)
		{
			input = ColorBlending.SaturateViaLCH(in input, _saturationLight);
			input2 = ColorBlending.SaturateViaLCH(in input2, _saturationDark);
		}
		if (_multiplyLight != 0.0)
		{
			input = ColorUtils.InterpolateColor(in input, ColorBlending.Blend(in bottom, in input, ColorBlendMode.Multiply), _multiplyLight, _interpolationMode);
		}
		if (_multiplyDark != 0.0)
		{
			input2 = ColorUtils.InterpolateColor(in input2, ColorBlending.Blend(in bottom, in input2, ColorBlendMode.Multiply), _multiplyDark, _interpolationMode);
		}
		if (_overlayLight != 0.0)
		{
			input = ColorUtils.InterpolateColor(in input, ColorBlending.Blend(in bottom, in input, ColorBlendMode.Overlay), _overlayLight, _interpolationMode);
		}
		if (_overlayDark != 0.0)
		{
			input2 = ColorUtils.InterpolateColor(in input2, ColorBlending.Blend(in bottom, in input2, ColorBlendMode.Overlay), _overlayDark, _interpolationMode);
		}
		return new ColorScale((IEnumerable<Color>)(object)new Color[3]
		{
			input.Denormalize(),
			rgb,
			input2.Denormalize()
		});
	}
}
