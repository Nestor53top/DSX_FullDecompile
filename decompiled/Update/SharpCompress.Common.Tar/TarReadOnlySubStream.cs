using System;
using System.IO;

namespace SharpCompress.Common.Tar;

internal class TarReadOnlySubStream : Stream
{
	private bool isDisposed;

	private long amountRead;

	private long BytesLeftToRead { get; set; }

	public Stream Stream { get; }

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

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

	public TarReadOnlySubStream(Stream stream, long bytesToRead)
	{
		Stream = stream;
		BytesLeftToRead = bytesToRead;
	}

	protected override void Dispose(bool disposing)
	{
		if (isDisposed)
		{
			return;
		}
		isDisposed = true;
		if (!disposing)
		{
			return;
		}
		long num = amountRead % 512;
		if (num != 0L)
		{
			num = 512 - num;
			if (num != 0L)
			{
				byte[] buffer = new byte[num];
				Stream.ReadFully(buffer);
			}
		}
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (BytesLeftToRead < count)
		{
			count = (int)BytesLeftToRead;
		}
		int num = Stream.Read(buffer, offset, count);
		if (num > 0)
		{
			BytesLeftToRead -= num;
			amountRead += num;
		}
		return num;
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
