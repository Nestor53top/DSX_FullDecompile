using System.Collections.Generic;
using System.IO;

namespace WpfAnimatedGif.Decoding;

internal abstract class GifBlock
{
	internal abstract GifBlockKind Kind { get; }

	internal static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		int num = stream.ReadByte();
		if (num < 0)
		{
			throw GifHelpers.UnexpectedEndOfStreamException();
		}
		return num switch
		{
			33 => GifExtension.ReadExtension(stream, controlExtensions, metadataOnly), 
			44 => GifFrame.ReadFrame(stream, controlExtensions, metadataOnly), 
			59 => GifTrailer.ReadTrailer(), 
			_ => throw GifHelpers.UnknownBlockTypeException(num), 
		};
	}
}
