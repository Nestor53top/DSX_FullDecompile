using System;

namespace NuGet;

internal class PackageRepositoryFactory : IPackageRepositoryFactory
{
	private static readonly PackageRepositoryFactory _default = new PackageRepositoryFactory();

	private static readonly Func<Uri, IHttpClient> _defaultHttpClientFactory = (Uri u) => new RedirectedHttpClient(u);

	private Func<Uri, IHttpClient> _httpClientFactory;

	public static PackageRepositoryFactory Default => _default;

	public Func<Uri, IHttpClient> HttpClientFactory
	{
		get
		{
			return _httpClientFactory ?? _defaultHttpClientFactory;
		}
		set
		{
			_httpClientFactory = value;
		}
	}

	public virtual IPackageRepository CreateRepository(string packageSource)
	{
		if (packageSource == null)
		{
			throw new ArgumentNullException("packageSource");
		}
		Uri uri = new Uri(packageSource);
		if (uri.IsFile)
		{
			return new LocalPackageRepository(uri.LocalPath);
		}
		return new DataServicePackageRepository(HttpClientFactory(uri));
	}
}
