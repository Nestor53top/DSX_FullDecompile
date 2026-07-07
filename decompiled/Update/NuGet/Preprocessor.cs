using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NuGet.Resources;

namespace NuGet;

internal class Preprocessor : IPackageFileTransformer
{
	public void TransformFile(IPackageFile file, string targetPath, IProjectSystem projectSystem)
	{
		ProjectSystemExtensions.TryAddFile(projectSystem, targetPath, () => Process(file, projectSystem).AsStream());
	}

	public void RevertFile(IPackageFile file, string targetPath, IEnumerable<IPackageFile> matchingFiles, IProjectSystem projectSystem)
	{
		FileSystemExtensions.DeleteFileSafe(streamFactory: () => Process(file, projectSystem).AsStream(), fileSystem: projectSystem, path: targetPath);
	}

	internal static string Process(IPackageFile file, IPropertyProvider propertyProvider)
	{
		using Stream stream = file.GetStream();
		return Process(stream, propertyProvider, throwIfNotFound: false);
	}

	public static string Process(Stream stream, IPropertyProvider propertyProvider, bool throwIfNotFound = true)
	{
		Tokenizer tokenizer = new Tokenizer(stream.ReadToEnd());
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			Token token = tokenizer.Read();
			if (token == null)
			{
				break;
			}
			if (token.Category == TokenCategory.Variable)
			{
				string value = ReplaceToken(token.Value, propertyProvider, throwIfNotFound);
				stringBuilder.Append(value);
			}
			else
			{
				stringBuilder.Append(token.Value);
			}
		}
		return stringBuilder.ToString();
	}

	private static string ReplaceToken(string propertyName, IPropertyProvider propertyProvider, bool throwIfNotFound)
	{
		dynamic propertyValue = propertyProvider.GetPropertyValue(propertyName);
		if (propertyValue == null && throwIfNotFound)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.TokenHasNoValue, new object[1] { propertyName }));
		}
		return propertyValue;
	}
}
