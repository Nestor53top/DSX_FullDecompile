using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal class DependencyResolverFromRepo : IDependencyResolver2
{
	private IPackageRepository _repo;

	public DependencyResolverFromRepo(IPackageRepository repo)
	{
		_repo = repo;
	}

	public IPackage ResolveDependency(PackageDependency dependency, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool preferListedPackages, DependencyVersion dependencyVersion)
	{
		if (_repo is IDependencyResolver dependencyResolver)
		{
			return dependencyResolver.ResolveDependency(dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion);
		}
		return DependencyResolveUtility.ResolveDependencyCore(_repo, dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion);
	}

	public IEnumerable<IPackage> FindPackages(IEnumerable<string> packageIds)
	{
		return packageIds.SelectMany((string id) => _repo.FindPackagesById(id));
	}
}
