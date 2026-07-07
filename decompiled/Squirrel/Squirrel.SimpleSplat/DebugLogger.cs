namespace Squirrel.SimpleSplat;

public class DebugLogger : ILogger
{
	public LogLevel Level { get; set; }

	public void Write(string message, LogLevel logLevel)
	{
		_ = Level;
	}
}
