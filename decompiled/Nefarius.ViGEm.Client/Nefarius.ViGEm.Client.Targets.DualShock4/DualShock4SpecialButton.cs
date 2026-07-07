namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4SpecialButton : DualShock4Button
{
	private class PsButton : DualShock4SpecialButton
	{
		public PsButton()
			: base(0, "PS", 1)
		{
		}
	}

	private class TouchpadButton : DualShock4SpecialButton
	{
		public TouchpadButton()
			: base(1, "Touchpad", 2)
		{
		}
	}

	public static DualShock4SpecialButton Ps = new PsButton();

	public static DualShock4SpecialButton Touchpad = new TouchpadButton();

	protected DualShock4SpecialButton(int id, string name, ushort value)
		: base(id, name, value)
	{
		base.Value = value;
	}
}
