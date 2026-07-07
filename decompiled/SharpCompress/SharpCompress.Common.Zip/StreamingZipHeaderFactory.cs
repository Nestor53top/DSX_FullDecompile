using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip;

internal class StreamingZipHeaderFactory : ZipHeaderFactory
{
	internal StreamingZipHeaderFactory(string password)
		: base(StreamingMode.Streaming, password)
	{
	}

	internal IEnumerable<ZipHeader> ReadStreamHeader(Stream stream)
	{
		RewindableStream rewindableStream = ((!(stream is RewindableStream)) ? new RewindableStream(stream) : (stream as RewindableStream));
		while (true)
		{
			BinaryReader binaryReader = new BinaryReader(rewindableStream);
			if (lastEntryHeader != null && (FlagUtility.HasFlag(lastEntryHeader.Flags, HeaderFlags.UsePostDataDescriptor) || lastEntryHeader.IsZip64))
			{
				binaryReader = (lastEntryHeader.Part as StreamingZipFilePart).FixStreamedFileLocation(ref rewindableStream);
				long? num = (rewindableStream.CanSeek ? new long?(rewindableStream.Position) : ((long?)null));
				uint num2 = binaryReader.ReadUInt32();
				if (num2 == 134695760)
				{
					num2 = binaryReader.ReadUInt32();
				}
				lastEntryHeader.Crc = num2;
				lastEntryHeader.CompressedSize = binaryReader.ReadUInt32();
				lastEntryHeader.UncompressedSize = binaryReader.ReadUInt32();
				if (num.HasValue)
				{
					lastEntryHeader.DataStartPosition = num - lastEntryHeader.CompressedSize;
				}
			}
			lastEntryHeader = null;
			uint headerBytes = binaryReader.ReadUInt32();
			ZipHeader zipHeader = ReadHeader(headerBytes, binaryReader);
			if (zipHeader.ZipHeaderType == ZipHeaderType.LocalEntry)
			{
				bool isRecording = rewindableStream.IsRecording;
				if (!isRecording)
				{
					rewindableStream.StartRecording();
				}
				uint headerBytes2 = binaryReader.ReadUInt32();
				zipHeader.HasData = !ZipHeaderFactory.IsHeader(headerBytes2);
				rewindableStream.Rewind(!isRecording);
			}
			yield return zipHeader;
		}
	}
}
