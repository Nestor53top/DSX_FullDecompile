using System;

namespace Device.Net;

public class DebugLogger : ILogger
{
	public bool LogToConsole { get; set; }

	public void Log(string message, string region, Exception ex, LogLevel logLevel)
	{
		string text = string.Format("Message: {0}\r\nLevel: {1}\r\nTime: {2}\r\nSection: {3}\r\nError: {4}", new object[5]
		{
			message,
			logLevel,
			DateTime.Now,
			region,
			ex
		});
		text = "--------------------------------------\r\n" + text + "\r\n--------------------------------------";
		if (LogToConsole)
		{
			Console.WriteLine(text);
		}
	}
}
