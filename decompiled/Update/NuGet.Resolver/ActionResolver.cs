using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet.Resolver;

internal class ActionResolver
{
	private class Operation
	{
		public NuGet.PackageAction OperationType { get; set; }

		public IPackage Package { get; set; }

		public IProjectManager ProjectManager { get; set; }
	}

	private List<Operation> _operations;

	private Dictionary<IProjectManager, VirtualRepository> _virtualProjectRepos;

	private Dictionary<IPackageManager, VirtualRepository> _virtualPackageRepos;

	private Dictionary<IPackageManager, Dictionary<IPackage, int>> _packageRefCounts;

	public DependencyVersion DependencyVersion { get; set; }

	public bool IgnoreDependencies { get; set; }

	public bool AllowPrereleaseVersions { get; set; }

	public bool ForceRemove { get; set; }

	public bool RemoveDependencies { get; set; }

	public ILogger Logger { get; set; }

	public ActionResolver()
	{
		Logger = NullLogger.Instance;
		DependencyVersion = DependencyVersion.Lowest;
		_operations = new List<Operation>();
	}

	public void AddOperation(NuGet.PackageAction operationType, IPackage package, IProjectManager projectManager)
	{
		_operations.Add(new Operation
		{
			OperationType = operationType,
			Package = package,
			ProjectManager = projectManager
		});
	}

	public IEnumerable<PackageAction> ResolveActions()
	{
		InitilizeVirtualRepos();
		InitializeRefCount();
		List<PackageAction> list = new List<PackageAction>();
		foreach (Operation operation in _operations)
		{
			list.AddRange(ResolveActionsForOperation(operation));
		}
		return list;
	}

	private void InitilizeVirtualRepos()
	{
		_virtualProjectRepos = new Dictionary<IProjectManager, VirtualRepository>();
		_virtualPackageRepos = new Dictionary<IPackageManager, VirtualRepository>();
		foreach (Operation operation in _operations)
		{
			if (!_virtualProjectRepos.ContainsKey(operation.ProjectManager))
			{
				_virtualProjectRepos.Add(operation.ProjectManager, new VirtualRepository(operation.ProjectManager.LocalRepository));
			}
			IPackageManager packageManager = operation.ProjectManager.PackageManager;
			if (!_virtualPackageRepos.ContainsKey(packageManager))
			{
				_virtualPackageRepos.Add(packageManager, new VirtualRepository(packageManager.LocalRepository));
			}
		}
	}

	private void InitializeRefCount()
	{
		_packageRefCounts = new Dictionary<IPackageManager, Dictionary<IPackage, int>>();
		foreach (IPackageManager item in _operations.Select((Operation op) => op.ProjectManager.PackageManager))
		{
			Dictionary<IPackage, int> dictionary = new Dictionary<IPackage, int>(PackageEqualityComparer.IdAndVersion);
			foreach (IPackage package in item.LocalRepository.GetPackages())
			{
				dictionary[package] = 0;
			}
			foreach (IPackageRepository item2 in item.LocalRepository.LoadProjectRepositories())
			{
				foreach (IPackage package2 in item2.GetPackages())
				{
					if (dictionary.ContainsKey(package2))
					{
						dictionary[package2]++;
					}
					else
					{
						dictionary[package2] = 1;
					}
				}
			}
			foreach (IPackage package3 in item.LocalRepository.GetPackages())
			{
				if (dictionary[package3] == 0)
				{
					dictionary[package3] = 1;
				}
			}
			_packageRefCounts[item] = dictionary;
		}
	}

	private IEnumerable<PackageOperation> ResolveOperationsToInstallProjectLevelPackage(Operation operation)
	{
		return new UpdateWalker(_virtualProjectRepos[operation.ProjectManager], dependentsResolver: new DependentsWalker(operation.ProjectManager.PackageManager.LocalRepository, operation.ProjectManager.GetTargetFrameworkForPackage(operation.Package.Id))
		{
			DependencyVersion = DependencyVersion
		}, sourceRepository: operation.ProjectManager.PackageManager.DependencyResolver, constraintProvider: operation.ProjectManager.ConstraintProvider, targetFramework: operation.ProjectManager.Project.TargetFramework, logger: Logger ?? NullLogger.Instance, updateDependencies: !IgnoreDependencies, allowPrereleaseVersions: AllowPrereleaseVersions)
		{
			AcceptedTargets = PackageTargets.All,
			DependencyVersion = DependencyVersion
		}.ResolveOperations(operation.Package);
	}

	private IEnumerable<PackageOperation> ResolveOperationsToUninstallProjectLevelPackage(Operation operation)
	{
		VirtualRepository repository = _virtualProjectRepos[operation.ProjectManager];
		FrameworkName targetFrameworkForPackage = operation.ProjectManager.GetTargetFrameworkForPackage(operation.Package.Id);
		return new UninstallWalker(repository, new DependentsWalker(repository, targetFrameworkForPackage), targetFrameworkForPackage, NullLogger.Instance, RemoveDependencies, ForceRemove).ResolveOperations(operation.Package);
	}

	private IEnumerable<PackageOperation> ResolveOperationsToUninstallSolutionLevelPackage(Operation operation)
	{
		IEnumerable<PackageOperation> enumerable = new UninstallWalker(_virtualPackageRepos[operation.ProjectManager.PackageManager], new DependentsWalker(operation.ProjectManager.PackageManager.LocalRepository, null), null, NullLogger.Instance, RemoveDependencies, ForceRemove).ResolveOperations(operation.Package);
		foreach (PackageOperation item in enumerable)
		{
			item.Target = PackageOperationTarget.PackagesFolder;
		}
		return enumerable;
	}

