#define TRACE
using System;
using System.Diagnostics;

namespace HidSharp.Utility;

public static class HidSharpDiagnostics
{
	public static bool EnableTracing { get; set; }

	public static bool PerformStrictChecks { get; set; }

	static HidSharpDiagnostics()
	{
		PerformStrictChecks = false;
	}

	internal static void PerformStrictCheck(bool condition, string message)
	{
		if (!condition)
		{
			message += "\n\nTo disable this exception, set HidSharpDiagnostics.PerformStrictChecks to false.";
			throw new InvalidOperationException(message);
		}
	}

	internal static void Trace(string message)
	{
		if (EnableTracing)
		{
			System.Diagnostics.Trace.WriteLine(message, "HIDSharp");
		}
	}

	internal static void Trace(string formattedMessage, object arg)
	{
		if (EnableTracing)
		{
			Trace(string.Format(formattedMessage, arg));
		}
	}

	internal static void Trace(string formattedMessage, params object[] args)
	{
		if (EnableTracing)
		{
			Trace(string.Format(formattedMessage, args));
		}
	}
}
