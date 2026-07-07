using System.Runtime.Serialization;

namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4DPadDirection : DualShock4Property
{
	private class NoneDPadDirection : DualShock4DPadDirection
	{
		public NoneDPadDirection()
			: base(0, "None", 8)
		{
		}
	}

	private class NorthwestDPadDirection : DualShock4DPadDirection
	{
		public NorthwestDPadDirection()
			: base(1, "Northwest", 7)
		{
		}
	}

	private class WestDPadDirection : DualShock4DPadDirection
	{
		public WestDPadDirection()
			: base(2, "West", 6)
		{
		}
	}

	private class SouthwestDPadDirection : DualShock4DPadDirection
	{
		public SouthwestDPadDirection()
			: base(3, "Southwest", 5)
		{
		}
	}

	private class SouthDPadDirection : DualShock4DPadDirection
	{
		public SouthDPadDirection()
			: base(4, "South", 4)
		{
		}
	}

	private class SoutheastDPadDirection : DualShock4DPadDirection
	{
		public SoutheastDPadDirection()
			: base(5, "Southeast", 3)
		{
		}
	}

	private class EastDPadDirection : DualShock4DPadDirection
	{
		public EastDPadDirection()
			: base(6, "East", 2)
		{
		}
	}

	private class NortheastDPadDirection : DualShock4DPadDirection
	{
		public NortheastDPadDirection()
			: base(7, "Northeast", 1)
		{
		}
	}

	private class NorthDPadDirection : DualShock4DPadDirection
	{
		public NorthDPadDirection()
			: base(8, "North", 0)
		{
		}
	}

	public static DualShock4DPadDirection None = new NoneDPadDirection();

	public static DualShock4DPadDirection Northwest = new NorthwestDPadDirection();

	public static DualShock4DPadDirection West = new WestDPadDirection();

	public static DualShock4DPadDirection Southwest = new SouthwestDPadDirection();

	public static DualShock4DPadDirection South = new SouthDPadDirection();

	public static DualShock4DPadDirection Southeast = new SoutheastDPadDirection();

	public static DualShock4DPadDirection East = new EastDPadDirection();

	public static DualShock4DPadDirection Northeast = new NortheastDPadDirection();

	public static DualShock4DPadDirection North = new NorthDPadDirection();

	[IgnoreDataMember]
	public ushort Value { get; }

	protected DualShock4DPadDirection(int id, string name, ushort value)
		: base(id, name)
	{
		Value = value;
	}
}
