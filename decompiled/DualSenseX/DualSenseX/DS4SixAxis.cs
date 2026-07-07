using System;
using System.Diagnostics;

namespace DualSenseX;

public class DS4SixAxis
{
	private SixAxis sPrev;

	private SixAxis now;

	private CalibData[] calibrationData = new CalibData[6]
	{
		new CalibData(),
		new CalibData(),
		new CalibData(),
		new CalibData(),
		new CalibData(),
		new CalibData()
	};

	private bool calibrationDone;

	private const int num_gyro_average_windows = 3;

	private int gyro_average_window_front_index;

	private const int gyro_average_window_ms = 5000;

	private GyroAverageWindow[] gyro_average_window = new GyroAverageWindow[3];

	private int gyro_offset_x;

	private int gyro_offset_y;

	private int gyro_offset_z;

	private double gyro_accel_magnitude = 1.0;

	private Stopwatch gyroAverageTimer = new Stopwatch();

	private int temInt;

	public long CntCalibrating
	{
		get
		{
			if (!gyroAverageTimer.IsRunning)
			{
				return 0L;
			}
			return gyroAverageTimer.ElapsedMilliseconds;
		}
	}

	public event SixAxisHandler<SixAxisEventArgs> SixAccelMoved;

	public DS4SixAxis()
	{
		sPrev = new SixAxis(0, 0, 0, 0, 0, 0, 0.0);
		now = new SixAxis(0, 0, 0, 0, 0, 0, 0.0);
		StartContinuousCalibration();
	}

	public void setCalibrationData(ref byte[] calibData, bool useAltGyroCalib)
	{
		calibrationData[0].bias = (short)((ushort)(calibData[2] << 8) | calibData[1]);
		calibrationData[1].bias = (short)((ushort)(calibData[4] << 8) | calibData[3]);
		calibrationData[2].bias = (short)((ushort)(calibData[6] << 8) | calibData[5]);
		int num;
		int num2;
		int num3;
		int num4;
		int num5;
		int num6;
		if (!useAltGyroCalib)
		{
			num = (temInt = (short)((ushort)(calibData[8] << 8) | calibData[7]));
			num2 = (temInt = (short)((ushort)(calibData[10] << 8) | calibData[9]));
			num3 = (temInt = (short)((ushort)(calibData[12] << 8) | calibData[11]));
			num4 = (temInt = (short)((ushort)(calibData[14] << 8) | calibData[13]));
			num5 = (temInt = (short)((ushort)(calibData[16] << 8) | calibData[15]));
			num6 = (temInt = (short)((ushort)(calibData[18] << 8) | calibData[17]));
		}
		else
		{
			num = (temInt = (short)((ushort)(calibData[8] << 8) | calibData[7]));
			num4 = (temInt = (short)((ushort)(calibData[10] << 8) | calibData[9]));
			num2 = (temInt = (short)((ushort)(calibData[12] << 8) | calibData[11]));
			num5 = (temInt = (short)((ushort)(calibData[14] << 8) | calibData[13]));
			num3 = (temInt = (short)((ushort)(calibData[16] << 8) | calibData[15]));
			num6 = (temInt = (short)((ushort)(calibData[18] << 8) | calibData[17]));
		}
		int num7 = (temInt = (short)((ushort)(calibData[20] << 8) | calibData[19]));
		int num8 = (temInt = (short)((ushort)(calibData[22] << 8) | calibData[21]));
		int num9 = (temInt = (short)((ushort)(calibData[24] << 8) | calibData[23]));
		int num10 = (temInt = (short)((ushort)(calibData[26] << 8) | calibData[25]));
		int num11 = (temInt = (short)((ushort)(calibData[28] << 8) | calibData[27]));
		int num12 = (temInt = (short)((ushort)(calibData[30] << 8) | calibData[29]));
		int num13 = (temInt = (short)((ushort)(calibData[32] << 8) | calibData[31]));
		int num14 = (temInt = (short)((ushort)(calibData[34] << 8) | calibData[33]));
		int num15 = (temInt = num7 + num8);
		calibrationData[0].sensNumer = num15 * 16;
		calibrationData[0].sensDenom = num - num4;
		calibrationData[1].sensNumer = num15 * 16;
		calibrationData[1].sensDenom = num2 - num5;
		calibrationData[2].sensNumer = num15 * 16;
		calibrationData[2].sensDenom = num3 - num6;
		int num16 = (temInt = num9 - num10);
		calibrationData[3].bias = num9 - num16 / 2;
		calibrationData[3].sensNumer = 16384;
		calibrationData[3].sensDenom = num16;
		num16 = (temInt = num11 - num12);
		calibrationData[4].bias = num11 - num16 / 2;
		calibrationData[4].sensNumer = 16384;
		calibrationData[4].sensDenom = num16;
		num16 = (temInt = num13 - num14);
		calibrationData[5].bias = num13 - num16 / 2;
		calibrationData[5].sensNumer = 16384;
		calibrationData[5].sensDenom = num16;
		calibrationDone = calibrationData[0].sensDenom != 0 && calibrationData[1].sensDenom != 0 && calibrationData[2].sensDenom != 0 && num16 != 0;
	}

