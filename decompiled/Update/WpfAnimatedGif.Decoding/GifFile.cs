using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfAnimatedGif.Decoding;

internal class GifFile
{
	public GifHeader Header { get; private set; }

	public GifColor[] GlobalColorTable { get; set; }

	public IList<GifFrame> Frames { get; set; }

	public IList<GifExtension> Extensions { get; set; }

	public ushort RepeatCount { get; set; }

	private GifFile()
	{
	}

	internal static GifFile ReadGifFile(Stream stream, bool metadataOnly)
	{
		GifFile gifFile = new GifFile();
		gifFile.Read(stream, metadataOnly);
		return gifFile;
	}

	private void Read(Stream stream, bool metadataOnly)
	{
		Header = GifHeader.ReadHeader(stream);
		if (Header.LogicalScreenDescriptor.HasGlobalColorTable)
		{
			GlobalColorTable = GifHelpers.ReadColorTable(stream, Header.LogicalScreenDescriptor.GlobalColorTableSize);
		}
		ReadFrames(stream, metadataOnly);
		GifApplicationExtension gifApplicationExtension = Extensions.OfType<GifApplicationExtension>().FirstOrDefault(GifHelpers.IsNetscapeExtension);
		if (gifApplicationExtension != null)
		{
			RepeatCount = GifHelpers.GetRepeatCount(gifApplicationExtension);
		}
		else
		{
			RepeatCount = 1;
		}
	}

	private void ReadFrames(Stream stream, bool metadataOnly)
	{
		List<GifFrame> list = new List<GifFrame>();
		List<GifExtension> list2 = new List<GifExtension>();
		List<GifExtension> list3 = new List<GifExtension>();
		while (true)
		{
			GifBlock gifBlock = GifBlock.ReadBlock(stream, list2, metadataOnly);
			if (gifBlock.Kind == GifBlockKind.GraphicRendering)
			{
				list2 = new List<GifExtension>();
			}
			if (gifBlock is GifFrame)
			{
				list.Add((GifFrame)gifBlock);
			}
			else if (gifBlock is GifExtension)
			{
				GifExtension gifExtension = (GifExtension)gifBlock;
				switch (gifExtension.Kind)
				{
				case GifBlockKind.Control:
					list2.Add(gifExtension);
					break;
				case GifBlockKind.SpecialPurpose:
					list3.Add(gifExtension);
					break;
				}
			}
			else if (gifBlock is GifTrailer)
			{
				break;
			}
		}
		Frames = list.AsReadOnly();
		Extensions = list3.AsReadOnly();
	}
}
