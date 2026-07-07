using System.Windows.Media;

namespace DualSenseX;

public class DualSenseXSaveFile
{
	public PlayerLEDCustom SaveFile_PlayerLEDCustom = new PlayerLEDCustom();

	public CustomTrigger SaveFile_CustomTrigger = new CustomTrigger();

	public ResistanceTrigger SaveFile_ResistanceValues = new ResistanceTrigger();

	public BowTrigger SaveFile_BowValues = new BowTrigger();

	public GallopingTrigger SaveFile_GallopingValues = new GallopingTrigger();

	public SemiAutomaticGunTrigger SaveFile_SemiAutomaticGunValues = new SemiAutomaticGunTrigger();

	public AutomaticGunTrigger SaveFile_AutomaticGunValues = new AutomaticGunTrigger();

	public MachineTrigger SaveFile_MachineValues = new MachineTrigger();

	public ControllerAudio SaveFile_ControllerAudio = new ControllerAudio();

	public LastActiveTrigger SaveFile_LastActiveTrigger = new LastActiveTrigger();

	public bool SaveFile_IsSetupComplete { get; set; }

	public string SaveFile_Language { get; set; } = "English (United States)";

	public bool SaveFile_MinimizeWindowToTray { get; set; }

	public bool SaveFile_StartAppOnWindowsStartup { get; set; } = true;

	public bool SaveFile_StartAppOnWindowsStartupMinimized { get; set; }

	public EmulationType SaveFile_ControllerEmulation { get; set; } = EmulationType.Xbox360;

	public bool SaveFile_EnableHidHide { get; set; } = true;

	public bool SaveFile_InputPage_UseXboxLabels { get; set; }

	public ControllerImageColor SaveFile_InputPage_ControllerColor { get; set; }

	public bool SaveFile_InputPage_SyncButtonColors { get; set; } = true;

	public bool SaveFile_DarkMode { get; set; } = true;

	public bool SaveFile_Settings_BorderColor { get; set; }

	public int SaveFile_Settings_RedValue { get; set; }

	public int SaveFile_Settings_GreenValue { get; set; }

	public int SaveFile_Settings_BlueValue { get; set; }

	public int SaveFile_Settings_AlphaValue { get; set; } = 60;

	public bool SaveFile_AutoConnectOnStart { get; set; } = true;

	public bool SaveFile_AutoConnectSearchUntilConnected { get; set; } = true;

	public bool SaveFile_KillSteamOnControllerConnect { get; set; }

	public bool SaveFile_DisableControllerVibration { get; set; }

	public bool SaveFile_EnableIncomingVibrationsWhileDisabled { get; set; }

	public int SaveFile_ControllerMotorStrength { get; set; } = 7;

	public AppBackgroundImage SaveFile_AppBackgroundImage { get; set; } = AppBackgroundImage.Background9;

	public bool SaveFile_CustomBackgroundImageToggle { get; set; }

	public string SaveFile_CustomBackgroundImageDirectory { get; set; } = "Null";

	public Stretch SaveFile_CustomBackgroundImageStretch { get; set; }

	public int SaveFile_CustomBackgroundImageBlurRadius { get; set; }

	public TouchpadColorMode SaveFile_ControllerLEDColorMode { get; set; }

	public int SaveFile_TouchpadLEDColorCustomR { get; set; }

	public int SaveFile_TouchpadLEDColorCustomG { get; set; } = 255;

	public int SaveFile_TouchpadLEDColorCustomB { get; set; }

	public Color SaveFile_TouchpadLEDColor { get; set; } = Color.FromRgb((byte)0, (byte)0, byte.MaxValue);

	public int SaveFile_TouchpadLEDBrightness { get; set; } = 90;

	public RainbowSpeedMode SaveFile_RainbowSpeedMode { get; set; }

	public MicLED SaveFile_MicLED { get; set; } = MicLED.ON;

	public bool SaveFile_PlayerLed_FlashWhileCharging { get; set; } = true;

	public PlayerLED SaveFile_ControllerPLayerLED { get; set; } = PlayerLED.Custom;

	public PlayerLEDBrightness SaveFile_PlayerLEDBrightness { get; set; } = PlayerLEDBrightness.HIGH;

	public bool SaveFile_PlayLightshow { get; set; }

	public bool SaveFile_OpenXboxGameBar { get; set; } = true;

	public bool SaveFile_LoadLastActiveTrigger { get; set; } = true;

	public bool SaveFile_UDPListen { get; set; } = true;

	public int SaveFile_UDPPortNumber { get; set; } = 6969;

	public int SaveFile_LeftTriggerThreshold { get; set; }

	public int SaveFile_RightTriggerThreshold { get; set; }

	public bool SaveFile_EnableTouchpadToMouse { get; set; } = true;

	public bool SaveFile_InvertScrolling { get; set; }

	public int SaveFile_TouchpadToMouse_ScrollSpeed { get; set; } = 5;

	public int SaveFile_TouchpadToMouse_SlowSensi { get; set; } = 20;

	public int SaveFile_TouchpadToMouse_FastSensi { get; set; } = 200;

	public int SaveFile_TouchpadToMouse_NormalSensi { get; set; } = 90;

	public DSButtonsEnum SaveFile_TouchpadToMouse_SlowSensiButton { get; set; } = DSButtonsEnum.L2;

	public DSButtonsEnum SaveFile_TouchpadToMouse_FastSensiButton { get; set; } = DSButtonsEnum.R2;

	public DSButtonsEnum SaveFile_TouchpadToMouse_RightClickButton { get; set; } = DSButtonsEnum.R1;

	public DSButtonsEnum SaveFile_TouchpadToMouse_LeftClickPressedButton { get; set; }
}
