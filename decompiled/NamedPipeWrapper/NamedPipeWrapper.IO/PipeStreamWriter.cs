using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace NamedPipeWrapper.IO;

public class PipeStreamWriter<T> where T : class
{
	private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

	public PipeStream BaseStream { get; private set; }

	public PipeStreamWriter(PipeStream stream)
	{
		BaseStream = stream;
	}

	private byte[] Serialize(T obj)
	{
		using MemoryStream memoryStream = new MemoryStream();
		_binaryFormatter.Serialize(memoryStream, obj);
		return memoryStream.ToArray();
	}

	private void WriteLength(int len)
	{
		byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));
		BaseStream.Write(bytes, 0, bytes.Length);
	}

	private void WriteObject(byte[] data)
	{
		BaseStream.Write(data, 0, data.Length);
	}

	private void Flush()
	{
		BaseStream.Flush();
	}

	public void WriteObject(T obj)
	{
		byte[] array = Serialize(obj);
		WriteLength(array.Length);
		WriteObject(array);
		Flush();
	}

	public void WaitForPipeDrain()
	{
		BaseStream.WaitForPipeDrain();
	}
}
