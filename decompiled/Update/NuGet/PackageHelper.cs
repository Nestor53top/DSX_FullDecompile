using System;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal static class PackageHelper
{
	public static bool IsManifest(string path)
	{
		return Path.GetExtension(path).Equals(Constants.ManifestExtension, StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsPackageFile(string path)
	{
		return Path.GetExtension(path).Equals(Constants.PackageExtension, StringComparison.OrdinalIgnoreCase);
	}

	public static bool IsAssembly(string path)
	{
		if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".winmd", StringComparison.OrdinalIgnoreCase))
		{
			return path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	public static bool IsSatellitePackage(IPackageMetadata package, IPackageRepository repository, FrameworkName targetFramework, out IPackage runtimePackage)
	{
		runtimePackage = null;
		if (package.IsSatellitePackage())
		{
			string packageId = package.Id.Substring(0, package.Id.Length - (package.Language.Length + 1));
			PackageDependency packageDependency = package.FindDependency(packageId, targetFramework);
			if (packageDependency != null)
			{
				runtimePackage = repository.FindPackage(packageId, packageDependency.VersionSpec, allowPrereleaseVersions: true, allowUnlisted: true);
			}
		}
		return runtimePackage != null;
	}

	public static IPackage ResolvePackage(IPackageRepository repository, string packageId, SemanticVersion version)
	{
		if (repository == null)
		{
			throw new ArgumentNullException("repository");
		}
		if (string.IsNullOrEmpty(packageId))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageId");
		}
		if (version == null)
		{
			throw new ArgumentNullException("version");
		}
		IPackage package = repository.FindPackage(packageId, version);
		if (package == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnknownPackageSpecificVersion, new object[2] { packageId, version }));
		}
		return package;
	}
}
