using System.Runtime.CompilerServices;
using System.Windows.Media;
using Windows.UI;

namespace ModernWpf;

internal static class WinRTColorHelper
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Color ToColor(this Color color)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return Color.FromArgb(((Color)(ref color)).A, ((Color)(ref color)).R, ((Color)(ref color)).G, ((Color)(ref color)).B);
	}
}
