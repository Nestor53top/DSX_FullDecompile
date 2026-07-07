using System;

namespace Squirrel.SimpleSplat;

internal static class LogHost
{
	internal static bool suppressLogging = false;

	private static readonly IFullLogger nullLogger = new WrappingFullLogger(new NullLogger(), typeof(string));

	public static IFullLogger Default
	{
		get
		{
			if (suppressLogging)
			{
				return nullLogger;
			}
			return (SquirrelLocator.Current.GetService<ILogManager>() ?? throw new Exception("ILogManager is null. This should never happen, your dependency resolver is broken")).GetLogger(typeof(LogHost));
		}
	}

	public static IFullLogger Log<T>(this T This) where T : IEnableLogger
	{
		if (suppressLogging)
		{
			return nullLogger;
		}
		return (SquirrelLocator.Current.GetService<ILogManager>() ?? throw new Exception("ILogManager is null. This should never happen, your dependency resolver is broken")).GetLogger<T>();
	}
}
