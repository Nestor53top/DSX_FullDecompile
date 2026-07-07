using System;
using System.Runtime.ExceptionServices;

namespace Polly.Utilities;

public static class ExceptionExtensions
{
	public static void RethrowWithOriginalStackTraceIfDiffersFrom(this Exception exceptionPossiblyToThrow, Exception exceptionToCompare)
	{
		if (exceptionPossiblyToThrow != exceptionToCompare)
		{
			ExceptionDispatchInfo.Capture(exceptionPossiblyToThrow).Throw();
		}
	}
}
