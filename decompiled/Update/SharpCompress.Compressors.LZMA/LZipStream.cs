using System;
using System.IO;
using SharpCompress.Converters;
using SharpCompress.Crypto;
using SharpCompress.IO;

namespace SharpCompress.Compressors.LZMA;

internal class LZipStream : Stream
{
	private readonly Stream stream;

	private readonly CountingWritableSubStream rawStream;

	private bool disposed;

	private readonly bool leaveOpen;

	private bool finished;

	private long writeCount;

	public CompressionMode Mode { get; }

	public override bool CanRead => Mode == CompressionMode.Decompress;

	public override bool CanSeek => false;

	public override bool CanWrite => Mode == CompressionMode.Compress;

	public override long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public LZipStream(Stream stream, CompressionMode mode, bool leaveOpen = false)
	{
		Mode = mode;
		this.leaveOpen = leaveOpen;
		if (mode == CompressionMode.Decompress)
		{
			int num = ValidateAndReadSize(stream);
			if (num == 0)
			{
				throw new IOException("Not an LZip stream");
			}
			byte[] properties = GetProperties(num);
			this.stream = new LzmaStream(properties, stream);
		}
		else
		{
			int dictionary = 106496;
			WriteHeaderSize(stream);
			rawStream = new CountingWritableSubStream(stream);
			this.stream = new Crc32Stream(new LzmaStream(new LzmaEncoderProperties(eos: true, dictionary), isLZMA2: false, rawStream));
		}
	}

	public void Finish()
	{
		if (!finished)
		{
			if (Mode == CompressionMode.Compress)
			{
				Crc32Stream crc32Stream = (Crc32Stream)stream;
				crc32Stream.WrappedStream.Dispose();
				crc32Stream.Dispose();
				ulong count = rawStream.Count;
				byte[] bytes = DataConverter.LittleEndian.GetBytes(crc32Stream.Crc);
				rawStream.Write(bytes, 0, bytes.Length);
				bytes = DataConverter.LittleEndian.GetBytes(writeCount);
				rawStream.Write(bytes, 0, bytes.Length);
				bytes = DataConverter.LittleEndian.GetBytes(count + 6 + 20);
				rawStream.Write(bytes, 0, bytes.Length);
			}
			finished = true;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		disposed = true;
		if (disposing)
		{
			Finish();
			if (!leaveOpen)
			{
				rawStream.Dispose();
			}
		}
	}

	public override void Flush()
	{
		stream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return stream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		stream.Write(buffer, offset, count);
		writeCount += count;
	}

	public static bool IsLZipFile(Stream stream)
	{
		return ValidateAndReadSize(stream) != 0;
	}

	public static int ValidateAndReadSize(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		byte[] array = new byte[6];
		if (stream.Read(array, 0, array.Length) != 6)
		{
			return 0;
		}
		if (array[0] != 76 || array[1] != 90 || array[2] != 73 || array[3] != 80 || array[4] != 1)
		{
			return 0;
		}
		int num = array[5] & 0x1F;
		int num2 = (array[5] & 0xE0) >> 5;
		return (1 << num) - num2 * (1 << num - 4);
	}

	public static void WriteHeaderSize(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		byte[] buffer = new byte[6] { 76, 90, 73, 80, 1, 113 };
		stream.Write(buffer, 0, 6);
	}

	private static byte[] GetProperties(int dictionarySize)
	{
		return new byte[5]
		{
			93,
			(byte)(dictionarySize & 0xFF),
			(byte)((dictionarySize >> 8) & 0xFF),
			(byte)((dictionarySize >> 16) & 0xFF),
			(byte)((dictionarySize >> 24) & 0xFF)
		};
	}
}
