using System;
using System.Runtime.Versioning;

namespace NuGet;

internal class PackageReference : IEquatable<PackageReference>
{
	public string Id { get; private set; }

	public SemanticVersion Version { get; private set; }

	public IVersionSpec VersionConstraint { get; set; }

	public FrameworkName TargetFramework { get; private set; }

	public bool IsDevelopmentDependency { get; private set; }

	public bool RequireReinstallation { get; private set; }

	public PackageReference(string id, SemanticVersion version, IVersionSpec versionConstraint, FrameworkName targetFramework, bool isDevelopmentDependency, bool requireReinstallation = false)
	{
		Id = id;
		Version = version;
		VersionConstraint = versionConstraint;
		TargetFramework = targetFramework;
		IsDevelopmentDependency = isDevelopmentDependency;
		RequireReinstallation = requireReinstallation;
	}

	public override bool Equals(object obj)
	{
		if (obj is PackageReference other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode() * 3137 + ((!(Version == null)) ? Version.GetHashCode() : 0);
	}

	public override string ToString()
	{
		if (Version == null)
		{
			return Id;
		}
		if (VersionConstraint == null)
		{
			return Id + " " + Version;
		}
		return Id + " " + Version?.ToString() + " (" + VersionConstraint?.ToString() + ")";
	}

	public bool Equals(PackageReference other)
	{
		if (Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
		{
			return Version == other.Version;
		}
		return false;
	}
}
