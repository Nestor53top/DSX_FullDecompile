using System.IO;

namespace SharpCompress.Compressors.Deflate;

internal class SharedUtils
{
	public static int URShift(int number, int bits)
	{
		return number >>> bits;
	}

	public static int ReadInput(TextReader sourceTextReader, byte[] target, int start, int count)
	{
		if (target.Length == 0)
		{
			return 0;
		}
		char[] array = new char[target.Length];
		int num = sourceTextReader.Read(array, start, count);
		if (num == 0)
		{
			return -1;
		}
		for (int i = start; i < start + num; i++)
		{
			target[i] = (byte)array[i];
		}
		return num;
	}
}
