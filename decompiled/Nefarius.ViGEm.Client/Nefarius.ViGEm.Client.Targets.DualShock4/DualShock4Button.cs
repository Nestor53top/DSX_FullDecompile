using System.Runtime.Serialization;

namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4Button : DualShock4Property
{
	private class ThumbRightButton : DualShock4Button
	{
		public ThumbRightButton()
			: base(0, "ThumbRight", 32768)
		{
		}
	}

	private class ThumbLeftButton : DualShock4Button
	{
		public ThumbLeftButton()
			: base(1, "ThumbLeft", 16384)
		{
		}
	}

	private class OptionsButton : DualShock4Button
	{
		public OptionsButton()
			: base(2, "Options", 8192)
		{
		}
	}

	private class ShareButton : DualShock4Button
	{
		public ShareButton()
			: base(3, "Share", 4096)
		{
		}
	}

	private class TriggerRightButton : DualShock4Button
	{
		public TriggerRightButton()
			: base(4, "TriggerRight", 2048)
		{
		}
	}

	private class TriggerLeftButton : DualShock4Button
	{
		public TriggerLeftButton()
			: base(5, "TriggerLeft", 1024)
		{
		}
	}

	private class ShoulderRightButton : DualShock4Button
	{
		public ShoulderRightButton()
			: base(6, "ShoulderRight", 512)
		{
		}
	}

	private class ShoulderLeftButton : DualShock4Button
	{
		public ShoulderLeftButton()
			: base(7, "ShoulderLeft", 256)
		{
		}
	}

	private class TriangleButton : DualShock4Button
	{
		public TriangleButton()
			: base(8, "Triangle", 128)
		{
		}
	}

	private class CircleButton : DualShock4Button
	{
		public CircleButton()
			: base(9, "Circle", 64)
		{
		}
	}

	private class CrossButton : DualShock4Button
	{
		public CrossButton()
			: base(10, "Cross", 32)
		{
		}
	}

	private class SquareButton : DualShock4Button
	{
		public SquareButton()
			: base(11, "Square", 16)
		{
		}
	}

	public static DualShock4Button ThumbRight = new ThumbRightButton();

	public static DualShock4Button ThumbLeft = new ThumbLeftButton();

	public static DualShock4Button Options = new OptionsButton();

	public static DualShock4Button Share = new ShareButton();

	public static DualShock4Button TriggerRight = new TriggerRightButton();

	public static DualShock4Button TriggerLeft = new TriggerLeftButton();

	public static DualShock4Button ShoulderRight = new ShoulderRightButton();

	public static DualShock4Button ShoulderLeft = new ShoulderLeftButton();

	public static DualShock4Button Triangle = new TriangleButton();

	public static DualShock4Button Circle = new CircleButton();

	public static DualShock4Button Cross = new CrossButton();

	public static DualShock4Button Square = new SquareButton();

	[IgnoreDataMember]
	public ushort Value { get; protected set; }

	protected DualShock4Button(int id, string name, ushort value)
		: base(id, name)
	{
		Value = value;
	}
}
