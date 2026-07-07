using System;
using System.Net;

namespace NuGet;

internal interface ICredentialProvider
{
	ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying);
}
