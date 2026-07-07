using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.IO;

namespace SharpCompress.Compressors.Xz;

internal class XZFooter
{
	private readonly BinaryReader _reader;

	private readonly byte[] _magicBytes = new byte[2] { 89, 90 };

	public long StreamStartPosition { get; private set; }

	public long BackwardSize { get; private set; }

	public byte[] StreamFlags { get; private set; }

	public XZFooter(BinaryReader reader)
	{
		_reader = reader;
		StreamStartPosition = reader.BaseStream.Position;
	}

	public static XZFooter FromStream(Stream stream)
	{
		XZFooter xZFooter = new XZFooter(new BinaryReader(new NonDisposingStream(stream), Encoding.UTF8));
		xZFooter.Process();
		return xZFooter;
	}

	public void Process()
	{
		uint num = _reader.ReadLittleEndianUInt32();
		byte[] buffer = _reader.ReadBytes(6);
		uint num2 = Crc32.Compute(buffer);
		if (num != num2)
		{
			throw new InvalidDataException("Footer corrupt");
		}
		using (MemoryStream input = new MemoryStream(buffer))
		{
			using BinaryReader binaryReader = new BinaryReader(input);
			BackwardSize = (binaryReader.ReadLittleEndianUInt32() + 1) * 4;
			StreamFlags = binaryReader.ReadBytes(2);
		}
		if (!_reader.ReadBytes(2).SequenceEqual(_magicBytes))
		{
			throw new InvalidDataException("Magic footer missing");
		}
	}
}
