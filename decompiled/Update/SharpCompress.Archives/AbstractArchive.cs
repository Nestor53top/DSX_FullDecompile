using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives;

internal abstract class AbstractArchive<TEntry, TVolume> : IArchive, IDisposable, IArchiveExtractionListener, IExtractionListener where TEntry : IArchiveEntry where TVolume : IVolume
{
	private readonly LazyReadOnlyCollection<TVolume> lazyVolumes;

	private readonly LazyReadOnlyCollection<TEntry> lazyEntries;

	private bool disposed;

	protected ReaderOptions ReaderOptions { get; }

	public ArchiveType Type { get; }

	public virtual ICollection<TEntry> Entries => lazyEntries;

	public ICollection<TVolume> Volumes => lazyVolumes;

	public virtual long TotalSize => Entries.Aggregate(0L, (long total, TEntry cf) => total + cf.CompressedSize);

	public virtual long TotalUncompressSize => Entries.Aggregate(0L, (long total, TEntry cf) => total + cf.Size);

	IEnumerable<IArchiveEntry> IArchive.Entries => Entries.Cast<IArchiveEntry>();

	IEnumerable<IVolume> IArchive.Volumes => lazyVolumes.Cast<IVolume>();

	public virtual bool IsSolid => false;

	public bool IsComplete
	{
		get
		{
			((IArchiveExtractionListener)this).EnsureEntriesLoaded();
			return Entries.All((TEntry x) => x.IsComplete);
		}
	}

	public event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionBegin;

	public event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionEnd;

	public event EventHandler<CompressedBytesReadEventArgs> CompressedBytesRead;

	public event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

	internal AbstractArchive(ArchiveType type, FileInfo fileInfo, ReaderOptions readerOptions)
	{
		Type = type;
		if (!fileInfo.Exists)
		{
			throw new ArgumentException("File does not exist: " + fileInfo.FullName);
		}
		ReaderOptions = readerOptions;
		readerOptions.LeaveStreamOpen = false;
		lazyVolumes = new LazyReadOnlyCollection<TVolume>(LoadVolumes(fileInfo));
		lazyEntries = new LazyReadOnlyCollection<TEntry>(LoadEntries(Volumes));
	}

	protected abstract IEnumerable<TVolume> LoadVolumes(FileInfo file);

	internal AbstractArchive(ArchiveType type, IEnumerable<Stream> streams, ReaderOptions readerOptions)
	{
		Type = type;
		ReaderOptions = readerOptions;
		lazyVolumes = new LazyReadOnlyCollection<TVolume>(LoadVolumes(streams.Select(CheckStreams)));
		lazyEntries = new LazyReadOnlyCollection<TEntry>(LoadEntries(Volumes));
	}

	internal AbstractArchive(ArchiveType type)
	{
		Type = type;
		lazyVolumes = new LazyReadOnlyCollection<TVolume>(Enumerable.Empty<TVolume>());
		lazyEntries = new LazyReadOnlyCollection<TEntry>(Enumerable.Empty<TEntry>());
	}

	void IArchiveExtractionListener.FireEntryExtractionBegin(IArchiveEntry entry)
	{
		this.EntryExtractionBegin?.Invoke(this, new ArchiveExtractionEventArgs<IArchiveEntry>(entry));
	}

	void IArchiveExtractionListener.FireEntryExtractionEnd(IArchiveEntry entry)
	{
		this.EntryExtractionEnd?.Invoke(this, new ArchiveExtractionEventArgs<IArchiveEntry>(entry));
	}

	private static Stream CheckStreams(Stream stream)
	{
		if (!stream.CanSeek || !stream.CanRead)
		{
			throw new ArgumentException("Archive streams must be Readable and Seekable");
		}
		return stream;
	}

	protected abstract IEnumerable<TVolume> LoadVolumes(IEnumerable<Stream> streams);

	protected abstract IEnumerable<TEntry> LoadEntries(IEnumerable<TVolume> volumes);

	public virtual void Dispose()
	{
		if (!disposed)
		{
			lazyVolumes.ForEach(delegate(TVolume v)
			{
				v.Dispose();
			});
			lazyEntries.GetLoaded().Cast<Entry>().ForEach(delegate(Entry x)
			{
				x.Close();
			});
			disposed = true;
		}
	}

	void IArchiveExtractionListener.EnsureEntriesLoaded()
	{
		lazyEntries.EnsureFullyLoaded();
		lazyVolumes.EnsureFullyLoaded();
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

	public IReader ExtractAllEntries()
	{
		((IArchiveExtractionListener)this).EnsureEntriesLoaded();
		return CreateReaderForSolidExtraction();
	}

	protected abstract IReader CreateReaderForSolidExtraction();
}
