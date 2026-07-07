using System;

namespace NuGet;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class ManifestVersionAttribute : Attribute
{
	public int Version { get; private set; }

	public ManifestVersionAttribute(int version)
	{
		Version = version;
	}
}
