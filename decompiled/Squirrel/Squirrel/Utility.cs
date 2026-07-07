using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NuGet;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal static class Utility
{
	[Flags]
	private enum MoveFileFlags
	{
		MOVEFILE_REPLACE_EXISTING = 1,
		MOVEFILE_COPY_ALLOWED = 2,
		MOVEFILE_DELAY_UNTIL_REBOOT = 4,
		MOVEFILE_WRITE_THROUGH = 8,
		MOVEFILE_CREATE_HARDLINK = 0x10,
		MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20
	}

	private static Lazy<string> directoryChars = new Lazy<string>(() => "abcdefghijklmnopqrstuvwxyz" + Enumerable.Range(944, 79).Concat(Enumerable.Range(1024, 255)).Aggregate(new StringBuilder(), delegate(StringBuilder acc, int x)
	{
		acc.Append(char.ConvertFromUtf32(x));
		return acc;
	})
		.ToString());

	private static readonly string[] peExtensions = new string[3] { ".exe", ".dll", ".node" };

	private static IFullLogger logger;

	public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

	public static readonly Guid UrlNamespace = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

	public static readonly Guid IsoOidNamespace = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

	public static string RemoveByteOrderMarkerIfPresent(string content)
	{
		if (!string.IsNullOrEmpty(content))
		{
			return RemoveByteOrderMarkerIfPresent(Encoding.UTF8.GetBytes(content));
		}
		return string.Empty;
	}

	public static string RemoveByteOrderMarkerIfPresent(byte[] content)
	{
		byte[] array = new byte[0];
		if (content != null)
		{
			Func<byte[], byte[], bool> func = (byte[] bom, byte[] src) => src.Length >= bom.Length && !bom.Where((byte chr, int index) => src[index] != chr).Any();
			byte[] array2 = new byte[4] { 0, 0, 254, 255 };
			byte[] array3 = new byte[4] { 255, 254, 0, 0 };
			byte[] array4 = new byte[2] { 254, 255 };
			byte[] array5 = new byte[2] { 255, 254 };
			byte[] array6 = new byte[3] { 239, 187, 191 };
			array = (func(array2, content) ? new byte[content.Length - array2.Length] : (func(array3, content) ? new byte[content.Length - array3.Length] : (func(array4, content) ? new byte[content.Length - array4.Length] : (func(array5, content) ? new byte[content.Length - array5.Length] : ((!func(array6, content)) ? content : new byte[content.Length - array6.Length])))));
		}
		if (array.Length != 0)
		{
			Buffer.BlockCopy(content, content.Length - array.Length, array, 0, array.Length);
		}
		return Encoding.UTF8.GetString(array);
	}

	public static IEnumerable<FileInfo> GetAllFilesRecursively(this DirectoryInfo rootPath)
	{
		return rootPath.EnumerateFiles("*", SearchOption.AllDirectories);
	}

	public static IEnumerable<string> GetAllFilePathsRecursively(string rootPath)
	{
		return Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories);
	}

	public static string CalculateFileSHA1(string filePath)
	{
		using FileStream file = File.OpenRead(filePath);
		return CalculateStreamSHA1(file);
	}

	public static string CalculateStreamSHA1(Stream file)
	{
		using SHA1 sHA = SHA1.Create();
		return BitConverter.ToString(sHA.ComputeHash(file)).Replace("-", string.Empty);
	}

	public static WebClient CreateWebClient()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		WebClient webClient = new WebClient();
		IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
		if (defaultWebProxy != null)
		{
			defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
			webClient.Proxy = defaultWebProxy;
		}
		return webClient;
	}

	public static async Task CopyToAsync(string from, string to)
	{
		if (!File.Exists(from))
		{
			Log().Warn("The file {0} does not exist", from);
			return;
		}
		await Task.Run(delegate
		{
			File.Copy(from, to, overwrite: true);
		});
	}

	public static void Retry(this Action block, int retries = 2)
	{
		Retry(delegate
		{
			block();
			return (object)null;
		}, retries);
	}

	public static T Retry<T>(this Func<T> block, int retries = 2)
	{
		while (true)
		{
			try
			{
				return block();
			}
			catch (Exception)
			{
				if (retries == 0)
				{
					throw;
				}
				retries--;
				Thread.Sleep(250);
			}
		}
	}

	public static Task<Tuple<int, string>> InvokeProcessAsync(string fileName, string arguments, CancellationToken ct, string workingDirectory = "")
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
		if (Environment.OSVersion.Platform != PlatformID.Win32NT && fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			processStartInfo = new ProcessStartInfo("wine", fileName + " " + arguments);
		}
		processStartInfo.UseShellExecute = false;
		processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		processStartInfo.ErrorDialog = false;
		processStartInfo.CreateNoWindow = true;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.RedirectStandardError = true;
		processStartInfo.WorkingDirectory = workingDirectory;
		return InvokeProcessAsync(processStartInfo, ct);
	}

	public static async Task<Tuple<int, string>> InvokeProcessAsync(ProcessStartInfo psi, CancellationToken ct)
	{
		Process pi = Process.Start(psi);
		await Task.Run(delegate
		{
			while (!ct.IsCancellationRequested)
			{
				if (pi.WaitForExit(2000))
				{
					return;
				}
			}
			if (ct.IsCancellationRequested)
			{
				pi.Kill();
				ct.ThrowIfCancellationRequested();
			}
		});
		string text = await pi.StandardOutput.ReadToEndAsync();
		if (string.IsNullOrWhiteSpace(text) || pi.ExitCode != 0)
		{
			string text2 = text;
			text = text2 + "\n" + await pi.StandardError.ReadToEndAsync();
			if (string.IsNullOrWhiteSpace(text))
			{
				text = string.Empty;
			}
		}
		return Tuple.Create(pi.ExitCode, text.Trim());
	}

	public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> body, int degreeOfParallelism = 4)
	{
		return source.ForEachAsync((T x) => Task.Run(delegate
		{
			body(x);
		}), degreeOfParallelism);
	}

	public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body, int degreeOfParallelism = 4)
	{
		return Task.WhenAll(from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
			select Task.Run(async delegate
			{
				using (partition)
				{
					while (partition.MoveNext())
					{
						await body(partition.Current);
					}
				}
			}));
	}

	internal static string tempNameForIndex(int index, string prefix)
	{
		if (index < directoryChars.Value.Length)
		{
			return prefix + directoryChars.Value[index];
		}
		return prefix + directoryChars.Value[index % directoryChars.Value.Length] + tempNameForIndex(index / directoryChars.Value.Length, "");
	}

	public static DirectoryInfo GetTempDirectory(string localAppDirectory)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetEnvironmentVariable("SQUIRREL_TEMP") ?? Path.Combine(localAppDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SquirrelTemp"));
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		return directoryInfo;
	}

	public static IDisposable WithTempDirectory(out string path, string localAppDirectory = null)
	{
		DirectoryInfo tempDirectory = GetTempDirectory(localAppDirectory);
		DirectoryInfo tempDir = null;
		foreach (string item in from x in Enumerable.Range(0, 1048576)
			select tempNameForIndex(x, "temp"))
		{
			string path2 = Path.Combine(tempDirectory.FullName, item);
			if (!File.Exists(path2) && !Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
				tempDir = new DirectoryInfo(path2);
				break;
			}
		}
		path = tempDir.FullName;
		return Disposable.Create(delegate
		{
			Task.Run(async delegate
			{
				await DeleteDirectory(tempDir.FullName);
			}).Wait();
		});
	}

	public static IDisposable WithTempFile(out string path, string localAppDirectory = null)
	{
		DirectoryInfo tempDirectory = GetTempDirectory(localAppDirectory);
		IEnumerable<string> enumerable = from x in Enumerable.Range(0, 1048576)
			select tempNameForIndex(x, "temp");
		path = "";
		foreach (string item in enumerable)
		{
			path = Path.Combine(tempDirectory.FullName, item);
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				break;
			}
		}
		string thePath = path;
		return Disposable.Create(delegate
		{
			File.Delete(thePath);
		});
	}

	public static async Task DeleteDirectory(string directoryPath)
	{
		Log().Debug("Starting to delete folder: {0}", directoryPath);
		if (!Directory.Exists(directoryPath))
		{
			Log().Warn("DeleteDirectory: does not exist - {0}", directoryPath);
			return;
		}
		string[] source = new string[0];
		try
		{
			source = Directory.GetFiles(directoryPath);
		}
		catch (UnauthorizedAccessException argument)
		{
			string message = $"The files inside {directoryPath} could not be read";
			Log().Warn(message, argument);
		}
		string[] source2 = new string[0];
		try
		{
			source2 = Directory.GetDirectories(directoryPath);
		}
		catch (UnauthorizedAccessException argument2)
		{
			string message2 = $"The directories inside {directoryPath} could not be read";
			Log().Warn(message2, argument2);
		}
		Task task = source.ForEachAsync(delegate(string file)
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		});
		Task task2 = source2.ForEachAsync(async delegate(string dir)
		{
			await DeleteDirectory(dir);
		});
		await Task.WhenAll(new Task[2] { task, task2 });
		Log().Debug("Now deleting folder: {0}", directoryPath);
		File.SetAttributes(directoryPath, FileAttributes.Normal);
		try
		{
			Directory.Delete(directoryPath, recursive: false);
		}
		catch (Exception exception)
		{
			string message3 = $"DeleteDirectory: could not delete - {directoryPath}";
			Log().ErrorException(message3, exception);
		}
	}

	public static string FindHelperExecutable(string toFind, IEnumerable<string> additionalDirs = null)
	{
		additionalDirs = additionalDirs ?? Enumerable.Empty<string>();
		IEnumerable<string> source = new string[1] { Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }.Concat(additionalDirs ?? Enumerable.Empty<string>());
		string text = ".\\" + toFind;
		return source.Select((string x) => Path.Combine(x, toFind)).FirstOrDefault((string x) => File.Exists(x)) ?? text;
	}

	private static string find7Zip()
	{
		if (ModeDetector.InUnitTestRunner())
		{
			string text = Path.Combine(new string[7]
			{
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "")),
				"..",
				"..",
				"..",
				"..",
				"vendor",
				"7zip"
			});
			return FindHelperExecutable("7z.exe", new string[1] { text });
		}
		return FindHelperExecutable("7z.exe");
	}

	public static async Task ExtractZipToDirectory(string zipFilePath, string outFolder)
	{
		string text = find7Zip();
		try
		{
			string fileName = text;
			string text2 = $"x \"{zipFilePath}\" -tzip -mmt on -aoa -y -o\"{outFolder}\" *";
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				fileName = "wine";
				text2 = text + " " + text2;
			}
			Tuple<int, string> tuple = await InvokeProcessAsync(fileName, text2, CancellationToken.None);
			if (tuple.Item1 != 0)
			{
				throw new Exception(tuple.Item2);
			}
		}
		catch (Exception ex)
		{
			Log().Error("Failed to extract file " + zipFilePath + " to " + outFolder + "\n" + ex.Message);
			throw;
		}
	}

	public static async Task CreateZipFromDirectory(string zipFilePath, string inFolder)
	{
		string text = find7Zip();
		try
		{
			string fileName = text;
			string text2 = $"a \"{zipFilePath}\" -tzip -aoa -y -mmt on *";
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				fileName = "wine";
				text2 = text + " " + text2;
			}
			Tuple<int, string> tuple = await InvokeProcessAsync(fileName, text2, CancellationToken.None, inFolder);
			if (tuple.Item1 != 0)
			{
				throw new Exception(tuple.Item2);
			}
		}
		catch (Exception ex)
		{
			Log().Error("Failed to extract file " + zipFilePath + " to " + inFolder + "\n" + ex.Message);
			throw;
		}
	}

	public static string AppDirForRelease(string rootAppDirectory, ReleaseEntry entry)
	{
		return Path.Combine(rootAppDirectory, "app-" + entry.Version.ToString());
	}

	public static string AppDirForVersion(string rootAppDirectory, SemanticVersion version)
	{
		return Path.Combine(rootAppDirectory, "app-" + version.ToString());
	}

	public static string PackageDirectoryForAppDir(string rootAppDirectory)
	{
		return Path.Combine(rootAppDirectory, "packages");
	}

	public static string LocalReleaseFileForAppDir(string rootAppDirectory)
	{
		return Path.Combine(PackageDirectoryForAppDir(rootAppDirectory), "RELEASES");
	}

	public static IEnumerable<ReleaseEntry> LoadLocalReleases(string localReleaseFile)
	{
		using StreamReader streamReader = new StreamReader(File.OpenRead(localReleaseFile), Encoding.UTF8);
		return ReleaseEntry.ParseReleaseFile(streamReader.ReadToEnd());
	}

	public static ReleaseEntry FindCurrentVersion(IEnumerable<ReleaseEntry> localReleases)
	{
		if (!localReleases.Any())
		{
			return null;
		}
		return localReleases.OrderByDescending((ReleaseEntry x) => x.Version).FirstOrDefault((ReleaseEntry x) => !x.IsDelta);
	}

	private static TAcc scan<T, TAcc>(this IEnumerable<T> This, TAcc initialValue, Func<TAcc, T, TAcc> accFunc)
	{
		TAcc val = initialValue;
		foreach (T item in This)
		{
			val = accFunc(val, item);
		}
		return val;
	}

	public static bool IsHttpUrl(string urlOrPath)
	{
		Uri result = null;
		if (!Uri.TryCreate(urlOrPath, UriKind.Absolute, out result))
		{
			return false;
		}
		if (!(result.Scheme == Uri.UriSchemeHttp))
		{
			return result.Scheme == Uri.UriSchemeHttps;
		}
		return true;
	}

	public static Uri AppendPathToUri(Uri uri, string path)
	{
		UriBuilder uriBuilder = new UriBuilder(uri);
		if (!uriBuilder.Path.EndsWith("/"))
		{
			uriBuilder.Path += "/";
		}
		uriBuilder.Path += path;
		return uriBuilder.Uri;
	}

	public static Uri EnsureTrailingSlash(Uri uri)
	{
		return AppendPathToUri(uri, "");
	}

	public static Uri AddQueryParamsToUri(Uri uri, IEnumerable<KeyValuePair<string, string>> newQuery)
	{
		NameValueCollection nameValueCollection = System.Web.HttpUtility.ParseQueryString(uri.Query);
		foreach (KeyValuePair<string, string> item in newQuery)
		{
			nameValueCollection[item.Key] = item.Value;
		}
		return new UriBuilder(uri)
		{
			Query = nameValueCollection.ToString()
		}.Uri;
	}

	public static void DeleteFileHarder(string path, bool ignoreIfFails = false)
	{
		try
		{
			Retry(delegate
			{
				File.Delete(path);
			});
		}
		catch (Exception exception)
		{
			if (ignoreIfFails)
			{
				return;
			}
			LogHost.Default.ErrorException("Really couldn't delete file: " + path, exception);
			throw;
		}
	}

	public static async Task DeleteDirectoryOrJustGiveUp(string dir)
	{
		try
		{
			await DeleteDirectory(dir);
		}
		catch
		{
			_ = $"Uninstall failed to delete dir '{dir}'";
		}
	}

	public static bool ExecutableUsesWin32Subsystem(string peImage)
	{
		using FileStream fileStream = new FileStream(peImage, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[4];
		fileStream.Seek(60L, SeekOrigin.Begin);
		fileStream.Read(array, 0, 4);
		int num = array[0];
		num |= array[1] << 8;
		num |= array[2] << 16;
		num |= array[3] << 24;
		byte[] array2 = new byte[24];
		fileStream.Seek(num, SeekOrigin.Begin);
		fileStream.Read(array2, 0, 24);
		byte[] array3 = new byte[4] { 80, 69, 0, 0 };
		for (int i = 0; i < 4; i++)
		{
			if (array2[i] != array3[i])
			{
				throw new Exception("File is not a PE image");
			}
		}
		byte[] array4 = new byte[2];
		fileStream.Seek(68L, SeekOrigin.Current);
		fileStream.Read(array4, 0, 2);
		return (array4[0] | (array4[1] << 8)) == 2;
	}

	public static bool FileIsLikelyPEImage(string name)
	{
		string ext = Path.GetExtension(name);
		return peExtensions.Any((string x) => ext.Equals(x, StringComparison.OrdinalIgnoreCase));
	}

	public static bool IsFileTopLevelInPackage(string fullName, string pkgPath)
	{
		string text = fullName.ToLowerInvariant();
		string oldValue = pkgPath.ToLowerInvariant();
		return text.Replace(oldValue, "").Split(new char[1] { Path.DirectorySeparatorChar }).Length == 4;
	}

	public static void LogIfThrows(this IFullLogger This, LogLevel level, string message, Action block)
	{
		try
		{
			block();
		}
		catch (Exception exception)
		{
			switch (level)
			{
			case LogLevel.Debug:
				This.DebugException(message ?? "", exception);
				break;
			case LogLevel.Info:
				This.InfoException(message ?? "", exception);
				break;
			case LogLevel.Warn:
				This.WarnException(message ?? "", exception);
				break;
			case LogLevel.Error:
				This.ErrorException(message ?? "", exception);
				break;
			}
			throw;
		}
	}

	public static async Task LogIfThrows(this IFullLogger This, LogLevel level, string message, Func<Task> block)
	{
		try
		{
			await block();
		}
		catch (Exception exception)
		{
			switch (level)
			{
			case LogLevel.Debug:
				This.DebugException(message ?? "", exception);
				break;
			case LogLevel.Info:
				This.InfoException(message ?? "", exception);
				break;
			case LogLevel.Warn:
				This.WarnException(message ?? "", exception);
				break;
			case LogLevel.Error:
				This.ErrorException(message ?? "", exception);
				break;
			}
			throw;
		}
	}

	public static async Task<T> LogIfThrows<T>(this IFullLogger This, LogLevel level, string message, Func<Task<T>> block)
	{
		try
		{
			return await block();
		}
		catch (Exception exception)
		{
			switch (level)
			{
			case LogLevel.Debug:
				This.DebugException(message ?? "", exception);
				break;
			case LogLevel.Info:
				This.InfoException(message ?? "", exception);
				break;
			case LogLevel.Warn:
				This.WarnException(message ?? "", exception);
				break;
			case LogLevel.Error:
				This.ErrorException(message ?? "", exception);
				break;
			}
			throw;
		}
	}

	public static void WarnIfThrows(this IEnableLogger This, Action block, string message = null)
	{
		This.Log().LogIfThrows(LogLevel.Warn, message, block);
	}

	public static Task WarnIfThrows(this IEnableLogger This, Func<Task> block, string message = null)
	{
		return This.Log().LogIfThrows(LogLevel.Warn, message, block);
	}

	public static Task<T> WarnIfThrows<T>(this IEnableLogger This, Func<Task<T>> block, string message = null)
	{
		return This.Log().LogIfThrows(LogLevel.Warn, message, block);
	}

	public static void ErrorIfThrows(this IEnableLogger This, Action block, string message = null)
	{
		This.Log().LogIfThrows(LogLevel.Error, message, block);
	}

	public static Task ErrorIfThrows(this IEnableLogger This, Func<Task> block, string message = null)
	{
		return This.Log().LogIfThrows(LogLevel.Error, message, block);
	}

	public static Task<T> ErrorIfThrows<T>(this IEnableLogger This, Func<Task<T>> block, string message = null)
	{
		return This.Log().LogIfThrows(LogLevel.Error, message, block);
	}

	private static IFullLogger Log()
	{
		return logger ?? (logger = SquirrelLocator.CurrentMutable.GetService<ILogManager>().GetLogger(typeof(Utility)));
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

	public static Guid CreateGuidFromHash(string text)
	{
		return CreateGuidFromHash(text, IsoOidNamespace);
	}

	public static Guid CreateGuidFromHash(byte[] data)
	{
		return CreateGuidFromHash(data, IsoOidNamespace);
	}

	public static Guid CreateGuidFromHash(string text, Guid namespaceId)
	{
		return CreateGuidFromHash(Encoding.UTF8.GetBytes(text), namespaceId);
	}

	public static Guid CreateGuidFromHash(byte[] nameBytes, Guid namespaceId)
	{
		byte[] array = namespaceId.ToByteArray();
		SwapByteOrder(array);
		byte[] hash;
		using (SHA1 sHA = SHA1.Create())
		{
			sHA.TransformBlock(array, 0, array.Length, null, 0);
			sHA.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
			hash = sHA.Hash;
		}
		byte[] array2 = new byte[16];
		Array.Copy(hash, 0, array2, 0, 16);
		array2[6] = (byte)((array2[6] & 0xF) | 0x50);
		array2[8] = (byte)((array2[8] & 0x3F) | 0x80);
		SwapByteOrder(array2);
		return new Guid(array2);
	}

	private static void SwapByteOrder(byte[] guid)
	{
		SwapBytes(guid, 0, 3);
		SwapBytes(guid, 1, 2);
		SwapBytes(guid, 4, 5);
		SwapBytes(guid, 6, 7);
	}

	private static void SwapBytes(byte[] guid, int left, int right)
	{
		byte b = guid[left];
		guid[left] = guid[right];
		guid[right] = b;
	}
}
