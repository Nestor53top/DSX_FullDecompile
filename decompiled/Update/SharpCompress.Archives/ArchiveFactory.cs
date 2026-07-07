using System;
using System.IO;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives;

internal class ArchiveFactory
{
	public static IArchive Open(Stream stream, ReaderOptions readerOptions = null)
	{
		stream.CheckNotNull("stream");
		if (!stream.CanRead || !stream.CanSeek)
		{
			throw new ArgumentException("Stream should be readable and seekable");
		}
		readerOptions = readerOptions ?? new ReaderOptions();
		if (ZipArchive.IsZipFile(stream))
		{
			stream.Seek(0L, SeekOrigin.Begin);
			return ZipArchive.Open(stream, readerOptions);
		}
		stream.Seek(0L, SeekOrigin.Begin);
		if (SevenZipArchive.IsSevenZipFile(stream))
		{
			stream.Seek(0L, SeekOrigin.Begin);
			return SevenZipArchive.Open(stream, readerOptions);
		}
		stream.Seek(0L, SeekOrigin.Begin);
		if (GZipArchive.IsGZipFile(stream))
		{
			stream.Seek(0L, SeekOrigin.Begin);
			return GZipArchive.Open(stream, readerOptions);
		}
		stream.Seek(0L, SeekOrigin.Begin);
		if (RarArchive.IsRarFile(stream, readerOptions))
		{
			stream.Seek(0L, SeekOrigin.Begin);
			return RarArchive.Open(stream, readerOptions);
		}
		stream.Seek(0L, SeekOrigin.Begin);
		if (TarArchive.IsTarFile(stream))
		{
			stream.Seek(0L, SeekOrigin.Begin);
			return TarArchive.Open(stream, readerOptions);
		}
		throw new InvalidOperationException("Cannot determine compressed stream type. Supported Archive Formats: Zip, GZip, Tar, Rar, 7Zip, LZip");
	}

	public static IWritableArchive Create(ArchiveType type)
	{
		return type switch
		{
			ArchiveType.Zip => ZipArchive.Create(), 
			ArchiveType.Tar => TarArchive.Create(), 
			ArchiveType.GZip => GZipArchive.Create(), 
			_ => throw new NotSupportedException("Cannot create Archives of type: " + type), 
		};
	}

	public static IArchive Open(string filePath, ReaderOptions options = null)
	{
		filePath.CheckNotNullOrEmpty("filePath");
		return Open(new FileInfo(filePath), options ?? new ReaderOptions());
	}

	public static IArchive Open(FileInfo fileInfo, ReaderOptions options = null)
	{
		fileInfo.CheckNotNull("fileInfo");
		options = options ?? new ReaderOptions();
		using FileStream fileStream = fileInfo.OpenRead();
		if (ZipArchive.IsZipFile(fileStream))
		{
			fileStream.Dispose();
			return ZipArchive.Open(fileInfo, options);
		}
		fileStream.Seek(0L, SeekOrigin.Begin);
		if (SevenZipArchive.IsSevenZipFile(fileStream))
		{
			fileStream.Dispose();
			return SevenZipArchive.Open(fileInfo, options);
		}
		fileStream.Seek(0L, SeekOrigin.Begin);
		if (GZipArchive.IsGZipFile(fileStream))
		{
			fileStream.Dispose();
			return GZipArchive.Open(fileInfo, options);
		}
		fileStream.Seek(0L, SeekOrigin.Begin);
		if (RarArchive.IsRarFile(fileStream, options))
		{
			fileStream.Dispose();
			return RarArchive.Open(fileInfo, options);
		}
		fileStream.Seek(0L, SeekOrigin.Begin);
		if (TarArchive.IsTarFile(fileStream))
		{
			fileStream.Dispose();
			return TarArchive.Open(fileInfo, options);
		}
		throw new InvalidOperationException("Cannot determine compressed stream type. Supported Archive Formats: Zip, GZip, Tar, Rar, 7Zip");
	}

	public static void WriteToDirectory(string sourceArchive, string destinationDirectory, ExtractionOptions options = null)
	{
		using IArchive archive = Open(sourceArchive);
		foreach (IArchiveEntry entry in archive.Entries)
		{
			entry.WriteToDirectory(destinationDirectory, options);
		}
	}
}
