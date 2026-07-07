using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet;

internal class PackageReferenceRepository : IPackageReferenceRepository, IPackageRepository, IPackageLookup, IPackageConstraintProvider, ILatestPackageLookup, IPackageReferenceRepository2
{
	private readonly PackageReferenceFile _packageReferenceFile;

	public string Source => Constants.PackageReferenceFile;

	public PackageSaveModes PackageSaveMode
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool SupportsPrereleasePackages => true;

	private ISharedPackageRepository SourceRepository { get; set; }

	public PackageReferenceFile ReferenceFile => _packageReferenceFile;

	public PackageReferenceRepository(IFileSystem fileSystem, string projectName, ISharedPackageRepository sourceRepository)
	{
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (sourceRepository == null)
		{
			throw new ArgumentNullException("sourceRepository");
		}
		_packageReferenceFile = new PackageReferenceFile(fileSystem, Constants.PackageReferenceFile, projectName);
		SourceRepository = sourceRepository;
	}

	public PackageReferenceRepository(string configFilePath, ISharedPackageRepository sourceRepository)
	{
		if (string.IsNullOrEmpty(configFilePath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "configFilePath");
		}
		if (sourceRepository == null)
		{
			throw new ArgumentNullException("sourceRepository");
		}
		_packageReferenceFile = new PackageReferenceFile(configFilePath);
		SourceRepository = sourceRepository;
	}

	public IQueryable<IPackage> GetPackages()
	{
		return GetPackagesCore().AsQueryable();
	}

	private IEnumerable<IPackage> GetPackagesCore()
	{
		return from p in _packageReferenceFile.GetPackageReferences().Select(GetPackage)
			where p != null
			select p;
	}

	public void AddPackage(IPackage package)
	{
		AddPackage(package.Id, package.Version, package.DevelopmentDependency, null);
	}

	public void RemovePackage(IPackage package)
	{
		if (_packageReferenceFile.DeleteEntry(package.Id, package.Version))
		{
			SourceRepository.UnregisterRepository(_packageReferenceFile);
		}
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		if (!_packageReferenceFile.EntryExists(packageId, version))
		{
			return null;
		}
		return SourceRepository.FindPackage(packageId, version);
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		return from p in GetPackageReferences(packageId).Select(GetPackage)
			where p != null
			select p;
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		return _packageReferenceFile.EntryExists(packageId, version);
	}

	public void RegisterIfNecessary()
	{
		if (GetPackages().Any())
		{
			SourceRepository.RegisterRepository(_packageReferenceFile);
		}
	}

	public IVersionSpec GetConstraint(string packageId)
	{
		return GetPackageReference(packageId)?.VersionConstraint;
	}

	public bool TryFindLatestPackageById(string id, out SemanticVersion latestVersion)
	{
		PackageReference packageReference = (from r in GetPackageReferences(id)
			orderby r.Version descending
			select r).FirstOrDefault();
		if (packageReference == null)
		{
			latestVersion = null;
			return false;
		}
		latestVersion = packageReference.Version;
		return true;
	}

	public bool TryFindLatestPackageById(string id, bool includePrerelease, out IPackage package)
	{
		IEnumerable<PackageReference> source = GetPackageReferences(id);
		if (!includePrerelease)
		{
			source = source.Where((PackageReference r) => string.IsNullOrEmpty(r.Version.SpecialVersion));
		}
		PackageReference packageReference = source.OrderByDescending((PackageReference r) => r.Version).FirstOrDefault();
		if (packageReference != null)
		{
			package = GetPackage(packageReference);
			return true;
		}
		package = null;
		return false;
	}

	public void AddPackage(string packageId, SemanticVersion version, bool developmentDependency, FrameworkName targetFramework)
	{
		_packageReferenceFile.AddEntry(packageId, version, developmentDependency, targetFramework);
		SourceRepository.RegisterRepository(_packageReferenceFile);
	}

	public FrameworkName GetPackageTargetFramework(string packageId)
	{
		return GetPackageReference(packageId)?.TargetFramework;
	}

	public PackageReference GetPackageReference(string packageId)
	{
		return GetPackageReferences(packageId).FirstOrDefault();
	}

	public IEnumerable<PackageReference> GetPackageReferences()
	{
		return _packageReferenceFile.GetPackageReferences();
	}

	public IEnumerable<PackageReference> GetPackageReferences(string packageId)
	{
		return from reference in _packageReferenceFile.GetPackageReferences()
			where IsValidReference(reference) && reference.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)
			select reference;
	}

	private IPackage GetPackage(PackageReference reference)
	{
		if (IsValidReference(reference))
		{
			return SourceRepository.FindPackage(reference.Id, reference.Version);
		}
		return null;
	}

	private static bool IsValidReference(PackageReference reference)
	{
		if (!string.IsNullOrEmpty(reference.Id))
		{
			return reference.Version != null;
		}
		return false;
	}
}
