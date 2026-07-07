using System;
using System.IO;
using System.Reflection;
using System.Text;
using Squirrel.SimpleSplat;

namespace Squirrel.Update;

internal class SetupLogLogger : ILogger, IDisposable
{
	private TextWriter inner;

	private readonly object gate = 42;

	public LogLevel Level { get; set; }

	public SetupLogLogger(bool saveInTemp, string commandSuffix = null)
	{
		for (int i = 0; i < 10; i++)
		{
			try
			{
				string path = (saveInTemp ? Path.GetTempPath() : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
				string text = ((commandSuffix == null) ? string.Format($"Squirrel.{i}.log", i) : string.Format($"Squirrel-{commandSuffix}.{i}.log", i));
				FileStream stream = File.Open(Path.Combine(path, text.Replace(".0.log", ".log")), FileMode.Append, FileAccess.Write, FileShare.Read);
				inner = new StreamWriter(stream, Encoding.UTF8, 4096, false)
				{
					AutoFlush = true
				};
				return;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Couldn't open log file, trying new file: " + ex.ToString());
			}
		}
		inner = Console.Error;
	}

	public void Write(string message, LogLevel logLevel)
	{
		if (logLevel < Level)
		{
			return;
		}
		lock (gate)
		{
			inner.WriteLine("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] " + logLevel.ToString().ToLower() + ": " + message);
		}
	}

	public void Dispose()
	{
		lock (gate)
		{
			inner.Flush();
			inner.Dispose();
		}
	}
}
