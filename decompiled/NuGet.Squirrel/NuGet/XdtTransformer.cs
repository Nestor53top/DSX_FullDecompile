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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		if (!projectSystem.FileExists(targetPath))
		{
			return;
		}
		string text = Preprocessor.Process(file, projectSystem);
		try
		{
			XmlTransformation val = new XmlTransformation(text, false, (IXmlTransformationLogger)null);
			try
			{
				XmlTransformableDocument val2 = new XmlTransformableDocument();
				try
				{
					((XmlDocument)val2).PreserveWhitespace = true;
					using (Stream stream = projectSystem.OpenFile(targetPath))
					{
						((XmlDocument)val2).Load(stream);
					}
					if (!val.Apply((XmlDocument)(object)val2))
					{
						return;
					}
					using MemoryStream memoryStream = new MemoryStream();
					((XmlDocument)val2).Save((Stream)memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using Stream destination = projectSystem.CreateFile(targetPath);
					memoryStream.CopyTo(destination);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.XdtError + " " + ex.Message, new object[2] { targetPath, projectSystem.ProjectName }), ex);
		}
	}
}
