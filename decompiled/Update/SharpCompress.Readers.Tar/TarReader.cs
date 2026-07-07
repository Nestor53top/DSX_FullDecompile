using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Common.Tar;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.Xz;
using SharpCompress.IO;

namespace SharpCompress.Readers.Tar;

internal class TarReader : AbstractReader<TarEntry, TarVolume>
{
	private readonly CompressionType compressionType;

	public override TarVolume Volume { get; }

	internal TarReader(Stream stream, ReaderOptions options, CompressionType compressionType)
		: base(options, ArchiveType.Tar)
	{
		this.compressionType = compressionType;
		Volume = new TarVolume(stream, options);
	}

	internal override Stream RequestInitialStream()
	{
		Stream stream = base.RequestInitialStream();
		return compressionType switch
		{
			CompressionType.BZip2 => new BZip2Stream(stream, CompressionMode.Decompress), 
			CompressionType.GZip => new GZipStream(stream, CompressionMode.Decompress), 
			CompressionType.LZip => new LZipStream(stream, CompressionMode.Decompress), 
			CompressionType.Xz => new XZStream(stream), 
			CompressionType.None => stream, 
			_ => throw new NotSupportedException("Invalid compression type: " + compressionType), 
		};
	}

	public static TarReader Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		options = options ?? new ReaderOptions();
		RewindableStream rewindableStream = new RewindableStream(stream);
		rewindableStream.StartRecording();
		if (GZipArchive.IsGZipFile(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new GZipStream(rewindableStream, CompressionMode.Decompress)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.GZip);
			}
			throw new InvalidFormatException("Not a tar file.");
		}
		rewindableStream.Rewind(stopRecording: false);
		if (BZip2Stream.IsBZip2(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new BZip2Stream(rewindableStream, CompressionMode.Decompress)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.BZip2);
			}
			throw new InvalidFormatException("Not a tar file.");
		}
		rewindableStream.Rewind(stopRecording: false);
		if (LZipStream.IsLZipFile(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new LZipStream(rewindableStream, CompressionMode.Decompress)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.LZip);
			}
			throw new InvalidFormatException("Not a tar file.");
		}
		rewindableStream.Rewind(stopRecording: true);
		return new TarReader(rewindableStream, options, CompressionType.None);
	}

	internal override IEnumerable<TarEntry> GetEntries(Stream stream)
	{
		return TarEntry.GetEntries(StreamingMode.Streaming, stream, compressionType);
	}
}
