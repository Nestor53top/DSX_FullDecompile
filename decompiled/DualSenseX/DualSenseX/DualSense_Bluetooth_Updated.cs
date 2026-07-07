using System;
using System.Windows.Media;
using ExtendInput.DataTools.DualSense;
using HidSharp;
using WindowsInput;

namespace DualSenseX;

internal class DualSense_Bluetooth_Updated : DuelSense_Base_Updated
{
	private InputSimulator input = new InputSimulator();

	public int mouseSensitivity = 300;

	public int refreshRate = 60;

	public static float accumulatedX;

	public static float accumulatedY;

	private int reportOffset = 1;

	protected new const int TOUCHPAD_DATA_OFFSET = 33;

	public new int RESOLUTION_X_MAX = 1920;

	public new int Gyroscope { get; set; } = 12;

	public new int Accelerometer { get; set; } = 18;

	public DualSense_Bluetooth_Updated(HidDevice _device)
		: base(_device)
	{
		if (ReadBufferSize != 78)
		{
			throw new Exception("Unknown Read Buffer!");
		}
	}

	public override void RefreshButtonStatus(byte[] DeviceInputByteArray)
	{
		_ = DeviceInputByteArray[33 + reportOffset];
		_ = DeviceInputByteArray[33 + reportOffset];
		_ = DeviceInputByteArray[33 + reportOffset];
		_ = DeviceInputByteArray[35 + reportOffset];
		_ = DeviceInputByteArray[34 + reportOffset];
		_ = DeviceInputByteArray[36 + reportOffset];
		_ = DeviceInputByteArray[35 + reportOffset];
		_ = DeviceInputByteArray[37 + reportOffset];
		_ = DeviceInputByteArray[37 + reportOffset];
		_ = DeviceInputByteArray[37 + reportOffset];
		_ = DeviceInputByteArray[39 + reportOffset];
		_ = DeviceInputByteArray[38 + reportOffset];
		_ = DeviceInputByteArray[40 + reportOffset];
		_ = DeviceInputByteArray[39 + reportOffset];
		int num = 0;
		_ = DeviceInputByteArray[32 + reportOffset + num];
		_ = DeviceInputByteArray[33 + reportOffset + num] >> 7 == 0;
		_ = DeviceInputByteArray[33 + reportOffset + num];
		bool flag = DeviceInputByteArray[37 + reportOffset + num] >> 7 == 0;
		_ = DeviceInputByteArray[37 + reportOffset + num];
		_ = ((DeviceInputByteArray[35 + reportOffset + num] & 0xF) << 8) | DeviceInputByteArray[34 + reportOffset + num];
		_ = RESOLUTION_X_MAX * 2 / 5;
		_ = RESOLUTION_X_MAX * 2 / 5;
		byte b = DeviceInputByteArray[Gyroscope];
		byte b2 = DeviceInputByteArray[Gyroscope + 1];
		byte b3 = DeviceInputByteArray[Gyroscope + 2];
		byte b4 = DeviceInputByteArray[Gyroscope + 3];
		byte b5 = DeviceInputByteArray[Gyroscope + 4];
		byte num2 = DeviceInputByteArray[Gyroscope + 5];
		short num3 = (short)((ushort)(b2 << 8) | b);
		short num4 = (short)((ushort)(b4 << 8) | b3);
		_ = (short)((ushort)(num2 << 8) | b5);
		byte b6 = DeviceInputByteArray[Accelerometer];
		byte b7 = DeviceInputByteArray[Accelerometer + 1];
		byte b8 = DeviceInputByteArray[Accelerometer + 2];
		byte b9 = DeviceInputByteArray[Accelerometer + 3];
		byte b10 = DeviceInputByteArray[Accelerometer + 4];
		byte num5 = DeviceInputByteArray[Accelerometer + 5];
		short num6 = (short)((ushort)(b7 << 8) | b6);
		short num7 = (short)((ushort)(b9 << 8) | b8);
		short num8 = (short)((ushort)(num5 << 8) | b10);
		_ = (float)(-num6) / 8192f;
		_ = (float)(-num7) / 8192f;
		_ = (float)(-num8) / 8192f;
	}

