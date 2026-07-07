using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32;
using NuGet;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal interface IUpdateManager : IDisposable, IEnableLogger
{
	Task<UpdateInfo> CheckForUpdate(bool ignoreDeltaUpdates = false, Action<int> progress = null, UpdaterIntention intention = UpdaterIntention.Update);

	Task DownloadReleases(IEnumerable<ReleaseEntry> releasesToDownload, Action<int> progress = null);

	Task<string> ApplyReleases(UpdateInfo updateInfo, Action<int> progress = null);

	Task FullInstall(bool silentInstall, Action<int> progress = null);

	Task FullUninstall();

	SemanticVersion CurrentlyInstalledVersion(string executable = null);

	Task<RegistryKey> CreateUninstallerRegistryEntry(string uninstallCmd, string quietSwitch);

	Task<RegistryKey> CreateUninstallerRegistryEntry();

	void RemoveUninstallerRegistryEntry();

	void CreateShortcutsForExecutable(string exeName, ShortcutLocation locations, bool updateOnly, string programArguments, string icon);

	void RemoveShortcutsForExecutable(string exeName, ShortcutLocation locations);
}
