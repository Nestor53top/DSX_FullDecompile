namespace NuGet;

internal interface IPackageRepositoryFactory
{
	IPackageRepository CreateRepository(string packageSource);
}
