using System.IO.Pipes;

namespace NamedPipeWrapper.IO;

public class PipeStreamWrapper<TReadWrite> : PipeStreamWrapper<TReadWrite, TReadWrite> where TReadWrite : class
{
	public PipeStreamWrapper(PipeStream stream)
		: base(stream)
	{
	}
}
public class PipeStreamWrapper<TRead, TWrite> where TRead : class where TWrite : class
{
	private readonly PipeStreamReader<TRead> _reader;

	private readonly PipeStreamWriter<TWrite> _writer;

	public PipeStream BaseStream { get; private set; }

	public bool IsConnected
	{
		get
		{
			if (BaseStream.IsConnected)
			{
				return _reader.IsConnected;
			}
			return false;
		}
	}

	public bool CanRead => BaseStream.CanRead;

	public bool CanWrite => BaseStream.CanWrite;

	public PipeStreamWrapper(PipeStream stream)
	{
		BaseStream = stream;
		_reader = new PipeStreamReader<TRead>(BaseStream);
		_writer = new PipeStreamWriter<TWrite>(BaseStream);
	}

	public TRead ReadObject()
	{
		return _reader.ReadObject();
	}

	public void WriteObject(TWrite obj)
	{
		_writer.WriteObject(obj);
	}

	public void WaitForPipeDrain()
	{
		_writer.WaitForPipeDrain();
	}

	public void Close()
	{
		BaseStream.Close();
	}
}
