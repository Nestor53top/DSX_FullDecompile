using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Web.XmlTransform;
using NuGet.Resources;

namespace NuGet;

internal class XdtTransformer : IPackageFileTransformer
{
	public void TransformFile(IPackageFile file, string targetPath, IProjectSystem projectSystem)
	{
		PerformXdtTransform(file, targetPath, projectSystem);
	}

	public void RevertFile(IPackageFile file, string targetPath, IEnumerable<IPackageFile> matchingFiles, IProjectSystem projectSystem)
	{
		PerformXdtTransform(file, targetPath, projectSystem);
	}

	private static void PerformXdtTransform(IPackageFile file, string targetPath, IProjectSystem projectSystem)
	{
		if (!projectSystem.FileExists(targetPath))
		{
			return;
		}
		string transform = Preprocessor.Process(file, projectSystem);
		try
		{
			using XmlTransformation xmlTransformation = new XmlTransformation(transform, isTransformAFile: false, null);
			using XmlTransformableDocument xmlTransformableDocument = new XmlTransformableDocument();
			((XmlDocument)xmlTransformableDocument).PreserveWhitespace = true;
			using (Stream stream = projectSystem.OpenFile(targetPath))
			{
				((XmlDocument)xmlTransformableDocument).Load(stream);
			}
			if (!xmlTransformation.Apply((XmlDocument)(object)xmlTransformableDocument))
			{
				return;
			}
			using MemoryStream memoryStream = new MemoryStream();
			((XmlDocument)xmlTransformableDocument).Save((Stream)memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			using Stream destination = projectSystem.CreateFile(targetPath);
			memoryStream.CopyTo(destination);
		}
		catch (Exception ex)
		{
			throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.XdtError + " " + ex.Message, new object[2] { targetPath, projectSystem.ProjectName }), ex);
		}
	}
}
