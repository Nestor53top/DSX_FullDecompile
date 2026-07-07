using System;
using System.Runtime.InteropServices;

namespace DeltaCompressionDotNet.PatchApi;

internal static class NativeMethods
{
	[DllImport("mspatcha.dll", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ApplyPatchToFile(string patchFileName, string oldFileName, string newFileName, uint applyOptionFlags);

	[DllImport("mspatchc.dll", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CreatePatchFile(string oldFileName, string newFileName, string patchFileName, uint optionFlags, IntPtr optionData);
}
