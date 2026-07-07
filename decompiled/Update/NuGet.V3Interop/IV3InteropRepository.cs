using System.Collections.Generic;

namespace NuGet.V3Interop;

internal interface IV3InteropRepository
{
	IEnumerable<IPackage> FindPackagesById(string packageId);
}
