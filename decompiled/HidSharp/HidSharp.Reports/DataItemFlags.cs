using System;

namespace HidSharp.Reports;

[Flags]
public enum DataItemFlags : uint
{
	None = 0u,
	Constant = 1u,
	Variable = 2u,
	Relative = 4u,
	Wrap = 8u,
	Nonlinear = 0x10u,
	NoPreferred = 0x20u,
	NullState = 0x40u,
	Volatile = 0x80u,
	BufferedBytes = 0x100u
}
