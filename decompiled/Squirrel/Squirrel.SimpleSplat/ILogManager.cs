using System;

namespace Squirrel.SimpleSplat;

public interface ILogManager
{
	IFullLogger GetLogger(Type type);
}
