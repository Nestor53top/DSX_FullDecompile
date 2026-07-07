using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class InitScriptNotUnderToolsRule : IPackageRule
{
	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		foreach (IPackageFile toolFile in package.GetToolFiles())
		{
			string fileName = Path.GetFileName(toolFile.Path);
			if (toolFile.TargetFramework != null && fileName.Equals("init.ps1", StringComparison.OrdinalIgnoreCase))
			{
				yield return CreatePackageIssue(toolFile);
			}
		}
	}

	private static PackageIssue CreatePackageIssue(IPackageFile file)
	{
		return new PackageIssue(AnalysisResources.MisplacedInitScriptTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.MisplacedInitScriptDescription, new object[1] { file.Path }), AnalysisResources.MisplacedInitScriptSolution);
	}
}
