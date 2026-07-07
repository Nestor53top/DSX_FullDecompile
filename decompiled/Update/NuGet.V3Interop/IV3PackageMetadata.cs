namespace NuGet.V3Interop;

internal interface IV3PackageMetadata : IPackageMetadata, IPackageName
{
	PackageTargets PackageTarget { get; }
}
