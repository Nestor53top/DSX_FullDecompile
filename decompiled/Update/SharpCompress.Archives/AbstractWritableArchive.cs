using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace SharpCompress.Archives;

internal abstract class AbstractWritableArchive<TEntry, TVolume> : AbstractArchive<TEntry, TVolume>, IWritableArchive, IArchive, IDisposable where TEntry : IArchiveEntry where TVolume : IVolume
{
	private readonly List<TEntry> newEntries = new List<TEntry>();

	private readonly List<TEntry> removedEntries = new List<TEntry>();

	private readonly List<TEntry> modifiedEntries = new List<TEntry>();

	private bool hasModifications;

	public override ICollection<TEntry> Entries
	{
		get
		{
			if (hasModifications)
			{
				return modifiedEntries;
			}
			return base.Entries;
		}
	}

	private IEnumerable<TEntry> OldEntries => base.Entries.Where((TEntry x) => !removedEntries.Contains(x));

	internal AbstractWritableArchive(ArchiveType type)
		: base(type)
	{
	}

	internal AbstractWritableArchive(ArchiveType type, Stream stream, ReaderOptions readerFactoryOptions)
		: base(type, stream.AsEnumerable(), readerFactoryOptions)
	{
	}

	internal AbstractWritableArchive(ArchiveType type, FileInfo fileInfo, ReaderOptions readerFactoryOptions)
		: base(type, fileInfo, readerFactoryOptions)
	{
	}

	private void RebuildModifiedCollection()
	{
		hasModifications = true;
		newEntries.RemoveAll((TEntry v) => removedEntries.Contains(v));
		modifiedEntries.Clear();
		modifiedEntries.AddRange(OldEntries.Concat(newEntries));
	}

	public void RemoveEntry(TEntry entry)
	{
		if (!removedEntries.Contains(entry))
		{
			removedEntries.Add(entry);
			RebuildModifiedCollection();
		}
	}

	void IWritableArchive.RemoveEntry(IArchiveEntry entry)
	{
		RemoveEntry((TEntry)entry);
	}

	public TEntry AddEntry(string key, Stream source, long size = 0L, DateTime? modified = null)
	{
		return AddEntry(key, source, closeStream: false, size, modified);
	}

	IArchiveEntry IWritableArchive.AddEntry(string key, Stream source, bool closeStream, long size, DateTime? modified)
	{
		return AddEntry(key, source, closeStream, size, modified);
	}

	public TEntry AddEntry(string key, Stream source, bool closeStream, long size = 0L, DateTime? modified = null)
	{
		if (key.StartsWith("/") || key.StartsWith("\\"))
		{
			key = key.Substring(1);
		}
		if (DoesKeyMatchExisting(key))
		{
			throw new ArchiveException("Cannot add entry with duplicate key: " + key);
		}
		TEntry val = CreateEntry(key, source, size, modified, closeStream);
		newEntries.Add(val);
		RebuildModifiedCollection();
		return val;
	}

	private bool DoesKeyMatchExisting(string key)
	{
		using (IEnumerator<string> enumerator = Entries.Select((TEntry x) => x.Key).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string text = enumerator.Current.Replace('/', '\\');
				if (text.StartsWith("\\"))
				{
					text = text.Substring(1);
				}
				return string.Equals(text, key, StringComparison.OrdinalIgnoreCase);
			}
		}
		return false;
	}

	public void SaveTo(Stream stream, WriterOptions options)
	{
		newEntries.Cast<IWritableArchiveEntry>().ForEach(delegate(IWritableArchiveEntry x)
		{
			x.Stream.Seek(0L, SeekOrigin.Begin);
		});
		SaveTo(stream, options, OldEntries, newEntries);
	}

	protected TEntry CreateEntry(string key, Stream source, long size, DateTime? modified, bool closeStream)
	{
		if (!source.CanRead || !source.CanSeek)
		{
			throw new ArgumentException("Streams must be readable and seekable to use the Writing Archive API");
		}
		return CreateEntryInternal(key, source, size, modified, closeStream);
	}

	protected abstract TEntry CreateEntryInternal(string key, Stream source, long size, DateTime? modified, bool closeStream);

	protected abstract void SaveTo(Stream stream, WriterOptions options, IEnumerable<TEntry> oldEntries, IEnumerable<TEntry> newEntries);

	public override void Dispose()
	{
		base.Dispose();
		newEntries.Cast<Entry>().ForEach(delegate(Entry x)
		{
			x.Close();
		});
		removedEntries.Cast<Entry>().ForEach(delegate(Entry x)
		{
			x.Close();
		});
		modifiedEntries.Cast<Entry>().ForEach(delegate(Entry x)
		{
			x.Close();
		});
	}
}
