using System;
using Mono.Options;

namespace Squirrel.Update;

internal class StartupOption
{
	private readonly OptionSet optionSet;

	internal bool silentInstall { get; private set; }

	internal UpdateAction updateAction { get; private set; }

	internal string target { get; private set; }

	internal string releaseDir { get; private set; }

	internal string packagesDir { get; private set; }

	internal string bootstrapperExe { get; private set; }

	internal string backgroundGif { get; private set; }

	internal string signingParameters { get; private set; }

	internal string baseUrl { get; private set; }

	internal string processStart { get; private set; }

	internal string processStartArgs { get; private set; }

	internal string setupIcon { get; private set; }

	internal string icon { get; private set; }

	internal string shortcutArgs { get; private set; }

	internal string frameworkVersion { get; private set; } = "net45";

	internal bool shouldWait { get; private set; }

	internal bool noMsi { get; private set; } = Environment.OSVersion.Platform != PlatformID.Win32NT;

	internal bool packageAs64Bit { get; private set; }

	internal bool noDelta { get; private set; }

	public StartupOption(string[] args)
	{
		optionSet = Parse(args);
	}

	private OptionSet Parse(string[] args)
	{
		OptionSet obj = new OptionSet();
		obj.Add("Usage: Squirrel.exe command [OPTS]");
		obj.Add("Manages Squirrel packages");
		obj.Add("");
		obj.Add("Commands");
		obj.Add("install=", "Install the app whose package is in the specified directory", delegate(string v)
		{
			updateAction = UpdateAction.Install;
			target = v;
		});
		obj.Add("uninstall", "Uninstall the app the same dir as Update.exe", (Action<string>)delegate
		{
			updateAction = UpdateAction.Uninstall;
		});
		obj.Add("download=", "Download the releases specified by the URL and write new results to stdout as JSON", delegate(string v)
		{
			updateAction = UpdateAction.Download;
			target = v;
		});
		obj.Add("checkForUpdate=", "Check for one available update and writes new results to stdout as JSON", delegate(string v)
		{
			updateAction = UpdateAction.CheckForUpdate;
			target = v;
		});
		obj.Add("update=", "Update the application to the latest remote version specified by URL", delegate(string v)
		{
			updateAction = UpdateAction.Update;
			target = v;
		});
		obj.Add("releasify=", "Update or generate a releases directory with a given NuGet package", delegate(string v)
		{
			updateAction = UpdateAction.Releasify;
			target = v;
		});
		obj.Add("createShortcut=", "Create a shortcut for the given executable name", delegate(string v)
		{
			updateAction = UpdateAction.Shortcut;
			target = v;
		});
		obj.Add("removeShortcut=", "Remove a shortcut for the given executable name", delegate(string v)
		{
			updateAction = UpdateAction.Deshortcut;
			target = v;
		});
		obj.Add("updateSelf=", "Copy the currently executing Update.exe into the default location", delegate(string v)
		{
			updateAction = UpdateAction.UpdateSelf;
			target = v;
		});
		obj.Add("processStart=", "Start an executable in the latest version of the app package", delegate(string v)
		{
			updateAction = UpdateAction.ProcessStart;
			processStart = v;
		}, hidden: true);
		obj.Add("processStartAndWait=", "Start an executable in the latest version of the app package", delegate(string v)
		{
			updateAction = UpdateAction.ProcessStart;
			processStart = v;
			shouldWait = true;
		}, hidden: true);
		obj.Add("");
		obj.Add("Options:");
		obj.Add("h|?|help", "Display Help and exit", (Action<string>)delegate
		{
		});
		obj.Add("r=|releaseDir=", "Path to a release directory to use with releasify", delegate(string v)
		{
			releaseDir = v;
		});
		obj.Add("p=|packagesDir=", "Path to the NuGet Packages directory for C# apps", delegate(string v)
		{
			packagesDir = v;
		});
		obj.Add("bootstrapperExe=", "Path to the Setup.exe to use as a template", delegate(string v)
		{
			bootstrapperExe = v;
		});
		obj.Add("g=|loadingGif=", "Path to an animated GIF to be displayed during installation", delegate(string v)
		{
			backgroundGif = v;
		});
		obj.Add("i=|icon", "Path to an ICO file that will be used for icon shortcuts", delegate(string v)
		{
			icon = v;
		});
		obj.Add("setupIcon=", "Path to an ICO file that will be used for the Setup executable's icon", delegate(string v)
		{
			setupIcon = v;
		});
		obj.Add("n=|signWithParams=", "Sign the installer via SignTool.exe with the parameters given", delegate(string v)
		{
			signingParameters = v;
		});
		obj.Add("s|silent", "Silent install", (Action<string>)delegate
		{
			silentInstall = true;
		});
		obj.Add("b=|baseUrl=", "Provides a base URL to prefix the RELEASES file packages with", delegate(string v)
		{
			baseUrl = v;
		}, hidden: true);
		obj.Add("a=|process-start-args=", "Arguments that will be used when starting executable", delegate(string v)
		{
			processStartArgs = v;
		}, hidden: true);
		obj.Add("l=|shortcut-locations=", "Comma-separated string of shortcut locations, e.g. 'Desktop,StartMenu'", delegate(string v)
		{
			shortcutArgs = v;
		});
		obj.Add("no-msi", "Don't generate an MSI package", (Action<string>)delegate
		{
			noMsi = true;
		});
		obj.Add("no-delta", "Don't generate delta packages to save time", (Action<string>)delegate
		{
			noDelta = true;
		});
		obj.Add("framework-version=", "Set the required .NET framework version, e.g. net461", delegate(string v)
		{
			frameworkVersion = v;
		});
		obj.Add("msi-win64", "Mark the MSI as 64-bit, which is useful in Enterprise deployment scenarios", (Action<string>)delegate
		{
			packageAs64Bit = true;
		});
		obj.Parse(args);
		setupIcon = setupIcon ?? icon;
		return obj;
	}

	internal void WriteOptionDescriptions()
	{
		optionSet.WriteOptionDescriptions(Console.Out);
	}
}
