using System;

namespace Microsoft.AppCenter.Utils;

public static class TimeHelper
{
	public static long CurrentTimeInMilliseconds()
	{
		return DateTime.Now.Ticks / 10000;
	}
}
