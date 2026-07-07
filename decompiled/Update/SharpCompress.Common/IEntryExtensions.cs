using System;
using System.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common;

internal static class IEntryExtensions
{
	internal static void PreserveExtractionOptions(this IEntry entry, string destinationFileName, ExtractionOptions options)
	{
		if (!options.PreserveFileTime && !options.PreserveAttributes)
		{
			return;
		}
		FileInfo fileInfo = new FileInfo(destinationFileName);
		if (!fileInfo.Exists)
		{
			return;
		}
		if (options.PreserveFileTime)
		{
			if (entry.CreatedTime.HasValue)
			{
				fileInfo.CreationTime = entry.CreatedTime.Value;
			}
			if (entry.LastModifiedTime.HasValue)
			{
				fileInfo.LastWriteTime = entry.LastModifiedTime.Value;
			}
			if (entry.LastAccessedTime.HasValue)
			{
				fileInfo.LastAccessTime = entry.LastAccessedTime.Value;
			}
		}
		if (options.PreserveAttributes && entry.Attrib.HasValue)
		{
			fileInfo.Attributes = (FileAttributes)Enum.ToObject(typeof(FileAttributes), entry.Attrib.Value);
		}
	}
}
