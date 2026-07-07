using System;

namespace ModernWpf.Media.Utils;

internal readonly struct XYZ : IEquatable<XYZ>
{
	public const int DefaultRoundingPrecision = 5;

	public readonly double X;

	public readonly double Y;

	public readonly double Z;

	public XYZ(double x, double y, double z, bool round = true, int roundingPrecision = 5)
	{
		if (round)
		{
			X = Math.Round(x, roundingPrecision);
			Y = Math.Round(y, roundingPrecision);
			Z = Math.Round(z, roundingPrecision);
		}
		else
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	public bool Equals(XYZ other)
	{
		if (X == other.X && Y == other.Y)
		{
			return Z == other.Z;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is XYZ xYZ)
		{
			if (X == xYZ.X && Y == xYZ.Y)
			{
				return Z == xYZ.Z;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		double x = X;
		int hashCode = x.GetHashCode();
		x = Y;
		int num = hashCode ^ x.GetHashCode();
		x = Z;
		return num ^ x.GetHashCode();
	}

	public override string ToString()
	{
		return $"{X},{Y},{Z}";
	}
}
