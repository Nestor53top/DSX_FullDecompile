using System;

namespace ExtendInput.DataTools.DualSense;

public class TriggerEffectGenerator
{
	public static bool SimpleResistance(byte[] destinationArray, int destinationIndex, byte start, byte force)
	{
		destinationArray[destinationIndex] = 1;
		destinationArray[destinationIndex + 1] = start;
		destinationArray[destinationIndex + 2] = force;
		destinationArray[destinationIndex + 3] = 0;
		destinationArray[destinationIndex + 4] = 0;
		destinationArray[destinationIndex + 5] = 0;
		destinationArray[destinationIndex + 6] = 0;
		destinationArray[destinationIndex + 7] = 0;
		destinationArray[destinationIndex + 8] = 0;
		destinationArray[destinationIndex + 9] = 0;
		destinationArray[destinationIndex + 10] = 0;
		return true;
	}

	public static bool SimpleSemiAutomaticGun(byte[] destinationArray, int destinationIndex, byte start, byte end, byte force)
	{
		destinationArray[destinationIndex] = 2;
		destinationArray[destinationIndex + 1] = start;
		destinationArray[destinationIndex + 2] = end;
		destinationArray[destinationIndex + 3] = force;
		destinationArray[destinationIndex + 4] = 0;
		destinationArray[destinationIndex + 5] = 0;
		destinationArray[destinationIndex + 6] = 0;
		destinationArray[destinationIndex + 7] = 0;
		destinationArray[destinationIndex + 8] = 0;
		destinationArray[destinationIndex + 9] = 0;
		destinationArray[destinationIndex + 10] = 0;
		return true;
	}

	public static bool Reset(byte[] destinationArray, int destinationIndex)
	{
		destinationArray[destinationIndex] = 5;
		destinationArray[destinationIndex + 1] = 0;
		destinationArray[destinationIndex + 2] = 0;
		destinationArray[destinationIndex + 3] = 0;
		destinationArray[destinationIndex + 4] = 0;
		destinationArray[destinationIndex + 5] = 0;
		destinationArray[destinationIndex + 6] = 0;
		destinationArray[destinationIndex + 7] = 0;
		destinationArray[destinationIndex + 8] = 0;
		destinationArray[destinationIndex + 9] = 0;
		destinationArray[destinationIndex + 10] = 0;
		return true;
	}

