using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.PPMd;
using SharpCompress.Converters;
using SharpCompress.IO;

namespace SharpCompress.Writers.Zip;

internal class ZipWriter : AbstractWriter
{
	internal class ZipWritingStream : Stream
	{
		private readonly CRC32 crc = new CRC32();

		private readonly ZipCentralDirectoryEntry entry;

		private readonly Stream originalStream;

		private readonly Stream writeStream;

		private readonly ZipWriter writer;

		private readonly ZipCompressionMethod zipCompressionMethod;

		private readonly CompressionLevel compressionLevel;

		private CountingWritableSubStream counting;

		private ulong decompressed;

		private bool limitsExceeded;

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

		internal ZipWritingStream(ZipWriter writer, Stream originalStream, ZipCentralDirectoryEntry entry, ZipCompressionMethod zipCompressionMethod, CompressionLevel compressionLevel)
		{
			this.writer = writer;
			this.originalStream = originalStream;
			this.writer = writer;
			this.entry = entry;
			this.zipCompressionMethod = zipCompressionMethod;
			this.compressionLevel = compressionLevel;
			writeStream = GetWriteStream(originalStream);
		}

		private Stream GetWriteStream(Stream writeStream)
		{
			counting = new CountingWritableSubStream(writeStream);
			Stream result = counting;
			switch (zipCompressionMethod)
			{
			case ZipCompressionMethod.None:
				return result;
			case ZipCompressionMethod.Deflate:
				return new DeflateStream(counting, CompressionMode.Compress, compressionLevel, leaveOpen: true);
			case ZipCompressionMethod.BZip2:
				return new BZip2Stream(counting, CompressionMode.Compress, leaveOpen: true);
			case ZipCompressionMethod.LZMA:
			{
				counting.WriteByte(9);
				counting.WriteByte(20);
				counting.WriteByte(5);
				counting.WriteByte(0);
				LzmaStream lzmaStream = new LzmaStream(new LzmaEncoderProperties(!originalStream.CanSeek), isLZMA2: false, counting);
				counting.Write(lzmaStream.Properties, 0, lzmaStream.Properties.Length);
				return lzmaStream;
			}
			case ZipCompressionMethod.PPMd:
				counting.Write(writer.PpmdProperties.Properties, 0, 2);
				return new PpmdStream(writer.PpmdProperties, counting, compress: true);
			default:
				throw new NotSupportedException("CompressionMethod: " + zipCompressionMethod);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			writeStream.Dispose();
			if (limitsExceeded)
			{
				originalStream.Dispose();
				return;
			}
			entry.Crc = (uint)crc.Crc32Result;
			entry.Compressed = counting.Count;
			entry.Decompressed = decompressed;
			bool flag = entry.Compressed >= uint.MaxValue || entry.Decompressed >= uint.MaxValue;
			uint compressed = (uint)(flag ? uint.MaxValue : counting.Count);
			uint uncompressed = (uint)(flag ? uint.MaxValue : entry.Decompressed);
			if (originalStream.CanSeek)
			{
				originalStream.Position = (long)(entry.HeaderOffset + 6);
				originalStream.WriteByte(0);
				originalStream.Position = (long)(entry.HeaderOffset + 14);
				writer.WriteFooter(entry.Crc, compressed, uncompressed);
				if (flag && entry.Zip64HeaderOffset == 0)
				{
					throw new NotSupportedException("Attempted to write a stream that is larger than 4GiB without setting the zip64 option");
				}
				if (entry.Zip64HeaderOffset != 0)
				{
					originalStream.Position = (long)(entry.HeaderOffset + entry.Zip64HeaderOffset);
					originalStream.Write(DataConverter.LittleEndian.GetBytes((ushort)1), 0, 2);
					originalStream.Write(DataConverter.LittleEndian.GetBytes((ushort)16), 0, 2);
					originalStream.Write(DataConverter.LittleEndian.GetBytes(entry.Decompressed), 0, 8);
					originalStream.Write(DataConverter.LittleEndian.GetBytes(entry.Compressed), 0, 8);
				}
				originalStream.Position = writer.streamPosition + (long)entry.Compressed;
				writer.streamPosition += (long)entry.Compressed;
			}
			else
			{
				if (flag)
				{
					throw new NotSupportedException("Streams larger than 4GiB are not supported for non-seekable streams");
				}
				originalStream.Write(DataConverter.LittleEndian.GetBytes(134695760u), 0, 4);
				writer.WriteFooter(entry.Crc, compressed, uncompressed);
				writer.streamPosition += (long)(entry.Compressed + 16);
			}
			writer.entries.Add(entry);
		}

		public override void Flush()
		{
			writeStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
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
			if (entry.Zip64HeaderOffset == 0 && (limitsExceeded || decompressed + (uint)count > uint.MaxValue || counting.Count + (uint)count > uint.MaxValue))
			{
				throw new NotSupportedException("Attempted to write a stream that is larger than 4GiB without setting the zip64 option");
			}
			decompressed += (uint)count;
			crc.SlurpBlock(buffer, offset, count);
			writeStream.Write(buffer, offset, count);
			if (entry.Zip64HeaderOffset == 0 && (decompressed > uint.MaxValue || counting.Count > uint.MaxValue))
			{
				limitsExceeded = true;
				throw new NotSupportedException("Attempted to write a stream that is larger than 4GiB without setting the zip64 option");
			}
		}
	}

