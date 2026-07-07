using System.Collections.Generic;

namespace NuGet;

internal interface ISharedPackageRepository : IPackageRepository
{
	bool IsReferenced(string packageId, SemanticVersion version);

	bool IsSolutionReferenced(string packageId, SemanticVersion version);

	void RegisterRepository(PackageReferenceFile packageReferenceFile);

	void UnregisterRepository(PackageReferenceFile packageReferenceFile);

	IEnumerable<IPackageRepository> LoadProjectRepositories();
}
