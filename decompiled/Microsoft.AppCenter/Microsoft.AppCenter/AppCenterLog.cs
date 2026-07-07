#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.AppCenter;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AppCenterLog
{
	private static readonly object LogLock;

	private static volatile LogLevel _level;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string LogTag { get; private set; }

	internal static LogLevel Level
	{
		get
		{
			return _level;
		}
		set
		{
			lock (LogLock)
			{
				_level = value;
			}
		}
	}

	static AppCenterLog()
	{
		LogLock = new object();
		_level = LogLevel.Assert;
		LogTag = "AppCenter";
		if (Debugger.IsAttached)
		{
			_level = LogLevel.Warn;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Verbose(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Verbose, "VERBOSE");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Debug(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Debug, "DEBUG");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Info(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Info, "INFO");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Warn(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Warn, "WARN");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Error(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Error, "ERROR");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Assert(string tag, string message)
	{
		LogMessage(tag, message, LogLevel.Assert, "ASSERT");
	}

	private static void LogMessage(string tag, string message, LogLevel level, string levelName)
	{
		lock (LogLock)
		{
			if (Level <= level)
			{
				string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
				System.Diagnostics.Debug.WriteLine(text + " [" + tag + "] " + levelName + ": " + message);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Verbose(string tag, string message, Exception exception)
	{
		Verbose(tag, ConcatMessageException(message, exception));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Debug(string tag, string message, Exception exception)
	{
		Debug(tag, ConcatMessageException(message, exception));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Info(string tag, string message, Exception exception)
	{
		Info(tag, ConcatMessageException(message, exception));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Warn(string tag, string message, Exception exception)
	{
		Warn(tag, ConcatMessageException(message, exception));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Error(string tag, string message, Exception exception)
	{
		Error(tag, ConcatMessageException(message, exception));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Assert(string tag, string message, Exception exception)
	{
		Assert(tag, ConcatMessageException(message, exception));
	}

	private static string ConcatMessageException(string message, Exception exception)
	{
		return message + "\n" + exception;
	}
}
