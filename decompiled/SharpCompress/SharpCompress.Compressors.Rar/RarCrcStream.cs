using SharpCompress.Common;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Compressors.Rar;

internal class RarCrcStream : RarStream
{
	private readonly MultiVolumeReadOnlyStream readStream;

	private uint currentCrc;

	public RarCrcStream(Unpack unpack, FileHeader fileHeader, MultiVolumeReadOnlyStream readStream)
		: base(unpack, fileHeader, readStream)
	{
		this.readStream = readStream;
		ResetCrc();
	}

	public uint GetCrc()
	{
		return ~currentCrc;
	}

	public void ResetCrc()
	{
		currentCrc = uint.MaxValue;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = base.Read(buffer, offset, count);
		if (num != 0)
		{
			currentCrc = RarCRC.CheckCrc(currentCrc, buffer, offset, num);
		}
		else if (GetCrc() != readStream.CurrentCrc)
		{
			throw new InvalidFormatException("file crc mismatch");
		}
		return num;
	}
}
