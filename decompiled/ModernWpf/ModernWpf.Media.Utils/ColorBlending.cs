using System;

namespace ModernWpf.Media.Utils;

internal static class ColorBlending
{
	public const double DefaultSaturationConstant = 18.0;

	public static NormalizedRGB SaturateViaLCH(in NormalizedRGB input, double saturation, double saturationConstant = 18.0)
	{
		LCH lCH = ColorUtils.RGBToLCH(in input, round: false);
		double num = lCH.C + saturation * saturationConstant;
		if (num < 0.0)
		{
			num = 0.0;
		}
		return ColorUtils.LCHToRGB(new LCH(lCH.L, num, lCH.H, round: false), round: false);
	}

	public static NormalizedRGB Blend(in NormalizedRGB bottom, in NormalizedRGB top, ColorBlendMode mode)
	{
		return mode switch
		{
			ColorBlendMode.Burn => BlendBurn(in bottom, in top), 
			ColorBlendMode.Darken => BlendDarken(in bottom, in top), 
			ColorBlendMode.Dodge => BlendDodge(in bottom, in top), 
			ColorBlendMode.Lighten => BlendLighten(in bottom, in top), 
			ColorBlendMode.Multiply => BlendMultiply(in bottom, in top), 
			ColorBlendMode.Overlay => BlendOverlay(in bottom, in top), 
			ColorBlendMode.Screen => BlendScreen(in bottom, in top), 
			_ => throw new ArgumentException("Unknown blend mode", "mode"), 
		};
	}

	public static NormalizedRGB BlendBurn(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendBurn(bottom.R, top.R), BlendBurn(bottom.G, top.G), BlendBurn(bottom.B, top.B), round: false);
	}

	public static double BlendBurn(double bottom, double top)
	{
		if (top == 0.0)
		{
			return 0.0;
		}
		return 1.0 - (1.0 - bottom) / top;
	}

	public static NormalizedRGB BlendDarken(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendDarken(bottom.R, top.R), BlendDarken(bottom.G, top.G), BlendDarken(bottom.B, top.B), round: false);
	}

	public static double BlendDarken(double bottom, double top)
	{
		return Math.Min(bottom, top);
	}

	public static NormalizedRGB BlendDodge(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendDodge(bottom.R, top.R), BlendDodge(bottom.G, top.G), BlendDodge(bottom.B, top.B), round: false);
	}

	public static double BlendDodge(double bottom, double top)
	{
		if (top >= 1.0)
		{
			return 1.0;
		}
		double num = bottom / (1.0 - top);
		if (num >= 1.0)
		{
			return 1.0;
		}
		return num;
	}

	public static NormalizedRGB BlendLighten(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendLighten(bottom.R, top.R), BlendLighten(bottom.G, top.G), BlendLighten(bottom.B, top.B), round: false);
	}

	public static double BlendLighten(double bottom, double top)
	{
		return Math.Max(bottom, top);
	}

	public static NormalizedRGB BlendMultiply(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendMultiply(bottom.R, top.R), BlendMultiply(bottom.G, top.G), BlendMultiply(bottom.B, top.B), round: false);
	}

	public static double BlendMultiply(double bottom, double top)
	{
		return bottom * top;
	}

	public static NormalizedRGB BlendOverlay(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendOverlay(bottom.R, top.R), BlendOverlay(bottom.G, top.G), BlendOverlay(bottom.B, top.B), round: false);
	}

	public static double BlendOverlay(double bottom, double top)
	{
		if (bottom < 0.5)
		{
			return MathUtils.ClampToUnit(2.0 * top * bottom);
		}
		return MathUtils.ClampToUnit(1.0 - 2.0 * (1.0 - top) * (1.0 - bottom));
	}

	public static NormalizedRGB BlendScreen(in NormalizedRGB bottom, in NormalizedRGB top)
	{
		return new NormalizedRGB(BlendScreen(bottom.R, top.R), BlendScreen(bottom.G, top.G), BlendScreen(bottom.B, top.B), round: false);
	}

	public static double BlendScreen(double bottom, double top)
	{
		return 1.0 - (1.0 - top) * (1.0 - bottom);
	}
}
