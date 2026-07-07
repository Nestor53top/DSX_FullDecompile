using System;
using System.Collections.Generic;

namespace NuGet;

internal interface IPackageSourceProvider
{
	event EventHandler PackageSourcesSaved;

	IEnumerable<PackageSource> LoadPackageSources();

	void SavePackageSources(IEnumerable<PackageSource> sources);

	void DisablePackageSource(PackageSource source);

	bool IsPackageSourceEnabled(PackageSource source);
}
