using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Squirrel.SimpleSplat;

internal class WrappingFullLogger : IFullLogger, ILogger
{
	private readonly ILogger _inner;

	private readonly string prefix;

	private readonly MethodInfo stringFormat;

	public LogLevel Level
	{
		get
		{
			return _inner.Level;
		}
		set
		{
			_inner.Level = value;
		}
	}

	public WrappingFullLogger(ILogger inner, Type callingType)
	{
		_inner = inner;
		prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", new object[1] { callingType.Name });
		stringFormat = typeof(string).GetMethod("Format", new Type[3]
		{
			typeof(IFormatProvider),
			typeof(string),
			typeof(object[])
		});
	}

	private string InvokeStringFormat(IFormatProvider formatProvider, string message, object[] args)
	{
		object[] parameters = new object[3] { formatProvider, message, args };
		return (string)stringFormat.Invoke(null, parameters);
	}

	public void Debug<T>(T value)
	{
		ILogger inner = _inner;
		string text = prefix;
		T val = value;
		inner.Write(text + val, LogLevel.Debug);
	}

	public void Debug<T>(IFormatProvider formatProvider, T value)
	{
		_inner.Write(string.Format(formatProvider, "{0}{1}", new object[2] { prefix, value }), LogLevel.Debug);
	}

	public void DebugException(string message, Exception exception)
	{
		_inner.Write($"{prefix}{message}: {exception}", LogLevel.Debug);
	}

	public void Debug(IFormatProvider formatProvider, string message, params object[] args)
	{
		string text = InvokeStringFormat(formatProvider, message, args);
		_inner.Write(prefix + text, LogLevel.Debug);
	}

	public void Debug(string message)
	{
		_inner.Write(prefix + message, LogLevel.Debug);
	}

	public void Debug(string message, params object[] args)
	{
		string text = InvokeStringFormat(CultureInfo.InvariantCulture, message, args);
		_inner.Write(prefix + text, LogLevel.Debug);
	}

