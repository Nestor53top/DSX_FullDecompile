using System;

namespace NuGet;

internal interface IHttpClientEvents : IProgressProvider
{
	event EventHandler<WebRequestEventArgs> SendingRequest;
}
