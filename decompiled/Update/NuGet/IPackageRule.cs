using System.Collections.Generic;

namespace NuGet;

internal interface IPackageRule
{
	IEnumerable<PackageIssue> Validate(IPackage package);
}
