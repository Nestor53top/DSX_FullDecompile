using System;

namespace SharpCompress.Converters;

internal abstract class DataConverter
{
	private class CopyConverter : DataConverter
	{
		public unsafe override double GetDouble(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			double result = default(double);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override ulong GetUInt64(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			ulong result = default(ulong);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override long GetInt64(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			long result = default(long);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override float GetFloat(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			float result = default(float);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override int GetInt32(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			int result = default(int);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override uint GetUInt32(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			uint result = default(uint);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override short GetInt16(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 2)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			short result = default(short);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 2; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override ushort GetUInt16(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 2)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			ushort result = default(ushort);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 2; i++)
			{
				ptr[i] = data[index + i];
			}
			return result;
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, double value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				long* ptr2 = (long*)(&value);
				*(long*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, float value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				uint* ptr2 = (uint*)(&value);
				*(uint*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, int value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				uint* ptr2 = (uint*)(&value);
				*(uint*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, uint value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				uint* ptr2 = &value;
				*(uint*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, long value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				long* ptr2 = &value;
				*(long*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, ulong value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				ulong* ptr2 = &value;
				*(ulong*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, short value)
		{
			Check(dest, destIdx, 2);
			fixed (byte* ptr = &dest[destIdx])
			{
				ushort* ptr2 = (ushort*)(&value);
				*(ushort*)ptr = *ptr2;
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, ushort value)
		{
			Check(dest, destIdx, 2);
			fixed (byte* ptr = &dest[destIdx])
			{
				ushort* ptr2 = &value;
				*(ushort*)ptr = *ptr2;
			}
		}
	}

	private class SwapConverter : DataConverter
	{
		public unsafe override double GetDouble(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			double result = default(double);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[7 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override ulong GetUInt64(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			ulong result = default(ulong);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[7 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override long GetInt64(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 8)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			long result = default(long);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 8; i++)
			{
				ptr[7 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override float GetFloat(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			float result = default(float);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[3 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override int GetInt32(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			int result = default(int);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[3 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override uint GetUInt32(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 4)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			uint result = default(uint);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 4; i++)
			{
				ptr[3 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override short GetInt16(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 2)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			short result = default(short);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 2; i++)
			{
				ptr[1 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override ushort GetUInt16(byte[] data, int index)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length - index < 2)
			{
				throw new ArgumentException("index");
			}
			if (index < 0)
			{
				throw new ArgumentException("index");
			}
			ushort result = default(ushort);
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < 2; i++)
			{
				ptr[1 - i] = data[index + i];
			}
			return result;
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, double value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = ptr2[7 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, float value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = ptr2[3 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, int value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = ptr2[3 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, uint value)
		{
			Check(dest, destIdx, 4);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = ptr2[3 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, long value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = ptr2[7 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, ulong value)
		{
			Check(dest, destIdx, 8);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = ptr2[7 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, short value)
		{
			Check(dest, destIdx, 2);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 2; i++)
				{
					ptr[i] = ptr2[1 - i];
				}
			}
		}

		public unsafe override void PutBytes(byte[] dest, int destIdx, ushort value)
		{
			Check(dest, destIdx, 2);
			fixed (byte* ptr = &dest[destIdx])
			{
				byte* ptr2 = (byte*)(&value);
				for (int i = 0; i < 2; i++)
				{
					ptr[i] = ptr2[1 - i];
				}
			}
		}
	}

	private static readonly DataConverter SwapConv = new SwapConverter();

	public static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

	public static DataConverter LittleEndian
	{
		get
		{
			if (!BitConverter.IsLittleEndian)
			{
				return SwapConv;
			}
			return Native;
		}
	}

	public static DataConverter BigEndian
	{
		get
		{
			if (!BitConverter.IsLittleEndian)
			{
				return Native;
			}
			return SwapConv;
		}
	}

	public static DataConverter Native { get; } = new CopyConverter();

	public abstract double GetDouble(byte[] data, int index);

	public abstract float GetFloat(byte[] data, int index);

	public abstract long GetInt64(byte[] data, int index);

	public abstract int GetInt32(byte[] data, int index);

	public abstract short GetInt16(byte[] data, int index);

	[CLSCompliant(false)]
	public abstract uint GetUInt32(byte[] data, int index);

	[CLSCompliant(false)]
	public abstract ushort GetUInt16(byte[] data, int index);

	[CLSCompliant(false)]
	public abstract ulong GetUInt64(byte[] data, int index);

	public abstract void PutBytes(byte[] dest, int destIdx, double value);

	public abstract void PutBytes(byte[] dest, int destIdx, float value);

	public abstract void PutBytes(byte[] dest, int destIdx, int value);

	public abstract void PutBytes(byte[] dest, int destIdx, long value);

	public abstract void PutBytes(byte[] dest, int destIdx, short value);

	[CLSCompliant(false)]
	public abstract void PutBytes(byte[] dest, int destIdx, ushort value);

	[CLSCompliant(false)]
	public abstract void PutBytes(byte[] dest, int destIdx, uint value);

	[CLSCompliant(false)]
	public abstract void PutBytes(byte[] dest, int destIdx, ulong value);

	public byte[] GetBytes(double value)
	{
		byte[] array = new byte[8];
		PutBytes(array, 0, value);
		return array;
	}

	public byte[] GetBytes(float value)
	{
		byte[] array = new byte[4];
		PutBytes(array, 0, value);
		return array;
	}

	public byte[] GetBytes(int value)
	{
		byte[] array = new byte[4];
		PutBytes(array, 0, value);
		return array;
	}

	public byte[] GetBytes(long value)
	{
		byte[] array = new byte[8];
		PutBytes(array, 0, value);
		return array;
	}

	public byte[] GetBytes(short value)
	{
		byte[] array = new byte[2];
		PutBytes(array, 0, value);
		return array;
	}

	[CLSCompliant(false)]
	public byte[] GetBytes(ushort value)
	{
		byte[] array = new byte[2];
		PutBytes(array, 0, value);
		return array;
	}

	[CLSCompliant(false)]
	public byte[] GetBytes(uint value)
	{
		byte[] array = new byte[4];
		PutBytes(array, 0, value);
		return array;
	}

	[CLSCompliant(false)]
	public byte[] GetBytes(ulong value)
	{
		byte[] array = new byte[8];
		PutBytes(array, 0, value);
		return array;
	}

	internal void Check(byte[] dest, int destIdx, int size)
	{
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		if (destIdx < 0 || destIdx > dest.Length - size)
		{
			throw new ArgumentException("destIdx");
		}
	}
}
