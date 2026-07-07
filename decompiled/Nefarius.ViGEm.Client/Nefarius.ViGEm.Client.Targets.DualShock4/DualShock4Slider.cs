namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4Slider : DualShock4Property
{
	private class LeftTriggerSlider : DualShock4Slider
	{
		public LeftTriggerSlider()
			: base(0, "LeftTrigger")
		{
		}
	}

	private class RightTriggerSlider : DualShock4Slider
	{
		public RightTriggerSlider()
			: base(1, "RightTrigger")
		{
		}
	}

	public static DualShock4Slider LeftTrigger = new LeftTriggerSlider();

	public static DualShock4Slider RightTrigger = new RightTriggerSlider();

	protected DualShock4Slider(int id, string name)
		: base(id, name)
	{
	}
}
