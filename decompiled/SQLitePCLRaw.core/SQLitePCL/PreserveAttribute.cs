using System;

namespace SQLitePCL;

public sealed class PreserveAttribute : Attribute
{
	public bool AllMembers;

	public bool Conditional;
}
