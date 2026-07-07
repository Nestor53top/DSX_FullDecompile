using System.Linq;

namespace SharpCompress.Archives.Rar;

public static class RarArchiveExtensions
{
	public static bool IsFirstVolume(this RarArchive archive)
	{
		return archive.Volumes.First().IsFirstVolume;
	}

	public static bool IsMultipartVolume(this RarArchive archive)
	{
		return archive.Volumes.First().IsMultiVolume;
	}
}
