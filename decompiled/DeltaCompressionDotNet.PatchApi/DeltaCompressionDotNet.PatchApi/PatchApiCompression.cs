using System;
using System.ComponentModel;

namespace DeltaCompressionDotNet.PatchApi;

public sealed class PatchApiCompression : IDeltaCompression
{
	public void CreateDelta(string oldFilePath, string newFilePath, string deltaFilePath)
	{
		if (!NativeMethods.CreatePatchFile(oldFilePath, newFilePath, deltaFilePath, 0u, IntPtr.Zero))
		{
			throw new Win32Exception();
		}
	}

	public void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath)
	{
		if (!NativeMethods.ApplyPatchToFile(deltaFilePath, oldFilePath, newFilePath, 0u))
		{
			throw new Win32Exception();
		}
	}
}
