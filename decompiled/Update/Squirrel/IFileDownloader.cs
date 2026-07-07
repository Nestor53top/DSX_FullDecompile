using System;
using System.Threading.Tasks;

namespace Squirrel;

internal interface IFileDownloader
{
	Task DownloadFile(string url, string targetFile, Action<int> progress);

	Task<byte[]> DownloadUrl(string url);
}
