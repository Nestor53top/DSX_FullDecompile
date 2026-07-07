using System.IO;
using System.Text;

namespace WpfAnimatedGif.Decoding;

internal class GifCommentExtension : GifExtension
{
	internal const int ExtensionLabel = 254;

	public string Text { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

	private GifCommentExtension()
	{
	}

	internal static GifCommentExtension ReadComment(Stream stream)
	{
		GifCommentExtension gifCommentExtension = new GifCommentExtension();
		gifCommentExtension.Read(stream);
		return gifCommentExtension;
	}

	private void Read(Stream stream)
	{
		byte[] array = GifHelpers.ReadDataBlocks(stream, discard: false);
		if (array != null)
		{
			Text = Encoding.ASCII.GetString(array);
		}
	}
}
