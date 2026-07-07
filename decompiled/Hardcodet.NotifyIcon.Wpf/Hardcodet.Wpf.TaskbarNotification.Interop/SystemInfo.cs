using System;
using System.Windows.Interop;
using System.Windows.Media;

namespace Hardcodet.Wpf.TaskbarNotification.Interop;

public static class SystemInfo
{
	public static double DpiFactorX { get; private set; }

	public static double DpiFactorY { get; private set; }

	static SystemInfo()
	{
		DpiFactorX = 1.0;
		DpiFactorY = 1.0;
		UpdateDpiFactors();
	}

	internal static void UpdateDpiFactors()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		HwndSource val = new HwndSource(default(HwndSourceParameters));
		try
		{
			HwndTarget compositionTarget = val.CompositionTarget;
			if (compositionTarget != null)
			{
				_ = ((CompositionTarget)compositionTarget).TransformToDevice;
				if (true)
				{
					Matrix transformToDevice = ((CompositionTarget)val.CompositionTarget).TransformToDevice;
					DpiFactorX = ((Matrix)(ref transformToDevice)).M11;
					transformToDevice = ((CompositionTarget)val.CompositionTarget).TransformToDevice;
					DpiFactorY = ((Matrix)(ref transformToDevice)).M22;
					return;
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		DpiFactorX = (DpiFactorY = 1.0);
	}

	public static Point ScaleWithDpi(this Point point)
	{
		return new Point
		{
			X = (int)((double)point.X / DpiFactorX),
			Y = (int)((double)point.Y / DpiFactorY)
		};
	}
}
