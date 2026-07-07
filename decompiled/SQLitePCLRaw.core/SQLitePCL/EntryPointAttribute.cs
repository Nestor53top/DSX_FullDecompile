using System;

namespace SQLitePCL;

public sealed class EntryPointAttribute : Attribute
{
	public string Name { get; private set; }

	public EntryPointAttribute(string name)
	{
		Name = name;
	}
}
