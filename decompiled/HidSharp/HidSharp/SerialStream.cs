using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HidSharp;

public abstract class SerialStream : DeviceStream
{
	private string _newLine;

	public abstract int BaudRate { get; set; }

	public abstract int DataBits { get; set; }

	public abstract SerialParity Parity { get; set; }

	public abstract int StopBits { get; set; }

	private Encoding Encoding => Encoding.UTF8;

	public string NewLine
	{
		get
		{
			return _newLine;
		}
		set
		{
			Throw.If.NullOrEmpty(value, "NewLine");
			_newLine = value;
		}
	}

	public new SerialDevice Device => (SerialDevice)base.Device;

	protected SerialStream(SerialDevice device)
		: base(device)
	{
		ReadTimeout = 3000;
		WriteTimeout = 3000;
		NewLine = "\r\n";
		BaudRate = SerialSettings.Default.BaudRate;
		DataBits = SerialSettings.Default.DataBits;
		Parity = SerialSettings.Default.Parity;
		StopBits = SerialSettings.Default.StopBits;
	}

	public string ReadTo(string ending)
	{
		Throw.If.NullOrEmpty(ending, "ending");
		List<byte> list = new List<byte>();
		byte[] bytes = Encoding.GetBytes(ending);
		int num = 0;
		while (true)
		{
			int num2 = ReadByte();
			if (num2 < 0)
			{
				break;
			}
			list.Add((byte)num2);
			if (num2 == bytes[num])
			{
				if (++num == bytes.Length)
				{
					break;
				}
			}
			else
			{
				num = 0;
			}
		}
		list.RemoveRange(list.Count - num, num);
		return Encoding.GetString(list.ToArray());
	}

	[Obfuscation(Exclude = true)]
	public string ReadLine()
	{
		return ReadTo(NewLine);
	}

	[Obfuscation(Exclude = true)]
	public void Write(string s)
	{
		Throw.If.Null(s, "s");
		byte[] bytes = Encoding.GetBytes(s);
		Write(bytes, 0, bytes.Length);
	}

	[Obfuscation(Exclude = true)]
	public void WriteLine(string s)
	{
		Throw.If.Null(s, "s");
		Write(s + NewLine);
	}
}
