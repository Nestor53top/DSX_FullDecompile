using System;

namespace ModernWpf.Media.Utils;

internal static class MathUtils
{
	public static byte ClampToByte(double c)
	{
		if (double.IsNaN(c))
		{
			return 0;
		}
		if (double.IsPositiveInfinity(c))
		{
			return byte.MaxValue;
		}
		if (double.IsNegativeInfinity(c))
		{
			return 0;
		}
		c = Math.Round(c);
		if (c <= 0.0)
		{
			return 0;
		}
		if (c >= 255.0)
		{
			return byte.MaxValue;
		}
		return (byte)c;
	}

	public static double ClampToUnit(double c)
	{
		if (double.IsNaN(c))
		{
			return 0.0;
		}
		if (double.IsPositiveInfinity(c))
		{
			return 1.0;
		}
		if (double.IsNegativeInfinity(c))
		{
			return 0.0;
		}
		if (c <= 0.0)
		{
			return 0.0;
		}
		if (c >= 1.0)
		{
			return 1.0;
		}
		return c;
	}

	public static double DegreesToRadians(double degrees)
	{
		return degrees * (Math.PI / 180.0);
	}

	public static double RadiansToDegrees(double radians)
	{
		return radians * (180.0 / Math.PI);
	}

	public static double Lerp(double left, double right, double scale)
	{
		if (scale <= 0.0)
		{
			return left;
		}
		if (scale >= 1.0)
		{
			return right;
		}
		return left + scale * (right - left);
	}

	public static byte Lerp(byte left, byte right, double scale)
	{
		if (scale <= 0.0)
		{
			return left;
		}
		if (scale >= 1.0)
		{
			return right;
		}
		if (left == right)
		{
			return left;
		}
		double num = (int)left;
		double num2 = (int)right;
		return (byte)Math.Round(num + scale * (num2 - num));
	}
}
