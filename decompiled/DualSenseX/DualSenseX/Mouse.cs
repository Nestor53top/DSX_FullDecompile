using System;
using System.Runtime.InteropServices;

namespace DualSenseX;

internal class Mouse
{
	internal struct INPUT
	{
		public uint Type;

		public MOUSEKEYBDHARDWAREINPUT Data;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct MOUSEKEYBDHARDWAREINPUT
	{
		[FieldOffset(0)]
		public HARDWAREINPUT Hardware;

		[FieldOffset(0)]
		public KEYBDINPUT Keyboard;

		[FieldOffset(0)]
		public MOUSEINPUT Mouse;
	}

	internal struct HARDWAREINPUT
	{
		public uint Msg;

		public ushort ParamL;

		public ushort ParamH;
	}

	internal struct KEYBDINPUT
	{
		public ushort Vk;

		public ushort Scan;

		public uint Flags;

		public uint Time;

		public IntPtr ExtraInfo;
	}

	internal struct MOUSEINPUT
	{
		public int X;

		public int Y;

		public uint MouseData;

		public uint Flags;

		public uint Time;

		public IntPtr ExtraInfo;
	}

	private static INPUT[] sendInputs = new INPUT[2];

	private DateTime pastTime;

	private DateTime touchpadButtonPress;

	private DateTime touchpadTapPress;

	private Touch firstTouch;

	private int deviceNum;

	private bool rightClick;

	internal const uint INPUT_MOUSE = 0u;

	internal const uint INPUT_KEYBOARD = 1u;

	internal const uint INPUT_HARDWARE = 2u;

	internal const uint MOUSEEVENTF_MOVE = 1u;

	internal const uint MOUSEEVENTF_LEFTDOWN = 2u;

	internal const uint MOUSEEVENTF_LEFTUP = 4u;

	internal const uint MOUSEEVENTF_RIGHTDOWN = 8u;

	internal const uint MOUSEEVENTF_RIGHTUP = 16u;

	internal const uint MOUSEEVENTF_MIDDLEDOWN = 32u;

	internal const uint MOUSEEVENTF_MIDDLEUP = 64u;

	internal const uint KEYEVENTF_KEYUP = 2u;

	internal const uint MOUSEEVENTF_WHEEL = 2048u;

	internal const uint MOUSEEVENTF_HWHEEL = 4096u;

	internal const uint KEYEVENTF_SCANCODE = 8u;

	internal const uint MAPVK_VK_TO_VSC = 0u;

	public Mouse(int deviceID)
	{
		deviceNum = deviceID;
	}

	private void MoveCursorBy(int x, int y)
	{
		if (x != 0 || y != 0)
		{
			sendInputs[0].Type = 0u;
			sendInputs[0].Data.Mouse.ExtraInfo = IntPtr.Zero;
			sendInputs[0].Data.Mouse.Flags = 1u;
			sendInputs[0].Data.Mouse.MouseData = 0u;
			sendInputs[0].Data.Mouse.Time = 0u;
			sendInputs[0].Data.Mouse.X = x;
			sendInputs[0].Data.Mouse.Y = y;
			SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
		}
	}

	public void MouseWheel(int vertical, int horizontal)
	{
		uint num = 0u;
		if (vertical != 0)
		{
			sendInputs[num].Type = 0u;
			sendInputs[num].Data.Mouse.ExtraInfo = IntPtr.Zero;
			sendInputs[num].Data.Mouse.Flags = 2048u;
			sendInputs[num].Data.Mouse.MouseData = (uint)vertical;
			sendInputs[num].Data.Mouse.Time = 0u;
			sendInputs[num].Data.Mouse.X = 0;
			sendInputs[num].Data.Mouse.Y = 0;
			num++;
		}
		if (horizontal != 0)
		{
			sendInputs[num].Type = 0u;
			sendInputs[num].Data.Mouse.ExtraInfo = IntPtr.Zero;
			sendInputs[num].Data.Mouse.Flags = 4096u;
			sendInputs[num].Data.Mouse.MouseData = (uint)horizontal;
			sendInputs[num].Data.Mouse.Time = 0u;
			sendInputs[num].Data.Mouse.X = 0;
			sendInputs[num].Data.Mouse.Y = 0;
			num++;
		}
		SendInput(num, sendInputs, (int)num * Marshal.SizeOf(sendInputs[0]));
	}