	public static bool SimpleAutomaticGun(byte[] destinationArray, int destinationIndex, byte start, byte strength, byte frequency)
	{
		if (frequency > 0 && strength > 0)
		{
			destinationArray[destinationIndex] = 6;
			destinationArray[destinationIndex + 1] = frequency;
			destinationArray[destinationIndex + 2] = strength;
			destinationArray[destinationIndex + 3] = start;
			destinationArray[destinationIndex + 4] = 0;
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool LimitedResistance(byte[] destinationArray, int destinationIndex, byte start, byte force)
	{
		if (force > 10)
		{
			return false;
		}
		if (force > 0)
		{
			destinationArray[destinationIndex] = 17;
			destinationArray[destinationIndex + 1] = start;
			destinationArray[destinationIndex + 2] = force;
			destinationArray[destinationIndex + 3] = 0;
			destinationArray[destinationIndex + 4] = 0;
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool LimitedSemiAutomaticGun(byte[] destinationArray, int destinationIndex, byte start, byte end, byte force)
	{
		if (start < 16)
		{
			return false;
		}
		if (end < start || start + 100 < end)
		{
			return false;
		}
		if (force > 10)
		{
			return false;
		}
		if (force > 0)
		{
			destinationArray[destinationIndex] = 18;
			destinationArray[destinationIndex + 1] = start;
			destinationArray[destinationIndex + 2] = end;
			destinationArray[destinationIndex + 3] = force;
			destinationArray[destinationIndex + 4] = 0;
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool Resistance(byte[] destinationArray, int destinationIndex, byte start, byte force)
	{
		if (start > 9)
		{
			return false;
		}
		if (force > 8)
		{
			return false;
		}
		if (force > 0)
		{
			byte b = (byte)((force - 1) & 7);
			uint num = 0u;
			ushort num2 = 0;
			for (int i = start; i < 10; i++)
			{
				num |= (uint)(b << 3 * i);
				num2 |= (ushort)(1 << i);
			}
			destinationArray[destinationIndex] = 33;
			destinationArray[destinationIndex + 1] = (byte)(num2 & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num2 >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 4] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 5] = (byte)((num >> 16) & 0xFF);
			destinationArray[destinationIndex + 6] = (byte)((num >> 24) & 0xFF);
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool Bow(byte[] destinationArray, int destinationIndex, byte start, byte end, byte force, byte snapForce)
	{
		if (start > 8)
		{
			return false;
		}
		if (end > 8)
		{
			return false;
		}
		if (start >= end)
		{
			return false;
		}
		if (force > 8)
		{
			return false;
		}
		if (snapForce > 8)
		{
			return false;
		}
		if (end > 0 && force > 0 && snapForce > 0)
		{
			ushort num = (ushort)((1 << (int)start) | (1 << (int)end));
			uint num2 = (uint)(((force - 1) & 7) | (((snapForce - 1) & 7) << 3));
			destinationArray[destinationIndex] = 34;
			destinationArray[destinationIndex + 1] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(num2 & 0xFF);
			destinationArray[destinationIndex + 4] = (byte)((num2 >> 8) & 0xFF);
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool Galloping(byte[] destinationArray, int destinationIndex, byte start, byte end, byte firstFoot, byte secondFoot, byte frequency)
	{
		if (start > 8)
		{
			return false;
		}
		if (end > 9)
		{
			return false;
		}
		if (start >= end)
		{
			return false;
		}
		if (secondFoot > 7)
		{
			return false;
		}
		if (firstFoot > 6)
		{
			return false;
		}
		if (firstFoot >= secondFoot)
		{
			return false;
		}
		if (frequency > 0)
		{
			ushort num = (ushort)((1 << (int)start) | (1 << (int)end));
			uint num2 = (uint)((secondFoot & 7) | ((firstFoot & 7) << 3));
			destinationArray[destinationIndex] = 35;
			destinationArray[destinationIndex + 1] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(num2 & 0xFF);
			destinationArray[destinationIndex + 4] = frequency;
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool SemiAutomaticGun(byte[] destinationArray, int destinationIndex, byte start, byte end, byte force)
	{
		if (start > 7 || start < 2)
		{
			return false;
		}
		if (end > 8)
		{
			return false;
		}
		if (end <= start)
		{
			return false;
		}
		if (force > 8)
		{
			return false;
		}
		if (force > 0)
		{
			ushort num = (ushort)((1 << (int)start) | (1 << (int)end));
			destinationArray[destinationIndex] = 37;
			destinationArray[destinationIndex + 1] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(force - 1);
			destinationArray[destinationIndex + 4] = 0;
			destinationArray[destinationIndex + 5] = 0;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool AutomaticGun(byte[] destinationArray, int destinationIndex, byte start, byte strength, byte frequency)
	{
		if (start > 9)
		{
			return false;
		}
		if (strength > 8)
		{
			return false;
		}
		if (strength > 0 && frequency > 0)
		{
			byte b = (byte)((strength - 1) & 7);
			uint num = 0u;
			ushort num2 = 0;
			for (int i = start; i < 10; i++)
			{
				num |= (uint)(b << 3 * i);
				num2 |= (ushort)(1 << i);
			}
			destinationArray[destinationIndex] = 38;
			destinationArray[destinationIndex + 1] = (byte)(num2 & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num2 >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 4] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 5] = (byte)((num >> 16) & 0xFF);
			destinationArray[destinationIndex + 6] = (byte)((num >> 24) & 0xFF);
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = frequency;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool Machine(byte[] destinationArray, int destinationIndex, byte start, byte end, byte strengthA, byte strengthB, byte frequency, byte period)
	{
		if (start > 8)
		{
			return false;
		}
		if (end > 9)
		{
			return false;
		}
		if (end <= start)
		{
			return false;
		}
		if (strengthA > 7)
		{
			return false;
		}
		if (strengthB > 7)
		{
			return false;
		}
		if (frequency > 0)
		{
			ushort num = (ushort)((1 << (int)start) | (1 << (int)end));
			uint num2 = (uint)((strengthA & 7) | ((strengthB & 7) << 3));
			destinationArray[destinationIndex] = 39;
			destinationArray[destinationIndex + 1] = (byte)(num & 0xFF);
			destinationArray[destinationIndex + 2] = (byte)((num >> 8) & 0xFF);
			destinationArray[destinationIndex + 3] = (byte)(num2 & 0xFF);
			destinationArray[destinationIndex + 4] = frequency;
			destinationArray[destinationIndex + 5] = period;
			destinationArray[destinationIndex + 6] = 0;
			destinationArray[destinationIndex + 7] = 0;
			destinationArray[destinationIndex + 8] = 0;
			destinationArray[destinationIndex + 9] = 0;
			destinationArray[destinationIndex + 10] = 0;
			return true;
		}
		return Reset(destinationArray, destinationIndex);
	}

	public static bool setModeFeedbackWithStartPosition(byte[] destinationArray, int destinationIndex, float startPosition, float resistiveStrength)
	{
		startPosition = (float)Math.Ceiling(startPosition * 9f);
		resistiveStrength = (float)Math.Ceiling(resistiveStrength * 8f);
		return Resistance(destinationArray, destinationIndex, (byte)startPosition, (byte)resistiveStrength);
	}

	public static bool setModeWeaponWithStartPosition(byte[] destinationArray, int destinationIndex, float startPosition, float endPosition, float resistiveStrength)
	{
		startPosition = (float)Math.Ceiling(Math.Min(Math.Max(startPosition * 9f, 2f), 7f));
		endPosition = (float)Math.Ceiling(Math.Min(Math.Max(endPosition * 9f, startPosition + 1f), 8f));
		resistiveStrength = (float)Math.Ceiling(resistiveStrength * 8f);
		return SemiAutomaticGun(destinationArray, destinationIndex, (byte)startPosition, (byte)endPosition, (byte)resistiveStrength);
	}

	public static bool setModeVibrationWithStartPosition(byte[] destinationArray, int destinationIndex, float startPosition, float amplitude, float frequency)
	{
		startPosition = (float)Math.Ceiling(startPosition * 9f);
		amplitude = (float)Math.Ceiling(amplitude * 8f);
		frequency = (float)Math.Ceiling(frequency * 255f);
		return AutomaticGun(destinationArray, destinationIndex, (byte)startPosition, (byte)amplitude, (byte)frequency);
	}
}
