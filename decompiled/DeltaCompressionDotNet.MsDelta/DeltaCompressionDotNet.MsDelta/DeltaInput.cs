using System;
using System.Runtime.InteropServices;

namespace DeltaCompressionDotNet.MsDelta;

internal struct DeltaInput
{
	public IntPtr Start;

	public IntPtr Size;

	[MarshalAs(UnmanagedType.Bool)]
	public bool Editable;
}
