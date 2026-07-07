using System.Drawing;

namespace Hardcodet.Wpf.TaskbarNotification.Interop;

public static class TrayInfo
{
	public static Point GetTrayLocation()
	{
		int num = 2;
		AppBarInfo appBarInfo = new AppBarInfo();
		appBarInfo.GetSystemTaskBarPosition();
		Rectangle workArea = appBarInfo.WorkArea;
		int x = 0;
		int y = 0;
		switch (appBarInfo.Edge)
		{
		case AppBarInfo.ScreenEdge.Left:
			x = workArea.Right + num;
			y = workArea.Bottom;
			break;
		case AppBarInfo.ScreenEdge.Bottom:
			x = workArea.Right;
			y = workArea.Bottom - workArea.Height - num;
			break;
		case AppBarInfo.ScreenEdge.Top:
			x = workArea.Right;
			y = workArea.Top + workArea.Height + num;
			break;
		case AppBarInfo.ScreenEdge.Right:
			x = workArea.Right - workArea.Width - num;
			y = workArea.Bottom;
			break;
		}
		return GetDeviceCoordinates(new Point
		{
			X = x,
			Y = y
		});
	}

	public static Point GetDeviceCoordinates(Point point)
	{
		return point.ScaleWithDpi();
	}
}
