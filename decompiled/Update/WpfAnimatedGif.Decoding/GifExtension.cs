using System.Collections.Generic;
using System.IO;

namespace WpfAnimatedGif.Decoding;

internal abstract class GifExtension : GifBlock
{
	internal const int ExtensionIntroducer = 33;

	internal static GifExtension ReadExtension(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
	{
		int num = stream.ReadByte();
		if (num < 0)
		{
			throw GifHelpers.UnexpectedEndOfStreamException();
		}
		return num switch
		{
			249 => GifGraphicControlExtension.ReadGraphicsControl(stream), 
			254 => GifCommentExtension.ReadComment(stream), 
			1 => GifPlainTextExtension.ReadPlainText(stream, controlExtensions, metadataOnly), 
			255 => GifApplicationExtension.ReadApplication(stream), 
			_ => throw GifHelpers.UnknownExtensionTypeException(num), 
		};
	}
}
