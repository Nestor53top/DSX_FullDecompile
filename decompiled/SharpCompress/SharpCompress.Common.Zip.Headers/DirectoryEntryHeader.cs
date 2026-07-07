using System.IO;
using System.Linq;

namespace SharpCompress.Common.Zip.Headers;

internal class DirectoryEntryHeader : ZipFileEntry
{
	internal ushort Version { get; private set; }

	public ushort VersionNeededToExtract { get; set; }

	public long RelativeOffsetOfEntryHeader { get; set; }

	public uint ExternalFileAttributes { get; set; }

	public ushort InternalFileAttributes { get; set; }

	public ushort DiskNumberStart { get; set; }

	public string Comment { get; private set; }

	public DirectoryEntryHeader()
		: base(ZipHeaderType.DirectoryEntry)
	{
	}

	internal override void Read(BinaryReader reader)
	{
		Version = reader.ReadUInt16();
		VersionNeededToExtract = reader.ReadUInt16();
		base.Flags = (HeaderFlags)reader.ReadUInt16();
		base.CompressionMethod = (ZipCompressionMethod)reader.ReadUInt16();
		base.LastModifiedTime = reader.ReadUInt16();
		base.LastModifiedDate = reader.ReadUInt16();
		base.Crc = reader.ReadUInt32();
		base.CompressedSize = reader.ReadUInt32();
		base.UncompressedSize = reader.ReadUInt32();
		ushort count = reader.ReadUInt16();
		ushort count2 = reader.ReadUInt16();
		ushort count3 = reader.ReadUInt16();
		DiskNumberStart = reader.ReadUInt16();
		InternalFileAttributes = reader.ReadUInt16();
		ExternalFileAttributes = reader.ReadUInt32();
		RelativeOffsetOfEntryHeader = reader.ReadUInt32();
		byte[] str = reader.ReadBytes(count);
		base.Name = DecodeString(str);
		byte[] extra = reader.ReadBytes(count2);
		byte[] str2 = reader.ReadBytes(count3);
		Comment = DecodeString(str2);
		LoadExtra(extra);
		ExtraData extraData = base.Extra.FirstOrDefault((ExtraData u) => u.Type == ExtraDataType.UnicodePathExtraField);
		if (extraData != null)
		{
			base.Name = ((ExtraUnicodePathExtraField)extraData).UnicodeName;
		}
		Zip64ExtendedInformationExtraField zip64ExtendedInformationExtraField = base.Extra.OfType<Zip64ExtendedInformationExtraField>().FirstOrDefault();
		if (zip64ExtendedInformationExtraField != null)
		{
			if (base.CompressedSize == uint.MaxValue)
			{
				base.CompressedSize = zip64ExtendedInformationExtraField.CompressedSize;
			}
			if (base.UncompressedSize == uint.MaxValue)
			{
				base.UncompressedSize = zip64ExtendedInformationExtraField.UncompressedSize;
			}
			if (RelativeOffsetOfEntryHeader == uint.MaxValue)
			{
				RelativeOffsetOfEntryHeader = zip64ExtendedInformationExtraField.RelativeOffsetOfEntryHeader;
			}
		}
	}
}
