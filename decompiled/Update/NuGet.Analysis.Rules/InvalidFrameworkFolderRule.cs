using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class InvalidFrameworkFolderRule : IPackageRule
{
	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (IPackageFile file in package.GetFiles())
		{
			string[] array = file.Path.Split(new char[1] { Path.DirectorySeparatorChar });
			if (array.Length >= 3 && array[0].Equals(Constants.LibDirectory, StringComparison.OrdinalIgnoreCase))
			{
				hashSet.Add(array[1]);
			}
		}
		return hashSet.Where((string s) => !IsValidFrameworkName(s) && !IsValidCultureName(package, s)).Select(CreatePackageIssue);
	}

	private static bool IsValidFrameworkName(string name)
	{
		FrameworkName frameworkName;
		try
		{
			frameworkName = VersionUtility.ParseFrameworkName(name);
		}
		catch (ArgumentException)
		{
			frameworkName = VersionUtility.UnsupportedFrameworkName;
		}
		return frameworkName != VersionUtility.UnsupportedFrameworkName;
	}

	private static bool IsValidCultureName(IPackage package, string name)
	{
		if (string.IsNullOrEmpty(package.Language))
		{
			return false;
		}
		return name.Equals(package.Language, StringComparison.OrdinalIgnoreCase);
	}

	private PackageIssue CreatePackageIssue(string target)
	{
		return new PackageIssue(AnalysisResources.InvalidFrameworkTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.InvalidFrameworkDescription, new object[1] { target }), AnalysisResources.InvalidFrameworkSolution);
	}
}
