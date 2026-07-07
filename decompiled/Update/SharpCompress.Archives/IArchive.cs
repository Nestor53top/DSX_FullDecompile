using System;
using System.Collections.Generic;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives;

internal interface IArchive : IDisposable
{
	IEnumerable<IArchiveEntry> Entries { get; }

	IEnumerable<IVolume> Volumes { get; }

	ArchiveType Type { get; }

	bool IsSolid { get; }

	bool IsComplete { get; }

	long TotalSize { get; }

	long TotalUncompressSize { get; }

	event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionBegin;

	event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionEnd;

	event EventHandler<CompressedBytesReadEventArgs> CompressedBytesRead;

	event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

	IReader ExtractAllEntries();
}
