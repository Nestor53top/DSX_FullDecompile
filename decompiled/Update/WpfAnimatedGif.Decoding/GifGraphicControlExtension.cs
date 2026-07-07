using System;
using System.IO;

namespace WpfAnimatedGif.Decoding;

internal class GifGraphicControlExtension : GifExtension
{
	internal const int ExtensionLabel = 249;

	public int BlockSize { get; private set; }

	public int DisposalMethod { get; private set; }

	public bool UserInput { get; private set; }

	public bool HasTransparency { get; private set; }

	public int Delay { get; private set; }

	public int TransparencyIndex { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.Control;

	private GifGraphicControlExtension()
	{
	}

	internal static GifGraphicControlExtension ReadGraphicsControl(Stream stream)
	{
		GifGraphicControlExtension gifGraphicControlExtension = new GifGraphicControlExtension();
		gifGraphicControlExtension.Read(stream);
		return gifGraphicControlExtension;
	}

	private void Read(Stream stream)
	{
		byte[] array = new byte[6];
		stream.ReadAll(array, 0, array.Length);
		BlockSize = array[0];
		if (BlockSize != 4)
		{
			throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, BlockSize);
		}
		byte b = array[1];
		DisposalMethod = (b & 0x1C) >> 2;
		UserInput = (b & 2) != 0;
		HasTransparency = (b & 1) != 0;
		Delay = BitConverter.ToUInt16(array, 2) * 10;
		TransparencyIndex = array[4];
	}
}
