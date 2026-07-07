using System;
using System.IO;
using System.Text;

namespace WpfAnimatedGif.Decoding;

internal class GifApplicationExtension : GifExtension
{
	internal const int ExtensionLabel = 255;

	public int BlockSize { get; private set; }

	public string ApplicationIdentifier { get; private set; }

	public byte[] AuthenticationCode { get; private set; }

	public byte[] Data { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

	private GifApplicationExtension()
	{
	}

	internal static GifApplicationExtension ReadApplication(Stream stream)
	{
		GifApplicationExtension gifApplicationExtension = new GifApplicationExtension();
		gifApplicationExtension.Read(stream);
		return gifApplicationExtension;
	}

	private void Read(Stream stream)
	{
		byte[] array = new byte[12];
		stream.ReadAll(array, 0, array.Length);
		BlockSize = array[0];
		if (BlockSize != 11)
		{
			throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, BlockSize);
		}
		ApplicationIdentifier = Encoding.ASCII.GetString(array, 1, 8);
		byte[] array2 = new byte[3];
		Array.Copy(array, 9, array2, 0, 3);
		AuthenticationCode = array2;
		Data = GifHelpers.ReadDataBlocks(stream, discard: false);
	}
}
