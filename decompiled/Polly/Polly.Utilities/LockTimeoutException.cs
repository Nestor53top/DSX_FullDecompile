using System;

namespace Polly.Utilities;

internal class LockTimeoutException : Exception
{
	public LockTimeoutException()
		: base("Timeout waiting for lock")
	{
	}
}
