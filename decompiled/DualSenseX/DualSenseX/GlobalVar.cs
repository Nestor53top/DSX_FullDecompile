using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows.Data;
using System.Windows.Media;
using HidSharp;
using LibraryUsb;
using Newtonsoft.Json;

namespace DualSenseX;

public class GlobalVar
{
	public enum TriggerModeTextFile
	{
		Normal,
		GameCube,
		VerySoft,
		Soft,
		Hard,
		VeryHard,
		Hardest,
		Rigid,
		VibrateTrigger,
		Choppy,
		Medium,
		VibrateTriggerPulse,
		CustomTriggerValue,
		Resistance,
		Bow,
		Galloping,
		SemiAutomaticGun,
		AutomaticGun,
		Machine
	}

	public enum TriggerModess
	{
		Normal,
		GameCube,
		VerySoft,
		Soft,
		Hard,
		VeryHard,
		Hardest,
		Rigid,
		VibrateTrigger,
		Choppy,
		Medium,
		VibrateTriggerPulse,
		CustomTriggerValue,
		Resistance,
		Bow,
		Galloping,
		SemiAutomaticGun,
		AutomaticGun,
		Machine
	}

	public enum OverlayPos
	{
		TopRight,
		TopLeft,
		Center,
		BottomRight,
		BottomLeft,
		TopCenter,
		BottomCenter,
		LeftCenter,
		RightCenter
	}

	public enum TriggerModeUDP
	{
		Normal,
		GameCube,
		VerySoft,
		Soft,
		Hard,
		VeryHard,
		Hardest,
		Rigid,
		VibrateTrigger,
		Choppy,
		Medium,
		VibrateTriggerPulse,
		CustomTriggerValue,
		Resistance,
		Bow,
		Galloping,
		SemiAutomaticGun,
		AutomaticGun,
		Machine
	}

	public enum CustomTriggerValueMode
	{
		OFF,
		Rigid,
		RigidA,
		RigidB,
		RigidAB,
		Pulse,
		PulseA,
		PulseB,
		PulseAB,
		VibrateResistance,
		VibrateResistanceA,
		VibrateResistanceB,
		VibrateResistanceAB,
		VibratePulse,
		VibratePulseA,
		VibratePulsB,
		VibratePulseAB
	}

	public enum Trigger
	{
		Invalid,
		Left,
		Right
	}

	public enum InstructionType
	{
		Invalid,
		TriggerUpdate,
		RGBUpdate,
		PlayerLED,
		TriggerThreshold
	}

	public struct Instruction
	{
		public InstructionType type;

		public object[] parameters;
	}

	public class Packet
	{
		public Instruction[] instructions;
	}

	public class CommunicationPacket
	{
		public string DataRecievedByServer;
	}

	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			if (value != null)
			{
				return (object)new SolidColorBrush((Color)value);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			object obj = ((value is SolidColorBrush) ? value : null);
			return (obj != null) ? new Color?(((SolidColorBrush)obj).Color) : ((Color?)null);
		}
	}

	public static int ReadBufferSize;

	public static float GyroX;

	public static float GyroY;

	public static float GyroZ;

	public static byte randByte1;

	public static byte randByte2;

	public static byte randByte3;

	public static byte randByte4;

	public static byte randByte5;

	public static byte randByte6;

	public static byte randByte7;

	public static byte randByte8;

	public static byte MotorStrenght;

	public static bool LoadedSaveFile;

	public static bool IsAppLaunched;

	public static bool EmulationChanged;

	public static bool IsControllerConnectedStatus;

	public static bool CheckIfMinimizedNoDrivers;

	public static bool DriverNotInstalled;

	public static bool ControllerLostConnection;

	public static bool ControllerLostConnection2;

	public static bool SettingsPage_AppPageBorder;

	public static bool SettingsPage_ControllerPageBorder;

	public static bool SettingsPage_BackgroundPageBorder;

	public static List<string> ListOfOutputDevices = new List<string>();

	public static bool DidDefaultDeviceChange;

	public static DSButtons ButtonPresses = new DSButtons();

	public static HidHideDevice vHidHideDevice = null;

	public static TriggerInfoWindow splashScreen = new TriggerInfoWindow();

	public static List<TriggerMode> TriggerModeList = new List<TriggerMode>();

	public static DualSenseXSaveFile Savefile = new DualSenseXSaveFile();

	public static OverlayPos overlayPosVar = OverlayPos.TopRight;

	public static int SaveFile_OverlayOffsetRight;

	public static int SaveFile_OverlayOffsetLeft;

	public static int SaveFile_OverlayOffsetUp;

	public static int SaveFile_OverlayOffsetDown;

	public static HidDevice DualSense;

	public static string DevicePath;

	public static IPAddress localhost = new IPAddress(new byte[4] { 127, 0, 0, 1 });

	public static string PacketToJson(CommunicationPacket packet)
	{
		return JsonConvert.SerializeObject(packet);
	}

	public static Packet JsonToPacket(string json)
	{
		return JsonConvert.DeserializeObject<Packet>(json);
	}
}
