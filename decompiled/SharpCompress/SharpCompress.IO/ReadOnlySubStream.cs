using System;
using System.IO;

namespace SharpCompress.IO;

internal class ReadOnlySubStream : Stream
{
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

	public ReadOnlySubStream(Stream stream, long bytesToRead)
		: this(stream, null, bytesToRead)
	{
	}

	public ReadOnlySubStream(Stream stream, long? origin, long bytesToRead)
	{
		Stream = stream;
		if (origin.HasValue)
		{
			stream.Position = origin.Value;
		}
		BytesLeftToRead = bytesToRead;
	}

	protected override void Dispose(bool disposing)
	{
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
