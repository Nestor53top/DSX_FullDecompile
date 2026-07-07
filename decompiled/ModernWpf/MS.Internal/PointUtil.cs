using System.Windows;
using MS.Win32;

namespace MS.Internal;

internal static class PointUtil
{
	internal static Rect ToRect(NativeMethods.RECT rc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Rect result = default(Rect);
		((Rect)(ref result)).X = rc.left;
		((Rect)(ref result)).Y = rc.top;
		((Rect)(ref result)).Width = rc.right - rc.left;
		((Rect)(ref result)).Height = rc.bottom - rc.top;
		return result;
	}
}
