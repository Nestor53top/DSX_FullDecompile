using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet;

internal class PackageSorter : PackageWalker
{
	private IPackageRepository _repository;

	private IList<IPackage> _sortedPackages;

	protected override bool RaiseErrorOnCycle => false;

	protected override bool IgnoreWalkInfo => true;

	protected override bool SkipDependencyResolveError => true;

	internal PackageSorter()
	{
	}

	public PackageSorter(FrameworkName targetFramework)
		: base(targetFramework)
	{
	}

	protected override void OnAfterPackageWalk(IPackage package)
	{
		base.OnAfterPackageWalk(package);
		_sortedPackages.Add(package);
	}

	protected override IPackage ResolveDependency(PackageDependency dependency)
	{
		return DependencyResolveUtility.ResolveDependency(_repository, dependency, allowPrereleaseVersions: true, preferListedPackages: false);
	}

	protected override void OnDependencyResolveError(PackageDependency dependency)
	{
	}

	public IEnumerable<IPackage> GetPackagesByDependencyOrder(IPackageRepository repository)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		base.Marker.Clear();
		_repository = repository;
		_sortedPackages = new List<IPackage>();
		foreach (IPackage package in _repository.GetPackages())
		{
			Walk(package);
		}
		return _sortedPackages;
	}
}