	private void applyCalibs(ref int yaw, ref int pitch, ref int roll, ref int accelX, ref int accelY, ref int accelZ)
	{
		CalibData calibData = calibrationData[0];
		temInt = pitch - calibData.bias;
		pitch = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
		calibData = calibrationData[1];
		temInt = yaw - calibData.bias;
		yaw = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
		calibData = calibrationData[2];
		temInt = roll - calibData.bias;
		roll = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
		calibData = calibrationData[3];
		temInt = accelX - calibData.bias;
		accelX = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
		calibData = calibrationData[4];
		temInt = accelY - calibData.bias;
		accelY = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
		calibData = calibrationData[5];
		temInt = accelZ - calibData.bias;
		accelZ = (temInt = (int)((float)temInt * ((float)calibData.sensNumer / (float)calibData.sensDenom)));
	}

	public unsafe void handleSixaxis(byte* gyro, byte* accel, DS4State state, double elapsedDelta)
	{
		int yaw = (short)((ushort)(gyro[3] << 8) | gyro[2]);
		int pitch = (short)((ushort)(gyro[1] << 8) | *gyro);
		int roll = (short)((ushort)(gyro[5] << 8) | gyro[4]);
		int accelX = (short)((ushort)(accel[1] << 8) | *accel);
		int accelY = (short)((ushort)(accel[3] << 8) | accel[2]);
		int accelZ = (short)((ushort)(accel[5] << 8) | accel[4]);
		if (calibrationDone)
		{
			applyCalibs(ref yaw, ref pitch, ref roll, ref accelX, ref accelY, ref accelZ);
		}
		if (gyroAverageTimer.IsRunning)
		{
			CalcSensorCamples(ref yaw, ref pitch, ref roll, ref accelX, ref accelY, ref accelZ);
		}
		yaw -= gyro_offset_x;
		pitch -= gyro_offset_y;
		roll -= gyro_offset_z;
		SixAxisEventArgs e = null;
		if ((accelX != 0 || accelY != 0 || accelZ != 0) && this.SixAccelMoved != null)
		{
			sPrev.copy(now);
			now.populate(yaw, pitch, roll, accelX, accelY, accelZ, elapsedDelta, sPrev);
			e = new SixAxisEventArgs(state.ReportTimeStamp, now);
			state.Motion = now;
			this.SixAccelMoved(this, e);
		}
	}

	public void PrepareNonDS4SixAxis(ref int currentYaw, ref int currentPitch, ref int currentRoll, ref int AccelX, ref int AccelY, ref int AccelZ)
	{
		if (gyroAverageTimer.IsRunning)
		{
			CalcSensorCamples(ref currentYaw, ref currentPitch, ref currentRoll, ref AccelX, ref AccelY, ref AccelZ);
		}
		currentYaw -= gyro_offset_x;
		currentPitch -= gyro_offset_y;
		currentRoll -= gyro_offset_z;
	}

