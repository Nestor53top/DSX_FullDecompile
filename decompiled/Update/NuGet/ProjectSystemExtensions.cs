using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NuGet.Resources;

namespace NuGet;

internal static class ProjectSystemExtensions
{
	public static void AddFiles(this IProjectSystem project, IEnumerable<IPackageFile> files, IDictionary<FileTransformExtensions, IPackageFileTransformer> fileTransformers)
	{
		List<IPackageFile> list = files.ToList();
		if (project is IComparer<IPackageFile> comparer)
		{
			list.Sort(comparer);
		}
		IBatchProcessor<string> batchProcessor = project as IBatchProcessor<string>;
		try
		{
			if (batchProcessor != null)
			{
				IEnumerable<string> batch = list.Select((IPackageFile file) => ResolvePath(fileTransformers, (FileTransformExtensions fte) => fte.InstallExtension, file.EffectivePath));
				batchProcessor.BeginProcessing(batch, PackageAction.Install);
			}
			foreach (IPackageFile item in list)
			{
				if (item.IsEmptyFolder())
				{
					continue;
				}
				IPackageFileTransformer transformer;
				string text = ResolveTargetPath(project, fileTransformers, (FileTransformExtensions fte) => fte.InstallExtension, item.EffectivePath, out transformer);
				if (project.IsSupportedFile(text))
				{
					string truncatedPath;
					if (transformer != null)
					{
						transformer.TransformFile(item, text, project);
					}
					else if (FindFileTransformer(fileTransformers, (FileTransformExtensions fte) => fte.UninstallExtension, item.EffectivePath, out truncatedPath) == null)
					{
						TryAddFile(project, text, item.GetStream);
					}
				}
			}
		}
		finally
		{
			batchProcessor?.EndProcessing();
		}
	}

	internal static void TryAddFile(IProjectSystem project, string path, Func<Stream> content)
	{
		if (project.FileExists(path) && project.FileExistsInProject(path))
		{
			string message = string.Format(CultureInfo.CurrentCulture, NuGetResources.FileConflictMessage, new object[2] { path, project.ProjectName });
			FileConflictResolution fileConflictResolution = project.Logger.ResolveFileConflict(message);
			if (fileConflictResolution == FileConflictResolution.Overwrite || fileConflictResolution == FileConflictResolution.OverwriteAll)
			{
				project.Logger.Log(MessageLevel.Info, NuGetResources.Info_OverwriteExistingFile, path);
				using Stream stream = content();
				project.AddFile(path, stream);
				return;
			}
			project.Logger.Log(MessageLevel.Info, NuGetResources.Warning_FileAlreadyExists, path);
			return;
		}
		using Stream stream2 = content();
		project.AddFile(path, stream2);
	}

