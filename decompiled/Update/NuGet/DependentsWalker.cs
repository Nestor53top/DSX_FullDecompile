using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet;

internal class DependentsWalker : PackageWalker, IDependentsResolver
{
	protected override bool RaiseErrorOnCycle => false;

	protected override bool IgnoreWalkInfo => true;

	protected IPackageRepository Repository { get; private set; }

	private IDictionary<IPackage, HashSet<IPackage>> DependentsLookup { get; set; }

	internal DependentsWalker(IPackageRepository repository)
		: this(repository, null)
	{
	}

	public DependentsWalker(IPackageRepository repository, FrameworkName targetFramework)
		: base(targetFramework)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		Repository = repository;
	}

	protected override IPackage ResolveDependency(PackageDependency dependency)
	{
		return DependencyResolveUtility.ResolveDependency(Repository, dependency, allowPrereleaseVersions: true, preferListedPackages: false);
	}

	protected override bool OnAfterResolveDependency(IPackage package, IPackage dependency)
	{
		if (!DependentsLookup.TryGetValue(dependency, out var value))
		{
			value = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);
			DependentsLookup[dependency] = value;
		}
		value.Add(package);
		return base.OnAfterResolveDependency(package, dependency);
	}

	public IEnumerable<IPackage> GetDependents(IPackage package)
	{
		if (DependentsLookup == null)
		{
			DependentsLookup = new Dictionary<IPackage, HashSet<IPackage>>(PackageEqualityComparer.IdAndVersion);
			foreach (IPackage package2 in Repository.GetPackages())
			{
				Walk(package2);
			}
		}
		if (DependentsLookup.TryGetValue(package, out var value))
		{
			return value;
		}
		return Enumerable.Empty<IPackage>();
	}
}
