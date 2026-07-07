using System;

namespace Microsoft.AppCenter.Utils.Synchronization;

public class StatefulMutexException : AppCenterException
{
	public StatefulMutexException(string message)
		: base(message)
	{
	}

	public StatefulMutexException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
