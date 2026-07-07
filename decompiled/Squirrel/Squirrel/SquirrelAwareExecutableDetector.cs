using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Squirrel;

internal static class SquirrelAwareExecutableDetector
{
	public static List<string> GetAllSquirrelAwareApps(string directory, int minimumVersion = 1)
	{
		return (from x in new DirectoryInfo(directory).EnumerateFiles()
			where x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
			select x.FullName into x
			where (GetPESquirrelAwareVersion(x) ?? (-1)) >= minimumVersion
			select x).ToList();
	}

	public static int? GetPESquirrelAwareVersion(string executable)
	{
		if (!File.Exists(executable))
		{
			return null;
		}
		string fullname = Path.GetFullPath(executable);
		return Utility.Retry(() => GetAssemblySquirrelAwareVersion(fullname) ?? GetVersionBlockSquirrelAwareValue(fullname));
	}

	private static int? GetAssemblySquirrelAwareVersion(string executable)
	{
		try
		{
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(executable);
			if (!assemblyDefinition.HasCustomAttributes)
			{
				return null;
			}
			CustomAttribute customAttribute = assemblyDefinition.CustomAttributes.FirstOrDefault(delegate(CustomAttribute x)
			{
				if (x.AttributeType.FullName != typeof(AssemblyMetadataAttribute).FullName)
				{
					return false;
				}
				return x.ConstructorArguments.Count == 2 && x.ConstructorArguments[0].Value.ToString() == "SquirrelAwareVersion";
			});
			if (customAttribute == null)
			{
				return null;
			}
			if (!int.TryParse(customAttribute.ConstructorArguments[1].Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var result))
			{
				return null;
			}
			return result;
		}
		catch (FileLoadException)
		{
			return null;
		}
		catch (BadImageFormatException)
		{
			return null;
		}
	}

	private static int? GetVersionBlockSquirrelAwareValue(string executable)
	{
		int fileVersionInfoSize = NativeMethods.GetFileVersionInfoSize(executable, IntPtr.Zero);
		if (fileVersionInfoSize <= 0 || fileVersionInfoSize > 4096)
		{
			return null;
		}
		byte[] array = new byte[fileVersionInfoSize];
		if (!NativeMethods.GetFileVersionInfo(executable, 0, fileVersionInfoSize, array))
		{
			return null;
		}
		if (!NativeMethods.VerQueryValue(array, "\\StringFileInfo\\040904B0\\SquirrelAwareVersion", out var _, out var _))
		{
			return null;
		}
		return 1;
	}
}
