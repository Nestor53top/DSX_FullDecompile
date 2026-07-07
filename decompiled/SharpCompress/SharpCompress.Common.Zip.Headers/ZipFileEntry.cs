using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpCompress.Converters;

namespace SharpCompress.Common.Zip.Headers;

internal abstract class ZipFileEntry : ZipHeader
{
	internal bool IsDirectory
	{
		get
		{
			if (Name.EndsWith("/"))
			{
				return true;
			}
			if (CompressedSize == 0L && UncompressedSize == 0L)
			{
				return Name.EndsWith("\\");
			}
			return false;
		}
	}

	internal Stream PackedStream { get; set; }

	internal string Name { get; set; }

	internal HeaderFlags Flags { get; set; }

	internal ZipCompressionMethod CompressionMethod { get; set; }

	internal long CompressedSize { get; set; }

	internal long? DataStartPosition { get; set; }

	internal long UncompressedSize { get; set; }

	internal List<ExtraData> Extra { get; set; }

	public string Password { get; set; }

	internal WinzipAesEncryptionData WinzipAesEncryptionData { get; set; }

	internal ushort LastModifiedDate { get; set; }

	internal ushort LastModifiedTime { get; set; }

	internal uint Crc { get; set; }

	internal ZipFilePart Part { get; set; }

	internal bool IsZip64 => CompressedSize == uint.MaxValue;

	protected ZipFileEntry(ZipHeaderType type)
		: base(type)
	{
		Extra = new List<ExtraData>();
	}

	protected string DecodeString(byte[] str)
	{
		if (FlagUtility.HasFlag(Flags, HeaderFlags.UTF8))
		{
			return Encoding.UTF8.GetString(str, 0, str.Length);
		}
		return ArchiveEncoding.Default.GetString(str, 0, str.Length);
	}

	protected byte[] EncodeString(string str)
	{
		if (FlagUtility.HasFlag(Flags, HeaderFlags.UTF8))
		{
			return Encoding.UTF8.GetBytes(str);
		}
		return ArchiveEncoding.Default.GetBytes(str);
	}

	internal PkwareTraditionalEncryptionData ComposeEncryptionData(Stream archiveStream)
	{
		if (archiveStream == null)
		{
			throw new ArgumentNullException("archiveStream");
		}
		byte[] array = new byte[12];
		archiveStream.Read(array, 0, 12);
		return PkwareTraditionalEncryptionData.ForRead(Password, this, array);
	}

	protected void LoadExtra(byte[] extra)
	{
		ushort uInt;
		for (int i = 0; i < extra.Length - 4; i += uInt + 4)
		{
			ExtraDataType extraDataType = (ExtraDataType)DataConverter.LittleEndian.GetUInt16(extra, i);
			if (!Enum.IsDefined(typeof(ExtraDataType), extraDataType))
			{
				extraDataType = ExtraDataType.NotImplementedExtraData;
			}
			uInt = DataConverter.LittleEndian.GetUInt16(extra, i + 2);
			byte[] array = new byte[uInt];
			Buffer.BlockCopy(extra, i + 4, array, 0, uInt);
			Extra.Add(LocalEntryHeaderExtraFactory.Create(extraDataType, uInt, array));
		}
	}
}
