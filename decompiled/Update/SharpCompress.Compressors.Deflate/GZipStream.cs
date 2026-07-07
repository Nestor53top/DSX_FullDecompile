using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Converters;

namespace SharpCompress.Compressors.Deflate;

internal class GZipStream : Stream
{
	internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private string comment;

	private string fileName;

	internal ZlibBaseStream BaseStream;

	private bool disposed;

	private bool firstReadDone;

	private int headerByteCount;

	public DateTime? LastModified { get; set; }

	public virtual FlushType FlushMode
	{
		get
		{
			return BaseStream._flushMode;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			BaseStream._flushMode = value;
		}
	}

	public int BufferSize
	{
		get
		{
			return BaseStream._bufferSize;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			if (BaseStream._workingBuffer != null)
			{
				throw new ZlibException("The working buffer is already set.");
			}
			if (value < 1024)
			{
				throw new ZlibException($"Don't be silly. {value} bytes?? Use a bigger buffer, at least {1024}.");
			}
			BaseStream._bufferSize = value;
		}
	}

	internal virtual long TotalIn => BaseStream._z.TotalBytesIn;

	internal virtual long TotalOut => BaseStream._z.TotalBytesOut;

	public override bool CanRead
	{
		get
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return BaseStream._stream.CanRead;
		}
	}

	public override bool CanSeek => false;

	public override bool CanWrite
	{
		get
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return BaseStream._stream.CanWrite;
		}
	}

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
			if (BaseStream._streamMode == ZlibBaseStream.StreamMode.Writer)
			{
				return BaseStream._z.TotalBytesOut + headerByteCount;
			}
			if (BaseStream._streamMode == ZlibBaseStream.StreamMode.Reader)
			{
				return BaseStream._z.TotalBytesIn + BaseStream._gzipHeaderByteCount;
			}
			return 0L;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			comment = value;
		}
	}

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			if (disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			fileName = value;
			if (fileName == null)
			{
				return;
			}
			if (fileName.IndexOf("/") != -1)
			{
				fileName = fileName.Replace("/", "\\");
			}
			if (fileName.EndsWith("\\"))
			{
				throw new InvalidOperationException("Illegal filename");
			}
			if (fileName.IndexOf("\\") == -1)
			{
				return;
			}
			int length = fileName.Length;
			int num = length;
			while (--num >= 0)
			{
				if (fileName[num] == '\\')
				{
					fileName = fileName.Substring(num + 1, length - num - 1);
				}
			}
		}
	}

	public int Crc32 { get; private set; }

	public GZipStream(Stream stream, CompressionMode mode)
		: this(stream, mode, CompressionLevel.Default, leaveOpen: false)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
		: this(stream, mode, level, leaveOpen: false)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
		: this(stream, mode, CompressionLevel.Default, leaveOpen)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
	{
		BaseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (!disposed)
			{
				if (disposing && BaseStream != null)
				{
					BaseStream.Dispose();
					Crc32 = BaseStream.Crc32;
				}
				disposed = true;
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override void Flush()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		BaseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		int result = BaseStream.Read(buffer, offset, count);
		if (!firstReadDone)
		{
			firstReadDone = true;
			FileName = BaseStream._GzipFileName;
			Comment = BaseStream._GzipComment;
		}
		return result;
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
		if (disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		if (BaseStream._streamMode == ZlibBaseStream.StreamMode.Undefined)
		{
			if (!BaseStream._wantCompress)
			{
				throw new InvalidOperationException();
			}
			headerByteCount = EmitHeader();
		}
		BaseStream.Write(buffer, offset, count);
	}

	private int EmitHeader()
	{
		byte[] array = ((Comment == null) ? null : ArchiveEncoding.Default.GetBytes(Comment));
		byte[] array2 = ((FileName == null) ? null : ArchiveEncoding.Default.GetBytes(FileName));
		int num = ((Comment != null) ? (array.Length + 1) : 0);
		int num2 = ((FileName != null) ? (array2.Length + 1) : 0);
		byte[] array3 = new byte[10 + num + num2];
		int num3 = 0;
		array3[num3++] = 31;
		array3[num3++] = 139;
		array3[num3++] = 8;
		byte b = 0;
		if (Comment != null)
		{
			b ^= 0x10;
		}
		if (FileName != null)
		{
			b ^= 8;
		}
		array3[num3++] = b;
		if (!LastModified.HasValue)
		{
			LastModified = DateTime.Now;
		}
		int value = (int)(LastModified.Value - UnixEpoch).TotalSeconds;
		DataConverter.LittleEndian.PutBytes(array3, num3, value);
		num3 += 4;
		array3[num3++] = 0;
		array3[num3++] = byte.MaxValue;
		if (num2 != 0)
		{
			Array.Copy(array2, 0, array3, num3, num2 - 1);
			num3 += num2 - 1;
			array3[num3++] = 0;
		}
		if (num != 0)
		{
			Array.Copy(array, 0, array3, num3, num - 1);
			num3 += num - 1;
			array3[num3++] = 0;
		}
		BaseStream._stream.Write(array3, 0, array3.Length);
		return array3.Length;
	}
}
