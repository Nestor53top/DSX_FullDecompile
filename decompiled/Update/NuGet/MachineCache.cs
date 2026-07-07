using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using NuGet.Resources;

namespace NuGet;

internal class MachineCache : LocalPackageRepository, IPackageCacheRepository, IPackageRepository
{
	private const int MaxPackages = 200;

	private const string NuGetCachePathEnvironmentVariable = "NuGetCachePath";

	private static readonly Lazy<MachineCache> _instance = new Lazy<MachineCache>(() => CreateDefault(GetCachePath));

	public static MachineCache Default => _instance.Value;

	internal MachineCache(IFileSystem fileSystem)
		: base(new DefaultPackagePathResolver(fileSystem), fileSystem, enableCaching: false)
	{
	}

	internal static MachineCache CreateDefault(Func<string> getCachePath)
	{
		IFileSystem fileSystem;
		try
		{
			string text = getCachePath();
			fileSystem = ((!string.IsNullOrEmpty(text)) ? ((IFileSystem)new PhysicalFileSystem(text)) : ((IFileSystem)NullFileSystem.Instance));
		}
		catch (SecurityException)
		{
			fileSystem = NullFileSystem.Instance;
		}
		return new MachineCache(fileSystem);
	}

	public override void AddPackage(IPackage package)
	{
		List<string> list = GetPackageFiles().ToList();
		if (list.Count >= 200)
		{
			List<string> files = list.OrderBy(base.FileSystem.GetLastAccessed).Take(list.Count - 160).ToList();
			TryClear(files);
		}
		string path = GetPackageFilePath(package);
		TryAct(delegate
		{
			if (base.FileSystem.FileExists(path))
			{
				return true;
			}
			string tempFile = GetTempFile(path);
			using (Stream stream = package.GetStream())
			{
				base.FileSystem.AddFile(tempFile, stream);
			}
			base.FileSystem.MoveFile(tempFile, path);
			return true;
		}, path);
	}

	private static string GetTempFile(string filename)
	{
		return filename + ".tmp";
	}

	public override bool Exists(string packageId, SemanticVersion version)
	{
		string packagePath = GetPackageFilePath(packageId, version);
		return TryAct(() => base.FileSystem.FileExists(packagePath), packagePath);
	}

	public bool InvokeOnPackage(string packageId, SemanticVersion version, Action<Stream> action)
	{
		if (base.FileSystem is NullFileSystem)
		{
			return false;
		}
		string packagePath = GetPackageFilePath(packageId, version);
		return TryAct(delegate
		{
			string tempFile = GetTempFile(packagePath);
			using (Stream stream = base.FileSystem.CreateFile(tempFile))
			{
				if (stream == null)
				{
					return false;
				}
				action(stream);
				if (stream == null || stream.Length == 0L)
				{
					return false;
				}
			}
			IPackage package = OpenPackage(base.FileSystem.GetFullPath(tempFile));
			packagePath = GetPackageFilePath(package.Id, package.Version);
			base.FileSystem.DeleteFile(packagePath);
			base.FileSystem.MoveFile(tempFile, packagePath);
			return true;
		}, packagePath);
	}

	protected override IPackage OpenPackage(string path)
	{
		//IL_0010: Expected O, but got Unknown
		try
		{
			return new OptimizedZipPackage(base.FileSystem, path);
		}
		catch (FileFormatException ex)
		{
			FileFormatException innerException = ex;
			throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorReadingPackage, new object[1] { path }), (Exception?)(object)innerException);
		}
	}

	public void Clear()
	{
		TryClear(GetPackageFiles().ToList());
	}

	private void TryClear(IEnumerable<string> files)
	{
		foreach (string packageFile in files)
		{
			TryAct(delegate
			{
				base.FileSystem.DeleteFileSafe(packageFile);
				return true;
			}, packageFile);
		}
	}

	protected override string GetPackageFilePath(IPackage package)
	{
		return Path.GetFileName(base.GetPackageFilePath(package));
	}

	protected override string GetPackageFilePath(string id, SemanticVersion version)
	{
		return Path.GetFileName(base.GetPackageFilePath(id, version));
	}

	internal static string GetCachePath()
	{
		return GetCachePath(Environment.GetEnvironmentVariable, Environment.GetFolderPath);
	}

	internal static string GetCachePath(Func<string, string> getEnvironmentVariable, Func<Environment.SpecialFolder, string> getFolderPath)
	{
		string text = getEnvironmentVariable("NuGetCachePath");
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		string text2 = getFolderPath(Environment.SpecialFolder.LocalApplicationData);
		if (string.IsNullOrEmpty(text2))
		{
			text2 = getEnvironmentVariable("LocalAppData");
		}
		if (string.IsNullOrEmpty(text2))
		{
			return null;
		}
		return Path.Combine(text2, "NuGet", "Cache");
	}

	private bool TryAct(Func<bool> action, string path)
	{
		try
		{
			string name = "Global\\" + EncryptionUtility.GenerateUniqueToken(base.FileSystem.GetFullPath(path) ?? path);
			using Mutex mutex = new Mutex(initiallyOwned: false, name);
			bool flag = false;
			try
			{
				try
				{
					flag = mutex.WaitOne(TimeSpan.FromMinutes(3.0));
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				return action();
			}
			finally
			{
				if (flag)
				{
					mutex.ReleaseMutex();
				}
			}
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
		return false;
	}
}
