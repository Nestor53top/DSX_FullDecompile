using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DeltaCompressionDotNet.MsDelta;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Readers;
using Squirrel.Bsdiff;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal class DeltaPackageBuilder : IEnableLogger, IDeltaPackageBuilder
{
	private readonly string localAppDirectory;

	public DeltaPackageBuilder(string localAppDataOverride = null)
	{
		localAppDirectory = localAppDataOverride;
	}

	public ReleasePackage CreateDeltaPackage(ReleasePackage basePackage, ReleasePackage newPackage, string outputFile)
	{
		if (basePackage.Version > newPackage.Version)
		{
			throw new InvalidOperationException($"You cannot create a delta package based on version {basePackage.Version} as it is a later version than {newPackage.Version}");
		}
		if (basePackage.ReleasePackageFile == null)
		{
			throw new ArgumentException("The base package's release file is null", "basePackage");
		}
		if (!File.Exists(basePackage.ReleasePackageFile))
		{
			throw new FileNotFoundException("The base package release does not exist", basePackage.ReleasePackageFile);
		}
		if (!File.Exists(newPackage.ReleasePackageFile))
		{
			throw new FileNotFoundException("The new package release does not exist", newPackage.ReleasePackageFile);
		}
		string path = null;
		string path2 = null;
		using (Utility.WithTempDirectory(out path))
		{
			using (Utility.WithTempDirectory(out path2))
			{
				DirectoryInfo baseTempInfo = new DirectoryInfo(path);
				DirectoryInfo directoryInfo = new DirectoryInfo(path2);
				this.Log().Info("Extracting {0} and {1} into {2}", basePackage.ReleasePackageFile, newPackage.ReleasePackageFile, path2);
				Utility.ExtractZipToDirectory(basePackage.ReleasePackageFile, baseTempInfo.FullName).Wait();
				Utility.ExtractZipToDirectory(newPackage.ReleasePackageFile, directoryInfo.FullName).Wait();
				Dictionary<string, string> baseFileListing = baseTempInfo.GetAllFilesRecursively().Where(delegate(FileInfo x)
				{
					string text = x.FullName.ToLowerInvariant();
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					return text.Contains("lib" + directorySeparatorChar);
				}).ToDictionary((FileInfo k) => k.FullName.Replace(baseTempInfo.FullName, ""), (FileInfo v) => v.FullName);
				foreach (FileInfo item in directoryInfo.GetDirectories().First((DirectoryInfo x) => x.Name.ToLowerInvariant() == "lib").GetAllFilesRecursively())
				{
					createDeltaForSingleFile(item, directoryInfo, baseFileListing);
				}
				ReleasePackage.addDeltaFilesToContentTypes(directoryInfo.FullName);
				Utility.CreateZipFromDirectory(outputFile, directoryInfo.FullName).Wait();
			}
		}
		return new ReleasePackage(outputFile);
	}

	public ReleasePackage ApplyDeltaPackage(ReleasePackage basePackage, ReleasePackage deltaPackage, string outputFile)
	{
		return ApplyDeltaPackage(basePackage, deltaPackage, outputFile, delegate
		{
		});
	}

	public ReleasePackage ApplyDeltaPackage(ReleasePackage basePackage, ReleasePackage deltaPackage, string outputFile, Action<int> progress)
	{
		string deltaPath;
		using (Utility.WithTempDirectory(out deltaPath, localAppDirectory))
		{
			string workingPath;
			using (Utility.WithTempDirectory(out workingPath, localAppDirectory))
			{
				ExtractionOptions options = new ExtractionOptions
				{
					ExtractFullPath = true,
					Overwrite = true,
					PreserveFileTime = true
				};
				using (ZipArchive zipArchive = ZipArchive.Open(deltaPackage.InputPackageFile))
				{
					using IReader reader = zipArchive.ExtractAllEntries();
					reader.WriteAllToDirectory(deltaPath, options);
				}
				progress(25);
				using (ZipArchive zipArchive2 = ZipArchive.Open(basePackage.InputPackageFile))
				{
					using IReader reader2 = zipArchive2.ExtractAllEntries();
					reader2.WriteAllToDirectory(workingPath, options);
				}
				progress(50);
				List<string> pathsVisited = new List<string>();
				string[] deltaPathRelativePaths = new DirectoryInfo(deltaPath).GetAllFilesRecursively().Select(delegate(FileInfo x)
				{
					string fullName = x.FullName;
					string text = deltaPath;
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					return fullName.Replace(text + directorySeparatorChar, "");
				}).ToArray();
				(from x in deltaPathRelativePaths
					where x.StartsWith("lib", StringComparison.InvariantCultureIgnoreCase)
					where !x.EndsWith(".shasum", StringComparison.InvariantCultureIgnoreCase)
					where !x.EndsWith(".diff", StringComparison.InvariantCultureIgnoreCase) || !Enumerable.Contains(deltaPathRelativePaths, x.Replace(".diff", ".bsdiff"))
					select x).ForEach(delegate(string file)
				{
					pathsVisited.Add(Regex.Replace(file, "\\.(bs)?diff$", "").ToLowerInvariant());
					applyDiffToFile(deltaPath, file, workingPath);
				});
				progress(75);
				(from x in new DirectoryInfo(workingPath).GetAllFilesRecursively().Select(delegate(FileInfo x)
					{
						string fullName = x.FullName;
						string text = workingPath;
						char directorySeparatorChar = Path.DirectorySeparatorChar;
						return fullName.Replace(text + directorySeparatorChar, "").ToLowerInvariant();
					})
					where x.StartsWith("lib", StringComparison.InvariantCultureIgnoreCase) && !pathsVisited.Contains(x)
					select x).ForEach(delegate(string x)
				{
					this.Log().Info("{0} was in old package but not in new one, deleting", x);
					File.Delete(Path.Combine(workingPath, x));
				});
				progress(80);
				deltaPathRelativePaths.Where((string x) => !x.StartsWith("lib", StringComparison.InvariantCultureIgnoreCase)).ForEach(delegate(string x)
				{
					this.Log().Info("Updating metadata file: {0}", x);
					File.Copy(Path.Combine(deltaPath, x), Path.Combine(workingPath, x), overwrite: true);
				});
				this.Log().Info("Repacking into full package: {0}", outputFile);
				using (ZipArchive zipArchive3 = ZipArchive.Create())
				{
					using FileStream stream = File.OpenWrite(outputFile);
					zipArchive3.DeflateCompressionLevel = CompressionLevel.BestSpeed;
					zipArchive3.AddAllFromDirectory(workingPath);
					zipArchive3.SaveTo(stream);
				}
				progress(100);
			}
		}
		return new ReleasePackage(outputFile);
	}

	private void createDeltaForSingleFile(FileInfo targetFile, DirectoryInfo workingDirectory, Dictionary<string, string> baseFileListing)
	{
		string text = targetFile.FullName.Replace(workingDirectory.FullName, "");
		if (!baseFileListing.ContainsKey(text))
		{
			this.Log().Info("{0} not found in base package, marking as new", text);
			return;
		}
		byte[] oldData = File.ReadAllBytes(baseFileListing[text]);
		byte[] array = File.ReadAllBytes(targetFile.FullName);
		if (bytesAreIdentical(oldData, array))
		{
			this.Log().Info("{0} hasn't changed, writing dummy file", text);
			File.Create(targetFile.FullName + ".diff").Dispose();
			File.Create(targetFile.FullName + ".shasum").Dispose();
			targetFile.Delete();
			return;
		}
		this.Log().Info("Delta patching {0} => {1}", baseFileListing[text], targetFile.FullName);
		MsDeltaCompression msDeltaCompression = new MsDeltaCompression();
		if (!targetFile.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) && !targetFile.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) && !targetFile.Extension.Equals(".node", StringComparison.OrdinalIgnoreCase))
		{
			goto IL_0140;
		}
		try
		{
			msDeltaCompression.CreateDelta(baseFileListing[text], targetFile.FullName, targetFile.FullName + ".diff");
		}
		catch (Exception)
		{
			this.Log().Warn("We couldn't create a delta for {0}, attempting to create bsdiff", targetFile.Name);
			goto IL_0140;
		}
		goto IL_01d7;
		IL_01d7:
		ReleaseEntry releaseEntry = ReleaseEntry.GenerateFromFile(new MemoryStream(array), targetFile.Name + ".shasum");
		File.WriteAllText(targetFile.FullName + ".shasum", releaseEntry.EntryAsString, Encoding.UTF8);
		targetFile.Delete();
		return;
		IL_0140:
		try
		{
			using FileStream output = File.Create(targetFile.FullName + ".bsdiff");
			BinaryPatchUtility.Create(oldData, array, output);
			File.WriteAllText(targetFile.FullName + ".diff", "1");
		}
		catch (Exception exception)
		{
			this.Log().WarnException($"We really couldn't create a delta for {targetFile.Name}", exception);
			Utility.DeleteFileHarder(targetFile.FullName + ".bsdiff", ignoreIfFails: true);
			Utility.DeleteFileHarder(targetFile.FullName + ".diff", ignoreIfFails: true);
			return;
		}
		goto IL_01d7;
	}

	private void applyDiffToFile(string deltaPath, string relativeFilePath, string workingDirectory)
	{
		string inputFile = Path.Combine(deltaPath, relativeFilePath);
		string text = Path.Combine(workingDirectory, Regex.Replace(relativeFilePath, "\\.(bs)?diff$", ""));
		string path = null;
		Utility.WithTempFile(out path, localAppDirectory);
		try
		{
			if (new FileInfo(inputFile).Length == 0L)
			{
				this.Log().Info("{0} exists unchanged, skipping", relativeFilePath);
				return;
			}
			if (relativeFilePath.EndsWith(".bsdiff", StringComparison.InvariantCultureIgnoreCase))
			{
				using (FileStream output = File.OpenWrite(path))
				{
					using FileStream input = File.OpenRead(text);
					this.Log().Info("Applying BSDiff to {0}", relativeFilePath);
					BinaryPatchUtility.Apply(input, () => File.OpenRead(inputFile), output);
				}
				verifyPatchedFile(relativeFilePath, inputFile, path);
			}
			else if (relativeFilePath.EndsWith(".diff", StringComparison.InvariantCultureIgnoreCase))
			{
				this.Log().Info("Applying MSDiff to {0}", relativeFilePath);
				new MsDeltaCompression().ApplyDelta(inputFile, text, path);
				verifyPatchedFile(relativeFilePath, inputFile, path);
			}
			else
			{
				using FileStream destination = File.OpenWrite(path);
				using FileStream fileStream = File.OpenRead(inputFile);
				this.Log().Info("Adding new file: {0}", relativeFilePath);
				fileStream.CopyTo(destination);
			}
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			DirectoryInfo parent = Directory.GetParent(text);
			if (!parent.Exists)
			{
				parent.Create();
			}
			File.Move(path, text);
		}
		finally
		{
			if (File.Exists(path))
			{
				Utility.DeleteFileHarder(path, ignoreIfFails: true);
			}
		}
	}

	private void verifyPatchedFile(string relativeFilePath, string inputFile, string tempTargetFile)
	{
		ReleaseEntry releaseEntry = ReleaseEntry.ParseReleaseEntry(File.ReadAllText(Regex.Replace(inputFile, "\\.(bs)?diff$", ".shasum"), Encoding.UTF8));
		ReleaseEntry releaseEntry2 = ReleaseEntry.GenerateFromFile(tempTargetFile);
		if (releaseEntry.Filesize != releaseEntry2.Filesize)
		{
			this.Log().Warn("Patched file {0} has incorrect size, expected {1}, got {2}", relativeFilePath, releaseEntry.Filesize, releaseEntry2.Filesize);
			throw new ChecksumFailedException
			{
				Filename = relativeFilePath
			};
		}
		if (releaseEntry.SHA1 != releaseEntry2.SHA1)
		{
			this.Log().Warn("Patched file {0} has incorrect SHA1, expected {1}, got {2}", relativeFilePath, releaseEntry.SHA1, releaseEntry2.SHA1);
			throw new ChecksumFailedException
			{
				Filename = relativeFilePath
			};
		}
	}

	private bool bytesAreIdentical(byte[] oldData, byte[] newData)
	{
		if (oldData == null || newData == null)
		{
			return oldData == newData;
		}
		if (oldData.LongLength != newData.LongLength)
		{
			return false;
		}
		for (long num = 0L; num < newData.LongLength; num++)
		{
			if (oldData[num] != newData[num])
			{
				return false;
			}
		}
		return true;
	}
}
