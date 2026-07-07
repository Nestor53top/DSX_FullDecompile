using System.Collections.Generic;

namespace NuGet;

internal interface IDependentsResolver
{
	IEnumerable<IPackage> GetDependents(IPackage package);
}