	public override void SetVibration(byte Left, byte Right)
	{
		LeftMotor = Left;
		RightMotor = Right;
		SendHapticsAsync();
	}

	public override void SetAdaptiveTrigger(bool IsLeftTrigger, long Parameter)
	{
		if (IsLeftTrigger)
		{
			LeftAdaptiveTrigger = Parameter;
		}
		else
		{
			RightAdaptiveTrigger = Parameter;
		}
		SendHapticsAsync();
	}

	public override void SetLedColor(byte R, byte G, byte B)
	{
		LedColor_R = R;
		LedColor_G = G;
		LedColor_B = B;
		SendHapticsAsync();
	}

	public override void SetTouchpadLEDBrightness(double Percentage)
	{
		SendHapticsAsync();
	}

	public override void SetMicLedStatus(MicLED Status)
	{
		MicLedStatus = Status;
		SendHapticsAsync();
	}

	public override void SetPlayerLedStatus(PlayerLED Status, bool Checkbox1, bool Checkbox2, bool Checkbox3, bool Checkbox4, bool Checkbox5)
	{
		PlayerLedStatus = Status;
		checkbox1 = Checkbox1;
		checkbox2 = Checkbox2;
		checkbox3 = Checkbox3;
		checkbox4 = Checkbox4;
		checkbox5 = Checkbox5;
		SendHapticsAsync();
	}

	public override void SetPlayerLedBrightness(PlayerLEDBrightness Status)
	{
		PlayerLedBrightness = Status;
		SendHapticsAsync();
	}

	public override void CheckConnection()
	{
		SendHapticsAsync();
	}

