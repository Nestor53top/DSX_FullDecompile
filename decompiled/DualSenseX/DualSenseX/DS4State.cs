using System;

namespace DualSenseX;

public class DS4State
{
	public struct TrackPadTouch
	{
		public bool IsActive;

		public byte Id;

		public short X;

		public short Y;

		public byte RawTrackingNum;
	}

	public uint PacketCounter;

	public DateTime ReportTimeStamp;

	public bool Square;

	public bool Triangle;

	public bool Circle;

	public bool Cross;

	public bool DpadUp;

	public bool DpadDown;

	public bool DpadLeft;

	public bool DpadRight;

	public bool L1;

	public bool L2Btn;

	public bool L3;

	public bool R1;

	public bool R2Btn;

	public bool R3;

	public bool Share;

	public bool Options;

	public bool PS;

	public bool Mute;

	public bool Touch1;

	public bool Touch2;

	public bool TouchButton;

	public bool TouchRight;

	public bool TouchLeft;

	public bool Touch1Finger;

	public bool Touch2Fingers;

	public bool OutputTouchButton;

	public byte Touch1Identifier;

	public byte Touch2Identifier;

	public byte LX;

	public byte RX;

	public byte LY;

	public byte RY;

	public byte L2;

	public byte R2;

	public byte FrameCounter;

	public byte TouchPacketCounter;

	public byte Battery;

	public double LSAngle;

	public double RSAngle;

	public double LSAngleRad;

	public double RSAngleRad;

	public double LXUnit;

	public double LYUnit;

	public double RXUnit;

	public double RYUnit;

	public double elapsedTime;

	public ulong totalMicroSec;

	public ushort ds4Timestamp;

	public SixAxis Motion;

	public static readonly int DEFAULT_AXISDIR_VALUE = 127;

	public int SASteeringWheelEmulationUnit;

	public TrackPadTouch TrackPadTouch0;

	public TrackPadTouch TrackPadTouch1;

	public DS4State()
	{
		PacketCounter = 0u;
		Square = (Triangle = (Circle = (Cross = false)));
		DpadUp = (DpadDown = (DpadLeft = (DpadRight = false)));
		L1 = (L2Btn = (L3 = (R1 = (R2Btn = (R3 = false)))));
		Share = (Options = (PS = (Mute = (Touch1 = (Touch2 = (TouchButton = (OutputTouchButton = (TouchRight = (TouchLeft = false)))))))));
		Touch1Finger = (Touch2Fingers = false);
		LX = (RX = (LY = (RY = 128)));
		L2 = (R2 = 0);
		FrameCounter = byte.MaxValue;
		TouchPacketCounter = byte.MaxValue;
		Battery = 0;
		LSAngle = 0.0;
		LSAngleRad = 0.0;
		RSAngle = 0.0;
		RSAngleRad = 0.0;
		LXUnit = 0.0;
		LYUnit = 0.0;
		RXUnit = 0.0;
		RYUnit = 0.0;
		elapsedTime = 0.0;
		totalMicroSec = 0uL;
		ds4Timestamp = 0;
		Motion = new SixAxis(0, 0, 0, 0, 0, 0, 0.0);
		TrackPadTouch0.IsActive = false;
		TrackPadTouch1.IsActive = false;
		SASteeringWheelEmulationUnit = 0;
	}

	public DS4State(DS4State state)
	{
		PacketCounter = state.PacketCounter;
		ReportTimeStamp = state.ReportTimeStamp;
		Square = state.Square;
		Triangle = state.Triangle;
		Circle = state.Circle;
		Cross = state.Cross;
		DpadUp = state.DpadUp;
		DpadDown = state.DpadDown;
		DpadLeft = state.DpadLeft;
		DpadRight = state.DpadRight;
		L1 = state.L1;
		L2 = state.L2;
		L2Btn = state.L2Btn;
		L3 = state.L3;
		R1 = state.R1;
		R2 = state.R2;
		R2Btn = state.R2Btn;
		R3 = state.R3;
		Share = state.Share;
		Options = state.Options;
		PS = state.PS;
		Mute = state.Mute;
		Touch1 = state.Touch1;
		TouchRight = state.TouchRight;
		TouchLeft = state.TouchLeft;
		Touch1Identifier = state.Touch1Identifier;
		Touch2 = state.Touch2;
		Touch2Identifier = state.Touch2Identifier;
		TouchButton = state.TouchButton;
		OutputTouchButton = state.OutputTouchButton;
		TouchPacketCounter = state.TouchPacketCounter;
		Touch1Finger = state.Touch1Finger;
		Touch2Fingers = state.Touch2Fingers;
		LX = state.LX;
		RX = state.RX;
		LY = state.LY;
		RY = state.RY;
		FrameCounter = state.FrameCounter;
		Battery = state.Battery;
		LSAngle = state.LSAngle;
		LSAngleRad = state.LSAngleRad;
		RSAngle = state.RSAngle;
		RSAngleRad = state.RSAngleRad;
		LXUnit = state.LXUnit;
		LYUnit = state.LYUnit;
		RXUnit = state.RXUnit;
		RYUnit = state.RYUnit;
		elapsedTime = state.elapsedTime;
		totalMicroSec = state.totalMicroSec;
		ds4Timestamp = state.ds4Timestamp;
		Motion = state.Motion;
		TrackPadTouch0 = state.TrackPadTouch0;
		TrackPadTouch1 = state.TrackPadTouch1;
		SASteeringWheelEmulationUnit = state.SASteeringWheelEmulationUnit;
	}

