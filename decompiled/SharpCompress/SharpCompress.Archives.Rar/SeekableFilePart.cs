using System.IO;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Archives.Rar;

internal class SeekableFilePart : RarFilePart
{
	private readonly Stream stream;

	private readonly string password;

	internal override string FilePartName => "Unknown Stream - File Entry: " + base.FileHeader.FileName;

	internal SeekableFilePart(MarkHeader mh, FileHeader fh, Stream stream, string password)
		: base(mh, fh)
	{
		this.stream = stream;
		this.password = password;
	}

	internal override Stream GetCompressedStream()
	{
		stream.Position = base.FileHeader.DataStartPosition;
		if (base.FileHeader.Salt != null)
		{
			return new RarCryptoWrapper(stream, password, base.FileHeader.Salt);
		}
		return stream;
	}
}
