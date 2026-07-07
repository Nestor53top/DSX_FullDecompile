namespace Nefarius.ViGEm.Client.Targets.Xbox360;

public abstract class Xbox360Slider : Xbox360Property
{
	private class LeftTriggerSlider : Xbox360Slider
	{
		public LeftTriggerSlider()
			: base(0, "LeftTrigger")
		{
		}
	}

	private class RightTriggerSlider : Xbox360Slider
	{
		public RightTriggerSlider()
			: base(1, "RightTrigger")
		{
		}
	}

	public static Xbox360Slider LeftTrigger = new LeftTriggerSlider();

	public static Xbox360Slider RightTrigger = new RightTriggerSlider();

	protected Xbox360Slider(int id, string name)
		: base(id, name)
	{
	}
}
