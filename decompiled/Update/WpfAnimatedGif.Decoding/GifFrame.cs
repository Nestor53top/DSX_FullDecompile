using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfAnimatedGif.Decoding;

internal class GifFrame : GifBlock
{
	internal const int ImageSeparator = 44;

	public GifImageDescriptor Descriptor { get; private set; }

	public GifColor[] LocalColorTable { get; private set; }

	public IList<GifExtension> Extensions { get; private set; }

	public GifImageData ImageData { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

	private GifFrame()
	{
	}

	internal static GifFrame ReadFrame(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		GifFrame gifFrame = new GifFrame();
		gifFrame.Read(stream, controlExtensions, metadataOnly);
		return gifFrame;
	}

	private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		Descriptor = GifImageDescriptor.ReadImageDescriptor(stream);
		if (Descriptor.HasLocalColorTable)
		{
			LocalColorTable = GifHelpers.ReadColorTable(stream, Descriptor.LocalColorTableSize);
		}
		ImageData = GifImageData.ReadImageData(stream, metadataOnly);
		Extensions = controlExtensions.ToList().AsReadOnly();
	}
}
