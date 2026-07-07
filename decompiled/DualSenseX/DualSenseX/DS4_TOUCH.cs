using System.Runtime.InteropServices;

namespace DualSenseX;

[StructLayout(LayoutKind.Sequential, Size = 9)]
internal struct DS4_TOUCH
{
	public byte bPacketCounter;

	public byte bIsUpTrackingNum1;

	public unsafe fixed byte bTouchData1[3];

	public byte bIsUpTrackingNum2;

	public unsafe fixed byte bTouchData2[3];
}
