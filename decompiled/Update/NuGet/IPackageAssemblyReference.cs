namespace NuGet;

internal interface IPackageAssemblyReference : IPackageFile, IFrameworkTargetable
{
	string Name { get; }
}
