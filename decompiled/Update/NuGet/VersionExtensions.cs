using System;
using System.Collections.Generic;
using System.Globalization;

namespace NuGet;

internal static class VersionExtensions
{
	public static Func<IPackage, bool> ToDelegate(this IVersionSpec versionInfo)
	{
		if (versionInfo == null)
		{
			throw new ArgumentNullException("versionInfo");
		}
		return versionInfo.ToDelegate((IPackage p) => p.Version);
	}

	public static Func<T, bool> ToDelegate<T>(this IVersionSpec versionInfo, Func<T, SemanticVersion> extractor)
	{
		if (versionInfo == null)
		{
			throw new ArgumentNullException("versionInfo");
		}
		if (extractor == null)
		{
			throw new ArgumentNullException("extractor");
		}
		return delegate(T p)
		{
			SemanticVersion semanticVersion = extractor(p);
			bool flag = true;
			if (versionInfo.MinVersion != null)
			{
				flag = ((!versionInfo.IsMinInclusive) ? (flag && semanticVersion > versionInfo.MinVersion) : (flag && semanticVersion >= versionInfo.MinVersion));
			}
			if (versionInfo.MaxVersion != null)
			{
				flag = ((!versionInfo.IsMaxInclusive) ? (flag && semanticVersion < versionInfo.MaxVersion) : (flag && semanticVersion <= versionInfo.MaxVersion));
			}
			return flag;
		};
	}

	public static bool Satisfies(this IVersionSpec versionSpec, SemanticVersion version)
	{
		return versionSpec?.ToDelegate((SemanticVersion v) => v)(version) ?? true;
	}

	public static IEnumerable<string> GetComparableVersionStrings(this SemanticVersion version)
	{
		Version version2 = version.Version;
		string text = (string.IsNullOrEmpty(version.SpecialVersion) ? string.Empty : ("-" + version.SpecialVersion));
		string originalVersion = version.ToString();
		string[] originalVersionComponents = version.GetOriginalVersionComponents();
		LinkedList<string> linkedList = new LinkedList<string>();
		if (version2.Revision == 0)
		{
			if (version2.Build == 0)
			{
				string nextVersion = string.Format(CultureInfo.InvariantCulture, "{0}.{1}{2}", new object[3]
				{
					originalVersionComponents[0],
					originalVersionComponents[1],
					text
				});
				AddVersionToList(originalVersion, linkedList, nextVersion);
			}
			string nextVersion2 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}{3}", new object[4]
			{
				originalVersionComponents[0],
				originalVersionComponents[1],
				originalVersionComponents[2],
				text
			});
			AddVersionToList(originalVersion, linkedList, nextVersion2);
		}
		string nextVersion3 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}{4}", new object[5]
		{
			originalVersionComponents[0],
			originalVersionComponents[1],
			originalVersionComponents[2],
			originalVersionComponents[3],
			text
		});
		AddVersionToList(originalVersion, linkedList, nextVersion3);
		return linkedList;
	}

	private static void AddVersionToList(string originalVersion, LinkedList<string> paths, string nextVersion)
	{
		if (nextVersion.Equals(originalVersion, StringComparison.OrdinalIgnoreCase))
		{
			paths.AddFirst(nextVersion);
		}
		else
		{
			paths.AddLast(nextVersion);
		}
	}
}
