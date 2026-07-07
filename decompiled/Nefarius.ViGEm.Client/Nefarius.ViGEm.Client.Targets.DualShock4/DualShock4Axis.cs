namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4Axis : DualShock4Property
{
	private class LeftThumbXAxis : DualShock4Axis
	{
		public LeftThumbXAxis()
			: base(0, "LeftThumbX")
		{
		}
	}

	private class LeftThumbYAxis : DualShock4Axis
	{
		public LeftThumbYAxis()
			: base(1, "LeftThumbY")
		{
		}
	}

	private class RightThumbXAxis : DualShock4Axis
	{
		public RightThumbXAxis()
			: base(2, "RightThumbX")
		{
		}
	}

	private class RightThumbYAxis : DualShock4Axis
	{
		public RightThumbYAxis()
			: base(3, "RightThumbY")
		{
		}
	}

	public static DualShock4Axis LeftThumbX = new LeftThumbXAxis();

	public static DualShock4Axis LeftThumbY = new LeftThumbYAxis();

	public static DualShock4Axis RightThumbX = new RightThumbXAxis();

	public static DualShock4Axis RightThumbY = new RightThumbYAxis();

	protected DualShock4Axis(int id, string name)
		: base(id, name)
	{
	}
}
