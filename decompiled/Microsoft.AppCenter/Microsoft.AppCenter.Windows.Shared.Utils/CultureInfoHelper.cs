using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AppCenter.Windows.Shared.Utils;

internal class CultureInfoHelper
{
	private const uint LOCALE_SNAME = 92u;

	private const string LOCALE_NAME_USER_DEFAULT = null;

	private const string LOCALE_NAME_SYSTEM_DEFAULT = "!x-sys-default-locale";

	private const int BUFFER_SIZE = 530;

	[DllImport("api-ms-win-core-localization-l1-2-0.dll", CharSet = CharSet.Unicode)]
	private static extern int GetLocaleInfoEx(string lpLocaleName, uint LCType, StringBuilder lpLCData, int cchData);

	public static CultureInfo GetCurrentCulture()
	{
		string text = InvokeGetLocaleInfoEx(null, 92u);
		if (text == null)
		{
			text = InvokeGetLocaleInfoEx("!x-sys-default-locale", 92u);
			if (text == null)
			{
				return CultureInfo.CurrentCulture;
			}
		}
		return new CultureInfo(text);
	}

	private static string InvokeGetLocaleInfoEx(string lpLocaleName, uint LCType)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder(530);
			if (GetLocaleInfoEx(lpLocaleName, LCType, stringBuilder, 530) > 0)
			{
				return stringBuilder.ToString();
			}
		}
		catch (DllNotFoundException ex)
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "Failed to call GetLocaleInfoEx: " + ex.Message);
		}
		return null;
	}
}
