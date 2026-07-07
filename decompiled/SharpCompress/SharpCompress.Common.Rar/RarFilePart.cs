using System.IO;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Common.Rar;

internal abstract class RarFilePart : FilePart
{
	internal MarkHeader MarkHeader { get; }

	internal FileHeader FileHeader { get; }

	internal RarFilePart(MarkHeader mh, FileHeader fh)
	{
		MarkHeader = mh;
		FileHeader = fh;
	}

	internal override Stream GetRawStream()
	{
		return null;
	}
}
