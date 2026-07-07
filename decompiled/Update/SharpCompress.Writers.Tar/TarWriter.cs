using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;

namespace SharpCompress.Writers.Tar;

internal class TarWriter : AbstractWriter
{
	public TarWriter(Stream destination, WriterOptions options)
		: base(ArchiveType.Tar)
	{
		if (!destination.CanWrite)
		{
			throw new ArgumentException("Tars require writable streams.");
		}
		switch (options.CompressionType)
		{
		case CompressionType.BZip2:
			destination = new BZip2Stream(destination, CompressionMode.Compress, leaveOpen: true);
			break;
		case CompressionType.GZip:
			destination = new GZipStream(destination, CompressionMode.Compress, leaveOpen: true);
			break;
		case CompressionType.LZip:
			destination = new LZipStream(destination, CompressionMode.Compress, leaveOpen: true);
			break;
		default:
			throw new InvalidFormatException("Tar does not support compression: " + options.CompressionType);
		case CompressionType.None:
			break;
		}
		InitalizeStream(destination, closeStream: true);
	}

	public override void Write(string filename, Stream source, DateTime? modificationTime)
	{
		Write(filename, source, modificationTime, null);
	}

	private string NormalizeFilename(string filename)
	{
		filename = filename.Replace('\\', '/');
		int num = filename.IndexOf(':');
		if (num >= 0)
		{
			filename = filename.Remove(0, num + 1);
		}
		return filename.Trim(new char[1] { '/' });
	}

	public void Write(string filename, Stream source, DateTime? modificationTime, long? size)
	{
		if (!source.CanSeek && !size.HasValue)
		{
			throw new ArgumentException("Seekable stream is required if no size is given.");
		}
		long size2 = size ?? source.Length;
		TarHeader tarHeader = new TarHeader();
		tarHeader.LastModifiedTime = modificationTime ?? TarHeader.Epoch;
		tarHeader.Name = NormalizeFilename(filename);
		tarHeader.Size = size2;
		tarHeader.Write(base.OutputStream);
		size = source.TransferTo(base.OutputStream);
		PadTo512(size.Value, forceZeros: false);
	}

	private void PadTo512(long size, bool forceZeros)
	{
		int num = (int)size % 512;
		if (num != 0 || forceZeros)
		{
			num = 512 - num;
			base.OutputStream.Write(new byte[num], 0, num);
		}
	}

	protected override void Dispose(bool isDisposing)
	{
		if (isDisposing)
		{
			PadTo512(0L, forceZeros: true);
			PadTo512(0L, forceZeros: true);
			Stream outputStream = base.OutputStream;
			if (outputStream != null)
			{
				if (!(outputStream is BZip2Stream bZip2Stream))
				{
					if (outputStream is LZipStream lZipStream)
					{
						lZipStream.Finish();
					}
				}
				else
				{
					bZip2Stream.Finish();
				}
			}
		}
		base.Dispose(isDisposing);
	}
}
