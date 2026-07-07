using SharpCompress.IO;

namespace SharpCompress.Common.Rar.Headers;

internal class ProtectHeader : RarHeader
{
	internal uint DataSize => base.AdditionalSize;

	internal byte Version { get; private set; }

	internal ushort RecSectors { get; private set; }

	internal uint TotalBlocks { get; private set; }

	internal byte[] Mark { get; private set; }

	protected override void ReadFromReader(MarkingBinaryReader reader)
	{
		Version = reader.ReadByte();
		RecSectors = reader.ReadUInt16();
		TotalBlocks = reader.ReadUInt32();
		Mark = reader.ReadBytes(8);
	}
}
