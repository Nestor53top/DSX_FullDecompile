using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal static class ManifestReader
{
	private static readonly string[] RequiredElements = new string[4] { "id", "version", "authors", "description" };

	public static Manifest ReadManifest(XDocument document)
	{
		XElement val = ((XContainer)(object)document.Root).ElementsNoNamespace("metadata").FirstOrDefault();
		if (val == null)
		{
			throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_RequiredElementMissing, new object[1] { "metadata" }));
		}
		return new Manifest
		{
			Metadata = ReadMetadata(val),
			Files = ReadFilesList(((XContainer)(object)document.Root).ElementsNoNamespace("files").FirstOrDefault())
		};
	}

	private static ManifestMetadata ReadMetadata(XElement xElement)
	{
		ManifestMetadata manifestMetadata = new ManifestMetadata();
		manifestMetadata.DependencySets = new List<ManifestDependencySet>();
		manifestMetadata.ReferenceSets = new List<ManifestReferenceSet>();
		manifestMetadata.MinClientVersionString = xElement.GetOptionalAttributeValue("minClientVersion");
		HashSet<string> hashSet = new HashSet<string>();
		for (XNode val = ((XContainer)xElement).FirstNode; val != null; val = val.NextNode)
		{
			XElement val2 = (XElement)(object)((val is XElement) ? val : null);
			if (val2 != null)
			{
				ReadMetadataValue(manifestMetadata, val2, hashSet);
			}
		}
		string[] requiredElements = RequiredElements;
		foreach (string text in requiredElements)
		{
			if (!hashSet.Contains(text))
			{
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_RequiredElementMissing, new object[1] { text }));
			}
		}
		return manifestMetadata;
	}

	private static void ReadMetadataValue(ManifestMetadata manifestMetadata, XElement element, HashSet<string> allElements)
	{
		if (element.Value != null)
		{
			allElements.Add(element.Name.LocalName);
			string text = element.Value.SafeTrim();
			switch (element.Name.LocalName)
			{
			case "id":
				manifestMetadata.Id = text;
				break;
			case "version":
				manifestMetadata.Version = text;
				break;
			case "authors":
				manifestMetadata.Authors = text;
				break;
			case "owners":
				manifestMetadata.Owners = text;
				break;
			case "licenseUrl":
				manifestMetadata.LicenseUrl = text;
				break;
			case "projectUrl":
				manifestMetadata.ProjectUrl = text;
				break;
			case "iconUrl":
				manifestMetadata.IconUrl = text;
				break;
			case "requireLicenseAcceptance":
				manifestMetadata.RequireLicenseAcceptance = XmlConvert.ToBoolean(text);
				break;
			case "developmentDependency":
				manifestMetadata.DevelopmentDependency = XmlConvert.ToBoolean(text);
				break;
			case "description":
				manifestMetadata.Description = text;
				break;
			case "summary":
				manifestMetadata.Summary = text;
				break;
			case "releaseNotes":
				manifestMetadata.ReleaseNotes = text;
				break;
			case "copyright":
				manifestMetadata.Copyright = text;
				break;
			case "language":
				manifestMetadata.Language = text;
				break;
			case "title":
				manifestMetadata.Title = text;
				break;
			case "tags":
				manifestMetadata.Tags = text;
				break;
			case "dependencies":
				manifestMetadata.DependencySets = ReadDependencySets(element);
				break;
			case "frameworkAssemblies":
				manifestMetadata.FrameworkAssemblies = ReadFrameworkAssemblies(element);
				break;
			case "references":
				manifestMetadata.ReferenceSets = ReadReferenceSets(element);
				break;
			}
		}
	}

	private static List<ManifestReferenceSet> ReadReferenceSets(XElement referencesElement)
	{
		if (!referencesElement.HasElements)
		{
			return new List<ManifestReferenceSet>(0);
		}
		if (((XContainer)(object)referencesElement).ElementsNoNamespace("group").Any() && ((XContainer)(object)referencesElement).ElementsNoNamespace("reference").Any())
		{
			throw new InvalidDataException(NuGetResources.Manifest_ReferencesHasMixedElements);
		}
		List<ManifestReference> list = ReadReference(referencesElement, throwIfEmpty: false);
		if (list.Count > 0)
		{
			ManifestReferenceSet item = new ManifestReferenceSet
			{
				References = list
			};
			return new List<ManifestReferenceSet> { item };
		}
		return (from element in ((XContainer)(object)referencesElement).ElementsNoNamespace("group")
			select new ManifestReferenceSet
			{
				TargetFramework = element.GetOptionalAttributeValue("targetFramework").SafeTrim(),
				References = ReadReference(element, throwIfEmpty: true)
			}).ToList();
	}

	public static List<ManifestReference> ReadReference(XElement referenceElement, bool throwIfEmpty)
	{
		List<ManifestReference> list = (from element in ((XContainer)(object)referenceElement).ElementsNoNamespace("reference")
			let fileAttribute = element.Attribute(XName.op_Implicit("file"))
			where fileAttribute != null && !string.IsNullOrEmpty(fileAttribute.Value)
			select new ManifestReference
			{
				File = fileAttribute.Value.SafeTrim()
			}).ToList();
		if (throwIfEmpty && list.Count == 0)
		{
			throw new InvalidDataException(NuGetResources.Manifest_ReferencesIsEmpty);
		}
		return list;
	}

	private static List<ManifestFrameworkAssembly> ReadFrameworkAssemblies(XElement frameworkElement)
	{
		if (!frameworkElement.HasElements)
		{
			return new List<ManifestFrameworkAssembly>(0);
		}
		return (from element in ((XContainer)(object)frameworkElement).ElementsNoNamespace("frameworkAssembly")
			let assemblyNameAttribute = element.Attribute(XName.op_Implicit("assemblyName"))
			where assemblyNameAttribute != null && !string.IsNullOrEmpty(assemblyNameAttribute.Value)
			select new ManifestFrameworkAssembly
			{
				AssemblyName = assemblyNameAttribute.Value.SafeTrim(),
				TargetFramework = element.GetOptionalAttributeValue("targetFramework").SafeTrim()
			}).ToList();
	}

	private static List<ManifestDependencySet> ReadDependencySets(XElement dependenciesElement)
	{
		if (!dependenciesElement.HasElements)
		{
			return new List<ManifestDependencySet>();
		}
		if (((XContainer)(object)dependenciesElement).ElementsNoNamespace("dependency").Any() && ((XContainer)(object)dependenciesElement).ElementsNoNamespace("group").Any())
		{
			throw new InvalidDataException(NuGetResources.Manifest_DependenciesHasMixedElements);
		}
		List<ManifestDependency> list = ReadDependencies(dependenciesElement);
		if (list.Count > 0)
		{
			ManifestDependencySet item = new ManifestDependencySet
			{
				Dependencies = list
			};
			return new List<ManifestDependencySet> { item };
		}
		return (from element in ((XContainer)(object)dependenciesElement).ElementsNoNamespace("group")
			select new ManifestDependencySet
			{
				TargetFramework = element.GetOptionalAttributeValue("targetFramework").SafeTrim(),
				Dependencies = ReadDependencies(element)
			}).ToList();
	}

	private static List<ManifestDependency> ReadDependencies(XElement containerElement)
	{
		return (from element in ((XContainer)(object)containerElement).ElementsNoNamespace("dependency")
			let idElement = element.Attribute(XName.op_Implicit("id"))
			where idElement != null && !string.IsNullOrEmpty(idElement.Value)
			select new ManifestDependency
			{
				Id = idElement.Value.SafeTrim(),
				Version = element.GetOptionalAttributeValue("version").SafeTrim()
			}).ToList();
	}

	private static List<ManifestFile> ReadFilesList(XElement xElement)
	{
		if (xElement == null)
		{
			return null;
		}
		List<ManifestFile> list = new List<ManifestFile>();
		foreach (XElement item in ((XContainer)(object)xElement).ElementsNoNamespace("file"))
		{
			XAttribute val = item.Attribute(XName.op_Implicit("src"));
			if (val != null && !string.IsNullOrEmpty(val.Value))
			{
				string target = item.GetOptionalAttributeValue("target").SafeTrim();
				string exclude = item.GetOptionalAttributeValue("exclude").SafeTrim();
				list.AddRange(from source in val.Value.Trim(new char[1] { ';' }).Split(new char[1] { ';' })
					select new ManifestFile
					{
						Source = source.SafeTrim(),
						Target = target.SafeTrim(),
						Exclude = exclude.SafeTrim()
					});
			}
		}
		return list;
	}
}
