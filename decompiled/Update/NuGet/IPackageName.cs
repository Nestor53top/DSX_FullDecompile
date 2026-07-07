namespace NuGet;

internal interface IPackageName
{
	string Id { get; }

	SemanticVersion Version { get; }
}
