using System;

namespace DualSenseX;

public class SixAxisEventArgs : EventArgs
{
	public readonly SixAxis sixAxis;

	public readonly DateTime timeStamp;

	public SixAxisEventArgs(DateTime utcTimestamp, SixAxis sa)
	{
		sixAxis = sa;
		timeStamp = utcTimestamp;
	}
}
