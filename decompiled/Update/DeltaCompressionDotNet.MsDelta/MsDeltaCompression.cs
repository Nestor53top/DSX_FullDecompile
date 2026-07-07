using System;
using System.ComponentModel;

namespace DeltaCompressionDotNet.MsDelta;

internal sealed class MsDeltaCompression : IDeltaCompression
{
	public void CreateDelta(string oldFilePath, string newFilePath, string deltaFilePath)
	{
		DeltaInput globalOptions = default(DeltaInput);
		IntPtr zero = IntPtr.Zero;
		if (!NativeMethods.CreateDelta(FileTypeSet.Executables, 0L, 0L, oldFilePath, newFilePath, null, null, globalOptions, zero, HashAlgId.Crc32, deltaFilePath))
		{
			throw new Win32Exception();
		}
	}

	public void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath)
	{
		if (!NativeMethods.ApplyDelta(ApplyFlags.AllowLegacy, oldFilePath, deltaFilePath, newFilePath))
		{
			throw new Win32Exception();
		}
	}
}
