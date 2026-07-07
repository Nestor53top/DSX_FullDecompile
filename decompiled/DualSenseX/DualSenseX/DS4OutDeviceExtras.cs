using System.Runtime.InteropServices;

namespace DualSenseX;

internal static class DS4OutDeviceExtras
{
	public static void CopyBytes(ref DS4_REPORT_EX outReport, byte[] outBuffer)
	{
		GCHandle gCHandle = GCHandle.Alloc(outReport, GCHandleType.Pinned);
		Marshal.Copy(gCHandle.AddrOfPinnedObject(), outBuffer, 0, 63);
		gCHandle.Free();
	}
}
