using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HidSharp.Utility;

[Obsolete("This class is experimental and its functionality may be moved elsewhere in the future. Please do not rely on it.")]
public static class HResult
{
	public const int FileNotFound = -2147024894;

	public const int SharingViolation = -2147024864;

	public const int SemTimeout = -2147024775;

	public static int FromException(Exception exception)
	{
		Throw.If.Null(exception);
		try
		{
			return (int)exception.GetType().InvokeMember("HResult", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, exception, new object[0]);
		}
		catch
		{
			return Marshal.GetHRForException(exception);
		}
	}
}
