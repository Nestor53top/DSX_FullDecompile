using System;
using System.Collections.Generic;
using System.Globalization;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class WinRTNameIsObsoleteRule : IPackageRule
{
	private static string[] Prefixes = new string[6] { "content\\winrt45\\", "lib\\winrt45\\", "tools\\winrt45\\", "content\\winrt\\", "lib\\winrt\\", "tools\\winrt\\" };

	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		foreach (IPackageFile file in package.GetFiles())
		{
			string[] prefixes = Prefixes;
			foreach (string value in prefixes)
			{
				if (file.Path.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					yield return CreateIssue(file);
				}
			}
		}
	}

	private static PackageIssue CreateIssue(IPackageFile file)
	{
		return new PackageIssue(AnalysisResources.WinRTObsoleteTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.WinRTObsoleteDescription, new object[1] { file.Path }), AnalysisResources.WinRTObsoleteSolution);
	}
}
