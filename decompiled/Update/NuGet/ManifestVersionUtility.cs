using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace NuGet;

internal static class ManifestVersionUtility
{
	public const int DefaultVersion = 1;

	public const int SemverVersion = 3;

	public const int TargetFrameworkSupportForDependencyContentsAndToolsVersion = 4;

	public const int TargetFrameworkSupportForReferencesVersion = 5;

	public const int XdtTransformationVersion = 6;

	private static readonly Type[] _xmlAttributes = new Type[3]
	{
		typeof(XmlElementAttribute),
		typeof(XmlAttributeAttribute),
		typeof(XmlArrayAttribute)
	};

	public static int GetManifestVersion(ManifestMetadata metadata)
	{
		return Math.Max(VisitObject(metadata), GetMaxVersionFromMetadata(metadata));
	}

	private static int GetMaxVersionFromMetadata(ManifestMetadata metadata)
	{
		if (metadata.ReferenceSets != null && metadata.ReferenceSets.Any((ManifestReferenceSet r) => r.TargetFramework != null))
		{
			return 5;
		}
		if (metadata.DependencySets != null && metadata.DependencySets.Any((ManifestDependencySet d) => d.TargetFramework != null))
		{
			return 4;
		}
		if (SemanticVersion.TryParse(metadata.Version, out var value) && !string.IsNullOrEmpty(value.SpecialVersion))
		{
			return 3;
		}
		return 1;
	}

	private static int VisitObject(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		return (from property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
			select VisitProperty(obj, property)).Max();
	}

	private static int VisitProperty(object obj, PropertyInfo property)
	{
		if (!IsManifestMetadata(property))
		{
			return 1;
		}
		object value = property.GetValue(obj, null);
		if (value == null)
		{
			return 1;
		}
		int propertyVersion = GetPropertyVersion(property);
		if (value is IList list)
		{
			if (list.Count > 0)
			{
				return Math.Max(propertyVersion, VisitList(list));
			}
			return propertyVersion;
		}
		if (value is string value2)
		{
			if (!string.IsNullOrEmpty(value2))
			{
				return propertyVersion;
			}
			return 1;
		}
		return propertyVersion;
	}

	private static int VisitList(IList list)
	{
		int num = 1;
		foreach (object item in list)
		{
			num = Math.Max(num, VisitObject(item));
		}
		return num;
	}

	private static int GetPropertyVersion(PropertyInfo property)
	{
		return property.GetCustomAttribute<ManifestVersionAttribute>()?.Version ?? 1;
	}

	private static bool IsManifestMetadata(PropertyInfo property)
	{
		return _xmlAttributes.Any((Type attr) => property.GetCustomAttribute(attr) != null);
	}
}
