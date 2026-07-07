using System;

namespace ModernWpf.Media.Utils;

internal readonly struct LCH : IEquatable<LCH>
{
	public const int DefaultRoundingPrecision = 5;

	public readonly double L;

	public readonly double C;

	public readonly double H;

	public LCH(double l, double c, double h, bool round = true, int roundingPrecision = 5)
	{
		if (round)
		{
			L = Math.Round(l, roundingPrecision);
			C = Math.Round(c, roundingPrecision);
			H = Math.Round(h, roundingPrecision);
		}
		else
		{
			L = l;
			C = c;
			H = h;
		}
	}

	public bool Equals(LCH other)
	{
		if (L == other.L && C == other.C)
		{
			return H == other.H;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is LCH lCH)
		{
			if (L == lCH.L && C == lCH.C)
			{
				return H == lCH.H;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		double l = L;
		int hashCode = l.GetHashCode();
		l = C;
		int num = hashCode ^ l.GetHashCode();
		l = H;
		return num ^ l.GetHashCode();
	}

	public override string ToString()
	{
		return $"{L},{C},{H}";
	}
}
