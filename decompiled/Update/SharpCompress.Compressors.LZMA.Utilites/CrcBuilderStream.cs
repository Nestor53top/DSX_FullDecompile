using System;
using System.IO;

namespace SharpCompress.Compressors.LZMA.Utilites;

internal class CrcBuilderStream : Stream
{
	private readonly Stream mTarget;

	private uint mCRC;

	private bool mFinished;

	private bool isDisposed;

	public long Processed { get; private set; }

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

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

	public CrcBuilderStream(Stream target)
	{
		mTarget = target;
		mCRC = uint.MaxValue;
	}

	protected override void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			mTarget.Dispose();
			base.Dispose(disposing);
		}
	}

	public uint Finish()
	{
		if (!mFinished)
		{
			mFinished = true;
			mCRC = CRC.Finish(mCRC);
		}
		return mCRC;
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new InvalidOperationException();
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
		if (mFinished)
		{
			throw new InvalidOperationException("CRC calculation has been finished.");
		}
		Processed += count;
		mCRC = CRC.Update(mCRC, buffer, offset, count);
		mTarget.Write(buffer, offset, count);
	}
}
