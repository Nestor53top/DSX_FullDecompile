using System;
using System.Net;
using System.Threading.Tasks;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal class FileDownloader : IFileDownloader, IEnableLogger
{
	private readonly WebClient _providedClient;

	public FileDownloader(WebClient providedClient = null)
	{
		_providedClient = providedClient;
	}

	public async Task DownloadFile(string url, string targetFile, Action<int> progress)
	{
		WebClient wc = _providedClient ?? Utility.CreateWebClient();
		try
		{
			string failedUrl = null;
			DateTime lastSignalled = DateTime.MinValue;
			wc.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs args)
			{
				DateTime now = DateTime.Now;
				if (now - lastSignalled > TimeSpan.FromMilliseconds(500.0))
				{
					lastSignalled = now;
					progress(args.ProgressPercentage);
				}
			};
			while (true)
			{
				try
				{
					this.Log().Info("Downloading file: " + (failedUrl ?? url));
					await this.WarnIfThrows(async delegate
					{
						await wc.DownloadFileTaskAsync(failedUrl ?? url, targetFile);
						progress(100);
					}, "Failed downloading URL: " + (failedUrl ?? url));
					break;
				}
				catch (Exception)
				{
					if (failedUrl != null)
					{
						throw;
					}
					failedUrl = url.ToLower();
					progress(0);
				}
			}
		}
		finally
		{
			if (wc != null)
			{
				((IDisposable)wc).Dispose();
			}
		}
	}

	public async Task<byte[]> DownloadUrl(string url)
	{
		WebClient wc = _providedClient ?? Utility.CreateWebClient();
		try
		{
			string failedUrl = null;
			while (true)
			{
				try
				{
					this.Log().Info("Downloading url: " + (failedUrl ?? url));
					return await this.WarnIfThrows(() => wc.DownloadDataTaskAsync(failedUrl ?? url), "Failed to download url: " + (failedUrl ?? url));
				}
				catch (Exception)
				{
					if (failedUrl != null)
					{
						throw;
					}
					failedUrl = url.ToLower();
				}
			}
		}
		finally
		{
			if (wc != null)
			{
				((IDisposable)wc).Dispose();
			}
		}
	}
}
