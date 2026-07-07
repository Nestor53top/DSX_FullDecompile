using System.Drawing;

namespace DualSenseX;

public struct DualSenseGamepadState
{
	public bool isPlayingAudioHaptics;

	public float LeftMotor;

	public float RightMotor;

	public int MotorStrength;

	public Color LightBarColor;

	public float LightBarBrightness;

	public TriggerModes LeftTrigger;

	public TriggerModes RightTrigger;

	public CustomTriggerValueMode LeftCustomTriggerValueMode;

	public CustomTriggerValueMode RightCustomTriggerValueMode;

	public int[] LeftTriggerParams;

	public int[] RightTriggerParams;

	public bool PlayerLed_1;

	public bool PlayerLed_2;

	public bool PlayerLed_3;

	public bool PlayerLed_4;

	public bool PlayerLed_5;

	public PlayerLEDBrightness PlayerLedBrightness;

	public MicLED MicLedState;
}