	private readonly CompressionType compressionType;

	private readonly CompressionLevel compressionLevel;

	private readonly List<ZipCentralDirectoryEntry> entries = new List<ZipCentralDirectoryEntry>();

	private readonly string zipComment;

	private long streamPosition;

	private PpmdProperties ppmdProps;

	private readonly bool isZip64;

	private PpmdProperties PpmdProperties
	{
		get
		{
			if (ppmdProps == null)
			{
				ppmdProps = new PpmdProperties();
			}
			return ppmdProps;
		}
	}

	public ZipWriter(Stream destination, ZipWriterOptions zipWriterOptions)
		: base(ArchiveType.Zip)
	{
		zipComment = zipWriterOptions.ArchiveComment ?? string.Empty;
		isZip64 = zipWriterOptions.UseZip64;
		if (destination.CanSeek)
		{
			streamPosition = destination.Position;
		}
		compressionType = zipWriterOptions.CompressionType;
		compressionLevel = zipWriterOptions.DeflateCompressionLevel;
		InitalizeStream(destination, !zipWriterOptions.LeaveStreamOpen);
	}

	protected override void Dispose(bool isDisposing)
	{
		if (isDisposing)
		{
			ulong num = 0uL;
			foreach (ZipCentralDirectoryEntry entry in entries)
			{
				num += entry.Write(base.OutputStream);
			}
			WriteEndRecord(num);
		}
		base.Dispose(isDisposing);
	}

	private static ZipCompressionMethod ToZipCompressionMethod(CompressionType compressionType)
	{
		return compressionType switch
		{
			CompressionType.None => ZipCompressionMethod.None, 
			CompressionType.Deflate => ZipCompressionMethod.Deflate, 
			CompressionType.BZip2 => ZipCompressionMethod.BZip2, 
			CompressionType.LZMA => ZipCompressionMethod.LZMA, 
			CompressionType.PPMd => ZipCompressionMethod.PPMd, 
			_ => throw new InvalidFormatException("Invalid compression method: " + compressionType), 
		};
	}

	public override void Write(string entryPath, Stream source, DateTime? modificationTime)
	{
		Write(entryPath, source, new ZipWriterEntryOptions
		{
			ModificationDateTime = modificationTime
		});
	}

	public void Write(string entryPath, Stream source, ZipWriterEntryOptions zipWriterEntryOptions)
	{
		using Stream destination = WriteToStream(entryPath, zipWriterEntryOptions);
		source.TransferTo(destination);
	}

	public Stream WriteToStream(string entryPath, ZipWriterEntryOptions options)
	{
		ZipCompressionMethod zipCompressionMethod = ToZipCompressionMethod(options.CompressionType ?? compressionType);
		entryPath = NormalizeFilename(entryPath);
		options.ModificationDateTime = options.ModificationDateTime ?? DateTime.Now;
		options.EntryComment = options.EntryComment ?? string.Empty;
		ZipCentralDirectoryEntry entry = new ZipCentralDirectoryEntry(zipCompressionMethod, entryPath, (ulong)streamPosition)
		{
			Comment = options.EntryComment,
			ModificationTime = options.ModificationDateTime
		};
		bool value = isZip64;
		if (options.EnableZip64.HasValue)
		{
			value = options.EnableZip64.Value;
		}
		uint num = (uint)WriteHeader(entryPath, options, entry, value);
		streamPosition += num;
		return new ZipWritingStream(this, base.OutputStream, entry, zipCompressionMethod, options.DeflateCompressionLevel ?? compressionLevel);
	}

