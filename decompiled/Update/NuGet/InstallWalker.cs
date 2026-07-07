using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class InstallWalker : PackageWalker, IPackageOperationResolver
{
	private class OperationLookup
	{
		private readonly List<PackageOperation> _operations = new List<PackageOperation>();

		private readonly Dictionary<PackageAction, Dictionary<IPackage, PackageOperation>> _operationLookup = new Dictionary<PackageAction, Dictionary<IPackage, PackageOperation>>();

		internal void Clear()
		{
			_operations.Clear();
			_operationLookup.Clear();
		}

		internal IList<PackageOperation> ToList()
		{
			return _operations;
		}

		internal IEnumerable<IPackage> GetPackages(PackageAction action)
		{
			Dictionary<IPackage, PackageOperation> packageLookup = GetPackageLookup(action);
			if (packageLookup != null)
			{
				return packageLookup.Keys;
			}
			return Enumerable.Empty<IPackage>();
		}

		internal void AddOperation(PackageOperation operation)
		{
			Dictionary<IPackage, PackageOperation> packageLookup = GetPackageLookup(operation.Action, createIfNotExists: true);
			if (!packageLookup.ContainsKey(operation.Package))
			{
				packageLookup.Add(operation.Package, operation);
				_operations.Add(operation);
			}
		}

		internal void RemoveOperation(IPackage package, PackageAction action)
		{
			Dictionary<IPackage, PackageOperation> packageLookup = GetPackageLookup(action);
			if (packageLookup != null && packageLookup.TryGetValue(package, out var value))
			{
				packageLookup.Remove(package);
				_operations.Remove(value);
			}
		}

		internal bool Contains(IPackage package, PackageAction action)
		{
			return GetPackageLookup(action)?.ContainsKey(package) ?? false;
		}

		private Dictionary<IPackage, PackageOperation> GetPackageLookup(PackageAction action, bool createIfNotExists = false)
		{
			if (!_operationLookup.TryGetValue(action, out var value) && createIfNotExists)
			{
				value = new Dictionary<IPackage, PackageOperation>(PackageEqualityComparer.IdAndVersion);
				_operationLookup.Add(action, value);
			}
			return value;
		}
	}

	private readonly bool _ignoreDependencies;

	private bool _allowPrereleaseVersions;

	private readonly OperationLookup _operations;

	private bool _isDowngrade;

	private readonly HashSet<IPackage> _packagesToKeep = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);

	private IDictionary<string, IList<IPackage>> _packagesByDependencyOrder;

	internal bool DisableWalkInfo { get; set; }

	internal bool CheckDowngrade { get; set; }

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

	protected ILogger Logger { get; private set; }

	protected IPackageRepository Repository { get; private set; }

	protected override bool IgnoreDependencies => _ignoreDependencies;

	protected override bool AllowPrereleaseVersions => _allowPrereleaseVersions;

	protected IDependencyResolver2 DependencyResolver { get; private set; }

	private IPackageConstraintProvider ConstraintProvider { get; set; }

	protected IList<PackageOperation> Operations => _operations.ToList();

	internal InstallWalker(IPackageRepository localRepository, IDependencyResolver2 dependencyResolver, ILogger logger, bool ignoreDependencies, bool allowPrereleaseVersions, DependencyVersion dependencyVersion)
		: this(localRepository, dependencyResolver, null, logger, ignoreDependencies, allowPrereleaseVersions, dependencyVersion)
	{
	}

	public InstallWalker(IPackageRepository localRepository, IDependencyResolver2 dependencyResolver, FrameworkName targetFramework, ILogger logger, bool ignoreDependencies, bool allowPrereleaseVersions, DependencyVersion dependencyVersion)
		: this(localRepository, dependencyResolver, NullConstraintProvider.Instance, targetFramework, logger, ignoreDependencies, allowPrereleaseVersions, dependencyVersion)
	{
	}

	public InstallWalker(IPackageRepository localRepository, IDependencyResolver2 dependencyResolver, IPackageConstraintProvider constraintProvider, FrameworkName targetFramework, ILogger logger, bool ignoreDependencies, bool allowPrereleaseVersions, DependencyVersion dependencyVersion)
		: base(targetFramework)
	{
		if (dependencyResolver == null)
		{
			throw new ArgumentNullException("dependencyResolver");
		}
		if (localRepository == null)
		{
			throw new ArgumentNullException("localRepository");
		}
		if (logger == null)
		{
			throw new ArgumentNullException("logger");
		}
		Repository = localRepository;
		Logger = logger;
		DependencyResolver = dependencyResolver;
		_ignoreDependencies = ignoreDependencies;
		ConstraintProvider = constraintProvider;
		_operations = new OperationLookup();
		_allowPrereleaseVersions = allowPrereleaseVersions;
		base.DependencyVersion = dependencyVersion;
		CheckDowngrade = true;
	}

	protected virtual ConflictResult GetConflict(IPackage package)
	{
		IPackage package2 = base.Marker.FindPackage(package.Id);
		if (package2 != null)
		{
			return new ConflictResult(package2, base.Marker, base.Marker);
		}
		return null;
	}

	protected override void OnBeforePackageWalk(IPackage package)
	{
		ConflictResult conflict = GetConflict(package);
		if (conflict != null && !PackageEqualityComparer.IdAndVersion.Equals(package, conflict.Package))
		{
			IEnumerable<IPackage> incompatiblePackages = from dependentPackage in GetDependents(conflict)
				let dependency = dependentPackage.FindDependency(package.Id, base.TargetFramework)
				where dependency != null && !dependency.VersionSpec.Satisfies(package.Version)
				select dependentPackage;
			if (incompatiblePackages.Any() && !TryUpdate(incompatiblePackages, conflict, package, out incompatiblePackages))
			{
				throw CreatePackageConflictException(package, conflict.Package, incompatiblePackages);
			}
			if (!_isDowngrade && package.Version < conflict.Package.Version)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.NewerVersionAlreadyReferenced, new object[1] { package.Id }));
			}
			Uninstall(conflict.Package, conflict.DependentsResolver, conflict.Repository);
		}
	}

	private void Uninstall(IPackage package, IDependentsResolver dependentsResolver, IPackageRepository repository)
	{
		_packagesToKeep.Remove(package);
		if (!base.Marker.Contains(package) && _operations.Contains(package, PackageAction.Uninstall))
		{
			return;
		}
		foreach (PackageOperation item in new UninstallWalker(repository, dependentsResolver, base.TargetFramework, NullLogger.Instance, !IgnoreDependencies, forceRemove: false)
		{
			DisableWalkInfo = DisableWalkInfo,
			ThrowOnConflicts = false
		}.ResolveOperations(package))
		{
			if (item.Action == PackageAction.Install || !_packagesToKeep.Contains(item.Package))
			{
				_operations.AddOperation(item);
			}
		}
	}

	private IPackage SelectDependency(IEnumerable<IPackage> dependencies)
	{
		return dependencies.SelectDependency(base.DependencyVersion);
	}

	private static IEnumerable<IPackage> FindCompatiblePackages(IDependencyResolver2 dependencyResolver, IPackageConstraintProvider constraintProvider, IEnumerable<string> packageIds, IPackage package, FrameworkName targetFramework, bool allowPrereleaseVersions)
	{
		return from p in dependencyResolver.FindPackages(packageIds)
			where allowPrereleaseVersions || p.IsReleaseVersion()
			let dependency = p.FindDependency(package.Id, targetFramework)
			let otherConstaint = constraintProvider.GetConstraint(p.Id)
			where dependency != null && dependency.VersionSpec.Satisfies(package.Version) && (otherConstaint == null || otherConstaint.Satisfies(package.Version))
			select p;
	}

	private bool TryUpdate(IEnumerable<IPackage> dependents, ConflictResult conflictResult, IPackage package, out IEnumerable<IPackage> incompatiblePackages)
	{
		Dictionary<string, IPackage> dependentsLookup = dependents.ToDictionary<IPackage, string>((IPackage d) => d.Id, StringComparer.OrdinalIgnoreCase);
		Dictionary<IPackage, IPackage> dictionary = new Dictionary<IPackage, IPackage>();
		foreach (IPackage dependent in dependents)
		{
			dictionary[dependent] = null;
		}
		foreach (var item in from p in FindCompatiblePackages(DependencyResolver, ConstraintProvider, dependentsLookup.Keys, package, base.TargetFramework, AllowPrereleaseVersions)
			group p by p.Id into g
			let oldPackage = dependentsLookup[g.Key]
			select new
			{
				OldPackage = oldPackage,
				NewPackage = SelectDependency(from p in g
					where p.Version > oldPackage.Version
					orderby p.Version
					select p)
			})
		{
			dictionary[item.OldPackage] = item.NewPackage;
		}
		incompatiblePackages = from p in dictionary
			where p.Value == null
			select p.Key;
		if (incompatiblePackages.Any())
		{
			return false;
		}
		IPackageConstraintProvider constraintProvider = ConstraintProvider;
		try
		{
			DefaultConstraintProvider defaultConstraintProvider = new DefaultConstraintProvider();
			defaultConstraintProvider.AddConstraint(package.Id, new VersionSpec(package.Version));
			ConstraintProvider = new AggregateConstraintProvider(ConstraintProvider, defaultConstraintProvider);
			base.Marker.MarkVisited(package);
			List<IPackage> list = new List<IPackage>();
			foreach (KeyValuePair<IPackage, IPackage> item2 in dictionary)
			{
				try
				{
					Uninstall(item2.Key, conflictResult.DependentsResolver, conflictResult.Repository);
					Walk(item2.Value);
				}
				catch
				{
					list.Add(item2.Key);
				}
			}
			incompatiblePackages = list;
			return !incompatiblePackages.Any();
		}
		finally
		{
			ConstraintProvider = constraintProvider;
			base.Marker.MarkProcessing(package);
		}
	}

	protected override void OnAfterPackageWalk(IPackage package)
	{
		if (!Repository.Exists(package))
		{
			PackageOperation packageOperation = new PackageOperation(package, PackageAction.Install);
			if (PackageWalker.GetPackageTarget(package) == PackageTargets.External)
			{
				packageOperation.Target = PackageOperationTarget.PackagesFolder;
			}
			_operations.AddOperation(packageOperation);
		}
		else
		{
			_operations.RemoveOperation(package, PackageAction.Uninstall);
			_packagesToKeep.Add(package);
		}
		if (_packagesByDependencyOrder != null)
		{
			if (!_packagesByDependencyOrder.TryGetValue(package.Id, out var value))
			{
				value = (_packagesByDependencyOrder[package.Id] = new List<IPackage>());
			}
			value.Add(package);
		}
	}

	protected override IPackage ResolveDependency(PackageDependency dependency)
	{
		Logger.Log(MessageLevel.Info, NuGetResources.Log_AttemptingToRetrievePackageFromSource, dependency);
		if (!_isDowngrade)
		{
			IPackage package = DependencyResolveUtility.ResolveDependency(Repository, dependency, ConstraintProvider, allowPrereleaseVersions: true, preferListedPackages: false, base.DependencyVersion);
			if (package != null)
			{
				return package;
			}
		}
		return DependencyResolver.ResolveDependency(dependency, ConstraintProvider, AllowPrereleaseVersions, preferListedPackages: true, base.DependencyVersion);
	}

	protected override void OnDependencyResolveError(PackageDependency dependency)
	{
		IVersionSpec constraint = ConstraintProvider.GetConstraint(dependency.Id);
		string text = string.Empty;
		if (constraint != null)
		{
			text = string.Format(CultureInfo.CurrentCulture, NuGetResources.AdditonalConstraintsDefined, new object[3]
			{
				dependency.Id,
				VersionUtility.PrettyPrint(constraint),
				ConstraintProvider.Source
			});
		}
		throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnableToResolveDependency + text, new object[1] { dependency }));
	}

	public IEnumerable<PackageOperation> ResolveOperations(IPackage package)
	{
		if (CheckDowngrade)
		{
			IPackage package2 = Repository.FindPackage(package.Id);
			if (package2 != null && package2.Version > package.Version)
			{
				_isDowngrade = true;
			}
		}
		else
		{
			_isDowngrade = false;
		}
		_operations.Clear();
		base.Marker.Clear();
		_packagesToKeep.Clear();
		Walk(package);
		return Operations.Reduce();
	}

	public IList<PackageOperation> ResolveOperations(IEnumerable<IPackage> packages, out IList<IPackage> packagesByDependencyOrder, bool allowPrereleaseVersionsBasedOnPackage = false)
	{
		_packagesByDependencyOrder = new Dictionary<string, IList<IPackage>>();
		_operations.Clear();
		base.Marker.Clear();
		_packagesToKeep.Clear();
		foreach (IPackage package in packages)
		{
			if (_operations.Contains(package, PackageAction.Install))
			{
				continue;
			}
			bool allowPrereleaseVersions = _allowPrereleaseVersions;
			try
			{
				if (allowPrereleaseVersionsBasedOnPackage)
				{
					_allowPrereleaseVersions = _allowPrereleaseVersions || !package.IsReleaseVersion();
				}
				Walk(package);
			}
			finally
			{
				_allowPrereleaseVersions = allowPrereleaseVersions;
			}
		}
		IEnumerable<IPackage> source = _packagesByDependencyOrder.SelectMany((KeyValuePair<string, IList<IPackage>> p) => p.Value).Distinct();
		packagesByDependencyOrder = source.Where((IPackage p) => packages.Any((IPackage q) => p.Id == q.Id && p.Version == q.Version)).ToList();
		_packagesByDependencyOrder.Clear();
		_packagesByDependencyOrder = null;
		return Operations.Reduce();
	}

	private IEnumerable<IPackage> GetDependents(ConflictResult conflict)
	{
		IEnumerable<IPackage> packages = _operations.GetPackages(PackageAction.Uninstall);
		return conflict.DependentsResolver.GetDependents(conflict.Package).Except(packages, (IEqualityComparer<IPackage>?)PackageEqualityComparer.IdAndVersion);
	}

	private static InvalidOperationException CreatePackageConflictException(IPackage resolvedPackage, IPackage package, IEnumerable<IPackage> dependents)
	{
		if (dependents.Count() == 1)
		{
			return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ConflictErrorWithDependent, new object[3]
			{
				package.GetFullName(),
				resolvedPackage.GetFullName(),
				dependents.Single().Id
			}));
		}
		return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ConflictErrorWithDependents, new object[3]
		{
			package.GetFullName(),
			resolvedPackage.GetFullName(),
			string.Join(", ", dependents.Select((IPackage d) => d.Id))
		}));
	}
}