	private void CalcSensorCamples(ref int currentYaw, ref int currentPitch, ref int currentRoll, ref int AccelX, ref int AccelY, ref int AccelZ)
	{
		double num = Math.Sqrt(AccelX * AccelX + AccelY * AccelY + AccelZ * AccelZ);
		PushSensorSamples(currentYaw, currentPitch, currentRoll, (float)num);
		if (gyroAverageTimer.ElapsedMilliseconds > 5000)
		{
			gyroAverageTimer.Stop();
			AverageGyro(ref gyro_offset_x, ref gyro_offset_y, ref gyro_offset_z, ref gyro_accel_magnitude);
		}
	}

	public bool fixupInvertedGyroAxis()
	{
		bool result = false;
		if (calibrationData[1].sensNumer > 0 && calibrationData[1].sensDenom < 0 && calibrationData[0].sensDenom > 0 && calibrationData[2].sensDenom > 0)
		{
			calibrationData[1].sensDenom *= -1;
			result = true;
		}
		return result;
	}

	public void FireSixAxisEvent(SixAxisEventArgs args)
	{
		this.SixAccelMoved?.Invoke(this, args);
	}

	public void StartContinuousCalibration()
	{
		for (int i = 0; i < gyro_average_window.Length; i++)
		{
			gyro_average_window[i] = new GyroAverageWindow();
		}
		gyroAverageTimer.Start();
	}

	public void StopContinuousCalibration()
	{
		gyroAverageTimer.Stop();
		gyroAverageTimer.Reset();
		for (int i = 0; i < gyro_average_window.Length; i++)
		{
			gyro_average_window[i].Reset();
		}
	}

	public void ResetContinuousCalibration()
	{
		StopContinuousCalibration();
		StartContinuousCalibration();
	}

	public void PushSensorSamples(int x, int y, int z, double accelMagnitude)
	{
		GyroAverageWindow gyroAverageWindow = gyro_average_window[gyro_average_window_front_index];
		if (gyroAverageWindow.StopIfElapsed(5000))
		{
			Console.WriteLine("GyroAvg[{0}], numSamples: {1}", gyro_average_window_front_index, gyroAverageWindow.numSamples);
			gyro_average_window_front_index = (gyro_average_window_front_index + 3 - 1) % 3;
			gyroAverageWindow = gyro_average_window[gyro_average_window_front_index];
			gyroAverageWindow.Reset();
		}
		gyroAverageWindow.numSamples++;
		gyroAverageWindow.x += x;
		gyroAverageWindow.y += y;
		gyroAverageWindow.z += z;
		gyroAverageWindow.accelMagnitude += accelMagnitude;
	}

	public void AverageGyro(ref int x, ref int y, ref int z, ref double accelMagnitude)
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		int num6 = 5000;
		for (int i = 0; i < 3; i++)
		{
			if (num6 <= 0)
			{
				break;
			}
			int num7 = (i + gyro_average_window_front_index) % 3;
			GyroAverageWindow gyroAverageWindow = gyro_average_window[num7];
			if (gyroAverageWindow.numSamples != 0 && gyroAverageWindow.DurationMs != 0)
			{
				double num8 = gyroAverageWindow.numSamples;
				double num9;
				if (num6 < gyroAverageWindow.DurationMs)
				{
					num9 = (float)num6 / (float)gyroAverageWindow.DurationMs;
					num6 = 0;
				}
				else
				{
					num9 = gyroAverageWindow.GetWeight(5000);
					num6 -= gyroAverageWindow.DurationMs;
				}
				num2 += (double)gyroAverageWindow.x / num8 * num9;
				num3 += (double)gyroAverageWindow.y / num8 * num9;
				num4 += (double)gyroAverageWindow.z / num8 * num9;
				num5 += gyroAverageWindow.accelMagnitude / num8 * num9;
				num += num9;
			}
		}
		if (num > 0.0)
		{
			x = (int)(num2 / num);
			y = (int)(num3 / num);
			z = (int)(num4 / num);
			accelMagnitude = num5 / num;
		}
	}
}
