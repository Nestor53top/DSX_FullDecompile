using System;
using System.IO;

namespace SharpCompress.Compressors.ADC;

internal class ADCStream : Stream
{
	private readonly Stream stream;

	private bool isDisposed;

	private long position;

	private byte[] outBuffer;

	private int outPosition;

	public override bool CanRead => stream.CanRead;

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
			return position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public ADCStream(Stream stream, CompressionMode compressionMode = CompressionMode.Decompress)
	{
		if (compressionMode == CompressionMode.Compress)
		{
			throw new NotSupportedException();
		}
		this.stream = stream;
	}

	public override void Flush()
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			base.Dispose(disposing);
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (count == 0)
		{
			return 0;
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (offset < buffer.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (offset + count > buffer.GetLength(0))
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (outBuffer == null)
		{
			ADCBase.Decompress(stream, out outBuffer);
			outPosition = 0;
		}
		int num = offset;
		int num2 = count;
		int num3 = 0;
		while (outPosition + num2 >= outBuffer.Length)
		{
			int num4 = outBuffer.Length - outPosition;
			Array.Copy(outBuffer, outPosition, buffer, num, num4);
			num += num4;
			num3 += num4;
			position += num4;
			num2 -= num4;
			int num5 = ADCBase.Decompress(stream, out outBuffer);
			outPosition = 0;
			if (num5 == 0 || outBuffer == null || outBuffer.Length == 0)
			{
				return num3;
			}
		}
		Array.Copy(outBuffer, outPosition, buffer, num, num2);
		outPosition += num2;
		position += num2;
		return num3 + num2;
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
