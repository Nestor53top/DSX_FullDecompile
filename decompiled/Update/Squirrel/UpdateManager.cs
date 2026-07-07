using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using NuGet;
using Squirrel.Json;
using Squirrel.Shell;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal sealed class UpdateManager : IUpdateManager, IDisposable, IEnableLogger
{
	internal class ApplyReleasesImpl : IEnableLogger
	{
		private readonly string rootAppDirectory;

		public ApplyReleasesImpl(string rootAppDirectory)
		{
			this.rootAppDirectory = rootAppDirectory;
		}

		public async Task<string> ApplyReleases(UpdateInfo updateInfo, bool silentInstall, bool attemptingFullInstall, Action<int> progress = null)
		{
			progress = progress ?? ((Action<int>)delegate
			{
			});
			progress(0);
			ReleaseEntry release = await createFullPackagesFromDeltas(updateInfo.ReleasesToApply, updateInfo.CurrentlyInstalledVersion, new ApplyReleasesProgress(updateInfo.ReleasesToApply.Count, delegate(int x)
			{
				progress(CalculateProgress(x, 0, 40));
			}));
			progress(40);
			if (release == null)
			{
				if (attemptingFullInstall)
				{
					this.Log().Info("No release to install, running the app");
					await invokePostInstall(updateInfo.CurrentlyInstalledVersion.Version, isInitialInstall: false, firstRunOnly: true, silentInstall);
				}
				progress(100);
				return getDirectoryForRelease(updateInfo.CurrentlyInstalledVersion.Version).FullName;
			}
			string ret = await this.ErrorIfThrows(() => installPackageToAppDir(updateInfo, release, delegate(int x)
			{
				progress(CalculateProgress(x, 40, 80));
			}), "Failed to install package to app dir");
			progress(80);
			List<ReleaseEntry> source = await this.ErrorIfThrows(() => updateLocalReleasesFile(), "Failed to update local releases file");
			progress(85);
			SemanticVersion newVersion = source.MaxBy((ReleaseEntry x) => x.Version).First().Version;
			executeSelfUpdate(newVersion);
			progress(90);
			await this.ErrorIfThrows(() => invokePostInstall(newVersion, attemptingFullInstall, firstRunOnly: false, silentInstall), "Failed to invoke post-install");
			progress(95);
			this.Log().Info("Starting fixPinnedExecutables");
			this.ErrorIfThrows(delegate
			{
				fixPinnedExecutables(updateInfo.FutureReleaseEntry.Version);
			});
			progress(96);
			this.Log().Info("Fixing up tray icons");
			TrayStateChanger trayFixer = new TrayStateChanger();
			DirectoryInfo directoryInfo = new DirectoryInfo(Utility.AppDirForRelease(rootAppDirectory, updateInfo.FutureReleaseEntry));
			List<string> allExes = (from x in directoryInfo.GetFiles("*.exe")
				select x.Name).ToList();
			this.ErrorIfThrows(delegate
			{
				trayFixer.RemoveDeadEntries(allExes, rootAppDirectory, updateInfo.FutureReleaseEntry.Version.ToString());
			});
			progress(97);
			unshimOurselves();
			progress(98);
			try
			{
				SemanticVersion currentVersion = ((updateInfo.CurrentlyInstalledVersion != null) ? updateInfo.CurrentlyInstalledVersion.Version : null);
				await cleanDeadVersions(currentVersion, newVersion);
			}
			catch (Exception exception)
			{
				this.Log().WarnException("Failed to clean dead versions, continuing anyways", exception);
			}
			progress(100);
			return ret;
		}

		public async Task FullUninstall()
		{
			DirectoryInfo directoryInfo = getReleases().MaxBy((DirectoryInfo x) => x.Name.ToSemanticVersion()).FirstOrDefault();
			this.Log().Info("Starting full uninstall");
			if (directoryInfo.Exists)
			{
				SemanticVersion version = directoryInfo.Name.ToSemanticVersion();
				try
				{
					List<string> allSquirrelAwareApps = SquirrelAwareExecutableDetector.GetAllSquirrelAwareApps(directoryInfo.FullName);
					if (isAppFolderDead(directoryInfo.FullName))
					{
						throw new Exception("App folder is dead, but we're trying to uninstall it?");
					}
					List<FileInfo> list = (from x in directoryInfo.EnumerateFiles()
						where x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
						where !x.Name.StartsWith("squirrel.", StringComparison.OrdinalIgnoreCase) && !x.Name.StartsWith("update.", StringComparison.OrdinalIgnoreCase)
						select x).ToList();
					if (allSquirrelAwareApps.Count > 0)
					{
						await allSquirrelAwareApps.ForEachAsync(async delegate(string exe)
						{
							using CancellationTokenSource cts = new CancellationTokenSource();
							cts.CancelAfter(10000);
							try
							{
								await Utility.InvokeProcessAsync(exe, $"--squirrel-uninstall {version}", cts.Token);
							}
							catch (Exception exception2)
							{
								this.Log().ErrorException("Failed to run cleanup hook, continuing: " + exe, exception2);
							}
						}, 1);
					}
					else
					{
						list.ForEach(delegate(FileInfo x)
						{
							RemoveShortcutsForExecutable(x.Name, ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
						});
					}
				}
				catch (Exception exception)
				{
					this.Log().WarnException("Failed to run pre-uninstall hooks, uninstalling anyways", exception);
				}
			}
			try
			{
				this.ErrorIfThrows(delegate
				{
					fixPinnedExecutables(new SemanticVersion(255, 255, 255, 255), removeAll: true);
				});
			}
			catch
			{
			}
			await this.ErrorIfThrows(() => Utility.DeleteDirectoryOrJustGiveUp(rootAppDirectory), "Failed to delete app directory: " + rootAppDirectory);
			if (!Directory.Exists(rootAppDirectory))
			{
				Directory.CreateDirectory(rootAppDirectory);
			}
			File.WriteAllText(Path.Combine(rootAppDirectory, ".dead"), " ");
		}

		public Dictionary<ShortcutLocation, ShellLink> GetShortcutsForExecutable(string exeName, ShortcutLocation locations, string programArguments)
		{
			this.Log().Info("About to create shortcuts for {0}, rootAppDir {1}", exeName, rootAppDirectory);
			ReleaseEntry releaseEntry = Utility.FindCurrentVersion(Utility.LoadLocalReleases(Utility.LocalReleaseFileForAppDir(rootAppDirectory)));
			ZipPackage zipPackage = new ZipPackage(Path.Combine(Utility.PackageDirectoryForAppDir(rootAppDirectory), releaseEntry.Filename));
			string text = Path.Combine(Utility.AppDirForRelease(rootAppDirectory, releaseEntry), exeName);
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text);
			Dictionary<ShortcutLocation, ShellLink> dictionary = new Dictionary<ShortcutLocation, ShellLink>();
			ShortcutLocation[] array = (ShortcutLocation[])Enum.GetValues(typeof(ShortcutLocation));
			foreach (ShortcutLocation shortcutLocation in array)
			{
				if (locations.HasFlag(shortcutLocation))
				{
					string argument = linkTargetForVersionInfo(shortcutLocation, zipPackage, versionInfo);
					string text2 = string.Format("com.squirrel.{0}.{1}", zipPackage.Id.Replace(" ", ""), exeName.Replace(".exe", "").Replace(" ", ""));
					string text3 = Utility.CreateGuidFromHash(text2).ToString();
					this.Log().Info("Creating shortcut for {0} => {1}", exeName, argument);
					this.Log().Info("appUserModelId: {0} | toastActivatorCLSID: {1}", text2, text3);
					string text4 = Path.Combine(rootAppDirectory, exeName);
					ShellLink shellLink = new ShellLink
					{
						Target = text4,
						IconPath = text4,
						IconIndex = 0,
						WorkingDirectory = Path.GetDirectoryName(text),
						Description = zipPackage.Description
					};
					if (!string.IsNullOrWhiteSpace(programArguments))
					{
						shellLink.Arguments += $" -a \"{programArguments}\"";
					}
					shellLink.SetAppUserModelId(text2);
					shellLink.SetToastActivatorCLSID(text3);
					dictionary.Add(shortcutLocation, shellLink);
				}
			}
			return dictionary;
		}

		public void CreateShortcutsForExecutable(string exeName, ShortcutLocation locations, bool updateOnly, string programArguments, string icon)
		{
			this.Log().Info("About to create shortcuts for {0}, rootAppDir {1}", exeName, rootAppDirectory);
			ReleaseEntry releaseEntry = Utility.FindCurrentVersion(Utility.LoadLocalReleases(Utility.LocalReleaseFileForAppDir(rootAppDirectory)));
			ZipPackage zf = new ZipPackage(Path.Combine(Utility.PackageDirectoryForAppDir(rootAppDirectory), releaseEntry.Filename));
			string exePath = Path.Combine(Utility.AppDirForRelease(rootAppDirectory, releaseEntry), exeName);
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(exePath);
			ShortcutLocation[] array = (ShortcutLocation[])Enum.GetValues(typeof(ShortcutLocation));
			foreach (ShortcutLocation shortcutLocation in array)
			{
				if (!locations.HasFlag(shortcutLocation))
				{
					continue;
				}
				string file = linkTargetForVersionInfo(shortcutLocation, zf, versionInfo);
				if (!File.Exists(file) && updateOnly)
				{
					this.Log().Warn("Wanted to update shortcut {0} but it appears user deleted it", file);
					continue;
				}
				this.Log().Info("Creating shortcut for {0} => {1}", exeName, file);
				ShellLink sl;
				this.ErrorIfThrows(delegate
				{
					Utility.Retry(delegate
					{
						File.Delete(file);
						string text = Path.Combine(rootAppDirectory, exeName);
						sl = new ShellLink
						{
							Target = text,
							IconPath = (icon ?? text),
							IconIndex = 0,
							WorkingDirectory = Path.GetDirectoryName(exePath),
							Description = zf.Description
						};
						if (!string.IsNullOrWhiteSpace(programArguments))
						{
							sl.Arguments += $" -a \"{programArguments}\"";
						}
						string text2 = string.Format("com.squirrel.{0}.{1}", zf.Id.Replace(" ", ""), exeName.Replace(".exe", "").Replace(" ", ""));
						string text3 = Utility.CreateGuidFromHash(text2).ToString();
						sl.SetAppUserModelId(text2);
						sl.SetToastActivatorCLSID(text3);
						this.Log().Info("About to save shortcut: {0} (target {1}, workingDir {2}, args {3}, toastActivatorCSLID {4})", file, sl.Target, sl.WorkingDirectory, sl.Arguments, text3);
						if (!ModeDetector.InUnitTestRunner())
						{
							sl.Save(file);
						}
					}, 4);
				}, "Can't write shortcut: " + file);
			}
			fixPinnedExecutables(zf.Version);
		}

		public void RemoveShortcutsForExecutable(string exeName, ShortcutLocation locations)
		{
			ReleaseEntry releaseEntry = Utility.FindCurrentVersion(Utility.LoadLocalReleases(Utility.LocalReleaseFileForAppDir(rootAppDirectory)));
			ZipPackage zipPackage = new ZipPackage(Path.Combine(Utility.PackageDirectoryForAppDir(rootAppDirectory), releaseEntry.Filename));
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(Utility.AppDirForRelease(rootAppDirectory, releaseEntry), exeName));
			ShortcutLocation[] array = (ShortcutLocation[])Enum.GetValues(typeof(ShortcutLocation));
			foreach (ShortcutLocation shortcutLocation in array)
			{
				if (!locations.HasFlag(shortcutLocation))
				{
					continue;
				}
				string file = linkTargetForVersionInfo(shortcutLocation, zipPackage, versionInfo);
				this.Log().Info("Removing shortcut for {0} => {1}", exeName, file);
				this.ErrorIfThrows(delegate
				{
					if (File.Exists(file))
					{
						File.Delete(file);
					}
				}, "Couldn't delete shortcut: " + file);
			}
			fixPinnedExecutables(zipPackage.Version);
		}

		private Task<string> installPackageToAppDir(UpdateInfo updateInfo, ReleaseEntry release, Action<int> progressCallback)
		{
			return Task.Run(async delegate
			{
				DirectoryInfo target = getDirectoryForRelease(release.Version);
				if (target.Exists)
				{
					this.Log().Warn("Found partially applied release folder, killing it: " + target.FullName);
					await Utility.DeleteDirectory(target.FullName);
				}
				target.Create();
				this.Log().Info("Writing files to app directory: {0}", target.FullName);
				await ReleasePackage.ExtractZipForInstall(Path.Combine(updateInfo.PackageDirectory, release.Filename), target.FullName, rootAppDirectory, progressCallback);
				return target.FullName;
			});
		}

		private async Task<ReleaseEntry> createFullPackagesFromDeltas(IEnumerable<ReleaseEntry> releasesToApply, ReleaseEntry currentVersion, ApplyReleasesProgress progress)
		{
			progress = progress ?? new ApplyReleasesProgress(releasesToApply.Count(), delegate
			{
			});
			if (!releasesToApply.Any())
			{
				return null;
			}
			if (releasesToApply.All((ReleaseEntry x) => !x.IsDelta))
			{
				return releasesToApply.MaxBy((ReleaseEntry x) => x.Version).FirstOrDefault();
			}
			if (!releasesToApply.All((ReleaseEntry x) => x.IsDelta))
			{
				throw new Exception("Cannot apply combinations of delta and full packages");
			}
			ReleasePackage releasePackage = await Task.Run(delegate
			{
				ReleasePackage basePackage = new ReleasePackage(Path.Combine(rootAppDirectory, "packages", currentVersion.Filename));
				ReleasePackage releasePackage2 = new ReleasePackage(Path.Combine(rootAppDirectory, "packages", releasesToApply.First().Filename));
				return new DeltaPackageBuilder(Directory.GetParent(rootAppDirectory).FullName).ApplyDeltaPackage(basePackage, releasePackage2, Regex.Replace(releasePackage2.InputPackageFile, "-delta.nupkg$", ".nupkg", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant), delegate(int x)
				{
					progress.ReportReleaseProgress(x);
				});
			});
			progress.FinishRelease();
			if (releasesToApply.Count() == 1)
			{
				return ReleaseEntry.GenerateFromFile(releasePackage.InputPackageFile);
			}
			FileInfo fileInfo = new FileInfo(releasePackage.InputPackageFile);
			ReleaseEntry currentVersion2 = ReleaseEntry.GenerateFromFile(fileInfo.OpenRead(), fileInfo.Name);
			return await createFullPackagesFromDeltas(releasesToApply.Skip(1), currentVersion2, progress);
		}

		private void executeSelfUpdate(SemanticVersion currentVersion)
		{
			DirectoryInfo targetDir = getDirectoryForRelease(currentVersion);
			string newSquirrel = Path.Combine(targetDir.FullName, "Squirrel.exe");
			if (!File.Exists(newSquirrel))
			{
				return;
			}
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null && Path.GetFileName(entryAssembly.Location).Equals("update.exe", StringComparison.OrdinalIgnoreCase))
			{
				_ = targetDir.Parent.Name;
				Process.Start(newSquirrel, "--updateSelf=" + entryAssembly.Location);
			}
			else
			{
				Utility.Retry(delegate
				{
					File.Copy(newSquirrel, Path.Combine(targetDir.Parent.FullName, "Update.exe"), overwrite: true);
				});
			}
		}

		private async Task invokePostInstall(SemanticVersion currentVersion, bool isInitialInstall, bool firstRunOnly, bool silentInstall)
		{
			DirectoryInfo targetDir = getDirectoryForRelease(currentVersion);
			string args = (isInitialInstall ? $"--squirrel-install {currentVersion}" : $"--squirrel-updated {currentVersion}");
			List<string> squirrelApps = SquirrelAwareExecutableDetector.GetAllSquirrelAwareApps(targetDir.FullName);
			this.Log().Info("Squirrel Enabled Apps: [{0}]", string.Join(",", squirrelApps));
			if (!firstRunOnly)
			{
				await squirrelApps.ForEachAsync(async delegate(string exe)
				{
					using CancellationTokenSource cts = new CancellationTokenSource();
					cts.CancelAfter(15000);
					try
					{
						await Utility.InvokeProcessAsync(exe, args, cts.Token);
					}
					catch (Exception exception)
					{
						this.Log().ErrorException("Couldn't run Squirrel hook, continuing: " + exe, exception);
					}
				}, 1);
			}
			if (squirrelApps.Count == 0)
			{
				this.Log().Warn("No apps are marked as Squirrel-aware! Going to run them all");
				squirrelApps = (from x in targetDir.EnumerateFiles()
					where x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
					where !x.Name.StartsWith("squirrel.", StringComparison.OrdinalIgnoreCase)
					select x.FullName).ToList();
				squirrelApps.ForEach(delegate(string x)
				{
					CreateShortcutsForExecutable(Path.GetFileName(x), ShortcutLocation.StartMenu | ShortcutLocation.Desktop, !isInitialInstall, null, null);
				});
			}
			if (!(!isInitialInstall || silentInstall))
			{
				string firstRunParam = (isInitialInstall ? "--squirrel-firstrun" : "");
				squirrelApps.Select((string exe) => new ProcessStartInfo(exe, firstRunParam)
				{
					WorkingDirectory = Path.GetDirectoryName(exe)
				}).ForEach(delegate(ProcessStartInfo info)
				{
					Process.Start(info);
				});
			}
		}

		private void fixPinnedExecutables(SemanticVersion newCurrentVersion, bool removeAll = false)
		{
			if (Environment.OSVersion.Version < new Version(6, 1))
			{
				this.Log().Warn("fixPinnedExecutables: Found OS Version '{0}', exiting...", Environment.OSVersion.VersionString);
				return;
			}
			string path = "app-" + newCurrentVersion;
			string newAppPath = Path.Combine(rootAppDirectory, path);
			string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar");
			if (!Directory.Exists(path2))
			{
				this.Log().Info("fixPinnedExecutables: PinnedExecutables directory doesn't exitsts, skiping...");
				return;
			}
			Func<FileInfo, ShellLink> selector = delegate(FileInfo file)
			{
				try
				{
					this.Log().Debug("Examining Pin: " + file);
					return new ShellLink(file.FullName);
				}
				catch (Exception exception2)
				{
					string message2 = $"File '{file.FullName}' could not be converted into a valid ShellLink";
					this.Log().WarnException(message2, exception2);
					return (ShellLink)null;
				}
			};
			ShellLink[] array = new DirectoryInfo(path2).GetFiles("*.lnk").Select(selector).ToArray();
			foreach (ShellLink shellLink in array)
			{
				try
				{
					if (shellLink != null && !string.IsNullOrWhiteSpace(shellLink.Target) && shellLink.Target.StartsWith(rootAppDirectory, StringComparison.OrdinalIgnoreCase))
					{
						if (removeAll)
						{
							Utility.DeleteFileHarder(shellLink.ShortCutFile);
						}
						else
						{
							updateLink(shellLink, newAppPath);
						}
					}
				}
				catch (Exception exception)
				{
					string message = $"fixPinnedExecutables: shortcut failed: {shellLink.Target}";
					this.Log().ErrorException(message, exception);
				}
			}
		}

		private void updateLink(ShellLink shortcut, string newAppPath)
		{
			this.Log().Info("Processing shortcut '{0}'", shortcut.ShortCutFile);
			string text = Environment.ExpandEnvironmentVariables(shortcut.Target);
			bool num = text.EndsWith("update.exe", StringComparison.OrdinalIgnoreCase);
			this.Log().Info("Old shortcut target: '{0}'", text);
			if (shortcut.Arguments.Contains("--processStart"))
			{
				shortcut.Arguments = "";
			}
			text = (num ? Path.Combine(rootAppDirectory, Path.GetFileName(shortcut.IconPath)) : Path.Combine(rootAppDirectory, Path.GetFileName(shortcut.Target)));
			this.Log().Info("New shortcut target: '{0}'", text);
			shortcut.WorkingDirectory = newAppPath;
			shortcut.Target = text;
			this.Log().Info("Old iconPath is: '{0}'", shortcut.IconPath);
			shortcut.IconPath = text;
			shortcut.IconIndex = 0;
			this.ErrorIfThrows(delegate
			{
				Utility.Retry(delegate
				{
					shortcut.Save();
				});
			}, "Couldn't write shortcut " + shortcut.ShortCutFile);
			this.Log().Info("Finished shortcut successfully");
		}

		internal void unshimOurselves()
		{
			new RegistryView[2]
			{
				RegistryView.Registry32,
				RegistryView.Registry64
			}.ForEach(delegate(RegistryView view)
			{
				RegistryKey registryKey = null;
				RegistryKey regKey = null;
				try
				{
					registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view);
					regKey = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers");
					if (regKey != null)
					{
						(from x in regKey.GetValueNames()
							where x.StartsWith(rootAppDirectory, StringComparison.OrdinalIgnoreCase)
							select x).ToList().ForEach(delegate(string x)
						{
							this.Log().LogIfThrows(LogLevel.Warn, "Failed to delete key: " + x, delegate
							{
								regKey.DeleteValue(x);
							});
						});
					}
				}
				catch (Exception exception)
				{
					this.Log().WarnException("Couldn't rewrite shim RegKey, most likely no apps are shimmed", exception);
				}
				finally
				{
					if (regKey != null)
					{
						regKey.Dispose();
					}
					registryKey?.Dispose();
				}
			});
		}

		private async Task cleanDeadVersions(SemanticVersion currentVersion, SemanticVersion newVersion, bool forceUninstall = false)
		{
			if (newVersion == null)
			{
				return;
			}
			DirectoryInfo di = new DirectoryInfo(rootAppDirectory);
			if (!di.Exists)
			{
				return;
			}
			this.Log().Info("cleanDeadVersions: checking for version {0}", newVersion);
			string currentVersionFolder = null;
			if (currentVersion != null)
			{
				currentVersionFolder = getDirectoryForRelease(currentVersion).Name;
				this.Log().Info("cleanDeadVersions: exclude current version folder {0}", currentVersionFolder);
			}
			string newVersionFolder = null;
			if (newVersion != null)
			{
				newVersionFolder = getDirectoryForRelease(newVersion).Name;
				this.Log().Info("cleanDeadVersions: exclude new version folder {0}", newVersionFolder);
			}
			IEnumerable<DirectoryInfo> source = from x in di.GetDirectories()
				where x.Name.ToLowerInvariant().Contains("app-")
				where x.Name != newVersionFolder && x.Name != currentVersionFolder
				where !isAppFolderDead(x.FullName)
				select x;
			if (!forceUninstall)
			{
				await source.ForEachAsync(async delegate(DirectoryInfo x)
				{
					List<string> allSquirrelAwareApps = SquirrelAwareExecutableDetector.GetAllSquirrelAwareApps(x.FullName);
					string args = string.Format("--squirrel-obsolete {0}", x.Name.Replace("app-", ""));
					if (allSquirrelAwareApps.Count > 0)
					{
						await allSquirrelAwareApps.ForEachAsync(async delegate(string exe)
						{
							using CancellationTokenSource cts = new CancellationTokenSource();
							cts.CancelAfter(10000);
							try
							{
								await Utility.InvokeProcessAsync(exe, args, cts.Token);
							}
							catch (Exception exception)
							{
								this.Log().ErrorException("Coudln't run Squirrel hook, continuing: " + exe, exception);
							}
						}, 1);
					}
				});
			}
			source = from x in di.GetDirectories()
				where x.Name.ToLowerInvariant().Contains("app-")
				where x.Name != newVersionFolder && x.Name != currentVersionFolder
				select x;
			List<Tuple<string, int>> runningProcesses = UnsafeUtility.EnumerateProcesses();
			await source.ForEachAsync(async delegate(DirectoryInfo x)
			{
				try
				{
					if (runningProcesses.All((Tuple<string, int> p) => p.Item1 == null || !p.Item1.StartsWith(x.FullName, StringComparison.OrdinalIgnoreCase)))
					{
						await Utility.DeleteDirectoryOrJustGiveUp(x.FullName);
					}
					if (Directory.Exists(x.FullName))
					{
						markAppFolderAsDead(x.FullName);
					}
				}
				catch (UnauthorizedAccessException exception)
				{
					this.Log().WarnException("Couldn't delete directory: " + x.FullName, exception);
					markAppFolderAsDead(x.FullName);
				}
			});
			string path = Utility.LocalReleaseFileForAppDir(rootAppDirectory);
			IEnumerable<ReleaseEntry> enumerable = ReleaseEntry.ParseReleaseFile(File.ReadAllText(path, Encoding.UTF8));
			string path2 = Utility.PackageDirectoryForAppDir(rootAppDirectory);
			ReleaseEntry releaseEntry = null;
			foreach (ReleaseEntry item in enumerable)
			{
				if (item.Version == newVersion)
				{
					releaseEntry = ReleaseEntry.GenerateFromFile(Path.Combine(path2, item.Filename));
				}
				else
				{
					File.Delete(Path.Combine(path2, item.Filename));
				}
			}
			ReleaseEntry.WriteReleaseFile(new ReleaseEntry[1] { releaseEntry }, path);
		}

		private static void markAppFolderAsDead(string appFolderPath)
		{
			File.WriteAllText(Path.Combine(appFolderPath, ".dead"), "");
		}

		private static bool isAppFolderDead(string appFolderPath)
		{
			return File.Exists(Path.Combine(appFolderPath, ".dead"));
		}

		internal async Task<List<ReleaseEntry>> updateLocalReleasesFile()
		{
			return await Task.Run(() => ReleaseEntry.BuildReleasesFile(Utility.PackageDirectoryForAppDir(rootAppDirectory)));
		}

		private IEnumerable<DirectoryInfo> getReleases()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(rootAppDirectory);
			if (!directoryInfo.Exists)
			{
				return Enumerable.Empty<DirectoryInfo>();
			}
			return from x in directoryInfo.GetDirectories()
				where x.Name.StartsWith("app-", StringComparison.InvariantCultureIgnoreCase)
				select x;
		}

		private DirectoryInfo getDirectoryForRelease(SemanticVersion releaseVersion)
		{
			return new DirectoryInfo(Path.Combine(rootAppDirectory, "app-" + releaseVersion));
		}

		private string linkTargetForVersionInfo(ShortcutLocation location, IPackage package, FileVersionInfo versionInfo)
		{
			string[] source = new string[4]
			{
				versionInfo.ProductName,
				package.Title,
				versionInfo.FileDescription,
				Path.GetFileNameWithoutExtension(versionInfo.FileName)
			};
			string applicationName = new string[2]
			{
				versionInfo.CompanyName,
				package.Authors.FirstOrDefault() ?? package.Id
			}.First((string x) => !string.IsNullOrWhiteSpace(x));
			string title = source.First((string x) => !string.IsNullOrWhiteSpace(x));
			return getLinkTarget(location, title, applicationName);
		}

		private string getLinkTarget(ShortcutLocation location, string title, string applicationName, bool createDirectoryIfNecessary = true)
		{
			string text = null;
			switch (location)
			{
			case ShortcutLocation.Desktop:
				text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				break;
			case ShortcutLocation.StartMenu:
				text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", applicationName);
				break;
			case ShortcutLocation.Startup:
				text = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
				break;
			case ShortcutLocation.AppRoot:
				text = rootAppDirectory;
				break;
			}
			if (createDirectoryIfNecessary && !Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return Path.Combine(text, title + ".lnk");
		}
	}

	internal class CheckForUpdateImpl : IEnableLogger
	{
		private readonly string rootAppDirectory;

		public CheckForUpdateImpl(string rootAppDirectory)
		{
			this.rootAppDirectory = rootAppDirectory;
		}

		public async Task<UpdateInfo> CheckForUpdate(UpdaterIntention intention, string localReleaseFile, string updateUrlOrPath, bool ignoreDeltaUpdates = false, Action<int> progress = null, IFileDownloader urlDownloader = null)
		{
			progress = progress ?? ((Action<int>)delegate
			{
			});
			IEnumerable<ReleaseEntry> localReleases = Enumerable.Empty<ReleaseEntry>();
			Guid? stagingId = ((intention == UpdaterIntention.Install) ? ((Guid?)null) : getOrCreateStagedUserId());
			bool flag = intention == UpdaterIntention.Install;
			if (intention != UpdaterIntention.Install)
			{
				try
				{
					localReleases = Utility.LoadLocalReleases(localReleaseFile);
				}
				catch (Exception exception)
				{
					this.Log().WarnException("Failed to load local releases, starting from scratch", exception);
					flag = true;
				}
			}
			if (flag)
			{
				await initializeClientAppDirectory();
			}
			ReleaseEntry latestLocalRelease = ((localReleases.Count() > 0) ? localReleases.MaxBy((ReleaseEntry x) => x.Version).First() : null);
			string fileContents;
			if (Utility.IsHttpUrl(updateUrlOrPath))
			{
				if (updateUrlOrPath.EndsWith("/"))
				{
					updateUrlOrPath = updateUrlOrPath.Substring(0, updateUrlOrPath.Length - 1);
				}
				this.Log().Info("Downloading RELEASES file from {0}", updateUrlOrPath);
				int retries = 3;
				while (true)
				{
					try
					{
						Uri uri = Utility.AppendPathToUri(new Uri(updateUrlOrPath), "RELEASES");
						if (latestLocalRelease != null)
						{
							uri = Utility.AddQueryParamsToUri(uri, new Dictionary<string, string>
							{
								{ "id", latestLocalRelease.PackageName },
								{
									"localVersion",
									latestLocalRelease.Version.ToString()
								},
								{
									"arch",
									Environment.Is64BitOperatingSystem ? "amd64" : "x86"
								}
							});
						}
						byte[] bytes = await urlDownloader.DownloadUrl(uri.ToString());
						fileContents = Encoding.UTF8.GetString(bytes);
					}
					catch (WebException exception2)
					{
						this.Log().InfoException("Download resulted in WebException (returning blank release list)", exception2);
						if (retries <= 0)
						{
							throw;
						}
						retries--;
						continue;
					}
					break;
				}
				progress(33);
			}
			else
			{
				this.Log().Info("Reading RELEASES file from {0}", updateUrlOrPath);
				if (!Directory.Exists(updateUrlOrPath))
				{
					throw new Exception($"The directory {updateUrlOrPath} does not exist, something is probably broken with your application");
				}
				FileInfo fileInfo = new FileInfo(Path.Combine(updateUrlOrPath, "RELEASES"));
				if (!fileInfo.Exists)
				{
					string message = $"The file {fileInfo.FullName} does not exist, something is probably broken with your application";
					this.Log().Warn(message);
					FileInfo[] files = new DirectoryInfo(updateUrlOrPath).GetFiles("*.nupkg");
					if (files.Length == 0)
					{
						throw new Exception(message);
					}
					ReleaseEntry.WriteReleaseFile(files.Select((FileInfo x) => ReleaseEntry.GenerateFromFile(x.FullName)), fileInfo.FullName);
				}
				fileContents = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
				progress(33);
			}
			IEnumerable<ReleaseEntry> enumerable = ReleaseEntry.ParseReleaseFileAndApplyStaging(fileContents, stagingId);
			progress(66);
			if (!enumerable.Any())
			{
				throw new Exception("Remote release File is empty or corrupted");
			}
			UpdateInfo result = determineUpdateInfo(intention, localReleases, enumerable, ignoreDeltaUpdates);
			progress(100);
			return result;
		}

		private async Task initializeClientAppDirectory()
		{
			string pkgDir = Path.Combine(rootAppDirectory, "packages");
			if (Directory.Exists(pkgDir))
			{
				await Utility.DeleteDirectory(pkgDir);
			}
			Directory.CreateDirectory(pkgDir);
		}

		private UpdateInfo determineUpdateInfo(UpdaterIntention intention, IEnumerable<ReleaseEntry> localReleases, IEnumerable<ReleaseEntry> remoteReleases, bool ignoreDeltaUpdates)
		{
			string packageDirectory = Utility.PackageDirectoryForAppDir(rootAppDirectory);
			localReleases = localReleases ?? Enumerable.Empty<ReleaseEntry>();
			if (remoteReleases == null)
			{
				this.Log().Warn("Release information couldn't be determined due to remote corrupt RELEASES file");
				throw new Exception("Corrupt remote RELEASES file");
			}
			ReleaseEntry releaseEntry = Utility.FindCurrentVersion(remoteReleases);
			ReleaseEntry releaseEntry2 = Utility.FindCurrentVersion(localReleases);
			if (releaseEntry == releaseEntry2)
			{
				this.Log().Info("No updates, remote and local are the same");
				return UpdateInfo.Create(releaseEntry2, new ReleaseEntry[1] { releaseEntry }, packageDirectory);
			}
			if (ignoreDeltaUpdates)
			{
				remoteReleases = remoteReleases.Where((ReleaseEntry x) => !x.IsDelta);
			}
			if (!localReleases.Any())
			{
				if (intention == UpdaterIntention.Install)
				{
					this.Log().Info("First run, starting from scratch");
				}
				else
				{
					this.Log().Warn("No local releases found, starting from scratch");
				}
				return UpdateInfo.Create(null, new ReleaseEntry[1] { releaseEntry }, packageDirectory);
			}
			if (localReleases.Max((ReleaseEntry x) => x.Version) > remoteReleases.Max((ReleaseEntry x) => x.Version))
			{
				this.Log().Warn("hwhat, local version is greater than remote version");
				return UpdateInfo.Create(Utility.FindCurrentVersion(localReleases), new ReleaseEntry[1] { releaseEntry }, packageDirectory);
			}
			return UpdateInfo.Create(releaseEntry2, remoteReleases, packageDirectory);
		}

		internal Guid? getOrCreateStagedUserId()
		{
			string path = Path.Combine(rootAppDirectory, "packages", ".betaId");
			Guid result = default(Guid);
			try
			{
				if (!Guid.TryParse(File.ReadAllText(path, Encoding.UTF8), out result))
				{
					throw new Exception("File was read but contents were invalid");
				}
				this.Log().Info("Using existing staging user ID: {0}", result.ToString());
				return result;
			}
			catch (Exception exception)
			{
				this.Log().DebugException("Couldn't read staging user ID, creating a blank one", exception);
			}
			Random random = new Random();
			byte[] array = new byte[4096];
			random.NextBytes(array);
			result = Utility.CreateGuidFromHash(array);
			try
			{
				File.WriteAllText(path, result.ToString(), Encoding.UTF8);
				this.Log().Info("Generated new staging user ID: {0}", result.ToString());
				return result;
			}
			catch (Exception exception2)
			{
				this.Log().WarnException("Couldn't write out staging user ID, this user probably shouldn't get beta anything", exception2);
				return null;
			}
		}
	}

	internal class DownloadReleasesImpl : IEnableLogger
	{
		private readonly string rootAppDirectory;

		public DownloadReleasesImpl(string rootAppDirectory)
		{
			this.rootAppDirectory = rootAppDirectory;
		}

		public async Task DownloadReleases(string updateUrlOrPath, IEnumerable<ReleaseEntry> releasesToDownload, Action<int> progress = null, IFileDownloader urlDownloader = null)
		{
			progress = progress ?? ((Action<int>)delegate
			{
			});
			urlDownloader = urlDownloader ?? new FileDownloader();
			string packagesDirectory = Path.Combine(rootAppDirectory, "packages");
			double current = 0.0;
			double toIncrement = 100.0 / (double)releasesToDownload.Count();
			if (Utility.IsHttpUrl(updateUrlOrPath))
			{
				await releasesToDownload.ForEachAsync(async delegate(ReleaseEntry x)
				{
					string targetFile = Path.Combine(packagesDirectory, x.Filename);
					double component = 0.0;
					await downloadRelease(updateUrlOrPath, x, urlDownloader, targetFile, delegate(int p)
					{
						lock (progress)
						{
							current -= component;
							component = toIncrement / 100.0 * (double)p;
							progress((int)Math.Round(current += component));
						}
					});
					checksumPackage(x);
				});
				return;
			}
			await releasesToDownload.ForEachAsync(delegate(ReleaseEntry x)
			{
				string destFileName = Path.Combine(packagesDirectory, x.Filename);
				File.Copy(Path.Combine(updateUrlOrPath, x.Filename), destFileName, overwrite: true);
				lock (progress)
				{
					progress((int)Math.Round(current += toIncrement));
				}
				checksumPackage(x);
			});
		}

		private bool isReleaseExplicitlyHttp(ReleaseEntry x)
		{
			if (x.BaseUrl != null)
			{
				return Uri.IsWellFormedUriString(x.BaseUrl, UriKind.Absolute);
			}
			return false;
		}

		private Task downloadRelease(string updateBaseUrl, ReleaseEntry releaseEntry, IFileDownloader urlDownloader, string targetFile, Action<int> progress)
		{
			Uri baseUri = Utility.EnsureTrailingSlash(new Uri(updateBaseUrl));
			string text = releaseEntry.BaseUrl + releaseEntry.Filename;
			if (!string.IsNullOrEmpty(releaseEntry.Query))
			{
				text += releaseEntry.Query;
			}
			string absoluteUri = new Uri(baseUri, text).AbsoluteUri;
			File.Delete(targetFile);
			return urlDownloader.DownloadFile(absoluteUri, targetFile, progress);
		}

		private Task checksumAllPackages(IEnumerable<ReleaseEntry> releasesDownloaded)
		{
			return releasesDownloaded.ForEachAsync(delegate(ReleaseEntry x)
			{
				checksumPackage(x);
			});
		}

		private void checksumPackage(ReleaseEntry downloadedRelease)
		{
			FileInfo fileInfo = new FileInfo(Path.Combine(rootAppDirectory, "packages", downloadedRelease.Filename));
			if (!fileInfo.Exists)
			{
				this.Log().Error("File {0} should exist but doesn't", fileInfo.FullName);
				throw new Exception("Checksummed file doesn't exist: " + fileInfo.FullName);
			}
			if (fileInfo.Length != downloadedRelease.Filesize)
			{
				this.Log().Error("File Length should be {0}, is {1}", downloadedRelease.Filesize, fileInfo.Length);
				fileInfo.Delete();
				throw new Exception("Checksummed file size doesn't match: " + fileInfo.FullName);
			}
			using FileStream file = fileInfo.OpenRead();
			string text = Utility.CalculateStreamSHA1(file);
			if (!text.Equals(downloadedRelease.SHA1, StringComparison.OrdinalIgnoreCase))
			{
				this.Log().Error("File SHA1 should be {0}, is {1}", downloadedRelease.SHA1, text);
				fileInfo.Delete();
				throw new Exception("Checksum doesn't match: " + fileInfo.FullName);
			}
		}
	}

	[DataContract]
	public class Release
	{
		[DataMember(Name = "prerelease")]
		public bool Prerelease { get; set; }

		[DataMember(Name = "published_at")]
		public DateTime PublishedAt { get; set; }

		[DataMember(Name = "html_url")]
		public string HtmlUrl { get; set; }
	}

	internal class InstallHelperImpl : IEnableLogger
	{
		private readonly string applicationName;

		private readonly string rootAppDirectory;

		private const string currentVersionRegSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		private const string uninstallRegSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		public InstallHelperImpl(string applicationName, string rootAppDirectory)
		{
			this.applicationName = applicationName;
			this.rootAppDirectory = rootAppDirectory;
		}

		public async Task<RegistryKey> CreateUninstallerRegistryEntry(string uninstallCmd, string quietSwitch)
		{
			ReleaseEntry releaseEntry = (from x in ReleaseEntry.ParseReleaseFile(File.ReadAllText(Path.Combine(rootAppDirectory, "packages", "RELEASES"), Encoding.UTF8))
				where !x.IsDelta
				orderby x.Version descending
				select x).First();
			string pkgPath = Path.Combine(rootAppDirectory, "packages", releaseEntry.Filename);
			ZipPackage zp = new ZipPackage(pkgPath);
			string targetPng = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
			string targetIco = Path.Combine(rootAppDirectory, "app.ico");
			using (RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey("Uninstall", RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
			}
			RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + applicationName, RegistryKeyPermissionCheck.ReadWriteSubTree);
			if (zp.IconUrl != null && !File.Exists(targetIco))
			{
				try
				{
					using WebClient wc = Utility.CreateWebClient();
					await wc.DownloadFileTaskAsync(zp.IconUrl, targetPng);
					using FileStream fileStream = new FileStream(targetIco, FileMode.Create);
					if (zp.IconUrl.AbsolutePath.EndsWith("ico"))
					{
						byte[] array = File.ReadAllBytes(targetPng);
						fileStream.Write(array, 0, array.Length);
					}
					else
					{
						Bitmap val = (Bitmap)Image.FromFile(targetPng);
						try
						{
							Icon val2 = Icon.FromHandle(val.GetHicon());
							try
							{
								val2.Save((Stream)fileStream);
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
					key.SetValue("DisplayIcon", targetIco, RegistryValueKind.String);
				}
				catch (Exception exception)
				{
					this.Log().InfoException("Couldn't write uninstall icon, don't care", exception);
				}
				finally
				{
					File.Delete(targetPng);
				}
			}
			var array2 = new[]
			{
				new
				{
					Key = "DisplayName",
					Value = (zp.Title ?? zp.Description ?? zp.Summary)
				},
				new
				{
					Key = "DisplayVersion",
					Value = zp.Version.ToString()
				},
				new
				{
					Key = "InstallDate",
					Value = DateTime.Now.ToString("yyyyMMdd")
				},
				new
				{
					Key = "InstallLocation",
					Value = rootAppDirectory
				},
				new
				{
					Key = "Publisher",
					Value = string.Join(",", zp.Authors)
				},
				new
				{
					Key = "QuietUninstallString",
					Value = $"{uninstallCmd} {quietSwitch}"
				},
				new
				{
					Key = "UninstallString",
					Value = uninstallCmd
				},
				new
				{
					Key = "URLUpdateInfo",
					Value = ((zp.ProjectUrl != null) ? zp.ProjectUrl.ToString() : "")
				}
			};
			var array3 = new[]
			{
				new
				{
					Key = "EstimatedSize",
					Value = (int)(new FileInfo(pkgPath).Length / 1024)
				},
				new
				{
					Key = "NoModify",
					Value = 1
				},
				new
				{
					Key = "NoRepair",
					Value = 1
				},
				new
				{
					Key = "Language",
					Value = 1033
				}
			};
			var array4 = array2;
			foreach (var anon in array4)
			{
				key.SetValue(anon.Key, anon.Value, RegistryValueKind.String);
			}
			var array5 = array3;
			foreach (var anon2 in array5)
			{
				key.SetValue(anon2.Key, anon2.Value, RegistryValueKind.DWord);
			}
			return key;
		}

		public void KillAllProcessesBelongingToPackage()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string ourExePath = ((entryAssembly != null) ? entryAssembly.Location : null);
			UnsafeUtility.EnumerateProcesses().Where(delegate(Tuple<string, int> x)
			{
				if (string.IsNullOrWhiteSpace(x.Item1))
				{
					return false;
				}
				if (!x.Item1.StartsWith(rootAppDirectory, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				if (ourExePath != null && x.Item1.Equals(ourExePath, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				string text = Path.GetFileName(x.Item1).ToLowerInvariant();
				return (!(text == "squirrel.exe") && !(text == "update.exe")) ? true : false;
			}).ForEach(delegate(Tuple<string, int> x)
			{
				try
				{
					this.WarnIfThrows(delegate
					{
						Process.GetProcessById(x.Item2).Kill();
					});
				}
				catch
				{
				}
			});
		}

		public Task<RegistryKey> CreateUninstallerRegistryEntry()
		{
			string arg = Path.Combine(rootAppDirectory, "Update.exe");
			return CreateUninstallerRegistryEntry($"\"{arg}\" --uninstall", "-s");
		}

		public void RemoveUninstallerRegistryEntry()
		{
			RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", writable: true).DeleteSubKeyTree(applicationName);
		}
	}

	private readonly string rootAppDirectory;

	private readonly string applicationName;

	private readonly IFileDownloader urlDownloader;

	private readonly string updateUrlOrPath;

	private IDisposable updateLock;

	private static bool exiting;

	public string ApplicationName => applicationName;

	public string RootAppDirectory => rootAppDirectory;

	public bool IsInstalledApp => Assembly.GetExecutingAssembly().Location.StartsWith(RootAppDirectory, StringComparison.OrdinalIgnoreCase);

	public UpdateManager(string urlOrPath, string applicationName = null, string rootDirectory = null, IFileDownloader urlDownloader = null)
	{
		updateUrlOrPath = urlOrPath;
		this.applicationName = applicationName ?? getApplicationName();
		this.urlDownloader = urlDownloader ?? new FileDownloader();
		if (rootDirectory != null)
		{
			rootAppDirectory = Path.Combine(rootDirectory, this.applicationName);
		}
		else
		{
			rootAppDirectory = Path.Combine(rootDirectory ?? GetLocalAppDataDirectory(), this.applicationName);
		}
	}

	public async Task<UpdateInfo> CheckForUpdate(bool ignoreDeltaUpdates = false, Action<int> progress = null, UpdaterIntention intention = UpdaterIntention.Update)
	{
		CheckForUpdateImpl checkForUpdate = new CheckForUpdateImpl(rootAppDirectory);
		await acquireUpdateLock();
		return await checkForUpdate.CheckForUpdate(intention, Utility.LocalReleaseFileForAppDir(rootAppDirectory), updateUrlOrPath, ignoreDeltaUpdates, progress, urlDownloader);
	}

	public async Task DownloadReleases(IEnumerable<ReleaseEntry> releasesToDownload, Action<int> progress = null)
	{
		DownloadReleasesImpl downloadReleases = new DownloadReleasesImpl(rootAppDirectory);
		await acquireUpdateLock();
		await downloadReleases.DownloadReleases(updateUrlOrPath, releasesToDownload, progress, urlDownloader);
	}

	public async Task<string> ApplyReleases(UpdateInfo updateInfo, Action<int> progress = null)
	{
		ApplyReleasesImpl applyReleases = new ApplyReleasesImpl(rootAppDirectory);
		await acquireUpdateLock();
		return await applyReleases.ApplyReleases(updateInfo, silentInstall: false, attemptingFullInstall: false, progress);
	}

	public async Task FullInstall(bool silentInstall = false, Action<int> progress = null)
	{
		UpdateInfo updateInfo = await CheckForUpdate(ignoreDeltaUpdates: false, null, UpdaterIntention.Install);
		await DownloadReleases(updateInfo.ReleasesToApply);
		ApplyReleasesImpl applyReleases = new ApplyReleasesImpl(rootAppDirectory);
		await acquireUpdateLock();
		await applyReleases.ApplyReleases(updateInfo, silentInstall, attemptingFullInstall: true, progress);
	}

	public async Task FullUninstall()
	{
		ApplyReleasesImpl applyReleases = new ApplyReleasesImpl(rootAppDirectory);
		await acquireUpdateLock();
		KillAllExecutablesBelongingToPackage();
		await applyReleases.FullUninstall();
	}

	public Task<RegistryKey> CreateUninstallerRegistryEntry(string uninstallCmd, string quietSwitch)
	{
		return new InstallHelperImpl(applicationName, rootAppDirectory).CreateUninstallerRegistryEntry(uninstallCmd, quietSwitch);
	}

	public Task<RegistryKey> CreateUninstallerRegistryEntry()
	{
		return new InstallHelperImpl(applicationName, rootAppDirectory).CreateUninstallerRegistryEntry();
	}

	public void RemoveUninstallerRegistryEntry()
	{
		new InstallHelperImpl(applicationName, rootAppDirectory).RemoveUninstallerRegistryEntry();
	}

	public void CreateShortcutsForExecutable(string exeName, ShortcutLocation locations, bool updateOnly, string programArguments = null, string icon = null)
	{
		new ApplyReleasesImpl(rootAppDirectory).CreateShortcutsForExecutable(exeName, locations, updateOnly, programArguments, icon);
	}

	public Dictionary<ShortcutLocation, ShellLink> GetShortcutsForExecutable(string exeName, ShortcutLocation locations, string programArguments = null)
	{
		return new ApplyReleasesImpl(rootAppDirectory).GetShortcutsForExecutable(exeName, locations, programArguments);
	}

	public void RemoveShortcutsForExecutable(string exeName, ShortcutLocation locations)
	{
		new ApplyReleasesImpl(rootAppDirectory).RemoveShortcutsForExecutable(exeName, locations);
	}

	public SemanticVersion CurrentlyInstalledVersion(string executable = null)
	{
		executable = executable ?? Path.GetDirectoryName(typeof(UpdateManager).Assembly.Location);
		if (!executable.StartsWith(rootAppDirectory, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		return executable.Split(new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		}).FirstOrDefault((string x) => x.StartsWith("app-", StringComparison.OrdinalIgnoreCase))?.ToSemanticVersion();
	}

	public void KillAllExecutablesBelongingToPackage()
	{
		new InstallHelperImpl(applicationName, rootAppDirectory).KillAllProcessesBelongingToPackage();
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref updateLock, null)?.Dispose();
	}

	public static void RestartApp(string exeToStart = null, string arguments = null)
	{
		exeToStart = exeToStart ?? Path.GetFileName(Assembly.GetEntryAssembly().Location);
		string arg = ((arguments != null) ? $"-a \"{arguments}\"" : "");
		exiting = true;
		Process.Start(getUpdateExe(), $"--processStartAndWait \"{exeToStart}\" {arg}");
		Thread.Sleep(500);
		Environment.Exit(0);
	}

	public static async Task<Process> RestartAppWhenExited(string exeToStart = null, string arguments = null)
	{
		exeToStart = exeToStart ?? Path.GetFileName(Assembly.GetEntryAssembly().Location);
		string arg = ((arguments != null) ? $"-a \"{arguments}\"" : "");
		exiting = true;
		Process updateProcess = Process.Start(getUpdateExe(), $"--processStartAndWait {exeToStart} {arg}");
		await Task.Delay(500);
		return updateProcess;
	}

	public static string GetLocalAppDataDirectory(string assemblyLocation = null)
	{
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		if (assemblyLocation == null && entryAssembly == null)
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		assemblyLocation = assemblyLocation ?? entryAssembly.Location;
		if (Path.GetFileName(assemblyLocation).Equals("update.exe", StringComparison.OrdinalIgnoreCase))
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyLocation), ".."));
		}
		return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyLocation), "..\\.."));
	}

	~UpdateManager()
	{
		if (updateLock != null && !exiting)
		{
			throw new Exception("You must dispose UpdateManager!");
		}
	}

	private Task<IDisposable> acquireUpdateLock()
	{
		if (updateLock != null)
		{
			return Task.FromResult(updateLock);
		}
		return Task.Run(delegate
		{
			string key = Utility.CalculateStreamSHA1(new MemoryStream(Encoding.UTF8.GetBytes(rootAppDirectory)));
			IDisposable theLock;
			try
			{
				IDisposable disposable2;
				if (!ModeDetector.InUnitTestRunner())
				{
					IDisposable disposable = new SingleGlobalInstance(key, TimeSpan.FromMilliseconds(2000.0));
					disposable2 = disposable;
				}
				else
				{
					disposable2 = Disposable.Create(delegate
					{
					});
				}
				theLock = disposable2;
			}
			catch (TimeoutException)
			{
				throw new TimeoutException("Couldn't acquire update lock, another instance may be running updates");
			}
			return updateLock = Disposable.Create(delegate
			{
				theLock.Dispose();
				updateLock = null;
			});
		});
	}

	internal static int CalculateProgress(int percentageOfCurrentStep, int stepStartPercentage, int stepEndPercentage)
	{
		percentageOfCurrentStep = Math.Max(Math.Min(percentageOfCurrentStep, 100), 0);
		return (int)((double)(stepEndPercentage - stepStartPercentage) / 100.0 * (double)percentageOfCurrentStep + (double)stepStartPercentage);
	}

	private static string getApplicationName()
	{
		return new FileInfo(getUpdateExe()).Directory.Name;
	}

	private static string getUpdateExe()
	{
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		if (entryAssembly != null && Path.GetFileName(entryAssembly.Location).Equals("update.exe", StringComparison.OrdinalIgnoreCase) && entryAssembly.Location.IndexOf("app-", StringComparison.OrdinalIgnoreCase) == -1 && entryAssembly.Location.IndexOf("SquirrelTemp", StringComparison.OrdinalIgnoreCase) == -1)
		{
			return Path.GetFullPath(entryAssembly.Location);
		}
		entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
		FileInfo fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "..\\Update.exe"));
		if (!fileInfo.Exists)
		{
			throw new Exception("Update.exe not found, not a Squirrel-installed app?");
		}
		return fileInfo.FullName;
	}

	public static async Task<UpdateManager> GitHubUpdateManager(string repoUrl, string applicationName = null, string rootDirectory = null, IFileDownloader urlDownloader = null, bool prerelease = false, string accessToken = null)
	{
		Uri uri = new Uri(repoUrl);
		ProductInfoHeaderValue item = new ProductInfoHeaderValue("Squirrel", Assembly.GetExecutingAssembly().GetName().Version.ToString());
		if (uri.Segments.Length != 3)
		{
			throw new Exception("Repo URL must be to the root URL of the repo e.g. https://github.com/myuser/myrepo");
		}
		StringBuilder stringBuilder = new StringBuilder("repos").Append(uri.AbsolutePath).Append("/releases");
		if (!string.IsNullOrWhiteSpace(accessToken))
		{
			stringBuilder.Append("?access_token=").Append(accessToken);
		}
		Uri baseAddress = ((!uri.Host.EndsWith("github.com", StringComparison.OrdinalIgnoreCase)) ? new Uri($"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Host}/api/v3/") : new Uri("https://api.github.com/"));
		using System.Net.Http.HttpClient client = new System.Net.Http.HttpClient
		{
			BaseAddress = baseAddress
		};
		client.DefaultRequestHeaders.UserAgent.Add(item);
		HttpResponseMessage obj = await client.GetAsync(stringBuilder.ToString());
		obj.EnsureSuccessStatusCode();
		return new UpdateManager((from x in SimpleJson.DeserializeObject<List<Release>>(await obj.Content.ReadAsStringAsync())
			where prerelease || !x.Prerelease
			orderby x.PublishedAt descending
			select x).First().HtmlUrl.Replace("/tag/", "/download/"), applicationName, rootDirectory, urlDownloader);
	}
}
