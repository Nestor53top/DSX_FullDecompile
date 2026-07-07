using System.IO;
using System.Linq;

namespace SharpCompress.Common.Zip.Headers;

internal class LocalEntryHeader : ZipFileEntry
{
	internal ushort Version { get; private set; }

	public LocalEntryHeader()
		: base(ZipHeaderType.LocalEntry)
	{
	}

	internal override void Read(BinaryReader reader)
	{
		Version = reader.ReadUInt16();
		base.Flags = (HeaderFlags)reader.ReadUInt16();
		base.CompressionMethod = (ZipCompressionMethod)reader.ReadUInt16();
		base.LastModifiedTime = reader.ReadUInt16();
		base.LastModifiedDate = reader.ReadUInt16();
		base.Crc = reader.ReadUInt32();
		base.CompressedSize = reader.ReadUInt32();
		base.UncompressedSize = reader.ReadUInt32();
		ushort count = reader.ReadUInt16();
		ushort count2 = reader.ReadUInt16();
		byte[] str = reader.ReadBytes(count);
		byte[] extra = reader.ReadBytes(count2);
		base.Name = DecodeString(str);
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
		}
	}
}
