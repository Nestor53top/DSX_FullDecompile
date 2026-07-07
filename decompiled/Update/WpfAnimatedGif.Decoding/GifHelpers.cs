using System;
using System.IO;
using System.Text;

namespace WpfAnimatedGif.Decoding;

internal static class GifHelpers
{
	public static string ReadString(Stream stream, int length)
	{
		byte[] array = new byte[length];
		stream.ReadAll(array, 0, length);
		return Encoding.ASCII.GetString(array);
	}

	public static byte[] ReadDataBlocks(Stream stream, bool discard)
	{
		MemoryStream memoryStream = (discard ? null : new MemoryStream());
		using (memoryStream)
		{
			int num;
			while ((num = stream.ReadByte()) > 0)
			{
				byte[] buffer = new byte[num];
				stream.ReadAll(buffer, 0, num);
				memoryStream?.Write(buffer, 0, num);
			}
			return memoryStream?.ToArray();
		}
	}

	public static GifColor[] ReadColorTable(Stream stream, int size)
	{
		int num = 3 * size;
		byte[] array = new byte[num];
		stream.ReadAll(array, 0, num);
		GifColor[] array2 = new GifColor[size];
		for (int i = 0; i < size; i++)
		{
			byte r = array[3 * i];
			byte g = array[3 * i + 1];
			byte b = array[3 * i + 2];
			array2[i] = new GifColor(r, g, b);
		}
		return array2;
	}

	public static bool IsNetscapeExtension(GifApplicationExtension ext)
	{
		if (ext.ApplicationIdentifier == "NETSCAPE")
		{
			return Encoding.ASCII.GetString(ext.AuthenticationCode) == "2.0";
		}
		return false;
	}

	public static ushort GetRepeatCount(GifApplicationExtension ext)
	{
		if (ext.Data.Length >= 3)
		{
			return BitConverter.ToUInt16(ext.Data, 1);
		}
		return 1;
	}

	public static Exception UnexpectedEndOfStreamException()
	{
		return new GifDecoderException("Unexpected end of stream before trailer was encountered");
	}

	public static Exception UnknownBlockTypeException(int blockId)
	{
		return new GifDecoderException("Unknown block type: 0x" + blockId.ToString("x2"));
	}

	public static Exception UnknownExtensionTypeException(int extensionLabel)
	{
		return new GifDecoderException("Unknown extension type: 0x" + extensionLabel.ToString("x2"));
	}

	public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize)
	{
		return new GifDecoderException($"Invalid block size for {blockName}. Expected {expectedBlockSize}, but was {actualBlockSize}");
	}

	public static Exception InvalidSignatureException(string signature)
	{
		return new GifDecoderException("Invalid file signature: " + signature);
	}

	public static Exception UnsupportedVersionException(string version)
	{
		return new GifDecoderException("Unsupported version: " + version);
	}

	public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
	{
		for (int i = 0; i < count; i += stream.Read(buffer, offset + i, count - i))
		{
		}
	}
}
