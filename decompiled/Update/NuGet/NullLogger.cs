namespace NuGet;

internal class NullLogger : ILogger, IFileConflictResolver
{
	private static readonly ILogger _instance = new NullLogger();

	public static ILogger Instance => _instance;

	public void Log(MessageLevel level, string message, params object[] args)
	{
	}

	public FileConflictResolution ResolveFileConflict(string message)
	{
		return FileConflictResolution.Ignore;
	}
}
