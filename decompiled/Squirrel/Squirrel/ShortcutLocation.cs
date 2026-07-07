using System;

namespace Squirrel;

[Flags]
public enum ShortcutLocation
{
	StartMenu = 1,
	Desktop = 2,
	Startup = 4,
	AppRoot = 8
}
