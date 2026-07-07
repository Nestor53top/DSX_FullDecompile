using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Squirrel.SimpleSplat;

internal class PlatformModeDetector : IModeDetector
{
	public bool? InUnitTestRunner()
	{
		string[] assemblyList = new string[5] { "CSUNIT", "NUNIT", "XUNIT", "MBUNIT", "NBEHAVE" };
		try
		{
			return searchForAssembly(assemblyList);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public bool? InDesignMode()
	{
		string[] source = new string[2] { "BLEND.EXE", "XDESPROC.EXE" };
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		if (entryAssembly != null)
		{
			string exeName = new FileInfo(entryAssembly.Location).Name.ToUpperInvariant();
			if (source.Any((string x) => x.Contains(exeName)))
			{
				return true;
			}
		}
		return false;
	}

	private static bool searchForAssembly(IEnumerable<string> assemblyList)
	{
		return AppDomain.CurrentDomain.GetAssemblies().Any((Assembly x) => assemblyList.Any((string name) => x.FullName.ToUpperInvariant().Contains(name)));
	}
}
