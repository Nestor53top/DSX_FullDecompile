using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

[EntityPropertyMapping(/*Could not decode attribute arguments.*/)]
[CLSCompliant(false)]
[EntityPropertyMapping(/*Could not decode attribute arguments.*/)]
[EntityPropertyMapping(/*Could not decode attribute arguments.*/)]
[EntityPropertyMapping(/*Could not decode attribute arguments.*/)]
[DataServiceKey(new string[] { "Id", "Version" })]
internal class DataServicePackage : IPackage, IPackageMetadata, IPackageName, IServerPackageMetadata
{
	private IHashProvider _hashProvider;

	private bool _usingMachineCache;

	private string _licenseNames;

	internal IPackage _package;

	public string Id { get; set; }

	public string Version { get; set; }

	public string Title { get; set; }

	public string Authors { get; set; }

	public string Owners { get; set; }

	public Uri IconUrl { get; set; }

	public Uri LicenseUrl { get; set; }

	public Uri ProjectUrl { get; set; }

	public Uri ReportAbuseUrl { get; set; }

	public Uri GalleryDetailsUrl { get; set; }

	public string LicenseNames
	{
		get
		{
			return _licenseNames;
		}
		set
		{
			_licenseNames = value;
			LicenseNameCollection = (string.IsNullOrEmpty(value) ? ((ICollection<string>)new string[0]) : ((ICollection<string>)value.Split(new char[1] { ';' }).ToArray()));
		}
	}

	public ICollection<string> LicenseNameCollection { get; private set; }

	public Uri LicenseReportUrl { get; set; }

	public Uri DownloadUrl { get; set; }

	public bool Listed { get; set; }

	public DateTimeOffset? Published { get; set; }

	public DateTimeOffset LastUpdated { get; set; }

	public int DownloadCount { get; set; }

	public bool RequireLicenseAcceptance { get; set; }

	public bool DevelopmentDependency { get; set; }

	public string Description { get; set; }

	public string Summary { get; set; }

	public string ReleaseNotes { get; set; }

	public string Language { get; set; }

	public string Tags { get; set; }

	public string Dependencies { get; set; }

	public string PackageHash { get; set; }

	public string PackageHashAlgorithm { get; set; }

	public bool IsLatestVersion { get; set; }

	public bool IsAbsoluteLatestVersion { get; set; }

	public string Copyright { get; set; }

	public string MinClientVersion { get; set; }

	private string OldHash { get; set; }

	private IPackage Package
	{
		get
		{
			EnsurePackage(MachineCache.Default);
			return _package;
		}
	}

	internal PackageDownloader Downloader { get; set; }

	internal IHashProvider HashProvider
	{
		get
		{
			return _hashProvider ?? new CryptoHashProvider(PackageHashAlgorithm);
		}
		set
		{
			_hashProvider = value;
		}
	}

	bool IPackage.Listed => Listed;

