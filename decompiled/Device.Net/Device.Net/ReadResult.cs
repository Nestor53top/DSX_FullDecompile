using System;

namespace Device.Net;

public struct ReadResult
{
	public byte[] Data { get; }

	public uint BytesRead { get; }

	public static implicit operator byte[](ReadResult readResult)
	{
		return readResult.Data;
	}

	public static implicit operator ReadResult(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		return new ReadResult(data, (uint)data.Length);
	}

	public ReadResult(byte[] data, uint bytesRead)
	{
		Data = data;
		BytesRead = bytesRead;
	}
}
