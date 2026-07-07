using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NuGet;
using Squirrel.Json;
using Squirrel.SimpleSplat;

namespace Squirrel.Update;

internal class Program : IEnableLogger
{
	private static StartupOption opt;

	private static int consoleCreated;

	public static int Main(string[] args)
	{
		Program program = new Program();
		try
		{
			return program.main(args);
		}
		catch (Exception value)
		{
			Console.Error.WriteLine(value);
			return -1;
		}
	}

	private int main(string[] args)
	{
		try
		{
			opt = new StartupOption(args);
		}
		catch (Exception ex)
		{
			SetupLogLogger logger = new SetupLogLogger(saveInTemp: true, "OptionParsing")
			{
				Level = LogLevel.Info
			};
			try
			{
				SquirrelLocator.CurrentMutable.Register(() => logger, typeof(Squirrel.SimpleSplat.ILogger));
				logger.Write("Failed to parse command line options. " + ex.Message, LogLevel.Error);
			}
			finally
			{
				if (logger != null)
				{
					((IDisposable)logger).Dispose();
				}
			}
			throw;
		}
		bool saveInTemp = opt.updateAction == UpdateAction.Uninstall;
		SetupLogLogger logger2 = new SetupLogLogger(saveInTemp, opt.updateAction.ToString())
		{
			Level = LogLevel.Info
		};
		try
		{
			SquirrelLocator.CurrentMutable.Register(() => logger2, typeof(Squirrel.SimpleSplat.ILogger));
			try
			{
				return executeCommandLine(args);
			}
			catch (Exception ex2)
			{
				logger2.Write("Finished with unhandled exception: " + ex2, LogLevel.Fatal);
				throw;
			}
		}
		finally
		{
			if (logger2 != null)
			{
				((IDisposable)logger2).Dispose();
			}
		}
	}

	private int executeCommandLine(string[] args)
	{
		CancellationTokenSource animatedGifWindowToken = new CancellationTokenSource();
		using (Disposable.Create(delegate
		{
			animatedGifWindowToken.Cancel();
		}))
		{
			this.Log().Info("Starting Squirrel Updater: " + string.Join(" ", args));
			if (args.Any((string x) => x.StartsWith("/squirrel", StringComparison.OrdinalIgnoreCase)))
			{
				return 0;
			}
			if (opt.updateAction == UpdateAction.Unset)
			{
				ShowHelp();
				return -1;
			}
			switch (opt.updateAction)
			{
			case UpdateAction.Install:
			{
				ProgressSource progressSource = new ProgressSource();
				if (!opt.silentInstall)
				{
					AnimatedGifWindow.ShowWindow(TimeSpan.FromSeconds(4.0), animatedGifWindowToken.Token, progressSource);
				}
				Install(opt.silentInstall, progressSource, Path.GetFullPath(opt.target)).Wait();
				animatedGifWindowToken.Cancel();
				break;
			}
			case UpdateAction.Uninstall:
				Uninstall().Wait();
				break;
			case UpdateAction.Download:
				Console.WriteLine(Download(opt.target).Result);
				break;
			case UpdateAction.Update:
				Update(opt.target).Wait();
				break;
			case UpdateAction.CheckForUpdate:
				Console.WriteLine(CheckForUpdate(opt.target).Result);
				break;
			case UpdateAction.UpdateSelf:
				UpdateSelf().Wait();
				break;
			case UpdateAction.Shortcut:
				Shortcut(opt.target, opt.shortcutArgs, opt.processStartArgs, opt.setupIcon);
				break;
			case UpdateAction.Deshortcut:
				Deshortcut(opt.target, opt.shortcutArgs);
				break;
			case UpdateAction.ProcessStart:
				ProcessStart(opt.processStart, opt.processStartArgs, opt.shouldWait);
				break;
			case UpdateAction.Releasify:
				Releasify(opt.target, opt.releaseDir, opt.packagesDir, opt.bootstrapperExe, opt.backgroundGif, opt.signingParameters, opt.baseUrl, opt.setupIcon, !opt.noMsi, opt.packageAs64Bit, opt.frameworkVersion, !opt.noDelta);
				break;
			}
		}
		this.Log().Info("Finished Squirrel Updater");
		return 0;
	}