	public void MouseEvent(uint mouseButton)
	{
		sendInputs[0].Type = 0u;
		sendInputs[0].Data.Mouse.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Mouse.Flags = mouseButton;
		sendInputs[0].Data.Mouse.MouseData = 0u;
		sendInputs[0].Data.Mouse.Time = 0u;
		sendInputs[0].Data.Mouse.X = 0;
		sendInputs[0].Data.Mouse.Y = 0;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performLeftClick()
	{
		sendInputs[0].Type = 0u;
		sendInputs[0].Data.Mouse.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Mouse.Flags = 0u;
		sendInputs[0].Data.Mouse.Flags |= 6u;
		sendInputs[0].Data.Mouse.MouseData = 0u;
		sendInputs[0].Data.Mouse.Time = 0u;
		sendInputs[0].Data.Mouse.X = 0;
		sendInputs[0].Data.Mouse.Y = 0;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performRightClick()
	{
		rightClick = true;
		sendInputs[0].Type = 0u;
		sendInputs[0].Data.Mouse.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Mouse.Flags = 0u;
		sendInputs[0].Data.Mouse.Flags |= 24u;
		sendInputs[0].Data.Mouse.MouseData = 0u;
		sendInputs[0].Data.Mouse.Time = 0u;
		sendInputs[0].Data.Mouse.X = 0;
		sendInputs[0].Data.Mouse.Y = 0;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performMiddleClick()
	{
		sendInputs[0].Type = 0u;
		sendInputs[0].Data.Mouse.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Mouse.Flags = 0u;
		sendInputs[0].Data.Mouse.Flags |= 96u;
		sendInputs[0].Data.Mouse.MouseData = 0u;
		sendInputs[0].Data.Mouse.Time = 0u;
		sendInputs[0].Data.Mouse.X = 0;
		sendInputs[0].Data.Mouse.Y = 0;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void touchesMoved(object sender, TouchpadEventArgs arg)
	{
		if (!((DateTime.Now - pastTime).TotalMilliseconds > 150.0))
		{
			return;
		}
		if (arg.touches.Length == 1)
		{
			double num = 0.0;
			num = (GlobalVar.ButtonPresses.L1 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.L1) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.L1) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.L2 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.L2) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.L2) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.L3 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.L3) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.L3) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.R1 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.R1) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.R1) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.R2 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.R2) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.R2) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.R3 ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.R3) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.R3) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Dpad_UP ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Dpad_UP) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Dpad_UP) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Dpad_Down ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Dpad_Down) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Dpad_Down) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Dpad_Left ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Dpad_Left) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Dpad_Left) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Dpad_Right ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Dpad_Right) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Dpad_Right) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Triangle ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Triangle) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Triangle) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Square ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Square) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Square) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Circle ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Circle) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Circle) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.Cross ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Cross) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Cross) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : (GlobalVar.ButtonPresses.PS_Button ? ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.PS_Button) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.PS_Button) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0))) : ((!GlobalVar.ButtonPresses.Mic_Button) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensiButton == DSButtonsEnum.Mic_Button) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_SlowSensi / 100.0) : ((GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensiButton != DSButtonsEnum.Mic_Button) ? ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_NormalSensi / 100.0) : ((double)GlobalVar.Savefile.SaveFile_TouchpadToMouse_FastSensi / 100.0)))))))))))))))))));
			int x = (int)(num * (double)arg.touches[0].deltaX);
			int y = (int)(num * (double)arg.touches[0].deltaY);
			MoveCursorBy(x, y);
		}
		else if (arg.touches.Length == 2)
		{
			Touch previousTouch = arg.touches[0].previousTouch;
			Touch previousTouch2 = arg.touches[1].previousTouch;
			Touch touch = arg.touches[0];
			Touch touch2 = arg.touches[1];
			int num2 = (previousTouch.hwX + previousTouch2.hwX) / 2;
			int num3 = (previousTouch.hwY + previousTouch2.hwY) / 2;
			int num4 = (touch.hwX + touch2.hwX) / 2;
			int num5 = (touch.hwY + touch2.hwY) / 2;
			double num6 = GlobalVar.Savefile.SaveFile_TouchpadToMouse_ScrollSpeed;
			double num7 = touch2.hwX - touch.hwX;
			double num8 = touch2.hwY - touch.hwY;
			Math.Sqrt(num7 * num7 + num8 * num8);
			num6 *= 1.0;
			if (GlobalVar.Savefile.SaveFile_InvertScrolling)
			{
				MouseWheel((int)(num6 * (double)(num5 - num3)), (int)(num6 * (double)(num2 - num4)));
			}
			else
			{
				MouseWheel((int)(num6 * (double)(num3 - num5)), (int)(num6 * (double)(num4 - num2)));
			}
		}
	}

	public void touchesBegan(object sender, TouchpadEventArgs arg)
	{
		pastTime = DateTime.Now;
		touchpadTapPress = DateTime.Now;
		firstTouch = arg.touches[0];
	}

	public void touchesEnded(object sender, TouchpadEventArgs arg)
	{
		TimeSpan timeSpan = DateTime.Now - touchpadTapPress;
		if (GlobalVar.ButtonPresses.L1 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L1)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.L2 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L2)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.L3 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L3)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R1 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R1)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R2 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R2)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R3 && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R3)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_UP && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_UP)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Down && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Down)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Left && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Left)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Right && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Right)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Triangle && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Triangle)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Square && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Square)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Circle && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Circle)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Cross && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Cross)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.PS_Button && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.PS_Button)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Mic_Button && timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Mic_Button)
			{
				performRightClick();
			}
		}
		else if (timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			rightClick = false;
			MouseEvent(2u);
			MouseEvent(4u);
		}
		if (GlobalVar.ButtonPresses.L1)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
		}
		else if (GlobalVar.ButtonPresses.L2)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 1;
		}
		else if (GlobalVar.ButtonPresses.L3)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 2;
		}
		else if (GlobalVar.ButtonPresses.R1)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 3;
		}
		else if (GlobalVar.ButtonPresses.R2)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 4;
		}
		else if (GlobalVar.ButtonPresses.R3)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 5;
		}
		else if (GlobalVar.ButtonPresses.Dpad_UP)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 6;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Down)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 7;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Left)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 8;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Right)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 9;
		}
		else if (GlobalVar.ButtonPresses.Triangle)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 10;
		}
		else if (GlobalVar.ButtonPresses.Square)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 11;
		}
		else if (GlobalVar.ButtonPresses.Circle)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 12;
		}
		else if (GlobalVar.ButtonPresses.Cross)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 13;
		}
		else if (GlobalVar.ButtonPresses.PS_Button)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 14;
		}
		else if (GlobalVar.ButtonPresses.Mic_Button)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 16;
		}
		else
		{
			rightClick = false;
			MouseEvent(4u);
		}
	}

	public void touchButtonUp(object sender, TouchpadEventArgs arg)
	{
		TimeSpan timeSpan = DateTime.Now - touchpadButtonPress;
		if (GlobalVar.ButtonPresses.L1 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L1)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.L2 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L2)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.L3 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.L3)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R1 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R1)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R2 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R2)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.R3 && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.R3)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_UP && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_UP)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Down && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Down)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Left && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Left)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Dpad_Right && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Dpad_Right)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Triangle && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Triangle)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Square && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Square)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Circle && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Circle)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Cross && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Cross)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.PS_Button && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.PS_Button)
			{
				performRightClick();
			}
		}
		else if (GlobalVar.ButtonPresses.Mic_Button && timeSpan.TotalMilliseconds < 150.0)
		{
			if (GlobalVar.Savefile.SaveFile_TouchpadToMouse_RightClickButton == DSButtonsEnum.Mic_Button)
			{
				performRightClick();
			}
		}
		else if (timeSpan.TotalMilliseconds < 150.0 && !arg.touchButtonPressed)
		{
			rightClick = false;
			MouseEvent(2u);
			MouseEvent(4u);
		}
		if (GlobalVar.ButtonPresses.L1)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
		}
		else if (GlobalVar.ButtonPresses.L2)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 1;
		}
		else if (GlobalVar.ButtonPresses.L3)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 2;
		}
		else if (GlobalVar.ButtonPresses.R1)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 3;
		}
		else if (GlobalVar.ButtonPresses.R2)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 4;
		}
		else if (GlobalVar.ButtonPresses.R3)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 5;
		}
		else if (GlobalVar.ButtonPresses.Dpad_UP)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 6;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Down)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 7;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Left)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 8;
		}
		else if (GlobalVar.ButtonPresses.Dpad_Right)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 9;
		}
		else if (GlobalVar.ButtonPresses.Triangle)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 10;
		}
		else if (GlobalVar.ButtonPresses.Square)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 11;
		}
		else if (GlobalVar.ButtonPresses.Circle)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 12;
		}
		else if (GlobalVar.ButtonPresses.Cross)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 13;
		}
		else if (GlobalVar.ButtonPresses.PS_Button)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 14;
		}
		else if (GlobalVar.ButtonPresses.Mic_Button)
		{
			_ = GlobalVar.Savefile.SaveFile_TouchpadToMouse_LeftClickPressedButton;
			_ = 16;
		}
		else
		{
			rightClick = false;
			MouseEvent(4u);
		}
	}

	public void touchButtonDown(object sender, TouchpadEventArgs arg)
	{
		touchpadButtonPress = DateTime.Now;
		rightClick = false;
		MouseEvent(2u);
	}

	public void performSCKeyPress(ushort key)
	{
		sendInputs[0].Type = 1u;
		sendInputs[0].Data.Keyboard.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Keyboard.Flags = 8u;
		sendInputs[0].Data.Keyboard.Scan = MapVirtualKey(key, 0u);
		sendInputs[0].Data.Keyboard.Time = 0u;
		sendInputs[0].Data.Keyboard.Vk = key;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performKeyPress(ushort key)
	{
		sendInputs[0].Type = 1u;
		sendInputs[0].Data.Keyboard.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Keyboard.Flags = 0u;
		sendInputs[0].Data.Keyboard.Scan = 0;
		sendInputs[0].Data.Keyboard.Time = 0u;
		sendInputs[0].Data.Keyboard.Vk = key;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performSCKeyRelease(ushort key)
	{
		sendInputs[0].Type = 1u;
		sendInputs[0].Data.Keyboard.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Keyboard.Flags = 10u;
		sendInputs[0].Data.Keyboard.Scan = MapVirtualKey(key, 0u);
		sendInputs[0].Data.Keyboard.Time = 0u;
		sendInputs[0].Data.Keyboard.Vk = key;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	public void performKeyRelease(ushort key)
	{
		sendInputs[0].Type = 1u;
		sendInputs[0].Data.Keyboard.ExtraInfo = IntPtr.Zero;
		sendInputs[0].Data.Keyboard.Flags = 2u;
		sendInputs[0].Data.Keyboard.Scan = 0;
		sendInputs[0].Data.Keyboard.Time = 0u;
		sendInputs[0].Data.Keyboard.Vk = key;
		SendInput(1u, sendInputs, Marshal.SizeOf(sendInputs[0]));
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputs);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern ushort MapVirtualKey(uint uCode, uint uMapType);
}
