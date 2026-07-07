using System.Collections.Generic;
using System.IO;

namespace SharpCompress.Common.Rar;

internal class RarCryptoBinaryReader : RarCrcBinaryReader
{
	private RarRijndael rijndael;

	private byte[] salt;

	private readonly string password;

	private readonly Queue<byte> data = new Queue<byte>();

	private long readCount;

	public override long CurrentReadByteCount
	{
		get
		{
			return readCount;
		}
		protected set
		{
		}
	}

	protected bool UseEncryption => salt != null;

	public RarCryptoBinaryReader(Stream stream, string password)
		: base(stream)
	{
		this.password = password;
	}

	public override void Mark()
	{
		readCount = 0L;
	}

	internal void InitializeAes(byte[] salt)
	{
		this.salt = salt;
		rijndael = RarRijndael.InitializeFrom(password, salt);
	}

	public override byte[] ReadBytes(int count)
	{
		if (UseEncryption)
		{
			return ReadAndDecryptBytes(count);
		}
		readCount += count;
		return base.ReadBytes(count);
	}

	private byte[] ReadAndDecryptBytes(int count)
	{
		int count2 = data.Count;
		int num = count - count2;
		if (num > 0)
		{
			int num2 = num + ((~num + 1) & 0xF);
			for (int i = 0; i < num2 / 16; i++)
			{
				byte[] cipherText = ReadBytesNoCrc(16);
				byte[] array = rijndael.ProcessBlock(cipherText);
				foreach (byte item in array)
				{
					data.Enqueue(item);
				}
			}
		}
		byte[] array2 = new byte[count];
		for (int k = 0; k < count; k++)
		{
			UpdateCrc(array2[k] = data.Dequeue());
		}
		readCount += count;
		return array2;
	}

	public void ClearQueue()
	{
		data.Clear();
	}

	public void SkipQueue()
	{
		long position = BaseStream.Position;
		BaseStream.Position = position + data.Count;
		ClearQueue();
	}
}
