using System;

namespace NuGet;

public class PackageNotInstalledException : Exception
{
	public PackageNotInstalledException()
	{
	}

	public PackageNotInstalledException(string message)
		: base(message)
	{
	}

	public PackageNotInstalledException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
