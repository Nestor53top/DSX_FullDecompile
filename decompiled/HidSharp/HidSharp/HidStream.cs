using System;
using System.Runtime.InteropServices;

namespace HidSharp;

[ComVisible(true)]
[Guid("0C263D05-0D58-4c6c-AEA7-EB9E0C5338A2")]
public abstract class HidStream : DeviceStream
{
	public new HidDevice Device => (HidDevice)base.Device;

	protected HidStream(HidDevice device)
		: base(device)
	{
		ReadTimeout = 3000;
		WriteTimeout = 3000;
	}

	public override void Flush()
	{
	}

	public void GetFeature(byte[] buffer)
	{
		Throw.If.Null(buffer, "buffer");
		GetFeature(buffer, 0, buffer.Length);
	}

	public abstract void GetFeature(byte[] buffer, int offset, int count);

	public byte[] Read()
	{
		byte[] array = new byte[Device.GetMaxInputReportLength()];
		int newSize = Read(array);
		Array.Resize(ref array, newSize);
		return array;
	}

	public int Read(byte[] buffer)
	{
		Throw.If.Null(buffer, "buffer");
		return Read(buffer, 0, buffer.Length);
	}

	public void SetFeature(byte[] buffer)
	{
		Throw.If.Null(buffer, "buffer");
		SetFeature(buffer, 0, buffer.Length);
	}

	public abstract void SetFeature(byte[] buffer, int offset, int count);

	public void Write(byte[] buffer)
	{
		Throw.If.Null(buffer, "buffer");
		Write(buffer, 0, buffer.Length);
	}
}