	public async Task Install(bool silentInstall, ProgressSource progressSource, string sourceDirectory = null)
	{
		sourceDirectory = sourceDirectory ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		string text = Path.Combine(sourceDirectory, "RELEASES");
		this.Log().Info("Starting install, writing to {0}", sourceDirectory);
		if (!File.Exists(text))
		{
			this.Log().Info("RELEASES doesn't exist, creating it at " + text);
			ReleaseEntry.WriteReleaseFile(from x in new DirectoryInfo(sourceDirectory).GetFiles()
				where x.Name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase)
				select ReleaseEntry.GenerateFromFile(x.FullName), text);
		}
		string packageName = ReleaseEntry.ParseReleaseFile(File.ReadAllText(text, Encoding.UTF8)).First().PackageName;
		UpdateManager mgr = new UpdateManager(sourceDirectory, packageName);
		try
		{
			this.Log().Info("About to install to: " + mgr.RootAppDirectory);
			if (Directory.Exists(mgr.RootAppDirectory))
			{
				this.Log().Warn("Install path {0} already exists, burning it to the ground", mgr.RootAppDirectory);
				mgr.KillAllExecutablesBelongingToPackage();
				await Task.Delay(500);
				await this.ErrorIfThrows(() => Utility.DeleteDirectory(mgr.RootAppDirectory), "Failed to remove existing directory on full install, is the app still running???");
				this.ErrorIfThrows(delegate
				{
					Utility.Retry(() => Directory.CreateDirectory(mgr.RootAppDirectory), 3);
				}, "Couldn't recreate app directory, perhaps Antivirus is blocking it");
			}
			Directory.CreateDirectory(mgr.RootAppDirectory);
			string updateTarget = Path.Combine(mgr.RootAppDirectory, "Update.exe");
			this.ErrorIfThrows(delegate
			{
				File.Copy(Assembly.GetExecutingAssembly().Location, updateTarget, overwrite: true);
			}, "Failed to copy Update.exe to " + updateTarget);
			await mgr.FullInstall(silentInstall, progressSource.Raise);
			await this.ErrorIfThrows(() => mgr.CreateUninstallerRegistryEntry(), "Failed to create uninstaller registry entry");
		}
		finally
		{
			if (mgr != null)
			{
				((IDisposable)mgr).Dispose();
			}
		}
	}

	public async Task Update(string updateUrl, string appName = null)
	{
		appName = appName ?? getAppNameFromDirectory();
		this.Log().Info("Starting update, downloading from " + updateUrl);
		UpdateManager mgr = new UpdateManager(updateUrl, appName);
		try
		{
			bool ignoreDeltaUpdates = false;
			this.Log().Info("About to update to: " + mgr.RootAppDirectory);
			while (true)
			{
				try
				{
					UpdateInfo updateInfo = await mgr.CheckForUpdate(ignoreDeltaUpdates, delegate(int x)
					{
						Console.WriteLine(x / 3);
					});
					await mgr.DownloadReleases(updateInfo.ReleasesToApply, delegate(int x)
					{
						Console.WriteLine(33 + x / 3);
					});
					await mgr.ApplyReleases(updateInfo, delegate(int x)
					{
						Console.WriteLine(66 + x / 3);
					});
				}
				catch (Exception exception)
				{
					if (ignoreDeltaUpdates)
					{
						this.Log().ErrorException("Really couldn't apply updates!", exception);
						throw;
					}
					this.Log().WarnException("Failed to apply updates, falling back to full updates", exception);
					ignoreDeltaUpdates = true;
					continue;
				}
				break;
			}
			Path.Combine(mgr.RootAppDirectory, "Update.exe");
			await this.ErrorIfThrows(() => mgr.CreateUninstallerRegistryEntry(), "Failed to create uninstaller registry entry");
		}
		finally
		{
			if (mgr != null)
			{
				((IDisposable)mgr).Dispose();
			}
		}
	}

	public async Task UpdateSelf()
	{
		waitForParentToExit();
		string src = Assembly.GetExecutingAssembly().Location;
		string updateDotExeForOurPackage = Path.Combine(Path.GetDirectoryName(src), "..", "Update.exe");
		await Task.Run(delegate
		{
			File.Copy(src, updateDotExeForOurPackage, overwrite: true);
		});
	}

	public async Task<string> Download(string updateUrl, string appName = null)
	{
		appName = appName ?? getAppNameFromDirectory();
		this.Log().Info("Fetching update information, downloading from " + updateUrl);
		using UpdateManager mgr = new UpdateManager(updateUrl, appName);
		UpdateInfo updateInfo = await mgr.CheckForUpdate(ignoreDeltaUpdates: false, delegate(int x)
		{
			Console.WriteLine(x / 3);
		});
		await mgr.DownloadReleases(updateInfo.ReleasesToApply, delegate(int x)
		{
			Console.WriteLine(33 + x / 3);
		});
		Dictionary<ReleaseEntry, string> releaseNotes = updateInfo.FetchReleaseNotes();
		return SimpleJson.SerializeObject(new
		{
			currentVersion = updateInfo.CurrentlyInstalledVersion.Version.ToString(),
			futureVersion = updateInfo.FutureReleaseEntry.Version.ToString(),
			releasesToApply = updateInfo.ReleasesToApply.Select((ReleaseEntry x) => new
			{
				version = x.Version.ToString(),
				releaseNotes = (releaseNotes.ContainsKey(x) ? releaseNotes[x] : "")
			}).ToArray()
		});
	}

	public async Task<string> CheckForUpdate(string updateUrl, string appName = null)
	{
		appName = appName ?? getAppNameFromDirectory();
		this.Log().Info("Fetching update information, downloading from " + updateUrl);
		using UpdateManager mgr = new UpdateManager(updateUrl, appName);
		UpdateInfo updateInfo = await mgr.CheckForUpdate(ignoreDeltaUpdates: false, delegate(int x)
		{
			Console.WriteLine(x);
		});
		Dictionary<ReleaseEntry, string> releaseNotes = updateInfo.FetchReleaseNotes();
		return SimpleJson.SerializeObject(new
		{
			currentVersion = updateInfo.CurrentlyInstalledVersion.Version.ToString(),
			futureVersion = updateInfo.FutureReleaseEntry.Version.ToString(),
			releasesToApply = updateInfo.ReleasesToApply.Select((ReleaseEntry x) => new
			{
				version = x.Version.ToString(),
				releaseNotes = (releaseNotes.ContainsKey(x) ? releaseNotes[x] : "")
			}).ToArray()
		});
	}

	public async Task Uninstall(string appName = null)
	{
		this.Log().Info("Starting uninstall for app: " + appName);
		appName = appName ?? getAppNameFromDirectory();
		using UpdateManager mgr = new UpdateManager("", appName);
		await mgr.FullUninstall();
		mgr.RemoveUninstallerRegistryEntry();
	}

	public void Releasify(string package, string targetDir = null, string packagesDir = null, string bootstrapperExe = null, string backgroundGif = null, string signingOpts = null, string baseUrl = null, string setupIcon = null, bool generateMsi = true, bool packageAs64Bit = false, string frameworkVersion = null, bool generateDeltas = true)
	{
		ensureConsole();
		if (baseUrl != null)
		{
			if (!Utility.IsHttpUrl(baseUrl))
			{
				throw new Exception($"Invalid --baseUrl '{baseUrl}'. A base URL must start with http or https and be a valid URI.");
			}
			if (!baseUrl.EndsWith("/"))
			{
				baseUrl += "/";
			}
		}
		targetDir = targetDir ?? Path.Combine(".", "Releases");
		packagesDir = packagesDir ?? ".";
		bootstrapperExe = bootstrapperExe ?? Path.Combine(".", "Setup.exe");
		if (!Directory.Exists(targetDir))
		{
			Directory.CreateDirectory(targetDir);
		}
		if (!File.Exists(bootstrapperExe))
		{
			bootstrapperExe = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Setup.exe");
		}
		this.Log().Info("Bootstrapper EXE found at:" + bootstrapperExe);
		DirectoryInfo directoryInfo = new DirectoryInfo(targetDir);
		File.Copy(package, Path.Combine(directoryInfo.FullName, Path.GetFileName(package)), overwrite: true);
		IEnumerable<FileInfo> enumerable = from x in directoryInfo.EnumerateFiles()
			where x.Name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase)
			where !x.Name.Contains("-delta") && !x.Name.Contains("-full")
			select x;
		List<string> list = new List<string>();
		string path = Path.Combine(directoryInfo.FullName, "RELEASES");
		List<ReleaseEntry> list2 = new List<ReleaseEntry>();
		if (File.Exists(path))
		{
			list2.AddRange(ReleaseEntry.ParseReleaseFile(File.ReadAllText(path, Encoding.UTF8)));
		}
		foreach (FileInfo item in enumerable)
		{
			this.Log().Info("Creating release package: " + item.FullName);
			ReleasePackage releasePackage = new ReleasePackage(item.FullName);
			releasePackage.CreateReleasePackage(Path.Combine(directoryInfo.FullName, releasePackage.SuggestedReleaseFileName), packagesDir, null, delegate(string pkgPath)
			{
				(from x in new DirectoryInfo(pkgPath).GetAllFilesRecursively()
					where x.Name.ToLowerInvariant().EndsWith(".exe")
					where !x.Name.ToLowerInvariant().Contains("squirrel.exe")
					where Utility.IsFileTopLevelInPackage(x.FullName, pkgPath)
					where Utility.ExecutableUsesWin32Subsystem(x.FullName)
					select x).ForEachAsync((FileInfo x) => createExecutableStubForExe(x.FullName)).Wait();
				if (signingOpts != null)
				{
					(from x in new DirectoryInfo(pkgPath).GetAllFilesRecursively()
						where Utility.FileIsLikelyPEImage(x.Name)
						select x).ForEachAsync(async delegate(FileInfo x)
					{
						if (isPEFileSigned(x.FullName))
						{
							this.Log().Info("{0} is already signed, skipping", x.FullName);
						}
						else
						{
							this.Log().Info("About to sign {0}", x.FullName);
							await signPEFile(x.FullName, signingOpts);
						}
					}, 1).Wait();
				}
			});
			list.Add(releasePackage.ReleasePackageFile);
			ReleasePackage previousRelease = ReleaseEntry.GetPreviousRelease(list2, releasePackage, targetDir);
			if (previousRelease != null && generateDeltas)
			{
				ReleasePackage releasePackage2 = new DeltaPackageBuilder().CreateDeltaPackage(previousRelease, releasePackage, Path.Combine(directoryInfo.FullName, releasePackage.SuggestedReleaseFileName.Replace("full", "delta")));
				list.Insert(0, releasePackage2.InputPackageFile);
			}
		}
		foreach (FileInfo item2 in enumerable)
		{
			File.Delete(item2.FullName);
		}
		List<ReleaseEntry> newReleaseEntries = list.Select((string packageFilename) => ReleaseEntry.GenerateFromFile(packageFilename, baseUrl)).ToList();
		List<ReleaseEntry> list3 = list2.Where((ReleaseEntry x) => !newReleaseEntries.Select((ReleaseEntry e) => e.Version).Contains(x.Version)).Concat(newReleaseEntries).ToList();
		ReleaseEntry.WriteReleaseFile(list3, path);
		string targetSetupExe = Path.Combine(directoryInfo.FullName, "Setup.exe");
		ReleaseEntry releaseEntry = (from x in list3.MaxBy((ReleaseEntry x) => x.Version)
			where !x.IsDelta
			select x).First();
		File.Copy(bootstrapperExe, targetSetupExe, overwrite: true);
		string result = createSetupEmbeddedZip(Path.Combine(directoryInfo.FullName, releaseEntry.Filename), directoryInfo.FullName, backgroundGif, signingOpts, setupIcon).Result;
		string fileName = Utility.FindHelperExecutable("WriteZipToSetup.exe");
		try
		{
			string arguments = $"\"{targetSetupExe}\" \"{result}\" \"--set-required-framework\" \"{frameworkVersion}\"";
			Tuple<int, string> result2 = Utility.InvokeProcessAsync(fileName, arguments, CancellationToken.None).Result;
			if (result2.Item1 != 0)
			{
				throw new Exception("Failed to write Zip to Setup.exe!\n\n" + result2.Item2);
			}
		}
		catch (Exception exception)
		{
			this.Log().ErrorException("Failed to update Setup.exe with new Zip file", exception);
		}
		finally
		{
			File.Delete(result);
		}
		Utility.Retry(delegate
		{
			setPEVersionInfoAndIcon(targetSetupExe, new ZipPackage(package), setupIcon).Wait();
		});
		if (signingOpts != null)
		{
			signPEFile(targetSetupExe, signingOpts).Wait();
		}
		if (generateMsi)
		{
			createMsiPackage(targetSetupExe, new ZipPackage(package), packageAs64Bit).Wait();
			if (signingOpts != null)
			{
				signPEFile(targetSetupExe.Replace(".exe", ".msi"), signingOpts).Wait();
			}
		}
	}

	public void Shortcut(string exeName, string shortcutArgs, string processStartArgs, string icon)
	{
		if (string.IsNullOrWhiteSpace(exeName))
		{
			ShowHelp();
			return;
		}
		string appNameFromDirectory = getAppNameFromDirectory();
		ShortcutLocation shortcutLocation = ShortcutLocation.StartMenu | ShortcutLocation.Desktop;
		ShortcutLocation? shortcutLocation2 = parseShortcutLocations(shortcutArgs);
		using UpdateManager updateManager = new UpdateManager("", appNameFromDirectory);
		updateManager.CreateShortcutsForExecutable(exeName, shortcutLocation2 ?? shortcutLocation, updateOnly: false, processStartArgs, icon);
	}

	public void Deshortcut(string exeName, string shortcutArgs)
	{
		if (string.IsNullOrWhiteSpace(exeName))
		{
			ShowHelp();
			return;
		}
		string appNameFromDirectory = getAppNameFromDirectory();
		ShortcutLocation shortcutLocation = ShortcutLocation.StartMenu | ShortcutLocation.Desktop;
		ShortcutLocation? shortcutLocation2 = parseShortcutLocations(shortcutArgs);
		using UpdateManager updateManager = new UpdateManager("", appNameFromDirectory);
		updateManager.RemoveShortcutsForExecutable(exeName, shortcutLocation2 ?? shortcutLocation);
	}

	public void ProcessStart(string exeName, string arguments, bool shouldWait)
	{
		if (string.IsNullOrWhiteSpace(exeName))
		{
			ShowHelp();
			return;
		}
		string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		string text = (from x in ReleaseEntry.ParseReleaseFile(File.ReadAllText(Utility.LocalReleaseFileForAppDir(appDir), Encoding.UTF8))
			orderby x.Version descending
			select x).SelectMany((ReleaseEntry x) => new string[2]
		{
			Utility.AppDirForRelease(appDir, x),
			Utility.AppDirForVersion(appDir, new SemanticVersion(x.Version.Version.Major, x.Version.Version.Minor, x.Version.Version.Build, ""))
		}).FirstOrDefault((string x) => Directory.Exists(x));
		FileInfo fileInfo = new FileInfo(Path.Combine(text, exeName.Replace("%20", " ")));
		this.Log().Info("Want to launch '{0}'", fileInfo);
		if (!fileInfo.FullName.StartsWith(text, StringComparison.Ordinal))
		{
			throw new ArgumentException();
		}
		if (!fileInfo.Exists)
		{
			this.Log().Error("File {0} doesn't exist in current release", fileInfo);
			throw new ArgumentException();
		}
		if (shouldWait)
		{
			waitForParentToExit();
		}
		try
		{
			this.Log().Info("About to launch: '{0}': {1}", fileInfo.FullName, arguments ?? "");
			Process.Start(new ProcessStartInfo(fileInfo.FullName, arguments ?? "")
			{
				WorkingDirectory = Path.GetDirectoryName(fileInfo.FullName)
			});
		}
		catch (Exception exception)
		{
			this.Log().ErrorException("Failed to start process", exception);
		}
	}

	public void ShowHelp()
	{
		ensureConsole();
		opt.WriteOptionDescriptions();
	}

	private void waitForParentToExit()
	{
		int parentProcessId = NativeMethods.GetParentProcessId();
		IntPtr intPtr = default(IntPtr);
		try
		{
			intPtr = NativeMethods.OpenProcess(ProcessAccess.Synchronize, bInheritHandle: false, parentProcessId);
			if (intPtr != IntPtr.Zero)
			{
				this.Log().Info("About to wait for parent PID {0}", parentProcessId);
				NativeMethods.WaitForSingleObject(intPtr, uint.MaxValue);
			}
			else
			{
				this.Log().Info("Parent PID {0} no longer valid - ignoring", parentProcessId);
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				NativeMethods.CloseHandle(intPtr);
			}
		}
	}

	private async Task<string> createSetupEmbeddedZip(string fullPackage, string releasesDir, string backgroundGif, string signingOpts, string setupIcon)
	{
		this.Log().Info("Building embedded zip file for Setup.exe");
		string tempPath;
		using (Utility.WithTempDirectory(out tempPath))
		{
			this.ErrorIfThrows(delegate
			{
				File.Copy(Assembly.GetEntryAssembly().Location.Replace("-Mono.exe", ".exe"), Path.Combine(tempPath, "Update.exe"));
				File.Copy(fullPackage, Path.Combine(tempPath, Path.GetFileName(fullPackage)));
			}, "Failed to write package files to temp dir: " + tempPath);
			if (!string.IsNullOrWhiteSpace(backgroundGif))
			{
				this.ErrorIfThrows(delegate
				{
					File.Copy(backgroundGif, Path.Combine(tempPath, "background.gif"));
				}, "Failed to write animated GIF to temp dir: " + tempPath);
			}
			if (!string.IsNullOrWhiteSpace(setupIcon))
			{
				this.ErrorIfThrows(delegate
				{
					File.Copy(setupIcon, Path.Combine(tempPath, "setupIcon.ico"));
				}, "Failed to write icon to temp dir: " + tempPath);
			}
			ReleaseEntry.WriteReleaseFile(new ReleaseEntry[1] { ReleaseEntry.GenerateFromFile(fullPackage) }, Path.Combine(tempPath, "RELEASES"));
			string target = Path.GetTempFileName();
			File.Delete(target);
			if (signingOpts != null)
			{
				await (from x in new DirectoryInfo(tempPath).EnumerateFiles()
					where x.Name.ToLowerInvariant().EndsWith(".exe")
					select x.FullName).ForEachAsync((string x) => signPEFile(x, signingOpts));
			}
			this.ErrorIfThrows(delegate
			{
				ZipFile.CreateFromDirectory(tempPath, target, CompressionLevel.Optimal, includeBaseDirectory: false);
			}, "Failed to create Zip file from directory: " + tempPath);
			return target;
		}
	}

	private static async Task signPEFile(string exePath, string signingOpts)
	{
		string exe = ".\\signtool.exe";
		if (!File.Exists(exe))
		{
			exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "signtool.exe");
			if (!File.Exists(exe))
			{
				exe = "signtool.exe";
			}
		}
		Tuple<int, string> tuple = await Utility.InvokeProcessAsync(exe, $"sign {signingOpts} \"{exePath}\"", CancellationToken.None);
		if (tuple.Item1 != 0)
		{
			string arg = new Regex("/p\\s+\\w+").Replace(signingOpts, "/p ********");
			throw new Exception($"Failed to sign, command invoked was: '{exe} sign {arg} {exePath}'");
		}
		Console.WriteLine(tuple.Item2);
	}

	private bool isPEFileSigned(string path)
	{
		try
		{
			return AuthenticodeTools.IsTrusted(path);
		}
		catch (Exception exception)
		{
			this.Log().ErrorException("Failed to determine signing status for " + path, exception);
			return false;
		}
	}

	private async Task createExecutableStubForExe(string fullName)
	{
		string text = Utility.FindHelperExecutable("StubExecutable.exe");
		string target = Path.Combine(Path.GetDirectoryName(fullName), Path.GetFileNameWithoutExtension(fullName) + "_ExecutionStub.exe");
		await Utility.CopyToAsync(text, target);
		await Utility.InvokeProcessAsync(Utility.FindHelperExecutable("WriteZipToSetup.exe"), $"--copy-stub-resources \"{fullName}\" \"{target}\"", CancellationToken.None);
	}

	private static async Task setPEVersionInfoAndIcon(string exePath, IPackage package, string iconPath = null)
	{
		string fullPath = Path.GetFullPath(exePath);
		string text = string.Join(",", package.Authors);
		Dictionary<string, string> source = new Dictionary<string, string>
		{
			{ "CompanyName", text },
			{
				"LegalCopyright",
				package.Copyright ?? ("Copyright © " + DateTime.Now.Year + " " + text)
			},
			{
				"FileDescription",
				package.Summary ?? package.Description ?? ("Installer for " + package.Id)
			},
			{
				"ProductName",
				package.Description ?? package.Summary ?? package.Id
			}
		};
		StringBuilder args = source.Aggregate(new StringBuilder("\"" + fullPath + "\""), delegate(StringBuilder acc, KeyValuePair<string, string> x)
		{
			acc.AppendFormat(" --set-version-string \"{0}\" \"{1}\"", x.Key, x.Value);
			return acc;
		});
		args.AppendFormat(" --set-file-version {0} --set-product-version {0}", package.Version.ToString());
		if (iconPath != null)
		{
			args.AppendFormat(" --set-icon \"{0}\"", Path.GetFullPath(iconPath));
		}
		string exe = Utility.FindHelperExecutable("rcedit.exe");
		Tuple<int, string> tuple = await Utility.InvokeProcessAsync(exe, args.ToString(), CancellationToken.None);
		if (tuple.Item1 != 0)
		{
			throw new Exception($"Failed to modify resources, command invoked was: '{exe} {args}'\n\nOutput was:\n{tuple.Item2}");
		}
		Console.WriteLine(tuple.Item2);
	}

	private static async Task createMsiPackage(string setupExe, IPackage package, bool packageAs64Bit)
	{
		string pathToWix = pathToWixTools();
		string setupExeDir = Path.GetDirectoryName(setupExe);
		string value = string.Join(",", package.Authors);
		int aNSICodePage = CultureInfo.GetCultureInfo(package.Language ?? "").TextInfo.ANSICodePage;
		string template = File.ReadAllText(Path.Combine(pathToWix, "template.wxs"));
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "Id", package.Id },
			{ "Title", package.Title },
			{ "Author", value },
			{
				"Version",
				Regex.Replace(package.Version.ToString(), "-.*$", "")
			},
			{
				"Summary",
				package.Summary ?? package.Description ?? package.Id
			},
			{
				"Codepage",
				$"{aNSICodePage}"
			},
			{
				"Platform",
				packageAs64Bit ? "x64" : "x86"
			},
			{
				"ProgramFilesFolder",
				packageAs64Bit ? "ProgramFiles64Folder" : "ProgramFilesFolder"
			},
			{
				"Win64YesNo",
				packageAs64Bit ? "yes" : "no"
			}
		};
		for (int i = 1; i <= 10; i++)
		{
			dictionary[$"IdAsGuid{i}"] = Utility.CreateGuidFromHash($"{package.Id}:{i}").ToString();
		}
		string contents = CopStache.Render(template, dictionary);
		string wxsTarget = Path.Combine(setupExeDir, "Setup.wxs");
		File.WriteAllText(wxsTarget, contents, Encoding.UTF8);
		string candleParams = string.Format("-nologo -ext WixNetFxExtension -out \"{0}\" \"{1}\"", wxsTarget.Replace(".wxs", ".wixobj"), wxsTarget);
		Tuple<int, string> tuple = await Utility.InvokeProcessAsync(Path.Combine(pathToWix, "candle.exe"), candleParams, CancellationToken.None, setupExeDir);
		if (tuple.Item1 != 0)
		{
			throw new Exception(string.Format("Failed to compile WiX template, command invoked was: '{0} {1}'\n\nOutput was:\n{2}", "candle.exe", candleParams, tuple.Item2));
		}
		string lightParams = string.Format("-ext WixNetFxExtension -sval -out \"{0}\" \"{1}\"", wxsTarget.Replace(".wxs", ".msi"), wxsTarget.Replace(".wxs", ".wixobj"));
		tuple = await Utility.InvokeProcessAsync(Path.Combine(pathToWix, "light.exe"), lightParams, CancellationToken.None, setupExeDir);
		if (tuple.Item1 != 0)
		{
			throw new Exception(string.Format("Failed to link WiX template, command invoked was: '{0} {1}'\n\nOutput was:\n{2}", "light.exe", lightParams, tuple.Item2));
		}
		await new string[3]
		{
			wxsTarget,
			wxsTarget.Replace(".wxs", ".wixobj"),
			wxsTarget.Replace(".wxs", ".wixpdb")
		}.ForEachAsync(delegate(string x)
		{
			Utility.DeleteFileHarder(x);
		});
	}

	private static string pathToWixTools()
	{
		string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		if (File.Exists(Path.Combine(directoryName, "candle.exe")))
		{
			return directoryName;
		}
		string path = Path.Combine(new string[7] { directoryName, "..", "..", "..", "vendor", "wix", "candle.exe" });
		if (File.Exists(path))
		{
			return Path.GetFullPath(path);
		}
		throw new Exception("WiX tools can't be found");
	}

	private static string getAppNameFromDirectory(string path = null)
	{
		path = path ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		return new DirectoryInfo(path).Name;
	}

	private static ShortcutLocation? parseShortcutLocations(string shortcutArgs)
	{
		ShortcutLocation? shortcutLocation = null;
		if (!string.IsNullOrWhiteSpace(shortcutArgs))
		{
			string[] array = shortcutArgs.Split(new char[1] { ',' });
			foreach (string value in array)
			{
				ShortcutLocation shortcutLocation2 = (ShortcutLocation)Enum.Parse(typeof(ShortcutLocation), value, ignoreCase: false);
				shortcutLocation = ((!shortcutLocation.HasValue) ? new ShortcutLocation?(shortcutLocation2) : (shortcutLocation | shortcutLocation2));
			}
		}
		return shortcutLocation;
	}

	private static void ensureConsole()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT && Interlocked.CompareExchange(ref consoleCreated, 1, 0) != 1)
		{
			if (!NativeMethods.AttachConsole(-1))
			{
				NativeMethods.AllocConsole();
			}
			NativeMethods.GetStdHandle(StandardHandles.STD_ERROR_HANDLE);
			NativeMethods.GetStdHandle(StandardHandles.STD_OUTPUT_HANDLE);
		}
	}
}
