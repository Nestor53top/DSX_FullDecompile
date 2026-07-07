using System;
using System.IO;

namespace Squirrel.Bsdiff;

internal class WrappingStream : Stream
{
	private Stream m_streamBase;

	private readonly Ownership m_ownership;

	public override bool CanRead
	{
		get
		{
			if (m_streamBase != null)
			{
				return m_streamBase.CanRead;
			}
			return false;
		}
	}

	public override bool CanSeek
	{
		get
		{
			if (m_streamBase != null)
			{
				return m_streamBase.CanSeek;
			}
			return false;
		}
	}

	public override bool CanWrite
	{
		get
		{
			if (m_streamBase != null)
			{
				return m_streamBase.CanWrite;
			}
			return false;
		}
	}

	public override long Length
	{
		get
		{
			ThrowIfDisposed();
			return m_streamBase.Length;
		}
	}

	public override long Position
	{
		get
		{
			ThrowIfDisposed();
			return m_streamBase.Position;
		}
		set
		{
			ThrowIfDisposed();
			m_streamBase.Position = value;
		}
	}

	protected Stream WrappedStream => m_streamBase;

	public WrappingStream(Stream streamBase, Ownership ownership)
	{
		if (streamBase == null)
		{
			throw new ArgumentNullException("streamBase");
		}
		m_streamBase = streamBase;
		m_ownership = ownership;
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		ThrowIfDisposed();
		return m_streamBase.BeginRead(buffer, offset, count, callback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		ThrowIfDisposed();
		return m_streamBase.BeginWrite(buffer, offset, count, callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		ThrowIfDisposed();
		return m_streamBase.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		ThrowIfDisposed();
		m_streamBase.EndWrite(asyncResult);
	}

	public override void Flush()
	{
		ThrowIfDisposed();
		m_streamBase.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		ThrowIfDisposed();
		return m_streamBase.Read(buffer, offset, count);
	}

	public override int ReadByte()
	{
		ThrowIfDisposed();
		return m_streamBase.ReadByte();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		ThrowIfDisposed();
		return m_streamBase.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		ThrowIfDisposed();
		m_streamBase.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		ThrowIfDisposed();
		m_streamBase.Write(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		ThrowIfDisposed();
		m_streamBase.WriteByte(value);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing)
			{
				if (m_streamBase != null && m_ownership == Ownership.Owns)
				{
					m_streamBase.Dispose();
				}
				m_streamBase = null;
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	private void ThrowIfDisposed()
	{
		if (m_streamBase == null)
		{
			throw new ObjectDisposedException(GetType().Name);
		}
	}
}
