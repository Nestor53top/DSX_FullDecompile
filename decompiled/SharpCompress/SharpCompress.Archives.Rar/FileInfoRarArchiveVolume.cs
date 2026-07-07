using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.IO;
using SharpCompress.Readers;

namespace SharpCompress.Archives.Rar;

internal class FileInfoRarArchiveVolume : RarVolume
{
	internal ReadOnlyCollection<RarFilePart> FileParts { get; }

	internal FileInfo FileInfo { get; }

	internal FileInfoRarArchiveVolume(FileInfo fileInfo, ReaderOptions options)
		: base(StreamingMode.Seekable, fileInfo.OpenRead(), FixOptions(options))
	{
		FileInfo = fileInfo;
		FileParts = GetVolumeFileParts().ToReadOnly();
	}

	private static ReaderOptions FixOptions(ReaderOptions options)
	{
		options.LeaveStreamOpen = false;
		return options;
	}

	internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
	{
		return new FileInfoRarFilePart(this, base.ReaderOptions.Password, markHeader, fileHeader, FileInfo);
	}

	internal override IEnumerable<RarFilePart> ReadFileParts()
	{
		return FileParts;
	}
}
