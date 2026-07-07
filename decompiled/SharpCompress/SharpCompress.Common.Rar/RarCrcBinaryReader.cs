using System.IO;
using SharpCompress.Compressors.Rar;
using SharpCompress.IO;

namespace SharpCompress.Common.Rar;

internal class RarCrcBinaryReader : MarkingBinaryReader
{
	private uint currentCrc;

	public RarCrcBinaryReader(Stream stream)
		: base(stream)
	{
	}

	public ushort GetCrc()
	{
		return (ushort)(~currentCrc);
	}

	public void ResetCrc()
	{
		currentCrc = uint.MaxValue;
	}

	protected void UpdateCrc(byte b)
	{
		currentCrc = RarCRC.CheckCrc(currentCrc, b);
	}

	protected byte[] ReadBytesNoCrc(int count)
	{
		return base.ReadBytes(count);
	}

	public override byte[] ReadBytes(int count)
	{
		byte[] array = base.ReadBytes(count);
		currentCrc = RarCRC.CheckCrc(currentCrc, array, 0, array.Length);
		return array;
	}
}
