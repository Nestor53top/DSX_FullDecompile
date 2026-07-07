using System.Runtime.InteropServices;

namespace DualSenseX;

[StructLayout(LayoutKind.Explicit)]
internal struct DS4_REPORT_UNION
{
	[FieldOffset(0)]
	public DS4_REPORT_EX reportStruct;

	[FieldOffset(0)]
	public unsafe fixed byte Report[63];
}
