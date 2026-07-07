using System;
using System.IO;

namespace SharpCompress.Compressors.Xz;

[CLSCompliant(false)]
internal class XZIndexRecord
{
	public ulong UnpaddedSize { get; private set; }

	public ulong UncompressedSize { get; private set; }

	protected XZIndexRecord()
	{
	}

	public static XZIndexRecord FromBinaryReader(BinaryReader br)
	{
		return new XZIndexRecord
		{
			UnpaddedSize = br.ReadXZInteger(),
			UncompressedSize = br.ReadXZInteger()
		};
	}
}
