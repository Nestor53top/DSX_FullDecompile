using SharpCompress.Converters;

namespace SharpCompress.Common.Zip.Headers;

internal class Zip64ExtendedInformationExtraField : ExtraData
{
	public long UncompressedSize { get; private set; }

	public long CompressedSize { get; private set; }

	public long RelativeOffsetOfEntryHeader { get; private set; }

	public uint VolumeNumber { get; private set; }

	public Zip64ExtendedInformationExtraField(ExtraDataType type, ushort length, byte[] dataBytes)
	{
		base.Type = type;
		base.Length = length;
		base.DataBytes = dataBytes;
		Process();
	}

	private void Process()
	{
		switch (base.DataBytes.Length)
		{
		case 4:
			VolumeNumber = DataConverter.LittleEndian.GetUInt32(base.DataBytes, 0);
			break;
		case 8:
			RelativeOffsetOfEntryHeader = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			break;
		case 12:
			RelativeOffsetOfEntryHeader = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			VolumeNumber = DataConverter.LittleEndian.GetUInt32(base.DataBytes, 8);
			break;
		case 16:
			UncompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			CompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 8);
			break;
		case 20:
			UncompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			CompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 8);
			VolumeNumber = DataConverter.LittleEndian.GetUInt32(base.DataBytes, 16);
			break;
		case 24:
			UncompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			CompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 8);
			RelativeOffsetOfEntryHeader = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 16);
			break;
		case 28:
			UncompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 0);
			CompressedSize = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 8);
			RelativeOffsetOfEntryHeader = (long)DataConverter.LittleEndian.GetUInt64(base.DataBytes, 16);
			VolumeNumber = DataConverter.LittleEndian.GetUInt32(base.DataBytes, 24);
			break;
		default:
			throw new ArchiveException("Unexpected size of of Zip64 extended information extra field");
		}
	}
}
