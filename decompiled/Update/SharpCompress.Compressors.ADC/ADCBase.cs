using System;
using System.IO;

namespace SharpCompress.Compressors.ADC;

internal static class ADCBase
{
	private const int Plain = 1;

	private const int TwoByte = 2;

	private const int ThreeByte = 3;

	private static int GetChunkType(byte byt)
	{
		if ((byt & 0x80) == 128)
		{
			return 1;
		}
		if ((byt & 0x40) == 64)
		{
			return 3;
		}
		return 2;
	}

	private static int GetChunkSize(byte byt)
	{
		return GetChunkType(byt) switch
		{
			1 => (byt & 0x7F) + 1, 
			2 => ((byt & 0x3F) >> 2) + 3, 
			3 => (byt & 0x3F) + 4, 
			_ => -1, 
		};
	}

	private static int GetOffset(byte[] chunk, int position)
	{
		return GetChunkType(chunk[position]) switch
		{
			1 => 0, 
			2 => ((chunk[position] & 3) << 8) + chunk[position + 1], 
			3 => (chunk[position + 1] << 8) + chunk[position + 2], 
			_ => -1, 
		};
	}

	public static int Decompress(byte[] input, out byte[] output, int bufferSize = 262144)
	{
		return Decompress(new MemoryStream(input), out output, bufferSize);
	}

	public static int Decompress(Stream input, out byte[] output, int bufferSize = 262144)
	{
		output = null;
		if (input == null || input.Length == 0L)
		{
			return 0;
		}
		int num = (int)input.Position;
		int num2 = (int)input.Position;
		byte[] array = new byte[bufferSize];
		int num3 = 0;
		bool flag = false;
		while (num2 < input.Length)
		{
			int num4 = input.ReadByte();
			if (num4 == -1)
			{
				break;
			}
			switch (GetChunkType((byte)num4))
			{
			case 1:
			{
				int chunkSize = GetChunkSize((byte)num4);
				if (num3 + chunkSize > bufferSize)
				{
					flag = true;
					break;
				}
				input.Read(array, num3, chunkSize);
				num3 += chunkSize;
				num2 += chunkSize + 1;
				break;
			}
			case 2:
			{
				MemoryStream memoryStream2 = new MemoryStream();
				int chunkSize = GetChunkSize((byte)num4);
				memoryStream2.WriteByte((byte)num4);
				memoryStream2.WriteByte((byte)input.ReadByte());
				int offset = GetOffset(memoryStream2.ToArray(), 0);
				if (num3 + chunkSize > bufferSize)
				{
					flag = true;
				}
				else if (offset == 0)
				{
					byte b2 = array[num3 - 1];
					for (int k = 0; k < chunkSize; k++)
					{
						array[num3] = b2;
						num3++;
					}
					num2 += 2;
				}
				else
				{
					for (int l = 0; l < chunkSize; l++)
					{
						array[num3] = array[num3 - offset - 1];
						num3++;
					}
					num2 += 2;
				}
				break;
			}
			case 3:
			{
				MemoryStream memoryStream = new MemoryStream();
				int chunkSize = GetChunkSize((byte)num4);
				memoryStream.WriteByte((byte)num4);
				memoryStream.WriteByte((byte)input.ReadByte());
				memoryStream.WriteByte((byte)input.ReadByte());
				int offset = GetOffset(memoryStream.ToArray(), 0);
				if (num3 + chunkSize > bufferSize)
				{
					flag = true;
				}
				else if (offset == 0)
				{
					byte b = array[num3 - 1];
					for (int i = 0; i < chunkSize; i++)
					{
						array[num3] = b;
						num3++;
					}
					num2 += 3;
				}
				else
				{
					for (int j = 0; j < chunkSize; j++)
					{
						array[num3] = array[num3 - offset - 1];
						num3++;
					}
					num2 += 3;
				}
				break;
			}
			}
			if (flag)
			{
				break;
			}
		}
		output = new byte[num3];
		Array.Copy(array, 0, output, 0, num3);
		return num2 - num;
	}
}
