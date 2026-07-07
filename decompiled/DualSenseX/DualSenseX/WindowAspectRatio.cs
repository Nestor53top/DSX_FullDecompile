using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DualSenseX;

internal class WindowAspectRatio
{
	internal enum WM
	{
		WINDOWPOSCHANGING = 70
	}

	[Flags]
	public enum SWP
	{
		NoMove = 2
	}

	internal struct WINDOWPOS
	{
		public IntPtr hwnd;

		public IntPtr hwndInsertAfter;

		public int x;

		public int y;

		public int cx;

		public int cy;

		public int flags;
	}

	private double _ratio;

	private WindowAspectRatio(Window window)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		_ratio = ((FrameworkElement)window).Width / ((FrameworkElement)window).Height;
		((HwndSource)PresentationSource.FromVisual((Visual)(object)window)).AddHook(new HwndSourceHook(DragHook));
	}

	public static void Register(Window window)
	{
		new WindowAspectRatio(window);
	}

	private IntPtr DragHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handeled)
	{
		if (msg == 70)
		{
			WINDOWPOS structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
			if ((structure.flags & 2) != 0 || ((PresentationSource)HwndSource.FromHwnd(hwnd)).RootVisual == null)
			{
				return IntPtr.Zero;
			}
			structure.cx = (int)((double)structure.cy * _ratio);
			Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
			handeled = true;
		}
		return IntPtr.Zero;
	}
}
