using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet.Analysis.Rules;

internal class MisplacedTransformFileRule : IPackageRule
{
	private const string CodeTransformExtension = ".pp";

	private const string ConfigTransformExtension = ".transform";

	public IEnumerable<PackageIssue> Validate(IPackage package)
	{
		foreach (IPackageFile file in package.GetFiles())
		{
			string path = file.Path;
			if (path.EndsWith(".pp", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".transform", StringComparison.OrdinalIgnoreCase))
			{
				string contentDirectory = Constants.ContentDirectory;
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				if (!path.StartsWith(contentDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				{
					yield return CreatePackageIssueForMisplacedContent(path);
				}
			}
		}
	}

	private static PackageIssue CreatePackageIssueForMisplacedContent(string path)
	{
		return new PackageIssue(AnalysisResources.MisplacedTransformFileTitle, string.Format(CultureInfo.CurrentCulture, AnalysisResources.MisplacedTransformFileDescription, new object[1] { path }), AnalysisResources.MisplacedTransformFileSolution);
	}
}
