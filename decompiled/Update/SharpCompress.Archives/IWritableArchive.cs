using System;
using System.IO;
using SharpCompress.Writers;

namespace SharpCompress.Archives;

internal interface IWritableArchive : IArchive, IDisposable
{
	void RemoveEntry(IArchiveEntry entry);

	IArchiveEntry AddEntry(string key, Stream source, bool closeStream, long size = 0L, DateTime? modified = null);

	void SaveTo(Stream stream, WriterOptions options);
}
