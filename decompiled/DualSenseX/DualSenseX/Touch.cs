namespace DualSenseX;

public class Touch
{
	public readonly int hwX;

	public readonly int hwY;

	public readonly int deltaX;

	public readonly int deltaY;

	public readonly byte touchID;

	public readonly Touch previousTouch;

	public Touch(int X, int Y, byte tID, Touch prevTouch = null)
	{
		hwX = X;
		hwY = Y;
		touchID = tID;
		previousTouch = prevTouch;
		if (previousTouch != null)
		{
			deltaX = X - previousTouch.hwX;
			deltaY = Y - previousTouch.hwY;
		}
	}
}
