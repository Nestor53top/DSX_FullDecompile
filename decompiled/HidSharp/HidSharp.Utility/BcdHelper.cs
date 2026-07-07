using System;

namespace HidSharp.Utility;

internal static class BcdHelper
{
	public static int FromVersion(Version version)
	{
		Throw.If.Null(version);
		return (version.Major / 10 << 12) | (version.Major % 10 << 8) | (version.Minor / 10 << 4) | (version.Minor % 10);
	}

	public static Version ToVersion(int bcd)
	{
		Throw.If.False(bcd >= 0 && bcd <= 65535);
		return new Version(((bcd >> 12) & 0xF) * 10 + ((bcd >> 8) & 0xF), ((bcd >> 4) & 0xF) * 10 + (bcd & 0xF));
	}
}
