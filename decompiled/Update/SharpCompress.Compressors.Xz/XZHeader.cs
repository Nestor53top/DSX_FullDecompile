using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.IO;

namespace SharpCompress.Compressors.Xz;

internal class XZHeader
{
	private readonly BinaryReader _reader;

	private readonly byte[] MagicHeader = new byte[6] { 253, 55, 122, 88, 90, 0 };

	public CheckType BlockCheckType { get; private set; }

	public int BlockCheckSize => (int)(BlockCheckType + 2) / 3 * 4;

	public XZHeader(BinaryReader reader)
	{
		_reader = reader;
	}

	public static XZHeader FromStream(Stream stream)
	{
		XZHeader xZHeader = new XZHeader(new BinaryReader(new NonDisposingStream(stream), Encoding.UTF8));
		xZHeader.Process();
		return xZHeader;
	}

	public void Process()
	{
		CheckMagicBytes(_reader.ReadBytes(6));
		ProcessStreamFlags();
	}

	private void ProcessStreamFlags()
	{
		byte[] array = _reader.ReadBytes(2);
		uint num = _reader.ReadLittleEndianUInt32();
		uint num2 = Crc32.Compute(array);
		if (num != num2)
		{
			throw new InvalidDataException("Stream header corrupt");
		}
		BlockCheckType = (CheckType)(array[1] & 0xF);
		if ((byte)(array[1] & 0xF0) != 0 || array[0] != 0)
		{
			throw new InvalidDataException("Unknown XZ Stream Version");
		}
	}

	private void CheckMagicBytes(byte[] header)
	{
		if (!header.SequenceEqual(MagicHeader))
		{
			throw new InvalidDataException("Invalid XZ Stream");
		}
	}
}
