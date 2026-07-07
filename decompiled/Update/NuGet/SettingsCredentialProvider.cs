using System;
using System.Linq;
using System.Net;
using NuGet.Resources;

namespace NuGet;

internal class SettingsCredentialProvider : ICredentialProvider
{
	private readonly ICredentialProvider _credentialProvider;

	private readonly IPackageSourceProvider _packageSourceProvider;

	private readonly ILogger _logger;

	public SettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider)
		: this(credentialProvider, packageSourceProvider, NullLogger.Instance)
	{
	}

	public SettingsCredentialProvider(ICredentialProvider credentialProvider, IPackageSourceProvider packageSourceProvider, ILogger logger)
	{
		if (credentialProvider == null)
		{
			throw new ArgumentNullException("credentialProvider");
		}
		if (packageSourceProvider == null)
		{
			throw new ArgumentNullException("packageSourceProvider");
		}
		_credentialProvider = credentialProvider;
		_packageSourceProvider = packageSourceProvider;
		_logger = logger;
	}

	public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
	{
		if (!retrying && credentialType == CredentialType.RequestCredentials && TryGetCredentials(uri, out var configurationCredentials))
		{
			_logger.Log(MessageLevel.Info, NuGetResources.SettingsCredentials_UsingSavedCredentials, configurationCredentials.UserName);
			return configurationCredentials;
		}
		return _credentialProvider.GetCredentials(uri, proxy, credentialType, retrying);
	}

	private bool TryGetCredentials(Uri uri, out NetworkCredential configurationCredentials)
	{
		PackageSource packageSource = _packageSourceProvider.LoadPackageSources().FirstOrDefault((PackageSource p) => !string.IsNullOrEmpty(p.UserName) && !string.IsNullOrEmpty(p.Password) && Uri.TryCreate(p.Source, UriKind.Absolute, out Uri result) && UriUtility.UriStartsWith(result, uri));
		if (packageSource == null)
		{
			configurationCredentials = null;
			return false;
		}
		configurationCredentials = new NetworkCredential(packageSource.UserName, packageSource.Password);
		return true;
	}
}
