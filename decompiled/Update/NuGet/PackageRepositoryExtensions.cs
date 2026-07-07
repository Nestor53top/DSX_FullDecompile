using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Versioning;
using NuGet.Resources;
using NuGet.V3Interop;

namespace NuGet;

internal static class PackageRepositoryExtensions
{
	public static IDisposable StartOperation(this IPackageRepository self, string operation, string mainPackageId, string mainPackageVersion)
	{
		if (self is IOperationAwareRepository operationAwareRepository)
		{
			return operationAwareRepository.StartOperation(operation, mainPackageId, mainPackageVersion);
		}
		return DisposableAction.NoOp;
	}

	public static bool Exists(this IPackageRepository repository, IPackageName package)
	{
		return repository.Exists(package.Id, package.Version);
	}

	public static bool Exists(this IPackageRepository repository, string packageId)
	{
		return repository.Exists(packageId, null);
	}

	public static bool Exists(this IPackageRepository repository, string packageId, SemanticVersion version)
	{
		if (repository is IPackageLookup packageLookup && !string.IsNullOrEmpty(packageId) && version != null)
		{
			return packageLookup.Exists(packageId, version);
		}
		return repository.FindPackage(packageId, version) != null;
	}

	public static bool TryFindPackage(this IPackageRepository repository, string packageId, SemanticVersion version, out IPackage package)
	{
		package = repository.FindPackage(packageId, version);
		return package != null;
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId)
	{
		return repository.FindPackage(packageId, null);
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId, SemanticVersion version)
	{
		return repository.FindPackage(packageId, version, NullConstraintProvider.Instance, allowPrereleaseVersions: true, allowUnlisted: true);
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId, SemanticVersion version, bool allowPrereleaseVersions, bool allowUnlisted)
	{
		return repository.FindPackage(packageId, version, NullConstraintProvider.Instance, allowPrereleaseVersions, allowUnlisted);
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId, SemanticVersion version, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool allowUnlisted)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		if (packageId == null)
		{
			throw new ArgumentNullException("packageId");
		}
		IPackage package;
		if (version != null)
		{
			allowUnlisted = true;
		}
		else if (!allowUnlisted && (constraintProvider == null || constraintProvider == NullConstraintProvider.Instance) && repository is ILatestPackageLookup latestPackageLookup && latestPackageLookup.TryFindLatestPackageById(packageId, allowPrereleaseVersions, out package))
		{
			return package;
		}
		if (repository is IPackageLookup packageLookup && version != null)
		{
			return packageLookup.FindPackage(packageId, version);
		}
		IEnumerable<IPackage> source = repository.FindPackagesById(packageId);
		source = from p in source.ToList()
			orderby p.Version descending
			select p;
		if (!allowUnlisted)
		{
			source = source.Where(PackageExtensions.IsListed);
		}
		if (version != null)
		{
			source = source.Where((IPackage p) => p.Version == version);
		}
		else if (constraintProvider != null)
		{
			source = DependencyResolveUtility.FilterPackagesByConstraints(constraintProvider, source, packageId, allowPrereleaseVersions);
		}
		return source.FirstOrDefault();
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId, IVersionSpec versionSpec, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool allowUnlisted)
	{
		IEnumerable<IPackage> enumerable = repository.FindPackages(packageId, versionSpec, allowPrereleaseVersions, allowUnlisted);
		if (constraintProvider != null)
		{
			enumerable = DependencyResolveUtility.FilterPackagesByConstraints(constraintProvider, enumerable, packageId, allowPrereleaseVersions);
		}
		return enumerable.FirstOrDefault();
	}

	public static IEnumerable<IPackage> FindPackages(this IPackageRepository repository, IEnumerable<string> packageIds)
	{
		if (packageIds == null)
		{
			throw new ArgumentNullException("packageIds");
		}
		IV3InteropRepository v3Repo = repository as IV3InteropRepository;
		if (v3Repo != null)
		{
			return packageIds.SelectMany((string id) => v3Repo.FindPackagesById(id)).ToList();
		}
		return repository.FindPackages(packageIds, GetFilterExpression);
	}

	public static IEnumerable<IPackage> FindPackagesById(this IPackageRepository repository, string packageId)
	{
		if (repository is IV3InteropRepository iV3InteropRepository)
		{
			return iV3InteropRepository.FindPackagesById(packageId);
		}
		if (repository is IPackageLookup packageLookup)
		{
			return packageLookup.FindPackagesById(packageId).ToList();
		}
		return FindPackagesByIdCore(repository, packageId);
	}

	internal static IEnumerable<IPackage> FindPackagesByIdCore(IPackageRepository repository, string packageId)
	{
		if (repository is ICultureAwareRepository cultureAwareRepository)
		{
			packageId = packageId.ToLower(cultureAwareRepository.Culture);
		}
		else
		{
			packageId = packageId.ToLower(CultureInfo.CurrentCulture);
		}
		return (from p in repository.GetPackages()
			where p.Id.ToLower() == packageId
			orderby p.Id
			select p).ToList();
	}

	private static IEnumerable<IPackage> FindPackages<T>(this IPackageRepository repository, IEnumerable<T> items, Func<IEnumerable<T>, Expression<Func<IPackage, bool>>> filterSelector)
	{
		while (items.Any())
		{
			IEnumerable<T> arg = items.Take(10);
			Expression<Func<IPackage, bool>> predicate = filterSelector(arg);
			IOrderedQueryable<IPackage> orderedQueryable = from p in repository.GetPackages().Where(predicate)
				orderby p.Id
				select p;
			foreach (IPackage item in orderedQueryable)
			{
				yield return item;
			}
			items = items.Skip(10);
		}
	}

	public static IEnumerable<IPackage> FindPackages(this IPackageRepository repository, string packageId, IVersionSpec versionSpec, bool allowPrereleaseVersions, bool allowUnlisted)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		if (packageId == null)
		{
			throw new ArgumentNullException("packageId");
		}
		IEnumerable<IPackage> enumerable = from p in repository.FindPackagesById(packageId)
			orderby p.Version descending
			select p;
		if (!allowUnlisted)
		{
			enumerable = enumerable.Where(PackageExtensions.IsListed);
		}
		if (versionSpec != null)
		{
			enumerable = enumerable.FindByVersion(versionSpec);
		}
		return DependencyResolveUtility.FilterPackagesByConstraints(NullConstraintProvider.Instance, enumerable, packageId, allowPrereleaseVersions);
	}

	public static IPackage FindPackage(this IPackageRepository repository, string packageId, IVersionSpec versionSpec, bool allowPrereleaseVersions, bool allowUnlisted)
	{
		return repository.FindPackages(packageId, versionSpec, allowPrereleaseVersions, allowUnlisted).FirstOrDefault();
	}

	public static PackageDependency FindDependency(this IPackageMetadata package, string packageId, FrameworkName targetFramework)
	{
		return (from dependency in package.GetCompatiblePackageDependencies(targetFramework)
			where dependency.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)
			select dependency).FirstOrDefault();
	}

	public static IQueryable<IPackage> Search(this IPackageRepository repository, string searchTerm, bool allowPrereleaseVersions)
	{
		return repository.Search(searchTerm, Enumerable.Empty<string>(), allowPrereleaseVersions);
	}

	public static IQueryable<IPackage> Search(this IPackageRepository repository, string searchTerm, IEnumerable<string> targetFrameworks, bool allowPrereleaseVersions, bool includeDelisted = false)
	{
		if (targetFrameworks == null)
		{
			throw new ArgumentNullException("targetFrameworks");
		}
		if (repository is IServiceBasedRepository serviceBasedRepository)
		{
			return serviceBasedRepository.Search(searchTerm, targetFrameworks, allowPrereleaseVersions, includeDelisted);
		}
		IEnumerable<IPackage> source = repository.GetPackages().Find(searchTerm).FilterByPrerelease(allowPrereleaseVersions);
		if (!includeDelisted)
		{
			source = source.Where((IPackage p) => p.IsListed());
		}
		return source.AsQueryable();
	}

	public static IEnumerable<IPackage> GetUpdates(this IPackageRepository repository, IEnumerable<IPackageName> packages, bool includePrerelease, bool includeAllVersions, IEnumerable<FrameworkName> targetFrameworks = null, IEnumerable<IVersionSpec> versionConstraints = null)
	{
		if (packages.IsEmpty())
		{
			return Enumerable.Empty<IPackage>();
		}
		if (!(repository is IServiceBasedRepository serviceBasedRepository))
		{
			return repository.GetUpdatesCore(packages, includePrerelease, includeAllVersions, targetFrameworks, versionConstraints);
		}
		return serviceBasedRepository.GetUpdates(packages, includePrerelease, includeAllVersions, targetFrameworks, versionConstraints);
	}

	public static IEnumerable<IPackage> GetUpdatesCore(this IPackageRepository repository, IEnumerable<IPackageName> packages, bool includePrerelease, bool includeAllVersions, IEnumerable<FrameworkName> targetFramework, IEnumerable<IVersionSpec> versionConstraints)
	{
		List<IPackageName> list = packages.ToList();
		if (!list.Any())
		{
			return Enumerable.Empty<IPackage>();
		}
		IList<IVersionSpec> list2 = ((versionConstraints != null) ? ((IList<IVersionSpec>)versionConstraints.ToList()) : ((IList<IVersionSpec>)new IVersionSpec[list.Count]));
		if (list.Count != list2.Count)
		{
			throw new ArgumentException(NuGetResources.GetUpdatesParameterMismatch);
		}
		ILookup<string, IPackage> lookup = GetUpdateCandidates(repository, list, includePrerelease).ToList().ToLookup<IPackage, string>((IPackage package2) => package2.Id, StringComparer.OrdinalIgnoreCase);
		List<IPackage> list3 = new List<IPackage>();
		for (int num = 0; num < list.Count; num++)
		{
			IPackageName package = list[num];
			IVersionSpec constraint = list2[num];
			IEnumerable<IPackage> collection = lookup[package.Id].Where((IPackage candidate) => candidate.Version > package.Version && SupportsTargetFrameworks(targetFramework, candidate) && (constraint == null || constraint.Satisfies(candidate.Version)));
			list3.AddRange(collection);
		}
		if (!includeAllVersions)
		{
			return list3.CollapseById();
		}
		return list3;
	}

	private static bool SupportsTargetFrameworks(IEnumerable<FrameworkName> targetFramework, IPackage package)
	{
		if (!targetFramework.IsEmpty())
		{
			return targetFramework.Any((FrameworkName t) => VersionUtility.IsCompatible(t, package.GetSupportedFrameworks()));
		}
		return true;
	}

	public static IPackageRepository Clone(this IPackageRepository repository)
	{
		if (repository is ICloneableRepository cloneableRepository)
		{
			return cloneableRepository.Clone();
		}
		return repository;
	}

	private static IEnumerable<IPackage> GetUpdateCandidates(IPackageRepository repository, IEnumerable<IPackageName> packages, bool includePrerelease)
	{
		IEnumerable<IPackage> source = repository.FindPackages(packages, GetFilterExpression);
		if (!includePrerelease)
		{
			source = source.Where((IPackage p) => p.IsReleaseVersion());
		}
		return source.Where(PackageExtensions.IsListed);
	}

	private static Expression<Func<IPackage, bool>> GetFilterExpression(IEnumerable<IPackageName> packages)
	{
		return GetFilterExpression(packages.Select((IPackageName p) => p.Id));
	}

	private static Expression<Func<IPackage, bool>> GetFilterExpression(IEnumerable<string> ids)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(IPackageName));
		return Expression.Lambda<Func<IPackage, bool>>(ids.Select((string id) => GetCompareExpression(parameterExpression, id.ToLower())).Aggregate(Expression.OrElse), new ParameterExpression[1] { parameterExpression });
	}

	private static Expression GetCompareExpression(Expression parameterExpression, object value)
	{
		return Expression.Equal(Expression.Call(Expression.Property(parameterExpression, "Id"), typeof(string).GetMethod("ToLower", Type.EmptyTypes)), Expression.Constant(value));
	}

	internal static IPackage SelectDependency(this IEnumerable<IPackage> packages, DependencyVersion dependencyVersion)
	{
		if (packages == null || !packages.Any())
		{
			return null;
		}
		return dependencyVersion switch
		{
			DependencyVersion.Lowest => packages.FirstOrDefault(), 
			DependencyVersion.Highest => packages.LastOrDefault(), 
			DependencyVersion.HighestPatch => (from p in (from p in packages
					group p by new
					{
						p.Version.Version.Major,
						p.Version.Version.Minor
					} into g
					orderby g.Key.Major, g.Key.Minor
					select g).First()
				orderby p.Version descending
				select p).FirstOrDefault(), 
			DependencyVersion.HighestMinor => (from p in (from p in packages
					group p by new { p.Version.Version.Major } into g
					orderby g.Key.Major
					select g).First()
				orderby p.Version descending
				select p).FirstOrDefault(), 
			_ => throw new ArgumentOutOfRangeException("dependencyVersion"), 
		};
	}
}