	public DS4State Clone()
	{
		return new DS4State(this);
	}

	public void CopyTo(DS4State state)
	{
		state.PacketCounter = PacketCounter;
		state.ReportTimeStamp = ReportTimeStamp;
		state.Square = Square;
		state.Triangle = Triangle;
		state.Circle = Circle;
		state.Cross = Cross;
		state.DpadUp = DpadUp;
		state.DpadDown = DpadDown;
		state.DpadLeft = DpadLeft;
		state.DpadRight = DpadRight;
		state.L1 = L1;
		state.L2 = L2;
		state.L2Btn = L2Btn;
		state.L3 = L3;
		state.R1 = R1;
		state.R2 = R2;
		state.R2Btn = R2Btn;
		state.R3 = R3;
		state.Share = Share;
		state.Options = Options;
		state.PS = PS;
		state.Mute = Mute;
		state.Touch1 = Touch1;
		state.Touch1Identifier = Touch1Identifier;
		state.Touch2 = Touch2;
		state.Touch2Identifier = Touch2Identifier;
		state.TouchLeft = TouchLeft;
		state.TouchRight = TouchRight;
		state.TouchButton = TouchButton;
		state.OutputTouchButton = OutputTouchButton;
		state.TouchPacketCounter = TouchPacketCounter;
		state.Touch1Finger = Touch1Finger;
		state.Touch2Fingers = Touch2Fingers;
		state.LX = LX;
		state.RX = RX;
		state.LY = LY;
		state.RY = RY;
		state.FrameCounter = FrameCounter;
		state.Battery = Battery;
		state.LSAngle = LSAngle;
		state.LSAngleRad = LSAngleRad;
		state.RSAngle = RSAngle;
		state.RSAngleRad = RSAngleRad;
		state.LXUnit = LXUnit;
		state.LYUnit = LYUnit;
		state.RXUnit = RXUnit;
		state.RYUnit = RYUnit;
		state.elapsedTime = elapsedTime;
		state.totalMicroSec = totalMicroSec;
		state.ds4Timestamp = ds4Timestamp;
		state.Motion = Motion;
		state.TrackPadTouch0 = TrackPadTouch0;
		state.TrackPadTouch1 = TrackPadTouch1;
		state.SASteeringWheelEmulationUnit = SASteeringWheelEmulationUnit;
	}

	public void calculateStickAngles()
	{
		double num = (LSAngleRad = Math.Atan2(-(LY - 128), LX - 128));
		num = ((num >= 0.0) ? num : (Math.PI * 2.0 + num)) * 180.0 / Math.PI;
		LSAngle = num;
		LXUnit = Math.Abs(Math.Cos(LSAngleRad));
		LYUnit = Math.Abs(Math.Sin(LSAngleRad));
		double num2 = (RSAngleRad = Math.Atan2(-(RY - 128), RX - 128));
		num2 = ((num2 >= 0.0) ? num2 : (Math.PI * 2.0 + num2)) * 180.0 / Math.PI;
		RSAngle = num2;
		RXUnit = Math.Abs(Math.Cos(RSAngleRad));
		RYUnit = Math.Abs(Math.Sin(RSAngleRad));
	}

	public void rotateLSCoordinates(double rotation)
	{
		double num = Math.Sin(rotation);
		double num2 = Math.Cos(rotation);
		double num3 = (double)(int)LX - 128.0;
		double num4 = (double)(int)LY - 128.0;
		LX = (byte)(Clamp(-128.0, num3 * num2 - num4 * num, 127.0) + 128.0);
		LY = (byte)(Clamp(-128.0, num3 * num + num4 * num2, 127.0) + 128.0);
	}

	public void rotateRSCoordinates(double rotation)
	{
		double num = Math.Sin(rotation);
		double num2 = Math.Cos(rotation);
		double num3 = (double)(int)RX - 128.0;
		double num4 = (double)(int)RY - 128.0;
		RX = (byte)(Clamp(-128.0, num3 * num2 - num4 * num, 127.0) + 128.0);
		RY = (byte)(Clamp(-128.0, num3 * num + num4 * num2, 127.0) + 128.0);
	}

	public static double Clamp(double min, double value, double max)
	{
		if (!(value < min))
		{
			if (!(value > max))
			{
				return value;
			}
			return max;
		}
		return min;
	}

	private static int ClampInt(int min, int value, int max)
	{
		if (value >= min)
		{
			if (value <= max)
			{
				return value;
			}
			return max;
		}
		return min;
	}
}
