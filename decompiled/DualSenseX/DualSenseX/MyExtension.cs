using System;

namespace DualSenseX;

public static class MyExtension
{
	public enum SizeUnits
	{
		Byte,
		KB,
		MB,
		GB,
		TB,
		PB,
		EB,
		ZB,
		YB
	}

	public static string ToSize(this long value, SizeUnits unit)
	{
		return ((double)value / Math.Pow(1024.0, (double)unit)).ToString("0.00");
	}
}
