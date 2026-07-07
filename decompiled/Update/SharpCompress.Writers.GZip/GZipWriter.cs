using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace SharpCompress.Writers.GZip;

internal class GZipWriter : AbstractWriter
{
	private bool wroteToStream;

	public GZipWriter(Stream destination, bool leaveOpen = false)
		: base(ArchiveType.GZip)
	{
		InitalizeStream(new GZipStream(destination, CompressionMode.Compress, leaveOpen), !leaveOpen);
	}

	protected override void Dispose(bool isDisposing)
	{
		if (isDisposing)
		{
			base.OutputStream.Dispose();
		}
		base.Dispose(isDisposing);
	}

	public override void Write(string filename, Stream source, DateTime? modificationTime)
	{
		if (wroteToStream)
		{
			throw new ArgumentException("Can only write a single stream to a GZip file.");
		}
		GZipStream gZipStream = base.OutputStream as GZipStream;
		gZipStream.FileName = filename;
		gZipStream.LastModified = modificationTime;
		source.TransferTo(gZipStream);
		wroteToStream = true;
	}
}
