using System;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet;

internal class PackageDownloader : IHttpClientEvents, IProgressProvider
{
	private const string DefaultUserAgentClient = "NuGet Core";

	public string CurrentDownloadPackageId { get; private set; }

	public string CurrentDownloadPackageVersion { get; private set; }

	public event EventHandler<ProgressEventArgs> ProgressAvailable = delegate
	{
	};

	public event EventHandler<WebRequestEventArgs> SendingRequest = delegate
	{
	};

	public virtual void DownloadPackage(Uri uri, IPackageMetadata package, Stream targetStream)
	{
		if (uri == null)
		{
			throw new ArgumentNullException("uri");
		}
		HttpClient downloadClient = new HttpClient(uri)
		{
			UserAgent = HttpUtility.CreateUserAgentString("NuGet Core")
		};
		DownloadPackage(downloadClient, package, targetStream);
	}

	public void DownloadPackage(IHttpClient downloadClient, IPackageName package, Stream targetStream)
	{
		if (downloadClient == null)
		{
			throw new ArgumentNullException("downloadClient");
		}
		if (targetStream == null)
		{
			throw new ArgumentNullException("targetStream");
		}
		string operation = string.Format(CultureInfo.CurrentCulture, NuGetResources.DownloadProgressStatus, new object[2] { package.Id, package.Version });
		CurrentDownloadPackageId = package.Id;
		CurrentDownloadPackageVersion = package.Version.ToString();
		EventHandler<ProgressEventArgs> value = delegate(object sender, ProgressEventArgs e)
		{
			OnPackageDownloadProgress(new ProgressEventArgs(operation, e.PercentComplete));
		};
		try
		{
			downloadClient.ProgressAvailable += value;
			downloadClient.SendingRequest += OnSendingRequest;
			downloadClient.DownloadData(targetStream);
		}
		finally
		{
			downloadClient.ProgressAvailable -= value;
			downloadClient.SendingRequest -= OnSendingRequest;
			CurrentDownloadPackageId = null;
		}
	}

	private void OnPackageDownloadProgress(ProgressEventArgs e)
	{
		this.ProgressAvailable(this, e);
	}

	private void OnSendingRequest(object sender, WebRequestEventArgs webRequestArgs)
	{
		this.SendingRequest(this, webRequestArgs);
	}
}
