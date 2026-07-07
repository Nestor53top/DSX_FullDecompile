using System;

namespace Polly.Utilities;

internal static class KeyHelper
{
	public static string GuidPart()
	{
		return Guid.NewGuid().ToString().Substring(0, 8);
	}
}
