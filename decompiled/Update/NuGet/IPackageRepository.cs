using System.Linq;

namespace NuGet;

internal interface IPackageRepository
{
	string Source { get; }

	PackageSaveModes PackageSaveMode { get; set; }

	bool SupportsPrereleasePackages { get; }

	IQueryable<IPackage> GetPackages();

	void AddPackage(IPackage package);

	void RemovePackage(IPackage package);
}
