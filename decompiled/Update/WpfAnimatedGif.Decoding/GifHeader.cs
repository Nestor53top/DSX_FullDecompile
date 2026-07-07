using System.IO;

namespace WpfAnimatedGif.Decoding;

internal class GifHeader : GifBlock
{
	public string Signature { get; private set; }

	public string Version { get; private set; }

	public GifLogicalScreenDescriptor LogicalScreenDescriptor { get; private set; }

	internal override GifBlockKind Kind => GifBlockKind.Other;

	private GifHeader()
	{
	}

	internal static GifHeader ReadHeader(Stream stream)
	{
		GifHeader gifHeader = new GifHeader();
		gifHeader.Read(stream);
		return gifHeader;
	}

	private void Read(Stream stream)
	{
		Signature = GifHelpers.ReadString(stream, 3);
		if (Signature != "GIF")
		{
			throw GifHelpers.InvalidSignatureException(Signature);
		}
		Version = GifHelpers.ReadString(stream, 3);
		if (Version != "87a" && Version != "89a")
		{
			throw GifHelpers.UnsupportedVersionException(Version);
		}
		LogicalScreenDescriptor = GifLogicalScreenDescriptor.ReadLogicalScreenDescriptor(stream);
	}
}
