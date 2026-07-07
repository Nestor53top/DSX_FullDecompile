using System;
using System.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common;

internal class EntryStream : Stream
{
	private readonly Stream stream;

	private bool completed;

	private bool isDisposed;

	public IReader Reader { get; }

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

	internal EntryStream(IReader reader, Stream stream)
	{
		Reader = reader;
		this.stream = stream;
	}

	public void SkipEntry()
	{
		byte[] array = new byte[4096];
		while (Read(array, 0, array.Length) > 0)
		{
		}
		completed = true;
	}

	protected override void Dispose(bool disposing)
	{
		if (!completed && !Reader.Cancelled)
		{
			SkipEntry();
		}
		if (!isDisposed)
		{
			isDisposed = true;
			base.Dispose(disposing);
			stream.Dispose();
		}
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = stream.Read(buffer, offset, count);
		if (num <= 0)
		{
			completed = true;
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
