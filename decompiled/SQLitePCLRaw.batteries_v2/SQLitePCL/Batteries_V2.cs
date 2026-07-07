using System;
using System.Reflection;

namespace SQLitePCL;

public static class Batteries_V2
{
	private class MyGetFunctionPointer : IGetFunctionPointer
	{
		private readonly IntPtr _dll;

		public MyGetFunctionPointer(IntPtr dll)
		{
			_dll = dll;
		}

		public IntPtr GetFunctionPointer(string name)
		{
			if (NativeLibrary.TryGetExport(_dll, name, out var address))
			{
				return address;
			}
			return IntPtr.Zero;
		}
	}

	private static IGetFunctionPointer MakeDynamic(string name, int flags)
	{
		Assembly assembly = typeof(raw).Assembly;
		return new MyGetFunctionPointer(NativeLibrary.Load(name, assembly, flags));
	}

	private static void DoDynamic_cdecl(string name, int flags)
	{
		IGetFunctionPointer gf = MakeDynamic(name, flags);
		SQLite3Provider_dynamic_cdecl.Setup(name, gf);
		raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
	}

	public static void Init()
	{
		DoDynamic_cdecl("e_sqlite3", 2);
	}
}
