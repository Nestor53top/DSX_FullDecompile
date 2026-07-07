using System;

namespace DualSenseX;

internal class Touchpad
{
	internal static int TOUCHPAD_DATA_OFFSET = 33;

	internal static int lastTouchPadX;

	internal static int lastTouchPadY;

	internal static int lastTouchPadX2;

	internal static int lastTouchPadY2;

	internal static bool lastTouchPadIsDown;

	internal static bool lastIsActive;

	internal static bool lastIsActive2;

	internal static byte lastTouchID;

	internal static byte lastTouchID2;

	internal static int ticks = -1;

	internal readonly int deviceNum;

	public event EventHandler<TouchpadEventArgs> TouchesBegan;

	public event EventHandler<TouchpadEventArgs> TouchesMoved;

	public event EventHandler<TouchpadEventArgs> TouchesEnded;

	public event EventHandler<TouchpadEventArgs> TouchButtonDown;

	public event EventHandler<TouchpadEventArgs> TouchButtonUp;

	public Touchpad(int controllerID)
	{
		deviceNum = controllerID;
	}

	public void handleTouchpad(byte[] DeviceInputByteArray, bool touchPadIsDown)
	{
		int num = ((DeviceInputByteArray.Length == 78) ? 1 : 0);
		int num2 = 1920;
		_ = DeviceInputByteArray[33 + num];
		byte b = (byte)(DeviceInputByteArray[33 + num] & 0x7F);
		_ = DeviceInputByteArray[33 + num];
		short num3 = (short)(((ushort)(DeviceInputByteArray[35 + num] & 0xF) << 8) | DeviceInputByteArray[34 + num]);
		short num4 = (short)((DeviceInputByteArray[36 + num] << 4) | ((ushort)(DeviceInputByteArray[35 + num] & 0xF0) >> 4));
		_ = DeviceInputByteArray[37 + num];
		byte b2 = (byte)(DeviceInputByteArray[37 + num] & 0x7F);
		_ = DeviceInputByteArray[37 + num];
		short num5 = (short)(((ushort)(DeviceInputByteArray[39 + num] & 0xF) << 8) | DeviceInputByteArray[38 + num]);
		short num6 = (short)((DeviceInputByteArray[40 + num] << 4) | ((ushort)(DeviceInputByteArray[39 + num] & 0xF0) >> 4));
		int num7 = 0;
		_ = DeviceInputByteArray[8 + TOUCHPAD_DATA_OFFSET + num + num7];
		bool num8 = DeviceInputByteArray[TOUCHPAD_DATA_OFFSET + num + num7] >> 7 == 0;
		_ = DeviceInputByteArray[TOUCHPAD_DATA_OFFSET + num + num7];
		bool flag = DeviceInputByteArray[4 + TOUCHPAD_DATA_OFFSET + num + num7] >> 7 == 0;
		_ = DeviceInputByteArray[4 + TOUCHPAD_DATA_OFFSET + num + num7];
		bool flag2 = num8 || flag;
		bool flag3 = num8 && flag;
		_ = ((DeviceInputByteArray[2 + TOUCHPAD_DATA_OFFSET + num + num7] & 0xF) << 8) | DeviceInputByteArray[1 + TOUCHPAD_DATA_OFFSET + num + num7];
		_ = num2 * 2 / 5;
		_ = num2 * 2 / 5;
		_ = DeviceInputByteArray[34];
		bool flag4 = flag2;
		bool flag5 = flag3;
		byte tID = b;
		byte tID2 = b2;
		int x = num3;
		int y = num4;
		int x2 = num5;
		int y2 = num6;
		if (flag4)
		{
			if (!lastTouchPadIsDown && touchPadIsDown && this.TouchButtonDown != null)
			{
				TouchpadEventArgs e = null;
				Touch t = new Touch(x, y, tID);
				if (flag5)
				{
					Touch t2 = new Touch(x2, y2, tID2);
					e = new TouchpadEventArgs(touchPadIsDown, t, t2);
				}
				else
				{
					e = new TouchpadEventArgs(touchPadIsDown, t);
				}
				this.TouchButtonDown(this, e);
			}
			else if (lastTouchPadIsDown && !touchPadIsDown && this.TouchButtonUp != null)
			{
				TouchpadEventArgs e2 = null;
				Touch t3 = new Touch(x, y, tID);
				if (flag5)
				{
					Touch t4 = new Touch(x2, y2, tID2);
					e2 = new TouchpadEventArgs(touchPadIsDown, t3, t4);
				}
				else
				{
					e2 = new TouchpadEventArgs(touchPadIsDown, t3);
				}
				this.TouchButtonUp(this, e2);
			}
			if (!lastIsActive || (flag5 && !lastIsActive2))
			{
				if (this.TouchesBegan != null)
				{
					TouchpadEventArgs e3 = null;
					Touch t5 = new Touch(x, y, tID);
					if (flag5 && !lastIsActive2)
					{
						Touch t6 = new Touch(x2, y2, tID2);
						e3 = new TouchpadEventArgs(touchPadIsDown, t5, t6);
					}
					else
					{
						e3 = new TouchpadEventArgs(touchPadIsDown, t5);
					}
					this.TouchesBegan(this, e3);
				}
			}
			else if (lastIsActive && this.TouchesMoved != null)
			{
				TouchpadEventArgs e4 = null;
				Touch prevTouch = new Touch(lastTouchPadX, lastTouchPadY, lastTouchID);
				Touch t7 = new Touch(x, y, tID, prevTouch);
				if (flag4 && flag5)
				{
					Touch prevTouch2 = new Touch(lastTouchPadX2, lastTouchPadY2, lastTouchID2);
					Touch t8 = new Touch(x2, y2, tID2, prevTouch2);
					e4 = new TouchpadEventArgs(touchPadIsDown, t7, t8);
				}
				else
				{
					e4 = new TouchpadEventArgs(touchPadIsDown, t7);
				}
				this.TouchesMoved(this, e4);
			}
			lastTouchPadX = x;
			lastTouchPadY = y;
			lastTouchPadX2 = x2;
			lastTouchPadY2 = y2;
			lastTouchPadIsDown = touchPadIsDown;
		}
		else
		{
			if (lastIsActive && this.TouchesEnded != null)
			{
				TouchpadEventArgs e5 = null;
				Touch t9 = new Touch(x, y, tID);
				if (lastIsActive2)
				{
					Touch t10 = new Touch(x2, y2, tID2);
					e5 = new TouchpadEventArgs(touchPadIsDown, t9, t10);
				}
				else
				{
					e5 = new TouchpadEventArgs(touchPadIsDown, t9);
				}
				this.TouchesEnded(this, e5);
			}
			if (touchPadIsDown && !lastTouchPadIsDown)
			{
				this.TouchButtonDown(this, new TouchpadEventArgs(touchPadIsDown, null));
			}
			else if (!touchPadIsDown && lastTouchPadIsDown)
			{
				this.TouchButtonUp(this, new TouchpadEventArgs(touchPadIsDown, null));
			}
		}
		lastIsActive = flag4;
		lastIsActive2 = flag5;
		lastTouchID = tID;
		lastTouchID2 = tID2;
		lastTouchPadIsDown = touchPadIsDown;
	}
}
