using System;
using System.Net;

namespace NuGet;

internal interface IProxyCache
{
	void Add(IWebProxy proxy);

	IWebProxy GetProxy(Uri uri);
}
