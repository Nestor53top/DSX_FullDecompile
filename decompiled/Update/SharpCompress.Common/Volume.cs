using System;
using System.IO;
using SharpCompress.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common;

internal abstract class Volume : IVolume, IDisposable
{
	private readonly Stream actualStream;

	private bool disposed;

	internal Stream Stream => new NonDisposingStream(actualStream);

	protected ReaderOptions ReaderOptions { get; }

	public virtual bool IsFirstVolume => true;

	public virtual bool IsMultiVolume => true;

	internal Volume(Stream stream, ReaderOptions readerOptions)
	{
		actualStream = stream;
		ReaderOptions = readerOptions;
	}

	public void Dispose()
	{
		if (!ReaderOptions.LeaveStreamOpen && !disposed)
		{
			actualStream.Dispose();
			disposed = true;
		}
	}
}
