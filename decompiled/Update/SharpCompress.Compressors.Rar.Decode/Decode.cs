namespace SharpCompress.Compressors.Rar.Decode;

internal class Decode
{
	internal int[] DecodeLen { get; }

	internal int[] DecodeNum { get; }

	internal int[] DecodePos { get; }

	internal int MaxNum { get; set; }

	internal Decode()
		: this(new int[2])
	{
	}

	protected Decode(int[] customDecodeNum)
	{
		DecodeLen = new int[16];
		DecodePos = new int[16];
		DecodeNum = customDecodeNum;
	}
}
