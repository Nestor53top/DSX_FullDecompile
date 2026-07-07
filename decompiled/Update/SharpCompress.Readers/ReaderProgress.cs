using System;
using SharpCompress.Common;

namespace SharpCompress.Readers;

internal class ReaderProgress
{
	private readonly IEntry _entry;

	public long BytesTransferred { get; }

	public int Iterations { get; }

	public int PercentageRead => (int)Math.Round(PercentageReadExact);

	public double PercentageReadExact => (float)BytesTransferred / (float)_entry.Size * 100f;

	public ReaderProgress(IEntry entry, long bytesTransferred, int iterations)
	{
		_entry = entry;
		BytesTransferred = bytesTransferred;
		Iterations = iterations;
	}
}
