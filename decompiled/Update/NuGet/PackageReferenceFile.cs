using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal class PackageReferenceFile
{
	private readonly IFileSystem _fileSystem;

	private readonly string _path;

	private readonly Dictionary<string, string> _constraints = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, string> _developmentFlags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public string FullPath => _fileSystem.GetFullPath(_path);

	public IFileSystem FileSystem => _fileSystem;

	public PackageReferenceFile(string path)
		: this(new PhysicalFileSystem(Path.GetDirectoryName(path)), Path.GetFileName(path))
	{
	}

	public PackageReferenceFile(IFileSystem fileSystem, string path)
		: this(fileSystem, path, null)
	{
	}

	public PackageReferenceFile(IFileSystem fileSystem, string path, string projectName)
	{
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "path");
		}
		_fileSystem = fileSystem;
		if (!string.IsNullOrEmpty(projectName))
		{
			string path2 = ConstructPackagesConfigFromProjectName(projectName);
			if (_fileSystem.FileExists(path2))
			{
				_path = path2;
			}
		}
		if (_path == null)
		{
			_path = path;
		}
	}

	public static PackageReferenceFile CreateFromProject(string projectFileFullPath)
	{
		return new PackageReferenceFile(new PhysicalFileSystem(Path.GetDirectoryName(projectFileFullPath)), projectName: Path.GetFileNameWithoutExtension(projectFileFullPath), path: Constants.PackageReferenceFile);
	}

	public static bool IsValidConfigFileName(string fileName)
	{
		if (fileName != null && fileName.StartsWith("packages.", StringComparison.OrdinalIgnoreCase))
		{
			return fileName.EndsWith(".config", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public IEnumerable<PackageReference> GetPackageReferences()
	{
		return GetPackageReferences(requireVersion: true);
	}

	public IEnumerable<PackageReference> GetPackageReferences(bool requireVersion)
	{
		XDocument document = GetDocument();
		if (document == null)
		{
			yield break;
		}
		foreach (XElement item in ((XContainer)document.Root).Elements(XName.op_Implicit("package")))
		{
			string optionalAttributeValue = item.GetOptionalAttributeValue("id");
			string optionalAttributeValue2 = item.GetOptionalAttributeValue("version");
			string optionalAttributeValue3 = item.GetOptionalAttributeValue("allowedVersions");
			string optionalAttributeValue4 = item.GetOptionalAttributeValue("targetFramework");
			string optionalAttributeValue5 = item.GetOptionalAttributeValue("developmentDependency");
			string optionalAttributeValue6 = item.GetOptionalAttributeValue("requireReinstallation");
			SemanticVersion value = null;
			if (string.IsNullOrEmpty(optionalAttributeValue))
			{
				continue;
			}
			if ((requireVersion || !string.IsNullOrEmpty(optionalAttributeValue2)) && !SemanticVersion.TryParse(optionalAttributeValue2, out value))
			{
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ReferenceFile_InvalidVersion, new object[2] { optionalAttributeValue2, _path }));
			}
			IVersionSpec result = null;
			if (!string.IsNullOrEmpty(optionalAttributeValue3))
			{
				if (!VersionUtility.TryParseVersionSpec(optionalAttributeValue3, out result))
				{
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ReferenceFile_InvalidVersion, new object[2] { optionalAttributeValue3, _path }));
				}
				_constraints[optionalAttributeValue] = optionalAttributeValue3;
			}
			FrameworkName frameworkName = null;
			if (!string.IsNullOrEmpty(optionalAttributeValue4))
			{
				frameworkName = VersionUtility.ParseFrameworkName(optionalAttributeValue4);
				if (frameworkName == VersionUtility.UnsupportedFrameworkName)
				{
					frameworkName = null;
				}
			}
			bool result2 = false;
			if (!string.IsNullOrEmpty(optionalAttributeValue5))
			{
				if (!bool.TryParse(optionalAttributeValue5, out result2))
				{
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ReferenceFile_InvalidDevelopmentFlag, new object[2] { optionalAttributeValue5, _path }));
				}
				_developmentFlags[optionalAttributeValue] = optionalAttributeValue5;
			}
			bool result3 = false;
			if (!string.IsNullOrEmpty(optionalAttributeValue6) && !bool.TryParse(optionalAttributeValue6, out result3))
			{
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ReferenceFile_InvalidRequireReinstallationFlag, new object[2] { optionalAttributeValue6, _path }));
			}
			yield return new PackageReference(optionalAttributeValue, value, result, frameworkName, result2, result3);
		}
	}

	public bool DeleteEntry(string id, SemanticVersion version)
	{
		XDocument document = GetDocument();
		if (document == null)
		{
			return false;
		}
		return DeleteEntry(document, id, version);
	}

	public bool EntryExists(string packageId, SemanticVersion version)
	{
		XDocument document = GetDocument();
		if (document == null)
		{
			return false;
		}
		return FindEntry(document, packageId, version) != null;
	}

	public void AddEntry(string id, SemanticVersion version)
	{
		AddEntry(id, version, developmentDependency: false);
	}

	public void AddEntry(string id, SemanticVersion version, bool developmentDependency)
	{
		AddEntry(id, version, developmentDependency, null);
	}

	public void AddEntry(string id, SemanticVersion version, bool developmentDependency, FrameworkName targetFramework)
	{
		XDocument document = GetDocument(createIfNotExists: true);
		AddEntry(document, id, version, developmentDependency, targetFramework);
	}

	public void MarkEntryForReinstallation(string id, SemanticVersion version, FrameworkName targetFramework, bool requireReinstallation)
	{
		XDocument document = GetDocument();
		if (document != null)
		{
			DeleteEntry(id, version);
			AddEntry(document, id, version, developmentDependency: false, targetFramework, requireReinstallation);
		}
	}

	private void AddEntry(XDocument document, string id, SemanticVersion version, bool developmentDependency, FrameworkName targetFramework)
	{
		AddEntry(document, id, version, developmentDependency, targetFramework, requireReinstallation: false);
	}

	private void AddEntry(XDocument document, string id, SemanticVersion version, bool developmentDependency, FrameworkName targetFramework, bool requireReinstallation)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		XElement val = FindEntry(document, id, version);
		if (val != null)
		{
			((XNode)val).Remove();
		}
		XElement val2 = new XElement(XName.op_Implicit("package"), new object[2]
		{
			(object)new XAttribute(XName.op_Implicit("id"), (object)id),
			(object)new XAttribute(XName.op_Implicit("version"), (object)version)
		});
		if (targetFramework != null)
		{
			((XContainer)val2).Add((object)new XAttribute(XName.op_Implicit("targetFramework"), (object)VersionUtility.GetShortFrameworkName(targetFramework)));
		}
		if (_constraints.TryGetValue(id, out var value))
		{
			((XContainer)val2).Add((object)new XAttribute(XName.op_Implicit("allowedVersions"), (object)value));
		}
		if (_developmentFlags.TryGetValue(id, out var value2))
		{
			((XContainer)val2).Add((object)new XAttribute(XName.op_Implicit("developmentDependency"), (object)value2));
		}
		else if (developmentDependency)
		{
			((XContainer)val2).Add((object)new XAttribute(XName.op_Implicit("developmentDependency"), (object)"true"));
		}
		if (requireReinstallation)
		{
			((XContainer)val2).Add((object)new XAttribute(XName.op_Implicit("requireReinstallation"), (object)bool.TrueString));
		}
		((XContainer)document.Root).Add((object)val2);
		SaveDocument(document);
	}

	private static XElement FindEntry(XDocument document, string id, SemanticVersion version)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		return (from e in ((XContainer)document.Root).Elements(XName.op_Implicit("package"))
			let entryId = e.GetOptionalAttributeValue("id")
			let entryVersion = SemanticVersion.ParseOptionalVersion(e.GetOptionalAttributeValue("version"))
			where entryId != null && entryVersion != null
			where id.Equals(entryId, StringComparison.OrdinalIgnoreCase) && (version == null || entryVersion.Equals(version))
			select e).FirstOrDefault();
	}

	private void SaveDocument(XDocument document)
	{
		List<XElement> list = (from e in ((XContainer)document.Root).Elements(XName.op_Implicit("package"))
			let id = e.GetOptionalAttributeValue("id")
			let version = e.GetOptionalAttributeValue("version")
			where !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(version)
			orderby id
			select e).ToList();
		document.Root.RemoveAll();
		((XContainer)document.Root).Add((object)list);
		_fileSystem.AddFile(_path, (Action<Stream>)document.Save);
	}

	private bool DeleteEntry(XDocument document, string id, SemanticVersion version)
	{
		XElement val = FindEntry(document, id, version);
		if (val != null)
		{
			string optionalAttributeValue = val.GetOptionalAttributeValue("allowedVersions");
			if (!string.IsNullOrEmpty(optionalAttributeValue))
			{
				_constraints[id] = optionalAttributeValue;
			}
			string optionalAttributeValue2 = val.GetOptionalAttributeValue("developmentDependency");
			if (!string.IsNullOrEmpty(optionalAttributeValue2))
			{
				_developmentFlags[id] = optionalAttributeValue2;
			}
			((XNode)val).Remove();
			SaveDocument(document);
			if (!document.Root.HasElements)
			{
				_fileSystem.DeleteFile(_path);
				return true;
			}
		}
		return false;
	}

	private XDocument GetDocument(bool createIfNotExists = false)
	{
		//IL_0060: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		try
		{
			if (_fileSystem.FileExists(_path))
			{
				using (Stream input = _fileSystem.OpenFile(_path))
				{
					return XmlUtility.LoadSafe(input);
				}
			}
			if (createIfNotExists)
			{
				return new XDocument(new object[1] { (object)new XElement(XName.op_Implicit("packages")) });
			}
			return null;
		}
		catch (XmlException ex)
		{
			XmlException innerException = ex;
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorReadingFile, new object[1] { FullPath }), (Exception?)(object)innerException);
		}
	}

	private static string ConstructPackagesConfigFromProjectName(string projectName)
	{
		return "packages." + projectName.Replace(' ', '_') + ".config";
	}
}
