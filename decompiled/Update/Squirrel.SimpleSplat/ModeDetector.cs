using System;
using System.Reflection;

namespace Squirrel.SimpleSplat;

internal static class ModeDetector
{
	private static bool? cachedInUnitTestRunnerResult;

	private static bool? cachedInDesignModeResult;

	private static IModeDetector current { get; set; }

	static ModeDetector()
	{
		current = AssemblyFinder.AttemptToLoadType<IModeDetector>("Squirrel.SimpleSplat.PlatformModeDetector");
	}

	public static void OverrideModeDetector(IModeDetector modeDetector)
	{
		current = modeDetector;
		cachedInDesignModeResult = null;
		cachedInUnitTestRunnerResult = null;
	}

	public static bool InUnitTestRunner()
	{
		if (cachedInUnitTestRunnerResult.HasValue)
		{
			return cachedInUnitTestRunnerResult.Value;
		}
		if (current != null)
		{
			cachedInUnitTestRunnerResult = current.InUnitTestRunner();
			if (cachedInUnitTestRunnerResult.HasValue)
			{
				return cachedInUnitTestRunnerResult.Value;
			}
		}
		return false;
	}

	public static bool InDesignMode()
	{
		if (cachedInDesignModeResult.HasValue)
		{
			return cachedInDesignModeResult.Value;
		}
		if (current != null)
		{
			cachedInDesignModeResult = current.InDesignMode();
			if (cachedInDesignModeResult.HasValue)
			{
				return cachedInDesignModeResult.Value;
			}
		}
		Type type = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", throwOnError: false);
		if (type != null)
		{
			MethodInfo method = type.GetMethod("GetIsInDesignMode");
			Type type2 = Type.GetType("System.Windows.Controls.Border, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", throwOnError: false);
			if (type2 != null)
			{
				cachedInDesignModeResult = (bool)method.Invoke(null, new object[1] { Activator.CreateInstance(type2) });
			}
		}
		else if ((type = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", throwOnError: false)) != null)
		{
			MethodInfo method2 = type.GetMethod("GetIsInDesignMode");
			Type type3 = Type.GetType("System.Windows.DependencyObject, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", throwOnError: false);
			if (type3 != null)
			{
				cachedInDesignModeResult = (bool)method2.Invoke(null, new object[1] { Activator.CreateInstance(type3) });
			}
		}
		else if ((type = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime", throwOnError: false)) != null)
		{
			cachedInDesignModeResult = (bool)type.GetProperty("DesignModeEnabled").GetMethod.Invoke(null, null);
		}
		else
		{
			cachedInDesignModeResult = false;
		}
		return cachedInDesignModeResult == true;
	}
}