	private void SendHapticsAsync()
	{
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		byte[] bytes = BitConverter.GetBytes(LeftAdaptiveTrigger);
		byte[] bytes2 = BitConverter.GetBytes(RightAdaptiveTrigger);
		byte[] array = new byte[WriteBufferSize];
		array[0] = 49;
		array[1] = 2;
		array[2] = 15;
		array[3] = 85;
		array[4] = RightMotor;
		array[5] = LeftMotor;
		if (GlobalVar.Savefile.SaveFile_DisableControllerVibration)
		{
			if (GlobalVar.Savefile.SaveFile_EnableIncomingVibrationsWhileDisabled)
			{
				if (LeftMotor != 0 || RightMotor != 0)
				{
					array[0] = 49;
					array[1] = 2;
					array[2] = 15;
					array[3] = 85;
					array[4] = RightMotor;
					array[5] = LeftMotor;
				}
				else
				{
					array[0] = 49;
					array[1] = 2;
					array[2] = 12;
					array[3] = 21;
				}
			}
			else
			{
				array[0] = 49;
				array[1] = 2;
				array[2] = 12;
				array[3] = 21;
			}
		}
		if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 0)
		{
			array[38] = 7;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 1)
		{
			array[38] = 6;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 2)
		{
			array[38] = 5;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 3)
		{
			array[38] = 4;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 4)
		{
			array[38] = 3;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 5)
		{
			array[38] = 2;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 6)
		{
			array[38] = 1;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 7)
		{
			array[38] = 0;
		}
		array[40] = 3;
		array[43] = 1;
		for (int i = 0; i < 7; i++)
		{
			array[12 + i] = bytes2[i];
			array[23 + i] = bytes[i];
		}
		array[21] = bytes2[7];
		array[32] = bytes[7];
		try
		{
			if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex != 999)
			{
				if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 9)
				{
					TriggerEffectGenerator.Resistance(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 11)
				{
					TriggerEffectGenerator.Bow(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 13)
				{
					TriggerEffectGenerator.Galloping(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 15)
				{
					TriggerEffectGenerator.SemiAutomaticGun(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 17)
				{
					TriggerEffectGenerator.AutomaticGun(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 19)
				{
					TriggerEffectGenerator.Machine(array, 12, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue7);
				}
				else
				{
					array[12] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue1;
					array[13] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2;
					array[14] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3;
					array[15] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4;
					array[16] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5;
					array[17] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6;
					array[18] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue7;
					array[21] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue8;
				}
			}
			if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex != 999)
			{
				if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 8)
				{
					TriggerEffectGenerator.Resistance(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 10)
				{
					TriggerEffectGenerator.Bow(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 12)
				{
					TriggerEffectGenerator.Galloping(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 14)
				{
					TriggerEffectGenerator.SemiAutomaticGun(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 16)
				{
					TriggerEffectGenerator.AutomaticGun(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 18)
				{
					TriggerEffectGenerator.Machine(array, 23, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue7);
				}
				else
				{
					array[23] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue1;
					array[24] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2;
					array[25] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3;
					array[26] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4;
					array[27] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5;
					array[28] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6;
					array[29] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue7;
					array[32] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue8;
				}
			}
		}
		catch
		{
		}
		if (PlayerLedStatus == PlayerLED.ONE)
		{
			array[45] = 1;
		}
		else if (PlayerLedStatus == PlayerLED.TWO)
		{
			array[45] = 2;
		}
		else if (PlayerLedStatus == PlayerLED.THREE)
		{
			array[45] = 4;
		}
		else if (PlayerLedStatus == PlayerLED.FOUR)
		{
			array[45] = 8;
		}
		else if (PlayerLedStatus == PlayerLED.FIVE)
		{
			array[45] = 16;
		}
		else if (PlayerLedStatus == PlayerLED.AllOn)
		{
			array[45] = byte.MaxValue;
		}
		else if (PlayerLedStatus == PlayerLED.AllOff)
		{
			array[45] = 0;
		}
		else if (PlayerLedStatus == PlayerLED.Custom)
		{
			array[45] = (byte)((checkbox1 ? 1 : 0) | (checkbox2 ? 2 : 0) | (checkbox3 ? 4 : 0) | (checkbox4 ? 8 : 0) | (checkbox5 ? 16 : 0) | 0x20);
		}
		if (PlayerLedBrightness == PlayerLEDBrightness.LOW)
		{
			array[44] = 2;
		}
		else if (PlayerLedBrightness == PlayerLEDBrightness.MEDIUM)
		{
			array[44] = 1;
		}
		else if (PlayerLedBrightness == PlayerLEDBrightness.HIGH)
		{
			array[44] = 0;
		}
		if (MicLedStatus == MicLED.ON)
		{
			array[10] = 1;
		}
		else if (MicLedStatus == MicLED.PULSE)
		{
			array[10] = 2;
		}
		else if (MicLedStatus == MicLED.OFF)
		{
			array[10] = 0;
		}
		Color saveFile_TouchpadLEDColor = GlobalVar.Savefile.SaveFile_TouchpadLEDColor;
		double num = Convert.ToDouble(100) / 100.0;
		double num2 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_TouchpadLEDBrightness) / 100.0;
		if (GlobalVar.IsAppLaunched)
		{
			array[46] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).R * num);
			array[47] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).G * num);
			array[48] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).B * num);
		}
		else if (GlobalVar.ControllerLostConnection)
		{
			array[46] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).R * num);
			array[47] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).G * num);
			array[48] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).B * num);
			array[43] = 2;
			array[46] = Convert.ToByte((double)(int)LedColor_R * num2);
			array[47] = Convert.ToByte((double)(int)LedColor_G * num2);
			array[48] = Convert.ToByte((double)(int)LedColor_B * num2);
		}
		else
		{
			array[43] = 2;
			array[46] = Convert.ToByte((double)(int)LedColor_R * num2);
			array[47] = Convert.ToByte((double)(int)LedColor_G * num2);
			array[48] = Convert.ToByte((double)(int)LedColor_B * num2);
			array[43] = 2;
		}
		Array.Copy(BitConverter.GetBytes(DuelSense_Base_Updated.ComputeCRC32(array, 74)), 0, array, 74, 4);
		SendData(array);
	}
}
