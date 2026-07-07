using System.Runtime.Versioning;

namespace NuGet;

internal interface IPackageReferenceRepository : IPackageRepository
{
	void AddPackage(string packageId, SemanticVersion version, bool developmentDependency, FrameworkName targetFramework);

	FrameworkName GetPackageTargetFramework(string packageId);
}
