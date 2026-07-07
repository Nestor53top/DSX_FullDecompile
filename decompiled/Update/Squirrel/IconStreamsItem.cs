using System;
using System.Runtime.InteropServices;

namespace Squirrel;

internal struct IconStreamsItem
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 528)]
	public byte[] exe_path;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1112)]
	public byte[] dontcare;

	public unsafe string ExePath
	{
		get
		{
			byte[] array = new byte[exe_path.Length];
			for (int i = 0; i < exe_path.Length; i++)
			{
				byte b = exe_path[i];
				if (b > 64 && b < 91)
				{
					array[i] = (byte)((b - 64 + 13) % 26 + 64);
				}
				else if (b > 96 && b < 123)
				{
					array[i] = (byte)((b - 96 + 13) % 26 + 96);
				}
				else
				{
					array[i] = b;
				}
			}
			fixed (byte* ptr = array)
			{
				return Marshal.PtrToStringUni((IntPtr)ptr);
			}
		}
	}
}
