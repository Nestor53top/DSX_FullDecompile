using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NuGet.Runtime;

internal static class AssemblyNameExtensions
{
	public static string GetPublicKeyTokenString(this AssemblyName assemblyName)
	{
		return string.Join(string.Empty, from b in assemblyName.GetPublicKeyToken()
			select b.ToString("x2", CultureInfo.InvariantCulture));
	}
}
