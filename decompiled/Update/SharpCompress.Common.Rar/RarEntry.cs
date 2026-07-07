using System;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Common.Rar;

internal abstract class RarEntry : Entry
{
	internal abstract FileHeader FileHeader { get; }

	public override long Crc => FileHeader.FileCRC;

	public override string Key => FileHeader.FileName;

	public override DateTime? LastModifiedTime => FileHeader.FileLastModifiedTime;

	public override DateTime? CreatedTime => FileHeader.FileCreatedTime;

	public override DateTime? LastAccessedTime => FileHeader.FileLastAccessedTime;

	public override DateTime? ArchivedTime => FileHeader.FileArchivedTime;

	public override bool IsEncrypted => FileHeader.FileFlags.HasFlag(FileFlags.PASSWORD);

	public override bool IsDirectory => FileHeader.FileFlags.HasFlag(FileFlags.WINDOWMASK);

	public override bool IsSplit => FileHeader.FileFlags.HasFlag(FileFlags.SPLIT_AFTER);

	public override string ToString()
	{
		return string.Format("Entry Path: {0} Compressed Size: {1} Uncompressed Size: {2} CRC: {3}", new object[4] { Key, CompressedSize, Size, Crc });
	}
}
