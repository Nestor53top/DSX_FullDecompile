using System;
using System.Net;

namespace NuGet;

public interface ICredentialProvider
{
	ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying);
}
