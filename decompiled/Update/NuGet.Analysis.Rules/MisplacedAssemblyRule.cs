using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class MisplacedAssemblyRule : IPackageRule
{
	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		foreach (IPackageFile file in package.GetFiles())
		{
			string path = file.Path;
			string directoryName = Path.GetDirectoryName(path);
			if (directoryName.Equals(Constants.LibDirectory, StringComparison.OrdinalIgnoreCase))
			{
				if (PackageHelper.IsAssembly(path))
				{
					yield return CreatePackageIssueForAssembliesUnderLib(path);
				}
				continue;
			}
			string libDirectory = Constants.LibDirectory;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			if (!directoryName.StartsWith(libDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase) && (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".winmd", StringComparison.OrdinalIgnoreCase)))
			{
				yield return CreatePackageIssueForAssembliesOutsideLib(path);
			}
		}
	}

	private static PackageIssue CreatePackageIssueForAssembliesUnderLib(string target)
	{
		return new PackageIssue(AnalysisResources.AssemblyUnderLibTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.AssemblyUnderLibDescription, new object[1] { target }), AnalysisResources.AssemblyUnderLibSolution);
	}

	private static PackageIssue CreatePackageIssueForAssembliesOutsideLib(string target)
	{
		return new PackageIssue(AnalysisResources.AssemblyOutsideLibTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.AssemblyOutsideLibDescription, new object[1] { target }), AnalysisResources.AssemblyOutsideLibSolution);
	}
}
