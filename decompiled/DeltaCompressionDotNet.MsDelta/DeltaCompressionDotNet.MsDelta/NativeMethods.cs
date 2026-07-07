using System;
using System.Runtime.InteropServices;

namespace DeltaCompressionDotNet.MsDelta;

internal static class NativeMethods
{
	[DllImport("msdelta.dll", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ApplyDelta([MarshalAs(UnmanagedType.I8)] ApplyFlags applyFlags, string sourceName, string deltaName, string targetName);

	[DllImport("msdelta.dll", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CreateDelta([MarshalAs(UnmanagedType.I8)] FileTypeSet fileTypeSet, long setFlags, long resetFlags, string sourceName, string targetName, string sourceOptionsName, string targetOptionsName, DeltaInput globalOptions, IntPtr targetFileTime, [MarshalAs(UnmanagedType.U4)] HashAlgId hashAlgId, string deltaName);
}
