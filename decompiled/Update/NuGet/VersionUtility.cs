using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using NuGet.Resources;

namespace NuGet;

internal static class VersionUtility
{
	private const string NetFrameworkIdentifier = ".NETFramework";

	private const string NetCoreFrameworkIdentifier = ".NETCore";

	private const string PortableFrameworkIdentifier = ".NETPortable";

	private const string AspNetFrameworkIdentifier = "ASP.NET";

	private const string AspNetCoreFrameworkIdentifier = "ASP.NETCore";

	private const string LessThanOrEqualTo = "≤";

	private const string GreaterThanOrEqualTo = "≥";

	public static readonly FrameworkName EmptyFramework = new FrameworkName("NoFramework", new Version());

	public static readonly FrameworkName NativeProjectFramework = new FrameworkName("Native", new Version());

	public static readonly FrameworkName UnsupportedFrameworkName = new FrameworkName("Unsupported", new Version());

	private static readonly Version _emptyVersion = new Version();

	private static readonly Dictionary<string, string> _knownIdentifiers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "NET", ".NETFramework" },
		{ ".NET", ".NETFramework" },
		{ "NETFramework", ".NETFramework" },
		{ ".NETFramework", ".NETFramework" },
		{ "NETCore", ".NETCore" },
		{ ".NETCore", ".NETCore" },
		{ "WinRT", ".NETCore" },
		{ ".NETMicroFramework", ".NETMicroFramework" },
		{ "netmf", ".NETMicroFramework" },
		{ "SL", "Silverlight" },
		{ "Silverlight", "Silverlight" },
		{ ".NETPortable", ".NETPortable" },
		{ "NETPortable", ".NETPortable" },
		{ "portable", ".NETPortable" },
		{ "wp", "WindowsPhone" },
		{ "WindowsPhone", "WindowsPhone" },
		{ "WindowsPhoneApp", "WindowsPhoneApp" },
		{ "wpa", "WindowsPhoneApp" },
		{ "Windows", "Windows" },
		{ "win", "Windows" },
		{ "aspnet", "ASP.NET" },
		{ "aspnetcore", "ASP.NETCore" },
		{ "asp.net", "ASP.NET" },
		{ "asp.netcore", "ASP.NETCore" },
		{ "native", "native" },
		{ "MonoAndroid", "MonoAndroid" },
		{ "MonoTouch", "MonoTouch" },
		{ "MonoMac", "MonoMac" },
		{ "Xamarin.iOS", "Xamarin.iOS" },
		{ "XamariniOS", "Xamarin.iOS" },
		{ "Xamarin.Mac", "Xamarin.Mac" },
		{ "XamarinMac", "Xamarin.Mac" },
		{ "Xamarin.PlayStationThree", "Xamarin.PlayStation3" },
		{ "XamarinPlayStationThree", "Xamarin.PlayStation3" },
		{ "XamarinPSThree", "Xamarin.PlayStation3" },
		{ "Xamarin.PlayStationFour", "Xamarin.PlayStation4" },
		{ "XamarinPlayStationFour", "Xamarin.PlayStation4" },
		{ "XamarinPSFour", "Xamarin.PlayStation4" },
		{ "Xamarin.PlayStationVita", "Xamarin.PlayStationVita" },
		{ "XamarinPlayStationVita", "Xamarin.PlayStationVita" },
		{ "XamarinPSVita", "Xamarin.PlayStationVita" },
		{ "Xamarin.XboxThreeSixty", "Xamarin.Xbox360" },
		{ "XamarinXboxThreeSixty", "Xamarin.Xbox360" },
		{ "Xamarin.XboxOne", "Xamarin.XboxOne" },
		{ "XamarinXboxOne", "Xamarin.XboxOne" }
	};

	private static readonly Dictionary<string, string> _knownProfiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "Client", "Client" },
		{ "WP", "WindowsPhone" },
		{ "WP71", "WindowsPhone71" },
		{ "CF", "CompactFramework" },
		{
			"Full",
			string.Empty
		}
	};

	private static readonly Dictionary<string, string> _identifierToFrameworkFolder = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ ".NETFramework", "net" },
		{ ".NETMicroFramework", "netmf" },
		{ "ASP.NET", "aspnet" },
		{ "ASP.NETCore", "aspnetcore" },
		{ "Silverlight", "sl" },
		{ ".NETCore", "win" },
		{ "Windows", "win" },
		{ ".NETPortable", "portable" },
		{ "WindowsPhone", "wp" },
		{ "WindowsPhoneApp", "wpa" },
		{ "Xamarin.iOS", "xamarinios" },
		{ "Xamarin.Mac", "xamarinmac" },
		{ "Xamarin.PlayStation3", "xamarinpsthree" },
		{ "Xamarin.PlayStation4", "xamarinpsfour" },
		{ "Xamarin.PlayStationVita", "xamarinpsvita" },
		{ "Xamarin.Xbox360", "xamarinxboxthreesixty" },
		{ "Xamarin.XboxOne", "xamarinxboxone" }
	};

	private static readonly Dictionary<string, string> _identifierToProfileFolder = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "WindowsPhone", "wp" },
		{ "WindowsPhone71", "wp71" },
		{ "CompactFramework", "cf" }
	};

	private static readonly Dictionary<string, Dictionary<string, string[]>> _compatibiltyMapping = new Dictionary<string, Dictionary<string, string[]>>(StringComparer.OrdinalIgnoreCase)
	{
		{
			".NETFramework",
			new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"",
					new string[1] { "Client" }
				},
				{
					"Client",
					new string[1] { "" }
				}
			}
		},
		{
			"Silverlight",
			new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"WindowsPhone",
					new string[1] { "WindowsPhone71" }
				},
				{
					"WindowsPhone71",
					new string[1] { "WindowsPhone" }
				}
			}
		}
	};

	private static readonly Dictionary<FrameworkName, FrameworkName> _frameworkNameAlias = new Dictionary<FrameworkName, FrameworkName>(FrameworkNameEqualityComparer.Default)
	{
		{
			new FrameworkName("WindowsPhone, Version=v0.0"),
			new FrameworkName("Silverlight, Version=v3.0, Profile=WindowsPhone")
		},
		{
			new FrameworkName("WindowsPhone, Version=v7.0"),
			new FrameworkName("Silverlight, Version=v3.0, Profile=WindowsPhone")
		},
		{
			new FrameworkName("WindowsPhone, Version=v7.1"),
			new FrameworkName("Silverlight, Version=v4.0, Profile=WindowsPhone71")
		},
		{
			new FrameworkName("WindowsPhone, Version=v8.0"),
			new FrameworkName("Silverlight, Version=v8.0, Profile=WindowsPhone")
		},
		{
			new FrameworkName("WindowsPhone, Version=v8.1"),
			new FrameworkName("Silverlight, Version=v8.1, Profile=WindowsPhone")
		},
		{
			new FrameworkName("Windows, Version=v0.0"),
			new FrameworkName(".NETCore, Version=v4.5")
		},
		{
			new FrameworkName("Windows, Version=v8.0"),
			new FrameworkName(".NETCore, Version=v4.5")
		},
		{
			new FrameworkName("Windows, Version=v8.1"),
			new FrameworkName(".NETCore, Version=v4.5.1")
		}
	};

	private static readonly Version MaxVersion = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

	private static readonly Dictionary<string, FrameworkName> _equivalentProjectFrameworks = new Dictionary<string, FrameworkName> { 
	{
		"ASP.NET",
		new FrameworkName(".NETFramework", MaxVersion)
	} };

	public static Version DefaultTargetFrameworkVersion => typeof(string).Assembly.GetName().Version;

	public static FrameworkName DefaultTargetFramework => new FrameworkName(".NETFramework", DefaultTargetFrameworkVersion);

	public static FrameworkName ParseFrameworkName(string frameworkName)
	{
		if (frameworkName == null)
		{
			throw new ArgumentNullException("frameworkName");
		}
		string text = null;
		string text2 = null;
		string[] array = frameworkName.Split(new char[1] { '-' });
		if (array.Length > 2)
		{
			throw new ArgumentException(NuGetResources.InvalidFrameworkNameFormat, "frameworkName");
		}
		string text3 = ((array.Length != 0) ? array[0].Trim() : null);
		string text4 = ((array.Length > 1) ? array[1].Trim() : null);
		if (string.IsNullOrEmpty(text3))
		{
			throw new ArgumentException(NuGetResources.MissingFrameworkName, "frameworkName");
		}
		Match match = Regex.Match(text3, "\\d+");
		if (match.Success)
		{
			text = text3.Substring(0, match.Index).Trim();
			text2 = text3.Substring(match.Index).Trim();
		}
		else
		{
			text = text3.Trim();
		}
		if (!string.IsNullOrEmpty(text) && !_knownIdentifiers.TryGetValue(text, out text))
		{
			return UnsupportedFrameworkName;
		}
		if (!string.IsNullOrEmpty(text4) && _knownProfiles.TryGetValue(text4, out var value))
		{
			text4 = value;
		}
		Version result = null;
		if (int.TryParse(text2, out var _))
		{
			if (text2.Length > 4)
			{
				text2 = text2.Substring(0, 4);
			}
			text2 = text2.PadRight(2, '0');
			text2 = string.Join(".", text2.ToCharArray());
		}
		if (!Version.TryParse(text2, out result))
		{
			if (string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				return UnsupportedFrameworkName;
			}
			result = _emptyVersion;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = ".NETFramework";
		}
		if (text.Equals(".NETPortable", StringComparison.OrdinalIgnoreCase))
		{
			ValidatePortableFrameworkProfilePart(text4);
		}
		return new FrameworkName(text, result, text4);
	}

	internal static void ValidatePortableFrameworkProfilePart(string profilePart)
	{
		if (string.IsNullOrEmpty(profilePart))
		{
			throw new ArgumentException(NuGetResources.PortableFrameworkProfileEmpty, "profilePart");
		}
		if (Enumerable.Contains(profilePart, '-'))
		{
			throw new ArgumentException(NuGetResources.PortableFrameworkProfileHasDash, "profilePart");
		}
		if (Enumerable.Contains(profilePart, ' '))
		{
			throw new ArgumentException(NuGetResources.PortableFrameworkProfileHasSpace, "profilePart");
		}
		string[] source = profilePart.Split(new char[1] { '+' });
		if (source.Any((string p) => string.IsNullOrEmpty(p)))
		{
			throw new ArgumentException(NuGetResources.PortableFrameworkProfileComponentIsEmpty, "profilePart");
		}
		if (source.Any((string p) => p.StartsWith("portable", StringComparison.OrdinalIgnoreCase)) || source.Any((string p) => p.StartsWith("NETPortable", StringComparison.OrdinalIgnoreCase)) || source.Any((string p) => p.StartsWith(".NETPortable", StringComparison.OrdinalIgnoreCase)))
		{
			throw new ArgumentException(NuGetResources.PortableFrameworkProfileComponentIsPortable, "profilePart");
		}
	}

	public static Version TrimVersion(Version version)
	{
		if (version == null)
		{
			throw new ArgumentNullException("version");
		}
		if (version.Build == 0 && version.Revision == 0)
		{
			version = new Version(version.Major, version.Minor);
		}
		else if (version.Revision == 0)
		{
			version = new Version(version.Major, version.Minor, version.Build);
		}
		return version;
	}

	public static IVersionSpec ParseVersionSpec(string value)
	{
		if (!TryParseVersionSpec(value, out var result))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidVersionString, new object[1] { value }));
		}
		return result;
	}

	public static bool TryParseVersionSpec(string value, out IVersionSpec result)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		VersionSpec versionSpec = new VersionSpec();
		value = value.Trim();
		if (SemanticVersion.TryParse(value, out var value2))
		{
			result = new VersionSpec
			{
				MinVersion = value2,
				IsMinInclusive = true
			};
			return true;
		}
		result = null;
		if (value.Length < 3)
		{
			return false;
		}
		switch (value.First())
		{
		case '[':
			versionSpec.IsMinInclusive = true;
			break;
		case '(':
			versionSpec.IsMinInclusive = false;
			break;
		default:
			return false;
		}
		switch (value.Last())
		{
		case ']':
			versionSpec.IsMaxInclusive = true;
			break;
		case ')':
			versionSpec.IsMaxInclusive = false;
			break;
		default:
			return false;
		}
		value = value.Substring(1, value.Length - 2);
		string[] array = value.Split(new char[1] { ',' });
		if (array.Length > 2)
		{
			return false;
		}
		if (array.All(string.IsNullOrEmpty))
		{
			return false;
		}
		string text = array[0];
		string text2 = ((array.Length == 2) ? array[1] : array[0]);
		if (!string.IsNullOrWhiteSpace(text))
		{
			if (!TryParseVersion(text, out value2))
			{
				return false;
			}
			versionSpec.MinVersion = value2;
		}
		if (!string.IsNullOrWhiteSpace(text2))
		{
			if (!TryParseVersion(text2, out value2))
			{
				return false;
			}
			versionSpec.MaxVersion = value2;
		}
		result = versionSpec;
		return true;
	}

	public static IVersionSpec GetSafeRange(SemanticVersion version)
	{
		return new VersionSpec
		{
			IsMinInclusive = true,
			MinVersion = version,
			MaxVersion = new SemanticVersion(new Version(version.Version.Major, version.Version.Minor + 1))
		};
	}

	public static string PrettyPrint(IVersionSpec versionSpec)
	{
		if (versionSpec.MinVersion != null && versionSpec.IsMinInclusive && versionSpec.MaxVersion == null && !versionSpec.IsMaxInclusive)
		{
			return string.Format(CultureInfo.InvariantCulture, "({0} {1})", new object[2] { "≥", versionSpec.MinVersion });
		}
		if (versionSpec.MinVersion != null && versionSpec.MaxVersion != null && versionSpec.MinVersion == versionSpec.MaxVersion && versionSpec.IsMinInclusive && versionSpec.IsMaxInclusive)
		{
			return string.Format(CultureInfo.InvariantCulture, "(= {0})", new object[1] { versionSpec.MinVersion });
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (versionSpec.MinVersion != null)
		{
			if (versionSpec.IsMinInclusive)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "({0} ", new object[1] { "≥" });
			}
			else
			{
				stringBuilder.Append("(> ");
			}
			stringBuilder.Append(versionSpec.MinVersion);
		}
		if (versionSpec.MaxVersion != null)
		{
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("(");
			}
			else
			{
				stringBuilder.Append(" && ");
			}
			if (versionSpec.IsMaxInclusive)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", new object[1] { "≤" });
			}
			else
			{
				stringBuilder.Append("< ");
			}
			stringBuilder.Append(versionSpec.MaxVersion);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Append(")");
		}
		return stringBuilder.ToString();
	}

	public static string GetFrameworkString(FrameworkName frameworkName)
	{
		string text = frameworkName.Identifier + frameworkName.Version;
		if (string.IsNullOrEmpty(frameworkName.Profile))
		{
			return text;
		}
		return text + "-" + frameworkName.Profile;
	}

	public static string GetShortFrameworkName(FrameworkName frameworkName)
	{
		return GetShortFrameworkName(frameworkName, NetPortableProfileTable.Default);
	}

	public static string GetShortFrameworkName(FrameworkName frameworkName, NetPortableProfileTable portableProfileTable)
	{
		if (frameworkName == null)
		{
			throw new ArgumentNullException("frameworkName");
		}
		foreach (KeyValuePair<FrameworkName, FrameworkName> item in _frameworkNameAlias)
		{
			if (FrameworkNameEqualityComparer.Default.Equals(item.Value, frameworkName))
			{
				frameworkName = item.Key;
				break;
			}
		}
		if (!_identifierToFrameworkFolder.TryGetValue(frameworkName.Identifier, out var value))
		{
			value = frameworkName.Identifier;
		}
		string value2;
		if (value.Equals("portable", StringComparison.OrdinalIgnoreCase))
		{
			if (portableProfileTable == null)
			{
				throw new ArgumentException(NuGetResources.PortableProfileTableMustBeSpecified, "portableProfileTable");
			}
			NetPortableProfile netPortableProfile = NetPortableProfile.Parse(frameworkName.Profile, treatOptionalFrameworksAsSupportedFrameworks: false, portableProfileTable);
			value2 = ((netPortableProfile == null) ? frameworkName.Profile : netPortableProfile.CustomProfileString);
		}
		else
		{
			if (frameworkName.Version > new Version())
			{
				value += frameworkName.Version.ToString().Replace(".", string.Empty);
			}
			if (string.IsNullOrEmpty(frameworkName.Profile))
			{
				return value;
			}
			if (!_identifierToProfileFolder.TryGetValue(frameworkName.Profile, out value2))
			{
				value2 = frameworkName.Profile;
			}
		}
		return value + "-" + value2;
	}

	public static string GetTargetFrameworkLogString(FrameworkName targetFramework)
	{
		if (!(targetFramework == null) && !(targetFramework == EmptyFramework))
		{
			return string.Empty;
		}
		return NuGetResources.Debug_TargetFrameworkInfo_NotFrameworkSpecific;
	}

	public static FrameworkName ParseFrameworkNameFromFilePath(string filePath, out string effectivePath)
	{
		string[] array = new string[4]
		{
			Constants.ContentDirectory,
			Constants.LibDirectory,
			Constants.ToolsDirectory,
			Constants.BuildDirectory
		};
		for (int i = 0; i < array.Length; i++)
		{
			string obj = array[i];
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string text = obj + directorySeparatorChar;
			if (filePath.Length > text.Length && filePath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				string text2 = filePath.Substring(text.Length);
				try
				{
					return ParseFrameworkFolderName(text2, array[i] == Constants.LibDirectory, out effectivePath);
				}
				catch (ArgumentException)
				{
					effectivePath = text2;
					return null;
				}
			}
		}
		effectivePath = filePath;
		return null;
	}

	public static FrameworkName ParseFrameworkFolderName(string path)
	{
		string effectivePath;
		return ParseFrameworkFolderName(path, strictParsing: true, out effectivePath);
	}

	public static FrameworkName ParseFrameworkFolderName(string path, bool strictParsing, out string effectivePath)
	{
		string text = Path.GetDirectoryName(path).Split(new char[1] { Path.DirectorySeparatorChar }).First();
		effectivePath = path;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		FrameworkName frameworkName = ParseFrameworkName(text);
		if (strictParsing || frameworkName != UnsupportedFrameworkName)
		{
			effectivePath = path.Substring(text.Length + 1);
			return frameworkName;
		}
		return null;
	}

	public static bool TryGetCompatibleItems<T>(FrameworkName projectFramework, IEnumerable<T> items, out IEnumerable<T> compatibleItems) where T : IFrameworkTargetable
	{
		return TryGetCompatibleItems(projectFramework, items, NetPortableProfileTable.Default, out compatibleItems);
	}

	public static bool TryGetCompatibleItems<T>(FrameworkName projectFramework, IEnumerable<T> items, NetPortableProfileTable portableProfileTable, out IEnumerable<T> compatibleItems) where T : IFrameworkTargetable
	{
		if (!items.Any())
		{
			compatibleItems = Enumerable.Empty<T>();
			return true;
		}
		FrameworkName internalProjectFramework = projectFramework ?? EmptyFramework;
		List<IGrouping<FrameworkName, T>> source = (from _003C_003Eh__TransparentIdentifier0 in items.Select(delegate(T item)
			{
				T val = item;
				IEnumerable<FrameworkName> frameworks;
				if (val.SupportedFrameworks != null)
				{
					val = item;
					if (val.SupportedFrameworks.Any())
					{
						val = item;
						frameworks = val.SupportedFrameworks;
						goto IL_0041;
					}
				}
				IEnumerable<FrameworkName> enumerable = new FrameworkName[1];
				frameworks = enumerable;
				goto IL_0041;
				IL_0041:
				return new { item, frameworks };
			})
			from framework in _003C_003Eh__TransparentIdentifier0.frameworks
			select new
			{
				Item = _003C_003Eh__TransparentIdentifier0.item,
				TargetFramework = framework
			} into g
			group g.Item by g.TargetFramework).ToList();
		compatibleItems = (from g in source
			where g.Key != null && IsCompatible(internalProjectFramework, g.Key, portableProfileTable)
			orderby GetProfileCompatibility(internalProjectFramework, g.Key, portableProfileTable) descending
			select g).FirstOrDefault();
		bool flag = compatibleItems != null && compatibleItems.Any();
		if (!flag)
		{
			compatibleItems = source.Where((IGrouping<FrameworkName, T> g) => g.Key == null).SelectMany((IGrouping<FrameworkName, T> g) => g);
			flag = compatibleItems != null && compatibleItems.Any();
		}
		if (!flag)
		{
			compatibleItems = null;
		}
		return flag;
	}

	internal static Version NormalizeVersion(Version version)
	{
		return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
	}

	public static FrameworkName NormalizeFrameworkName(FrameworkName framework)
	{
		if (_frameworkNameAlias.TryGetValue(framework, out var value))
		{
			return value;
		}
		return framework;
	}

	public static IEnumerable<SemanticVersion> GetPossibleVersions(SemanticVersion semver)
	{
		Version version = TrimVersion(semver.Version);
		yield return new SemanticVersion(version, semver.SpecialVersion);
		if (version.Build == -1 && version.Revision == -1)
		{
			yield return new SemanticVersion(new Version(version.Major, version.Minor, 0), semver.SpecialVersion);
			yield return new SemanticVersion(new Version(version.Major, version.Minor, 0, 0), semver.SpecialVersion);
		}
		else if (version.Revision == -1)
		{
			yield return new SemanticVersion(new Version(version.Major, version.Minor, version.Build, 0), semver.SpecialVersion);
		}
	}

	public static bool IsCompatible(FrameworkName projectFrameworkName, IEnumerable<FrameworkName> packageSupportedFrameworks)
	{
		return IsCompatible(projectFrameworkName, packageSupportedFrameworks, NetPortableProfileTable.Default);
	}

	public static bool IsCompatible(FrameworkName projectFrameworkName, IEnumerable<FrameworkName> packageSupportedFrameworks, NetPortableProfileTable portableProfileTable)
	{
		if (packageSupportedFrameworks.Any())
		{
			return packageSupportedFrameworks.Any((FrameworkName packageSupportedFramework) => IsCompatible(projectFrameworkName, packageSupportedFramework, portableProfileTable));
		}
		return true;
	}

	internal static bool IsCompatible(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName)
	{
		return IsCompatible(projectFrameworkName, packageTargetFrameworkName, NetPortableProfileTable.Default);
	}

	internal static bool IsCompatible(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName, NetPortableProfileTable portableProfileTable)
	{
		if (projectFrameworkName == null)
		{
			return true;
		}
		if (packageTargetFrameworkName.IsPortableFramework())
		{
			return IsPortableLibraryCompatible(projectFrameworkName, packageTargetFrameworkName, portableProfileTable);
		}
		packageTargetFrameworkName = NormalizeFrameworkName(packageTargetFrameworkName);
		projectFrameworkName = NormalizeFrameworkName(projectFrameworkName);
		if (!projectFrameworkName.Identifier.Equals(packageTargetFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase))
		{
			if (!_equivalentProjectFrameworks.TryGetValue(projectFrameworkName.Identifier, out var value) || !value.Identifier.Equals(packageTargetFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			projectFrameworkName = value;
		}
		if (NormalizeVersion(projectFrameworkName.Version) < NormalizeVersion(packageTargetFrameworkName.Version))
		{
			return false;
		}
		if (string.Equals(projectFrameworkName.Profile, packageTargetFrameworkName.Profile, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (_compatibiltyMapping.TryGetValue(projectFrameworkName.Identifier, out var value2) && value2.TryGetValue(packageTargetFrameworkName.Profile, out var value3))
		{
			return Enumerable.Contains<string>(value3, projectFrameworkName.Profile, StringComparer.OrdinalIgnoreCase);
		}
		return false;
	}

	private static bool IsPortableLibraryCompatible(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName, NetPortableProfileTable portableProfileTable)
	{
		if (string.IsNullOrEmpty(packageTargetFrameworkName.Profile))
		{
			return false;
		}
		NetPortableProfile netPortableProfile = NetPortableProfile.Parse(packageTargetFrameworkName.Profile, treatOptionalFrameworksAsSupportedFrameworks: false, portableProfileTable);
		if (netPortableProfile == null)
		{
			return false;
		}
		if (projectFrameworkName.IsPortableFramework())
		{
			if (string.Equals(projectFrameworkName.Profile, packageTargetFrameworkName.Profile, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			NetPortableProfile netPortableProfile2 = NetPortableProfile.Parse(projectFrameworkName.Profile, treatOptionalFrameworksAsSupportedFrameworks: false, portableProfileTable);
			if (netPortableProfile2 == null)
			{
				return false;
			}
			return netPortableProfile.IsCompatibleWith(netPortableProfile2, portableProfileTable);
		}
		return netPortableProfile.IsCompatibleWith(projectFrameworkName);
	}

	private static long GetProfileCompatibility(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName, NetPortableProfileTable portableProfileTable)
	{
		projectFrameworkName = NormalizeFrameworkName(projectFrameworkName);
		packageTargetFrameworkName = NormalizeFrameworkName(packageTargetFrameworkName);
		if (packageTargetFrameworkName.IsPortableFramework())
		{
			if (projectFrameworkName.IsPortableFramework())
			{
				return GetCompatibilityBetweenPortableLibraryAndPortableLibrary(projectFrameworkName, packageTargetFrameworkName, portableProfileTable);
			}
			return GetCompatibilityBetweenPortableLibraryAndNonPortableLibrary(projectFrameworkName, packageTargetFrameworkName, portableProfileTable) / 2;
		}
		long num = 0L;
		num += CalculateVersionDistance(projectFrameworkName.Version, GetEffectiveFrameworkVersion(projectFrameworkName, packageTargetFrameworkName, portableProfileTable));
		if (packageTargetFrameworkName.Profile.Equals(projectFrameworkName.Profile, StringComparison.OrdinalIgnoreCase))
		{
			num++;
		}
		if (packageTargetFrameworkName.Identifier.Equals(projectFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase))
		{
			num += 42949672960L;
		}
		return num;
	}

	private static long CalculateVersionDistance(Version projectVersion, Version targetFrameworkVersion)
	{
		long num = (long)(projectVersion.Major - targetFrameworkVersion.Major) * 255L * 255 * 255 + (long)(projectVersion.Minor - targetFrameworkVersion.Minor) * 255L * 255 + (long)(projectVersion.Build - targetFrameworkVersion.Build) * 255L + (projectVersion.Revision - targetFrameworkVersion.Revision);
		return 137438953472L - num;
	}

	private static Version GetEffectiveFrameworkVersion(FrameworkName projectFramework, FrameworkName targetFrameworkVersion, NetPortableProfileTable portableProfileTable)
	{
		if (targetFrameworkVersion.IsPortableFramework())
		{
			NetPortableProfile netPortableProfile = NetPortableProfile.Parse(targetFrameworkVersion.Profile, treatOptionalFrameworksAsSupportedFrameworks: false, portableProfileTable);
			if (netPortableProfile != null)
			{
				FrameworkName frameworkName = netPortableProfile.SupportedFrameworks.FirstOrDefault((FrameworkName f) => IsCompatible(projectFramework, f, portableProfileTable));
				if (frameworkName != null)
				{
					return frameworkName.Version;
				}
			}
		}
		return targetFrameworkVersion.Version;
	}

	internal static int GetCompatibilityBetweenPortableLibraryAndPortableLibrary(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName)
	{
		return GetCompatibilityBetweenPortableLibraryAndPortableLibrary(projectFrameworkName, packageTargetFrameworkName, NetPortableProfileTable.Default);
	}

	internal static int GetCompatibilityBetweenPortableLibraryAndPortableLibrary(FrameworkName projectFrameworkName, FrameworkName packageTargetFrameworkName, NetPortableProfileTable portableProfileTable)
	{
		NetPortableProfile netPortableProfile = NetPortableProfile.Parse(projectFrameworkName.Profile, treatOptionalFrameworksAsSupportedFrameworks: false, portableProfileTable);
		NetPortableProfile netPortableProfile2 = NetPortableProfile.Parse(packageTargetFrameworkName.Profile, treatOptionalFrameworksAsSupportedFrameworks: true, portableProfileTable);
		int num = 0;
		int num2 = 0;
		foreach (FrameworkName supportedPackageTargetFramework in netPortableProfile2.SupportedFrameworks)
		{
			FrameworkName frameworkName = netPortableProfile.SupportedFrameworks.FirstOrDefault((FrameworkName f) => IsCompatible(f, supportedPackageTargetFramework, portableProfileTable));
			if (frameworkName != null && frameworkName.Version > supportedPackageTargetFramework.Version)
			{
				num++;
			}
		}
		foreach (FrameworkName optionalProjectFramework in netPortableProfile.OptionalFrameworks)
		{
			FrameworkName frameworkName2 = netPortableProfile2.SupportedFrameworks.FirstOrDefault((FrameworkName f) => IsCompatible(f, optionalProjectFramework, portableProfileTable));
			if (frameworkName2 == null || frameworkName2.Version > optionalProjectFramework.Version)
			{
				num2++;
			}
			else if (frameworkName2 != null && frameworkName2.Version < optionalProjectFramework.Version)
			{
				num++;
			}
		}
		return -(((1 + netPortableProfile.SupportedFrameworks.Count + netPortableProfile.OptionalFrameworks.Count) * num2 + num) * 50 + netPortableProfile2.SupportedFrameworks.Count);
	}

	internal static long GetCompatibilityBetweenPortableLibraryAndNonPortableLibrary(FrameworkName projectFrameworkName, FrameworkName packagePortableFramework)
	{
		return GetCompatibilityBetweenPortableLibraryAndNonPortableLibrary(projectFrameworkName, packagePortableFramework, NetPortableProfileTable.Default);
	}

	internal static long GetCompatibilityBetweenPortableLibraryAndNonPortableLibrary(FrameworkName projectFrameworkName, FrameworkName packagePortableFramework, NetPortableProfileTable portableProfileTable)
	{
		NetPortableProfile netPortableProfile = NetPortableProfile.Parse(packagePortableFramework.Profile, treatOptionalFrameworksAsSupportedFrameworks: true, portableProfileTable);
		if (netPortableProfile == null)
		{
			return long.MinValue;
		}
		FrameworkName frameworkName = netPortableProfile.SupportedFrameworks.FirstOrDefault((FrameworkName f) => IsCompatible(projectFrameworkName, f, portableProfileTable));
		if (frameworkName != null)
		{
			return GetProfileCompatibility(projectFrameworkName, frameworkName, portableProfileTable) - netPortableProfile.SupportedFrameworks.Count * 2;
		}
		if (portableProfileTable.HasCompatibleProfileWith(netPortableProfile, projectFrameworkName, portableProfileTable))
		{
			return -(netPortableProfile.SupportedFrameworks.Count * 2);
		}
		return long.MinValue;
	}

	private static bool TryParseVersion(string versionString, out SemanticVersion version)
	{
		version = null;
		if (!SemanticVersion.TryParse(versionString, out version) && int.TryParse(versionString, out var result) && result > 0)
		{
			version = new SemanticVersion(new Version(result, 0));
		}
		return version != null;
	}

	public static bool IsPortableFramework(this FrameworkName framework)
	{
		if (framework != null)
		{
			return ".NETPortable".Equals(framework.Identifier, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}
}
