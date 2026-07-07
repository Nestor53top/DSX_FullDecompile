using System;
using System.IO;

namespace WpfAnimatedGif.Decoding;

internal class GifImageDescriptor
{
	public int Left { get; private set; }

	public int Top { get; private set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public bool HasLocalColorTable { get; private set; }

	public bool Interlace { get; private set; }

	public bool IsLocalColorTableSorted { get; private set; }

	public int LocalColorTableSize { get; private set; }

	private GifImageDescriptor()
	{
	}

	internal static GifImageDescriptor ReadImageDescriptor(Stream stream)
	{
		GifImageDescriptor gifImageDescriptor = new GifImageDescriptor();
		gifImageDescriptor.Read(stream);
		return gifImageDescriptor;
	}

	private void Read(Stream stream)
	{
		byte[] array = new byte[9];
		stream.ReadAll(array, 0, array.Length);
		Left = BitConverter.ToUInt16(array, 0);
		Top = BitConverter.ToUInt16(array, 2);
		Width = BitConverter.ToUInt16(array, 4);
		Height = BitConverter.ToUInt16(array, 6);
		byte b = array[8];
		HasLocalColorTable = (b & 0x80) != 0;
		Interlace = (b & 0x40) != 0;
		IsLocalColorTableSorted = (b & 0x20) != 0;
		LocalColorTableSize = 1 << (b & 7) + 1;
	}
}
