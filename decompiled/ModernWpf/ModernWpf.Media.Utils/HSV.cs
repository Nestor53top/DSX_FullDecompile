using System;

namespace ModernWpf.Media.Utils;

internal readonly struct HSV : IEquatable<HSV>
{
	public readonly double H;

	public readonly double S;

	public readonly double V;

	public bool Equals(HSV other)
	{
		if (H == other.H && S == other.S)
		{
			return V == other.V;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is HSV hSV)
		{
			if (H == hSV.H && S == hSV.S)
			{
				return V == hSV.V;
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
		h = V;
		return num ^ h.GetHashCode();
	}

	public override string ToString()
	{
		return $"{H},{S},{V}";
	}
}
