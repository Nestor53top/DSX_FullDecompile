using System;
using System.Net;

namespace NuGet;

internal class NullCredentialProvider : ICredentialProvider
{
	private static readonly NullCredentialProvider _instance = new NullCredentialProvider();

	public static ICredentialProvider Instance => _instance;

	private NullCredentialProvider()
	{
	}

	public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
	{
		return null;
	}
}