	private string NormalizeFilename(string filename)
	{
		filename = filename.Replace('\\', '/');
		int num = filename.IndexOf(':');
		if (num >= 0)
		{
			filename = filename.Remove(0, num + 1);
		}
		return filename.Trim(new char[1] { '/' });
	}

	private int WriteHeader(string filename, ZipWriterEntryOptions zipWriterEntryOptions, ZipCentralDirectoryEntry entry, bool useZip64)
	{
		if (!base.OutputStream.CanSeek && useZip64)
		{
			throw new NotSupportedException("Zip64 extensions are not supported on non-seekable streams");
		}
		ZipCompressionMethod zipCompressionMethod = ToZipCompressionMethod(zipWriterEntryOptions.CompressionType ?? compressionType);
		byte[] bytes = ArchiveEncoding.Default.GetBytes(filename);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(67324752u), 0, 4);
		if (zipCompressionMethod == ZipCompressionMethod.Deflate)
		{
			if (base.OutputStream.CanSeek && useZip64)
			{
				base.OutputStream.Write(new byte[2] { 45, 0 }, 0, 2);
			}
			else
			{
				base.OutputStream.Write(new byte[2] { 20, 0 }, 0, 2);
			}
		}
		else
		{
			base.OutputStream.Write(new byte[2] { 63, 0 }, 0, 2);
		}
		HeaderFlags headerFlags = ((ArchiveEncoding.Default == Encoding.UTF8) ? HeaderFlags.UTF8 : ((HeaderFlags)0));
		if (!base.OutputStream.CanSeek)
		{
			headerFlags |= HeaderFlags.UsePostDataDescriptor;
			if (zipCompressionMethod == ZipCompressionMethod.LZMA)
			{
				headerFlags |= HeaderFlags.Bit1;
			}
		}
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)headerFlags), 0, 2);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)zipCompressionMethod), 0, 2);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(zipWriterEntryOptions.ModificationDateTime.DateTimeToDosTime()), 0, 4);
		base.OutputStream.Write(new byte[12], 0, 12);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)bytes.Length), 0, 2);
		int num = 0;
		if (base.OutputStream.CanSeek && useZip64)
		{
			num = 20;
		}
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)num), 0, 2);
		base.OutputStream.Write(bytes, 0, bytes.Length);
		if (num != 0)
		{
			base.OutputStream.Write(new byte[num], 0, num);
			entry.Zip64HeaderOffset = (ushort)(30 + bytes.Length);
		}
		return 30 + bytes.Length + num;
	}

	private void WriteFooter(uint crc, uint compressed, uint uncompressed)
	{
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(crc), 0, 4);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(compressed), 0, 4);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(uncompressed), 0, 4);
	}

	private void WriteEndRecord(ulong size)
	{
		byte[] bytes = ArchiveEncoding.Default.GetBytes(zipComment);
		bool num = isZip64 || entries.Count > 65535 || streamPosition >= uint.MaxValue || size >= uint.MaxValue;
		uint value = (uint)((size >= uint.MaxValue) ? uint.MaxValue : size);
		uint num2 = (uint)((streamPosition >= uint.MaxValue) ? uint.MaxValue : streamPosition);
		if (num)
		{
			int num3 = 44;
			base.OutputStream.Write(new byte[4] { 80, 75, 6, 6 }, 0, 4);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ulong)num3), 0, 8);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)0), 0, 2);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)45), 0, 2);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(0u), 0, 4);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(0u), 0, 4);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ulong)entries.Count), 0, 8);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ulong)entries.Count), 0, 8);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(size), 0, 8);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ulong)streamPosition), 0, 8);
			base.OutputStream.Write(new byte[4] { 80, 75, 6, 7 }, 0, 4);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(0uL), 0, 4);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ulong)streamPosition + size), 0, 8);
			base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(0u), 0, 4);
			streamPosition += num3 + 20;
			num2 = ((streamPosition >= uint.MaxValue) ? uint.MaxValue : num2);
		}
		base.OutputStream.Write(new byte[8] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)entries.Count), 0, 2);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)entries.Count), 0, 2);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(value), 0, 4);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes(num2), 0, 4);
		base.OutputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)bytes.Length), 0, 2);
		base.OutputStream.Write(bytes, 0, bytes.Length);
	}
}
