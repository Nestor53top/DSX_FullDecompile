using System;

namespace Squirrel.SimpleSplat;

internal class FuncLogManager : ILogManager
{
	private readonly Func<Type, IFullLogger> _inner;

	public FuncLogManager(Func<Type, IFullLogger> getLogger)
	{
		_inner = getLogger;
	}

	public IFullLogger GetLogger(Type type)
	{
		return _inner(type);
	}
}
