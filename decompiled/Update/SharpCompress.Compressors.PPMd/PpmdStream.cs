using System;
using System.IO;
using SharpCompress.Compressors.LZMA.RangeCoder;
using SharpCompress.Compressors.PPMd.H;
using SharpCompress.Compressors.PPMd.I1;

namespace SharpCompress.Compressors.PPMd;

internal class PpmdStream : Stream
{
	private readonly PpmdProperties properties;

	private readonly Stream stream;

	private readonly bool compress;

	private readonly Model model;

	private readonly ModelPPM modelH;

	private readonly Decoder decoder;

	private long position;

	private bool isDisposed;

	public override bool CanRead => !compress;

	public override bool CanSeek => false;

	public override bool CanWrite => compress;

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

	public PpmdStream(PpmdProperties properties, Stream stream, bool compress)
	{
		this.properties = properties;
		this.stream = stream;
		this.compress = compress;
		if (properties.Version == PpmdVersion.I1)
		{
			model = new Model();
			if (compress)
			{
				model.EncodeStart(properties);
			}
			else
			{
				model.DecodeStart(stream, properties);
			}
		}
		if (properties.Version == PpmdVersion.H)
		{
			modelH = new ModelPPM();
			if (compress)
			{
				throw new NotImplementedException();
			}
			modelH.decodeInit(stream, properties.ModelOrder, properties.AllocatorSize);
		}
		if (properties.Version == PpmdVersion.H7z)
		{
			modelH = new ModelPPM();
			if (compress)
			{
				throw new NotImplementedException();
			}
			modelH.decodeInit(null, properties.ModelOrder, properties.AllocatorSize);
			decoder = new Decoder();
			decoder.Init(stream);
		}
	}

	public override void Flush()
	{
	}

	protected override void Dispose(bool isDisposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			if (isDisposing && compress)
			{
				model.EncodeBlock(stream, new MemoryStream(), final: true);
			}
			base.Dispose(isDisposing);
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (compress)
		{
			return 0;
		}
		int i = 0;
		if (properties.Version == PpmdVersion.I1)
		{
			i = model.DecodeBlock(stream, buffer, offset, count);
		}
		if (properties.Version == PpmdVersion.H)
		{
			for (; i < count; i++)
			{
				int num;
				if ((num = modelH.decodeChar()) < 0)
				{
					break;
				}
				buffer[offset++] = (byte)num;
			}
		}
		if (properties.Version == PpmdVersion.H7z)
		{
			for (; i < count; i++)
			{
				int num2;
				if ((num2 = modelH.decodeChar(decoder)) < 0)
				{
					break;
				}
				buffer[offset++] = (byte)num2;
			}
		}
		position += i;
		return i;
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
		if (compress)
		{
			model.EncodeBlock(stream, new MemoryStream(buffer, offset, count), final: false);
		}
	}
}
