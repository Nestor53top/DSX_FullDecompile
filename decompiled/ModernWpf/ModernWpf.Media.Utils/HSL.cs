using System;

namespace ModernWpf.Media.Utils;

internal readonly struct HSL : IEquatable<HSL>
{
	public const int DefaultRoundingPrecision = 5;

	public readonly double H;

	public readonly double S;

	public readonly double L;

	public HSL(double h, double s, double l, bool round = true, int roundingPrecision = 5)
	{
		if (round)
		{
			H = Math.Round(h, roundingPrecision);
			S = Math.Round(s, roundingPrecision);
			L = Math.Round(l, roundingPrecision);
		}
		else
		{
			H = h;
			S = s;
			L = l;
		}
	}

	public bool Equals(HSL other)
	{
		if (H == other.H && S == other.S)
		{
			return L == other.L;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is HSL hSL)
		{
			if (H == hSL.H && S == hSL.S)
			{
				return L == hSL.L;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		double h = H;
		int hashCode = h.GetHashCode();
		h = S;
		int num = hashCode ^ h.GetHashCode();
		h = L;
		return num ^ h.GetHashCode();
	}

	public override string ToString()
	{
		return $"{H},{S},{L}";
	}
}
