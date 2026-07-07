using System;

namespace HidSharp;

internal struct SerialSettings
{
	public static readonly SerialSettings Default = new SerialSettings
	{
		BaudRate = 9600,
		DataBits = 8,
		Parity = SerialParity.None,
		StopBits = 1
	};

	public int BaudRate;

	public int DataBits;

	public SerialParity Parity;

	public int StopBits;

	public void SetBaudRate(int baudRate, object @lock, ref bool settingsChanged)
	{
		if (baudRate < 0)
		{
			throw new NotSupportedException();
		}
		lock (@lock)
		{
			if (BaudRate != baudRate)
			{
				BaudRate = baudRate;
				settingsChanged = true;
			}
		}
	}

	public void SetDataBits(int dataBits, object @lock, ref bool settingsChanged)
	{
		if (dataBits < 7 || dataBits > 8)
		{
			throw new NotSupportedException();
		}
		lock (@lock)
		{
			if (DataBits != dataBits)
			{
				DataBits = dataBits;
				settingsChanged = true;
			}
		}
	}

	public void SetParity(SerialParity parity, object @lock, ref bool settingsChanged)
	{
		lock (@lock)
		{
			if (Parity != parity)
			{
				Parity = parity;
				settingsChanged = true;
			}
		}
	}

	public void SetStopBits(int stopBits, object @lock, ref bool settingsChanged)
	{
		if (stopBits < 1 || stopBits > 2)
		{
			throw new NotSupportedException();
		}
		lock (@lock)
		{
			if (StopBits != stopBits)
			{
				StopBits = stopBits;
				settingsChanged = true;
			}
		}
	}
}
