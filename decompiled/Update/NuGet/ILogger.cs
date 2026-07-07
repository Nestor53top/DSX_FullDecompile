namespace NuGet;

internal interface ILogger : IFileConflictResolver
{
	void Log(MessageLevel level, string message, params object[] args);
}
