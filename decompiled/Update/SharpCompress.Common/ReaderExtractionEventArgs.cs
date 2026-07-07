using System;
using SharpCompress.Readers;

namespace SharpCompress.Common;

internal class ReaderExtractionEventArgs<T> : EventArgs
{
	public T Item { get; }

	public ReaderProgress ReaderProgress { get; }

	internal ReaderExtractionEventArgs(T entry, ReaderProgress readerProgress = null)
	{
		Item = entry;
		ReaderProgress = readerProgress;
	}
}
