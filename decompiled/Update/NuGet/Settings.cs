using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal class Settings : ISettings
{
	private readonly XDocument _config;

	private readonly IFileSystem _fileSystem;

	private readonly string _fileName;

	private Settings _next;

	private readonly bool _isMachineWideSettings;

	private int _priority;

	public bool IsMachineWideSettings => _isMachineWideSettings;

	public string ConfigFilePath
	{
		get
		{
			if (!Path.IsPathRooted(_fileName))
			{
				return Path.GetFullPath(Path.Combine(_fileSystem.Root, _fileName));
			}
			return _fileName;
		}
	}

	public Settings(IFileSystem fileSystem)
		: this(fileSystem, Constants.SettingsFileName, isMachineWideSettings: false)
	{
	}

	public Settings(IFileSystem fileSystem, string fileName)
		: this(fileSystem, fileName, isMachineWideSettings: false)
	{
	}

	public Settings(IFileSystem fileSystem, string fileName, bool isMachineWideSettings)
	{
		Settings settings = this;
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (string.IsNullOrEmpty(fileName))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "fileName");
		}
		_fileSystem = fileSystem;
		_fileName = fileName;
		XDocument conf = null;
		ExecuteSynchronized(delegate
		{
			conf = XmlUtility.GetOrCreateDocument(XName.op_Implicit("configuration"), settings._fileSystem, settings._fileName);
		});
		_config = conf;
		_isMachineWideSettings = isMachineWideSettings;
	}

	public static ISettings LoadDefaultSettings(IFileSystem fileSystem, string configFileName, IMachineWideSettings machineWideSettings)
	{
		List<Settings> list = new List<Settings>();
		if (fileSystem != null)
		{
			list.AddRange(from f in GetSettingsFileNames(fileSystem)
				select ReadSettings(fileSystem, f) into f
				where f != null
				select f);
		}
		LoadUserSpecificSettings(list, fileSystem, configFileName);
		if (machineWideSettings != null)
		{
			list.AddRange(machineWideSettings.Settings.Select((Settings s) => new Settings(s._fileSystem, s._fileName, s._isMachineWideSettings)));
		}
		if (list.IsEmpty())
		{
			return NullSettings.Instance;
		}
		list[0]._priority = list.Count;
		for (int num = 1; num < list.Count; num++)
		{
			list[num]._next = list[num - 1];
			list[num]._priority = list[num - 1]._priority - 1;
		}
		return list.Last();
	}

	private static void LoadUserSpecificSettings(List<Settings> validSettingFiles, IFileSystem fileSystem, string configFileName)
	{
		Settings settings = null;
		if (configFileName == null)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			if (!string.IsNullOrEmpty(folderPath))
			{
				string settingsPath = Path.Combine(folderPath, "NuGet", Constants.SettingsFileName);
				settings = ReadSettings(fileSystem ?? new PhysicalFileSystem("c:\\"), settingsPath);
			}
		}
		else
		{
			if (!fileSystem.FileExists(configFileName))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.FileDoesNotExit, new object[1] { fileSystem.GetFullPath(configFileName) }));
			}
			settings = ReadSettings(fileSystem, configFileName);
		}
		if (settings != null)
		{
			validSettingFiles.Add(settings);
		}
	}

	public static IEnumerable<Settings> LoadMachineWideSettings(IFileSystem fileSystem, params string[] paths)
	{
		List<Settings> list = new List<Settings>();
		string path = "NuGet\\Config";
		string text = Path.Combine(paths);
		while (true)
		{
			string path2 = Path.Combine(path, text);
			foreach (string file in fileSystem.GetFiles(path2, "*.config"))
			{
				Settings settings = ReadSettings(fileSystem, file, isMachineWideSettings: true);
				if (settings != null)
				{
					list.Add(settings);
				}
			}
			if (text.Length == 0)
			{
				break;
			}
			int num = text.LastIndexOf(Path.DirectorySeparatorChar);
			if (num < 0)
			{
				num = 0;
			}
			text = text.Substring(0, num);
		}
		return list;
	}

	public string GetValue(string section, string key)
	{
		return GetValue(section, key, isPath: false);
	}

	public string GetValue(string section, string key, bool isPath)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		XElement val = null;
		string result = null;
		for (Settings settings = this; settings != null; settings = settings._next)
		{
			XElement valueInternal = settings.GetValueInternal(section, key, val);
			if (val != valueInternal)
			{
				val = valueInternal;
				result = settings.ElementToValue(val, isPath);
			}
		}
		return result;
	}

	private static string ResolvePath(string configDirectory, string value)
	{
		string pathRoot = Path.GetPathRoot(value);
		if (pathRoot != null && pathRoot.Length == 1 && (pathRoot[0] == Path.DirectorySeparatorChar || value[0] == Path.AltDirectorySeparatorChar))
		{
			return Path.Combine(Path.GetPathRoot(configDirectory), value.Substring(1));
		}
		return Path.Combine(configDirectory, value);
	}

	private string ElementToValue(XElement element, bool isPath)
	{
		if (element == null)
		{
			return null;
		}
		string optionalAttributeValue = element.GetOptionalAttributeValue("value");
		if (!isPath || string.IsNullOrEmpty(optionalAttributeValue))
		{
			return optionalAttributeValue;
		}
		return _fileSystem.GetFullPath(ResolvePath(Path.GetDirectoryName(ConfigFilePath), optionalAttributeValue));
	}

	private XElement GetValueInternal(string section, string key, XElement curr)
	{
		XElement section2 = GetSection(_config.Root, section);
		if (section2 == null)
		{
			return curr;
		}
		return FindElementByKey(section2, key, curr);
	}

	public IList<KeyValuePair<string, string>> GetValues(string section)
	{
		return GetValues(section, isPath: false);
	}

	private IList<KeyValuePair<string, string>> GetValues(string section, bool isPath)
	{
		return (from v in GetSettingValues(section, isPath)
			select new KeyValuePair<string, string>(v.Key, v.Value)).ToList().AsReadOnly();
	}

	public IList<SettingValue> GetSettingValues(string section, bool isPath)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		List<SettingValue> list = new List<SettingValue>();
		for (Settings settings = this; settings != null; settings = settings._next)
		{
			settings.PopulateValues(section, list, isPath);
		}
		return list.AsReadOnly();
	}

	private void PopulateValues(string section, List<SettingValue> current, bool isPath)
	{
		XElement section2 = GetSection(_config.Root, section);
		if (section2 != null)
		{
			ReadSection((XContainer)(object)section2, current, isPath);
		}
	}

	public IList<KeyValuePair<string, string>> GetNestedValues(string section, string key)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		List<SettingValue> list = new List<SettingValue>();
		for (Settings settings = this; settings != null; settings = settings._next)
		{
			settings.PopulateNestedValues(section, key, list);
		}
		return list.Select((SettingValue v) => new KeyValuePair<string, string>(v.Key, v.Value)).ToList().AsReadOnly();
	}

	private void PopulateNestedValues(string section, string key, List<SettingValue> current)
	{
		XElement section2 = GetSection(_config.Root, section);
		if (section2 != null)
		{
			XElement section3 = GetSection(section2, key);
			if (section3 != null)
			{
				ReadSection((XContainer)(object)section3, current, isPath: false);
			}
		}
	}

	public void SetValue(string section, string key, string value)
	{
		if (IsMachineWideSettings)
		{
			if (_next == null)
			{
				throw new InvalidOperationException(NuGetResources.Error_NoWritableConfig);
			}
			_next.SetValue(section, key, value);
			return;
		}
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		XElement orCreateSection = GetOrCreateSection(_config.Root, section);
		SetValueInternal(orCreateSection, key, value);
		Save();
	}

	public void SetValues(string section, IList<KeyValuePair<string, string>> values)
	{
		if (IsMachineWideSettings)
		{
			if (_next == null)
			{
				throw new InvalidOperationException(NuGetResources.Error_NoWritableConfig);
			}
			_next.SetValues(section, values);
			return;
		}
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		XElement orCreateSection = GetOrCreateSection(_config.Root, section);
		foreach (KeyValuePair<string, string> value in values)
		{
			SetValueInternal(orCreateSection, value.Key, value.Value);
		}
		Save();
	}

	public void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
	{
		if (IsMachineWideSettings)
		{
			if (_next == null)
			{
				throw new InvalidOperationException(NuGetResources.Error_NoWritableConfig);
			}
			_next.SetNestedValues(section, key, values);
			return;
		}
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		XElement orCreateSection = GetOrCreateSection(GetOrCreateSection(_config.Root, section), key);
		foreach (KeyValuePair<string, string> value in values)
		{
			SetValueInternal(orCreateSection, value.Key, value.Value);
		}
		Save();
	}

	private void SetValueInternal(XElement sectionElement, string key, string value)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		XElement val = FindElementByKey(sectionElement, key, null);
		if (val != null)
		{
			val.SetAttributeValue(XName.op_Implicit("value"), (object)value);
			Save();
		}
		else
		{
			((XContainer)(object)sectionElement).AddIndented((XContainer)new XElement(XName.op_Implicit("add"), new object[2]
			{
				(object)new XAttribute(XName.op_Implicit("key"), (object)key),
				(object)new XAttribute(XName.op_Implicit("value"), (object)value)
			}));
		}
	}

	public bool DeleteValue(string section, string key)
	{
		if (IsMachineWideSettings)
		{
			if (_next == null)
			{
				throw new InvalidOperationException(NuGetResources.Error_NoWritableConfig);
			}
			return _next.DeleteValue(section, key);
		}
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "key");
		}
		XElement section2 = GetSection(_config.Root, section);
		if (section2 == null)
		{
			return false;
		}
		XElement val = FindElementByKey(section2, key, null);
		if (val == null)
		{
			return false;
		}
		((XNode)(object)val).RemoveIndented();
		Save();
		return true;
	}

	public bool DeleteSection(string section)
	{
		if (IsMachineWideSettings)
		{
			if (_next == null)
			{
				throw new InvalidOperationException(NuGetResources.Error_NoWritableConfig);
			}
			return _next.DeleteSection(section);
		}
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "section");
		}
		XElement section2 = GetSection(_config.Root, section);
		if (section2 == null)
		{
			return false;
		}
		((XNode)(object)section2).RemoveIndented();
		Save();
		return true;
	}

	private void ReadSection(XContainer sectionElement, ICollection<SettingValue> values, bool isPath)
	{
		foreach (XElement item in sectionElement.Elements())
		{
			string localName = item.Name.LocalName;
			if (localName.Equals("add", StringComparison.OrdinalIgnoreCase))
			{
				KeyValuePair<string, string> keyValuePair = ReadValue(item, isPath);
				values.Add(new SettingValue(keyValuePair.Key, keyValuePair.Value, _isMachineWideSettings, _priority));
			}
			else if (localName.Equals("clear", StringComparison.OrdinalIgnoreCase))
			{
				values.Clear();
			}
		}
	}

	private void Save()
	{
		ExecuteSynchronized(delegate
		{
			_fileSystem.AddFile(_fileName, (Action<Stream>)_config.Save);
		});
	}

	private KeyValuePair<string, string> ReadValue(XElement element, bool isPath)
	{
		XAttribute val = element.Attribute(XName.op_Implicit("key"));
		XAttribute val2 = element.Attribute(XName.op_Implicit("value"));
		if (val == null || string.IsNullOrEmpty(val.Value) || val2 == null)
		{
			throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UserSettings_UnableToParseConfigFile, new object[1] { ConfigFilePath }));
		}
		string text = val2.Value;
		if (isPath && Uri.TryCreate(text, UriKind.Relative, out Uri _))
		{
			string directoryName = Path.GetDirectoryName(ConfigFilePath);
			text = _fileSystem.GetFullPath(Path.Combine(directoryName, text));
		}
		return new KeyValuePair<string, string>(val.Value, text);
	}

	private static XElement GetSection(XElement parentElement, string section)
	{
		section = XmlConvert.EncodeLocalName(section);
		return ((XContainer)parentElement).Element(XName.op_Implicit(section));
	}

	private static XElement GetOrCreateSection(XElement parentElement, string sectionName)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		sectionName = XmlConvert.EncodeLocalName(sectionName);
		XElement val = ((XContainer)parentElement).Element(XName.op_Implicit(sectionName));
		if (val == null)
		{
			val = new XElement(XName.op_Implicit(sectionName));
			((XContainer)(object)parentElement).AddIndented((XContainer)(object)val);
		}
		return val;
	}

	private static XElement FindElementByKey(XElement sectionElement, string key, XElement curr)
	{
		XElement result = curr;
		foreach (XElement item in ((XContainer)sectionElement).Elements())
		{
			string localName = item.Name.LocalName;
			if (localName.Equals("clear", StringComparison.OrdinalIgnoreCase))
			{
				result = null;
			}
			else if (localName.Equals("add", StringComparison.OrdinalIgnoreCase) && item.GetOptionalAttributeValue("key").Equals(key, StringComparison.OrdinalIgnoreCase))
			{
				result = item;
			}
		}
		return result;
	}

	private static IEnumerable<string> GetSettingsFileNames(IFileSystem fileSystem)
	{
		foreach (string settingsFilePath in GetSettingsFilePaths(fileSystem))
		{
			string text = Path.Combine(settingsFilePath, Constants.SettingsFileName);
			if (fileSystem.FileExists(text))
			{
				yield return text;
			}
		}
	}

	private static IEnumerable<string> GetSettingsFilePaths(IFileSystem fileSystem)
	{
		for (string root = fileSystem.Root; root != null; root = Path.GetDirectoryName(root))
		{
			yield return root;
		}
	}

	private static Settings ReadSettings(IFileSystem fileSystem, string settingsPath)
	{
		return ReadSettings(fileSystem, settingsPath, isMachineWideSettings: false);
	}

	private static Settings ReadSettings(IFileSystem fileSystem, string settingsPath, bool isMachineWideSettings)
	{
		try
		{
			return new Settings(fileSystem, settingsPath, isMachineWideSettings);
		}
		catch (XmlException)
		{
			return null;
		}
	}

	private void ExecuteSynchronized(Action ioOperation)
	{
		string fullPath = _fileSystem.GetFullPath(_fileName);
		using Mutex mutex = new Mutex(initiallyOwned: false, "Global\\" + EncryptionUtility.GenerateUniqueToken(fullPath));
		bool flag = false;
		try
		{
			flag = mutex.WaitOne(TimeSpan.FromMinutes(1.0));
			ioOperation();
		}
		finally
		{
			if (flag)
			{
				mutex.ReleaseMutex();
			}
		}
	}
}
