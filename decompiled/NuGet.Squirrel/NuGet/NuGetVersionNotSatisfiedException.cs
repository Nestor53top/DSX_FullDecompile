using System;

namespace NuGet;

[Serializable]
public class NuGetVersionNotSatisfiedException : Exception
{
	public NuGetVersionNotSatisfiedException()
	{
	}

	public NuGetVersionNotSatisfiedException(string message)
		: base(message)
	{
	}

	public NuGetVersionNotSatisfiedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
