using System;
using System.IO;
using System.Linq;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip;

internal class ZipHeaderFactory
{
	internal const uint ENTRY_HEADER_BYTES = 67324752u;

	internal const uint POST_DATA_DESCRIPTOR = 134695760u;

	internal const uint DIRECTORY_START_HEADER_BYTES = 33639248u;

	internal const uint DIRECTORY_END_HEADER_BYTES = 101010256u;

	internal const uint DIGITAL_SIGNATURE = 84233040u;

	internal const uint SPLIT_ARCHIVE_HEADER_BYTES = 808471376u;

	internal const uint ZIP64_END_OF_CENTRAL_DIRECTORY = 101075792u;

	internal const uint ZIP64_END_OF_CENTRAL_DIRECTORY_LOCATOR = 117853008u;

	protected LocalEntryHeader lastEntryHeader;

	private readonly string password;

	private readonly StreamingMode mode;

	protected ZipHeaderFactory(StreamingMode mode, string password)
	{
		this.mode = mode;
		this.password = password;
	}

	protected ZipHeader ReadHeader(uint headerBytes, BinaryReader reader, bool zip64 = false)
	{
		switch (headerBytes)
		{
		case 67324752u:
		{
			LocalEntryHeader localEntryHeader = new LocalEntryHeader();
			localEntryHeader.Read(reader);
			LoadHeader(localEntryHeader, reader.BaseStream);
			lastEntryHeader = localEntryHeader;
			return localEntryHeader;
		}
		case 33639248u:
		{
			DirectoryEntryHeader directoryEntryHeader = new DirectoryEntryHeader();
			directoryEntryHeader.Read(reader);
			return directoryEntryHeader;
		}
		case 134695760u:
			if (FlagUtility.HasFlag(lastEntryHeader.Flags, HeaderFlags.UsePostDataDescriptor))
			{
				lastEntryHeader.Crc = reader.ReadUInt32();
				lastEntryHeader.CompressedSize = (long)(zip64 ? reader.ReadUInt64() : reader.ReadUInt32());
				lastEntryHeader.UncompressedSize = (long)(zip64 ? reader.ReadUInt64() : reader.ReadUInt32());
			}
			else
			{
				reader.ReadBytes(zip64 ? 20 : 12);
			}
			return null;
		case 84233040u:
			return null;
		case 101010256u:
		{
			DirectoryEndHeader directoryEndHeader = new DirectoryEndHeader();
			directoryEndHeader.Read(reader);
			return directoryEndHeader;
		}
		case 808471376u:
			return new SplitHeader();
		case 101075792u:
		{
			Zip64DirectoryEndHeader zip64DirectoryEndHeader = new Zip64DirectoryEndHeader();
			zip64DirectoryEndHeader.Read(reader);
			return zip64DirectoryEndHeader;
		}
		case 117853008u:
		{
			Zip64DirectoryEndLocatorHeader zip64DirectoryEndLocatorHeader = new Zip64DirectoryEndLocatorHeader();
			zip64DirectoryEndLocatorHeader.Read(reader);
			return zip64DirectoryEndLocatorHeader;
		}
		default:
			throw new NotSupportedException("Unknown header: " + headerBytes);
		}
	}

	internal static bool IsHeader(uint headerBytes)
	{
		switch (headerBytes)
		{
		case 33639248u:
		case 67324752u:
		case 84233040u:
		case 101010256u:
		case 101075792u:
		case 117853008u:
		case 134695760u:
		case 808471376u:
			return true;
		default:
			return false;
		}
	}

	private void LoadHeader(ZipFileEntry entryHeader, Stream stream)
	{
		if (FlagUtility.HasFlag(entryHeader.Flags, HeaderFlags.Encrypted))
		{
			if (!entryHeader.IsDirectory && entryHeader.CompressedSize == 0L && FlagUtility.HasFlag(entryHeader.Flags, HeaderFlags.UsePostDataDescriptor))
			{
				throw new NotSupportedException("SharpCompress cannot currently read non-seekable Zip Streams with encrypted data that has been written in a non-seekable manner.");
			}
			if (password == null)
			{
				throw new CryptographicException("No password supplied for encrypted zip.");
			}
			entryHeader.Password = password;
			if (entryHeader.CompressionMethod == ZipCompressionMethod.WinzipAes)
			{
				ExtraData extraData = entryHeader.Extra.SingleOrDefault((ExtraData x) => x.Type == ExtraDataType.WinZipAes);
				if (extraData != null)
				{
					WinzipAesKeySize keySize = (WinzipAesKeySize)extraData.DataBytes[4];
					byte[] array = new byte[WinzipAesEncryptionData.KeyLengthInBytes(keySize) / 2];
					byte[] array2 = new byte[2];
					stream.Read(array, 0, array.Length);
					stream.Read(array2, 0, 2);
					entryHeader.WinzipAesEncryptionData = new WinzipAesEncryptionData(keySize, array, array2, password);
					entryHeader.CompressedSize -= (uint)(array.Length + 2);
				}
			}
		}
		if (!entryHeader.IsDirectory)
		{
			switch (mode)
			{
			case StreamingMode.Seekable:
				entryHeader.DataStartPosition = stream.Position;
				stream.Position += entryHeader.CompressedSize;
				break;
			case StreamingMode.Streaming:
				entryHeader.PackedStream = stream;
				break;
			default:
				throw new InvalidFormatException("Invalid StreamingMode");
			}
		}
	}
}
