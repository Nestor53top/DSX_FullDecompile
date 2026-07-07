using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace NuGet;

[CompilerGenerated]
internal static class CommonResources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				string text = Assembly.GetExecutingAssembly().GetManifestResourceNames().First((string s) => s.EndsWith("CommonResources.resources", StringComparison.OrdinalIgnoreCase));
				text = text.Substring(0, text.Length - 10);
				resourceMan = new ResourceManager(text, typeof(CommonResources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string Argument_Cannot_Be_Null_Or_Empty => ResourceManager.GetString("Argument_Cannot_Be_Null_Or_Empty", resourceCulture);

	internal static string Argument_Must_Be_Between => ResourceManager.GetString("Argument_Must_Be_Between", resourceCulture);

	internal static string Argument_Must_Be_Enum_Member => ResourceManager.GetString("Argument_Must_Be_Enum_Member", resourceCulture);

	internal static string Argument_Must_Be_GreaterThan => ResourceManager.GetString("Argument_Must_Be_GreaterThan", resourceCulture);

	internal static string Argument_Must_Be_GreaterThanOrEqualTo => ResourceManager.GetString("Argument_Must_Be_GreaterThanOrEqualTo", resourceCulture);

	internal static string Argument_Must_Be_LessThan => ResourceManager.GetString("Argument_Must_Be_LessThan", resourceCulture);

	internal static string Argument_Must_Be_LessThanOrEqualTo => ResourceManager.GetString("Argument_Must_Be_LessThanOrEqualTo", resourceCulture);

	internal static string Argument_Must_Be_Null_Or_Non_Empty => ResourceManager.GetString("Argument_Must_Be_Null_Or_Non_Empty", resourceCulture);

	internal static string EnsureImportedMessage => ResourceManager.GetString("EnsureImportedMessage", resourceCulture);
}
