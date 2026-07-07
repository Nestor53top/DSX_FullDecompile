using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;

namespace SharpCompress.Readers;

internal abstract class AbstractReader<TEntry, TVolume> : IReader, IDisposable, IReaderExtractionListener, IExtractionListener where TEntry : Entry where TVolume : Volume
{
	private bool completed;

	private IEnumerator<TEntry> entriesForCurrentReadStream;

	private bool wroteCurrentEntry;

	private readonly byte[] skipBuffer = new byte[4096];

	internal ReaderOptions Options { get; }

	public ArchiveType ArchiveType { get; }

	public abstract TVolume Volume { get; }

	public TEntry Entry => entriesForCurrentReadStream.Current;

	public bool Cancelled { get; private set; }

	IEntry IReader.Entry => Entry;

	public event EventHandler<ReaderExtractionEventArgs<IEntry>> EntryExtractionProgress;

	public event EventHandler<CompressedBytesReadEventArgs> CompressedBytesRead;

	public event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

	internal AbstractReader(ReaderOptions options, ArchiveType archiveType)
	{
		ArchiveType = archiveType;
		Options = options;
	}

	public void Dispose()
	{
		if (entriesForCurrentReadStream != null)
		{
			entriesForCurrentReadStream.Dispose();
		}
		Volume.Dispose();
	}

	public void Cancel()
	{
		if (!completed)
		{
			Cancelled = true;
		}
	}

	public bool MoveToNextEntry()
	{
		if (completed)
		{
			return false;
		}
		if (Cancelled)
		{
			throw new InvalidOperationException("Reader has been cancelled.");
		}
		if (entriesForCurrentReadStream == null)
		{
			return LoadStreamForReading(RequestInitialStream());
		}
		if (!wroteCurrentEntry)
		{
			SkipEntry();
		}
		wroteCurrentEntry = false;
		if (NextEntryForCurrentStream())
		{
			return true;
		}
		completed = true;
		return false;
	}

	internal bool LoadStreamForReading(Stream stream)
	{
		if (entriesForCurrentReadStream != null)
		{
			entriesForCurrentReadStream.Dispose();
		}
		if (stream == null || !stream.CanRead)
		{
			throw new MultipartStreamRequiredException("File is split into multiple archives: '" + Entry.Key + "'. A new readable stream is required.  Use Cancel if it was intended.");
		}
		entriesForCurrentReadStream = GetEntries(stream).GetEnumerator();
		if (entriesForCurrentReadStream.MoveNext())
		{
			return true;
		}
		return false;
	}

	internal virtual Stream RequestInitialStream()
	{
		return Volume.Stream;
	}

	internal virtual bool NextEntryForCurrentStream()
	{
		return entriesForCurrentReadStream.MoveNext();
	}

	internal abstract IEnumerable<TEntry> GetEntries(Stream stream);

	private void SkipEntry()
	{
		if (!Entry.IsDirectory)
		{
			Skip();
		}
	}

	private void Skip()
	{
		if (ArchiveType != ArchiveType.Rar && !Entry.IsSolid && Entry.CompressedSize > 0)
		{
			Stream rawStream = Entry.Parts.First().GetRawStream();
			if (rawStream != null)
			{
				long compressedSize = Entry.CompressedSize;
				for (int i = 0; i < compressedSize / skipBuffer.Length; i++)
				{
					rawStream.Read(skipBuffer, 0, skipBuffer.Length);
				}
				rawStream.Read(skipBuffer, 0, (int)(compressedSize % skipBuffer.Length));
				return;
			}
		}
		using EntryStream entryStream = OpenEntryStream();
		while (entryStream.Read(skipBuffer, 0, skipBuffer.Length) > 0)
		{
		}
	}

	public void WriteEntryTo(Stream writableStream)
	{
		if (wroteCurrentEntry)
		{
			throw new ArgumentException("WriteEntryTo or OpenEntryStream can only be called once.");
		}
		if (writableStream == null || !writableStream.CanWrite)
		{
			throw new ArgumentNullException("A writable Stream was required.  Use Cancel if that was intended.");
		}
		Write(writableStream);
		wroteCurrentEntry = true;
	}

	internal void Write(Stream writeStream)
	{
		using Stream source = OpenEntryStream();
		source.TransferTo(writeStream, Entry, this);
	}

	public EntryStream OpenEntryStream()
	{
		if (wroteCurrentEntry)
		{
			throw new ArgumentException("WriteEntryTo or OpenEntryStream can only be called once.");
		}
		EntryStream entryStream = GetEntryStream();
		wroteCurrentEntry = true;
		return entryStream;
	}

	protected EntryStream CreateEntryStream(Stream decompressed)
	{
		return new EntryStream(this, decompressed);
	}

	protected virtual EntryStream GetEntryStream()
	{
		return CreateEntryStream(Entry.Parts.First().GetCompressedStream());
	}

	void IExtractionListener.FireCompressedBytesRead(long currentPartCompressedBytes, long compressedReadBytes)
	{
		this.CompressedBytesRead?.Invoke(this, new CompressedBytesReadEventArgs
		{
			CurrentFilePartCompressedBytesRead = currentPartCompressedBytes,
			CompressedBytesRead = compressedReadBytes
		});
	}

	void IExtractionListener.FireFilePartExtractionBegin(string name, long size, long compressedSize)
	{
		this.FilePartExtractionBegin?.Invoke(this, new FilePartExtractionBeginEventArgs
		{
			CompressedSize = compressedSize,
			Size = size,
			Name = name
		});
	}

	void IReaderExtractionListener.FireEntryExtractionProgress(Entry entry, long bytesTransferred, int iterations)
	{
		this.EntryExtractionProgress?.Invoke(this, new ReaderExtractionEventArgs<IEntry>(entry, new ReaderProgress(entry, bytesTransferred, iterations)));
	}
}
