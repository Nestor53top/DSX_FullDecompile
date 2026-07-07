using System.Runtime.InteropServices;
using System.Text;

namespace ModernWpf;

internal static class PackagedAppHelper
{
	private const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

	public static bool IsPackagedApp
	{
		get
		{
			if (OSVersionHelper.IsWindows8OrGreater)
			{
				int packageFullNameLength = 0;
				StringBuilder stringBuilder = new StringBuilder(0);
				GetCurrentPackageFullName(ref packageFullNameLength, stringBuilder);
				stringBuilder.Length = packageFullNameLength;
				return (long)GetCurrentPackageFullName(ref packageFullNameLength, stringBuilder) != 15700;
			}
			return false;
		}
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);
}
