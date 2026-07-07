using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace NamedPipeWrapper.IO;

public class PipeStreamReader<T> where T : class
{
	private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

	public PipeStream BaseStream { get; private set; }

	public bool IsConnected { get; private set; }

	public PipeStreamReader(PipeStream stream)
	{
		BaseStream = stream;
		IsConnected = stream.IsConnected;
	}

	private int ReadLength()
	{
		byte[] array = new byte[4];
		int num = BaseStream.Read(array, 0, 4);
		switch (num)
		{
		case 0:
			IsConnected = false;
			return 0;
		default:
			throw new IOException($"Expected {4} bytes but read {num}");
		case 4:
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(array, 0));
		}
	}

	private T ReadObject(int len)
	{
		byte[] buffer = new byte[len];
		BaseStream.Read(buffer, 0, len);
		using MemoryStream serializationStream = new MemoryStream(buffer);
		return (T)_binaryFormatter.Deserialize(serializationStream);
	}

	public T ReadObject()
	{
		int num = ReadLength();
		if (num != 0)
		{
			return ReadObject(num);
		}
		return null;
	}
}
