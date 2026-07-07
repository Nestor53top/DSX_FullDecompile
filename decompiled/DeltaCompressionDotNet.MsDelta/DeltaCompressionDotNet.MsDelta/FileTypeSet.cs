using System;

namespace DeltaCompressionDotNet.MsDelta;

[Flags]
internal enum FileTypeSet : long
{
	Executables = 0xFL
}
