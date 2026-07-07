using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet;

internal class UnzippedPackageRepository : PackageRepositoryBase, IPackageLookup, IPackageRepository
{
	protected IFileSystem FileSystem { get; private set; }

	internal IPackagePathResolver PathResolver { get; set; }

	public override string Source => FileSystem.Root;

	public override bool SupportsPrereleasePackages => true;

	public UnzippedPackageRepository(string physicalPath)
		: this(new DefaultPackagePathResolver(physicalPath), new PhysicalFileSystem(physicalPath))
	{
	}

	public UnzippedPackageRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
	{
		FileSystem = fileSystem;
		PathResolver = pathResolver;
	}

	public override IQueryable<IPackage> GetPackages()
	{
		return (from file in FileSystem.GetFiles("", "*" + Constants.PackageExtension)
			let packageName = Path.GetFileNameWithoutExtension(file)
			where FileSystem.DirectoryExists(packageName)
			select new UnzippedPackage(FileSystem, packageName)).AsQueryable();
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		string packageFileName = GetPackageFileName(packageId, version);
		if (Exists(packageId, version))
		{
			return new UnzippedPackage(FileSystem, packageFileName);
		}
		return null;
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		return from p in GetPackages()
			where p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)
			select p;
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		string packageFileName = GetPackageFileName(packageId, version);
		string path = packageFileName + Constants.PackageExtension;
		if (FileSystem.FileExists(path))
		{
			return FileSystem.DirectoryExists(packageFileName);
		}
		return false;
	}

	private static string GetPackageFileName(string packageId, SemanticVersion version)
	{
		return packageId + "." + version.ToString();
	}
}
