using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfAnimatedGif.Decoding;

internal class GifPlainTextExtension : GifExtension
{
	internal const int ExtensionLabel = 1;

	public int BlockSize { get; private set; }

	public int Left { get; private set; }

	public int Top { get; private set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public int CellWidth { get; private set; }

	public int CellHeight { get; private set; }

	public int ForegroundColorIndex { get; private set; }

	public int BackgroundColorIndex { get; private set; }

	public string Text { get; private set; }

	public IList<GifExtension> Extensions { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

	private GifPlainTextExtension()
	{
	}

	internal static GifPlainTextExtension ReadPlainText(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		GifPlainTextExtension gifPlainTextExtension = new GifPlainTextExtension();
		gifPlainTextExtension.Read(stream, controlExtensions, metadataOnly);
		return gifPlainTextExtension;
	}

	private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		byte[] array = new byte[13];
		stream.ReadAll(array, 0, array.Length);
		BlockSize = array[0];
		if (BlockSize != 12)
		{
			throw GifHelpers.InvalidBlockSizeException("Plain Text Extension", 12, BlockSize);
		}
		Left = BitConverter.ToUInt16(array, 1);
		Top = BitConverter.ToUInt16(array, 3);
		Width = BitConverter.ToUInt16(array, 5);
		Height = BitConverter.ToUInt16(array, 7);
		CellWidth = array[9];
		CellHeight = array[10];
		ForegroundColorIndex = array[11];
		BackgroundColorIndex = array[12];
		byte[] bytes = GifHelpers.ReadDataBlocks(stream, metadataOnly);
		Text = Encoding.ASCII.GetString(bytes);
		Extensions = controlExtensions.ToList().AsReadOnly();
	}
}
