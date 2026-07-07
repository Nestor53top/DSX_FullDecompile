using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Squirrel.Shell;

internal static class TaskbarHelper
{
	public static bool IsPinnedToTaskbar(string executablePath)
	{
		return (from pinnedShortcut in Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar"), "*.lnk")
			select new ShellLink(pinnedShortcut)).Any((ShellLink shortcut) => string.Equals(shortcut.Target, executablePath, StringComparison.OrdinalIgnoreCase));
	}

	public static void PinToTaskbar(string executablePath)
	{
		pinUnpin(executablePath, "pin to taskbar");
		if (!IsPinnedToTaskbar(executablePath))
		{
			throw new Exception("Pinning executable to taskbar failed.");
		}
	}

	public static void UnpinFromTaskbar(string executablePath)
	{
		pinUnpin(executablePath, "unpin from taskbar");
		if (IsPinnedToTaskbar(executablePath))
		{
			throw new Exception("Executable is still pinned to taskbar.");
		}
	}

	private static void pinUnpin(string executablePath, string verbToExecute)
	{
		if (!File.Exists(executablePath))
		{
			throw new FileNotFoundException(executablePath);
		}
		dynamic val = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
		try
		{
			string directoryName = Path.GetDirectoryName(executablePath);
			string fileName = Path.GetFileName(executablePath);
			dynamic val2 = val.NameSpace(directoryName);
			dynamic val3 = val2.ParseName(fileName);
			dynamic val4 = val3.Verbs();
			for (int i = 0; i < val4.Count(); i++)
			{
				dynamic val5 = val4.Item(i);
				if (((string)val5.Name.Replace("&", string.Empty).ToLower()).Equals(verbToExecute))
				{
					val5.DoIt();
				}
			}
		}
		finally
		{
			Marshal.ReleaseComObject(val);
		}
	}
}
