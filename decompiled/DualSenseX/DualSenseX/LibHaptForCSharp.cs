using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DualSenseX;

public class LibHaptForCSharp
{
	public const int SOUND = 0;

	public const int VIB = 1;

	public const int TYPE_NUM = 2;

	public const int SOUND_FLG = 1;

	public const int VIB_FLG = 2;

	public const int LEFT = 0;

	public const int RIGHT = 1;

	public const int CH_NUM = 2;

	public const int LEFT_FLG = 1;

	public const int RIGHT_FLG = 2;

	public const int FS = 48000;

	public const int BIT = 16;

	public const int FREQ_RANGE_MIN = 20;

	public const int FREQ_RANGE_MAX = 500;

	public const int UNKNOWN_ERROR = -1;

	public const int INVALIDE_FILE_ERROR = -2;

	public const int UNSUPPORTED_FILE_ERROR = -3;

	public const int CH_STEREO_TYPE = 1;

	public const int CH_QUAD_TYPE = 2;

	public const int READ_WAV_MAX_CH = 4;

	[DllImport("libVibrationDesigner")]
	public static extern int Init(int type);

	[DllImport("libVibrationDesigner")]
	public static extern int Close();

	[DllImport("libVibrationDesigner")]
	public static extern int DeleteFilter(ulong key);

	[DllImport("libVibrationDesigner")]
	public static extern int SetKeys(int type, int ch, IntPtr keys, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int ApplyFilter();

	[DllImport("libVibrationDesigner")]
	public static extern int CheckNewData();

	[DllImport("libVibrationDesigner")]
	public static extern int SetFilterRange(ulong key, double start, double end);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateLowPassFilter(ulong key, int cutoff);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateHighPassFilter(ulong key, int cutoff);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateNormalizeFilter(ulong key, double gain);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateFadeFilter(ulong key, bool isBeginning);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateImpactFilter(ulong key, double pulseNum, int freq, double ratio);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreatePitchShiftFilter(ulong key, int shiftTargetFrequency);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateEqualizer(ulong key, IntPtr hz, IntPtr gain, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int GetEqualizerCurveSize(ulong key);

	[DllImport("libVibrationDesigner")]
	public static extern int GetEqualizerCurve(ulong key, IntPtr x, IntPtr y, int size);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateAmlifyFilter(ulong key, double gain);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateDcOffsetCancelFilter();

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateClampFilter();

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateFlatteningFilter();

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateBexFilter(ulong key, float balance);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateDnnFilter(ulong key);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateSinPulse(ulong key, int frequency, double pulseNum, double amp);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateWhiteNoise(ulong key, int msTime, double amp, uint seed);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateWhiteNoiseFromEnvelope(ulong key, double amp, uint seed);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateChirp(ulong key, int startFrequency, int endFrequency, int msTime, double amp);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateChirpFromEnvelope(ulong key, int startFrequency, int endFrequency, double amp);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateSilence(ulong key, int msTime);

	[DllImport("libVibrationDesigner")]
	public static extern int ReadWaveFile(string file, IntPtr keys, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int GetWaveDataSize(ulong key);

	[DllImport("libVibrationDesigner")]
	public static extern int GetWaveData(ulong key, IntPtr data, int size);

	[DllImport("libVibrationDesigner")]
	public static extern ulong CreateRawDataHolder(IntPtr data, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int WriteData(int typeFlg, int chFlg, string file);

	[DllImport("libVibrationDesigner")]
	public static extern int GetMaxDataSize();

	[DllImport("libVibrationDesigner")]
	public static extern int GetDataSize(int type, int ch);

	[DllImport("libVibrationDesigner")]
	public static extern int GetData(int type, int ch, IntPtr data, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int GetSpectrumResultSize(int type, int ch);

	[DllImport("libVibrationDesigner")]
	public static extern int GetSpectrumResult(int type, int ch, IntPtr freq, IntPtr db, int size);

	[DllImport("libVibrationDesigner")]
	public static extern int GetMeta(int type, string tag, StringBuilder info, ulong size);

	[DllImport("libVibrationDesigner")]
	public static extern int SetMeta(int type, string tag, string info);

	[DllImport("libVibrationDesigner")]
	public static extern int SetName(int type, string fileName);

	[DllImport("libVibrationDesigner")]
	public static extern int GetName(int type, StringBuilder fileName, ulong size);

	[DllImport("libVibrationDesigner")]
	public static extern double ConvertToMsTime(int dataSize);

	[DllImport("libVibrationDesigner")]
	public static extern int ConvertToDataSize(double msTime);

	[DllImport("libVibrationDesigner")]
	public static extern int Play(double soundVol, double vibVol);

	[DllImport("libVibrationDesigner")]
	public static extern int Stop();

	[DllImport("libVibrationDesigner")]
	public static extern int SetVolume(double soundVol, double vibVol);

	[DllImport("libVibrationDesigner")]
	public static extern bool IsPlaying();

	[DllImport("libVibrationDesigner")]
	public static extern double GetCurrentPlayingPos();
}
