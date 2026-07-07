using System;

namespace DualSenseX;

public class TouchpadEventArgs : EventArgs
{
	public readonly Touch[] touches;

	public readonly bool touchButtonPressed;

	public TouchpadEventArgs(bool tButtonDown, Touch t0, Touch t1 = null)
	{
		if (t1 != null)
		{
			touches = new Touch[2];
			touches[0] = t0;
			touches[1] = t1;
		}
		else if (t0 != null)
		{
			touches = new Touch[1];
			touches[0] = t0;
		}
		touchButtonPressed = tButtonDown;
	}
}
