using System.Collections.Generic;
using System.Linq;

namespace NuGet.Resolver;

internal class VirtualRepository : IPackageRepository
{
	private HashSet<IPackage> _packages;

	public string Source => string.Empty;

	public PackageSaveModes PackageSaveMode { get; set; }

	public bool SupportsPrereleasePackages => true;

	public VirtualRepository(IPackageRepository repo)
	{
		_packages = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);
		if (repo != null)
		{
			_packages.AddRange(repo.GetPackages());
		}
	}

	public IQueryable<IPackage> GetPackages()
	{
		return _packages.AsQueryable();
	}

	public void AddPackage(IPackage package)
	{
		_packages.Add(package);
	}

	public void RemovePackage(IPackage package)
	{
		_packages.Remove(package);
	}
}
