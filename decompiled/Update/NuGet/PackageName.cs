using System;

namespace NuGet;

internal class PackageName : IPackageName, IEquatable<PackageName>
{
	private readonly string _packageId;

	private readonly SemanticVersion _version;

	public string Id => _packageId;

	public SemanticVersion Version => _version;

	public string Name => _packageId + "." + _version.ToString();

	public PackageName(string packageId, SemanticVersion version)
	{
		_packageId = packageId;
		_version = version;
	}

	public bool Equals(PackageName other)
	{
		if (_packageId.Equals(other._packageId, StringComparison.OrdinalIgnoreCase))
		{
			return _version.Equals(other._version);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _packageId.GetHashCode() * 3137 + _version.GetHashCode();
	}

	public override string ToString()
	{
		return _packageId + " " + _version;
	}
}
