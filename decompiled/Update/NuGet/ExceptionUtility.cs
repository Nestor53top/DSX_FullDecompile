using System;
using System.Reflection;

namespace NuGet;

internal static class ExceptionUtility
{
	public static Exception Unwrap(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		if (exception.InnerException == null)
		{
			return exception;
		}
		if (exception is AggregateException || exception is TargetInvocationException)
		{
			return exception.GetBaseException();
		}
		return exception;
	}
}
