using System;
using System.IO;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.Xz;
using SharpCompress.IO;
using SharpCompress.Readers.GZip;
using SharpCompress.Readers.Rar;
using SharpCompress.Readers.Tar;
using SharpCompress.Readers.Zip;

namespace SharpCompress.Readers;

internal static class ReaderFactory
{
	public static IReader Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		options = options ?? new ReaderOptions
		{
			LeaveStreamOpen = false
		};
		RewindableStream rewindableStream = new RewindableStream(stream);
		rewindableStream.StartRecording();
		if (ZipArchive.IsZipFile(rewindableStream, options.Password))
		{
			rewindableStream.Rewind(stopRecording: true);
			return ZipReader.Open(rewindableStream, options);
		}
		rewindableStream.Rewind(stopRecording: false);
		if (GZipArchive.IsGZipFile(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new GZipStream(rewindableStream, CompressionMode.Decompress)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.GZip);
			}
			rewindableStream.Rewind(stopRecording: true);
			return GZipReader.Open(rewindableStream, options);
		}
		rewindableStream.Rewind(stopRecording: false);
		if (BZip2Stream.IsBZip2(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new BZip2Stream(rewindableStream, CompressionMode.Decompress, leaveOpen: true)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.BZip2);
			}
		}
		rewindableStream.Rewind(stopRecording: false);
		if (LZipStream.IsLZipFile(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: false);
			if (TarArchive.IsTarFile(new LZipStream(rewindableStream, CompressionMode.Decompress, leaveOpen: true)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.LZip);
			}
		}
		rewindableStream.Rewind(stopRecording: false);
		if (RarArchive.IsRarFile(rewindableStream, options))
		{
			rewindableStream.Rewind(stopRecording: true);
			return RarReader.Open(rewindableStream, options);
		}
		rewindableStream.Rewind(stopRecording: false);
		if (TarArchive.IsTarFile(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: true);
			return TarReader.Open(rewindableStream, options);
		}
		rewindableStream.Rewind(stopRecording: false);
		if (XZStream.IsXZStream(rewindableStream))
		{
			rewindableStream.Rewind(stopRecording: true);
			if (TarArchive.IsTarFile(new XZStream(rewindableStream)))
			{
				rewindableStream.Rewind(stopRecording: true);
				return new TarReader(rewindableStream, options, CompressionType.Xz);
			}
		}
		throw new InvalidOperationException("Cannot determine compressed stream type.  Supported Reader Formats: Zip, GZip, BZip2, Tar, Rar, LZip, XZ");
	}
}
