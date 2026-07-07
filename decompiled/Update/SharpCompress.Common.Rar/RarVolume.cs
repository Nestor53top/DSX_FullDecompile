using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common.Rar;

internal abstract class RarVolume : Volume
{
	private readonly RarHeaderFactory headerFactory;

	internal StreamingMode Mode => headerFactory.StreamingMode;

	internal ArchiveHeader ArchiveHeader { get; private set; }

	public override bool IsFirstVolume
	{
		get
		{
			EnsureArchiveHeaderLoaded();
			return ArchiveHeader.ArchiveHeaderFlags.HasFlag(ArchiveFlags.FIRSTVOLUME);
		}
	}

	public override bool IsMultiVolume
	{
		get
		{
			EnsureArchiveHeaderLoaded();
			return ArchiveHeader.ArchiveHeaderFlags.HasFlag(ArchiveFlags.VOLUME);
		}
	}

	public bool IsSolidArchive
	{
		get
		{
			EnsureArchiveHeaderLoaded();
			return ArchiveHeader.ArchiveHeaderFlags.HasFlag(ArchiveFlags.SOLID);
		}
	}

	internal RarVolume(StreamingMode mode, Stream stream, ReaderOptions options)
		: base(stream, options)
	{
		headerFactory = new RarHeaderFactory(mode, options);
	}

	internal abstract IEnumerable<RarFilePart> ReadFileParts();

	internal abstract RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader);

	internal IEnumerable<RarFilePart> GetVolumeFileParts()
	{
		MarkHeader previousMarkHeader = null;
		foreach (RarHeader item in headerFactory.ReadHeaders(base.Stream))
		{
			switch (item.HeaderType)
			{
			case HeaderType.ArchiveHeader:
				ArchiveHeader = item as ArchiveHeader;
				break;
			case HeaderType.MarkHeader:
				previousMarkHeader = item as MarkHeader;
				break;
			case HeaderType.FileHeader:
			{
				FileHeader fileHeader = item as FileHeader;
				yield return CreateFilePart(fileHeader, previousMarkHeader);
				break;
			}
			}
		}
	}

	private void EnsureArchiveHeaderLoaded()
	{
		if (ArchiveHeader == null)
		{
			if (Mode == StreamingMode.Streaming)
			{
				throw new InvalidOperationException("ArchiveHeader should never been null in a streaming read.");
			}
			GetVolumeFileParts().First();
			base.Stream.Position = 0L;
		}
	}
}
