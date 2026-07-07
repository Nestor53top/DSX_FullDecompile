using System;

namespace SharpCompress.Common;

internal class ArchiveExtractionEventArgs<T> : EventArgs
{
	public T Item { get; }

	internal ArchiveExtractionEventArgs(T entry)
	{
		Item = entry;
	}
}
