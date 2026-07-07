using System;

namespace ModernWpf.Media.Utils;

internal readonly struct LAB : IEquatable<LAB>
{
	public const int DefaultRoundingPrecision = 5;

	public readonly double L;

	public readonly double A;

	public readonly double B;

	public LAB(double l, double a, double b, bool round = true, int roundingPrecision = 5)
	{
		if (round)
		{
			L = Math.Round(l, roundingPrecision);
			A = Math.Round(a, roundingPrecision);
			B = Math.Round(b, roundingPrecision);
		}
		else
		{
			L = l;
			A = a;
			B = b;
		}
	}

	public bool Equals(LAB other)
	{
		if (L == other.L && A == other.A)
		{
			return B == other.B;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is LAB lAB)
		{
			if (L == lAB.L && A == lAB.A)
			{
				return B == lAB.B;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		double l = L;
		int hashCode = l.GetHashCode();
		l = A;
		int num = hashCode ^ l.GetHashCode();
		l = B;
		return num ^ l.GetHashCode();
	}

	public override string ToString()
	{
		return $"{L},{A},{B}";
	}
}
