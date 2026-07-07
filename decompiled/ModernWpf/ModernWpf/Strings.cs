using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ModernWpf;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Strings
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
				resourceMan = new ResourceManager("ModernWpf.Resources.Strings", typeof(Strings).Assembly);
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

	internal static string AppBarMoreButtonClosedToolTip => ResourceManager.GetString("AppBarMoreButtonClosedToolTip", resourceCulture);

	internal static string AppBarMoreButtonName => ResourceManager.GetString("AppBarMoreButtonName", resourceCulture);

	internal static string AppBarMoreButtonOpenToolTip => ResourceManager.GetString("AppBarMoreButtonOpenToolTip", resourceCulture);

	internal static string IgnoreMenuItemLabel => ResourceManager.GetString("IgnoreMenuItemLabel", resourceCulture);

	internal static string ToggleSwitchOff => ResourceManager.GetString("ToggleSwitchOff", resourceCulture);

	internal static string ToggleSwitchOn => ResourceManager.GetString("ToggleSwitchOn", resourceCulture);

	internal Strings()
	{
	}
}
