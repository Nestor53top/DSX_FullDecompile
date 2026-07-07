using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class NetPortableProfile : IEquatable<NetPortableProfile>
{
	private string _customProfile;

	public string Name { get; private set; }

	public string FrameworkVersion { get; private set; }

	public ISet<FrameworkName> SupportedFrameworks { get; private set; }

	public ISet<FrameworkName> OptionalFrameworks { get; private set; }

	public string CustomProfileString
	{
		get
		{
			if (_customProfile == null)
			{
				IEnumerable<FrameworkName> source = SupportedFrameworks.Concat(OptionalFrameworks);
				_customProfile = string.Join("+", source.Select((FrameworkName f) => VersionUtility.GetShortFrameworkName(f, null)));
			}
			return _customProfile;
		}
	}

	public NetPortableProfile(string name, IEnumerable<FrameworkName> supportedFrameworks, IEnumerable<FrameworkName> optionalFrameworks = null)
		: this("v0.0", name, supportedFrameworks, optionalFrameworks)
	{
	}

	public NetPortableProfile(string version, string name, IEnumerable<FrameworkName> supportedFrameworks, IEnumerable<FrameworkName> optionalFrameworks)
	{
		if (string.IsNullOrEmpty(version))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "version");
		}
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "name");
		}
		if (supportedFrameworks == null)
		{
			throw new ArgumentNullException("supportedFrameworks");
		}
		List<FrameworkName> list = supportedFrameworks.ToList();
		if (list.Any((FrameworkName f) => f == null))
		{
			throw new ArgumentException(NuGetResources.SupportedFrameworkIsNull, "supportedFrameworks");
		}
		if (list.Count == 0)
		{
			throw new ArgumentOutOfRangeException("supportedFrameworks");
		}
		Name = name;
		SupportedFrameworks = new ReadOnlyHashSet<FrameworkName>(list);
		OptionalFrameworks = ((optionalFrameworks == null || optionalFrameworks.IsEmpty()) ? new ReadOnlyHashSet<FrameworkName>(Enumerable.Empty<FrameworkName>()) : new ReadOnlyHashSet<FrameworkName>(optionalFrameworks));
		FrameworkVersion = version;
	}

	public bool Equals(NetPortableProfile other)
	{
		if (Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) && SupportedFrameworks.SetEquals(other.SupportedFrameworks))
		{
			return OptionalFrameworks.SetEquals(other.OptionalFrameworks);
		}
		return false;
	}

	public override int GetHashCode()
	{
		HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
		hashCodeCombiner.AddObject(Name);
		hashCodeCombiner.AddObject(SupportedFrameworks);
		hashCodeCombiner.AddObject(OptionalFrameworks);
		return hashCodeCombiner.CombinedHash;
	}

	public bool IsCompatibleWith(NetPortableProfile projectFrameworkProfile)
	{
		return IsCompatibleWith(projectFrameworkProfile, NetPortableProfileTable.Default);
	}

	public bool IsCompatibleWith(NetPortableProfile projectFrameworkProfile, NetPortableProfileTable portableProfileTable)
	{
		if (projectFrameworkProfile == null)
		{
			throw new ArgumentNullException("projectFrameworkProfile");
		}
		return projectFrameworkProfile.SupportedFrameworks.All((FrameworkName projectFramework) => SupportedFrameworks.Any((FrameworkName packageFramework) => VersionUtility.IsCompatible(projectFramework, packageFramework, portableProfileTable)));
	}

	public bool IsCompatibleWith(FrameworkName projectFramework)
	{
		return IsCompatibleWith(projectFramework, NetPortableProfileTable.Default);
	}

	public bool IsCompatibleWith(FrameworkName projectFramework, NetPortableProfileTable portableProfileTable)
	{
		if (projectFramework == null)
		{
			throw new ArgumentNullException("projectFramework");
		}
		if (!SupportedFrameworks.Any((FrameworkName packageFramework) => VersionUtility.IsCompatible(projectFramework, packageFramework, portableProfileTable)))
		{
			return portableProfileTable.HasCompatibleProfileWith(this, projectFramework, portableProfileTable);
		}
		return true;
	}

	public static NetPortableProfile Parse(string profileValue, bool treatOptionalFrameworksAsSupportedFrameworks = false, NetPortableProfileTable portableProfileTable = null)
	{
		portableProfileTable = portableProfileTable ?? NetPortableProfileTable.Default;
		if (string.IsNullOrEmpty(profileValue))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "profileValue");
		}
		NetPortableProfile netPortableProfile = portableProfileTable.GetProfile(profileValue);
		if (netPortableProfile != null)
		{
			if (treatOptionalFrameworksAsSupportedFrameworks)
			{
				netPortableProfile = new NetPortableProfile(netPortableProfile.Name, netPortableProfile.SupportedFrameworks.Concat(netPortableProfile.OptionalFrameworks));
			}
			return netPortableProfile;
		}
		if (profileValue.StartsWith("Profile", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		VersionUtility.ValidatePortableFrameworkProfilePart(profileValue);
		IEnumerable<FrameworkName> supportedFrameworks = profileValue.Split(new char[1] { '+' }, StringSplitOptions.RemoveEmptyEntries).Select(VersionUtility.ParseFrameworkName);
		return new NetPortableProfile(profileValue, supportedFrameworks);
	}
}
