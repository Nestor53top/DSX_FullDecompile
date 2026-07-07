using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Compressors.Rar;

internal class MultiVolumeReadOnlyStream : Stream
{
	private long currentPosition;

	private long maxPosition;

	private IEnumerator<RarFilePart> filePartEnumerator;

	private Stream currentStream;

	private readonly IExtractionListener streamListener;

	private long currentPartTotalReadBytes;

	private long currentEntryTotalReadBytes;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public uint CurrentCrc { get; private set; }

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	internal MultiVolumeReadOnlyStream(IEnumerable<RarFilePart> parts, IExtractionListener streamListener)
	{
		this.streamListener = streamListener;
		filePartEnumerator = parts.GetEnumerator();
		filePartEnumerator.MoveNext();
		InitializeNextFilePart();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing)
		{
			if (filePartEnumerator != null)
			{
				filePartEnumerator.Dispose();
				filePartEnumerator = null;
			}
			if (currentStream != null)
			{
				currentStream.Dispose();
				currentStream = null;
			}
		}
	}

	private void InitializeNextFilePart()
	{
		maxPosition = filePartEnumerator.Current.FileHeader.CompressedSize;
		currentPosition = 0L;
		if (currentStream != null)
		{
			currentStream.Dispose();
		}
		currentStream = filePartEnumerator.Current.GetCompressedStream();
		currentPartTotalReadBytes = 0L;
		CurrentCrc = filePartEnumerator.Current.FileHeader.FileCRC;
		streamListener.FireFilePartExtractionBegin(filePartEnumerator.Current.FilePartName, filePartEnumerator.Current.FileHeader.CompressedSize, filePartEnumerator.Current.FileHeader.UncompressedSize);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		int num2 = offset;
		int num3 = count;
		while (num3 > 0)
		{
			int count2 = num3;
			if (num3 > maxPosition - currentPosition)
			{
				count2 = (int)(maxPosition - currentPosition);
			}
			int num4 = currentStream.Read(buffer, num2, count2);
			if (num4 < 0)
			{
				throw new EndOfStreamException();
			}
			currentPosition += num4;
			num2 += num4;
			num3 -= num4;
			num += num4;
			if (maxPosition - currentPosition != 0L || !filePartEnumerator.Current.FileHeader.FileFlags.HasFlag(FileFlags.SPLIT_AFTER))
			{
				break;
			}
			if (filePartEnumerator.Current.FileHeader.Salt != null)
			{
				throw new InvalidFormatException("Sharpcompress currently does not support multi-volume decryption.");
			}
			string fileName = filePartEnumerator.Current.FileHeader.FileName;
			if (!filePartEnumerator.MoveNext())
			{
				throw new InvalidFormatException("Multi-part rar file is incomplete.  Entry expects a new volume: " + fileName);
			}
			InitializeNextFilePart();
		}
		currentPartTotalReadBytes += num;
		currentEntryTotalReadBytes += num;
		streamListener.FireCompressedBytesRead(currentPartTotalReadBytes, currentEntryTotalReadBytes);
		return num;
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
