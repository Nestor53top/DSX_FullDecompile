using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip;

internal class SeekableZipHeaderFactory : ZipHeaderFactory
{
	private const int MAX_ITERATIONS_FOR_DIRECTORY_HEADER = 4096;

	private bool zip64;

	internal SeekableZipHeaderFactory(string password)
		: base(StreamingMode.Seekable, password)
	{
	}

	internal IEnumerable<DirectoryEntryHeader> ReadSeekableHeader(Stream stream)
	{
		BinaryReader reader = new BinaryReader(stream);
		SeekBackToHeader(stream, reader, 101010256u);
		DirectoryEndHeader directoryEndHeader = new DirectoryEndHeader();
		directoryEndHeader.Read(reader);
		if (directoryEndHeader.IsZip64)
		{
			zip64 = true;
			SeekBackToHeader(stream, reader, 117853008u);
			Zip64DirectoryEndLocatorHeader zip64DirectoryEndLocatorHeader = new Zip64DirectoryEndLocatorHeader();
			zip64DirectoryEndLocatorHeader.Read(reader);
			stream.Seek(zip64DirectoryEndLocatorHeader.RelativeOffsetOfTheEndOfDirectoryRecord, SeekOrigin.Begin);
			if (reader.ReadUInt32() != 101075792)
			{
				throw new ArchiveException("Failed to locate the Zip64 Header");
			}
			Zip64DirectoryEndHeader zip64DirectoryEndHeader = new Zip64DirectoryEndHeader();
			zip64DirectoryEndHeader.Read(reader);
			stream.Seek(zip64DirectoryEndHeader.DirectoryStartOffsetRelativeToDisk, SeekOrigin.Begin);
		}
		else
		{
			stream.Seek(directoryEndHeader.DirectoryStartOffsetRelativeToDisk, SeekOrigin.Begin);
		}
		long position = stream.Position;
		while (true)
		{
			stream.Position = position;
			uint headerBytes = reader.ReadUInt32();
			DirectoryEntryHeader directoryEntryHeader = ReadHeader(headerBytes, reader, zip64) as DirectoryEntryHeader;
			position = stream.Position;
			if (directoryEntryHeader == null)
			{
				break;
			}
			directoryEntryHeader.HasData = directoryEntryHeader.CompressedSize != 0;
			yield return directoryEntryHeader;
		}
	}

	private static void SeekBackToHeader(Stream stream, BinaryReader reader, uint headerSignature)
	{
		long num = 0L;
		int num2 = 0;
		uint num3;
		do
		{
			if (stream.Length + num - 4 < 0)
			{
				throw new ArchiveException("Failed to locate the Zip Header");
			}
			stream.Seek(num - 4, SeekOrigin.End);
			num3 = reader.ReadUInt32();
			num--;
			num2++;
			if (num2 > 4096)
			{
				throw new ArchiveException("Could not find Zip file Directory at the end of the file.  File may be corrupted.");
			}
		}
		while (num3 != headerSignature);
	}

	internal LocalEntryHeader GetLocalHeader(Stream stream, DirectoryEntryHeader directoryEntryHeader)
	{
		stream.Seek(directoryEntryHeader.RelativeOffsetOfEntryHeader, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(stream);
		uint headerBytes = binaryReader.ReadUInt32();
		return (ReadHeader(headerBytes, binaryReader, zip64) as LocalEntryHeader) ?? throw new InvalidOperationException();
	}
}
