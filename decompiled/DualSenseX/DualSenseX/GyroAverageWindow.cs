using System;

namespace DualSenseX;

public class GyroAverageWindow
{
	public int x;

	public int y;

	public int z;

	public double accelMagnitude;

	public int numSamples;

	public DateTime start;

	public DateTime stop;

	public int DurationMs => Convert.ToInt32((stop - start).TotalMilliseconds);

	public GyroAverageWindow()
	{
		Reset();
	}

	public void Reset()
	{
		x = (y = (z = (numSamples = 0)));
		accelMagnitude = 0.0;
		start = (stop = DateTime.UtcNow);
	}

	public bool StopIfElapsed(int ms)
	{
		DateTime utcNow = DateTime.UtcNow;
		bool num = Convert.ToInt32((utcNow - start).TotalMilliseconds) >= ms;
		if (!num)
		{
			stop = utcNow;
		}
		return num;
	}

	public double GetWeight(int expectedMs)
	{
		if (expectedMs == 0)
		{
			return 0.0;
		}
		return Math.Min(1.0, DurationMs / expectedMs);
	}
}
