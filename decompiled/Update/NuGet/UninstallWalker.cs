using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class UninstallWalker : PackageWalker, IPackageOperationResolver
{
	private readonly IDictionary<IPackage, IEnumerable<IPackage>> _forcedRemoved = new Dictionary<IPackage, IEnumerable<IPackage>>(PackageEqualityComparer.IdAndVersion);

	private readonly IDictionary<IPackage, IEnumerable<IPackage>> _skippedPackages = new Dictionary<IPackage, IEnumerable<IPackage>>(PackageEqualityComparer.IdAndVersion);

	private readonly bool _removeDependencies;

	protected ILogger Logger { get; private set; }

	protected IPackageRepository Repository { get; private set; }

	protected override bool IgnoreDependencies => !_removeDependencies;

	protected override bool SkipDependencyResolveError => true;

	internal bool DisableWalkInfo { get; set; }

	protected override bool IgnoreWalkInfo
	{
		get
		{
			if (!DisableWalkInfo)
			{
				return base.IgnoreWalkInfo;
			}
			return true;
		}
	}

	private Stack<PackageOperation> Operations { get; set; }

	public bool Force { get; private set; }

	public bool ThrowOnConflicts { get; set; }

	protected IDependentsResolver DependentsResolver { get; private set; }

	internal UninstallWalker(IPackageRepository repository, IDependentsResolver dependentsResolver, ILogger logger, bool removeDependencies, bool forceRemove)
		: this(repository, dependentsResolver, null, logger, removeDependencies, forceRemove)
	{
	}

	public UninstallWalker(IPackageRepository repository, IDependentsResolver dependentsResolver, FrameworkName targetFramework, ILogger logger, bool removeDependencies, bool forceRemove)
		: base(targetFramework)
	{
		if (dependentsResolver == null)
		{
			throw new ArgumentNullException("dependentsResolver");
		}
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		if (logger == null)
		{
			throw new ArgumentNullException("logger");
		}
		Logger = logger;
		Repository = repository;
		DependentsResolver = dependentsResolver;
		Force = forceRemove;
		ThrowOnConflicts = true;
		Operations = new Stack<PackageOperation>();
		_removeDependencies = removeDependencies;
	}

	protected override void OnBeforePackageWalk(IPackage package)
	{
		IEnumerable<IPackage> dependents = GetDependents(package);
		if (dependents.Any())
		{
			if (Force)
			{
				_forcedRemoved[package] = dependents;
			}
			else if (ThrowOnConflicts)
			{
				throw CreatePackageHasDependentsException(package, dependents);
			}
		}
	}

	protected override bool OnAfterResolveDependency(IPackage package, IPackage dependency)
	{
		if (!Force)
		{
			IEnumerable<IPackage> dependents = GetDependents(dependency);
			if (dependents.Any())
			{
				_skippedPackages[dependency] = dependents;
				return false;
			}
		}
		return true;
	}

	protected override void OnAfterPackageWalk(IPackage package)
	{
		Operations.Push(new PackageOperation(package, PackageAction.Uninstall));
	}

	protected override IPackage ResolveDependency(PackageDependency dependency)
	{
		return DependencyResolveUtility.ResolveDependency(Repository, dependency, allowPrereleaseVersions: true, preferListedPackages: false);
	}

	protected virtual void WarnRemovingPackageBreaksDependents(IPackage package, IEnumerable<IPackage> dependents)
	{
		Logger.Log(MessageLevel.Warning, NuGetResources.Warning_UninstallingPackageWillBreakDependents, package.GetFullName(), string.Join(", ", dependents.Select((IPackage d) => d.GetFullName())));
	}

	protected virtual InvalidOperationException CreatePackageHasDependentsException(IPackage package, IEnumerable<IPackage> dependents)
	{
		if (dependents.Count() == 1)
		{
			return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageHasDependent, new object[2]
			{
				package.GetFullName(),
				dependents.Single().GetFullName()
			}));
		}
		return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageHasDependents, new object[2]
		{
			package.GetFullName(),
			string.Join(", ", dependents.Select((IPackage d) => d.GetFullName()))
		}));
	}

	protected override void OnDependencyResolveError(PackageDependency dependency)
	{
		Logger.Log(MessageLevel.Warning, NuGetResources.UnableToLocateDependency, dependency);
	}

	public IEnumerable<PackageOperation> ResolveOperations(IPackage package)
	{
		Operations.Clear();
		base.Marker.Clear();
		Walk(package);
		foreach (KeyValuePair<IPackage, IEnumerable<IPackage>> item in _forcedRemoved)
		{
			Logger.Log(MessageLevel.Warning, NuGetResources.Warning_UninstallingPackageWillBreakDependents, item.Key, string.Join(", ", item.Value.Select((IPackage p) => p.GetFullName())));
		}
		foreach (KeyValuePair<IPackage, IEnumerable<IPackage>> skippedPackage in _skippedPackages)
		{
			Logger.Log(MessageLevel.Warning, NuGetResources.Warning_PackageSkippedBecauseItIsInUse, skippedPackage.Key, string.Join(", ", skippedPackage.Value.Select((IPackage p) => p.GetFullName())));
		}
		return Operations.Reduce();
	}

	private IEnumerable<IPackage> GetDependents(IPackage package)
	{
		return from p in DependentsResolver.GetDependents(package)
			where !IsConnected(p)
			select p;
	}

	private bool IsConnected(IPackage package)
	{
		if (base.Marker.Contains(package))
		{
			return true;
		}
		IEnumerable<IPackage> dependents = DependentsResolver.GetDependents(package);
		if (dependents.Any())
		{
			return dependents.All(IsConnected);
		}
		return false;
	}
}
