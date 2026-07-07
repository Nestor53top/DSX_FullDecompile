using System;
using System.Runtime.InteropServices;

namespace DualSenseX;

public class libHaptController
{
	public const int CONTROLLER_DATA_MAX_NUM = 64;

	[DllImport("libVibrationDesigner")]
	public static extern void InitController();

	[DllImport("libVibrationDesigner")]
	public static extern int UpdateController(int handle);

	[DllImport("libVibrationDesigner")]
	public static extern int CloseController(int handle);

	[DllImport("libVibrationDesigner")]
	public static extern int GetControllerHandle();

	[DllImport("libVibrationDesigner")]
	public static extern bool IsConnected(int handle);

	[DllImport("libVibrationDesigner")]
	public static extern int GetDigitalButtons(int handle, IntPtr buttonFlags, int num);
}
