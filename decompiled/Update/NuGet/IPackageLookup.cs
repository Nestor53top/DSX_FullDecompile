using System.Collections.Generic;

namespace NuGet;

internal interface IPackageLookup : IPackageRepository
{
	bool Exists(string packageId, SemanticVersion version);

	IPackage FindPackage(string packageId, SemanticVersion version);

	IEnumerable<IPackage> FindPackagesById(string packageId);
}
