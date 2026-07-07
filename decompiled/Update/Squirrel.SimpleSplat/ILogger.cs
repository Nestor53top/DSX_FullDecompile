using System.ComponentModel;

namespace Squirrel.SimpleSplat;

internal interface ILogger
{
	LogLevel Level { get; set; }

	void Write([Localizable(false)] string message, LogLevel logLevel);
}
