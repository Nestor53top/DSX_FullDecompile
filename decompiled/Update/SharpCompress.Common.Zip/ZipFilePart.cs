using System;
using System.IO;
using System.Linq;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.PPMd;
using SharpCompress.Converters;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip;

internal abstract class ZipFilePart : FilePart
{
	internal Stream BaseStream { get; }

	internal ZipFileEntry Header { get; set; }

	internal override string FilePartName => Header.Name;

	protected bool LeaveStreamOpen
	{
		get
		{
			if (!FlagUtility.HasFlag(Header.Flags, HeaderFlags.UsePostDataDescriptor))
			{
				return Header.IsZip64;
			}
			return true;
		}
	}

	internal ZipFilePart(ZipFileEntry header, Stream stream)
	{
		Header = header;
		header.Part = this;
		BaseStream = stream;
	}

	internal override Stream GetCompressedStream()
	{
		if (!Header.HasData)
		{
			return Stream.Null;
		}
		Stream stream = CreateDecompressionStream(GetCryptoStream(CreateBaseStream()), Header.CompressionMethod);
		if (LeaveStreamOpen)
		{
			return new NonDisposingStream(stream);
		}
		return stream;
	}

	internal override Stream GetRawStream()
	{
		if (!Header.HasData)
		{
			return Stream.Null;
		}
		return CreateBaseStream();
	}

	protected abstract Stream CreateBaseStream();

	protected Stream CreateDecompressionStream(Stream stream, ZipCompressionMethod method)
	{
		switch (method)
		{
		case ZipCompressionMethod.None:
			return stream;
		case ZipCompressionMethod.Deflate:
			return new DeflateStream(stream, CompressionMode.Decompress);
		case ZipCompressionMethod.BZip2:
			return new BZip2Stream(stream, CompressionMode.Decompress);
		case ZipCompressionMethod.LZMA:
		{
			if (FlagUtility.HasFlag(Header.Flags, HeaderFlags.Encrypted))
			{
				throw new NotSupportedException("LZMA with pkware encryption.");
			}
			BinaryReader binaryReader = new BinaryReader(stream);
			binaryReader.ReadUInt16();
			byte[] array = new byte[binaryReader.ReadUInt16()];
			binaryReader.Read(array, 0, array.Length);
			return new LzmaStream(array, stream, (Header.CompressedSize > 0) ? (Header.CompressedSize - 4 - array.Length) : (-1), FlagUtility.HasFlag(Header.Flags, HeaderFlags.Bit1) ? (-1) : Header.UncompressedSize);
		}
		case ZipCompressionMethod.PPMd:
		{
			byte[] array2 = new byte[2];
			stream.Read(array2, 0, array2.Length);
			return new PpmdStream(new PpmdProperties(array2), stream, compress: false);
		}
		case ZipCompressionMethod.WinzipAes:
		{
			ExtraData extraData = Header.Extra.Where((ExtraData x) => x.Type == ExtraDataType.WinZipAes).SingleOrDefault();
			if (extraData == null)
			{
				throw new InvalidFormatException("No Winzip AES extra data found.");
			}
			if (extraData.Length != 7)
			{
				throw new InvalidFormatException("Winzip data length is not 7.");
			}
			ushort uInt = DataConverter.LittleEndian.GetUInt16(extraData.DataBytes, 0);
			if (uInt != 1 && uInt != 2)
			{
				throw new InvalidFormatException("Unexpected vendor version number for WinZip AES metadata");
			}
			if (DataConverter.LittleEndian.GetUInt16(extraData.DataBytes, 2) != 17729)
			{
				throw new InvalidFormatException("Unexpected vendor ID for WinZip AES metadata");
			}
			return CreateDecompressionStream(stream, (ZipCompressionMethod)DataConverter.LittleEndian.GetUInt16(extraData.DataBytes, 5));
		}
		default:
			throw new NotSupportedException("CompressionMethod: " + Header.CompressionMethod);
		}
	}

	protected Stream GetCryptoStream(Stream plainStream)
	{
		bool flag = FlagUtility.HasFlag(Header.Flags, HeaderFlags.Encrypted);
		if (Header.CompressedSize == 0 && flag)
		{
			throw new NotSupportedException("Cannot encrypt file with unknown size at start.");
		}
		plainStream = (((Header.CompressedSize != 0L || !FlagUtility.HasFlag(Header.Flags, HeaderFlags.UsePostDataDescriptor)) && !Header.IsZip64) ? ((Stream)new ReadOnlySubStream(plainStream, Header.CompressedSize)) : ((Stream)new NonDisposingStream(plainStream)));
		if (flag)
		{
			switch (Header.CompressionMethod)
			{
			case ZipCompressionMethod.None:
			case ZipCompressionMethod.Deflate:
			case ZipCompressionMethod.Deflate64:
			case ZipCompressionMethod.BZip2:
			case ZipCompressionMethod.LZMA:
			case ZipCompressionMethod.PPMd:
				return new PkwareTraditionalCryptoStream(plainStream, Header.ComposeEncryptionData(plainStream), CryptoMode.Decrypt);
			case ZipCompressionMethod.WinzipAes:
				if (Header.WinzipAesEncryptionData != null)
				{
					return new WinzipAesCryptoStream(plainStream, Header.WinzipAesEncryptionData, Header.CompressedSize - 10);
				}
				return plainStream;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return plainStream;
	}
}
