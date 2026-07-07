using System;
using System.Collections.Generic;

namespace SharpCompress.Common;

internal abstract class Entry : IEntry
{
	public abstract long Crc { get; }

	public abstract string Key { get; }

	public abstract long CompressedSize { get; }

	public abstract CompressionType CompressionType { get; }

	public abstract long Size { get; }

	public abstract DateTime? LastModifiedTime { get; }

	public abstract DateTime? CreatedTime { get; }

	public abstract DateTime? LastAccessedTime { get; }

	public abstract DateTime? ArchivedTime { get; }

	public abstract bool IsEncrypted { get; }

	public abstract bool IsDirectory { get; }

	public abstract bool IsSplit { get; }

	internal abstract IEnumerable<FilePart> Parts { get; }

	internal bool IsSolid { get; set; }

	public virtual int? Attrib
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal virtual void Close()
	{
	}
}
