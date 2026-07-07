using System;
using System.Windows.Media;

namespace ModernWpf.Media.Utils;

internal static class ColorUtils
{
	public const int DefaultRoundingPrecision = 5;

	public static HSL RGBToHSL(Color rgb, bool round = true, int roundingPrecision = 5)
	{
		return RGBToHSL(new NormalizedRGB(in rgb, round: false), round, roundingPrecision);
	}

	public static HSL RGBToHSL(in NormalizedRGB rgb, bool round = true, int roundingPrecision = 5)
	{
		double num = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
		double num2 = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
		double num3 = num - num2;
		double num4 = 0.0;
		num4 = ((num3 == 0.0) ? 0.0 : ((num == rgb.R) ? (60.0 * ((rgb.G - rgb.B) / num3 % 6.0)) : ((num != rgb.G) ? (60.0 * ((rgb.R - rgb.G) / num3 + 4.0)) : (60.0 * ((rgb.B - rgb.R) / num3 + 2.0)))));
		if (num4 < 0.0)
		{
			num4 += 360.0;
		}
		double num5 = (num + num2) / 2.0;
		double s = 0.0;
		if (num3 != 0.0)
		{
			s = num3 / (1.0 - Math.Abs(2.0 * num5 - 1.0));
		}
		return new HSL(num4, s, num5, round, roundingPrecision);
	}

	public static LAB LCHToLAB(in LCH lch, bool round = true, int roundingPrecision = 5)
	{
		double a = 0.0;
		double b = 0.0;
		if (lch.H != 0.0)
		{
			a = Math.Cos(MathUtils.DegreesToRadians(lch.H)) * lch.C;
			b = Math.Sin(MathUtils.DegreesToRadians(lch.H)) * lch.C;
		}
		return new LAB(lch.L, a, b, round, roundingPrecision);
	}

	public static LCH LABToLCH(in LAB lab, bool round = true, int roundingPrecision = 5)
	{
		double h = (MathUtils.RadiansToDegrees(Math.Atan2(lab.B, lab.A)) + 360.0) % 360.0;
		double c = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
		return new LCH(lab.L, c, h, round, roundingPrecision);
	}

	public static XYZ LABToXYZ(in LAB lab, bool round = true, int roundingPrecision = 5)
	{
		double num = (lab.L + 16.0) / 116.0;
		double i = num + lab.A / 500.0;
		double i2 = num - lab.B / 200.0;
		i = 0.95047 * LABToXYZHelper(i);
		num = LABToXYZHelper(num);
		i2 = 1.08883 * LABToXYZHelper(i2);
		return new XYZ(i, num, i2, round, roundingPrecision);
		static double LABToXYZHelper(double num2)
		{
			if (num2 > 0.206896552)
			{
				return Math.Pow(num2, 3.0);
			}
			return 0.12841855 * (num2 - 0.137931034);
		}
	}

	public static LAB XYZToLAB(in XYZ xyz, bool round = true, int roundingPrecision = 5)
	{
		double num = XYZToLABHelper(xyz.X / 0.95047);
		double num2 = XYZToLABHelper(xyz.Y);
		double num3 = XYZToLABHelper(xyz.Z / 1.08883);
		double l = 116.0 * num2 - 16.0;
		double a = 500.0 * (num - num2);
		double b = -200.0 * (num3 - num2);
		return new LAB(l, a, b, round, roundingPrecision);
		static double XYZToLABHelper(double i)
		{
			if (i > 0.008856452)
			{
				return Math.Pow(i, 1.0 / 3.0);
			}
			return i / 0.12841855 + 0.137931034;
		}
	}

	public static XYZ RGBToXYZ(Color rgb, bool round = true, int roundingPrecision = 5)
	{
		return RGBToXYZ(new NormalizedRGB(in rgb, round: false), round, roundingPrecision);
	}

	public static XYZ RGBToXYZ(in NormalizedRGB rgb, bool round = true, int roundingPrecision = 5)
	{
		double num = RGBToXYZHelper(rgb.R);
		double num2 = RGBToXYZHelper(rgb.G);
		double num3 = RGBToXYZHelper(rgb.B);
		double x = num * 0.4124564 + num2 * 0.3575761 + num3 * 0.1804375;
		double y = num * 0.2126729 + num2 * 0.7151522 + num3 * 0.072175;
		double z = num * 0.0193339 + num2 * 0.119192 + num3 * 0.9503041;
		return new XYZ(x, y, z, round, roundingPrecision);
		static double RGBToXYZHelper(double i)
		{
			if (i <= 0.04045)
			{
				return i / 12.92;
			}
			return Math.Pow((i + 0.055) / 1.055, 2.4);
		}
	}

