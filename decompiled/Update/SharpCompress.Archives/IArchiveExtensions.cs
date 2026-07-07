using System.Linq;
using SharpCompress.Readers;

namespace SharpCompress.Archives;

internal static class IArchiveExtensions
{
	public static void WriteToDirectory(this IArchive archive, string destinationDirectory, ExtractionOptions options = null)
	{
		foreach (IArchiveEntry item in archive.Entries.Where((IArchiveEntry x) => !x.IsDirectory))
		{
			item.WriteToDirectory(destinationDirectory, options);
		}
	}
}
