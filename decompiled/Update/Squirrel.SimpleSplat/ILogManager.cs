using System;

namespace Squirrel.SimpleSplat;

internal interface ILogManager
{
	IFullLogger GetLogger(Type type);
}
