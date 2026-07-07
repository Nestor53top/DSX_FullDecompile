using System.Collections.Generic;

namespace NuGet;

internal interface IPackageOperationResolver
{
	IEnumerable<PackageOperation> ResolveOperations(IPackage package);
}
