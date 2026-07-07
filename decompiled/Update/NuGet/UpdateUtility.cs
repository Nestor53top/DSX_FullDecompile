using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NuGet.Resolver;
using NuGet.Resources;

namespace NuGet;

internal class UpdateUtility
{
	public ActionResolver Resolver { get; private set; }

	public bool Safe { get; set; }

	public ILogger Logger { get; set; }

	public bool AllowPrereleaseVersions { get; set; }

	public UpdateUtility(ActionResolver resolver)
	{
		Resolver = resolver;
		Logger = NullLogger.Instance;
	}

	public IEnumerable<NuGet.Resolver.PackageAction> ResolveActionsForUpdate(string id, SemanticVersion version, IEnumerable<IProjectManager> projectManagers, bool projectNameSpecified)
	{
		if (string.IsNullOrEmpty(id))
		{
			return ResolveActionsToUpdateAllPackages(projectManagers);
		}
		return ResolveActionsToUpdateOnePackage(id, version, projectManagers, projectNameSpecified);
	}

	private IEnumerable<NuGet.Resolver.PackageAction> ResolveActionsToUpdateAllPackages(IEnumerable<IProjectManager> projectManagers)
	{
		IEnumerable<IPackage> enumerable = new PackageSorter(null).GetPackagesByDependencyOrder(projectManagers.First().PackageManager.LocalRepository).Reverse();
		foreach (IProjectManager projectManager in projectManagers)
		{
			foreach (IPackage item in enumerable)
			{
				AddUpdateOperations(item.Id, null, new IProjectManager[1] { projectManager });
			}
		}
		return Resolver.ResolveActions();
	}

	private void AddUpdateOperations(string id, SemanticVersion version, IEnumerable<IProjectManager> projectManagers)
	{
		if (!Safe)
		{
			foreach (IProjectManager projectManager in projectManagers)
			{
				AddUnsafeUpdateOperation(id, version, version != null, projectManager);
			}
			return;
		}
		foreach (IProjectManager projectManager2 in projectManagers)
		{
			IPackage package = projectManager2.LocalRepository.FindPackage(id);
			if (package != null)
			{
				IVersionSpec safeRange = VersionUtility.GetSafeRange(package.Version);
				IPackage package2 = projectManager2.PackageManager.SourceRepository.FindPackage(id, safeRange, projectManager2.ConstraintProvider, AllowPrereleaseVersions, allowUnlisted: false);
				Resolver.AddOperation(PackageAction.Install, package2, projectManager2);
			}
		}
	}

	private void AddUnsafeUpdateOperation(string id, SemanticVersion version, bool targetVersionSetExplicitly, IProjectManager projectManager)
	{
		IPackage package = projectManager.LocalRepository.FindPackage(id);
		if (package != null)
		{
			Logger.Log(MessageLevel.Debug, NuGetResources.Debug_LookingForUpdates, id);
			IPackage package2 = projectManager.PackageManager.SourceRepository.FindPackage(id, version, projectManager.ConstraintProvider, AllowPrereleaseVersions, allowUnlisted: false);
			if (package2 != null && package.Version != package2.Version && (AllowPrereleaseVersions || targetVersionSetExplicitly || package.IsReleaseVersion() || !package2.IsReleaseVersion() || package.Version < package2.Version))
			{
				Logger.Log(MessageLevel.Info, NuGetResources.Log_UpdatingPackages, package2.Id, package.Version, package2.Version, projectManager.Project.ProjectName);
				Resolver.AddOperation(PackageAction.Install, package2, projectManager);
			}
			IVersionSpec constraint = projectManager.ConstraintProvider.GetConstraint(package2.Id);
			if (constraint != null)
			{
				Logger.Log(MessageLevel.Info, NuGetResources.Log_ApplyingConstraints, package2.Id, VersionUtility.PrettyPrint(constraint), projectManager.ConstraintProvider.Source);
			}
			Logger.Log(MessageLevel.Info, NuGetResources.Log_NoUpdatesAvailableForProject, package2.Id, projectManager.Project.ProjectName);
		}
	}

