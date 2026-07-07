using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class MisplacedScriptFileRule : IPackageRule
{
	private const string ScriptExtension = ".ps1";

	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		foreach (IPackageFile file in package.GetFiles())
		{
			string path = file.Path;
			if (!path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			string toolsDirectory = Constants.ToolsDirectory;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			if (!path.StartsWith(toolsDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
			{
				yield return CreatePackageIssueForMisplacedScript(path);
				continue;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			if (!fileNameWithoutExtension.Equals("install", StringComparison.OrdinalIgnoreCase) && !fileNameWithoutExtension.Equals("uninstall", StringComparison.OrdinalIgnoreCase) && !fileNameWithoutExtension.Equals("init", StringComparison.OrdinalIgnoreCase))
			{
				yield return CreatePackageIssueForUnrecognizedScripts(path);
			}
		}
	}

	private static PackageIssue CreatePackageIssueForMisplacedScript(string target)
	{
		return new PackageIssue(AnalysisResources.ScriptOutsideToolsTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.ScriptOutsideToolsDescription, new object[1] { target }), AnalysisResources.ScriptOutsideToolsSolution);
	}

	private static PackageIssue CreatePackageIssueForUnrecognizedScripts(string target)
	{
		return new PackageIssue(AnalysisResources.UnrecognizedScriptTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.UnrecognizedScriptDescription, new object[1] { target }), AnalysisResources.UnrecognizedScriptSolution);
	}
}