	public void Debug<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[1] { argument }), LogLevel.Debug);
	}

	public void Debug<TArgument>(string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[1] { argument }), LogLevel.Debug);
	}

	public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[2] { argument1, argument2 }), LogLevel.Debug);
	}

	public void Debug<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[2] { argument1, argument2 }), LogLevel.Debug);
	}

	public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Debug);
	}

	public void Debug<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Debug);
	}

	public void Info<T>(T value)
	{
		ILogger inner = _inner;
		string text = prefix;
		T val = value;
		inner.Write(text + val, LogLevel.Info);
	}

	public void Info<T>(IFormatProvider formatProvider, T value)
	{
		_inner.Write(string.Format(formatProvider, "{0}{1}", new object[2] { prefix, value }), LogLevel.Info);
	}

	public void InfoException(string message, Exception exception)
	{
		_inner.Write($"{prefix}{message}: {exception}", LogLevel.Info);
	}

	public void Info(IFormatProvider formatProvider, string message, params object[] args)
	{
		string text = InvokeStringFormat(formatProvider, message, args);
		_inner.Write(prefix + text, LogLevel.Info);
	}

	public void Info(string message)
	{
		_inner.Write(prefix + message, LogLevel.Info);
	}

	public void Info(string message, params object[] args)
	{
		string text = InvokeStringFormat(CultureInfo.InvariantCulture, message, args);
		_inner.Write(prefix + text, LogLevel.Info);
	}

	public void Info<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[1] { argument }), LogLevel.Info);
	}

	public void Info<TArgument>(string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[1] { argument }), LogLevel.Info);
	}

	public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[2] { argument1, argument2 }), LogLevel.Info);
	}

	public void Info<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[2] { argument1, argument2 }), LogLevel.Info);
	}

	public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Info);
	}

	public void Info<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Info);
	}

	public void Warn<T>(T value)
	{
		ILogger inner = _inner;
		string text = prefix;
		T val = value;
		inner.Write(text + val, LogLevel.Warn);
	}

	public void Warn<T>(IFormatProvider formatProvider, T value)
	{
		_inner.Write(string.Format(formatProvider, "{0}{1}", new object[2] { prefix, value }), LogLevel.Warn);
	}

	public void WarnException(string message, Exception exception)
	{
		_inner.Write($"{prefix}{message}: {exception}", LogLevel.Warn);
	}

	public void Warn(IFormatProvider formatProvider, string message, params object[] args)
	{
		string text = InvokeStringFormat(formatProvider, message, args);
		_inner.Write(prefix + text, LogLevel.Warn);
	}

	public void Warn(string message)
	{
		_inner.Write(prefix + message, LogLevel.Warn);
	}

	public void Warn(string message, params object[] args)
	{
		string text = InvokeStringFormat(CultureInfo.InvariantCulture, message, args);
		_inner.Write(prefix + text, LogLevel.Warn);
	}

	public void Warn<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[1] { argument }), LogLevel.Warn);
	}

	public void Warn<TArgument>(string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[1] { argument }), LogLevel.Warn);
	}

	public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[2] { argument1, argument2 }), LogLevel.Warn);
	}

	public void Warn<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[2] { argument1, argument2 }), LogLevel.Warn);
	}

	public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Warn);
	}

	public void Warn<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Warn);
	}

	public void Error<T>(T value)
	{
		ILogger inner = _inner;
		string text = prefix;
		T val = value;
		inner.Write(text + val, LogLevel.Error);
	}

	public void Error<T>(IFormatProvider formatProvider, T value)
	{
		_inner.Write(string.Format(formatProvider, "{0}{1}", new object[2] { prefix, value }), LogLevel.Error);
	}

	public void ErrorException(string message, Exception exception)
	{
		_inner.Write($"{prefix}{message}: {exception}", LogLevel.Error);
	}

	public void Error(IFormatProvider formatProvider, string message, params object[] args)
	{
		string text = InvokeStringFormat(formatProvider, message, args);
		_inner.Write(prefix + text, LogLevel.Error);
	}

	public void Error(string message)
	{
		_inner.Write(prefix + message, LogLevel.Error);
	}

	public void Error(string message, params object[] args)
	{
		string text = InvokeStringFormat(CultureInfo.InvariantCulture, message, args);
		_inner.Write(prefix + text, LogLevel.Error);
	}

	public void Error<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[1] { argument }), LogLevel.Error);
	}

	public void Error<TArgument>(string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[1] { argument }), LogLevel.Error);
	}

	public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[2] { argument1, argument2 }), LogLevel.Error);
	}

	public void Error<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[2] { argument1, argument2 }), LogLevel.Error);
	}

	public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Error);
	}

	public void Error<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Error);
	}

	public void Fatal<T>(T value)
	{
		ILogger inner = _inner;
		string text = prefix;
		T val = value;
		inner.Write(text + val, LogLevel.Fatal);
	}

	public void Fatal<T>(IFormatProvider formatProvider, T value)
	{
		_inner.Write(string.Format(formatProvider, "{0}{1}", new object[2] { prefix, value }), LogLevel.Fatal);
	}

	public void FatalException(string message, Exception exception)
	{
		_inner.Write($"{prefix}{message}: {exception}", LogLevel.Fatal);
	}

	public void Fatal(IFormatProvider formatProvider, string message, params object[] args)
	{
		string text = InvokeStringFormat(formatProvider, message, args);
		_inner.Write(prefix + text, LogLevel.Fatal);
	}

	public void Fatal(string message)
	{
		_inner.Write(prefix + message, LogLevel.Fatal);
	}

	public void Fatal(string message, params object[] args)
	{
		string text = InvokeStringFormat(CultureInfo.InvariantCulture, message, args);
		_inner.Write(prefix + text, LogLevel.Fatal);
	}

	public void Fatal<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[1] { argument }), LogLevel.Fatal);
	}

	public void Fatal<TArgument>(string message, TArgument argument)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[1] { argument }), LogLevel.Fatal);
	}

	public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[2] { argument1, argument2 }), LogLevel.Fatal);
	}

	public void Fatal<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[2] { argument1, argument2 }), LogLevel.Fatal);
	}

	public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(formatProvider, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Fatal);
	}

	public void Fatal<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
	{
		_inner.Write(prefix + string.Format(CultureInfo.InvariantCulture, message, new object[3] { argument1, argument2, argument3 }), LogLevel.Fatal);
	}

	public void Write([Localizable(false)] string message, LogLevel logLevel)
	{
		_inner.Write(message, logLevel);
	}
}
