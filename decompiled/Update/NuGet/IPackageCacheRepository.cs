using System;
using System.IO;

namespace NuGet;

internal interface IPackageCacheRepository : IPackageRepository
{
	bool InvokeOnPackage(string packageId, SemanticVersion version, Action<Stream> action);
}
