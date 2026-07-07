using System;

namespace Squirrel.SimpleSplat;

internal class DefaultLogManager : ILogManager
{
	private readonly MemoizingMRUCache<Type, IFullLogger> loggerCache;

	private static readonly IFullLogger nullLogger = new WrappingFullLogger(new NullLogger(), typeof(MemoizingMRUCache<Type, IFullLogger>));

	public DefaultLogManager(IDependencyResolver dependencyResolver = null)
	{
		dependencyResolver = dependencyResolver ?? SquirrelLocator.Current;
		loggerCache = new MemoizingMRUCache<Type, IFullLogger>((Type type, object _) => new WrappingFullLogger(dependencyResolver.GetService<ILogger>() ?? throw new Exception("Couldn't find an ILogger. This should never happen, your dependency resolver is probably broken."), type), 64);
	}

	public IFullLogger GetLogger(Type type)
	{
		if (LogHost.suppressLogging)
		{
			return nullLogger;
		}
		if (type == typeof(MemoizingMRUCache<Type, IFullLogger>))
		{
			return nullLogger;
		}
		lock (loggerCache)
		{
			return loggerCache.Get(type);
		}
	}
}
