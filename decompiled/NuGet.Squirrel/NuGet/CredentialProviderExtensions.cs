using System;
using System.Net;

namespace NuGet;

internal static class CredentialProviderExtensions
{
	private static readonly string[] _authenticationSchemes = new string[3] { "Basic", "NTLM", "Negotiate" };

	internal static ICredentials GetCredentials(this ICredentialProvider provider, WebRequest request, CredentialType credentialType, bool retrying = false)
	{
		return provider.GetCredentials(request.RequestUri, request.Proxy, credentialType, retrying);
	}

	internal static ICredentials AsCredentialCache(this ICredentials credentials, Uri uri)
	{
		if (credentials == null)
		{
			return null;
		}
		if (credentials == CredentialCache.DefaultCredentials || credentials == CredentialCache.DefaultNetworkCredentials)
		{
			return credentials;
		}
		if (!(credentials is NetworkCredential cred))
		{
			return credentials;
		}
		CredentialCache credentialCache = new CredentialCache();
		string[] authenticationSchemes = _authenticationSchemes;
		foreach (string authType in authenticationSchemes)
		{
			credentialCache.Add(uri, authType, cred);
		}
		return credentialCache;
	}
}
