using System;
using System.Collections.Generic;
using System.Linq;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal static class SquirrelAwareApp
{
	public static void HandleEvents(Action<Version> onInitialInstall = null, Action<Version> onAppUpdate = null, Action<Version> onAppObsoleted = null, Action<Version> onAppUninstall = null, Action onFirstRun = null, string[] arguments = null)
	{
		Action<Version> action = delegate
		{
		};
		string[] array = arguments ?? Environment.GetCommandLineArgs().Skip(1).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		Dictionary<string, Action<Version>> dictionary = new[]
		{
			new
			{
				Key = "--squirrel-install",
				Value = (onInitialInstall ?? action)
			},
			new
			{
				Key = "--squirrel-updated",
				Value = (onAppUpdate ?? action)
			},
			new
			{
				Key = "--squirrel-obsolete",
				Value = (onAppObsoleted ?? action)
			},
			new
			{
				Key = "--squirrel-uninstall",
				Value = (onAppUninstall ?? action)
			}
		}.ToDictionary(k => k.Key, v => v.Value);
		if (array[0] == "--squirrel-firstrun")
		{
			(onFirstRun ?? ((Action)delegate
			{
			}))();
		}
		else
		{
			if (array.Length != 2 || !dictionary.ContainsKey(array[0]))
			{
				return;
			}
			Version version = array[1].ToSemanticVersion().Version;
			try
			{
				dictionary[array[0]](version);
				if (!ModeDetector.InUnitTestRunner())
				{
					Environment.Exit(0);
				}
			}
			catch (Exception exception)
			{
				LogHost.Default.ErrorException("Failed to handle Squirrel events", exception);
				if (!ModeDetector.InUnitTestRunner())
				{
					Environment.Exit(-1);
				}
			}
		}
	}
}
