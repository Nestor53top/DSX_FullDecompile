using System;
using System.Linq;
using System.Threading.Tasks;
using Device.Net.Exceptions;

namespace Device.Net;

public class BulkIOService
{
	public IDevice Device { get; }

	public int ReadBufferSize { get; }

	public BulkIOService(IDevice device, int readBufferSize)
	{
		Device = device;
		ReadBufferSize = readBufferSize;
	}

	public async Task<int> ReadAsync(byte[] buff, int offset, int len)
	{
		if (buff == null)
		{
			throw new ArgumentNullException("buff");
		}
		if (buff.Length - offset < len)
		{
			throw new ValidationException("Index out of bounds");
		}
		int totalRead = 0;
		while (totalRead < len)
		{
			byte[] array = (await Device.ReadAsync()).Data;
			if (array.Length > len - totalRead)
			{
				array = array.Take(len - totalRead).ToArray();
			}
			for (int i = 0; i < array.Length; i++)
			{
				buff[totalRead + offset] = array[i];
				int num = totalRead + 1;
				totalRead = num;
			}
			if (array.Length < ReadBufferSize)
			{
				break;
			}
		}
		return totalRead;
	}

	public async Task WriteAsync(byte[] data, int offset, int len)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (data.Length - offset < len)
		{
			throw new ValidationException("Index out of bounds");
		}
		byte[] array;
		if (offset == 0 && len == data.Length)
		{
			array = data;
		}
		else
		{
			array = new byte[len];
			for (int i = 0; i < len; i++)
			{
				array[i] = data[offset + i];
			}
		}
		await Device.WriteAsync(array);
	}
}
