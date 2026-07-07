namespace MS.Internal;

internal static class DoubleUtil
{
	public static int DoubleToInt(double val)
	{
		if (!(0.0 < val))
		{
			return (int)(val - 0.5);
		}
		return (int)(val + 0.5);
	}
}
