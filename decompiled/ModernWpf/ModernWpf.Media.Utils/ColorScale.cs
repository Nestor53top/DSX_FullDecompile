using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ModernWpf.Media.Utils;

internal class ColorScale
{
	private readonly ColorScaleStop[] _stops;

	public ColorScale(IEnumerable<Color> colors)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (colors == null)
		{
			throw new ArgumentNullException("colors");
		}
		int num = colors.Count();
		_stops = new ColorScaleStop[num];
		int num2 = 0;
		foreach (Color color in colors)
		{
			if (num2 == 0)
			{
				_stops[num2] = new ColorScaleStop(color, 0.0);
			}
			else if (num2 == num - 1)
			{
				_stops[num2] = new ColorScaleStop(color, 1.0);
			}
			else
			{
				_stops[num2] = new ColorScaleStop(color, (double)num2 * (1.0 / (double)(num - 1)));
			}
			num2++;
		}
	}

	public ColorScale(IEnumerable<ColorScaleStop> stops)
	{
		if (stops == null)
		{
			throw new ArgumentNullException("stops");
		}
		int num = stops.Count();
		_stops = new ColorScaleStop[num];
		int num2 = 0;
		foreach (ColorScaleStop stop in stops)
		{
			_stops[num2] = new ColorScaleStop(stop);
			num2++;
		}
	}

	public Color GetColor(double position, ColorScaleInterpolationMode mode = ColorScaleInterpolationMode.RGB)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		if (_stops.Length == 1)
		{
			return _stops[0].Color;
		}
		if (position <= 0.0)
		{
			return _stops[0].Color;
		}
		if (position >= 1.0)
		{
			return _stops[_stops.Length - 1].Color;
		}
		int num = 0;
		for (int i = 0; i < _stops.Length; i++)
		{
			if (_stops[i].Position <= position)
			{
				num = i;
			}
		}
		int num2 = num + 1;
		if (num2 >= _stops.Length)
		{
			num2 = _stops.Length - 1;
		}
		double position2 = (position - _stops[num].Position) * (1.0 / (_stops[num2].Position - _stops[num].Position));
		return (Color)(mode switch
		{
			ColorScaleInterpolationMode.LAB => ColorUtils.LABToRGB(ColorUtils.InterpolateLAB(ColorUtils.RGBToLAB(_stops[num].Color, round: false), ColorUtils.RGBToLAB(_stops[num2].Color, round: false), position2), round: false).Denormalize(), 
			ColorScaleInterpolationMode.XYZ => ColorUtils.XYZToRGB(ColorUtils.InterpolateXYZ(ColorUtils.RGBToXYZ(_stops[num].Color, round: false), ColorUtils.RGBToXYZ(_stops[num2].Color, round: false), position2), round: false).Denormalize(), 
			_ => ColorUtils.InterpolateRGB(_stops[num].Color, _stops[num2].Color, position2), 
		});
	}

	public ColorScale Trim(double lowerBound, double upperBound, ColorScaleInterpolationMode mode = ColorScaleInterpolationMode.RGB)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (lowerBound < 0.0 || upperBound > 1.0 || upperBound < lowerBound)
		{
			throw new ArgumentException("Invalid bounds");
		}
		if (lowerBound == upperBound)
		{
			return new ColorScale((IEnumerable<Color>)(object)new Color[1] { GetColor(lowerBound, mode) });
		}
		List<ColorScaleStop> list = new List<ColorScaleStop>(_stops.Length);
		for (int i = 0; i < _stops.Length; i++)
		{
			if (_stops[i].Position >= lowerBound && _stops[i].Position <= upperBound)
			{
				list.Add(_stops[i]);
			}
		}
		if (list.Count == 0)
		{
			return new ColorScale((IEnumerable<Color>)(object)new Color[2]
			{
				GetColor(lowerBound, mode),
				GetColor(upperBound, mode)
			});
		}
		if (list.First().Position != lowerBound)
		{
			list.Insert(0, new ColorScaleStop(GetColor(lowerBound, mode), lowerBound));
		}
		if (list.Last().Position != upperBound)
		{
			list.Add(new ColorScaleStop(GetColor(upperBound, mode), upperBound));
		}
		double num = upperBound - lowerBound;
		ColorScaleStop[] array = new ColorScaleStop[list.Count];
		for (int j = 0; j < array.Length; j++)
		{
			double position = (list[j].Position - lowerBound) / num;
			array[j] = new ColorScaleStop(list[j].Color, position);
		}
		return new ColorScale(array);
	}
}
