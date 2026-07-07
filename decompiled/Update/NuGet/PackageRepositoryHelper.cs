using System;
using System.Globalization;
using NuGet.Resources;

namespace NuGet;

internal static class PackageRepositoryHelper
{
	public static IPackage ResolvePackage(IPackageRepository sourceRepository, IPackageRepository localRepository, string packageId, SemanticVersion version, bool allowPrereleaseVersions)
	{
		return ResolvePackage(sourceRepository, localRepository, NullConstraintProvider.Instance, packageId, version, allowPrereleaseVersions);
	}

	public static IPackage ResolvePackage(IPackageRepository sourceRepository, IPackageRepository localRepository, IPackageConstraintProvider constraintProvider, string packageId, SemanticVersion version, bool allowPrereleaseVersions)
	{
		if (string.IsNullOrEmpty(packageId))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageId");
		}
		IPackage package = null;
		if (version != null)
		{
			package = localRepository.FindPackage(packageId, version, allowPrereleaseVersions, allowUnlisted: true);
		}
		if (package == null)
		{
			package = sourceRepository.FindPackage(packageId, version, constraintProvider, allowPrereleaseVersions, allowUnlisted: false);
			if (package != null)
			{
				package = localRepository.FindPackage(package.Id, package.Version, allowPrereleaseVersions, allowUnlisted: true) ?? package;
			}
		}
		if (package == null)
		{
			if (version != null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnknownPackageSpecificVersion, new object[2] { packageId, version }));
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnknownPackage, new object[1] { packageId }));
		}
		return package;
	}
}