	private IEnumerable<NuGet.Resolver.PackageAction> ResolveActionsToUpdateOnePackage(string id, SemanticVersion version, IEnumerable<IProjectManager> projectManagers, bool projectNameSpecified)
	{
		IPackageManager packageManager = projectManagers.First().PackageManager;
		if ((projectNameSpecified ? FindPackageToUpdate(id, version, packageManager, projectManagers.First()) : FindPackageToUpdate(id, version, packageManager, projectManagers, Logger)).Item2 == null)
		{
			IPackage package = packageManager.SourceRepository.FindPackage(id, version, AllowPrereleaseVersions, allowUnlisted: false);
			if (package == null)
			{
				Logger.Log(MessageLevel.Info, "No updates available for {0}", id);
				return Enumerable.Empty<NuGet.Resolver.PackageAction>();
			}
			Resolver.AddOperation(PackageAction.Update, package, new NullProjectManager(packageManager));
		}
		else
		{
			AddUpdateOperations(id, version, projectManagers);
		}
		return Resolver.ResolveActions();
	}

	public static Tuple<IPackage, IProjectManager> FindPackageToUpdate(string id, SemanticVersion version, IPackageManager packageManager, IProjectManager projectManager)
	{
		IPackage package = null;
		package = projectManager.LocalRepository.FindPackage(id, null);
		if (package != null)
		{
			return Tuple.Create(package, projectManager);
		}
		if (version != null)
		{
			package = packageManager.LocalRepository.FindPackage(id, version);
		}
		else
		{
			List<IPackage> list = packageManager.LocalRepository.FindPackagesById(id).ToList();
			if (list.Count > 1)
			{
				if (list.Any((IPackage p) => packageManager.IsProjectLevel(p)))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown package in Project {0}: {1}", new object[2]
					{
						list[0].Id,
						projectManager.Project.ProjectName
					}));
				}
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Ambiguous update: {0}", new object[1] { list[0].Id }));
			}
			package = list.SingleOrDefault();
		}
		if (package == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown Package: {0}", new object[1] { id }));
		}
		if (packageManager.IsProjectLevel(package))
		{
			if (version == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown package {0} in project {1}", new object[2]
				{
					package.Id,
					projectManager.Project.ProjectName
				}));
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown package {0} in project {1}", new object[2]
			{
				package.GetFullName(),
				projectManager.Project.ProjectName
			}));
		}
		return Tuple.Create<IPackage, IProjectManager>(package, null);
	}

	public static Tuple<IPackage, IProjectManager> FindPackageToUpdate(string id, SemanticVersion version, IPackageManager packageManager, IEnumerable<IProjectManager> projectManagers, ILogger logger)
	{
		IPackage package = null;
		foreach (IProjectManager projectManager in projectManagers)
		{
			package = projectManager.LocalRepository.FindPackage(id, null);
			if (package != null)
			{
				return Tuple.Create(package, projectManager);
			}
		}
		if (version != null)
		{
			package = packageManager.LocalRepository.FindPackage(id, version);
		}
		else
		{
			List<IPackage> list = packageManager.LocalRepository.FindPackagesById(id).ToList();
			foreach (IPackage item in list)
			{
				if (!packageManager.IsProjectLevel(item))
				{
					if (list.Count > 1)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Ambiguous update: {0}", new object[1] { id }));
					}
					package = item;
					break;
				}
				if (!packageManager.LocalRepository.IsReferenced(item.Id, item.Version))
				{
					logger.Log(MessageLevel.Warning, string.Format(CultureInfo.CurrentCulture, "Package not referenced by any project {0}, {1}", new object[2] { item.Id, item.Version }));
					continue;
				}
				package = item;
				break;
			}
			if (package == null)
			{
				throw new PackageNotInstalledException(string.Format(CultureInfo.CurrentCulture, "Package not installed in any project: {0}", new object[1] { id }));
			}
		}
		if (package == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown Package: {0}", new object[1] { id }));
		}
		return Tuple.Create<IPackage, IProjectManager>(package, null);
	}
}