	public static NormalizedRGB XYZToRGB(in XYZ xyz, bool round = true, int roundingPrecision = 5)
	{
		double c = XYZToRGBHelper(xyz.X * 3.2404542 - xyz.Y * 1.5371385 - xyz.Z * 0.4985314);
		double c2 = XYZToRGBHelper(xyz.X * -0.969266 + xyz.Y * 1.8760108 + xyz.Z * 0.041556);
		return new NormalizedRGB(b: MathUtils.ClampToUnit(XYZToRGBHelper(xyz.X * 0.0556434 - xyz.Y * 0.2040259 + xyz.Z * 1.0572252)), r: MathUtils.ClampToUnit(c), g: MathUtils.ClampToUnit(c2), round: round, roundingPrecision: roundingPrecision);
		static double XYZToRGBHelper(double i)
		{
			if (i <= 0.0031308)
			{
				return i * 12.92;
			}
			return 1.055 * Math.Pow(i, 5.0 / 12.0) - 0.055;
		}
	}

	public static LAB RGBToLAB(Color rgb, bool round = true, int roundingPrecision = 5)
	{
		return RGBToLAB(new NormalizedRGB(in rgb, round: false), round, roundingPrecision);
	}

	public static LAB RGBToLAB(in NormalizedRGB rgb, bool round = true, int roundingPrecision = 5)
	{
		return XYZToLAB(RGBToXYZ(in rgb, round: false), round, roundingPrecision);
	}

	public static NormalizedRGB LABToRGB(in LAB lab, bool round = true, int roundingPrecision = 5)
	{
		return XYZToRGB(LABToXYZ(in lab, round: false), round, roundingPrecision);
	}

	public static LCH RGBToLCH(in NormalizedRGB rgb, bool round = true, int roundingPrecision = 5)
	{
		LAB lAB = RGBToLAB(in rgb, round: true, 4);
		double l = ((lAB.L == 0.0) ? 0.0 : lAB.L);
		double a = ((lAB.A == 0.0) ? 0.0 : lAB.A);
		double b = ((lAB.B == 0.0) ? 0.0 : lAB.B);
		return LABToLCH(new LAB(l, a, b, round: false), round, roundingPrecision);
	}

	public static NormalizedRGB LCHToRGB(in LCH lch, bool round = true, int roundingPrecision = 5)
	{
		return LABToRGB(LCHToLAB(in lch, round: false), round, roundingPrecision);
	}

	public static Color InterpolateRGB(Color left, Color right, double position)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (position <= 0.0)
		{
			return left;
		}
		if (position >= 1.0)
		{
			return right;
		}
		return Color.FromArgb(MathUtils.Lerp(((Color)(ref left)).A, ((Color)(ref right)).A, position), MathUtils.Lerp(((Color)(ref left)).R, ((Color)(ref right)).R, position), MathUtils.Lerp(((Color)(ref left)).G, ((Color)(ref right)).G, position), MathUtils.Lerp(((Color)(ref left)).B, ((Color)(ref right)).B, position));
	}

	public static NormalizedRGB InterpolateRGB(in NormalizedRGB left, in NormalizedRGB right, double position)
	{
		if (position <= 0.0)
		{
			return left;
		}
		if (position >= 1.0)
		{
			return right;
		}
		return new NormalizedRGB(MathUtils.Lerp(left.R, right.R, position), MathUtils.Lerp(left.G, right.G, position), MathUtils.Lerp(left.B, right.B, position), round: false);
	}

	public static LAB InterpolateLAB(in LAB left, in LAB right, double position)
	{
		if (position <= 0.0)
		{
			return left;
		}
		if (position >= 1.0)
		{
			return right;
		}
		return new LAB(MathUtils.Lerp(left.L, right.L, position), MathUtils.Lerp(left.A, right.A, position), MathUtils.Lerp(left.B, right.B, position), round: false);
	}

	public static XYZ InterpolateXYZ(in XYZ left, in XYZ right, double position)
	{
		if (position <= 0.0)
		{
			return left;
		}
		if (position >= 1.0)
		{
			return right;
		}
		return new XYZ(MathUtils.Lerp(left.X, right.X, position), MathUtils.Lerp(left.Y, right.Y, position), MathUtils.Lerp(left.Z, right.Z, position), round: false);
	}

	public static NormalizedRGB InterpolateColor(in NormalizedRGB left, in NormalizedRGB right, double position, ColorScaleInterpolationMode mode)
	{
		return mode switch
		{
			ColorScaleInterpolationMode.LAB => LABToRGB(InterpolateLAB(RGBToLAB(in left, round: false), RGBToLAB(in right, round: false), position)), 
			ColorScaleInterpolationMode.XYZ => XYZToRGB(InterpolateXYZ(RGBToXYZ(in left, round: false), RGBToXYZ(in right, round: false), position)), 
			_ => InterpolateRGB(in left, in right, position), 
		};
	}
}
