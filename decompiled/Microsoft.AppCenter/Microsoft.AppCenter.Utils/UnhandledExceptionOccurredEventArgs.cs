using System;

namespace Microsoft.AppCenter.Utils;

public class UnhandledExceptionOccurredEventArgs : EventArgs
{
	public Exception Exception { get; }

	public UnhandledExceptionOccurredEventArgs(Exception e)
	{
		Exception = e;
	}
}
