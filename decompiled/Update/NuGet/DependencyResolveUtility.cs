using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal static class DependencyResolveUtility
{
	public static IPackage ResolveDependency(object repository, PackageDependency dependency, bool allowPrereleaseVersions, bool preferListedPackages)
	{
		return ResolveDependency(repository, dependency, null, allowPrereleaseVersions, preferListedPackages, DependencyVersion.Lowest);
	}

	public static IPackage ResolveDependency(object repository, PackageDependency dependency, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool preferListedPackages, DependencyVersion dependencyVersion)
	{
		if (repository is IDependencyResolver dependencyResolver)
		{
			return dependencyResolver.ResolveDependency(dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion);
		}
		return ResolveDependencyCore((IPackageRepository)repository, dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion);
	}

	public static IPackage ResolveDependencyCore(IPackageRepository repository, PackageDependency dependency, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool preferListedPackages, DependencyVersion dependencyVersion)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		if (dependency == null)
		{
			throw new ArgumentNullException("dependency");
		}
		IEnumerable<IPackage> packages = repository.FindPackagesById(dependency.Id).ToList();
		packages = FilterPackagesByConstraints(constraintProvider, packages, dependency.Id, allowPrereleaseVersions);
		IList<IPackage> list = packages.ToList();
		if (preferListedPackages)
		{
			IPackage package = ResolveDependencyCore(list.Where(PackageExtensions.IsListed), dependency, dependencyVersion);
			if (package != null)
			{
				return package;
			}
		}
		return ResolveDependencyCore(list, dependency, dependencyVersion);
	}

	internal static IEnumerable<IPackage> FilterPackagesByConstraints(IPackageConstraintProvider constraintProvider, IEnumerable<IPackage> packages, string packageId, bool allowPrereleaseVersions)
	{
		constraintProvider = constraintProvider ?? NullConstraintProvider.Instance;
		IVersionSpec constraint = constraintProvider.GetConstraint(packageId);
		if (constraint != null)
		{
			packages = packages.FindByVersion(constraint);
		}
		if (!allowPrereleaseVersions)
		{
			packages = packages.Where((IPackage p) => p.IsReleaseVersion());
		}
		return packages;
	}

	private static IPackage ResolveDependencyCore(IEnumerable<IPackage> packages, PackageDependency dependency, DependencyVersion dependencyVersion)
	{
		if (dependency.VersionSpec != null)
		{
			packages = from p in packages.FindByVersion(dependency.VersionSpec)
				orderby p.Version
				select p;
			return packages.SelectDependency(dependencyVersion);
		}
		return packages.OrderByDescending((IPackage p) => p.Version).FirstOrDefault();
	}
}
