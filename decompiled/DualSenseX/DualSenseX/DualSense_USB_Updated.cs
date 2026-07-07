using System;
using System.Windows.Media;
using ExtendInput.DataTools.DualSense;
using HidSharp;

namespace DualSenseX;

internal class DualSense_USB_Updated : DuelSense_Base_Updated
{
	public DualSense_USB_Updated(HidDevice _device)
		: base(_device)
	{
		_ = ReadBufferSize;
		_ = 64;
	}

	public override void RefreshButtonStatus(byte[] DeviceInputByteArray)
	{
	}

	public override void SetVibration(byte Left, byte Right)
	{
		LeftMotor = Left;
		RightMotor = Right;
		SendHaptics();
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
		SendHaptics();
	}

	public override void SetLedColor(byte R, byte G, byte B)
	{
		LedColor_R = R;
		LedColor_G = G;
		LedColor_B = B;
		SendHaptics();
	}

	public override void SetTouchpadLEDBrightness(double Percentage)
	{
		SendHaptics();
	}

	public override void SetMicLedStatus(MicLED Status)
	{
		MicLedStatus = Status;
		SendHaptics();
	}

	public override void SetPlayerLedStatus(PlayerLED Status, bool Checkbox1, bool Checkbox2, bool Checkbox3, bool Checkbox4, bool Checkbox5)
	{
		PlayerLedStatus = Status;
		checkbox1 = Checkbox1;
		checkbox2 = Checkbox2;
		checkbox3 = Checkbox3;
		checkbox4 = Checkbox4;
		checkbox5 = Checkbox5;
		SendHaptics();
	}

	public override void SetPlayerLedBrightness(PlayerLEDBrightness Status)
	{
		PlayerLedBrightness = Status;
		SendHaptics();
	}

	public override void CheckConnection()
	{
		SendHaptics();
	}

