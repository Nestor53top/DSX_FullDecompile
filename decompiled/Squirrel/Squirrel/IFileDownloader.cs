using System;
using System.Threading.Tasks;

namespace Squirrel;

public interface IFileDownloader
{
	Task DownloadFile(string url, string targetFile, Action<int> progress);

	Task<byte[]> DownloadUrl(string url);
}