	IEnumerable<string> IPackageMetadata.Authors
	{
		get
		{
			if (string.IsNullOrEmpty(Authors))
			{
				return Enumerable.Empty<string>();
			}
			return Authors.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	IEnumerable<string> IPackageMetadata.Owners
	{
		get
		{
			if (string.IsNullOrEmpty(Owners))
			{
				return Enumerable.Empty<string>();
			}
			return Owners.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	public IEnumerable<PackageDependencySet> DependencySets
	{
		get
		{
			if (string.IsNullOrEmpty(Dependencies))
			{
				return Enumerable.Empty<PackageDependencySet>();
			}
			return ParseDependencySet(Dependencies);
		}
	}

	public ICollection<PackageReferenceSet> PackageAssemblyReferences => Package.PackageAssemblyReferences;

	SemanticVersion IPackageName.Version
	{
		get
		{
			if (Version != null)
			{
				return new SemanticVersion(Version);
			}
			return null;
		}
	}

	Version IPackageMetadata.MinClientVersion
	{
		get
		{
			if (!string.IsNullOrEmpty(MinClientVersion))
			{
				return new Version(MinClientVersion);
			}
			return null;
		}
	}

	public IEnumerable<IPackageAssemblyReference> AssemblyReferences => Package.AssemblyReferences;

	public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies => Package.FrameworkAssemblies;

	public virtual IEnumerable<FrameworkName> GetSupportedFrameworks()
	{
		return Package.GetSupportedFrameworks();
	}

	public IEnumerable<IPackageFile> GetFiles()
	{
		return Package.GetFiles();
	}

	public Stream GetStream()
	{
		return Package.GetStream();
	}

	public override string ToString()
	{
		return this.GetFullName();
	}

	internal void EnsurePackage(IPackageCacheRepository cacheRepository)
	{
		if (_package != null && (!(_package is OptimizedZipPackage) || ((OptimizedZipPackage)_package).IsValid) && string.Equals(OldHash, PackageHash, StringComparison.OrdinalIgnoreCase) && (!_usingMachineCache || cacheRepository.Exists(Id, ((IPackageName)this).Version)))
		{
			return;
		}
		IPackage package = null;
		bool flag = false;
		bool flag2 = false;
		if (TryGetPackage(cacheRepository, this, out package) && MatchPackageHash(package))
		{
			flag2 = true;
		}
		else
		{
			if (cacheRepository.InvokeOnPackage(((IPackageName)this).Id, ((IPackageName)this).Version, delegate(Stream stream)
			{
				Downloader.DownloadPackage(DownloadUrl, this, stream);
			}))
			{
				package = cacheRepository.FindPackage(((IPackageName)this).Id, ((IPackageName)this).Version);
			}
			else
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					Downloader.DownloadPackage(DownloadUrl, this, memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					package = new ZipPackage(memoryStream);
				}
				flag = true;
			}
			flag2 = true;
		}
		if (flag2)
		{
			_package = package;
			Id = _package.Id;
			Version = _package.Version.ToString();
			_usingMachineCache = !flag;
			OldHash = PackageHash;
			return;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Error_InvalidPackage, new object[2] { Version, Id }));
	}

	private bool MatchPackageHash(IPackage package)
	{
		return package?.GetHash(HashProvider).Equals(PackageHash, StringComparison.OrdinalIgnoreCase) ?? false;
	}

	private static List<PackageDependencySet> ParseDependencySet(string value)
	{
		List<PackageDependencySet> list = new List<PackageDependencySet>();
		IEnumerable<IGrouping<FrameworkName, Tuple<string, IVersionSpec, FrameworkName>>> source = from d in value.Split(new char[1] { '|' }).Select(ParseDependency).ToList()
			group d by d.Item3;
		list.AddRange(source.Select((IGrouping<FrameworkName, Tuple<string, IVersionSpec, FrameworkName>> g) => new PackageDependencySet(g.Key, from pair in g
			where !string.IsNullOrEmpty(pair.Item1)
			select new PackageDependency(pair.Item1, pair.Item2))));
		return list;
	}

	private static Tuple<string, IVersionSpec, FrameworkName> ParseDependency(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		string[] array = value.Trim().Split(new char[1] { ':' });
		if (array.Length == 0)
		{
			return null;
		}
		string item = array[0].Trim();
		IVersionSpec result = null;
		if (array.Length > 1)
		{
			VersionUtility.TryParseVersionSpec(array[1], out result);
		}
		return Tuple.Create(item3: (array.Length > 2 && !string.IsNullOrEmpty(array[2])) ? VersionUtility.ParseFrameworkName(array[2]) : null, item1: item, item2: result);
	}

	private static bool TryGetPackage(IPackageRepository repository, IPackageMetadata packageMetadata, out IPackage package)
	{
		try
		{
			package = repository.FindPackage(packageMetadata.Id, packageMetadata.Version);
		}
		catch
		{
			package = null;
		}
		return package != null;
	}
}
