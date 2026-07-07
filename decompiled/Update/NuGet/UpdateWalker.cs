using System.Runtime.Versioning;

namespace NuGet;

internal class UpdateWalker : InstallWalker
{
	private readonly IDependentsResolver _dependentsResolver;

	public PackageTargets AcceptedTargets { get; set; }

	internal UpdateWalker(IPackageRepository localRepository, IDependencyResolver2 sourceRepository, IDependentsResolver dependentsResolver, IPackageConstraintProvider constraintProvider, ILogger logger, bool updateDependencies, bool allowPrereleaseVersions)
		: this(localRepository, sourceRepository, dependentsResolver, constraintProvider, null, logger, updateDependencies, allowPrereleaseVersions)
	{
	}

	public UpdateWalker(IPackageRepository localRepository, IDependencyResolver2 sourceRepository, IDependentsResolver dependentsResolver, IPackageConstraintProvider constraintProvider, FrameworkName targetFramework, ILogger logger, bool updateDependencies, bool allowPrereleaseVersions)
		: base(localRepository, sourceRepository, constraintProvider, targetFramework, logger, !updateDependencies, allowPrereleaseVersions, DependencyVersion.Lowest)
	{
		_dependentsResolver = dependentsResolver;
		AcceptedTargets = PackageTargets.All;
	}

	protected override ConflictResult GetConflict(IPackage package)
	{
		ConflictResult conflictResult = base.GetConflict(package);
		if (conflictResult == null)
		{
			IPackage package2 = base.Repository.FindPackage(package.Id);
			if (package2 != null)
			{
				conflictResult = new ConflictResult(package2, base.Repository, _dependentsResolver);
			}
		}
		return conflictResult;
	}

	protected override void OnAfterPackageWalk(IPackage package)
	{
		if (base.DisableWalkInfo)
		{
			base.OnAfterPackageWalk(package);
			return;
		}
		PackageWalkInfo packageInfo = GetPackageInfo(package);
		if (AcceptedTargets.HasFlag(packageInfo.Target))
		{
			base.OnAfterPackageWalk(package);
		}
	}
}
