using System.Globalization;

namespace Device.Net;

public static class Helpers
{
	public static CultureInfo ParsingCulture { get; } = new CultureInfo("en-US");

	public static bool ContainsIgnoreCase(this string paragraph, string word)
	{
		return ParsingCulture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
	}

	public static string GetHex(uint? id)
	{
		return id?.ToString("X").ToLower().PadLeft(4, '0');
	}
}
