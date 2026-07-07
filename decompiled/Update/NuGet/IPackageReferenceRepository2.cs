using System.Collections.Generic;

namespace NuGet;

internal interface IPackageReferenceRepository2 : IPackageRepository
{
	PackageReference GetPackageReference(string packageId);

	IEnumerable<PackageReference> GetPackageReferences(string packageId);
}
