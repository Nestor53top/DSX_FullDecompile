using System.Runtime.InteropServices;

namespace MS.Win32;

internal class NativeMethods
{
	[StructLayout(LayoutKind.Sequential)]
	public class POINT
	{
		public int x;

		public int y;

		public POINT()
		{
		}

		public POINT(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public struct RECT(int left, int top, int right, int bottom)
	{
		public int left = left;

		public int top = top;

		public int right = right;

		public int bottom = bottom;

		public int Width => right - left;

		public int Height => bottom - top;

		public bool IsEmpty
		{
			get
			{
				if (left < right)
				{
					return top >= bottom;
				}
				return true;
			}
		}

		public void Offset(int dx, int dy)
		{
			left += dx;
			top += dy;
			right += dx;
			bottom += dy;
		}
	}

	public const int SWP_NOSIZE = 1;

	public const int SWP_NOMOVE = 2;

	public const int SWP_NOZORDER = 4;

	public const int SWP_NOACTIVATE = 16;

	public const int SWP_SHOWWINDOW = 64;

	public const int SWP_HIDEWINDOW = 128;

	public const int SWP_DRAWFRAME = 32;
}
