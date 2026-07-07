using System.Collections.Generic;

namespace NuGet.V3Interop;

public interface IV3InteropRepository
{
	IEnumerable<IPackage> FindPackagesById(string packageId);
}
