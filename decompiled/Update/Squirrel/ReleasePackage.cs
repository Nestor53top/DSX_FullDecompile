using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using MarkdownSharp;
using NuGet;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal class ReleasePackage : IEnableLogger, IReleasePackage
{
	public string InputPackageFile { get; protected set; }

	public string ReleasePackageFile { get; protected set; }

	public string SuggestedReleaseFileName
	{
		get
		{
			ZipPackage zipPackage = new ZipPackage(InputPackageFile);
			return $"{zipPackage.Id}-{zipPackage.Version}-full.nupkg";
		}
	}

	public SemanticVersion Version => InputPackageFile.ToSemanticVersion();

	public ReleasePackage(string inputPackageFile, bool isReleasePackage = false)
	{
		InputPackageFile = inputPackageFile;
		if (isReleasePackage)
		{
			ReleasePackageFile = inputPackageFile;
		}
	}

	public string CreateReleasePackage(string outputFile, string packagesRootDir = null, Func<string, string> releaseNotesProcessor = null, Action<string> contentsPostProcessHook = null)
	{
		releaseNotesProcessor = releaseNotesProcessor ?? ((Func<string, string>)((string x) => new Markdown().Transform(x)));
		if (ReleasePackageFile != null)
		{
			return ReleasePackageFile;
		}
		ZipPackage zipPackage = new ZipPackage(InputPackageFile);
		SemanticVersion value = null;
		if (!ModeDetector.InUnitTestRunner() && !SemanticVersion.TryParseStrict(zipPackage.Version.ToString(), out value))
		{
			throw new Exception($"Your package version is currently {zipPackage.Version.ToString()}, which is *not* SemVer-compatible, change this to be a SemVer version number");
		}
		IEnumerable<FrameworkName> supportedFrameworks = zipPackage.GetSupportedFrameworks();
		if (supportedFrameworks.Count() > 1)
		{
			StringBuilder arg = supportedFrameworks.Aggregate(new StringBuilder(), (StringBuilder sb, FrameworkName f) => sb.Append(f.ToString() + "; "));
			throw new InvalidOperationException($"The input package file {InputPackageFile} targets multiple platforms - {arg} - and cannot be transformed into a release package.");
		}
		if (!supportedFrameworks.Any())
		{
			throw new InvalidOperationException($"The input package file {InputPackageFile} targets no platform and cannot be transformed into a release package.");
		}
		FrameworkName frameworkName = supportedFrameworks.Single();
		this.Log().Info("Creating release package: {0} => {1}", InputPackageFile, outputFile);
		IPackage[] array = findAllDependentPackages(zipPackage, new LocalPackageRepository(packagesRootDir), null, frameworkName).ToArray();
		string path = null;
		using (Utility.WithTempDirectory(out path))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			extractZipWithEscaping(InputPackageFile, path).Wait();
			this.Log().Info("Extracting dependent packages: [{0}]", string.Join(",", array.Select((IPackage x) => x.Id)));
			extractDependentPackages(array, directoryInfo, frameworkName);
			string fullName = directoryInfo.GetFiles("*.nuspec").First().FullName;
			this.Log().Info("Removing unnecessary data");
			removeDependenciesFromPackageSpec(fullName);
			removeDeveloperDocumentation(directoryInfo);
			if (releaseNotesProcessor != null)
			{
				renderReleaseNotesMarkdown(fullName, releaseNotesProcessor);
			}
			addDeltaFilesToContentTypes(directoryInfo.FullName);
			contentsPostProcessHook?.Invoke(path);
			Utility.CreateZipFromDirectory(outputFile, path).Wait();
			ReleasePackageFile = outputFile;
			return ReleasePackageFile;
		}
	}

	private static Task extractZipWithEscaping(string zipFilePath, string outFolder)
	{
		return Task.Run(delegate
		{
			using ZipArchive zipArchive = ZipArchive.Open(zipFilePath);
			IReader reader = zipArchive.ExtractAllEntries();
			try
			{
				while (reader.MoveToNextEntry())
				{
					IEnumerable<string> values = from x in reader.Entry.Key.Split(new char[2] { '\\', '/' })
						select Uri.UnescapeDataString(x);
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					string decoded = string.Join(directorySeparatorChar.ToString(), values);
					Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(outFolder, decoded)));
					Utility.Retry(delegate
					{
						if (reader.Entry.IsDirectory)
						{
							Directory.CreateDirectory(Path.Combine(outFolder, decoded));
						}
						else
						{
							reader.WriteEntryToFile(Path.Combine(outFolder, decoded));
						}
					}, 5);
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Dispose();
				}
			}
		});
	}

	public static Task ExtractZipForInstall(string zipFilePath, string outFolder, string rootPackageFolder)
	{
		return ExtractZipForInstall(zipFilePath, outFolder, rootPackageFolder, delegate
		{
		});
	}

	public static Task ExtractZipForInstall(string zipFilePath, string outFolder, string rootPackageFolder, Action<int> progress)
	{
		Regex re = new Regex("lib[\\\\\\/][^\\\\\\/]*[\\\\\\/]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		return Task.Run(delegate
		{
			using (ZipArchive zipArchive = ZipArchive.Open(zipFilePath))
			{
				IReader reader = zipArchive.ExtractAllEntries();
				try
				{
					int count = zipArchive.Entries.Count;
					int num = 0;
					while (reader.MoveToNextEntry())
					{
						num++;
						double num2 = (double)num * 100.0 / (double)count;
						progress((int)num2);
						string[] value = reader.Entry.Key.Split(new char[2] { '\\', '/' });
						char directorySeparatorChar = Path.DirectorySeparatorChar;
						string input = string.Join(directorySeparatorChar.ToString(), value);
						if (re.IsMatch(input))
						{
							input = re.Replace(input, "", 1);
							string fullTargetFile = Path.Combine(outFolder, input);
							Directory.CreateDirectory(Path.GetDirectoryName(fullTargetFile));
							bool flag = false;
							if (!reader.Entry.IsDirectory && input.Contains("_ExecutionStub.exe"))
							{
								flag = true;
								fullTargetFile = Path.Combine(rootPackageFolder, Path.GetFileName(input).Replace("_ExecutionStub.exe", ".exe"));
								LogHost.Default.Info("Rigging execution stub for {0} to {1}", input, fullTargetFile);
							}
							try
							{
								Utility.Retry(delegate
								{
									if (reader.Entry.IsDirectory)
									{
										Directory.CreateDirectory(fullTargetFile);
									}
									else
									{
										reader.WriteEntryToFile(fullTargetFile);
									}
								}, 5);
							}
							catch (Exception exception)
							{
								if (!flag)
								{
									throw;
								}
								LogHost.Default.WarnException("Can't write execution stub, probably in use", exception);
							}
						}
					}
				}
				finally
				{
					if (reader != null)
					{
						reader.Dispose();
					}
				}
			}
			progress(100);
		});
	}

	private void extractDependentPackages(IEnumerable<IPackage> dependencies, DirectoryInfo tempPath, FrameworkName framework)
	{
		dependencies.ForEach(delegate(IPackage pkg)
		{
			this.Log().Info("Scanning {0}", pkg.Id);
			pkg.GetLibFiles().ForEach(delegate(IPackageFile file)
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(tempPath.FullName, file.Path));
				if (!VersionUtility.IsCompatible(framework, new FrameworkName[1] { file.TargetFramework }))
				{
					this.Log().Info("Ignoring {0} as the target framework is not compatible", fileInfo);
					return;
				}
				Directory.CreateDirectory(fileInfo.Directory.FullName);
				using FileStream destination = File.Create(fileInfo.FullName);
				this.Log().Info("Writing {0} to {1}", file.Path, fileInfo);
				file.GetStream().CopyTo(destination);
			});
		});
	}

	private void removeDeveloperDocumentation(DirectoryInfo expandedRepoPath)
	{
		(from x in expandedRepoPath.GetAllFilesRecursively()
			where x.Name.EndsWith(".dll", ignoreCase: true, CultureInfo.InvariantCulture)
			select new FileInfo(x.FullName.ToLowerInvariant().Replace(".dll", ".xml")) into x
			where x.Exists
			select x).ForEach(delegate(FileInfo x)
		{
			x.Delete();
		});
	}

	private void renderReleaseNotesMarkdown(string specPath, Func<string, string> releaseNotesProcessor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		XmlDocument val = new XmlDocument();
		val.Load(specPath);
		XmlElement val2 = ((IEnumerable)((XmlNode)((IEnumerable)((XmlNode)val.DocumentElement).ChildNodes).OfType<XmlElement>().First((XmlElement x) => ((XmlNode)x).Name.ToLowerInvariant() == "metadata")).ChildNodes).OfType<XmlElement>().FirstOrDefault((XmlElement x) => ((XmlNode)x).Name.ToLowerInvariant() == "releasenotes");
		if (val2 == null)
		{
			this.Log().Info("No release notes found in {0}", specPath);
			return;
		}
		((XmlNode)val2).InnerText = $"<![CDATA[\n{releaseNotesProcessor(((XmlNode)val2).InnerText)}\n]]>";
		val.Save(specPath);
	}

	private void removeDependenciesFromPackageSpec(string specPath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		XmlDocument val = new XmlDocument();
		val.Load(specPath);
		XmlNode firstChild = ((XmlNode)val.DocumentElement).FirstChild;
		XmlElement val2 = ((IEnumerable)firstChild.ChildNodes).OfType<XmlElement>().FirstOrDefault((XmlElement x) => ((XmlNode)x).Name.ToLowerInvariant() == "dependencies");
		if (val2 != null)
		{
			firstChild.RemoveChild((XmlNode)(object)val2);
		}
		val.Save(specPath);
	}

	internal IEnumerable<IPackage> findAllDependentPackages(IPackage package = null, IPackageRepository packageRepository = null, HashSet<string> packageCache = null, FrameworkName frameworkName = null)
	{
		package = package ?? new ZipPackage(InputPackageFile);
		packageCache = packageCache ?? new HashSet<string>();
		return package.DependencySets.Where((PackageDependencySet x) => x.TargetFramework == null || x.TargetFramework == frameworkName).SelectMany((PackageDependencySet x) => x.Dependencies).SelectMany(delegate(PackageDependency dependency)
		{
			IPackage package2 = matchPackage(packageRepository, dependency.Id, dependency.VersionSpec);
			if (package2 == null)
			{
				string message = string.Format("Couldn't find file for package in {1}: {0}", dependency.Id, packageRepository.Source);
				this.Log().Error(message);
				throw new Exception(message);
			}
			if (packageCache.Contains(package2.GetFullName()))
			{
				return Enumerable.Empty<IPackage>();
			}
			packageCache.Add(package2.GetFullName());
			return findAllDependentPackages(package2, packageRepository, packageCache, frameworkName).StartWith(package2).Distinct((IPackage y) => y.GetFullName());
		})
			.ToArray();
	}

	private IPackage matchPackage(IPackageRepository packageRepository, string id, IVersionSpec version)
	{
		return packageRepository.FindPackagesById(id).FirstOrDefault((IPackage x) => VersionComparer.Matches(version, x.Version));
	}

	internal static void addDeltaFilesToContentTypes(string rootDirectory)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		XmlDocument val = new XmlDocument();
		string text = Path.Combine(rootDirectory, "[Content_Types].xml");
		val.Load(text);
		ContentType.Merge(val);
		ContentType.Clean(val);
		using StreamWriter streamWriter = new StreamWriter(text, append: false, Encoding.UTF8);
		val.Save((TextWriter)streamWriter);
	}
}
