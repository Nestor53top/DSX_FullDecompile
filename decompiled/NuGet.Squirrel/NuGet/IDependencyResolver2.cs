using System.Collections.Generic;

namespace NuGet;

public interface IDependencyResolver2
{
	IPackage ResolveDependency(PackageDependency dependency, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool preferListedPackages, DependencyVersion dependencyVersion);

	IEnumerable<IPackage> FindPackages(IEnumerable<string> packageIds);
}
