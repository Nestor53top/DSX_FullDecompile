using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal class DataServicePackageRepository : PackageRepositoryBase, IHttpClientEvents, IProgressProvider, IServiceBasedRepository, IPackageRepository, ICloneableRepository, ICultureAwareRepository, IOperationAwareRepository, IPackageLookup, ILatestPackageLookup, IWeakEventListener
{
	private const string FindPackagesByIdSvcMethod = "FindPackagesById";

	private const string PackageServiceEntitySetName = "Packages";

	private const string SearchSvcMethod = "Search";

	private const string GetUpdatesSvcMethod = "GetUpdates";

	private IDataServiceContext _context;

	private readonly IHttpClient _httpClient;

	private readonly PackageDownloader _packageDownloader;

	private CultureInfo _culture;

	private Tuple<string, string, string> _currentOperation;

	public CultureInfo Culture
	{
		get
		{
			if (_culture == null)
			{
				_culture = (_httpClient.Uri.IsLoopback ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture);
			}
			return _culture;
		}
	}

	public PackageDownloader PackageDownloader => _packageDownloader;

	public override string Source => _httpClient.Uri.OriginalString;

	public override bool SupportsPrereleasePackages => Context.SupportsProperty("IsAbsoluteLatestVersion");

	internal IDataServiceContext Context
	{
		private get
		{
			if (_context == null)
			{
				_context = new DataServiceContextWrapper(_httpClient.Uri);
				_context.SendingRequest += OnSendingRequest;
				_context.ReadingEntity += OnReadingEntity;
				_context.IgnoreMissingProperties = true;
			}
			return _context;
		}
		set
		{
			_context = value;
		}
	}

	private event EventHandler<WebRequestEventArgs> _sendingRequest;

	public event EventHandler<ProgressEventArgs> ProgressAvailable
	{
		add
		{
			_packageDownloader.ProgressAvailable += value;
		}
		remove
		{
			_packageDownloader.ProgressAvailable -= value;
		}
	}

	public event EventHandler<WebRequestEventArgs> SendingRequest
	{
		add
		{
			_packageDownloader.SendingRequest += value;
			_httpClient.SendingRequest += value;
			_sendingRequest += value;
		}
		remove
		{
			_packageDownloader.SendingRequest -= value;
			_httpClient.SendingRequest -= value;
			_sendingRequest -= value;
		}
	}

	public DataServicePackageRepository(Uri serviceRoot)
		: this(new HttpClient(serviceRoot))
	{
	}

	public DataServicePackageRepository(IHttpClient client)
		: this(client, new PackageDownloader())
	{
	}

	public DataServicePackageRepository(IHttpClient client, PackageDownloader packageDownloader)
	{
		if (client == null)
		{
			throw new ArgumentNullException("client");
		}
		if (packageDownloader == null)
		{
			throw new ArgumentNullException("packageDownloader");
		}
		_httpClient = client;
		_httpClient.AcceptCompression = true;
		_packageDownloader = packageDownloader;
		if (EnvironmentUtility.RunningFromCommandLine || EnvironmentUtility.IsMonoRuntime)
		{
			_packageDownloader.SendingRequest += OnPackageDownloaderSendingRequest;
		}
		else
		{
			SendingRequestEventManager.AddListener(_packageDownloader, (IWeakEventListener)(object)this);
		}
	}

	private void OnPackageDownloaderSendingRequest(object sender, WebRequestEventArgs e)
	{
		if (_currentOperation == null)
		{
			return;
		}
		string text = _currentOperation.Item1;
		string item = _currentOperation.Item2;
		string item2 = _currentOperation.Item3;
		if (!string.IsNullOrEmpty(item) && !string.IsNullOrEmpty(_packageDownloader.CurrentDownloadPackageId) && !item.Equals(_packageDownloader.CurrentDownloadPackageId, StringComparison.OrdinalIgnoreCase))
		{
			text += "-Dependency";
		}
		if (!string.IsNullOrEmpty(_packageDownloader.CurrentDownloadPackageId) && !string.IsNullOrEmpty(_packageDownloader.CurrentDownloadPackageVersion))
		{
			e.Request.Headers[RepositoryOperationNames.PackageId] = _packageDownloader.CurrentDownloadPackageId;
			e.Request.Headers[RepositoryOperationNames.PackageVersion] = _packageDownloader.CurrentDownloadPackageVersion;
		}
		e.Request.Headers[RepositoryOperationNames.OperationHeaderName] = text;
		if (!text.Equals(_currentOperation.Item1, StringComparison.OrdinalIgnoreCase))
		{
			e.Request.Headers[RepositoryOperationNames.DependentPackageHeaderName] = item;
			if (!string.IsNullOrEmpty(item2))
			{
				e.Request.Headers[RepositoryOperationNames.DependentPackageVersionHeaderName] = item2;
			}
		}
		RaiseSendingRequest(e);
	}

	private void OnReadingEntity(object sender, ReadingWritingEntityEventArgs e)
	{
		DataServicePackage obj = (DataServicePackage)e.Entity;
		string value = ((XContainer)e.Data).Element(e.Data.Name.Namespace.GetName("content")).Attribute(XName.Get("src")).Value;
		obj.DownloadUrl = new Uri(value);
		obj.Downloader = _packageDownloader;
	}

	private void OnSendingRequest(object sender, SendingRequest2EventArgs e)
	{
		ShimDataRequestMessage shimDataRequestMessage = new ShimDataRequestMessage(e);
		_httpClient.InitializeRequest(shimDataRequestMessage.WebRequest);
		RaiseSendingRequest(new WebRequestEventArgs(shimDataRequestMessage.WebRequest));
	}

	private void RaiseSendingRequest(WebRequestEventArgs e)
	{
		if (this._sendingRequest != null)
		{
			this._sendingRequest(this, e);
		}
	}

	public override IQueryable<IPackage> GetPackages()
	{
		return new SmartDataServiceQuery<DataServicePackage>(Context, "Packages");
	}

	public IQueryable<IPackage> Search(string searchTerm, IEnumerable<string> targetFrameworks, bool allowPrereleaseVersions, bool includeDelisted)
	{
		if (!Context.SupportsServiceMethod("Search"))
		{
			IEnumerable<IPackage> source = GetPackages().Find(searchTerm).FilterByPrerelease(allowPrereleaseVersions);
			if (!includeDelisted)
			{
				source = source.Where((IPackage p) => p.IsListed());
			}
			return source.AsQueryable();
		}
		string value = string.Join("|", targetFrameworks);
		Dictionary<string, object> dictionary = new Dictionary<string, object>
		{
			{
				"searchTerm",
				"'" + UrlEncodeOdataParameter(searchTerm) + "'"
			},
			{
				"targetFramework",
				"'" + UrlEncodeOdataParameter(value) + "'"
			}
		};
		if (SupportsPrereleasePackages)
		{
			dictionary.Add("includePrerelease", ToLowerCaseString(allowPrereleaseVersions));
		}
		if (includeDelisted)
		{
			dictionary.Add("includeDelisted", "true");
		}
		IDataServiceQuery<DataServicePackage> query = Context.CreateQuery<DataServicePackage>("Search", dictionary);
		return new SmartDataServiceQuery<DataServicePackage>(Context, query);
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		IQueryable<DataServicePackage> source = Context.CreateQuery<DataServicePackage>("Packages").AsQueryable();
		foreach (string comparableVersionString in version.GetComparableVersionStrings())
		{
			string versionString = comparableVersionString;
			try
			{
				if ((from p in source
					where p.Id == packageId && p.Version == versionString
					select p.Id).ToArray().Length == 1)
				{
					return true;
				}
			}
			catch (DataServiceQueryException)
			{
			}
		}
		return false;
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		IQueryable<DataServicePackage> source = Context.CreateQuery<DataServicePackage>("Packages").AsQueryable();
		foreach (string comparableVersionString in version.GetComparableVersionStrings())
		{
			string versionString = comparableVersionString;
			try
			{
				DataServicePackage[] array = source.Where((DataServicePackage p) => p.Id == packageId && p.Version == versionString).ToArray();
				if (array.Length != 0)
				{
					return array[0];
				}
			}
			catch (DataServiceQueryException)
			{
			}
		}
		return null;
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		try
		{
			if (!Context.SupportsServiceMethod("FindPackagesById"))
			{
				return PackageRepositoryExtensions.FindPackagesByIdCore(this, packageId);
			}
			Dictionary<string, object> queryOptions = new Dictionary<string, object> { 
			{
				"id",
				"'" + UrlEncodeOdataParameter(packageId) + "'"
			} };
			IDataServiceQuery<DataServicePackage> query = Context.CreateQuery<DataServicePackage>("FindPackagesById", queryOptions);
			return new SmartDataServiceQuery<DataServicePackage>(Context, query);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorLoadingPackages, new object[2] { _httpClient.OriginalUri, ex.Message }), ex);
		}
	}

	public IEnumerable<IPackage> GetUpdates(IEnumerable<IPackageName> packages, bool includePrerelease, bool includeAllVersions, IEnumerable<FrameworkName> targetFrameworks, IEnumerable<IVersionSpec> versionConstraints)
	{
		if (!Context.SupportsServiceMethod("GetUpdates"))
		{
			return this.GetUpdatesCore(packages, includePrerelease, includeAllVersions, targetFrameworks, versionConstraints);
		}
		string text = string.Join("|", packages.Select((IPackageName p) => p.Id));
		string text2 = string.Join("|", packages.Select((IPackageName p) => p.Version.ToString()));
		string value = (targetFrameworks.IsEmpty() ? "" : string.Join("|", targetFrameworks.Select(VersionUtility.GetShortFrameworkName)));
		string value2 = (versionConstraints.IsEmpty() ? "" : string.Join("|", versionConstraints.Select((IVersionSpec v) => (v != null) ? v.ToString() : "")));
		Dictionary<string, object> queryOptions = new Dictionary<string, object>
		{
			{
				"packageIds",
				"'" + text + "'"
			},
			{
				"versions",
				"'" + text2 + "'"
			},
			{
				"includePrerelease",
				ToLowerCaseString(includePrerelease)
			},
			{
				"includeAllVersions",
				ToLowerCaseString(includeAllVersions)
			},
			{
				"targetFrameworks",
				"'" + UrlEncodeOdataParameter(value) + "'"
			},
			{
				"versionConstraints",
				"'" + UrlEncodeOdataParameter(value2) + "'"
			}
		};
		IDataServiceQuery<DataServicePackage> query = Context.CreateQuery<DataServicePackage>("GetUpdates", queryOptions);
		return new SmartDataServiceQuery<DataServicePackage>(Context, query);
	}

	public IPackageRepository Clone()
	{
		return new DataServicePackageRepository(_httpClient, _packageDownloader);
	}

	public IDisposable StartOperation(string operation, string mainPackageId, string mainPackageVersion)
	{
		Tuple<string, string, string> oldOperation = _currentOperation;
		_currentOperation = Tuple.Create(operation, mainPackageId, mainPackageVersion);
		return new DisposableAction(delegate
		{
			_currentOperation = oldOperation;
		});
	}

	public bool TryFindLatestPackageById(string id, out SemanticVersion latestVersion)
	{
		latestVersion = null;
		try
		{
			Dictionary<string, object> queryOptions = new Dictionary<string, object> { 
			{
				"id",
				"'" + UrlEncodeOdataParameter(id) + "'"
			} };
			var anon = (from p in Context.CreateQuery<DataServicePackage>("FindPackagesById", queryOptions).AsQueryable()
				where p.IsLatestVersion
				select new { p.Id, p.Version }).FirstOrDefault();
			if (anon != null)
			{
				latestVersion = new SemanticVersion(anon.Version);
				return true;
			}
		}
		catch (DataServiceQueryException)
		{
		}
		return false;
	}

	public bool TryFindLatestPackageById(string id, bool includePrerelease, out IPackage package)
	{
		try
		{
			Dictionary<string, object> queryOptions = new Dictionary<string, object> { 
			{
				"id",
				"'" + UrlEncodeOdataParameter(id) + "'"
			} };
			IQueryable<DataServicePackage> source = Context.CreateQuery<DataServicePackage>("FindPackagesById", queryOptions).AsQueryable();
			if (includePrerelease)
			{
				package = (from p in source
					where p.IsAbsoluteLatestVersion
					orderby p.Version descending
					select p).FirstOrDefault();
			}
			else
			{
				package = (from p in source
					where p.IsLatestVersion
					orderby p.Version descending
					select p).FirstOrDefault();
			}
			return package != null;
		}
		catch (DataServiceQueryException)
		{
			package = null;
			return false;
		}
	}

	private static string UrlEncodeOdataParameter(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return Uri.EscapeDataString(value).Replace("'", "''").Replace("%27", "''");
		}
		return value;
	}

	private static string ToLowerCaseString(bool value)
	{
		return value.ToString().ToLowerInvariant();
	}

	public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(SendingRequestEventManager))
		{
			OnPackageDownloaderSendingRequest(sender, (WebRequestEventArgs)e);
			return true;
		}
		return false;
	}
}
