using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCompress.Compressors.LZMA;

internal class BitVector
{
	private readonly uint[] mBits;

	public int Length { get; }

	public bool this[int index]
	{
		get
		{
			if (index < 0 || index >= Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return (mBits[index >> 5] & (uint)(1 << index)) != 0;
		}
	}

	public BitVector(int length)
	{
		Length = length;
		mBits = new uint[length + 31 >> 5];
	}

	public BitVector(int length, bool initValue)
	{
		Length = length;
		mBits = new uint[length + 31 >> 5];
		if (initValue)
		{
			for (int i = 0; i < mBits.Length; i++)
			{
				mBits[i] = uint.MaxValue;
			}
		}
	}

	public BitVector(List<bool> bits)
		: this(bits.Count)
	{
		for (int i = 0; i < bits.Count; i++)
		{
			if (bits[i])
			{
				SetBit(i);
			}
		}
	}

	public bool[] ToArray()
	{
		bool[] array = new bool[Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this[i];
		}
		return array;
	}

	public void SetBit(int index)
	{
		if (index < 0 || index >= Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		mBits[index >> 5] |= (uint)(1 << index);
	}

	internal bool GetAndSet(int index)
	{
		if (index < 0 || index >= Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		uint num = mBits[index >> 5];
		uint num2 = (uint)(1 << index);
		mBits[index >> 5] |= num2;
		return (num & num2) != 0;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(Length);
		for (int i = 0; i < Length; i++)
		{
			stringBuilder.Append(this[i] ? 'x' : '.');
		}
		return stringBuilder.ToString();
	}
}
