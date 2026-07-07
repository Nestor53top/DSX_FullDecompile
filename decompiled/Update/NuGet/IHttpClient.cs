using System;
using System.IO;
using System.Net;

namespace NuGet;

internal interface IHttpClient : IHttpClientEvents, IProgressProvider
{
	string UserAgent { get; set; }

	Uri Uri { get; }

	Uri OriginalUri { get; }

	bool AcceptCompression { get; set; }

	WebResponse GetResponse();

	void InitializeRequest(WebRequest request);

	void DownloadData(Stream targetStream);
}
