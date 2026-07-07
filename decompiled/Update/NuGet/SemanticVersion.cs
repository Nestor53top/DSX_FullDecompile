using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using NuGet.Resources;

namespace NuGet;

[Serializable]
[TypeConverter(typeof(SemanticVersionTypeConverter))]
internal sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
	private const RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled;

	private static readonly Regex _semanticVersionRegex = new Regex("^(?<Version>\\d+(\\s*\\.\\s*\\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private static readonly Regex _strictSemanticVersionRegex = new Regex("^(?<Version>\\d+(\\.\\d+){2})(?<Release>-[a-z][0-9a-z-]*)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private static readonly Regex _preReleaseVersionRegex = new Regex("(?<PreReleaseString>[a-z]+)(?<PreReleaseNumber>[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private readonly string _originalString;

	public Version Version { get; private set; }

	public string SpecialVersion { get; private set; }

	public SemanticVersion(string version)
		: this(Parse(version))
	{
		_originalString = version;
	}

	public SemanticVersion(int major, int minor, int build, int revision)
		: this(new Version(major, minor, build, revision))
	{
	}

	public SemanticVersion(int major, int minor, int build, string specialVersion)
		: this(new Version(major, minor, build), specialVersion)
	{
	}

	public SemanticVersion(Version version)
		: this(version, string.Empty)
	{
	}

	public SemanticVersion(Version version, string specialVersion)
		: this(version, specialVersion, null)
	{
	}

	private SemanticVersion(Version version, string specialVersion, string originalString)
	{
		if (version == null)
		{
			throw new ArgumentNullException("version");
		}
		Version = NormalizeVersionValue(version);
		SpecialVersion = specialVersion ?? string.Empty;
		_originalString = (string.IsNullOrEmpty(originalString) ? (version.ToString() + ((!string.IsNullOrEmpty(specialVersion)) ? ("-" + specialVersion) : null)) : originalString);
	}

	internal SemanticVersion(SemanticVersion semVer)
	{
		_originalString = semVer.ToString();
		Version = semVer.Version;
		SpecialVersion = semVer.SpecialVersion;
	}

	public string[] GetOriginalVersionComponents()
	{
		if (!string.IsNullOrEmpty(_originalString))
		{
			int num = _originalString.IndexOf('-');
			string version = ((num == -1) ? _originalString : _originalString.Substring(0, num));
			return SplitAndPadVersionString(version);
		}
		return SplitAndPadVersionString(Version.ToString());
	}

	private static string[] SplitAndPadVersionString(string version)
	{
		string[] array = version.Split(new char[1] { '.' });
		if (array.Length == 4)
		{
			return array;
		}
		string[] array2 = new string[4] { "0", "0", "0", "0" };
		Array.Copy(array, 0, array2, 0, array.Length);
		return array2;
	}

	public static SemanticVersion Parse(string version)
	{
		if (string.IsNullOrEmpty(version))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "version");
		}
		if (!TryParse(version, out var value))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidVersionString, new object[1] { version }), "version");
		}
		return value;
	}

	public static bool TryParse(string version, out SemanticVersion value)
	{
		return TryParseInternal(version, _semanticVersionRegex, out value);
	}

	public static bool TryParseStrict(string version, out SemanticVersion value)
	{
		return TryParseInternal(version, _strictSemanticVersionRegex, out value);
	}

	private static bool TryParseInternal(string version, Regex regex, out SemanticVersion semVer)
	{
		semVer = null;
		if (string.IsNullOrEmpty(version))
		{
			return false;
		}
		Match match = regex.Match(version.Trim());
		if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out Version result))
		{
			return false;
		}
		semVer = new SemanticVersion(NormalizeVersionValue(result), match.Groups["Release"].Value.TrimStart(new char[1] { '-' }), version.Replace(" ", ""));
		return true;
	}

	public static SemanticVersion ParseOptionalVersion(string version)
	{
		TryParse(version, out var value);
		return value;
	}

	private static Version NormalizeVersionValue(Version version)
	{
		return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		SemanticVersion semanticVersion = obj as SemanticVersion;
		if (semanticVersion == null)
		{
			throw new ArgumentException(NuGetResources.TypeMustBeASemanticVersion, "obj");
		}
		return CompareTo(semanticVersion);
	}

	public int CompareTo(SemanticVersion other)
	{
		if ((object)other == null)
		{
			return 1;
		}
		int num = Version.CompareTo(other.Version);
		if (num != 0)
		{
			return num;
		}
		bool flag = string.IsNullOrEmpty(SpecialVersion);
		bool flag2 = string.IsNullOrEmpty(other.SpecialVersion);
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		Match match = _preReleaseVersionRegex.Match(SpecialVersion.Trim());
		Match match2 = _preReleaseVersionRegex.Match(other.SpecialVersion.Trim());
		if (match.Success && match2.Success && string.Equals(match.Groups["PreReleaseString"].Value, match2.Groups["PreReleaseString"].Value, StringComparison.OrdinalIgnoreCase))
		{
			int num2 = int.Parse(match.Groups["PreReleaseNumber"].Value) - int.Parse(match2.Groups["PreReleaseNumber"].Value);
			if (num2 == 0)
			{
				return 0;
			}
			return num2 / Math.Abs(num2);
		}
		return StringComparer.OrdinalIgnoreCase.Compare(SpecialVersion, other.SpecialVersion);
	}

	public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
	{
		return version1?.Equals(version2) ?? ((object)version2 == null);
	}

	public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
	{
		return !(version1 == version2);
	}

	public static bool operator <(SemanticVersion version1, SemanticVersion version2)
	{
		if (version1 == null)
		{
			throw new ArgumentNullException("version1");
		}
		return version1.CompareTo(version2) < 0;
	}

	public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
	{
		if (!(version1 == version2))
		{
			return version1 < version2;
		}
		return true;
	}

	public static bool operator >(SemanticVersion version1, SemanticVersion version2)
	{
		if (version1 == null)
		{
			throw new ArgumentNullException("version1");
		}
		return version2 < version1;
	}

	public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
	{
		if (!(version1 == version2))
		{
			return version1 > version2;
		}
		return true;
	}

	public override string ToString()
	{
		return _originalString;
	}

	public bool Equals(SemanticVersion other)
	{
		if ((object)other != null && Version.Equals(other.Version))
		{
			return SpecialVersion.Equals(other.SpecialVersion, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is SemanticVersion other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = Version.GetHashCode();
		if (SpecialVersion != null)
		{
			num = num * 4567 + SpecialVersion.GetHashCode();
		}
		return num;
	}
}
