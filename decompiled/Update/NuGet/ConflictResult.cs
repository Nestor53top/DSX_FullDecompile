namespace NuGet;

internal class ConflictResult
{
	public IPackage Package { get; private set; }

	public IPackageRepository Repository { get; private set; }

	public IDependentsResolver DependentsResolver { get; private set; }

	public ConflictResult(IPackage conflictingPackage, IPackageRepository repository, IDependentsResolver resolver)
	{
		Package = conflictingPackage;
		Repository = repository;
		DependentsResolver = resolver;
	}
}
