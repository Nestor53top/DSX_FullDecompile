namespace NuGet;

public interface IPackageAssemblyReference : IPackageFile, IFrameworkTargetable
{
	string Name { get; }
}
