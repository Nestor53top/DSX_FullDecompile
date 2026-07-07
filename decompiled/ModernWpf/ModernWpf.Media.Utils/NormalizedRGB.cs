using System;
using System.Windows.Media;

namespace ModernWpf.Media.Utils;

internal readonly struct NormalizedRGB : IEquatable<NormalizedRGB>
{
	public const int DefaultRoundingPrecision = 5;

	public readonly double R;

	public readonly double G;

	public readonly double B;

	public NormalizedRGB(double r, double g, double b, bool round = true, int roundingPrecision = 5)
	{
		if (round)
		{
			R = Math.Round(r, roundingPrecision);
			G = Math.Round(g, roundingPrecision);
			B = Math.Round(b, roundingPrecision);
		}
		else
		{
			R = r;
			G = g;
			B = b;
		}
	}

	public NormalizedRGB(in Color rgb, bool round = true, int roundingPrecision = 5)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Color val;
		if (round)
		{
			val = rgb;
			R = Math.Round((double)(int)((Color)(ref val)).R / 255.0, roundingPrecision);
			val = rgb;
			G = Math.Round((double)(int)((Color)(ref val)).G / 255.0, roundingPrecision);
			val = rgb;
			B = Math.Round((double)(int)((Color)(ref val)).B / 255.0, roundingPrecision);
		}
		else
		{
			val = rgb;
			R = (double)(int)((Color)(ref val)).R / 255.0;
			val = rgb;
			G = (double)(int)((Color)(ref val)).G / 255.0;
			val = rgb;
			B = (double)(int)((Color)(ref val)).B / 255.0;
		}
	}

	public Color Denormalize(byte a = byte.MaxValue)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		return Color.FromArgb(a, MathUtils.ClampToByte(R * 255.0), MathUtils.ClampToByte(G * 255.0), MathUtils.ClampToByte(B * 255.0));
	}

	public bool Equals(NormalizedRGB other)
	{
		if (R == other.R && G == other.G)
		{
			return B == other.B;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is NormalizedRGB normalizedRGB)
		{
			if (R == normalizedRGB.R && G == normalizedRGB.G)
			{
				return B == normalizedRGB.B;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		double r = R;
		int hashCode = r.GetHashCode();
		r = G;
		int num = hashCode ^ r.GetHashCode();
		r = B;
		return num ^ r.GetHashCode();
	}

	public override string ToString()
	{
		return $"{R},{G},{B}";
	}
}