	public static void DeleteFiles(this IProjectSystem project, IEnumerable<IPackageFile> files, IEnumerable<IPackage> otherPackages, IDictionary<FileTransformExtensions, IPackageFileTransformer> fileTransformers)
	{
		IPackageFileTransformer transformer;
		ILookup<string, IPackageFile> lookup = files.ToLookup((IPackageFile p) => Path.GetDirectoryName(ResolveTargetPath(project, fileTransformers, (FileTransformExtensions fte) => fte.UninstallExtension, p.EffectivePath, out transformer)));
		foreach (string item in from grouping in lookup
			from directory in FileSystemExtensions.GetDirectories(grouping.Key)
			orderby directory.Length descending
			select directory)
		{
			IEnumerable<IPackageFile> enumerable = (lookup.Contains(item) ? lookup[item] : Enumerable.Empty<IPackageFile>());
			if (!project.DirectoryExists(item))
			{
				continue;
			}
			IBatchProcessor<string> batchProcessor = project as IBatchProcessor<string>;
			try
			{
				if (batchProcessor != null)
				{
					IEnumerable<string> batch = enumerable.Select((IPackageFile packageFile) => ResolvePath(fileTransformers, (FileTransformExtensions fte) => fte.UninstallExtension, packageFile.EffectivePath));
					batchProcessor.BeginProcessing(batch, PackageAction.Uninstall);
				}
				foreach (IPackageFile file in enumerable)
				{
					if (file.IsEmptyFolder())
					{
						continue;
					}
					string text = ResolveTargetPath(project, fileTransformers, (FileTransformExtensions fte) => fte.UninstallExtension, file.EffectivePath, out transformer);
					if (!project.IsSupportedFile(text))
					{
						continue;
					}
					if (transformer != null)
					{
						IEnumerable<IPackageFile> matchingFiles = from p in otherPackages
							from otherFile in project.GetCompatibleItemsCore(p.GetContentFiles())
							where otherFile.EffectivePath.Equals(file.EffectivePath, StringComparison.OrdinalIgnoreCase)
							select otherFile;
						try
						{
							transformer.RevertFile(file, text, matchingFiles, project);
						}
						catch (Exception ex)
						{
							project.Logger.Log(MessageLevel.Warning, ex.Message);
						}
					}
					else
					{
						project.DeleteFileSafe(text, file.GetStream);
					}
				}
				if (!project.GetFilesSafe(item).Any() && !project.GetDirectoriesSafe(item).Any())
				{
					project.DeleteDirectorySafe(item, recursive: false);
				}
			}
			finally
			{
				batchProcessor?.EndProcessing();
			}
		}
	}

	public static bool TryGetCompatibleItems<T>(this IProjectSystem projectSystem, IEnumerable<T> items, out IEnumerable<T> compatibleItems) where T : IFrameworkTargetable
	{
		if (projectSystem == null)
		{
			throw new ArgumentNullException("projectSystem");
		}
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		return VersionUtility.TryGetCompatibleItems(projectSystem.TargetFramework, items, out compatibleItems);
	}

	internal static IEnumerable<T> GetCompatibleItemsCore<T>(this IProjectSystem projectSystem, IEnumerable<T> items) where T : IFrameworkTargetable
	{
		if (VersionUtility.TryGetCompatibleItems(projectSystem.TargetFramework, items, out var compatibleItems))
		{
			return compatibleItems;
		}
		return Enumerable.Empty<T>();
	}

	private static string ResolvePath(IDictionary<FileTransformExtensions, IPackageFileTransformer> fileTransformers, Func<FileTransformExtensions, string> extensionSelector, string effectivePath)
	{
		if (FindFileTransformer(fileTransformers, extensionSelector, effectivePath, out var truncatedPath) != null)
		{
			effectivePath = truncatedPath;
		}
		return effectivePath;
	}

	private static string ResolveTargetPath(IProjectSystem projectSystem, IDictionary<FileTransformExtensions, IPackageFileTransformer> fileTransformers, Func<FileTransformExtensions, string> extensionSelector, string effectivePath, out IPackageFileTransformer transformer)
	{
		transformer = FindFileTransformer(fileTransformers, extensionSelector, effectivePath, out var truncatedPath);
		if (transformer != null)
		{
			effectivePath = truncatedPath;
		}
		return projectSystem.ResolvePath(effectivePath);
	}

	private static IPackageFileTransformer FindFileTransformer(IDictionary<FileTransformExtensions, IPackageFileTransformer> fileTransformers, Func<FileTransformExtensions, string> extensionSelector, string effectivePath, out string truncatedPath)
	{
		foreach (FileTransformExtensions key in fileTransformers.Keys)
		{
			string text = extensionSelector(key);
			if (effectivePath.EndsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				truncatedPath = effectivePath.Substring(0, effectivePath.Length - text.Length);
				string fileName = Path.GetFileName(truncatedPath);
				if (!Constants.PackageReferenceFile.Equals(fileName, StringComparison.OrdinalIgnoreCase))
				{
					return fileTransformers[key];
				}
			}
		}
		truncatedPath = effectivePath;
		return null;
	}
}
