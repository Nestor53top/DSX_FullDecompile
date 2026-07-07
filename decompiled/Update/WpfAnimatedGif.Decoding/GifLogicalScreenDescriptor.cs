using System;
using System.IO;

namespace WpfAnimatedGif.Decoding;

internal class GifLogicalScreenDescriptor
{
	public int Width { get; private set; }

	public int Height { get; private set; }

	public bool HasGlobalColorTable { get; private set; }

	public int ColorResolution { get; private set; }

	public bool IsGlobalColorTableSorted { get; private set; }

	public int GlobalColorTableSize { get; private set; }

	public int BackgroundColorIndex { get; private set; }

	public double PixelAspectRatio { get; private set; }

	internal static GifLogicalScreenDescriptor ReadLogicalScreenDescriptor(Stream stream)
	{
		GifLogicalScreenDescriptor gifLogicalScreenDescriptor = new GifLogicalScreenDescriptor();
		gifLogicalScreenDescriptor.Read(stream);
		return gifLogicalScreenDescriptor;
	}

	private void Read(Stream stream)
	{
		byte[] array = new byte[7];
		stream.ReadAll(array, 0, array.Length);
		Width = BitConverter.ToUInt16(array, 0);
		Height = BitConverter.ToUInt16(array, 2);
		byte b = array[4];
		HasGlobalColorTable = (b & 0x80) != 0;
		ColorResolution = ((b & 0x70) >> 4) + 1;
		IsGlobalColorTableSorted = (b & 8) != 0;
		GlobalColorTableSize = 1 << (b & 7) + 1;
		BackgroundColorIndex = array[5];
		PixelAspectRatio = ((array[5] == 0) ? 0.0 : ((double)(15 + array[5]) / 64.0));
	}
}
