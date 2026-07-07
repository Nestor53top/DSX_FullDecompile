using System;

namespace SharpCompress.Common;

internal class CompressedBytesReadEventArgs : EventArgs
{
	public long CompressedBytesRead { get; internal set; }

	public long CurrentFilePartCompressedBytesRead { get; internal set; }
}
