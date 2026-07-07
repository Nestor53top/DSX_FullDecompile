using System.Runtime.Serialization;

namespace Nefarius.ViGEm.Client.Targets.Xbox360;

public abstract class Xbox360Button : Xbox360Property
{
	private class UpButton : Xbox360Button
	{
		public UpButton()
			: base(0, "Up", 1)
		{
		}
	}

	private class DownButton : Xbox360Button
	{
		public DownButton()
			: base(1, "Down", 2)
		{
		}
	}

	private class LeftButton : Xbox360Button
	{
		public LeftButton()
			: base(2, "Left", 4)
		{
		}
	}

	private class RightButton : Xbox360Button
	{
		public RightButton()
			: base(3, "Right", 8)
		{
		}
	}

	private class StartButton : Xbox360Button
	{
		public StartButton()
			: base(4, "Start", 16)
		{
		}
	}

	private class BackButton : Xbox360Button
	{
		public BackButton()
			: base(5, "Back", 32)
		{
		}
	}

	private class LeftThumbButton : Xbox360Button
	{
		public LeftThumbButton()
			: base(6, "LeftThumb", 64)
		{
		}
	}

	private class RightThumbButton : Xbox360Button
	{
		public RightThumbButton()
			: base(7, "RightThumb", 128)
		{
		}
	}

	private class LeftShoulderButton : Xbox360Button
	{
		public LeftShoulderButton()
			: base(8, "LeftShoulder", 256)
		{
		}
	}

	private class RightShoulderButton : Xbox360Button
	{
		public RightShoulderButton()
			: base(9, "RightShoulder", 512)
		{
		}
	}

	private class GuideButton : Xbox360Button
	{
		public GuideButton()
			: base(10, "Guide", 1024)
		{
		}
	}

	private class AButton : Xbox360Button
	{
		public AButton()
			: base(11, "A", 4096)
		{
		}
	}

	private class BButton : Xbox360Button
	{
		public BButton()
			: base(12, "B", 8192)
		{
		}
	}

	private class XButton : Xbox360Button
	{
		public XButton()
			: base(13, "X", 16384)
		{
		}
	}

	private class YButton : Xbox360Button
	{
		public YButton()
			: base(14, "Y", 32768)
		{
		}
	}

	public static Xbox360Button Up = new UpButton();

	public static Xbox360Button Down = new DownButton();

	public static Xbox360Button Left = new LeftButton();

	public static Xbox360Button Right = new RightButton();

	public static Xbox360Button Start = new StartButton();

	public static Xbox360Button Back = new BackButton();

	public static Xbox360Button LeftThumb = new LeftThumbButton();

	public static Xbox360Button RightThumb = new RightThumbButton();

	public static Xbox360Button LeftShoulder = new LeftShoulderButton();

	public static Xbox360Button RightShoulder = new RightShoulderButton();

	public static Xbox360Button Guide = new GuideButton();

	public static Xbox360Button A = new AButton();

	public static Xbox360Button B = new BButton();

	public static Xbox360Button X = new XButton();

	public static Xbox360Button Y = new YButton();

	[IgnoreDataMember]
	public ushort Value { get; }

	protected Xbox360Button(int id, string name, ushort value)
		: base(id, name)
	{
		Value = value;
	}
}
