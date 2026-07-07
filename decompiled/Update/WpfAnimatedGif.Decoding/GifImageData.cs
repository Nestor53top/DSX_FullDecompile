using System.IO;

namespace WpfAnimatedGif.Decoding;

internal class GifImageData
{
	public byte LzwMinimumCodeSize { get; set; }

	public byte[] CompressedData { get; set; }

	private GifImageData()
	{
	}

	internal static GifImageData ReadImageData(Stream stream, bool metadataOnly)
	{
		GifImageData gifImageData = new GifImageData();
		gifImageData.Read(stream, metadataOnly);
		return gifImageData;
	}

	private void Read(Stream stream, bool metadataOnly)
	{
		LzwMinimumCodeSize = (byte)stream.ReadByte();
		CompressedData = GifHelpers.ReadDataBlocks(stream, metadataOnly);
	}
}
