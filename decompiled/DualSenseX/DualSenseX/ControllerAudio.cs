namespace DualSenseX;

public class ControllerAudio
{
	public bool SaveFile_Audio_Enable { get; set; }

	public AudioMode SaveFile_Audio_Mode { get; set; }

	public int SaveFile_Audio_SpeakerVolume { get; set; } = 100;

	public int SaveFile_Audio_HeadsetVolume { get; set; } = 100;

	public int SaveFile_Audio_MicVolume { get; set; } = 64;

	public bool SaveFile_EnableAudioHaptics { get; set; }

	public int SaveFile_AudioHaptics_Delay { get; set; } = 10;

	public AudioHapticsSyncOptions SaveFile_AudioHaptics_SyncOptions { get; set; }

	public int SaveFile_AudioHaptics_LeftSpeakerVolume { get; set; } = 100;

	public int SaveFile_AudioHaptics_RightSpeakerVolume { get; set; } = 100;

	public int SaveFile_AudioHaptics_LeftMotorVolume { get; set; } = 100;

	public int SaveFile_AudioHaptics_RightMotorVolume { get; set; } = 100;
}
