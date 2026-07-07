using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public static class NativeLibrary
{
	private static class NativeLib_dlopen
	{
		private const string SO = "dl";

		public const int RTLD_NOW = 2;

		[DllImport("dl")]
		public static extern IntPtr dlopen(string dllToLoad, int flags);

		[DllImport("dl")]
		public static extern IntPtr dlsym(IntPtr hModule, string procedureName);

		[DllImport("dl")]
		public static extern int dlclose(IntPtr hModule);
	}

	private static class NativeLib_Win
	{
		public const uint LOAD_WITH_ALTERED_SEARCH_PATH = 8u;

		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FreeLibrary(IntPtr hModule);
	}

	private enum LibSuffix
	{
		DLL,
		DYLIB,
		SO
	}

	private enum Loader
	{
		win,
		dlopen
	}

	public const int WHERE_PLAIN = 1;

	public const int WHERE_RUNTIME_RID = 2;

	public const int WHERE_ARCH = 4;

	public static IntPtr Load(string libraryName, Assembly assy, int flags)
	{
		IntPtr intPtr = MyLoad(libraryName, assy, flags, delegate
		{
		});
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception("Library " + libraryName + " not found");
		}
		return intPtr;
	}

	private static IntPtr MyGetExport(IntPtr handle, string name)
	{
		return WhichLoader() switch
		{
			Loader.win => NativeLib_Win.GetProcAddress(handle, name), 
			Loader.dlopen => NativeLib_dlopen.dlsym(handle, name), 
			_ => throw new NotImplementedException(), 
		};
	}

	public static IntPtr GetExport(IntPtr handle, string name)
	{
		IntPtr intPtr = MyGetExport(handle, name);
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception("Symbol " + name + " not found");
		}
		return intPtr;
	}

	public static void Free(IntPtr handle)
	{
		switch (WhichLoader())
		{
		case Loader.win:
			NativeLib_Win.FreeLibrary(handle);
			break;
		case Loader.dlopen:
			NativeLib_dlopen.dlclose(handle);
			break;
		default:
			throw new NotImplementedException();
		}
	}

	public static bool TryLoad(string libraryName, Assembly assy, int flags, out IntPtr handle)
	{
		return (handle = MyLoad(libraryName, assy, flags, delegate
		{
		})) != IntPtr.Zero;
	}

	public static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
	{
		return (address = MyGetExport(handle, name)) != IntPtr.Zero;
	}

	private static string basename_to_libname(string basename, LibSuffix suffix)
	{
		return suffix switch
		{
			LibSuffix.DLL => $"{basename}.dll", 
			LibSuffix.DYLIB => $"lib{basename}.dylib", 
			LibSuffix.SO => $"lib{basename}.so", 
			_ => throw new NotImplementedException(), 
		};
	}

	private static bool TryLoad(string name, Loader plat, Action<string> log, out IntPtr h)
	{
		try
		{
			switch (plat)
			{
			case Loader.win:
			{
				log("win TryLoad: " + name);
				IntPtr intPtr2 = NativeLib_Win.LoadLibrary(name);
				if (intPtr2 != IntPtr.Zero)
				{
					log($"LoadLibrary gave: {intPtr2}");
					h = intPtr2;
					return true;
				}
				Marshal.GetLastWin32Error();
				throw new Win32Exception();
			}
			case Loader.dlopen:
			{
				log("dlopen TryLoad: " + name);
				IntPtr intPtr = NativeLib_dlopen.dlopen(name, 2);
				log($"dlopen gave: {intPtr}");
				if (intPtr != IntPtr.Zero)
				{
					h = intPtr;
					return true;
				}
				h = IntPtr.Zero;
				return false;
			}
			default:
				throw new NotImplementedException();
			}
		}
		catch (NotImplementedException)
		{
			throw;
		}
		catch (Exception arg)
		{
			log($"thrown: {arg}");
			h = IntPtr.Zero;
			return false;
		}
	}

	private static Loader WhichLoader()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return Loader.win;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return Loader.dlopen;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return Loader.dlopen;
		}
		throw new NotImplementedException();
	}

	private static LibSuffix WhichLibSuffix()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return LibSuffix.DLL;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return LibSuffix.SO;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return LibSuffix.DYLIB;
		}
		throw new NotImplementedException();
	}

	private static string get_rid_front()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return "win";
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return "linux";
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return "osx";
		}
		throw new NotImplementedException();
	}

	private static string get_rid_back()
	{
		switch (RuntimeInformation.OSArchitecture)
		{
		case Architecture.Arm:
			return "arm";
		case Architecture.Arm64:
			return "arm64";
		case Architecture.X64:
			if (IntPtr.Size != 8)
			{
				return "x86";
			}
			return "x64";
		case Architecture.X86:
			return "x86";
		default:
			throw new NotImplementedException();
		}
	}

	private static string get_rid()
	{
		string rid_front = get_rid_front();
		string rid_back = get_rid_back();
		return rid_front + "-" + rid_back;
	}

	private static bool Search(IList<string> a, Loader plat, Action<string> log, out string name, out IntPtr h)
	{
		foreach (string item in a)
		{
			if (TryLoad(item, plat, log, out var h2))
			{
				name = item;
				h = h2;
				return true;
			}
		}
		name = null;
		h = IntPtr.Zero;
		return false;
	}

	private static List<string> MakePossibilitiesFor(string basename, Assembly assy, int flags, LibSuffix suffix)
	{
		List<string> list = new List<string>();
		string text = basename_to_libname(basename, suffix);
		if ((flags & 1) != 0)
		{
			list.Add(text);
		}
		if ((flags & 2) != 0)
		{
			string rid = get_rid();
			string directoryName = Path.GetDirectoryName(assy.Location);
			list.Add(Path.Combine(directoryName, "runtimes", rid, "native", text));
		}
		if ((flags & 4) != 0)
		{
			string directoryName2 = Path.GetDirectoryName(assy.Location);
			string rid_back = get_rid_back();
			list.Add(Path.Combine(directoryName2, rid_back, text));
		}
		return list;
	}

	private static IntPtr MyLoad(string basename, Assembly assy, int flags, Action<string> log)
	{
		Loader loader = WhichLoader();
		log($"plat: {loader}");
		LibSuffix libSuffix = WhichLibSuffix();
		log($"suffix: {libSuffix}");
		List<string> list = MakePossibilitiesFor(basename, assy, flags, libSuffix);
		log("possibilities:");
		foreach (string item in list)
		{
			log("    " + item);
		}
		if (Search(list, loader, log, out var name, out var h))
		{
			log("found: " + name);
			return h;
		}
		log("NOT FOUND");
		return IntPtr.Zero;
	}
}
