namespace Squirrel.SimpleSplat;

internal class NullLogger : ILogger
{
	public LogLevel Level { get; set; }

	public void Write(string message, LogLevel logLevel)
	{
	}
}
