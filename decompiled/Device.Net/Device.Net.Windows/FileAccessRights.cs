using System;

namespace Device.Net.Windows;

[Flags]
public enum FileAccessRights : uint
{
	None = 0u,
	GenericRead = 0x80000000u,
	GenericWrite = 0x40000000u
}
