namespace NuGet;

internal interface IFileConflictResolver
{
	FileConflictResolution ResolveFileConflict(string message);
}
