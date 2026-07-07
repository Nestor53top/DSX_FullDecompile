using System;
using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;

namespace SharpCompress.Writers.Zip;

internal class ZipWriterEntryOptions
{
	public CompressionType? CompressionType { get; set; }

	public CompressionLevel? DeflateCompressionLevel { get; set; }

	public string EntryComment { get; set; }

	public DateTime? ModificationDateTime { get; set; }

	public bool? EnableZip64 { get; set; }
}
