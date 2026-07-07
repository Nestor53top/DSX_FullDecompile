using System;
using System.Net;

namespace NuGet;

internal interface ICredentialCache
{
	void Add(Uri uri, ICredentials credentials);

	ICredentials GetCredentials(Uri uri);
}
