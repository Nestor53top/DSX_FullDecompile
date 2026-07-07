namespace NuGet.V3Interop;

public interface IV3PackageMetadata : IPackageMetadata, IPackageName
{
	PackageTargets PackageTarget { get; }
}
