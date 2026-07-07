using System.Runtime.InteropServices;

namespace DualSenseX;

[StructLayout(LayoutKind.Explicit, Size = 63)]
internal struct DS4_REPORT_EX
{
	[FieldOffset(0)]
	public byte bThumbLX;

	[FieldOffset(1)]
	public byte bThumbLY;

	[FieldOffset(2)]
	public byte bThumbRX;

	[FieldOffset(3)]
	public byte bThumbRY;

	[FieldOffset(4)]
	public ushort wButtons;

	[FieldOffset(6)]
	public byte bSpecial;

	[FieldOffset(7)]
	public byte bTriggerL;

	[FieldOffset(8)]
	public byte bTriggerR;

	[FieldOffset(9)]
	public ushort wTimestamp;

	[FieldOffset(11)]
	public byte bBatteryLvl;

	[FieldOffset(12)]
	public short wGyroX;

	[FieldOffset(14)]
	public short wGyroY;

	[FieldOffset(16)]
	public short wGyroZ;

	[FieldOffset(18)]
	public short wAccelX;

	[FieldOffset(20)]
	public short wAccelY;

	[FieldOffset(22)]
	public short wAccelZ;

	[FieldOffset(24)]
	public unsafe fixed byte _bUnknown1[5];

	[FieldOffset(29)]
	public byte bBatteryLvlSpecial;

	[FieldOffset(30)]
	public unsafe fixed byte _bUnknown2[2];

	[FieldOffset(32)]
	public byte bTouchPacketsN;

	[FieldOffset(33)]
	public DS4_TOUCH sCurrentTouch;

	[FieldOffset(42)]
	public DS4_TOUCH sPreviousTouch1;

	[FieldOffset(51)]
	public DS4_TOUCH sPreviousTouch2;
}
