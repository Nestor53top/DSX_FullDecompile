using System;

namespace HidSharp;

internal static class CommonException
{
	public static ObjectDisposedException CreateClosedException()
	{
		return new ObjectDisposedException("Closed.", (Exception?)null);
	}
}