	private IEnumerable<PackageOperation> ResolveOperationsToInstallSolutionLevelPackage(Operation operation)
	{
		IEnumerable<PackageOperation> enumerable = new InstallWalker(_virtualPackageRepos[operation.ProjectManager.PackageManager], operation.ProjectManager.PackageManager.DependencyResolver, null, Logger, IgnoreDependencies, AllowPrereleaseVersions, DependencyVersion).ResolveOperations(operation.Package);
		foreach (PackageOperation item in enumerable)
		{
			item.Target = PackageOperationTarget.PackagesFolder;
		}
		return enumerable;
	}

	private IEnumerable<PackageOperation> ResolveOperationsToUpdateSolutionLevelPackage(Operation operation)
	{
		VirtualRepository virtualRepository = _virtualPackageRepos[operation.ProjectManager.PackageManager];
		IEnumerable<PackageOperation> enumerable = new UpdateWalker(virtualRepository, operation.ProjectManager.PackageManager.DependencyResolver, new DependentsWalker(virtualRepository, null)
		{
			DependencyVersion = DependencyVersion
		}, NullConstraintProvider.Instance, null, Logger ?? NullLogger.Instance, !IgnoreDependencies, AllowPrereleaseVersions)
		{
			AcceptedTargets = PackageTargets.All,
			DependencyVersion = DependencyVersion
		}.ResolveOperations(operation.Package);
		foreach (PackageOperation item in enumerable)
		{
			item.Target = PackageOperationTarget.PackagesFolder;
		}
		return enumerable;
	}

	private IEnumerable<PackageAction> ResolveActionsForOperation(Operation operation)
	{
		IEnumerable<PackageOperation> enumerable = Enumerable.Empty<PackageOperation>();
		bool flag = operation.ProjectManager.PackageManager.IsProjectLevel(operation.Package);
		enumerable = ((operation.OperationType == NuGet.PackageAction.Install) ? ((!flag) ? ResolveOperationsToInstallSolutionLevelPackage(operation) : ResolveOperationsToInstallProjectLevelPackage(operation)) : ((operation.OperationType == NuGet.PackageAction.Update) ? ((!flag) ? ResolveOperationsToUpdateSolutionLevelPackage(operation) : ResolveOperationsToInstallProjectLevelPackage(operation)) : ((!flag) ? ResolveOperationsToUninstallSolutionLevelPackage(operation) : ResolveOperationsToUninstallProjectLevelPackage(operation))));
		List<PackageAction> list = new List<PackageAction>();
		foreach (PackageOperation item in enumerable)
		{
			PackageActionType actionType = ((item.Action != NuGet.PackageAction.Install) ? PackageActionType.Uninstall : PackageActionType.Install);
			if (item.Target == PackageOperationTarget.Project)
			{
				list.Add(new PackageProjectAction(actionType, item.Package, operation.ProjectManager));
			}
			else
			{
				list.Add(new PackageSolutionAction(actionType, item.Package, operation.ProjectManager.PackageManager));
			}
		}
		IList<PackageAction> list2 = ResolveFinalActions(operation.ProjectManager.PackageManager, list);
		UpdateVirtualRepos(list2);
		return list2;
	}

	private void UpdateVirtualRepos(IList<PackageAction> actions)
	{
		foreach (PackageAction action in actions)
		{
			if (!(action is PackageProjectAction packageProjectAction))
			{
				PackageSolutionAction packageSolutionAction = (PackageSolutionAction)action;
				VirtualRepository virtualRepository = _virtualPackageRepos[packageSolutionAction.PackageManager];
				if (packageSolutionAction.ActionType == PackageActionType.AddToPackagesFolder)
				{
					virtualRepository.AddPackage(packageSolutionAction.Package);
				}
				else if (packageSolutionAction.ActionType == PackageActionType.DeleteFromPackagesFolder)
				{
					virtualRepository.RemovePackage(packageSolutionAction.Package);
				}
			}
			else
			{
				VirtualRepository virtualRepository2 = _virtualProjectRepos[packageProjectAction.ProjectManager];
				if (packageProjectAction.ActionType == PackageActionType.Install)
				{
					virtualRepository2.AddPackage(action.Package);
				}
				else
				{
					virtualRepository2.RemovePackage(action.Package);
				}
			}
		}
	}

	private IList<PackageAction> ResolveFinalActions(IPackageManager packageManager, IEnumerable<PackageAction> projectActions)
	{
		Dictionary<IPackage, int> dictionary = _packageRefCounts[packageManager];
		List<PackageSolutionAction> list = new List<PackageSolutionAction>();
		List<PackageSolutionAction> list2 = new List<PackageSolutionAction>();
		foreach (PackageAction projectAction in projectActions)
		{
			if (projectAction.ActionType == PackageActionType.Uninstall)
			{
				if (dictionary.ContainsKey(projectAction.Package))
				{
					dictionary[projectAction.Package]--;
					if (dictionary[projectAction.Package] <= 0)
					{
						list2.Add(new PackageSolutionAction(PackageActionType.DeleteFromPackagesFolder, projectAction.Package, packageManager));
					}
				}
				continue;
			}
			bool flag = false;
			if (dictionary.TryGetValue(projectAction.Package, out var value))
			{
				if (value > 0)
				{
					flag = true;
				}
				dictionary[projectAction.Package] = value + 1;
			}
			else
			{
				dictionary.Add(projectAction.Package, 1);
			}
			if (!flag)
			{
				list.Add(new PackageSolutionAction(PackageActionType.AddToPackagesFolder, projectAction.Package, packageManager));
			}
		}
		List<PackageAction> list3 = new List<PackageAction>();
		list3.AddRange(list);
		list3.AddRange(projectActions);
		list3.AddRange(list2);
		return list3;
	}
}
