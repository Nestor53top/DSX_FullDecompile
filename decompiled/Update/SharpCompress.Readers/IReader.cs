using System;
using System.IO;
using SharpCompress.Common;

namespace SharpCompress.Readers;

internal interface IReader : IDisposable
{
	ArchiveType ArchiveType { get; }

	IEntry Entry { get; }

	bool Cancelled { get; }

	event EventHandler<ReaderExtractionEventArgs<IEntry>> EntryExtractionProgress;

	event EventHandler<CompressedBytesReadEventArgs> CompressedBytesRead;

	event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

	void WriteEntryTo(Stream writableStream);

	void Cancel();

	bool MoveToNextEntry();

	EntryStream OpenEntryStream();
}
