using System;

namespace ModernWpf.Controls;

[Flags]
public enum ElementRealizationOptions
{
	None = 0,
	ForceCreate = 1,
	SuppressAutoRecycle = 2
}
