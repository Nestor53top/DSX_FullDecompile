using System.ComponentModel;

namespace Squirrel.SimpleSplat;

public interface ILogger
{
	LogLevel Level { get; set; }

	void Write([Localizable(false)] string message, LogLevel logLevel);
}
