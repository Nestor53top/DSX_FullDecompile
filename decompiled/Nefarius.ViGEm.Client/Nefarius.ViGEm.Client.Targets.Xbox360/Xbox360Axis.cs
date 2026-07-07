namespace Nefarius.ViGEm.Client.Targets.Xbox360;

public abstract class Xbox360Axis : Xbox360Property
{
	private class LeftThumbXAxis : Xbox360Axis
	{
		public LeftThumbXAxis()
			: base(0, "LeftThumbX")
		{
		}
	}

	private class LeftThumbYAxis : Xbox360Axis
	{
		public LeftThumbYAxis()
			: base(1, "LeftThumbY")
		{
		}
	}

	private class RightThumbXAxis : Xbox360Axis
	{
		public RightThumbXAxis()
			: base(2, "RightThumbX")
		{
		}
	}

	private class RightThumbYAxis : Xbox360Axis
	{
		public RightThumbYAxis()
			: base(3, "RightThumbY")
		{
		}
	}

	public static Xbox360Axis LeftThumbX = new LeftThumbXAxis();

	public static Xbox360Axis LeftThumbY = new LeftThumbYAxis();

	public static Xbox360Axis RightThumbX = new RightThumbXAxis();

	public static Xbox360Axis RightThumbY = new RightThumbYAxis();

	protected Xbox360Axis(int id, string name)
		: base(id, name)
	{
	}
}
