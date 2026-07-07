using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.IO;

namespace SharpCompress.Compressors.Xz;

[CLSCompliant(false)]
internal class XZIndex
{
	private readonly BinaryReader _reader;

	private bool _indexMarkerAlreadyVerified;

	public long StreamStartPosition { get; private set; }

	public ulong NumberOfRecords { get; private set; }

	public List<XZIndexRecord> Records { get; } = new List<XZIndexRecord>();

	public XZIndex(BinaryReader reader, bool indexMarkerAlreadyVerified)
	{
		_reader = reader;
		_indexMarkerAlreadyVerified = indexMarkerAlreadyVerified;
		StreamStartPosition = reader.BaseStream.Position;
		if (indexMarkerAlreadyVerified)
		{
			StreamStartPosition--;
		}
	}

	public static XZIndex FromStream(Stream stream, bool indexMarkerAlreadyVerified)
	{
		XZIndex xZIndex = new XZIndex(new BinaryReader(new NonDisposingStream(stream), Encoding.UTF8), indexMarkerAlreadyVerified);
		xZIndex.Process();
		return xZIndex;
	}

	public void Process()
	{
		if (!_indexMarkerAlreadyVerified)
		{
			VerifyIndexMarker();
		}
		NumberOfRecords = _reader.ReadXZInteger();
		for (ulong num = 0uL; num < NumberOfRecords; num++)
		{
			Records.Add(XZIndexRecord.FromBinaryReader(_reader));
		}
		SkipPadding();
		VerifyCrc32();
	}

	private void VerifyIndexMarker()
	{
		if (_reader.ReadByte() != 0)
		{
			throw new InvalidDataException("Not an index block");
		}
	}

	private void SkipPadding()
	{
		int num = (int)(_reader.BaseStream.Position - StreamStartPosition) % 4;
		if (num > 0 && _reader.ReadBytes(num).Any((byte b) => b != 0))
		{
			throw new InvalidDataException("Padding bytes were non-null");
		}
	}

	private void VerifyCrc32()
	{
		_reader.ReadLittleEndianUInt32();
	}
}