	private void SendHaptics()
	{
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
		byte[] bytes = BitConverter.GetBytes(LeftAdaptiveTrigger);
		byte[] bytes2 = BitConverter.GetBytes(RightAdaptiveTrigger);
		byte[] array = new byte[WriteBufferSize];
		array[0] = 2;
		if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Enable)
		{
			if (!GlobalVar.Savefile.SaveFile_DisableControllerVibration)
			{
				int num = 100;
				int num2 = -41;
				int num3 = 100;
				int num4 = -127;
				double num5 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_SpeakerVolume) * (double)num2 / (double)num + 41.0;
				double num6 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_HeadsetVolume) * (double)num4 / (double)num3;
				if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.InternalSpeaker)
				{
					array[1] = byte.MaxValue;
					array[2] = 215;
					array[3] = LeftMotor;
					array[4] = RightMotor;
					if ((byte)(41.0 - num5 + 61.0) == 61)
					{
						array[6] = 0;
					}
					else
					{
						array[6] = (byte)(41.0 - num5 + 61.0);
					}
					array[8] = 124;
				}
				else if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.Headset)
				{
					array[1] = byte.MaxValue;
					array[2] = 215;
					array[3] = LeftMotor;
					array[4] = RightMotor;
					array[5] = (byte)((byte)(0.0 - num6) + 30);
					array[8] = 76;
				}
				else
				{
					array[1] = byte.MaxValue;
					array[2] = 215;
					array[3] = LeftMotor;
					array[4] = RightMotor;
					array[5] = (byte)((byte)(0.0 - num6) + 30);
					if ((byte)(41.0 - num5 + 61.0) == 61)
					{
						array[6] = 0;
					}
					else
					{
						array[6] = (byte)(41.0 - num5 + 61.0);
					}
					array[8] = 108;
				}
				array[7] = Convert.ToByte(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_MicVolume);
				array[38] = (byte)((array[38] & 0xF8) | 5);
			}
			else if (GlobalVar.Savefile.SaveFile_EnableIncomingVibrationsWhileDisabled)
			{
				if (LeftMotor != 0 || RightMotor != 0)
				{
					int num7 = 100;
					int num8 = -41;
					int num9 = 100;
					int num10 = -127;
					double num11 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_SpeakerVolume) * (double)num8 / (double)num7 + 41.0;
					double num12 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_HeadsetVolume) * (double)num10 / (double)num9;
					if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.InternalSpeaker)
					{
						array[1] = byte.MaxValue;
						array[2] = 215;
						array[3] = LeftMotor;
						array[4] = RightMotor;
						if ((byte)(41.0 - num11 + 61.0) == 61)
						{
							array[6] = 0;
						}
						else
						{
							array[6] = (byte)(41.0 - num11 + 61.0);
						}
						array[8] = 124;
					}
					else if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.Headset)
					{
						array[1] = byte.MaxValue;
						array[2] = 215;
						array[3] = LeftMotor;
						array[4] = RightMotor;
						array[5] = (byte)((byte)(0.0 - num12) + 30);
						array[8] = 76;
					}
					else
					{
						array[1] = byte.MaxValue;
						array[2] = 215;
						array[3] = LeftMotor;
						array[4] = RightMotor;
						array[5] = (byte)((byte)(0.0 - num12) + 30);
						if ((byte)(41.0 - num11 + 61.0) == 61)
						{
							array[6] = 0;
						}
						else
						{
							array[6] = (byte)(41.0 - num11 + 61.0);
						}
						array[8] = 108;
					}
					array[7] = Convert.ToByte(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_MicVolume);
					array[38] = (byte)((array[38] & 0xF8) | 5);
				}
				else
				{
					int num13 = 100;
					int num14 = -41;
					int num15 = 100;
					int num16 = -127;
					double num17 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_SpeakerVolume) * (double)num14 / (double)num13 + 41.0;
					double num18 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_HeadsetVolume) * (double)num16 / (double)num15;
					if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.InternalSpeaker)
					{
						array[1] = 252;
						array[2] = 215;
						if ((byte)(41.0 - num17 + 61.0) == 61)
						{
							array[6] = 0;
						}
						else
						{
							array[6] = (byte)(41.0 - num17 + 61.0);
						}
						array[8] = 124;
					}
					else if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.Headset)
					{
						array[1] = 252;
						array[2] = 215;
						array[5] = (byte)((byte)(0.0 - num18) + 30);
						array[8] = 76;
					}
					else
					{
						array[1] = 252;
						array[2] = 215;
						array[5] = (byte)((byte)(0.0 - num18) + 30);
						if ((byte)(41.0 - num17 + 61.0) == 61)
						{
							array[6] = 0;
						}
						else
						{
							array[6] = (byte)(41.0 - num17 + 61.0);
						}
						array[8] = 108;
					}
					array[7] = Convert.ToByte(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_MicVolume);
					array[38] = (byte)((array[38] & 0xF8) | 5);
				}
			}
			else
			{
				int num19 = 100;
				int num20 = -41;
				int num21 = 100;
				int num22 = -127;
				double num23 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_SpeakerVolume) * (double)num20 / (double)num19 + 41.0;
				double num24 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_HeadsetVolume) * (double)num22 / (double)num21;
				if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.InternalSpeaker)
				{
					array[1] = 252;
					array[2] = 215;
					if ((byte)(41.0 - num23 + 61.0) == 61)
					{
						array[6] = 0;
					}
					else
					{
						array[6] = (byte)(41.0 - num23 + 61.0);
					}
					array[8] = 124;
				}
				else if (GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_Mode == AudioMode.Headset)
				{
					array[1] = 252;
					array[2] = 215;
					array[5] = (byte)((byte)(0.0 - num24) + 30);
					array[8] = 76;
				}
				else
				{
					array[1] = 252;
					array[2] = 215;
					array[5] = (byte)((byte)(0.0 - num24) + 30);
					if ((byte)(41.0 - num23 + 61.0) == 61)
					{
						array[6] = 0;
					}
					else
					{
						array[6] = (byte)(41.0 - num23 + 61.0);
					}
					array[8] = 108;
				}
				array[7] = Convert.ToByte(GlobalVar.Savefile.SaveFile_ControllerAudio.SaveFile_Audio_MicVolume);
				array[38] = (byte)((array[38] & 0xF8) | 5);
			}
		}
		else if (!GlobalVar.Savefile.SaveFile_DisableControllerVibration)
		{
			array[0] = 2;
			array[1] = 15;
			array[2] = 85;
			array[3] = LeftMotor;
			array[4] = RightMotor;
		}
		else if (GlobalVar.Savefile.SaveFile_EnableIncomingVibrationsWhileDisabled)
		{
			if (LeftMotor != 0 || RightMotor != 0)
			{
				array[0] = 2;
				array[1] = 15;
				array[2] = 85;
				array[3] = LeftMotor;
				array[4] = RightMotor;
			}
			else
			{
				array[0] = 2;
				array[1] = 12;
				array[2] = 21;
			}
		}
		else
		{
			array[0] = 2;
			array[1] = 12;
			array[2] = 21;
		}
		if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 0)
		{
			array[37] = 7;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 1)
		{
			array[37] = 6;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 2)
		{
			array[37] = 5;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 3)
		{
			array[37] = 4;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 4)
		{
			array[37] = 3;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 5)
		{
			array[37] = 2;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 6)
		{
			array[37] = 1;
		}
		else if (GlobalVar.Savefile.SaveFile_ControllerMotorStrength == 7)
		{
			array[37] = 0;
		}
		array[39] = 3;
		array[42] = 1;
		for (int i = 0; i < 7; i++)
		{
			array[11 + i] = bytes2[i];
			array[22 + i] = bytes[i];
		}
		array[20] = bytes2[7];
		array[31] = bytes[7];
		try
		{
			if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex != 999)
			{
				if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 9)
				{
					TriggerEffectGenerator.Resistance(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 11)
				{
					TriggerEffectGenerator.Bow(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 13)
				{
					TriggerEffectGenerator.Galloping(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 15)
				{
					TriggerEffectGenerator.SemiAutomaticGun(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 17)
				{
					TriggerEffectGenerator.AutomaticGun(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomRightTriggerValuesIndex == 19)
				{
					TriggerEffectGenerator.Machine(array, 11, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue7);
				}
				else
				{
					array[11] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue1;
					array[12] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue2;
					array[13] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue3;
					array[14] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue4;
					array[15] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue5;
					array[16] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue6;
					array[17] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue7;
					array[20] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomRightTriggerValuesIndex].TriggerValue8;
				}
			}
			if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex != 999)
			{
				if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 8)
				{
					TriggerEffectGenerator.Resistance(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 10)
				{
					TriggerEffectGenerator.Bow(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 12)
				{
					TriggerEffectGenerator.Galloping(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 14)
				{
					TriggerEffectGenerator.SemiAutomaticGun(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 16)
				{
					TriggerEffectGenerator.AutomaticGun(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4);
				}
				else if (DuelSense_Base_Updated.CustomLeftTriggerValuesIndex == 18)
				{
					TriggerEffectGenerator.Machine(array, 22, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6, GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue7);
				}
				else
				{
					array[22] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue1;
					array[23] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue2;
					array[24] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue3;
					array[25] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue4;
					array[26] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue5;
					array[27] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue6;
					array[28] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue7;
					array[31] = GlobalVar.TriggerModeList[DuelSense_Base_Updated.CustomLeftTriggerValuesIndex].TriggerValue8;
				}
			}
		}
		catch
		{
		}
		if (PlayerLedStatus == PlayerLED.ONE)
		{
			array[44] = 1;
		}
		else if (PlayerLedStatus == PlayerLED.TWO)
		{
			array[44] = 2;
		}
		else if (PlayerLedStatus == PlayerLED.THREE)
		{
			array[44] = 4;
		}
		else if (PlayerLedStatus == PlayerLED.FOUR)
		{
			array[44] = 8;
		}
		else if (PlayerLedStatus == PlayerLED.FIVE)
		{
			array[44] = 16;
		}
		else if (PlayerLedStatus == PlayerLED.AllOn)
		{
			array[44] = byte.MaxValue;
		}
		else if (PlayerLedStatus == PlayerLED.AllOff)
		{
			array[44] = 0;
		}
		else if (PlayerLedStatus == PlayerLED.Custom)
		{
			array[44] = (byte)((checkbox1 ? 1 : 0) | (checkbox2 ? 2 : 0) | (checkbox3 ? 4 : 0) | (checkbox4 ? 8 : 0) | (checkbox5 ? 16 : 0) | 0x20);
		}
		if (PlayerLedBrightness == PlayerLEDBrightness.LOW)
		{
			array[43] = 2;
		}
		else if (PlayerLedBrightness == PlayerLEDBrightness.MEDIUM)
		{
			array[43] = 1;
		}
		else if (PlayerLedBrightness == PlayerLEDBrightness.HIGH)
		{
			array[43] = 0;
		}
		if (MicLedStatus == MicLED.ON)
		{
			array[9] = 1;
		}
		else if (MicLedStatus == MicLED.PULSE)
		{
			array[9] = 2;
		}
		else if (MicLedStatus == MicLED.OFF)
		{
			array[9] = 0;
		}
		Color saveFile_TouchpadLEDColor = GlobalVar.Savefile.SaveFile_TouchpadLEDColor;
		double num25 = Convert.ToDouble(100) / 100.0;
		double num26 = Convert.ToDouble(GlobalVar.Savefile.SaveFile_TouchpadLEDBrightness) / 100.0;
		if (GlobalVar.IsAppLaunched)
		{
			array[45] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).R * num25);
			array[46] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).G * num25);
			array[47] = Convert.ToByte((double)(int)((Color)(ref saveFile_TouchpadLEDColor)).B * num25);
		}
		else
		{
			array[42] = 2;
			array[45] = Convert.ToByte((double)(int)LedColor_R * num26);
			array[46] = Convert.ToByte((double)(int)LedColor_G * num26);
			array[47] = Convert.ToByte((double)(int)LedColor_B * num26);
		}
		SendData(array);
	}
}
