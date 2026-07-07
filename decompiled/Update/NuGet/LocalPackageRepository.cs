using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class LocalPackageRepository : PackageRepositoryBase, IPackageLookup, IPackageRepository
{
	private class PackageCacheEntry
	{
		public IPackage Package { get; private set; }

		public DateTimeOffset LastModifiedTime { get; private set; }

		public PackageCacheEntry(IPackage package, DateTimeOffset lastModifiedTime)
		{
			Package = package;
			LastModifiedTime = lastModifiedTime;
		}
	}

	private readonly ConcurrentDictionary<string, PackageCacheEntry> _packageCache = new ConcurrentDictionary<string, PackageCacheEntry>(StringComparer.OrdinalIgnoreCase);

	private readonly ConcurrentDictionary<PackageName, string> _packagePathLookup = new ConcurrentDictionary<PackageName, string>();

	private readonly bool _enableCaching;

	public override string Source => FileSystem.Root;

	public IPackagePathResolver PathResolver { get; set; }

	public override bool SupportsPrereleasePackages => true;

	protected IFileSystem FileSystem { get; private set; }

	public LocalPackageRepository(string physicalPath)
		: this(physicalPath, enableCaching: true)
	{
	}

	public LocalPackageRepository(string physicalPath, bool enableCaching)
		: this(new DefaultPackagePathResolver(physicalPath), new PhysicalFileSystem(physicalPath), enableCaching)
	{
	}

	public LocalPackageRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		: this(pathResolver, fileSystem, enableCaching: true)
	{
	}

	public LocalPackageRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem, bool enableCaching)
	{
		if (pathResolver == null)
		{
			throw new ArgumentNullException("pathResolver");
		}
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		FileSystem = fileSystem;
		PathResolver = pathResolver;
		_enableCaching = enableCaching;
	}

	public override IQueryable<IPackage> GetPackages()
	{
		return GetPackages(OpenPackage).AsQueryable();
	}

	public override void AddPackage(IPackage package)
	{
		if (base.PackageSaveMode.HasFlag(PackageSaveModes.Nuspec))
		{
			string manifestFilePath = GetManifestFilePath(package.Id, package.Version);
			Manifest manifest = Manifest.Create(package);
			manifest.Metadata.ReferenceSets = (from f in package.AssemblyReferences
				group f by f.TargetFramework into g
				select new ManifestReferenceSet
				{
					TargetFramework = ((g.Key == null) ? null : VersionUtility.GetFrameworkString(g.Key)),
					References = g.Select((IPackageAssemblyReference p) => new ManifestReference
					{
						File = p.Name
					}).ToList()
				}).ToList();
			FileSystem.AddFileWithCheck(manifestFilePath, manifest.Save);
		}
		if (base.PackageSaveMode.HasFlag(PackageSaveModes.Nupkg))
		{
			string packageFilePath = GetPackageFilePath(package);
			FileSystem.AddFileWithCheck(packageFilePath, package.GetStream);
		}
	}

	public override void RemovePackage(IPackage package)
	{
		string manifestFilePath = GetManifestFilePath(package.Id, package.Version);
		if (FileSystem.FileExists(manifestFilePath))
		{
			FileSystem.DeleteFileSafe(manifestFilePath);
		}
		string packageFilePath = GetPackageFilePath(package);
		FileSystem.DeleteFileSafe(packageFilePath);
		FileSystem.DeleteDirectorySafe(PathResolver.GetPackageDirectory(package), recursive: false);
		if (!FileSystem.GetFilesSafe(string.Empty).Any() && !FileSystem.GetDirectoriesSafe(string.Empty).Any())
		{
			FileSystem.DeleteDirectorySafe(string.Empty, recursive: false);
		}
	}

	public virtual IPackage FindPackage(string packageId, SemanticVersion version)
	{
		if (string.IsNullOrEmpty(packageId))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageId");
		}
		if (version == null)
		{
			throw new ArgumentNullException("version");
		}
		return FindPackage(OpenPackage, packageId, version);
	}

	public virtual IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		if (string.IsNullOrEmpty(packageId))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageId");
		}
		return FindPackagesById(OpenPackage, packageId);
	}

	public virtual bool Exists(string packageId, SemanticVersion version)
	{
		return FindPackage(packageId, version) != null;
	}

	public virtual IEnumerable<string> GetPackageLookupPaths(string packageId, SemanticVersion version)
	{
		string packageFileName = PathResolver.GetPackageFileName(packageId, version);
		string filter = Path.ChangeExtension(packageFileName, Constants.ManifestExtension);
		IEnumerable<string> enumerable = GetPackageFiles(packageFileName).Concat(GetPackageFiles(filter));
		if (version != null && version.Version.Revision < 1)
		{
			string text = ((version.Version.Build < 1) ? string.Join(".", new object[3]
			{
				packageId,
				version.Version.Major,
				version.Version.Minor
			}) : string.Join(".", new object[4]
			{
				packageId,
				version.Version.Major,
				version.Version.Minor,
				version.Version.Build
			}));
			string filter2 = text + "*" + Constants.ManifestExtension;
			text = text + "*" + Constants.PackageExtension;
			IEnumerable<string> second = from path in GetPackageFiles(text)
				where FileNameMatchesPattern(packageId, version, path)
				select path;
			IEnumerable<string> second2 = from path in GetPackageFiles(filter2)
				where FileNameMatchesPattern(packageId, version, path)
				select path;
			return enumerable.Concat(second).Concat(second2);
		}
		return enumerable;
	}

	internal IPackage FindPackage(Func<string, IPackage> openPackage, string packageId, SemanticVersion version)
	{
		PackageName lookupPackageName = new PackageName(packageId, version);
		if (_enableCaching && _packagePathLookup.TryGetValue(lookupPackageName, out var value) && FileSystem.FileExists(value))
		{
			return GetPackage(openPackage, value);
		}
		return (from path in GetPackageLookupPaths(packageId, version)
			let package = GetPackage(openPackage, path)
			where lookupPackageName.Equals(new PackageName(package.Id, package.Version))
			select package).FirstOrDefault();
	}

	internal IEnumerable<IPackage> FindPackagesById(Func<string, IPackage> openPackage, string packageId)
	{
		HashSet<IPackage> hashSet = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);
		hashSet.AddRange(GetPackages(openPackage, packageId, GetPackageFiles(packageId + "*" + Constants.PackageExtension)));
		hashSet.AddRange(GetPackages(openPackage, packageId, GetPackageFiles(packageId + "*" + Constants.ManifestExtension)));
		return hashSet;
	}

	internal IEnumerable<IPackage> GetPackages(Func<string, IPackage> openPackage, string packageId, IEnumerable<string> packagePaths)
	{
		foreach (string packagePath in packagePaths)
		{
			IPackage package = null;
			try
			{
				package = GetPackage(openPackage, packagePath);
			}
			catch (InvalidOperationException)
			{
				if (!string.Equals(Constants.ManifestExtension, Path.GetExtension(packagePath), StringComparison.OrdinalIgnoreCase))
				{
					throw;
				}
			}
			if (package != null && package.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase))
			{
				yield return package;
			}
		}
	}

	internal IEnumerable<IPackage> GetPackages(Func<string, IPackage> openPackage)
	{
		return from path in GetPackageFiles()
			select GetPackage(openPackage, path);
	}

	private IPackage GetPackage(Func<string, IPackage> openPackage, string path)
	{
		DateTimeOffset lastModified = FileSystem.GetLastModified(path);
		if (!_packageCache.TryGetValue(path, out var value) || (value != null && lastModified > value.LastModifiedTime))
		{
			IPackage package = openPackage(path);
			value = new PackageCacheEntry(package, lastModified);
			if (_enableCaching)
			{
				_packageCache[path] = value;
				_packagePathLookup.GetOrAdd(new PackageName(package.Id, package.Version), path);
			}
		}
		return value.Package;
	}

	internal IEnumerable<string> GetPackageFiles(string filter = null)
	{
		filter = filter ?? ("*" + Constants.PackageExtension);
		foreach (string directory in FileSystem.GetDirectories(string.Empty))
		{
			foreach (string file in FileSystem.GetFiles(directory, filter))
			{
				yield return file;
			}
		}
		foreach (string file2 in FileSystem.GetFiles(string.Empty, filter))
		{
			yield return file2;
		}
	}

	protected virtual IPackage OpenPackage(string path)
	{
		//IL_0032: Expected O, but got Unknown
		if (!FileSystem.FileExists(path))
		{
			return null;
		}
		if (Path.GetExtension(path) == Constants.PackageExtension)
		{
			OptimizedZipPackage optimizedZipPackage;
			try
			{
				optimizedZipPackage = new OptimizedZipPackage(FileSystem, path);
			}
			catch (FileFormatException ex)
			{
				FileFormatException innerException = ex;
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorReadingPackage, new object[1] { path }), (Exception?)(object)innerException);
			}
			optimizedZipPackage.Published = FileSystem.GetLastModified(path);
			return optimizedZipPackage;
		}
		if (Path.GetExtension(path) == Constants.ManifestExtension && FileSystem.FileExists(path))
		{
			return new UnzippedPackage(FileSystem, Path.GetFileNameWithoutExtension(path));
		}
		return null;
	}

	protected virtual string GetPackageFilePath(IPackage package)
	{
		return Path.Combine(PathResolver.GetPackageDirectory(package), PathResolver.GetPackageFileName(package));
	}

	protected virtual string GetPackageFilePath(string id, SemanticVersion version)
	{
		return Path.Combine(PathResolver.GetPackageDirectory(id, version), PathResolver.GetPackageFileName(id, version));
	}

	private static bool FileNameMatchesPattern(string packageId, SemanticVersion version, string path)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		if (fileNameWithoutExtension.Length > packageId.Length && SemanticVersion.TryParse(fileNameWithoutExtension.Substring(packageId.Length + 1), out var value))
		{
			return value == version;
		}
		return false;
	}

	private string GetManifestFilePath(string packageId, SemanticVersion version)
	{
		string packageDirectory = PathResolver.GetPackageDirectory(packageId, version);
		string path = packageDirectory + Constants.ManifestExtension;
		return Path.Combine(packageDirectory, path);
	}
}
