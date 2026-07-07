using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal static class EasyModeMixin
{
	public static async Task<ReleaseEntry> UpdateApp(this IUpdateManager This, Action<int> progress = null)
	{
		progress = progress ?? ((Action<int>)delegate
		{
		});
		This.Log().Info("Starting automatic update");
		bool ignoreDeltaUpdates = false;
		UpdateInfo updateInfo;
		while (true)
		{
			updateInfo = null;
			try
			{
				updateInfo = await This.ErrorIfThrows(() => This.CheckForUpdate(ignoreDeltaUpdates, delegate(int x)
				{
					progress(x / 3);
				}), "Failed to check for updates");
				await This.ErrorIfThrows(() => This.DownloadReleases(updateInfo.ReleasesToApply, delegate(int x)
				{
					progress(x / 3 + 33);
				}), "Failed to download updates");
				await This.ErrorIfThrows(() => This.ApplyReleases(updateInfo, delegate(int x)
				{
					progress(x / 3 + 66);
				}), "Failed to apply updates");
				await This.ErrorIfThrows(() => This.CreateUninstallerRegistryEntry(), "Failed to set up uninstaller");
			}
			catch
			{
				if (!ignoreDeltaUpdates)
				{
					ignoreDeltaUpdates = true;
					continue;
				}
				throw;
			}
			break;
		}
		return updateInfo.ReleasesToApply.Any() ? updateInfo.ReleasesToApply.MaxBy((ReleaseEntry x) => x.Version).Last() : null;
	}

	public static void CreateShortcutForThisExe(this IUpdateManager This)
	{
		This.CreateShortcutsForExecutable(Path.GetFileName(Assembly.GetEntryAssembly().Location), ShortcutLocation.StartMenu | ShortcutLocation.Desktop, !Environment.CommandLine.Contains("squirrel-install"), null, null);
	}

	public static void RemoveShortcutForThisExe(this IUpdateManager This)
	{
		This.RemoveShortcutsForExecutable(Path.GetFileName(Assembly.GetEntryAssembly().Location), ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
	}
}
