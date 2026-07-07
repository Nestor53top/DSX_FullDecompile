using System;
using System.IO;
using System.Linq;
using SharpCompress.IO;

namespace SharpCompress.Common.SevenZip;

internal class SevenZipFilePart : FilePart
{
	private CompressionType? type;

	private readonly Stream stream;

	private readonly ArchiveDatabase database;

	private const uint k_Copy = 0u;

	private const uint k_Delta = 3u;

	private const uint k_LZMA2 = 33u;

	private const uint k_LZMA = 196865u;

	private const uint k_PPMD = 197633u;

	private const uint k_BCJ = 50528515u;

	private const uint k_BCJ2 = 50528539u;

	private const uint k_Deflate = 262408u;

	private const uint k_BZip2 = 262658u;

	internal Stream BaseStream { get; private set; }

	internal CFileItem Header { get; }

	internal CFolder Folder { get; }

	internal int Index { get; }

	internal override string FilePartName => Header.Name;

	public CompressionType CompressionType
	{
		get
		{
			if (!type.HasValue)
			{
				type = GetCompression();
			}
			return type.Value;
		}
	}

	internal SevenZipFilePart(Stream stream, ArchiveDatabase database, int index, CFileItem fileEntry)
	{
		this.stream = stream;
		this.database = database;
		Index = index;
		Header = fileEntry;
		if (Header.HasStream)
		{
			Folder = database.Folders[database.FileIndexToFolderIndexMap[index]];
		}
	}

	internal override Stream GetRawStream()
	{
		return null;
	}

	internal override Stream GetCompressedStream()
	{
		if (!Header.HasStream)
		{
			return null;
		}
		Stream folderStream = database.GetFolderStream(stream, Folder, null);
		int num = database.FolderStartFileIndex[database.Folders.IndexOf(Folder)];
		int num2 = Index - num;
		long num3 = 0L;
		for (int i = 0; i < num2; i++)
		{
			num3 += database.Files[num + i].Size;
		}
		if (num3 > 0)
		{
			folderStream.Skip(num3);
		}
		return new ReadOnlySubStream(folderStream, Header.Size);
	}

	internal CompressionType GetCompression()
	{
		switch (Folder.Coders.First().MethodId.Id)
		{
		case 33uL:
		case 196865uL:
			return CompressionType.LZMA;
		case 197633uL:
			return CompressionType.PPMd;
		case 262658uL:
			return CompressionType.BZip2;
		default:
			throw new NotImplementedException();
		}
	}
}
