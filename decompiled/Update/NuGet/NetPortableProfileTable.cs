using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security;
using System.Xml;
using System.Xml.Linq;

namespace NuGet;

internal class NetPortableProfileTable
{
	private const string PortableReferenceAssemblyPathEnvironmentVariableName = "NuGetPortableReferenceAssemblyPath";

	private static readonly Lazy<NetPortableProfileTable> _defaultTable = new Lazy<NetPortableProfileTable>(LoadDefaultTable);

	private IDictionary<string, NetPortableProfile> _portableProfilesByCustomProfileString;

	private IDictionary<string, List<Tuple<Version, ISet<string>>>> _portableProfilesSetByOptionalFrameworks;

	public static NetPortableProfileTable Default => _defaultTable.Value;

	public NetPortableProfileCollection Profiles { get; private set; }

	public NetPortableProfileTable(IEnumerable<NetPortableProfile> profiles)
	{
		Profiles = new NetPortableProfileCollection();
		Profiles.AddRange(profiles);
		_portableProfilesByCustomProfileString = Profiles.ToDictionary((NetPortableProfile p) => p.CustomProfileString);
		CreateOptionalFrameworksDictionary();
	}

	public NetPortableProfile GetProfile(string profileName)
	{
		if (string.IsNullOrEmpty(profileName))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "profileName");
		}
		if (Profiles.Contains(profileName))
		{
			return Profiles[profileName];
		}
		NetPortableProfile value = null;
		_portableProfilesByCustomProfileString.TryGetValue(profileName, out value);
		return value;
	}

	public void Serialize(Stream output)
	{
		NetPortableProfileTableSerializer.Serialize(this, output);
	}

	public static NetPortableProfileTable Deserialize(Stream input)
	{
		return NetPortableProfileTableSerializer.Deserialize(input);
	}

	private void CreateOptionalFrameworksDictionary()
	{
		_portableProfilesSetByOptionalFrameworks = new Dictionary<string, List<Tuple<Version, ISet<string>>>>();
		foreach (NetPortableProfile profile in Profiles)
		{
			foreach (FrameworkName optionalFramework in profile.OptionalFrameworks)
			{
				if (!_portableProfilesSetByOptionalFrameworks.ContainsKey(optionalFramework.Identifier))
				{
					_portableProfilesSetByOptionalFrameworks.Add(optionalFramework.Identifier, new List<Tuple<Version, ISet<string>>>());
				}
				List<Tuple<Version, ISet<string>>> list = _portableProfilesSetByOptionalFrameworks[optionalFramework.Identifier];
				if (list != null)
				{
					Tuple<Version, ISet<string>> tuple = list.Where((Tuple<Version, ISet<string>> tuple2) => tuple2.Item1.Equals(optionalFramework.Version)).FirstOrDefault();
					if (tuple == null)
					{
						tuple = new Tuple<Version, ISet<string>>(optionalFramework.Version, new HashSet<string>());
						list.Add(tuple);
					}
					tuple.Item2.Add(profile.Name);
				}
			}
		}
	}

	internal bool HasCompatibleProfileWith(NetPortableProfile packageFramework, FrameworkName projectOptionalFrameworkName, NetPortableProfileTable portableProfileTable)
	{
		List<Tuple<Version, ISet<string>>> value = null;
		if (_portableProfilesSetByOptionalFrameworks != null && _portableProfilesSetByOptionalFrameworks.TryGetValue(projectOptionalFrameworkName.Identifier, out value))
		{
			foreach (Tuple<Version, ISet<string>> item in value)
			{
				if (!(projectOptionalFrameworkName.Version >= item.Item1))
				{
					continue;
				}
				foreach (string item2 in item.Item2)
				{
					NetPortableProfile profile = GetProfile(item2);
					if (profile != null && packageFramework.IsCompatibleWith(profile, portableProfileTable))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static NetPortableProfileTable LoadFromProfileDirectory(string portableRootDirectory)
	{
		return new NetPortableProfileTable(LoadPortableProfiles(portableRootDirectory));
	}

	private static NetPortableProfileTable LoadDefaultTable()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("NuGetPortableReferenceAssemblyPath");
		string portableRootDirectory = (string.IsNullOrEmpty(environmentVariable) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.DoNotVerify), "Reference Assemblies\\Microsoft\\Framework\\.NETPortable") : environmentVariable);
		return LoadFromProfileDirectory(portableRootDirectory);
	}

	private static IEnumerable<NetPortableProfile> LoadPortableProfiles(string portableRootDirectory)
	{
		if (Directory.Exists(portableRootDirectory))
		{
			return Directory.EnumerateDirectories(portableRootDirectory, "v*", SearchOption.TopDirectoryOnly).SelectMany((string versionDir) => LoadProfilesFromFramework(versionDir, versionDir + "\\Profile\\"));
		}
		return Enumerable.Empty<NetPortableProfile>();
	}

	private static IEnumerable<NetPortableProfile> LoadProfilesFromFramework(string version, string profileFilesPath)
	{
		if (Directory.Exists(profileFilesPath))
		{
			try
			{
				return from profileDir in Directory.EnumerateDirectories(profileFilesPath, "Profile*")
					select LoadPortableProfile(version, profileDir) into p
					where p != null
					select p;
			}
			catch (IOException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}
		return Enumerable.Empty<NetPortableProfile>();
	}

	private static NetPortableProfile LoadPortableProfile(string version, string profileDirectory)
	{
		string fileName = Path.GetFileName(profileDirectory);
		string text = Path.Combine(profileDirectory, "SupportedFrameworks");
		if (!Directory.Exists(text))
		{
			return null;
		}
		return LoadPortableProfile(version, fileName, new PhysicalFileSystem(text), Directory.EnumerateFiles(text, "*.xml"));
	}

	internal static NetPortableProfile LoadPortableProfile(string version, string profileName, IFileSystem fileSystem, IEnumerable<string> frameworkFiles)
	{
		IEnumerable<FrameworkName> enumerable = from p in frameworkFiles
			select LoadSupportedFramework(fileSystem, p) into p
			where p != null
			select p;
		List<FrameworkName> optionalFrameworks = enumerable.Where((FrameworkName p) => IsOptionalFramework(p)).ToList();
		IEnumerable<FrameworkName> supportedFrameworks = (optionalFrameworks.IsEmpty() ? enumerable : enumerable.Where((FrameworkName p) => !optionalFrameworks.Contains(p)));
		return new NetPortableProfile(version, profileName, supportedFrameworks, optionalFrameworks);
	}

	private static bool IsOptionalFramework(FrameworkName framework)
	{
		if (!framework.Identifier.StartsWith("Mono", StringComparison.OrdinalIgnoreCase))
		{
			return framework.Identifier.StartsWith("Xamarin", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static FrameworkName LoadSupportedFramework(IFileSystem fileSystem, string frameworkFile)
	{
		using Stream stream = fileSystem.OpenFile(frameworkFile);
		return LoadSupportedFramework(stream);
	}

	internal static FrameworkName LoadSupportedFramework(Stream stream)
	{
		try
		{
			XElement root = XmlUtility.LoadSafe(stream).Root;
			if (root.Name.LocalName.Equals("Framework", StringComparison.Ordinal))
			{
				string optionalAttributeValue = root.GetOptionalAttributeValue("Identifier");
				if (optionalAttributeValue == null)
				{
					return null;
				}
				string optionalAttributeValue2 = root.GetOptionalAttributeValue("MinimumVersion");
				if (optionalAttributeValue2 == null)
				{
					return null;
				}
				if (!Version.TryParse(optionalAttributeValue2, out Version result))
				{
					return null;
				}
				string text = root.GetOptionalAttributeValue("Profile");
				if (text == null)
				{
					text = "";
				}
				if (text.EndsWith("*", StringComparison.Ordinal))
				{
					text = text.Substring(0, text.Length - 1);
					if (text.Equals("WindowsPhone7", StringComparison.OrdinalIgnoreCase))
					{
						text = "WindowsPhone71";
					}
					else if (optionalAttributeValue.Equals("Silverlight", StringComparison.OrdinalIgnoreCase) && text.Equals("WindowsPhone", StringComparison.OrdinalIgnoreCase) && result == new Version(4, 0))
					{
						result = new Version(3, 0);
					}
				}
				return new FrameworkName(optionalAttributeValue, result, text);
			}
		}
		catch (XmlException)
		{
		}
		catch (IOException)
		{
		}
		catch (SecurityException)
		{
		}
		return null;
	}
}
