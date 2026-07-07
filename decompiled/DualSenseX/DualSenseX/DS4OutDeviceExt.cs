using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace DualSenseX;

internal class DS4OutDeviceExt(ViGEmClient client) : DS4OutDevice(client)
{
	private byte[] rawOutReportEx = new byte[63];

	private DS4_REPORT_EX outDS4Report;

	public unsafe override void ConvertandSendReport(DS4State state, int device)
	{
		if (connected)
		{
			ushort num = 0;
			DualShock4DPadDirection dualShock4DPadDirection = DualShock4DPadDirection.None;
			ushort num2 = 0;
			if (state.Share)
			{
				num |= DualShock4Button.Share.Value;
			}
			if (state.L3)
			{
				num |= DualShock4Button.ThumbLeft.Value;
			}
			if (state.R3)
			{
				num |= DualShock4Button.ThumbRight.Value;
			}
			if (state.Options)
			{
				num |= DualShock4Button.Options.Value;
			}
			if (state.DpadUp && state.DpadRight)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Northeast;
			}
			else if (state.DpadUp && state.DpadLeft)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Northwest;
			}
			else if (state.DpadUp)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.North;
			}
			else if (state.DpadRight && state.DpadDown)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Southeast;
			}
			else if (state.DpadRight)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.East;
			}
			else if (state.DpadDown && state.DpadLeft)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Southwest;
			}
			else if (state.DpadDown)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.South;
			}
			else if (state.DpadLeft)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.West;
			}
			if (state.L1)
			{
				num |= DualShock4Button.ShoulderLeft.Value;
			}
			if (state.R1)
			{
				num |= DualShock4Button.ShoulderRight.Value;
			}
			if (state.L2 > 0)
			{
				num |= DualShock4Button.TriggerLeft.Value;
			}
			if (state.R2 > 0)
			{
				num |= DualShock4Button.TriggerRight.Value;
			}
			if (state.Triangle)
			{
				num |= DualShock4Button.Triangle.Value;
			}
			if (state.Circle)
			{
				num |= DualShock4Button.Circle.Value;
			}
			if (state.Cross)
			{
				num |= DualShock4Button.Cross.Value;
			}
			if (state.Square)
			{
				num |= DualShock4Button.Square.Value;
			}
			if (state.PS)
			{
				num2 |= DualShock4SpecialButton.Ps.Value;
			}
			if (state.OutputTouchButton)
			{
				num2 |= DualShock4SpecialButton.Touchpad.Value;
			}
			outDS4Report.wButtons = num;
			outDS4Report.bSpecial = (byte)num2;
			outDS4Report.wButtons |= dualShock4DPadDirection.Value;
			outDS4Report.bTriggerL = state.L2;
			outDS4Report.bTriggerR = state.R2;
			outDS4Report.bTouchPacketsN = 1;
			outDS4Report.sCurrentTouch.bPacketCounter = state.TouchPacketCounter;
			outDS4Report.sCurrentTouch.bIsUpTrackingNum1 = state.TrackPadTouch0.RawTrackingNum;
			ref byte bTouchData = ref outDS4Report.sCurrentTouch.bTouchData1[0];
			bTouchData = (byte)(state.TrackPadTouch0.X & 0xFF);
			outDS4Report.sCurrentTouch.bTouchData1[1] = (byte)(((state.TrackPadTouch0.X >> 8) & 0xF) | ((state.TrackPadTouch0.Y << 4) & 0xF0));
			outDS4Report.sCurrentTouch.bTouchData1[2] = (byte)(state.TrackPadTouch0.Y >> 4);
			outDS4Report.sCurrentTouch.bIsUpTrackingNum2 = state.TrackPadTouch1.RawTrackingNum;
			ref byte bTouchData2 = ref outDS4Report.sCurrentTouch.bTouchData2[0];
			bTouchData2 = (byte)(state.TrackPadTouch1.X & 0xFF);
			outDS4Report.sCurrentTouch.bTouchData2[1] = (byte)(((state.TrackPadTouch1.X >> 8) & 0xF) | ((state.TrackPadTouch1.Y << 4) & 0xF0));
			outDS4Report.sCurrentTouch.bTouchData2[2] = (byte)(state.TrackPadTouch1.Y >> 4);
			outDS4Report.wGyroX = (short)state.Motion.gyroPitchFull;
			outDS4Report.wGyroY = (short)(-state.Motion.gyroYawFull);
			outDS4Report.wGyroZ = (short)(-state.Motion.gyroRollFull);
			outDS4Report.wAccelX = (short)(-state.Motion.accelXFull);
			outDS4Report.wAccelY = (short)(-state.Motion.accelYFull);
			outDS4Report.wAccelZ = (short)state.Motion.accelZFull;
			outDS4Report.bBatteryLvlSpecial = (byte)(state.Battery / 11);
			outDS4Report.wTimestamp = state.ds4Timestamp;
			DS4OutDeviceExtras.CopyBytes(ref outDS4Report, rawOutReportEx);
			cont.SubmitRawReport(rawOutReportEx);
		}
	}

	public override void ResetState(bool submit = true)
	{
		outDS4Report = default(DS4_REPORT_EX);
		outDS4Report.wButtons &= 65520;
		outDS4Report.wButtons |= 8;
		outDS4Report.bThumbLX = 128;
		outDS4Report.bThumbLY = 128;
		outDS4Report.bThumbRX = 128;
		outDS4Report.bThumbRY = 128;
		DS4OutDeviceExtras.CopyBytes(ref outDS4Report, rawOutReportEx);
		if (submit)
		{
			cont.SubmitRawReport(rawOutReportEx);
		}
	}
}
